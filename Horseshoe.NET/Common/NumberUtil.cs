using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Horseshoe.NET.Common
{
    public static class NumberUtil
    {
        public static double Trunc(double value, int decimalPlaces = 0)
        {
            if (decimalPlaces == 0)
            {
                return Math.Truncate(value);
            }
            else if (decimalPlaces > 0)
            {
                var multiplier = Math.Pow(10, decimalPlaces);
                return Math.Truncate(value * multiplier) / multiplier;
            }
            else
            {
                var multiplier = Math.Pow(10, -decimalPlaces);
                return Math.Truncate(value / multiplier) * multiplier;
            }
        }
    }
}
