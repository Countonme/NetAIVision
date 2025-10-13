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
    public partial class FrmGaussianBlur : UIForm
    {
        public int _KernelSize { get; set; } = 5;
        public double _SigmaX { get; set; } = 1.5;

        public FrmGaussianBlur()
        {
            InitializeComponent();
        }
    }
}