using NetAIVision.Model;
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
    public partial class FrmGaussianBlur : UIEditForm
    {
        public FrmGaussianBlur()
        {
            InitializeComponent();
        }

        private GaussianBlur param;

        public GaussianBlur Param
        {
            get
            {
                if (param == null)
                {
                    param = new GaussianBlur();
                }
                //卷积核大小
                param.KernelSize = kernelSize.Value;
                //标注差值
                param.SigmaX = SigmaX.Value;
                return param;
            }

            set
            {
                param = value;
            }
        }
    }
}