using System;
using System.Security.Cryptography;
using System.Text;

using Horseshoe.NET.Application;
using Horseshoe.NET.Objects;

namespace Horseshoe.NET.Cryptography
{
    public static class CryptoSettings
    {
        private static SymmetricAlgorithm _defaultSymmetricAlgorithm;

        /// <summary>
        /// Gets or sets the default symmetric algorithm used by Cryptography.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:Cryptography.SymmetricAlgorithm and OrganizationalDefaultSettings: key = Cryptography.SymmetricAlgorithm)
        /// </summary>
        public static SymmetricAlgorithm DefaultSymmetricAlgorithm
        {
            get
            {
                if (_defaultSymmetricAlgorithm == null)
                {
                    _defaultSymmetricAlgorithm = CryptoUtil.BuildSymmetricAlgorithm
                    (
                        Config.Get<SymmetricAlgorithm>("Horseshoe.NET:Cryptography.SymmetricAlgorithm"),   // e.g. "System.Security.Cryptography.AesCryptoServiceProvider"
                        Config.GetBytes("Horseshoe.NET:Cryptography.SymmetricKey", encoding: DefaultEncoding),
                        false,
                        Config.GetBytes("Horseshoe.NET:Cryptography.SymmetricIV", encoding: DefaultEncoding),
                        true,
                        Config.GetNInt("Horseshoe.NET:Cryptography.SymmetricBlockSize"),
                        Config.GetNEnum<CipherMode>("Horseshoe.NET:Cryptography.SymmetricCipherMode"),
                        Config.GetNEnum<PaddingMode>("Horseshoe.NET:Cryptography.SymmetricPadding")
                    )
                    ?? OrganizationalDefaultSettings.Get<SymmetricAlgorithm>("Cryptography.SymmetricAlgorithm")
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
        /// Gets or sets the default hash algorithm used by Cryptography.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:Cryptography.HashAlgorithm and OrganizationalDefaultSettings: key = Cryptography.HashAlgorithm)
        /// </summary>
        public static HashAlgorithm DefaultHashAlgorithm
        {
            get
            {
                return _defaultHashAlgorithm  // example "System.Security.Cryptography.SHA256CryptoServiceProvider"
                    ?? Config.Get<HashAlgorithm>("Horseshoe.NET:Cryptography.HashAlgorithm")
                    ?? OrganizationalDefaultSettings.Get<HashAlgorithm>("Cryptography.HashAlgorithm")
                    ?? new SHA1CryptoServiceProvider();
            }
            set
            {
                _defaultHashAlgorithm = value;
            }
        }

        static byte? _defaultHashSalt;

        /// <summary>
        /// Gets or sets the default hash salt used by Cryptography.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:Cryptography.HashSalt and OrganizationalDefaultSettings: key = Cryptography.HashSalt)
        /// </summary>
        public static byte? DefaultHashSalt
        {
            get
            {
                return _defaultHashSalt
                    ?? Config.GetNByte("Horseshoe.NET:Cryptography.HashSalt")    // example: 240 or HashSalt[hex] F0
                    ?? OrganizationalDefaultSettings.GetNByte("Cryptography.HashSalt");
            }
            set
            {
                _defaultHashSalt = value;
            }
        }

        static Encoding _defaultEncoding;

        /// <summary>
        /// Gets or sets the text encoding used by Cryptography. Defaults to UTF8Encoding. Note: Override by passing directly to a Cryptography function or via config file: key = "Horseshoe.NET:Cryptography.Encoding"
        /// </summary>
        public static Encoding DefaultEncoding
        {
            get
            {
                return _defaultEncoding
                    ?? ObjectUtil.GetInstance<Encoding>(Config.Get("Horseshoe.NET:Cryptography.Encoding"), suppressErrors: true)   // example: "System.Text.UTF8Encoding"
                    ?? OrganizationalDefaultSettings.Get<Encoding>("Cryptography.Encoding")
                    ?? Encoding.Default;
            }
            set
            {
                _defaultEncoding = value;
            }
        }
    }
}
