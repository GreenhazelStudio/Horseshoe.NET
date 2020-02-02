using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Horseshoe.NET.Text;

namespace Horseshoe.NET.IO.FileImport
{
    public static class Extensions
    {
        public static bool IsBlank(this object[] array)
        {
            foreach (var obj in array)
            {
                if (obj is string str)
                {
                    if (TextUtil.Zap(str) != null) return false;
                }
                else if (obj != null)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
