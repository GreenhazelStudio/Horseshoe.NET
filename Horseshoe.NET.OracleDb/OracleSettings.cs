using System;
using System.Collections.Generic;

using Horseshoe.NET.Application;
using Horseshoe.NET.Db;
using Horseshoe.NET.OracleDb.Meta;

namespace Horseshoe.NET.OracleDb
{
    public static class OracleSettings
    {
        static string _defaultConnectionStringName;

        /// <summary>
        /// Gets or sets the default Oracle connection string name used by Horseshoe.NET.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:Oracle.ConnectionStringName)
        /// </summary>
        public static string DefaultConnectionStringName
        {
            get
            {
                return _defaultConnectionStringName
                    ?? Config.Get("Horseshoe.NET:Oracle.ConnectionStringName");
            }
            set
            {
                _defaultConnectionStringName = value;
            }
        }

        private static string _defaultConnectionString;
        private static bool _isEncryptedPassword;

        /// <summary>
        /// Gets the default Oracle connection string used by Horseshoe.NET.  Note: Overrides other settings (i.e. OrganizationalDefaultSettings: key = Oracle.ConnectionString)
        /// </summary>
        public static string DefaultConnectionString
        {
            get
            {
                return _GetConnectionString(_defaultConnectionString, _isEncryptedPassword)
                    ?? _GetConnectionString(Config.GetConnectionString(DefaultConnectionStringName, suppressErrors: true), Config.GetBool("Horseshoe.NET:Oracle.IsEncryptedPassword"))
                    ?? _GetConnectionString(OrganizationalDefaultSettings.GetString("Oracle.ConnectionString"), OrganizationalDefaultSettings.GetBoolean("Oracle.IsEncryptedPassword"));
            }
        }

        private static string _GetConnectionString(string connectionString, bool isEncryptedPassword)
        {
            if (connectionString == null) return null;
            return isEncryptedPassword
                ? DataUtil.DecryptInlinePassword(connectionString)
                : connectionString;
        }

        /// <summary>
        /// Sets the default Oracle connection string used by Horseshoe.NET. 
        /// </summary>
        public static void SetDefaultConnectionString(string connectionString, bool isEncryptedPassword = false)
        {
            _defaultConnectionString = connectionString;
            _isEncryptedPassword = isEncryptedPassword;
        }

        private static OraServer _defaultServer;

        /// <summary>
        /// Gets or sets the default Oracle server used by Horseshoe.NET.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:Oracle.Server and OrganizationalDefaultSettings: key = Oracle.Server)
        /// </summary>
        public static OraServer DefaultServer
        {
            get
            {
                if (_defaultServer == null)
                {
                    _defaultServer =      // e.g. ORADBSVR01 or 'NAME'11.22.33.44:9999;SERVICE1 or ORADBSVR02:9999;SERVICE1;INSTANCE1
                        Config.Get("Horseshoe.NET:Oracle.Server", parseFunc: (raw) => OracleUtil.ParseServer(raw)) ?? 
                        OrganizationalDefaultSettings.Get("Oracle.Server", parseFunc: (raw) => OracleUtil.ParseServer((string)raw)); 
                }
                return _defaultServer;
            }
            set
            {
                _defaultServer = value;
            }
        }

        private static string _defaultDataSource;

        /// <summary>
        /// Gets or sets the default Oracle datasource used by Horseshoe.NET.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:Oracle.DataSource and OrganizationalDefaultSettings: key = Oracle.DataSource)
        /// </summary>
        public static string DefaultDataSource
        {
            get
            {
                return _defaultDataSource     // e.g. ORADBSVR01
                    ?? Config.Get("Horseshoe.NET:Oracle.DataSource")  
                    ?? OrganizationalDefaultSettings.GetString("Oracle.DataSource") 
                    ?? DefaultServer?.DataSource;
            }
            set
            {
                _defaultDataSource = value;
            }
        }

        private static Credential? _defaultCredentials;

