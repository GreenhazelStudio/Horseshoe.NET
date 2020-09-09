using System;
using System.Security;
using System.Text;

using Horseshoe.NET.Application;
using Horseshoe.NET.Db;
using Horseshoe.NET.Objects;
using Horseshoe.NET.OracleDb.Meta;

using Oracle.ManagedDataAccess.Client;

namespace Horseshoe.NET.OracleDb
{
    public class OraConnectionInfo : ConnectionInfo
    {
        /* Defined in base class...
         * 
         * public string ConnectionString { get; set; }     
         * public string ConnectionStringName { get; set; }
         * public bool IsEncryptedPassword { get; set; }
         * public string DataSource { get; set; }
         * public Credential? Credentials { get; set; }
         * public IDictionary<string, string> AdditionalConnectionStringAttributes  { get; set; }
         */

        private OraServer _server;

        public OraServer Server
        {
            get { return _server; }
            set
            {
                _server = value;
                DataSource = _server.DataSource;
            }
        }

        public OraConnectionInfo() { }

        public OraConnectionInfo(ConnectionInfo connectionInfo)
        {
            if (connectionInfo is OraConnectionInfo oraConnectionInfo)
            {
                ObjectUtil.EasyMap(oraConnectionInfo, this);
            }
            else if (connectionInfo != null)
            {
                ObjectUtil.EasyMap(connectionInfo, this);
            }
        }

        public OraConnectionInfo(OraServer server, OraConnectionInfo connectionInfo) : this(connectionInfo)
        {
            Server = server;
        }

        public string GetConnectionString(out string announcePrefix)   // Uses EZ Connect style connection string
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

            if (Server != null)
            {
                announcePrefix = "CONNINFO SERVER";
            }
            else
            {
                announcePrefix = "CONNINFO DATASOURCE";
            }

            // data source
            var sb = new StringBuilder("Data Source=//" + DataSource);  // e.g. Server.DataSource (includes port)

            // server attributes
            if (Server != null)
            {
                if (Server.ServiceName != null || Server.InstanceName != null)
                {
                    sb.Append("/");
                    if (Server.ServiceName != null)
                    {
                        sb.Append(Server.ServiceName);
                    }
                    if (Server.InstanceName != null)
                    {
                        sb.Append("/" + Server.InstanceName);
                    }
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

        public OracleCredential GetOracleCredentials()
        {
            return OracleUtil.GetOracleCredentials(Credentials);
        }
    }
}