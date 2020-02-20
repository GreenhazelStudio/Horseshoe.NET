using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security;
using System.Text;

using Horseshoe.NET.Cryptography;
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
            if (SqlSettings.DefaultConnectionString != null)
            {
                announcePrefix = "CONFIG CONNSTR";
                if (SqlSettings.DefaultConnectionStringName != null)
                {
                    announcePrefix += "(" + SqlSettings.DefaultConnectionStringName + "?)";
                }
                return SqlSettings.DefaultConnectionString;
            }

            if (SqlSettings.DefaultDataSource != null)
            {
                if (SqlSettings.DefaultServer != null)
                {
                    announcePrefix = "CONFIG SERVER";
                }
                else
                {
                    announcePrefix = "CONFIG DATASOURCE";
                }

                // data source
                var sb = new StringBuilder("Data Source=" + SqlSettings.DefaultDataSource.Replace(":", ","));  // DBSVR02:9999 -> Data Source=DBSVR02,9999;...

                // initial catalog
                if (SqlSettings.DefaultInitialCatalog != null)
                {
                    sb.Append(";Initial Catalog=" + SqlSettings.DefaultInitialCatalog);
                }

                // integrated security
                if (!SqlSettings.DefaultCredentials.HasValue)
                {
                    sb.Append(";Integrated Security=true");
                }

                // additional attributes
                if (SqlSettings.DefaultAdditionalConnectionAttributes != null)
                {
                    foreach (var kvp in SqlSettings.DefaultAdditionalConnectionAttributes)
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
                credentials = SqlSettings.DefaultCredentials;
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
                    DataUtil.UsingCredentials?.Invoke(source, credentials.Value.UserName, passwordDescription);
                }
            }
            return credentials;
        }

        public static SqlConnection LaunchConnection(SqlConnectionInfo connectionInfo = null, CryptoOptions options = null)
        {
            var conn = new SqlConnection
            {
                ConnectionString = WhichConnectionString(connectionInfo: connectionInfo, announce: true) ?? throw new UtilityException("No connection string or data source was found"),
                Credential = GetSqlCredentials(WhichCredentials(connectionInfo: connectionInfo, announce: true), options: options)
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
            if ((timeout ?? SqlSettings.DefaultTimeout).HasValue)
            {
                cmd.CommandTimeout = (timeout ?? SqlSettings.DefaultTimeout).Value;
            }
            return cmd;
        }

        public static SqlCredential GetSqlCredentials(Credential? credentials, CryptoOptions options = null)
        {
            if (!credentials.HasValue)
            {
                return null;
            }

            if (credentials.Value.HasSecurePassword)
            {
                return new SqlCredential(credentials.Value.UserName, credentials.Value.SecurePassword);
            }

            if (credentials.Value.Password != null)
            {
                SecureString securePassword;
                if (credentials.Value.IsEncryptedPassword)
                {
                    securePassword = Decrypt.SecureString(credentials.Value.Password, options: options);
                }
                else
                {
                    securePassword = new SecureString();
                    foreach (char c in credentials.Value.Password)
                    {
                        securePassword.AppendChar(c);
                    }
                    securePassword.MakeReadOnly();
                }
                return new SqlCredential(credentials.Value.UserName, securePassword);
            }
            return new SqlCredential(credentials.Value.UserName, null);
        }
    }
}
