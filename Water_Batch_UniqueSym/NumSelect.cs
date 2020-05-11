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
    public partial class NumSelect : Form
    {
        public double Result = 0;
        public bool DisableCache = false;

        public NumSelect(string Title = "输入偏移数值",bool DisableCheckBox = false)
        {
            InitializeComponent();
            this.Text = Title;
            if (DisableCheckBox)
            {
                this.DisableCacheCheckBox.Enabled = false;
                this.DisableCacheCheckBox.Visible = false;
            }
        }

        //确认
        private void ConfirmButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (Double.TryParse(NumTextBox.Text.Trim(), out Result) == true)
                {
                    this.DisableCache = DisableCacheCheckBox.Checked;
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    throw new InvalidCastException("输入的字符串无效。");
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString());
            }
        }

        //取消
        private void AbortButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
