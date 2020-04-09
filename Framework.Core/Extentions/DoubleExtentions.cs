using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.Core.Extentions
{
    public static class DoubleExtentions
    {
        public static string GetSizeFileToMegaBayt(this double size)
        {
            double sizefile = (double)size / 1000000;
            string sizeToMegabayt = sizefile.ToString("0.##");
            return sizeToMegabayt;
        }
    }
}
