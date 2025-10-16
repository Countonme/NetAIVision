using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetAIVision.Model
{
    public class SettingSpellCheck
    {
        /// <summary>
        /// 步驟
        /// </summary>
        public int step_number { get; set; }

        /// <summary>
        /// 建議
        /// </summary>
        public bool suggest { get; set; }
    }
}