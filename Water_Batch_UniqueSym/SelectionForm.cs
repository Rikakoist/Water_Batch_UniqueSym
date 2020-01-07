using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Water_Batch_UniqueSym
{
    public partial class SelectionForm : Form
    {
        public IRgbColor FromC = new RgbColorClass();
        public IRgbColor ToC = new RgbColorClass();
        Color F = Color.White;
        Color T = Color.Black;

        public SelectionForm()
        {
            InitializeComponent();
        }

        private void SelectionForm_Load(object sender, EventArgs e)
        {
            ResetIRgbColor();
            ResetF();
            ResetT();
            CreateColorRamp();
        }

        //点击PictureBox选择颜色。
        private void FromColorPictureBox_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.Default;
                this.Enabled = false;
                IColorSelector FromColorSelector = new ColorSelectorClass()
                {
                    Color = FromC,
                };
                if (FromColorSelector.DoModal(0))
                {
                    FromC = FromColorSelector.Color as IRgbColor;
                    if (FromC == null)
                    {
                        throw new Exception("选择了非RGB颜色。。。");
                    }
                    else
                    {
                        ResetF();
                    }
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString());
                FromC = new RgbColorClass()
                {
                    Transparency = F.A,
                    Red = F.R,
                    Green = F.G,
                    Blue = F.B,
                };
            }
            finally
            {
                this.Enabled = true;
                CreateColorRamp();
            }
        }

        private void ToColorPictureBox_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.Default;
                this.Enabled = false;
                IColorSelector ToColorSelector = new ColorSelectorClass()
                {
                    Color = ToC,
                };
                if (ToColorSelector.DoModal(0))
                {
                    ToC = ToColorSelector.Color as IRgbColor;
                    if (ToC == null)
                    {
                        throw new Exception("选择了非RGB颜色。。。");
                    }
                    else
                    {
                        ResetT();
                    }
                }
            }
            catch(Exception err)
            {
                MessageBox.Show(err.ToString());
                ToC = new RgbColorClass()
                {
                    Transparency = T.A,
                    Red = T.R,
                    Green = T.G,
                    Blue = T.B,
                };
            }
            finally
            {
                this.Enabled = true;
                CreateColorRamp();
            }
        }

        /// <summary>
        /// IRgb颜色对象转换为ARGB颜色。
        /// </summary>
        /// <param name="IrgbColor">IRgbColor颜色对象。</param>
        /// <returns>转换到的ARGB颜色。</returns>
        private Color IRgb2RGB(IRgbColor IrgbColor)
        {
            if (IrgbColor.NullColor)
                return Color.FromArgb(0, IrgbColor.Red, IrgbColor.Green, IrgbColor.Blue);
            return Color.FromArgb(IrgbColor.Transparency, IrgbColor.Red, IrgbColor.Green, IrgbColor.Blue);
        }

        /// <summary>
        /// ARGB颜色转换为IRgb颜色对象。
        /// </summary>
        /// <param name="rgbColor">ARGB颜色。</param>
        /// <returns>转换到的IRgb颜色对象。</returns>
        private IRgbColor RGB2IRgb(Color rgbColor)
        {
            IRgbColor c = new RgbColorClass()
            {
                Transparency = rgbColor.A,
                Red = rgbColor.R,
                Green = rgbColor.G,
                Blue = rgbColor.B,
                NullColor = false,
            };
            return c;
        }

        /// <summary>
        /// 创建渐变色带（ColorRamp不好使啊！）。
        /// </summary>
        private void CreateColorRamp()
        {
            //IAlgorithmicColorRamp colorRamp = new AlgorithmicColorRampClass
            //{
            //    FromColor = FromC,
            //    ToColor = ToC,
            //    Size = PreviewPictureBox.Width,
            //};
            //bool pOk;
            //colorRamp.CreateRamp(out pOk);
            Bitmap bitmap = new Bitmap(PreviewPictureBox.Width, PreviewPictureBox.Height);
            for (int i = 0; i < PreviewPictureBox.Width; i++)
            {
                for (int j = 0; j < PreviewPictureBox.Height; j++)
                {
                    //bitmap.SetPixel(i, 0, Color.FromArgb(255,colorRamp.Color[i].RGB));
                    bitmap.SetPixel(i, j, Color.FromArgb(Gradient(F.A, T.A, i, PreviewPictureBox.Width), Gradient(F.R, T.R, i, PreviewPictureBox.Width), Gradient(F.G, T.G, i, PreviewPictureBox.Width), Gradient(F.B, T.B, i, PreviewPictureBox.Width)));
                }
            }
            PreviewPictureBox.Image = bitmap;
            //return pOk;
        }

        /// <summary>
        /// 渐变计算。
        /// </summary>
        /// <param name="from">起始值。</param>
        /// <param name="to">终止值。</param>
        /// <param name="now">迭代当前值。</param>
        /// <param name="desti">迭代终点值。</param>
        /// <returns>8位整型。</returns>
        private byte Gradient(int from, int to, int now, int desti)
        {
            return (byte)(from+((double)now/desti*(to-from)));
        }

        //重置IRgb颜色。
        private void ResetIRgbColor()
        {
            F = Properties.IRgbColorSettings.Default.FromColor;
            if (F == null)
            {
                F = Color.Black;
            }
            FromC = RGB2IRgb(F);
            T = Properties.IRgbColorSettings.Default.ToColor;
            if (T == null)
            {
                T = Color.White;
            }
            ToC = RGB2IRgb(T);
        }

        //重置预览颜色及文本。
        private void ResetF()
        {
            F = IRgb2RGB(FromC);
            FromColorPictureBox.BackColor = IRgb2RGB(FromC);
            FromColorLabel.Text = "A: " + F.A + "\r\nR: " + F.R + "\r\nG: " + F.G + "\r\nB: " + F.B;
        }
        private void ResetT()
        {
            T = IRgb2RGB(ToC);
            ToColorPictureBox.BackColor = T;
            ToColorLabel.Text = "A: " + T.A + "\r\nR: " + T.R + "\r\nG: " + T.G + "\r\nB: " + T.B;
        }

        private void ConfirmButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void AbortButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void SelectionForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                Properties.IRgbColorSettings.Default.FromColor = F;
                Properties.IRgbColorSettings.Default.ToColor = T;
                Properties.IRgbColorSettings.Default.Save();
            }
            catch(Exception err)
            {
                MessageBox.Show(err.ToString());
            }
        }
    }
}
