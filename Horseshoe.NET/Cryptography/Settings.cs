using System;
using System.Security.Cryptography;
using System.Text;

using Horseshoe.NET.Application;
using Horseshoe.NET.Objects;

namespace Horseshoe.NET.Cryptography
{
    public static class Settings
    {
        private static SymmetricAlgorithm _defaultSymmetricAlgorithm;

        /// <summary>
        /// Gets or sets the default symmetric algorithm used by Cryptography.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:Cryptography:SymmetricAlgorithm and OrganizationalDefaultSettings: key = Cryptography.SymmetricAlgorithm)
        /// </summary>
        public static SymmetricAlgorithm DefaultSymmetricAlgorithm
        {
            get
            {
                return _defaultSymmetricAlgorithm           // example "System.Security.Cryptography.AesCryptoServiceProvider"
                    ?? Config.Get<SymmetricAlgorithm>("Horseshoe.NET:Cryptography:SymmetricAlgorithm")
                    ?? OrganizationalDefaultSettings.Get<SymmetricAlgorithm>("Cryptography.SymmetricAlgorithm")
                    ?? new RijndaelManaged();
            }
            set
            {
                _defaultSymmetricAlgorithm = value;
            }
        }

        /// <summary>
        /// Gets or sets the default symmetric key used by Cryptography.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:Cryptography:SymmetricKey and OrganizationalDefaultSettings: key = Cryptography.SymmetricKey)
        /// </summary>
        public static string DefaultSymmetricKeyText { get; set; }

        private static byte[] _defaultSymmetricKey;


        /// <summary>
        /// Gets or sets the default symmetric key used by Cryptography.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:Cryptography:SymmetricKey and OrganizationalDefaultSettings: key = Cryptography.SymmetricKey)
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "Array is the only option for symmetric keys")]
        public static byte[] DefaultSymmetricKey
        {
            get
            {
                return _defaultSymmetricKey
                    ?? (DefaultSymmetricKeyText != null ? DefaultEncoding.GetBytes(DefaultSymmetricKeyText) : null)
                    ?? Config.GetBytes("Horseshoe.NET:Cryptography:SymmetricKey", encoding: DefaultEncoding)
                    ?? OrganizationalDefaultSettings.GetBytes("Cryptography.SymmetricKey", encoding: DefaultEncoding)
                    ?? DefaultEncoding.GetBytes(")0ju3#2!u83&+2ez");
            }
            set
            {
                _defaultSymmetricKey = value;
            }
        }

        static int? _defaultSymmetricKeySize;

        /// <summary>
        /// Gets or sets the default symmetric key size used by Cryptography.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:Cryptography:SymmetricKeySize and OrganizationalDefaultSettings: key = Cryptography.SymmetricKeySize)
        /// </summary>
        public static int? DefaultSymmetricKeySize
        {
            get
            {
                return _defaultSymmetricKeySize
                    ?? Config.GetNInt("Horseshoe.NET:Cryptography:SymmetricKeySize")
                    ?? OrganizationalDefaultSettings.GetNInt("Cryptography.SymmetricKeySize");
            }
            set
            {
                _defaultSymmetricKeySize = value;
            }
        }

        static int? _defaultSymmetricBlockSize;

        /// <summary>
        /// Gets or sets the default symmetric block size used by Cryptography.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:Cryptography:SymmetricBlockSize and OrganizationalDefaultSettings: key = Cryptography.SymmetricBlockSize)
        /// </summary>
        public static int? DefaultSymmetricBlockSize
        {
            get
            {
                return _defaultSymmetricBlockSize
                    ?? Config.GetNInt("Horseshoe.NET:Cryptography:SymmetricBlockSize")
                    ?? OrganizationalDefaultSettings.GetNInt("Cryptography.SymmetricBlockSize");
            }
            set
            {
                _defaultSymmetricBlockSize = value;
            }
        }

        static PaddingMode? _defaultSymmetricPaddingMode;

        /// <summary>
        /// Gets or sets the default symmetric padding mode used by Cryptography.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:Cryptography:SymmetricPaddingMode and OrganizationalDefaultSettings: key = Cryptography.SymmetricPaddingMode)
        /// </summary>
        public static PaddingMode DefaultSymmetricPaddingMode 
        {
            get
            {
                return _defaultSymmetricPaddingMode
                    ?? Config.GetNEnum<PaddingMode>("Horseshoe.NET:Cryptography:SymmetricPaddingMode")    // example "PKCS7"
                    ?? OrganizationalDefaultSettings.GetNullable<PaddingMode>("Cryptography.SymmetricPaddingMode")
                    ?? PaddingMode.None;
            }
            set
            {
                _defaultSymmetricPaddingMode = value;
            }
        }

        private static HashAlgorithm _defaultHashAlgorithm;

        /// <summary>
        /// Gets or sets the default hash algorithm used by Cryptography.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:Cryptography:HashAlgorithm and OrganizationalDefaultSettings: key = Cryptography.HashAlgorithm)
        /// </summary>
        public static HashAlgorithm DefaultHashAlgorithm
        {
            get
            {
                return _defaultHashAlgorithm  // example "System.Security.Cryptography.SHA256CryptoServiceProvider"
                    ?? Config.Get<HashAlgorithm>("Horseshoe.NET:Cryptography:HashAlgorithm")
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
        /// Gets or sets the default hash salt used by Cryptography.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:Cryptography:HashSalt and OrganizationalDefaultSettings: key = Cryptography.HashSalt)
        /// </summary>
        public static byte? DefaultHashSalt
        {
            get
            {
                return _defaultHashSalt
                    ?? Config.GetNByte("Horseshoe.NET:Cryptography:HashSalt")    // example: 240 or HashSalt[hex] F0
                    ?? OrganizationalDefaultSettings.GetNByte("Cryptography.HashSalt");
            }
            set
            {
                _defaultHashSalt = value;
            }
        }

        static Encoding _defaultEncoding;

        /// <summary>
        /// Gets or sets the text encoding used by Cryptography. Defaults to UTF8Encoding. Note: Override by passing directly to a Cryptography function or via config file: key = "Horseshoe.NET:Cryptography:Encoding"
        /// </summary>
        public static Encoding DefaultEncoding
        {
            get
            {
                return _defaultEncoding
                    ?? ObjectUtil.GetInstance<Encoding>(Config.Get("Horseshoe.NET:Cryptography:Encoding"), suppressErrors: true)   // example: "System.Text.UTF8Encoding"
                    ?? OrganizationalDefaultSettings.Get<Encoding>("Cryptography:Encoding")
                    ?? Encoding.Default;
            }
            set
            {
                _defaultEncoding = value;
            }
        }
    }
}
