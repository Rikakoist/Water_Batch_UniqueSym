using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Analyst3D;
using ESRI.ArcGIS.ArcScene;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geodatabase;

namespace Water_Batch_UniqueSym
{
    /// <summary>
    /// Command that works in ArcScene or SceneControl
    /// </summary>
    [Guid("6a445039-2e0f-44bd-b89d-839e937307da")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("Water_Batch_UniqueSym.Surface_Offset")]
    public sealed class Surface_Offset : BaseCommand
    {
        #region COM Registration Function(s)
        [ComRegisterFunction()]
        [ComVisible(false)]
        static void RegisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryRegistration(registerType);

            //
            // TODO: Add any COM registration code here
            //
        }

        [ComUnregisterFunction()]
        [ComVisible(false)]
        static void UnregisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryUnregistration(registerType);

            //
            // TODO: Add any COM unregistration code here
            //
        }

        #region ArcGIS Component Category Registrar generated code
        /// <summary>
        /// Required method for ArcGIS Component Category registration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryRegistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            SxCommands.Register(regKey);
            ControlsCommands.Register(regKey);
        }
        /// <summary>
        /// Required method for ArcGIS Component Category unregistration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryUnregistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            SxCommands.Unregister(regKey);
            ControlsCommands.Unregister(regKey);
        }

        #endregion
        #endregion

        private ISceneHookHelper m_sceneHookHelper = null;
        IApplication m_application;
        IActiveView activeView;
        IScene m_scene = null;
        List<int> SelectedLyrIndex;

        public Surface_Offset()
        {
            //
            // TODO: Define values for the public properties
            //
            base.m_category = "SRTP"; //localizable text
            base.m_caption = "洪水自定义表面及偏移调整v2.1(20200212)。";  //localizable text
            base.m_message = "将所有用户选定的栅格图层在用户选择的自定义表面上浮动，并批量设置图层偏移。";  //localizable text 
            base.m_toolTip = "洪水自定义表面及偏移调整v2.1(20200212)。";  //localizable text 
            base.m_name = "Surface_Offset";   //unique id, non-localizable (e.g. "MyCategory_ArcMapCommand")

            try
            {
                //
                // TODO: change bitmap name if necessary
                //
                string bitmapResourceName = GetType().Name + ".bmp";
                base.m_bitmap = new Bitmap(GetType(), bitmapResourceName);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message, "Invalid Bitmap");
            }
        }

        #region Overridden Class Methods

        /// <summary>
        /// Occurs when this command is created
        /// </summary>
        /// <param name="hook">Instance of the application</param>
        public override void OnCreate(object hook)
        {
            try
            {
                if (hook == null)
                    return;

                m_application = hook as IApplication;
                m_sceneHookHelper = new SceneHookHelperClass();
                m_sceneHookHelper.Hook = hook;
                if (m_sceneHookHelper.ActiveViewer == null)
                {
                    m_sceneHookHelper = null;
                }

                if (m_sceneHookHelper == null)
                    base.m_enabled = false;
                else
                    base.m_enabled = true;
            }
            catch (Exception err)
            {
                m_sceneHookHelper = null;
                MessageBox.Show(err.ToString());
            }

            // TODO:  Add other initialization code

        }

        /// <summary>
        /// Occurs when this command is clicked
        /// </summary>
        public override void OnClick()
        {
            try
            {
                if (m_application == null)
                    return;
                IDocument document = m_application.Document;
                ISxDocument sxDocument = (ISxDocument)(document);
                if (sxDocument != null)
                    m_scene = sxDocument.Scene;
                if (m_scene == null)
                    return;
                activeView = m_scene as IActiveView;

                //有图层选图层
                if (m_scene.LayerCount == 0)
                    return;

                //选择基准面
                if (Common.SelectLayer(m_scene.Layers, out SelectedLyrIndex, true, "选择自定义表面") == false)
                    return;

                //QI
                IRasterLayer baseRasterLayer = m_scene.Layer[SelectedLyrIndex[0]] as IRasterLayer;  //不管选多少个只选第一个
                if (baseRasterLayer == null)
                    throw new ArgumentNullException("自定义表面RasterLayer转换失败，为空。");
                IRaster raster = baseRasterLayer.Raster;
                if (raster == null)
                    throw new ArgumentNullException("自定义表面Raster转换失败，为空。");
                IRasterSurface rasterSurface = new RasterSurfaceClass();
                rasterSurface.PutRaster(raster, 0);
                ISurface surface = rasterSurface as ISurface;

                //选择图层
                if (Common.SelectLayer(m_scene.Layers, out SelectedLyrIndex, false, "选择要进行偏移的图层") == false)
                    return;

                //选择偏移量
                NumSelect NS = new NumSelect();
                if (NS.ShowDialog() != DialogResult.OK)
                    return;
                double Offset = NS.Result;
                bool DisableCache = NS.DisableCache;

                //Create a CancelTracker.
                ITrackCancel pTrackCancel = new CancelTrackerClass();

                //Create the ProgressDialog. This automatically displays the dialog
                IProgressDialogFactory pProgDlgFactory = new ProgressDialogFactoryClass();
                IProgressDialog2 pProDlg = pProgDlgFactory.Create(pTrackCancel, m_application.hWnd) as IProgressDialog2;
                pProDlg.CancelEnabled = true;
                pProDlg.Title = "正在进行自定义表面设置及偏移调整";
                pProDlg.Description = "设置中，请稍候...";

                pProDlg.Animation = esriProgressAnimationTypes.esriProgressSpiral;

                IStepProgressor pStepPro = pProDlg as IStepProgressor;
                pStepPro.MinRange = 0;
                pStepPro.MaxRange = SelectedLyrIndex.Count;
                pStepPro.StepValue = 1;
                pStepPro.Message = "初始化中...";

                bool bCont = true;

                //对每一个选中的图层进行操作
                for (int i = 0; i < SelectedLyrIndex.Count; i++)
                {
                    //m_application.StatusBar.set_Message(0, i.ToString());
                    pStepPro.Message = "已完成(" + i.ToString() + "/" + SelectedLyrIndex.Count.ToString() + ")";
                    bCont = pTrackCancel.Continue();
                    if (!bCont)
                        break;

                    //选中一个栅格图层
                    IRasterLayer rasterLayer = m_scene.Layer[SelectedLyrIndex[i]] as IRasterLayer;
                    if (rasterLayer == null)
                    {
                        pStepPro.Message = "选中的图层非栅格图层...";
                        continue;
                    }

                    I3DProperties p3DProperties = null;
                    ILayerExtensions layerExtensions = rasterLayer as ILayerExtensions;

                    //遍历LayerExtensions找到I3DProperties
                    for (int j = 0; j < layerExtensions.ExtensionCount; j++)
                    {
                        if (layerExtensions.get_Extension(j) is I3DProperties)
                        {
                            p3DProperties = layerExtensions.get_Extension(j) as I3DProperties;
                        }
                    }

                    //设置I3DProperties
                    p3DProperties.BaseOption = esriBaseOption.esriBaseSurface;  //基准面浮动
                    p3DProperties.BaseSurface = surface;    //基准面
                    p3DProperties.OffsetExpressionString = Offset.ToString();   //偏移常量
                    if (DisableCache)
                    {
                        p3DProperties.RenderMode = esriRenderMode.esriRenderImmediate;  //直接从文件渲染
                        p3DProperties.RenderVisibility = esriRenderVisibility.esriRenderWhenStopped;    //停止导航时渲染
                    }
                    p3DProperties.Apply3DProperties(rasterLayer);
                }
                pProDlg.HideDialog();

                //==========================================
                //刷新，不起作用
                if (activeView == null)
                    throw new Exception("活动视图为空！ ");
                if (m_sceneHookHelper.ActiveViewer == null)
                    throw new Exception("无活动视图！");
                activeView.Refresh();
                m_sceneHookHelper.ActiveViewer.Redraw(true);
                //==========================================
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString());
            }
        }
        #endregion
    }
}
