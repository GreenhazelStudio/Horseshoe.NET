using System;
using System.Collections.Generic;

using Horseshoe.NET.Application;
using Horseshoe.NET.Db;

namespace Horseshoe.NET.Odbc
{
    public static class Settings
    {
        static string _defaultConnectionStringName;

        /// <summary>
        /// Gets or sets the default ODBC connection string name used by DataAccess.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:DataAccess.ODBC:ConnectionStringName)
        /// </summary>
        public static string DefaultConnectionStringName
        {
            get
            {
                return _defaultConnectionStringName
                    ?? Config.Get("Horseshoe.NET:DataAccess.ODBC:ConnectionStringName");
            }
            set
            {
                _defaultConnectionStringName = value;
            }
        }

        private static string _defaultConnectionString;
        private static bool _isEncryptedPassword;

        /// <summary>
        /// Gets the default ODBC connection string used by DataAccess.  Note: Overrides other settings (i.e. OrganizationalDefaultSettings: key = DataAccess.ODBC.ConnectionString)
        /// </summary>
        public static string DefaultConnectionString
        {
            get
            {
                return _GetConnectionString(_defaultConnectionString, _isEncryptedPassword)
                    ?? _GetConnectionString(Config.GetConnectionString(DefaultConnectionStringName, suppressErrors: true), Config.GetBoolean("Horseshoe.NET:DataAccess.ODBC:IsEncryptedPassword"))
                    ?? _GetConnectionString(OrganizationalDefaultSettings.GetString("DataAccess.ODBC.ConnectionString"), OrganizationalDefaultSettings.GetBoolean("DataAccess.ODBC.IsEncryptedPassword"));
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
        /// Sets the default ODBC connection string used by DataAccess. 
        /// </summary>
        public static void SetDefaultConnectionString(string connectionString, bool isEncryptedPassword = false)
        {
            _defaultConnectionString = connectionString;
            _isEncryptedPassword = isEncryptedPassword;
        }

        private static string _defaultDataSource;

        /// <summary>
        /// Gets or sets the default ODBC data source used by DataAccess.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:DataAccess.ODBC:DataSource and OrganizationalDefaultSettings: key = DataAccess.ODBC.DataSource)
        /// </summary>
        public static string DefaultDataSource
        {
            get
            {
                return _defaultDataSource     // e.g. DBSVR01
                    ?? Config.Get("Horseshoe.NET:DataAccess.ODBC:DataSource") 
                    ?? OrganizationalDefaultSettings.GetString("DataAccess.ODBC.DataSource");
            }
            set
            {
                _defaultDataSource = value;
            }
        }

        private static Credential? _defaultCredentials;

        /// <summary>
        /// Gets or sets the default ODBC credentials used by DataAccess.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:DataAccess.ODBC:UserName|Password and OrganizationalDefaultSettings: key = DataAccess.ODBC.Credentials)
        /// </summary>
        public static Credential? DefaultCredentials
        {
            get
            {
                return _defaultCredentials
                    ?? Credential.Build
                    (
                        Config.Get("Horseshoe.NET:DataAccess.ODBC:UserID"),
                        Config.Get("Horseshoe.NET:DataAccess.ODBC:Password"),
                        isEncryptedPassword: Config.GetBoolean("Horseshoe.NET:DataAccess.ODBC:IsEncryptedPassword")
                    )
                    ?? OrganizationalDefaultSettings.GetNullable<Credential>("DataAccess.ODBC.Credentials");
            }
            set
            {
                _defaultCredentials = value;
            }
        }

        private static IDictionary<string, string> _defaultAdditionalConnectionAttributes;

        /// <summary>
        /// Gets or sets the default additional ODBC connection attributes used by DataAccess.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:DataAccess.ODBC:AdditionalConnectionAttributes and OrganizationalDefaultSettings: key = DataAccess.ODBC.AdditionalConnectionAttributes)
        /// </summary>
        public static IDictionary<string, string> DefaultAdditionalConnectionAttributes
        {
            get
            {
                return _defaultAdditionalConnectionAttributes      // e.g. Integrated Security=SSQI|Attribute1=Value1
                    ?? Config.Get("Horseshoe.NET:DataAccess.ODBC:AdditionalConnectionAttributes", parseFunc: (raw) => DataUtil.ParseAdditionalConnectionAttributes(raw)) 
                    ?? OrganizationalDefaultSettings.Get("DataAccess.ODBC.AdditionalConnectionAttributes", parseFunc: (raw) => DataUtil.ParseAdditionalConnectionAttributes((string)raw));
            }
            set
            {
                _defaultAdditionalConnectionAttributes = value;
            }
        }

        private static int? _defaultTimeout;

        /// <summary>
        /// Gets or sets the default ODBC timeout used by DataAccess.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:DataAccess.ODBC:Timeout and OrganizationalDefaultSettings: key = DataAccess.ODBC.Timeout)
        /// </summary>
        public static int? DefaultTimeout
        {
            get
            {
                return _defaultTimeout       // e.g. 30 (Microsoft default?)
                    ?? Config.GetNInt("Horseshoe.NET:DataAccess.ODBC:Timeout")
                    ?? OrganizationalDefaultSettings.GetNInt("DataAccess.ODBC.Timeout");
            }
            set
            {
                _defaultTimeout = value;
            }
        }
    }
}
