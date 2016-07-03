using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FingerPrintMatching
{
    public class FingerPrintModifierOption
    {
        public int KernelSize { get; set; }
        public int ColorFactor { get; set; }

        public FingerPrintModifierOption()
        {
            KernelSize = 3;
            ColorFactor = 8;
        }

        public FingerPrintModifierOption(int kernelSize, int colorFactor)
        {
            KernelSize = kernelSize;
            ColorFactor = colorFactor;
        }
    }
}
