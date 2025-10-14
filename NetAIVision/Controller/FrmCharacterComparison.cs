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
    public partial class FrmCharacterComparison : UIEditForm
    {
        public FrmCharacterComparison(int max)
        {
            InitializeComponent();
            stepNumber.Maximum = max;
            stepNumber.Minimum = 0;
        }

        private CharacterComparison person;

        public CharacterComparison Person
        {
            get
            {
                if (person == null)
                {
                    person = new CharacterComparison();
                }

                person.step_number = stepNumber.Value;
                person.base_string = txtBaseString.Text;
                return person;
            }

            set
            {
                person = value;
            }
        }
    }
}