        /// <summary>
        /// Gets or sets the default Oracle credentials used by Horseshoe.NET.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:Oracle.UserID|Password and OrganizationalDefaultSettings: key = Oracle.Credentials)
        /// </summary>
        public static Credential? DefaultCredentials
        {
            get
            {
                return _defaultCredentials
                    ?? Credential.Build(Config.Get("Horseshoe.NET:Oracle.UserID"), Config.Get("Horseshoe.NET:Oracle.Password"), isEncryptedPassword: Config.GetBool("Horseshoe.NET:Oracle.IsEncryptedPassword"))
                    ?? OrganizationalDefaultSettings.GetNullable<Credential>("Oracle.Credentials");
            }
            set
            {
                _defaultCredentials = value;
            }
        }

        private static IDictionary<string, string> _defaultAdditionalConnectionAttributes;

        /// <summary>
        /// Gets or sets the additional Oracle connection string attributes used by Horseshoe.NET.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:Oracle.AdditionalConnectionAttributes and OrganizationalDefaultSettings: key = Oracle.AdditionalConnectionAttributes)
        /// </summary>
        public static IDictionary<string, string> DefaultAdditionalConnectionAttributes
        {
            get
            {
                return _defaultAdditionalConnectionAttributes         // e.g. Integrated Security=SSQI|Attribute1=Value1
                    ?? Config.Get("Horseshoe.NET:Oracle.AdditionalConnectionAttributes", parseFunc: (raw) => DataUtil.ParseAdditionalConnectionAttributes(raw))
                    ?? OrganizationalDefaultSettings.Get("Oracle.AdditionalConnectionAttributes", parseFunc: (raw) => DataUtil.ParseAdditionalConnectionAttributes((string)raw));
            }
            set
            {
                _defaultAdditionalConnectionAttributes = value;
            }
        }

        private static int? _defaultTimeout;

        /// <summary>
        /// Gets or sets the Oracle timeout used by Horseshoe.NET.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:Oracle.Timeout and OrganizationalDefaultSettings: key = Oracle.Timeout)
        /// </summary>
        public static int? DefaultTimeout
        {
            get
            {
                return _defaultTimeout
                    ?? Config.GetNInt("Horseshoe.NET:Oracle.Timeout")
                    ?? OrganizationalDefaultSettings.GetNInt("Oracle.Timeout");
            }
            set
            {
                _defaultTimeout = value;
            }
        }

        private static bool? _defaultAutoClearConnectionPool;

        /// <summary>
        /// Gets or sets whether Horseshoe.NET instructs connections to clear their pool upon closing.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:Oracle.AutoClearConnectionPool and OrganizationalDefaultSettings: key = Oracle.AutoClearConnectionPool)
        /// </summary>
        public static bool AutoClearConnectionPool
        {
            // ref: https://stackoverflow.com/questions/54373754/oracle-managed-dataaccess-connection-object-is-keeping-the-connection-open
            get
            {
                return _defaultAutoClearConnectionPool
                    ?? Config.GetNBool("Horseshoe.NET:Oracle.AutoClearConnectionPool")
                    ?? OrganizationalDefaultSettings.GetNBoolean("Oracle.AutoClearConnectionPool")
                    ?? false;
            }
            set
            {
                _defaultAutoClearConnectionPool = value;
            }
        }

        private static IEnumerable<OraServer> _serverList;

        /// <summary>
        /// Gets or sets a list of Oracle servers for OraServer's Lookup() method.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:Oracle.ServerList and OrganizationalDefaultSettings: key = Oracle.ServerList)
        /// </summary>
        public static IEnumerable<OraServer> ServerList
        {
            get
            {
                if (_serverList == null)
                {
                    _serverList =      // e.g. ORADBSVR01|'NAME'11.22.33.44:9999;SERVICE1|ORADBSVR02:9999;SERVICE1;INSTANCE1
                        Config.Get("Horseshoe.NET:Oracle.ServerList", parseFunc: (raw) => OracleUtil.ParseServerList(raw)) ??  
                        OrganizationalDefaultSettings.Get("Oracle.ServerList", parseFunc: (raw) => OracleUtil.ParseServerList((string)raw));
                }
                return _serverList;
            }
            set
            {
                _serverList = value;
            }
        }
    }
}
