using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Water_Batch_UniqueSym
{
    /// <summary>
    /// 应用类型枚举。
    /// </summary>
    enum ApplicationType
    {
        None = 0,
        ArcMap = 1,
        ArcScene = 2,
    }

    class Common
    {
        /// <summary>
        /// 选择起止颜色。
        /// </summary>
        /// <param name="FromIC">输出的起始颜色。</param>
        /// <param name="ToIC">输出的终止颜色。</param>
        /// <returns>操作是否成功。</returns>
        public static bool SelectColor(out IRgbColor FromIC, out IRgbColor ToIC)
        {
            FromIC = null;
            ToIC = null;
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
        /// 选择需要进行操作的图层。
        /// </summary>
        /// <param name="enumLayer">用于初始化选择列表的枚举图层。</param>
        /// <param name="SelectedLyrIndex">输出的选中图层索引。</param>
        /// <returns>操作是否成功。</returns>
        public static bool SelectLayer(IEnumLayer enumLayer, out List<int> SelectedLyrIndex, bool IsSingleSelect = false, string Title = "选择要进行唯一值化的图层")
        {
            SelectedLyrIndex = null;
            LayerSelection LS = new LayerSelection(enumLayer, IsSingleSelect, Title);
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
    }
}
