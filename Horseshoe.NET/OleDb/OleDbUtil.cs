using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Diagnostics.CodeAnalysis;
using System.Text;

using Horseshoe.NET.Crypto;
using Horseshoe.NET.Db;
using Horseshoe.NET.Text.Extensions;

namespace Horseshoe.NET.OleDb
{
    public static class OleDbUtil
    {
        public static event UsingConnectionString UsingConnectionString;
        public static event UsingCredentials UsingCredentials;
        public static UsingStatement UsingStatement;

        /* * * * * * * * * * * * * * * * * * * * * * * * 
         *   CONNECTION-RELATED METHODS                *
         * * * * * * * * * * * * * * * * * * * * * * * */
        public static string GetDefaultConnectionString(out string announcePrefix, CryptoOptions options = null)
        {
            if (OleDbSettings.DefaultConnectionString != null)
            {
                announcePrefix = "CONFIG CONNSTR";
                if (OleDbSettings.DefaultConnectionStringName != null)
                {
                    announcePrefix += "(" + OleDbSettings.DefaultConnectionStringName + "?)";
                }
                return OleDbSettings.DefaultConnectionString;
            }

            if (OleDbSettings.DefaultDataSource != null)
            {
                announcePrefix = "CONFIG DATASOURCE";

                // data source
                var sb = new StringBuilder("Data Source=" + OleDbSettings.DefaultDataSource);

                // credentials
                if (OleDbSettings.DefaultCredentials.HasValue)
                {
                    sb.Append(";User ID=" + OleDbSettings.DefaultCredentials.Value.UserName);
                    if (OleDbSettings.DefaultCredentials.Value.HasSecurePassword)
                    {
                        sb.Append(";Password=" + OleDbSettings.DefaultCredentials.Value.SecurePassword.ToUnsecureString());
                    }
                    else if (OleDbSettings.DefaultCredentials.Value.IsEncryptedPassword)
                    {
                        sb.Append(";Password=" + Decrypt.String(OleDbSettings.DefaultCredentials.Value.Password, options: options));
                    }
                    else if (OleDbSettings.DefaultCredentials.Value.Password != null)
                    {
                        sb.Append(";Password=" + OleDbSettings.DefaultCredentials.Value.Password);
                    }
                }

                // additional attributes
                if (OleDbSettings.DefaultAdditionalConnectionAttributes != null)
                {
                    foreach (var kvp in OleDbSettings.DefaultAdditionalConnectionAttributes)
                    {
                        sb.Append(";" + kvp.Key + "=" + kvp.Value);
                    }
                }

                return sb.ToString();
            }
            announcePrefix = "CONFIG [null] DATASOURCE";
            return null;
        }

        public static string WhichConnectionString(OleDbConnectionInfo connectionInfo = null, bool announce = false, CryptoOptions options = null)
        {
            string announcePrefix;
            string connectionString;
            if (connectionInfo != null)
            {
                connectionString = connectionInfo.GetConnectionString(out announcePrefix, options: options);
            }
            else
            {
                connectionString = GetDefaultConnectionString(out announcePrefix, options: options);
            }
            if (connectionString != null && announce)
            {
                UsingConnectionString?.Invoke(connectionString, source: announcePrefix);
            }
            return connectionString;
        }

        public static Credential? WhichCredentials(OleDbConnectionInfo connectionInfo = null, bool announce = false)
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
                credentials = OleDbSettings.DefaultCredentials;
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
                    UsingCredentials?.Invoke(credentials.Value.UserName, passwordDescription, source: source);
                }
            }
            return credentials;
        }

        public static OleDbConnection LaunchConnection(OleDbConnectionInfo connectionInfo = null, CryptoOptions options = null)
        {
            var conn = new OleDbConnection
            {
                ConnectionString = WhichConnectionString(connectionInfo: connectionInfo, announce: true, options: options) ?? throw new UtilityException("No connection string or data source was found")
            };
            conn.Open();
            return conn;
        }

        [SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "Data sources and queries are all developer-controlled")]
        internal static OleDbCommand BuildCommand
        (
            OleDbConnection conn,
            CommandType commandType,
            string commandText,
            IEnumerable<DbParameter> parameters = null,
            int? timeout = null
        )
        {
            var cmd = new OleDbCommand
            {
                Connection = conn,
                CommandType = commandType,
                CommandText = commandText
            };
            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    if (param is OleDbParameter)
                    {
                        cmd.Parameters.Add(param);
                    }
                    else
                    {
                        var oledbParam = new OleDbParameter(param.ParameterName, param.Value)
                        {
                            Direction = param.Direction,
                            Size = param.Size,
                            SourceColumn = param.SourceColumn,
                            SourceColumnNullMapping = param.SourceColumnNullMapping,
                            SourceVersion = param.SourceVersion,
                            IsNullable = param.IsNullable
                        };
                        if (param is Parameter uparam)
                        {
                            if (uparam.IsDbTypeSet)
                            {
                                oledbParam.DbType = uparam.DbType;
                            }
                        }
                        cmd.Parameters.Add(oledbParam);
                    }
                }
            }
            if ((timeout ?? OleDbSettings.DefaultTimeout).HasValue)
            {
                cmd.CommandTimeout = (timeout ?? OleDbSettings.DefaultTimeout).Value;
            }
            return cmd;
        }
    }
}
