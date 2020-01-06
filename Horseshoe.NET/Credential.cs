using System;
using System.Security;

namespace Horseshoe.NET
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1815:Override equals and operator equals on value types", Justification = "Should only be used to authenticate, never compared or sorted")]
    public struct Credential : IDisposable
    {
        public string UserID { get; }

        public string Password { get; }

        public bool IsEncryptedPassword { get; }

        public SecureString SecurePassword { get; }

        public bool HasSecurePassword => SecurePassword != null;

        public string Domain { get; }

        public Credential(string userID, string domain = null)
        {
            UserID = userID ?? throw new ArgumentNullException(nameof(userID));
            Password = null;
            IsEncryptedPassword = false;
            SecurePassword = null;
            Domain = domain;
        }

        public Credential(string userID, string password, bool isEncryptedPassword = false, string domain = null) : this(userID, domain: domain)
        {
            Password = password;
            IsEncryptedPassword = password != null && isEncryptedPassword;
        }

        public Credential(string userID, SecureString securePassword, string domain = null) : this(userID, domain: domain)
        {
            SecurePassword = securePassword ?? throw new ArgumentNullException(nameof(securePassword));
        }

        public static Credential? Build(string userID, string password, bool isEncryptedPassword = false, string domain = null)
        {
            if (userID == null) return null;
            return new Credential(userID, password, isEncryptedPassword: isEncryptedPassword, domain: domain);
        }

        public static Credential? Build(string userID, SecureString securePassword, string domain = null)
        {
            if (userID == null) return null;
            return new Credential(userID, securePassword, domain: domain);
        }

        public void Dispose()
        {
            SecurePassword?.Dispose();
        }
    }
}
