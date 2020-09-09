using System;
using System.Security;

namespace Horseshoe.NET
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1815:Override equals and operator equals on value types", Justification = "Should only be used to authenticate, never compared or sorted")]
    public struct Credential : IDisposable
    {
        public string UserName { get; }

        public string Password { get; }

        public bool IsEncryptedPassword { get; }

        public SecureString SecurePassword { get; }

        public bool HasSecurePassword => SecurePassword != null;

        public string Domain { get; }

        private Credential(string userName, string domain = null)
        {
            UserName = userName ?? throw new ArgumentNullException(nameof(userName));
            Password = null;
            IsEncryptedPassword = false;
            SecurePassword = null;
            Domain = domain;
        }

        public Credential(string userName, string password, bool isEncryptedPassword = false, string domain = null) : this(userName, domain: domain)
        {
            Password = password;
            IsEncryptedPassword = password != null && isEncryptedPassword;
        }

        public Credential(string userID, SecureString securePassword, string domain = null) : this(userID, domain: domain)
        {
            SecurePassword = securePassword ?? throw new ArgumentNullException(nameof(securePassword));
        }

        public static Credential? Build(string userName, string password, bool isEncryptedPassword = false, string domain = null)
        {
            if (userName == null) return null;
            return new Credential(userName, password, isEncryptedPassword: isEncryptedPassword, domain: domain);
        }

        public static Credential? Build(string userName, SecureString securePassword, string domain = null)
        {
            if (userName == null) return null;
            return new Credential(userName, securePassword, domain: domain);
        }

        public void Dispose()
        {
            SecurePassword?.Dispose();
        }
    }
}
