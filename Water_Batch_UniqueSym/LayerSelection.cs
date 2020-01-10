using ESRI.ArcGIS.Carto;
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
    public partial class LayerSelection : Form
    {
        public List<int> selectionIndex = new List<int>();
        public LayerSelection(IEnumLayer enumLayer)
        {
            InitializeComponent();
            LayerSelectionCheckedListBox.Items.Clear(); //启动时清空

            //遍历图层
            enumLayer.Reset();
            ILayer layer = enumLayer.Next();
            while (layer != null)
            {
                string lyrInfo = layer.Name;
                if (layer is IRasterLayer rasterLayer)
                {
                    LayerSelectionCheckedListBox.Items.Add(lyrInfo, true);   //栅格勾选
                }
                else
                {
                    LayerSelectionCheckedListBox.Items.Add(lyrInfo, false);
                }
                layer =enumLayer.Next();
            }
            if(LayerSelectionCheckedListBox.Items.Count<1)
            {
                throw new ArgumentOutOfRangeException("传入图层个数非法！");
            }
        }
        

        //收集选中图层索引
        private void CollectIndex()
        {
            selectionIndex.Clear();
            for (int i = 0; i < LayerSelectionCheckedListBox.Items.Count; i++)
            {
                if (LayerSelectionCheckedListBox.GetItemChecked(i))
                {
                    selectionIndex.Add(i);
                }
            }
        }

        //选择按钮
        private void ChangeSelection(object sender, EventArgs e)
        {
            for (int i = 0; i < LayerSelectionCheckedListBox.Items.Count; i++)
            {
                if (sender == SelectAllButton)
                {
                    LayerSelectionCheckedListBox.SetItemChecked(i, true);
                }
                if (sender == SelectNoneButton)
                {
                    LayerSelectionCheckedListBox.SetItemChecked(i, false);
                }
                if (sender == SelectInverseButton)
                {
                    LayerSelectionCheckedListBox.SetItemChecked(i, !LayerSelectionCheckedListBox.GetItemChecked(i));
                }
            }
            //CheckNullSelection();
        }

        //有项目选中更改后事件
        private void CheckNullSelection()
        {
            int i = 0;
            for (i = 0; i < LayerSelectionCheckedListBox.Items.Count; i++)
            {
                if (LayerSelectionCheckedListBox.GetItemChecked(i))
                {
                    break;
                }
            }
            ConfirmButton.Enabled = (i == LayerSelectionCheckedListBox.Items.Count ? false : true);
        }

        //有bug
        private void LayerSelectionCheckedListBox_Validated(object sender, EventArgs e)
        {
            CheckNullSelection();
        }

        //确认
        private void ConfirmButton_Click(object sender, EventArgs e)
        {
            CollectIndex();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        //取消
        private void AbortButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
