using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using System.Diagnostics.CodeAnalysis;
using System.Text;

using Horseshoe.NET.Crypto;
using Horseshoe.NET.Db;
using Horseshoe.NET.Text;

namespace Horseshoe.NET.Odbc
{
    public static class OdbcUtil
    {
        /* * * * * * * * * * * * * * * * * * * * * * * * 
         *   CONNECTION-RELATED METHODS                *
         * * * * * * * * * * * * * * * * * * * * * * * */
        public static string GetDefaultConnectionString(out string announcePrefix, CryptoOptions options = null)
        {
            if (OdbcSettings.DefaultConnectionString != null)
            {
                announcePrefix = "CONFIG CONNSTR";
                if (OdbcSettings.DefaultConnectionStringName != null)
                {
                    announcePrefix += "(" + OdbcSettings.DefaultConnectionStringName + "?)";
                }
                return OdbcSettings.DefaultConnectionString;
            }

            if (OdbcSettings.DefaultDataSource != null)
            {
                announcePrefix = "CONFIG DATASOURCE";

                // data source
                var sb = new StringBuilder("DSN=" + OdbcSettings.DefaultDataSource);

                // credentials
                if (OdbcSettings.DefaultCredentials.HasValue)
                {
                    sb.Append(";UID=" + OdbcSettings.DefaultCredentials.Value.UserName);
                    if (OdbcSettings.DefaultCredentials.Value.HasSecurePassword)
                    {
                        sb.Append(";PWD=" + OdbcSettings.DefaultCredentials.Value.SecurePassword.ToUnsecureString());
                    }
                    else if (OdbcSettings.DefaultCredentials.Value.IsEncryptedPassword)
                    {
                        sb.Append(";PWD=" + Decrypt.String(OdbcSettings.DefaultCredentials.Value.Password, options: options));
                    }
                    else if (OdbcSettings.DefaultCredentials.Value.Password != null)
                    {
                        sb.Append(";PWD=" + OdbcSettings.DefaultCredentials.Value.Password);
                    }
                }

                // additional attributes
                if (OdbcSettings.DefaultAdditionalConnectionAttributes != null)
                {
                    foreach (var kvp in OdbcSettings.DefaultAdditionalConnectionAttributes)
                    {
                        sb.Append(";" + kvp.Key + "=" + kvp.Value);
                    }
                }

                return sb.ToString();
            }
            announcePrefix = "CONFIG [null] DATASOURCE";
            return null;
        }

        public static string WhichConnectionString(OdbcConnectionInfo connectionInfo = null, bool announce = false, CryptoOptions options = null)
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
                DataUtil.UsingConnectionString?.Invoke(announcePrefix, connectionString);
            }
            return connectionString;
        }

        public static Credential? WhichCredentials(OdbcConnectionInfo connectionInfo = null, bool announce = false)
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
                credentials = OdbcSettings.DefaultCredentials;
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

        public static OdbcConnection LaunchConnection(OdbcConnectionInfo connectionInfo, CryptoOptions options = null)
        {
            var conn = new OdbcConnection
            {
                ConnectionString = WhichConnectionString(connectionInfo: connectionInfo, announce: true, options: options) ?? throw new UtilityException("No connection string or data source was found")
            };
            conn.Open();
            return conn;
        }

        [SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "Data sources and queries are all developer-controlled")]
        internal static OdbcCommand BuildCommand
        (
            OdbcConnection conn,
            CommandType commandType,
            string commandText,
            IEnumerable<DbParameter> parameters = null,
            int? timeout = null
        )
        {
            var cmd = new OdbcCommand
            {
                Connection = conn,
                CommandType = commandType,
                CommandText = commandText
            };
            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    if (param is OdbcParameter)
                    {
                        cmd.Parameters.Add(param);
                    }
                    else
                    {
                        var odbcParam = new OdbcParameter(param.ParameterName, param.Value)
                        {
                            Direction = param.Direction,
                            Size = param.Size,
                            SourceColumn = param.SourceColumn,
                            SourceColumnNullMapping = param.SourceColumnNullMapping,
                            SourceVersion = param.SourceVersion,
                            DbType = param.DbType,
                            IsNullable = param.IsNullable
                        };
                        if (param is Parameter uparam)
                        {
                            if (uparam.IsDbTypeSet)
                            {
                                odbcParam.DbType = uparam.DbType;
                            }
                        }
                        cmd.Parameters.Add(odbcParam);
                    }
                }
            }
            if ((timeout ?? OdbcSettings.DefaultTimeout).HasValue)
            {
                cmd.CommandTimeout = (timeout ?? OdbcSettings.DefaultTimeout).Value;
            }
            return cmd;
        }
    }
}
