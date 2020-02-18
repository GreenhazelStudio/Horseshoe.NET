using System;
using System.Text;

using Horseshoe.NET.Application;
using Horseshoe.NET.Cryptography;
using Horseshoe.NET.Db;
using Horseshoe.NET.Objects;
using Horseshoe.NET.Text;

namespace Horseshoe.NET.OleDb
{
    public class OleDbConnectionInfo : ConnectionInfo
    {
        /* Other members, see base class...
         * 
         * public string ConnectionStringName { get; set; }
         * public bool IsEncryptedPassword { get; set; }
         * public string DataSource { get; set; }
         * public Credential? Credentials { get; set; }
         * public IDictionary<string, string> AdditionalConnectionAttributes  { get; set; }
         */

        public OleDbConnectionInfo() { }

        public OleDbConnectionInfo(ConnectionInfo connectionInfo)
        {
            if (connectionInfo is OleDbConnectionInfo oleDbConnectionInfo)
            {
                ObjectUtil.EasyMap(oleDbConnectionInfo, this);
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
            var sb = new StringBuilder("Data Source=" + DataSource);

            // credentials
            if (Settings.DefaultCredentials.HasValue)
            {
                sb.Append(";User ID=" + Credentials.Value.UserName);
                if (Credentials.Value.HasSecurePassword)
                {
                    sb.Append(";Password=" + Credentials.Value.SecurePassword.ToUnsecureString());
                }
                else if (Credentials.Value.IsEncryptedPassword)
                {
                    sb.Append(";Password=" + Decrypt.String(Credentials.Value.Password, options: options));
                }
                else if (Credentials.Value.Password != null)
                {
                    sb.Append(";Password=" + Credentials.Value.Password);
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
