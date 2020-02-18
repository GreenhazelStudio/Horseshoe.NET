using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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

        public static IEnumerable<int> GetValidKeySizes(this SymmetricAlgorithm algorithm)
        {
            var list = new List<int>();
            foreach (var ks in algorithm.LegalKeySizes)
            {
                for (int i = ks.MinSize; i <= ks.MaxSize; i += ks.SkipSize)
                {
                    list.Add(i);
                }
            }
            list = list
                .Distinct()
                .OrderBy(i => i)
                .ToList();
            return list;
        }

        public static IEnumerable<int> GetValidBlockSizes(this SymmetricAlgorithm algorithm)
        {
            var list = new List<int>();
            foreach (var ks in algorithm.LegalBlockSizes)
            {
                for (int i = ks.MinSize; i <= ks.MaxSize; i += ks.SkipSize)
                {
                    list.Add(i);
                }
            }
            list = list
                .Distinct()
                .OrderBy(i => i)
                .ToList();
            return list;
        }
    }
}
