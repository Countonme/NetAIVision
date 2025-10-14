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
    public partial class FrmCharacterComparison : UIForm
    {
        public int step_number { get; set; }
        public string base_string { get; set; }

        public FrmCharacterComparison(int max)
        {
            InitializeComponent();
            stepNumber.Maximum = max;
            stepNumber.Minimum = 0;
            this.FormClosing += FrmCharacterComparison_FormClosing;
        }

        private void FrmCharacterComparison_FormClosing(object sender, FormClosingEventArgs e)
        {
            step_number = stepNumber.Value;
            base_string = txtBaseString.Text;
        }
    }
}