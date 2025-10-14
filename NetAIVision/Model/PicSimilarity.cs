using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetAIVision.Model
{
    public class PicSimilarity
    {
        /// <summary>
        /// 路径
        /// </summary>
        public string path { get; set; }

        /// <summary>
        /// 阈值
        /// </summary>
        public double threshold { get; set; } = 0.98;
    }
}