﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security;
using System.Text;

using Horseshoe.NET.Db;

namespace Horseshoe.NET.SqlDb
{
    public static class SqlUtil
    {
        /* * * * * * * * * * * * * * * * * * * * * * * * 
         *   CONNECTION-RELATED METHODS                *
         * * * * * * * * * * * * * * * * * * * * * * * */
        public static string GetDefaultConnectionString(out string announcePrefix)
        {
            if (Settings.DefaultConnectionString != null)
            {
                announcePrefix = "CONFIG CONNSTR";
                if (Settings.DefaultConnectionStringName != null)
                {
                    announcePrefix += "(" + Settings.DefaultConnectionStringName + "?)";
                }
                return Settings.DefaultConnectionString;
            }

            if (Settings.DefaultDataSource != null)
            {
                if (Settings.DefaultServer != null)
                {
                    announcePrefix = "CONFIG SERVER";
                }
                else
                {
                    announcePrefix = "CONFIG DATASOURCE";
                }

                // data source
                var sb = new StringBuilder("Data Source=" + Settings.DefaultDataSource.Replace(":", ","));  // DBSVR02:9999 -> Data Source=DBSVR02,9999;...

                // initial catalog
                if (Settings.DefaultInitialCatalog != null)
                {
                    sb.Append(";Initial Catalog=" + Settings.DefaultInitialCatalog);
                }

                // integrated security
                if (!Settings.DefaultCredentials.HasValue)
                {
                    sb.Append(";Integrated Security=true");
                }

                // additional attributes
                if (Settings.DefaultAdditionalConnectionAttributes != null)
                {
                    foreach (var kvp in Settings.DefaultAdditionalConnectionAttributes)
                    {
                        sb.Append(";" + kvp.Key + "=" + kvp.Value);
                    }
                }

                return sb.ToString();
            }
            announcePrefix = "CONFIG [null] DATASOURCE";
            return null;
        }

        public static string WhichConnectionString(SqlConnectionInfo connectionInfo = null, bool announce = false)
        {
            string announcePrefix;
            string connectionString;
            if (connectionInfo != null)
            {
                connectionString = connectionInfo.GetConnectionString(out announcePrefix);
            }
            else
            {
                connectionString = GetDefaultConnectionString(out announcePrefix);
            }
            if (connectionString != null && announce)
            {
                DataUtil.UsingConnectionString?.Invoke(announcePrefix, connectionString);
            }
            return connectionString;
        }

        public static Credential? WhichCredentials(SqlConnectionInfo connectionInfo = null, bool announce = false)
        {
            string source;
            Credential? credentials;
            string passwordDescription;

            if (connectionInfo != null)
            {
                source = "CONNINFO";
                credentials = connectionInfo.Credentials;
            }
            else
            {
                source = "CONFIG";
                credentials = Settings.DefaultCredentials;
            }

            if (credentials.HasValue)
            {
                if (credentials.Value.HasSecurePassword)
                {
                    passwordDescription = "secure-password";
                }
                else if (credentials.Value.IsEncryptedPassword)
                {
                    passwordDescription = "password-decrypted-secure";
                }
                else if (credentials.Value.Password != null)
                {
                    passwordDescription = "password-secured*";
                }
                else
                {
                    passwordDescription = "no-password";
                }
                if (announce)
                {
                    DataUtil.UsingCredentials?.Invoke(source, credentials.Value.UserID, passwordDescription);
                }
            }
            return credentials;
        }

        public static SqlConnection LaunchConnection(SqlConnectionInfo connectionInfo = null)
        {
            var conn = new SqlConnection
            {
                ConnectionString = WhichConnectionString(connectionInfo: connectionInfo, announce: true) ?? throw new UtilityException("No connection string or data source was found"),
                Credential = GetSqlCredentials(WhichCredentials(connectionInfo: connectionInfo, announce: true))
            };
            conn.Open();
            return conn;
        }

        [SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "Client code is responsible for statement and method parameter sterilization")]
        internal static SqlCommand BuildCommand
        (
            SqlConnection conn,
            CommandType commandType,
            string commandText,
            IEnumerable<DbParameter> parameters = null,
            int? timeout = null
        )
        {
            var cmd = new SqlCommand
            {
                Connection = conn,
                CommandType = commandType,
                CommandText = commandText
            };
            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    if (param is SqlParameter && param.Value != null)
                    {
                        cmd.Parameters.Add(param);
                    }
                    else
                    {
                        var sqlParam = new SqlParameter(param.ParameterName, param.Value)
                        {
                            //DbType = param.DbType,
                            Direction = param.Direction,
                            IsNullable = param.IsNullable,
                            Size = param.Size,
                            SourceColumn = param.SourceColumn,
                            SourceColumnNullMapping = param.SourceColumnNullMapping,
                            SourceVersion = param.SourceVersion
                        };
                        if (param is Parameter uparam)
                        {
                            if (uparam.IsDbTypeSet)
                            {
                                sqlParam.DbType = uparam.DbType;
                            }
                        }
                        cmd.Parameters.Add(sqlParam);
                    }
                }
            }
            if ((timeout ?? Settings.DefaultTimeout).HasValue)
            {
                cmd.CommandTimeout = (timeout ?? Settings.DefaultTimeout).Value;
            }
            return cmd;
        }

        public static SqlCredential GetSqlCredentials(Credential? credentials)
        {
            if (!credentials.HasValue)
            {
                return null;
            }

            if (credentials.Value.HasSecurePassword)
            {
                return new SqlCredential(credentials.Value.UserID, credentials.Value.SecurePassword);
            }

            if (credentials.Value.Password != null)
            {
                SecureString securePassword;
                if (credentials.Value.IsEncryptedPassword)
                {
                    securePassword = DataUtil.DecryptSecure(credentials.Value.Password);
                }
                else
                {
                    var nonsecurePassword = DataUtil.Decrypt(credentials.Value.Password);
                    securePassword = new SecureString();
                    foreach (char c in nonsecurePassword)
                    {
                        securePassword.AppendChar(c);
                    }
                    securePassword.MakeReadOnly();
                }
                return new SqlCredential(credentials.Value.UserID, securePassword);
            }
            return new SqlCredential(credentials.Value.UserID, null);
        }
    }
}