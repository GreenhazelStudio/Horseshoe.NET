using System;
using System.Collections.Generic;

using Horseshoe.NET.Application;
using Horseshoe.NET.Db;
using Horseshoe.NET.SqlDb.Meta;

namespace Horseshoe.NET.SqlDb
{
    public static class Settings
    {
        static string _defaultConnectionStringName;

        /// <summary>
        /// Gets or sets the default SQL Server connection string name used by DataAccess.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:DataAccess.SQL:ConnectionStringName)
        /// </summary>
        public static string DefaultConnectionStringName
        {
            get
            {
                return _defaultConnectionStringName
                    ?? Config.Get("Horseshoe.NET:DataAccess.SQL:ConnectionStringName");
            }
            set
            {
                _defaultConnectionStringName = value;
            }
        }

        private static string _defaultConnectionString;
        private static bool _isEncryptedPassword;

        /// <summary>
        /// Gets the default SQL Server connection string used by DataAccess.  Note: Overrides other settings (i.e. OrganizationalDefaultSettings: key = DataAccess.SQL.ConnectionString)
        /// </summary>
        public static string DefaultConnectionString
        {
            get
            {
                return _GetConnectionString(_defaultConnectionString, _isEncryptedPassword)
                    ?? _GetConnectionString(Config.GetConnectionString(DefaultConnectionStringName, suppressErrors: true), Config.GetBoolean("Horseshoe.NET:DataAccess.SQL:IsEncryptedPassword"))
                    ?? _GetConnectionString(OrganizationalDefaultSettings.GetString("DataAccess.SQL.ConnectionString"), OrganizationalDefaultSettings.GetBoolean("DataAccess.SQL.IsEncryptedPassword"));
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
        /// Sets the default SQL Server connection string used by DataAccess. 
        /// </summary>
        public static void SetDefaultConnectionString(string connectionString, bool isEncryptedPassword = false)
        {
            _defaultConnectionString = connectionString;
            _isEncryptedPassword = isEncryptedPassword;
        }

        private static DbServer _defaultServer;

        /// <summary>
        /// Gets or sets the default SQL Server instance used by DataAccess.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:DataAccess.SQL:Server and OrganizationalDefaultSettings: key = DataAccess.SQL.Server)
        /// </summary>
        public static DbServer DefaultServer
        {
            get
            {
                if (_defaultServer == null)
                {
                    _defaultServer =      // DBSVR01 (lookup / versionless) or 'NAME'11.22.33.44:9999;2012 or DBSVR02;2008R2
                        Config.Get("Horseshoe.NET:DataAccess.SQL:Server", parseFunc: (raw) => DbServer.Parse(raw)) ??
                        OrganizationalDefaultSettings.Get("DataAccess.SQL.Server", parseFunc: (raw) => DbServer.Parse((string)raw));
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
        /// Gets or sets the default SQL Server data source used by DataAccess.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:DataAccess.SQL:DataSource and OrganizationalDefaultSettings: key = DataAccess.SQL.DataSource)
        /// </summary>
        public static string DefaultDataSource
        {
            get
            {
                return _defaultDataSource         // e.g. DBSVR01
                    ?? Config.Get("Horseshoe.NET:DataAccess.SQL:DataSource")
                    ?? OrganizationalDefaultSettings.GetString("DataAccess.SQL.DataSource")
                    ?? DefaultServer?.DataSource;
            }
            set
            {
                _defaultDataSource = value;
            }
        }

        private static string _defaultInitialCatalog;

        /// <summary>
        /// Gets or sets the default SQL Server initial catalog (database) used by DataAccess.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:DataAccess.SQL:InitialCatalog and OrganizationalDefaultSettings: key = DataAccess.SQL.InitialCatalog)
        /// </summary>
        public static string DefaultInitialCatalog
        {
            get
            {
                return _defaultInitialCatalog           // e.g. CustomerDatabase
                    ?? Config.Get("Horseshoe.NET:DataAccess.SQL:InitialCatalog")
                    ?? OrganizationalDefaultSettings.GetString("DataAccess.SQL.InitialCatalog");
            }
            set
            {
                _defaultInitialCatalog = value;
            }
        }

        private static Credential? _defaultCredentials;

        /// <summary>
        /// Gets or sets the default SQL Server credentials used by DataAccess.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:DataAccess.SQL:UserName|Password and OrganizationalDefaultSettings: key = DataAccess.SQL.Credentials)
        /// </summary>
        public static Credential? DefaultCredentials
        {
            get
            {
                return _defaultCredentials
                    ?? Credential.Build
                    (
                        Config.Get("Horseshoe.NET:DataAccess.SQL:UserID"), 
                        Config.Get("Horseshoe.NET:DataAccess.SQL:Password"), 
                        isEncryptedPassword: Config.GetBoolean("Horseshoe.NET:DataAccess.SQL:IsEncryptedPassword")
                    )
                    ?? OrganizationalDefaultSettings.GetNullable<Credential>("DataAccess.SQL.Credentials");
            }
            set
            {
                _defaultCredentials = value;
            }
        }

        private static IDictionary<string, string> _defaultAdditionalConnectionAttributes;

        /// <summary>
        /// Gets or sets the default additional SQL Server connection attributes used by DataAccess.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:DataAccess.SQL:AdditionalConnectionAttributes and OrganizationalDefaultSettings: key = DataAccess.SQL.AdditionalConnectionAttributes)
        /// </summary>
        public static IDictionary<string, string> DefaultAdditionalConnectionAttributes
        {
            get
            {
                return _defaultAdditionalConnectionAttributes         // e.g. Integrated Security=SSQI|Attribute1=Value1
                    ?? Config.Get("Horseshoe.NET:DataAccess.SQL:AdditionalConnectionAttributes", parseFunc: (raw) => DataUtil.ParseAdditionalConnectionAttributes(raw))
                    ?? OrganizationalDefaultSettings.Get("DataAccess.SQL.AdditionalConnectionAttributes", parseFunc: (raw) => DataUtil.ParseAdditionalConnectionAttributes((string)raw));
            }
            set
            {
                _defaultAdditionalConnectionAttributes = value;
            }
        }

        private static int? _defaultTimeout;

        /// <summary>
        /// Gets or sets the default SQL Server timeout used by DataAccess.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:DataAccess.SQL:Timeout and OrganizationalDefaultSettings: key = DataAccess.SQL.Timeout)
        /// </summary>
        public static int? DefaultTimeout
        {
            get
            {
                return _defaultTimeout         // e.g. 30 (Microsoft default)
                    ?? Config.GetNInt("Horseshoe.NET:DataAccess.SQL:Timeout")
                    ?? OrganizationalDefaultSettings.GetNInt("DataAccess.SQL.Timeout");
            }
            set
            {
                _defaultTimeout = value;
            }
        }

        private static IEnumerable<DbServer> _serverList;

        /// <summary>
        /// Gets or sets a list of SQL Servers for DbServer's Lookup() method.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:DataAccess.SQL:ServerList and OrganizationalDefaultSettings: key = DataAccess.SQL.ServerList)
        /// </summary>
        public static IEnumerable<DbServer> ServerList
        {
            get
            {
                if (_serverList == null)
                {
                    _serverList =          // e.g. DBSVR01|'NAME'11.22.33.44:9999;2012|DBSVR02;2008R2
                        Config.Get("Horseshoe.NET:DataAccess.SQL:ServerList", parseFunc: (raw) => DbServer.ParseList(raw)) ??
                        OrganizationalDefaultSettings.Get("DataAccess.SQL.ServerList", parseFunc: (raw) => DbServer.ParseList((string)raw));
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
