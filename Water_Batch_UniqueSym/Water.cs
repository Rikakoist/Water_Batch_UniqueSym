using System;
using System.Drawing;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.DataSourcesRaster;
using System.Windows.Forms;
using System.Collections.Generic;
using ESRI.ArcGIS.ArcScene;
using ESRI.ArcGIS.Analyst3D;

namespace Water_Batch_UniqueSym
{
    /// <summary>
    /// Summary description for Water.
    /// </summary>
    [Guid("f24f817d-ae86-4445-a2d7-9b76f3065247")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("Water_Batch_UniqueSym.Water")]
    public sealed class Water : BaseCommand
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
            MxCommands.Register(regKey);

        }
        /// <summary>
        /// Required method for ArcGIS Component Category unregistration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryUnregistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            MxCommands.Unregister(regKey);

        }

        #endregion
        #endregion

        /// <summary>
        /// 应用类型枚举。
        /// </summary>
        private enum ApplicationType
        {
            None = 0,
            ArcMap = 1,
            ArcScene = 2,
        }

        IApplication m_application;
        IActiveView activeView;
        IMap m_map = null;
        IScene m_scene = null;
        IRgbColor FromIC = new RgbColorClass();
        IRgbColor ToIC = new RgbColorClass();
        ApplicationType applicationType = ApplicationType.None;
        List<int> SelectedLyrIndex;

        public Water()
        {
            //
            // TODO: Define values for the public properties
            //
            base.m_category = "SRTP"; //localizable text
            base.m_caption = "洪水色带表示v4.1(20200210)。";  //localizable text
            base.m_message = "将所有用户选定的栅格图层以用户选择的起止颜色创建的色带表示，并将值为0的栅格设为透明色。";  //localizable text 
            base.m_toolTip = "洪水色带表示v4.1(20200210)。";  //localizable text 
            base.m_name = "Water_unique";   //unique id, non-localizable (e.g. "MyCategory_ArcMapCommand")

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
            if (hook == null)
                return;

            m_application = hook as IApplication;
            //判断应用类型
            if (hook is IMxApplication)
                applicationType = ApplicationType.ArcMap;
            if (hook is ISxApplication)
                applicationType = ApplicationType.ArcScene;

            //判断应用类型不为None则启用工具
            if (applicationType != ApplicationType.None)
                base.m_enabled = true;
            else
                base.m_enabled = false;
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

                //根据应用类型初始化
                switch (applicationType)
                {
                    case ApplicationType.ArcMap:
                        {
                            IMxDocument mxDocument = (IMxDocument)(document);
                            if (mxDocument != null)
                                m_map = mxDocument.FocusMap;
                            if (m_map == null)
                                return;
                            activeView = m_map as IActiveView;

                            //有图层则选颜色
                            if (m_map.LayerCount == 0)
                                return;
                            if (SelectColor() == false)
                                return;

                            //选图层
                            if (SelectLayer(m_map.Layers) == false)
                                return;
                            break;
                        }
                    case ApplicationType.ArcScene:
                        {
                            ISxDocument sxDocument = (ISxDocument)(document);
                            if (sxDocument != null)
                                m_scene = sxDocument.Scene;
                            if (m_scene == null)
                                return;
                            activeView = m_scene as IActiveView;

                            //有图层则选颜色
                            if (m_scene.LayerCount == 0)
                                return;
                            if (SelectColor() == false)
                                return;

                            //选图层
                            if (SelectLayer(m_scene.Layers) == false)
                                return;
                            break;
                        }
                    default:
                        {
                            throw new ArgumentException("未指定应用类型。");
                        }
                }

                //Create a CancelTracker.
                ITrackCancel pTrackCancel = new CancelTrackerClass();

                //Create the ProgressDialog. This automatically displays the dialog
                IProgressDialogFactory pProgDlgFactory = new ProgressDialogFactoryClass();
                IProgressDialog2 pProDlg = pProgDlgFactory.Create(pTrackCancel, m_application.hWnd) as IProgressDialog2;
                pProDlg.CancelEnabled = true;
                pProDlg.Title = "正在进行唯一值计算";
                pProDlg.Description = "唯一值计算中，请稍候...";

                pProDlg.Animation = esriProgressAnimationTypes.esriProgressSpiral;

                IStepProgressor pStepPro = pProDlg as IStepProgressor;
                pStepPro.MinRange = 0;
                pStepPro.MaxRange = SelectedLyrIndex.Count;
                pStepPro.StepValue = 1;
                pStepPro.Message = "初始化中...";


                bool bCont = true;

                //对每一个选中的图层进行唯一值化
                for (int i = 0; i < SelectedLyrIndex.Count; i++)
                {
                    //m_application.StatusBar.set_Message(0, i.ToString());
                    pStepPro.Message = "已完成(" + i.ToString() + "/" + SelectedLyrIndex.Count.ToString() + ")";
                    bCont = pTrackCancel.Continue();
                    if (!bCont)
                        break;

                    IRasterLayer rasterLayer = null;
                    switch (applicationType)
                    {
                        case ApplicationType.ArcMap:
                            {
                                rasterLayer = m_map.Layer[SelectedLyrIndex[i]] as IRasterLayer;
                                break;
                            }
                        case ApplicationType.ArcScene:
                            {
                                rasterLayer = m_scene.Layer[SelectedLyrIndex[i]] as IRasterLayer;
                                break;
                            }
                        default:
                            {
                                throw new ArgumentException("未指定应用类型。");
                            }
                    }
                    if (rasterLayer == null)
                    {
                        pStepPro.Message = "选中的图层非栅格图层...";
                        continue;
                    }

                    //设置图层渲染器并更新
                    IRasterRenderer pRasterRenderer = UniqueValueRender(rasterLayer); //渲染唯一值
                    rasterLayer.Renderer = pRasterRenderer;
                    pRasterRenderer.Update();
                }
                pProDlg.HideDialog();
                //刷新
                if (activeView == null)
                    throw new Exception("活动视图为空！ ");
                //activeView.Activate(m_application.hWnd);
                activeView.Refresh();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString());
            }
        }

        /// <summary>
        /// 选择起止颜色。
        /// </summary>
        /// <returns>操作是否成功。</returns>
        private bool SelectColor()
        {
            ColorSelection CS = new ColorSelection();
            if (CS.ShowDialog() != DialogResult.OK)
            {
                CS.Dispose();
                return false;
            }
            FromIC = CS.FromC;
            ToIC = CS.ToC;
            CS.Dispose();
            return true;
        }

        /// <summary>
        /// 选择需要进行唯一值化操作的图层。
        /// </summary>
        /// <param name="enumLayer">用于初始化选择列表的枚举图层。</param>
        /// <returns>操作是否成功。</returns>
        private bool SelectLayer(IEnumLayer enumLayer)
        {
            LayerSelection LS = new LayerSelection(enumLayer);
            if (LS.ShowDialog() != DialogResult.OK)
            {
                LS.Dispose();
                return false;
            }
            SelectedLyrIndex = LS.selectionIndex;
            LS.Dispose();
            if (SelectedLyrIndex.Count < 1)
            {
                throw new ArgumentOutOfRangeException("未选中任何图层！");
            }
            return true;
        }

        /// <summary>
        /// 设置图层渲染器。
        /// </summary>
        /// <param name="rasterLayer">需要渲染唯一值的栅格图层。</param>
        /// <param name="renderfiled">渲染的字段（可选，默认为Value）。</param>
        /// <returns></returns>
        public IRasterRenderer UniqueValueRender(IRasterLayer rasterLayer, string renderfiled = "Value")
        {
            try
            {
                //这是从头用到尾的对象
                IRasterUniqueValueRenderer uniqueValueRenderer = new RasterUniqueValueRendererClass
                {
                    Field = renderfiled
                };
                IRasterRenderer pRasterRenderer = uniqueValueRenderer as IRasterRenderer;

                //计算栅格唯一值
                IRasterCalcUniqueValues calcUniqueValues = new RasterCalcUniqueValuesClass();
                IUniqueValues uniqueValues = new UniqueValuesClass();
                calcUniqueValues.AddFromRaster(rasterLayer.Raster, 0, uniqueValues);

                //设置唯一值
                IRasterRendererUniqueValues renderUniqueValues = uniqueValueRenderer as IRasterRendererUniqueValues;
                renderUniqueValues.UniqueValues = uniqueValues;

                //创建色带
                IRgbColor pFromColor = FromIC;
                IRgbColor pToColor = ToIC;
                IAlgorithmicColorRamp colorRamp = new AlgorithmicColorRampClass
                {
                    FromColor = pFromColor,
                    ToColor = pToColor,
                    Size = uniqueValues.Count
                };
                bool pOk;
                colorRamp.CreateRamp(out pOk);

                //设置标题
                uniqueValueRenderer.HeadingCount = 1;
                uniqueValueRenderer.set_Heading(0, "All Data Value");
                uniqueValueRenderer.set_ClassCount(0, uniqueValues.Count);

                //设置色带
                IRasterRendererColorRamp pRasterRendererColorRamp = uniqueValueRenderer as IRasterRendererColorRamp;
                pRasterRendererColorRamp.ColorRamp = colorRamp;

                //需要对算出来的唯一值升序重排
                double[] tmp = new double[uniqueValues.Count];
                for (int i = 0; i < uniqueValues.Count; i++)
                {
                    tmp[i] = Convert.ToDouble(uniqueValues.get_UniqueValue(i));
                }
                System.Array.Sort(tmp);

                //对每一个唯一值设置颜色
                for (int i = 0; i < uniqueValues.Count; i++)
                {
                    //添加唯一值并设置标签
                    uniqueValueRenderer.AddValue(0, i, tmp[i]);
                    uniqueValueRenderer.set_Label(0, i, tmp[i].ToString());

                    //透明色及参数
                    IRgbColor zerocolor = new RgbColorClass()
                    {
                        Transparency = 0,
                        NullColor = true,
                    };

                    IFillSymbol fs = new SimpleFillSymbol();    //唯一值填充符号
                    if (tmp[i] == 0)    //将值为0的栅格颜色设为透明
                    {
                        fs = new SimpleFillSymbol
                        {
                            Color = zerocolor
                        };
                    }
                    else   //值不为零则设置为色带对应索引颜色
                    {
                        fs.Color = colorRamp.get_Color(i);
                    }
                    uniqueValueRenderer.set_Symbol(0, i, fs as ISymbol);    //对唯一值设置色带对应颜色
                }
                return pRasterRenderer;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return null;
            }
        }
        #endregion
    }
}
