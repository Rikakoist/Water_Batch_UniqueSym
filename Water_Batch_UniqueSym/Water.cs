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

        private IApplication m_application;
        IMap m_map = null;
        IRgbColor FromIC= new RgbColorClass();
        IRgbColor ToIC= new RgbColorClass();

        public Water()
        {
            //
            // TODO: Define values for the public properties
            //
            base.m_category = "SRTP"; //localizable text
            base.m_caption = "洪水色带表示v2.1(20191121)。";  //localizable text
            base.m_message = "将所有栅格图层以用户选择的起止颜色创建的色带表示，并将值为0的栅格设为透明色。";  //localizable text 
            base.m_toolTip = "洪水色带表示v2.1(20191121)。";  //localizable text 
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

            //Disable if it is not ArcMap
            if (hook is IMxApplication)
                base.m_enabled = true;
            else
                base.m_enabled = false;

            // TODO:  Add other initialization code
        }

        /// <summary>
        /// Occurs when this command is clicked
        /// </summary>
        public override void OnClick()
        {
            try
            {
                // TODO: Add Water.OnClick implementation
                if (m_application == null)
                {
                    return;
                }
                IDocument document = m_application.Document;
                IMxDocument mxDocument = (IMxDocument)(document);
                m_map = mxDocument.FocusMap;
                if (m_map == null)
                    return;
                //ISelection selection = m_map.FeatureSelection;
                
                if (m_map.LayerCount != 0)
                {
                    SelectionForm SF = new SelectionForm();
                    if (SF.ShowDialog() == DialogResult.OK)
                    {
                        FromIC = SF.FromC;
                        ToIC = SF.ToC;
                        SF.Dispose();

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
                        pStepPro.MaxRange = m_map.LayerCount;
                        pStepPro.StepValue = 1;
                        pStepPro.Message = "初始化中...";


                        bool bCont = true;

                        for (int i = 0; i < m_map.LayerCount; i++)
                        {
                            //m_application.StatusBar.set_Message(0, i.ToString());
                            pStepPro.Message = "已完成(" + i.ToString() + "/" + m_map.LayerCount.ToString() + ")";
                            bCont = pTrackCancel.Continue();
                            if (!bCont)
                                break;

                            IRasterLayer rasterLayer = m_map.Layer[i] as IRasterLayer;
                            if (rasterLayer == null)
                                continue;

                            UniqueValueRender(rasterLayer);
                        }
                        pProDlg.HideDialog();
                        IActiveView activeView = m_map as IActiveView;
                        activeView.Refresh();
                    }

                    //IColorSelector colorSelector1 = new ColorSelectorClass();
                    //if (colorSelector1.DoModal(m_application.hWnd))
                    //{
                    //    FromIC = colorSelector1.Color as IRgbColor;
                    //    IColorSelector colorSelector2 = new ColorSelectorClass();
                    //    if (colorSelector2.DoModal(m_application.hWnd))
                    //    {
                    //        ToIC = colorSelector2.Color as IRgbColor;
                    //        for (int i = 0; i < m_map.LayerCount; i++)
                    //        {
                    //            IRasterLayer rasterLayer = m_map.Layer[i] as IRasterLayer;
                    //            if (rasterLayer == null)
                    //                continue;
                    //            UniqueValueRender(rasterLayer);
                    //        }
                    //    }
                    //}
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString());
            }
        }

        public void UniqueValueRender(IRasterLayer rasterLayer, string renderfiled = "Value")
        {
            try
            {
                IRasterUniqueValueRenderer uniqueValueRenderer = new RasterUniqueValueRendererClass();
                uniqueValueRenderer.Field = renderfiled;
                IRasterRenderer pRasterRenderer = uniqueValueRenderer as IRasterRenderer;

                IRasterCalcUniqueValues calcUniqueValues = new RasterCalcUniqueValuesClass();
                IUniqueValues uniqueValues = new UniqueValuesClass();
                calcUniqueValues.AddFromRaster(rasterLayer.Raster, 0, uniqueValues);//iBand=0  

                IRasterRendererUniqueValues renderUniqueValues = uniqueValueRenderer as IRasterRendererUniqueValues;
                renderUniqueValues.UniqueValues = uniqueValues;
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

                uniqueValueRenderer.HeadingCount = 1;
                uniqueValueRenderer.set_Heading(0, "All Data Value");
                uniqueValueRenderer.set_ClassCount(0, uniqueValues.Count);
                IRasterRendererColorRamp pRasterRendererColorRamp = uniqueValueRenderer as IRasterRendererColorRamp;
                pRasterRendererColorRamp.ColorRamp = colorRamp;
                //需要对算出来的唯一值升序重排
                double[] tmp = new double[uniqueValues.Count];
                for (int i = 0; i < uniqueValues.Count; i++)
                {
                    tmp[i] = Convert.ToDouble(uniqueValues.get_UniqueValue(i));
                }
                System.Array.Sort(tmp);
                for (int i = 0; i < uniqueValues.Count; i++)
                {
                    uniqueValueRenderer.AddValue(0, i, tmp[i]);
                    uniqueValueRenderer.set_Label(0, i, tmp[i].ToString());
                    IRgbColor zerocolor = new RgbColorClass()
                    {
                        Transparency = 0,
                        NullColor = true,
                    };
                    IFillSymbol fs = new SimpleFillSymbol();
                    if (tmp[i] == 0)
                    {
                        fs = new SimpleFillSymbol();
                        fs.Color = zerocolor;
                    }
                    else
                    {
                        fs.Color = colorRamp.get_Color(i);
                    }

                    uniqueValueRenderer.set_Symbol(0, i, fs as ISymbol);
                }
                rasterLayer.Renderer = pRasterRenderer;
                pRasterRenderer.Update();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        #endregion
    }
}
