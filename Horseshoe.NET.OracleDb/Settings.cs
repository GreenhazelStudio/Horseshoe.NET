using System;
using System.Collections.Generic;

using Horseshoe.NET.Application;
using Horseshoe.NET.Db;
using Horseshoe.NET.OracleDb.Meta;

namespace Horseshoe.NET.OracleDb
{
    public static class Settings
    {
        static string _defaultConnectionStringName;

        /// <summary>
        /// Gets or sets the default Oracle connection string name used by DataAccess.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:OracleDb:ConnectionStringName)
        /// </summary>
        public static string DefaultConnectionStringName
        {
            get
            {
                return _defaultConnectionStringName
                    ?? Config.Get("Horseshoe.NET:OracleDb:ConnectionStringName");
            }
            set
            {
                _defaultConnectionStringName = value;
            }
        }

        private static string _defaultConnectionString;
        private static bool _isEncryptedPassword;

        /// <summary>
        /// Gets the default Oracle connection string used by DataAccess.  Note: Overrides other settings (i.e. OrganizationalDefaultSettings: key = OracleDb.ConnectionString)
        /// </summary>
        public static string DefaultConnectionString
        {
            get
            {
                return _GetConnectionString(_defaultConnectionString, _isEncryptedPassword)
                    ?? _GetConnectionString(Config.GetConnectionString(DefaultConnectionStringName, suppressErrors: true), Config.GetBoolean("Horseshoe.NET:OracleDb:IsEncryptedPassword"))
                    ?? _GetConnectionString(OrganizationalDefaultSettings.GetString("OracleDb.ConnectionString"), OrganizationalDefaultSettings.GetBoolean("OracleDb.IsEncryptedPassword"));
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
        /// Sets the default Oracle connection string used by DataAccess. 
        /// </summary>
        public static void SetDefaultConnectionString(string connectionString, bool isEncryptedPassword = false)
        {
            _defaultConnectionString = connectionString;
            _isEncryptedPassword = isEncryptedPassword;
        }

        private static OraServer _defaultServer;

        /// <summary>
        /// Gets or sets the default Oracle server used by DataAccess.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:OracleDb:Server and OrganizationalDefaultSettings: key = OracleDb.Server)
        /// </summary>
        public static OraServer DefaultServer
        {
            get
            {
                if (_defaultServer == null)
                {
                    _defaultServer =      // e.g. ORADBSVR01 or 'NAME'11.22.33.44:9999;SERVICE1 or ORADBSVR02:9999;SERVICE1;INSTANCE1
                        Config.Get("Horseshoe.NET:OracleDb:Server", parseFunc: (raw) => OracleUtil.ParseServer(raw)) ?? 
                        OrganizationalDefaultSettings.Get("OracleDb.Server", parseFunc: (raw) => OracleUtil.ParseServer((string)raw)); 
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
        /// Gets or sets the default Oracle datasource used by DataAccess.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:OracleDb:DataSource and OrganizationalDefaultSettings: key = OracleDb.DataSource)
        /// </summary>
        public static string DefaultDataSource
        {
            get
            {
                return _defaultDataSource     // e.g. ORADBSVR01
                    ?? Config.Get("Horseshoe.NET:OracleDb:DataSource")  
                    ?? OrganizationalDefaultSettings.GetString("OracleDb.DataSource") 
                    ?? DefaultServer?.DataSource;
            }
            set
            {
                _defaultDataSource = value;
            }
        }

        private static Credential? _defaultCredentials;

        /// <summary>
        /// Gets or sets the default Oracle credentials used by DataAccess.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:OracleDb:UserID|Password and OrganizationalDefaultSettings: key = OracleDb.Credentials)
        /// </summary>
        public static Credential? DefaultCredentials
        {
            get
            {
                return _defaultCredentials
                    ?? Credential.Build(Config.Get("Horseshoe.NET:OracleDb:UserID"), Config.Get("Horseshoe.NET:OracleDb:Password"), isEncryptedPassword: Config.GetBoolean("Horseshoe.NET:OracleDb:IsEncryptedPassword"))
                    ?? OrganizationalDefaultSettings.GetNullable<Credential>("OracleDb.Credentials");
            }
            set
            {
                _defaultCredentials = value;
            }
        }

        private static IDictionary<string, string> _defaultAdditionalConnectionAttributes;

        /// <summary>
        /// Gets or sets the additional Oracle connection string attributes used by DataAccess.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:OracleDb:AdditionalConnectionAttributes and OrganizationalDefaultSettings: key = OracleDb.AdditionalConnectionAttributes)
        /// </summary>
        public static IDictionary<string, string> DefaultAdditionalConnectionAttributes
        {
            get
            {
                return _defaultAdditionalConnectionAttributes         // e.g. Integrated Security=SSQI|Attribute1=Value1
                    ?? Config.Get("Horseshoe.NET:OracleDb:AdditionalConnectionAttributes", parseFunc: (raw) => DataUtil.ParseAdditionalConnectionAttributes(raw))
                    ?? OrganizationalDefaultSettings.Get("OracleDb.AdditionalConnectionAttributes", parseFunc: (raw) => DataUtil.ParseAdditionalConnectionAttributes((string)raw));
            }
            set
            {
                _defaultAdditionalConnectionAttributes = value;
            }
        }

        private static int? _defaultTimeout;

        /// <summary>
        /// Gets or sets the Oracle timeout used by DataAccess.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:OracleDb:Timeout and OrganizationalDefaultSettings: key = OracleDb.Timeout)
        /// </summary>
        public static int? DefaultTimeout
        {
            get
            {
                return _defaultTimeout
                    ?? Config.GetNInt("Horseshoe.NET:OracleDb:Timeout")
                    ?? OrganizationalDefaultSettings.GetNInt("OracleDb.Timeout"); 
            }
            set
            {
                _defaultTimeout = value;
            }
        }

        private static IEnumerable<OraServer> _serverList;

        /// <summary>
        /// Gets or sets a list of Oracle servers for OraServer's Lookup() method.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:OracleDb:ServerList and OrganizationalDefaultSettings: key = OracleDb.ServerList)
        /// </summary>
        public static IEnumerable<OraServer> ServerList
        {
            get
            {
                if (_serverList == null)
                {
                    _serverList =      // e.g. ORADBSVR01|'NAME'11.22.33.44:9999;SERVICE1|ORADBSVR02:9999;SERVICE1;INSTANCE1
                        Config.Get("Horseshoe.NET:OracleDb:ServerList", parseFunc: (raw) => OracleUtil.ParseServerList(raw)) ??  
                        OrganizationalDefaultSettings.Get("OracleDb.ServerList", parseFunc: (raw) => OracleUtil.ParseServerList((string)raw));
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
