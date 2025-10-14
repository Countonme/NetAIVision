using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetAIVision.Model
{
    public class GaussianBlur
    {
        public int KernelSize { get; set; } = 5;
        public double SigmaX { get; set; } = 1.5;
    }
}