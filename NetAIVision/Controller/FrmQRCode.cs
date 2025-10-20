using NetAIVision.Model;
using OpenCvSharp;
using Sunny.UI;
using Sunny.UI.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ZXing;

namespace NetAIVision.Controller
{
    public partial class FrmQRCode : UIEditForm
    {
        public FrmQRCode(int max)
        {
            InitializeComponent();
            stepNumber.Maximum = max;
            stepNumber.Minimum = 0;
        }

        private QRCode param;

        public QRCode Param
        {
            get
            {
                if (param == null)
                {
                    param = new QRCode();
                }
                //步骤
                param.step_number = stepNumber.Value;

                return param;
            }

            set
            {
                param = value;
            }
        }
    }
}