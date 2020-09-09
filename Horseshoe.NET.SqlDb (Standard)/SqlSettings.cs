using System;
using System.Collections.Generic;

using Horseshoe.NET.Application;
using Horseshoe.NET.Db;
using Horseshoe.NET.SqlDb.Meta;

namespace Horseshoe.NET.SqlDb
{
    public static class SqlSettings
    {
        private static string _defaultConnectionString;
        private static bool _isEncryptedPassword;

        /// <summary>
        /// Gets the default SQL Server connection string used by DataAccess.  Note: Overrides other settings (i.e. OrganizationalDefaultSettings: key = SqlDb.ConnectionString)
        /// </summary>
        public static string DefaultConnectionString
        {
            get
            {
                return _GetConnectionString(_defaultConnectionString, _isEncryptedPassword)
                    ?? _GetConnectionString(OrganizationalDefaultSettings.GetString("Sql.ConnectionString"), OrganizationalDefaultSettings.GetBoolean("Sql.IsEncryptedPassword"));
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
        /// Gets or sets the default SQL Server instance used by DataAccess.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:Sql.Server and OrganizationalDefaultSettings: key = SqlDb.Server)
        /// </summary>
        public static DbServer DefaultServer
        {
            get
            {
                if (_defaultServer == null)
                {
                    _defaultServer =      // DBSVR01 (lookup / versionless) or 'NAME'11.22.33.44:9999;2012 or DBSVR02;2008R2
                        OrganizationalDefaultSettings.Get("Sql.Server", parseFunc: (raw) => DbServer.Parse((string)raw));
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
        /// Gets or sets the default SQL Server data source used by DataAccess.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:Sql.DataSource and OrganizationalDefaultSettings: key = SqlDb.DataSource)
        /// </summary>
        public static string DefaultDataSource
        {
            get
            {
                return _defaultDataSource         // e.g. DBSVR01
                    ?? OrganizationalDefaultSettings.GetString("Sql.DataSource")
                    ?? DefaultServer?.DataSource;
            }
            set
            {
                _defaultDataSource = value;
            }
        }

        private static string _defaultInitialCatalog;

        /// <summary>
        /// Gets or sets the default SQL Server initial catalog (database) used by DataAccess.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:Sql.InitialCatalog and OrganizationalDefaultSettings: key = SqlDb.InitialCatalog)
        /// </summary>
        public static string DefaultInitialCatalog
        {
            get
            {
                return _defaultInitialCatalog           // e.g. CustomerDatabase
                    ?? OrganizationalDefaultSettings.GetString("Sql.InitialCatalog");
            }
            set
            {
                _defaultInitialCatalog = value;
            }
        }

        private static Credential? _defaultCredentials;

        /// <summary>
        /// Gets or sets the default SQL Server credentials used by DataAccess.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:Sql.UserName|Password and OrganizationalDefaultSettings: key = SqlDb.Credentials)
        /// </summary>
        public static Credential? DefaultCredentials
        {
            get
            {
                return _defaultCredentials
                    ?? OrganizationalDefaultSettings.GetNullable<Credential>("Sql.Credentials");
            }
            set
            {
                _defaultCredentials = value;
            }
        }

        private static IDictionary<string, string> _defaultAdditionalConnectionAttributes;

        /// <summary>
        /// Gets or sets the default additional SQL Server connection attributes used by DataAccess.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:Sql.AdditionalConnectionAttributes and OrganizationalDefaultSettings: key = SqlDb.AdditionalConnectionAttributes)
        /// </summary>
        public static IDictionary<string, string> DefaultAdditionalConnectionAttributes
        {
            get
            {
                return _defaultAdditionalConnectionAttributes         // e.g. Integrated Security=SSQI|Attribute1=Value1
                    ?? OrganizationalDefaultSettings.Get("Sql.AdditionalConnectionAttributes", parseFunc: (raw) => DataUtil.ParseAdditionalConnectionAttributes((string)raw));
            }
            set
            {
                _defaultAdditionalConnectionAttributes = value;
            }
        }

        private static int? _defaultTimeout;

        /// <summary>
        /// Gets or sets the default SQL Server timeout used by DataAccess.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:Sql.Timeout and OrganizationalDefaultSettings: key = SqlDb.Timeout)
        /// </summary>
        public static int? DefaultTimeout
        {
            get
            {
                return _defaultTimeout         // e.g. 30 (Microsoft default)
                    ?? OrganizationalDefaultSettings.GetNInt("Sql.Timeout");
            }
            set
            {
                _defaultTimeout = value;
            }
        }

        private static IEnumerable<DbServer> _serverList;

        /// <summary>
        /// Gets or sets a list of SQL Servers for DbServer's Lookup() method.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:Sql.ServerList and OrganizationalDefaultSettings: key = SqlDb.ServerList)
        /// </summary>
        public static IEnumerable<DbServer> ServerList
        {
            get
            {
                if (_serverList == null)
                {
                    _serverList =          // e.g. DBSVR01|'NAME'11.22.33.44:9999;2012|DBSVR02;2008R2
                        OrganizationalDefaultSettings.Get("Sql.ServerList", parseFunc: (raw) => DbServer.ParseList((string)raw));
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
