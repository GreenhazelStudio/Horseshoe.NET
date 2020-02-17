using System;
using System.Text;

using Horseshoe.NET.Application;
using Horseshoe.NET.Cryptography;
using Horseshoe.NET.Db;
using Horseshoe.NET.Objects;
using Horseshoe.NET.Text;

namespace Horseshoe.NET.Odbc
{
    public class OdbcConnectionInfo : ConnectionInfo
    {
        /* Other members, see base class...
         * 
         * public string ConnectionStringName { get; set; }
         * public bool IsEncryptedPassword { get; set; }
         * public string DataSource { get; set; }
         * public Credential? Credentials { get; set; }
         * public IDictionary<string, string> AdditionalConnectionAttributes  { get; set; }
         */

        public OdbcConnectionInfo() { }

        public OdbcConnectionInfo(ConnectionInfo connectionInfo)
        {
            if (connectionInfo is OdbcConnectionInfo odbcConnectionInfo)
            {
                ObjectUtil.EasyMap(odbcConnectionInfo, this);
            }
            else if (connectionInfo != null)
            {
                ObjectUtil.EasyMap(connectionInfo, this);
            }
        }

        public string GetConnectionString(out string announcePrefix, CryptoOptions options = null)
        {
            if (ConnectionString != null)
            {
                announcePrefix = "CONNINFO CONNSTR";
                return ConnectionString;
            }

            if (ConnectionStringName != null)
            {
                announcePrefix = "CONNINFO CONNSTR('" + ConnectionStringName + "')";
                return Config.GetConnectionString(ConnectionStringName);
            }

            if (DataSource == null)
            {
                announcePrefix = "CONNINFO [null] DATASOURCE";
                return null;
            }

            announcePrefix = "CONNINFO DATASOURCE";

            // data source
            var sb = new StringBuilder("DSN=" + DataSource);

            // credentials
            if (Settings.DefaultCredentials.HasValue)
            {
                sb.Append(";UID=" + Credentials.Value.UserID);
                if (Credentials.Value.HasSecurePassword)
                {
                    sb.Append(";PWD=" + Credentials.Value.SecurePassword.ToUnsecureString());
                }
                else if (Credentials.Value.IsEncryptedPassword)
                {
                    sb.Append(";PWD=" + Decrypt.String(Credentials.Value.Password, options: options));
                }
                else if (Credentials.Value.Password != null)
                {
                    sb.Append(";PWD=" + Credentials.Value.Password);
                }
            }

            // additional attributes
            if (AdditionalConnectionAttributes != null)
            {
                foreach (var kvp in AdditionalConnectionAttributes)
                {
                    sb.Append(";" + kvp.Key + "=" + kvp.Value);
                }
            }

            return sb.ToString();
        }
    }
}
