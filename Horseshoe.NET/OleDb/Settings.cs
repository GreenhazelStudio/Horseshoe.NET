using System;
using System.Collections.Generic;

using Horseshoe.NET.Application;
using Horseshoe.NET.Db;

namespace Horseshoe.NET.OleDb
{
    public static class Settings
    {
        static string _defaultConnectionStringName;

        /// <summary>
        /// Gets or sets the default OLEDB connection string name used by DataAccess.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:DataAccess.OLEDB:ConnectionStringName)
        /// </summary>
        public static string DefaultConnectionStringName
        {
            get
            {
                return _defaultConnectionStringName
                    ?? Config.Get("Horseshoe.NET:DataAccess.OLEDB:ConnectionStringName");
            }
            set
            {
                _defaultConnectionStringName = value;
            }
        }

        private static string _defaultConnectionString;
        private static bool _isEncryptedPassword;

        /// <summary>
        /// Gets the default OLEDB connection string used by DataAccess.  Note: Overrides other settings (i.e. OrganizationalDefaultSettings: key = DataAccess.OLEDB.ConnectionString)
        /// </summary>
        public static string DefaultConnectionString
        {
            get
            {
                return _GetConnectionString(_defaultConnectionString, _isEncryptedPassword)
                    ?? _GetConnectionString(Config.GetConnectionString(DefaultConnectionStringName, suppressErrors: true), Config.GetBoolean("Horseshoe.NET:DataAccess.OLEDB:IsEncryptedPassword"))
                    ?? _GetConnectionString(OrganizationalDefaultSettings.GetString("DataAccess.OLEDB.ConnectionString"), OrganizationalDefaultSettings.GetBoolean("DataAccess.OLEDB.IsEncryptedPassword"));
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
        /// Sets the default OLEDB connection string used by DataAccess. 
        /// </summary>
        public static void SetDefaultConnectionString(string connectionString, bool isEncryptedPassword = false)
        {
            _defaultConnectionString = connectionString;
            _isEncryptedPassword = isEncryptedPassword;
        }

        private static string _defaultDataSource;

        /// <summary>
        /// Gets or sets the default OLEDB data source used by DataAccess.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:DataAccess.OLEDB:DataSource and OrganizationalDefaultSettings: key = DataAccess.OLEDB.DataSource)
        /// </summary>
        public static string DefaultDataSource
        {
            get
            {
                return _defaultDataSource       // e.g. DBSVR01
                    ?? Config.Get("Horseshoe.NET:DataAccess.OLEDB:DataSource")
                    ?? OrganizationalDefaultSettings.GetString("DataAccess.OLEDB.DataSource");
            }
            set
            {
                _defaultDataSource = value;
            }
        }

        private static Credential? _defaultCredentials;

        /// <summary>
        /// Gets or sets the default OLEDB credentials used by DataAccess.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:DataAccess.OLEDB:UserName|Password and OrganizationalDefaultSettings: key = DataAccess.OLEDB.Credentials)
        /// </summary>
        public static Credential? DefaultCredentials
        {
            get
            {
                return _defaultCredentials
                    ?? Credential.Build
                    (
                        Config.Get("Horseshoe.NET:DataAccess.OLEDB:UserID"),
                        Config.Get("Horseshoe.NET:DataAccess.OLEDB:Password"),
                        isEncryptedPassword: Config.GetBoolean("Horseshoe.NET:DataAccess.OLEDB:IsEncryptedPassword")
                    )
                    ?? OrganizationalDefaultSettings.GetNullable<Credential>("DataAccess.OLEDB.Credentials");
            }
            set
            {
                _defaultCredentials = value;
            }
        }

        private static IDictionary<string, string> _defaultAdditionalConnectionAttributes;

        /// <summary>
        /// Gets or sets the default additional OLEDB connection attributes used by DataAccess.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:DataAccess.OLEDB:AdditionalConnectionAttributes and OrganizationalDefaultSettings: key = DataAccess.OLEDB.AdditionalConnectionAttributes)
        /// </summary>
        public static IDictionary<string, string> DefaultAdditionalConnectionAttributes
        {
            get
            {
                return _defaultAdditionalConnectionAttributes        // e.g. Integrated Security=SSQI|Attribute1=Value1
                    ?? Config.Get("Horseshoe.NET:DataAccess.OLEDB:AdditionalConnectionAttributes", parseFunc: (raw) => DataUtil.ParseAdditionalConnectionAttributes(raw))
                    ?? OrganizationalDefaultSettings.Get("DataAccess.OLEDB.AdditionalConnectionAttributes", parseFunc: (raw) => DataUtil.ParseAdditionalConnectionAttributes((string)raw));
            }
            set
            {
                _defaultAdditionalConnectionAttributes = value;
            }
        }

        private static int? _defaultTimeout;

        /// <summary>
        /// Gets or sets the default OLEDB timeout used by DataAccess.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:DataAccess.OLEDB:Timeout and OrganizationalDefaultSettings: key = DataAccess.OLEDB.Timeout)
        /// </summary>
        public static int? DefaultTimeout
        {
            get
            {
                return _defaultTimeout           // e.g. 30 (Microsoft default?)
                    ?? Config.GetNInt("Horseshoe.NET:DataAccess.OLEDB:Timeout")
                    ?? OrganizationalDefaultSettings.GetNInt("DataAccess.OLEDB.Timeout");
            }
            set
            {
                _defaultTimeout = value;
            }
        }
    }
}
