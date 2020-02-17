using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;

using Oracle.ManagedDataAccess.Client;

using Horseshoe.NET.Collections;
using Horseshoe.NET.Cryptography;
using Horseshoe.NET.Db;
using Horseshoe.NET.OracleDb.Meta;
using Horseshoe.NET.Text;

namespace Horseshoe.NET.OracleDb
{
    public static class OracleUtil
    {
        /* * * * * * * * * * * * * * * * * * * * * * * * 
         *   CONNECTION-RELATED METHODS                *
         * * * * * * * * * * * * * * * * * * * * * * * */
        public static string GetDefaultConnectionString(out string announcePrefix)   // Uses EZ Connect style connection string
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
                var sb = new StringBuilder("Data Source=//" + Settings.DefaultDataSource);  // e.g. Settings.DefaultServer.DataSource (includes port)

                // server attributes
                if (Settings.DefaultServer != null)
                {
                    if (Settings.DefaultServer.ServiceName != null || Settings.DefaultServer.InstanceName != null)
                    {
                        sb.Append("/");
                        if (Settings.DefaultServer.ServiceName != null)
                        {
                            sb.Append(Settings.DefaultServer.ServiceName);
                        }
                        if (Settings.DefaultServer.InstanceName != null)
                        {
                            sb.Append("/" + Settings.DefaultServer.InstanceName);
                        }
                    }
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

        public static OracleCredential GetDefaultOracleCredentials()
        {
            return GetOracleCredentials(Settings.DefaultCredentials);
        }

        public static string WhichConnectionString(OraConnectionInfo connectionInfo = null, bool announce = false)
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

        public static Credential? WhichCredentials(OraConnectionInfo connectionInfo = null, bool announce = false)
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

        public static OracleConnection LaunchConnection(OraConnectionInfo connectionInfo = null, CryptoOptions options = null)
        {
            var conn = new OracleConnection
            {
                ConnectionString = WhichConnectionString(connectionInfo: connectionInfo, announce: true) ?? throw new UtilityException("No connection string or data source was found"),
                Credential = GetOracleCredentials(WhichCredentials(connectionInfo: connectionInfo, announce: true), options: options)
            };
            conn.Open();
            return conn;
        }

        [SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "Client code is responsible for statement and method parameter pre-validation.")]
        internal static OracleCommand BuildCommand
        (
            OracleConnection conn,
            CommandType commandType,
            string commandText,
            IEnumerable<DbParameter> parameters = null,
            int? timeout = null
        )
        {
            var cmd = new OracleCommand
            {
                Connection = conn,
                CommandType = commandType,
                CommandText = commandText
            };
            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    if (param is OracleParameter)
                    {
                        cmd.Parameters.Add(param);
                    }
                    else
                    {
                        var oraParam = new OracleParameter(param.ParameterName, param.Value)
                        {
                            //DbType = param.DbType,
                            Direction = param.Direction,
                            Size = param.Size,
                            SourceColumn = param.SourceColumn,
                            SourceColumnNullMapping = param.SourceColumnNullMapping,
                            SourceVersion = param.SourceVersion,
                            IsNullable = param.IsNullable
                        };
                        if (param is OraParameter uparam)
                        {
                            if (uparam.IsDbTypeSet)
                            {
                                oraParam.DbType = uparam.DbType;
                            }
                            if (uparam.IsOracleDbTypeSet)
                            {
                                oraParam.OracleDbType = uparam.OracleDbType;
                            }
                        }
                        cmd.Parameters.Add(oraParam);
                    }
                }
            }
            if ((timeout ?? Settings.DefaultTimeout).HasValue)
            {
                cmd.CommandTimeout = (timeout ?? Settings.DefaultTimeout).Value;
            }
            return cmd;
        }

        /* * * * * * * * * * * * * * * * * * * * 
         * ORACLE SPECIFIC METHODS             *
         * * * * * * * * * * * * * * * * * * * */
        public static IEnumerable<OraServer> ParseServerList(string rawList)
        {
            try
            {
                if (string.IsNullOrEmpty(rawList)) return null;
                var rawServers = rawList.Split('|');
                var servers = rawServers
                    .ZapAndPrune()
                    .Select(raw => ParseServer(raw))
                    .ToArray();
                return servers;
            }
            catch (UtilityException ex)
            {
                throw new UtilityException("Malformed server list.  Must resemble: ORADBSVR01|'NAME'11.22.33.44:9999;SERVICE1|ORADBSVR02:9999;SERVICE1;INSTANCE1", ex);
            }
        }

        private static readonly Regex ServerNamePattern = new Regex("(?<=')[^']+(?='.+)");

        public static OraServer ParseServer(string raw)
        {
            if (string.IsNullOrEmpty(raw)) return null;
            string name = null;
            var nameMatch = ServerNamePattern.Match(raw);

            if (nameMatch.Success)
            {
                name = nameMatch.Value;
                raw = raw.Replace("'" + name + "'", "");
            }

            var parts = raw.Split(';')
                .ZapAndPrune(prunePolicy: PrunePolicy.Trailing)
                .ToArray();

            if (parts.Length > 0 && parts.Length <= 3)
            {
                string dataSource = parts[0].Trim();
                string serviceName = null;
                string instanceName = null;

                if (parts.Length == 1 && name == null)
                {
                    var serverFromList = OraServer.Lookup(dataSource, suppressErrors: true);
                    if (serverFromList != null) return serverFromList;
                }

                if (parts.Length > 1)
                {
                    serviceName = parts[1].Zap();
                    if (parts.Length > 2)
                    {
                        instanceName = parts[2].Zap();
                    }
                }
                return new OraServer(dataSource, serviceName: serviceName, instanceName: instanceName, name: name ?? dataSource);
            }
            throw new UtilityException("Malformed server.  Must resemble { ORADBSVR01 or 'NAME'11.22.33.44:9999;SERVICE1 or ORADBSVR02:9999;SERVICE1;INSTANCE1 }.");
        }

        public static string BuildServerListString(IEnumerable<OraServer> list)
        {
            if (list == null || !list.Any()) return null;
            var listString = string.Join("|", list.Select(svr => svr.DataSource + (svr.ServiceName != null ? ";" + svr.ServiceName : "") + (svr.InstanceName != null ? (svr.ServiceName == null ? ";" : "") + ";" + svr.InstanceName : "")));
            return listString;
        }

        public static OracleCredential GetOracleCredentials(Credential? credentials, CryptoOptions options = null)
        {
            if (!credentials.HasValue) return null;

            if (credentials.Value.HasSecurePassword)
            {
                return new OracleCredential(credentials.Value.UserID, credentials.Value.SecurePassword);
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
                return new OracleCredential(credentials.Value.UserID, securePassword);
            }

            return new OracleCredential(credentials.Value.UserID, null);
        }
    }
}
