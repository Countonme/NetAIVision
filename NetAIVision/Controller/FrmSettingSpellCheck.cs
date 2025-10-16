using NetAIVision.Model;
using OpenCvSharp;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NetAIVision.Controller
{
    public partial class FrmSettingSpellCheck : UIEditForm
    {
        public FrmSettingSpellCheck(int max)
        {
            InitializeComponent();
            stepNumber.Maximum = max;
            stepNumber.Minimum = 0;
        }

        private SettingSpellCheck param;

        public SettingSpellCheck Param
        {
            get
            {
                if (param == null)
                {
                    param = new SettingSpellCheck();
                }
                //卷积核大小
                param.step_number = stepNumber.Value;
                //标注差值
                param.suggest = uiCheckBox1.Checked;
                return param;
            }

            set
            {
                param = value;
            }
        }
    }
}