using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Horseshoe.NET.Cryptography
{
    public class HashOptions
    {
        public HashAlgorithm Algorithm { get; set; }

        public string SaltText
        {
            set
            {
                Salt = byte.Parse(value, NumberStyles.HexNumber);
            }
        }

        public byte? Salt { get; set; }

        public Encoding Encoding { get; set; }
    }
}
