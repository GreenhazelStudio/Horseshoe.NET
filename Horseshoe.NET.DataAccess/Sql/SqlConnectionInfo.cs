using System;
using System.Data.SqlClient;
using System.Text;

using Horseshoe.NET.Application;
using Horseshoe.NET.DataAccess.Sql.Meta;
using Horseshoe.NET.Objects;

namespace Horseshoe.NET.DataAccess.Sql
{
    public class SqlConnectionInfo : ConnectionInfo
    {
        /* Other members, see base class...
         * 
         * public string ConnectionStringName { get; set; }
         * public bool IsEncryptedPassword { get; set; }
         * public string DataSource { get; set; }
         * public Credential? Credentials { get; set; }
         * public IDictionary<string, string> AdditionalConnectionAttributes  { get; set; }
         */

        private DbServer _server;

        public DbServer Server
        {
            get { return _server; }
            set
            {
                _server = value;
                DataSource = _server?.DataSource;
            }
        }

        public string InitialCatalog { get; set; }

        public SqlConnectionInfo() { }

        public SqlConnectionInfo(ConnectionInfo connectionInfo)
        {
            if (connectionInfo is SqlConnectionInfo sqlConnectionInfo)
            {
                ObjectUtil.EasyMap(sqlConnectionInfo, this);
            }
            else if (connectionInfo != null)
            {
                ObjectUtil.EasyMap(connectionInfo, this);
            }
        }

        public SqlConnectionInfo(DbServer server, SqlConnectionInfo connectionInfo) : this(connectionInfo)
        {
            Server = server;
        }

        public string GetConnectionString(out string announcePrefix)
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
            var sb = new StringBuilder("Data Source=" + DataSource.Replace(":", ","));  // DBSVR02:9999 -> Data Source=DBSVR02,9999;...

            // initial catalog
            if (InitialCatalog != null)
            {
                sb.Append(";Initial Catalog=" + InitialCatalog);
            }

            // integrated security
            if (!Credentials.HasValue)
            {
                sb.Append(";Integrated Security=true");
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
