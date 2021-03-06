﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Horseshoe.NET.Objects;

namespace Horseshoe.NET.IO.FileImport.Extensions
{
    public static class Extensions
    {
        public static bool IsBlank(this object[] objects)
        {
            foreach (var obj in objects)
            {
                if (obj is string str)
                {
                    if (Zap.String(str) != null) return false;
                }
                else if (obj != null)
                {
                    return false;
                }
            }
            return true;
        }

        public static bool HasDataImportErrors(this object[] objects)
        {
            return objects.Any(o => o is DataImportException);
        }
    }
}
