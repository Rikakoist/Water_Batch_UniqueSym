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
        public List<int> selectionIndex = new List<int>();  //选择的索引列表
        private bool singleSelect = false;  //是否单选

        public LayerSelection(IEnumLayer enumLayer, bool SingleSelect, string Title)
        {
            InitializeComponent();
            LayerSelectionCheckedListBox.Items.Clear(); //启动时清空选定项
            singleSelect = SingleSelect;
            this.Text = Title;

            if (singleSelect)
            {
                SelectedCountLabel.Visible = false;
            }
            else
            {
                SelectedCountLabel.Visible = true;
            }
            //遍历图层
            enumLayer.Reset();
            ILayer layer = enumLayer.Next();
            while (layer != null)
            {
                string lyrInfo = layer.Name;
                if ((layer is IRasterLayer rasterLayer) && (!singleSelect))
                {
                    LayerSelectionCheckedListBox.Items.Add(lyrInfo, true);   //是栅格则勾选
                }
                else
                {
                    LayerSelectionCheckedListBox.Items.Add(lyrInfo, false);
                }

                layer = enumLayer.Next();
            }
            if (LayerSelectionCheckedListBox.Items.Count < 1)
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
        }

        //选择更改后的显示与操作
        private void LayerSelectionCheckedListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (e.NewValue == CheckState.Checked)
            {
                SelectedCountLabel.Text = "已选择" + (LayerSelectionCheckedListBox.CheckedItems.Count + 1).ToString() + "/" + LayerSelectionCheckedListBox.Items.Count.ToString() + "图层。";
                CheckNullSelection(LayerSelectionCheckedListBox.CheckedItems.Count + 1);
            }
            else
            {
                SelectedCountLabel.Text = "已选择" + (LayerSelectionCheckedListBox.CheckedItems.Count - 1).ToString() + "/" + LayerSelectionCheckedListBox.Items.Count.ToString() + "图层。";
                CheckNullSelection(LayerSelectionCheckedListBox.CheckedItems.Count - 1);
            }
        }

        //有项目选中更改后事件
        private void CheckNullSelection(int value)
        {
            ConfirmButton.Enabled = value == 0 ? false : true;
            ConfirmButton.Cursor = value == 0 ? Cursors.No : Cursors.Hand;
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
