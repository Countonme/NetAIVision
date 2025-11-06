using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetAIVision.Model.ROI
{
    public class ScriptROI
    {
        /// <summary>
        /// ROI 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// ROI ID
        /// </summary>
        public string id { get; set; } = global::System.Guid.NewGuid().ToString();

        /// <summary>
        /// ROI 区域
        /// </summary>
        public Rectangle Rect { get; set; }

        /// <summary>
        /// msg
        /// </summary>
        public string msg { get; set; }

        /// <summary>
        /// 测试脚本信息
        /// </summary>
        public List<string> step_script { get; set; }

        /// <summary>
        /// 边框画笔颜色
        /// </summary>
        public Color pen_color { get; set; } = Color.Red;

        /// <summary>
        /// 字体颜色
        /// </summary>
        public Brush Brushes_color { get; set; } = Brushes.Blue;
    }
}