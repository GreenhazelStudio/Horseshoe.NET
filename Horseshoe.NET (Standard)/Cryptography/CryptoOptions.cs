using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Horseshoe.NET.Cryptography
{
    public class CryptoOptions
    {
        private byte[] _Key;
        private string _KeyText;

        /// <summary>
        /// The desired encryption / decryption algorithm
        /// </summary>
        public SymmetricAlgorithm Algorithm { get; set; }

        /// <summary>
        /// The encryption key
        /// </summary>
        public byte[] Key
        {
            get
            {
                if (_Key != null) return _Key;
                if (_KeyText != null) return (Encoding ?? Settings.DefaultEncoding).GetBytes(_KeyText);
                return null;
            }
            set
            {
                _Key = value;
            }
        }

        /// <summary>
        /// The encryption key in text format
        /// </summary>
        public string KeyText
        {
            set
            {
                _KeyText = value;
            }
        }

        /// <summary>
        /// Max key size in bits (e.g. 128 aka 16 bytes)
        /// </summary>
        public int? KeySize { get; set; }

        /// <summary>
        /// Block size in bits (e.g. 128 aka 16 bytes)
        /// </summary>
        public int? BlockSize { get; set; }

        /// <summary>
        /// See <c>System.Security.Cryptography.PaddingMode</c>.
        /// </summary>
        public PaddingMode? PaddingMode { get; set; }

        /// <summary>
        /// Generates a random key and IV and appends them to the encryption, then extracts them prior to decrypting 
        /// </summary>
        public bool UseEmbeddedKIV { get; set; }

        public Encoding Encoding { get; set; }
    }
}
