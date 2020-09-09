using System;
using System.Security.Cryptography;
using System.Text;

namespace Horseshoe.NET.Crypto
{
    public static class CryptoSettings
    {
        private static SymmetricAlgorithm _defaultSymmetricAlgorithm;

        /// <summary>
        /// Gets or sets the default symmetric algorithm used by Cryptography.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:Crypto.SymmetricAlgorithm and OrganizationalDefaultSettings: key = Cryptography.SymmetricAlgorithm)
        /// </summary>
        public static SymmetricAlgorithm DefaultSymmetricAlgorithm
        {
            get
            {
                if (_defaultSymmetricAlgorithm == null)
                {
                    _defaultSymmetricAlgorithm = OrganizationalDefaultSettings.Get<SymmetricAlgorithm>("Crypto.SymmetricAlgorithm")
                        ?? CryptoUtil.BuildSymmetricAlgorithm(new RijndaelManaged(), DefaultEncoding.GetBytes("k+ (&tw!tBv~$6u7"), false, null, true, null, null, null);
                }
                return _defaultSymmetricAlgorithm;
            }
            set
            {
                _defaultSymmetricAlgorithm = value;
            }
        }

        private static HashAlgorithm _defaultHashAlgorithm;

        /// <summary>
        /// Gets or sets the default hash algorithm used by Cryptography.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:Crypto.HashAlgorithm and OrganizationalDefaultSettings: key = Cryptography.HashAlgorithm)
        /// </summary>
        public static HashAlgorithm DefaultHashAlgorithm
        {
            get
            {
                return _defaultHashAlgorithm  // example "System.Security.Cryptography.SHA256CryptoServiceProvider"
                    ?? OrganizationalDefaultSettings.Get<HashAlgorithm>("Crypto.HashAlgorithm")
                    ?? new SHA1CryptoServiceProvider();
            }
            set
            {
                _defaultHashAlgorithm = value;
            }
        }

        static byte? _defaultHashSalt;

        /// <summary>
        /// Gets or sets the default hash salt used by Cryptography.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:Crypto.HashSalt and OrganizationalDefaultSettings: key = Cryptography.HashSalt)
        /// </summary>
        public static byte? DefaultHashSalt
        {
            get
            {
                return _defaultHashSalt
                    ?? OrganizationalDefaultSettings.GetNByte("Crypto.HashSalt");
            }
            set
            {
                _defaultHashSalt = value;
            }
        }

        static Encoding _defaultEncoding;

        /// <summary>
        /// Gets or sets the text encoding used by Cryptography. Defaults to UTF8Encoding. Note: Override by passing directly to a Cryptography function or via config file: key = "Horseshoe.NET:Crypto.Encoding"
        /// </summary>
        public static Encoding DefaultEncoding
        {
            get
            {
                return _defaultEncoding
                    ?? OrganizationalDefaultSettings.Get<Encoding>("Crypto.Encoding")
                    ?? Encoding.Default;
            }
            set
            {
                _defaultEncoding = value;
            }
        }
    }
}
