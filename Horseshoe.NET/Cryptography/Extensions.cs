using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Horseshoe.NET.Cryptography
{
    public static class Extensions
    {
        public static void Clear(this byte[] bytes)
        {
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = byte.MinValue;
            }
        }

        public static void Clear(this char[] chars)
        {
            for (int i = 0; i < chars.Length; i++)
            {
                chars[i] = char.MinValue;
            }
        }
    }
}
