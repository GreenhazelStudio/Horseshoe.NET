using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

using Horseshoe.NET.Cryptography;

using Oracle.ManagedDataAccess.Client;

namespace Horseshoe.NET.OracleDb
{
    public static class Execute
    {
        public static int StoredProcedure
        (
            string procedureName, 
            IEnumerable<DbParameter> parameters = null, 
            OraConnectionInfo connectionInfo = null, 
            int? timeout = null,
            CryptoOptions options = null
        )
        {
            using (var conn = OracleUtil.LaunchConnection(connectionInfo, options: options))
            {
                return StoredProcedure(conn, procedureName, parameters: parameters, timeout: timeout);
            }
        }

        public static int StoredProcedure
        (
            OracleConnection conn, 
            string procedureName, 
            IEnumerable<DbParameter> parameters = null, 
            int? timeout = null
        )
        {
            using (var cmd = OracleUtil.BuildCommand(conn, CommandType.StoredProcedure, procedureName, parameters: parameters, timeout: timeout))
            {
                return cmd.ExecuteNonQuery();
            }
        }

        public static int SQL
        (
            string statement, 
            IEnumerable<DbParameter> parameters = null, 
            OraConnectionInfo connectionInfo = null, 
            int? timeout = null,
            CryptoOptions options = null
        )
        {
            using (var conn = OracleUtil.LaunchConnection(connectionInfo, options: options))
            {
                return SQL(conn, statement, parameters: parameters, timeout: timeout);   // parameters optional here, e.g. may already be included in the SQL statement
            }
        }

        public static int SQL
        (
            OracleConnection conn, 
            string statement, 
            IEnumerable<DbParameter> parameters = null, 
            int? timeout = null
        )
        {
            using (var cmd = OracleUtil.BuildCommand(conn, CommandType.Text, statement, parameters: parameters, timeout: timeout))   // parameters optional here, e.g. may already be included in the SQL statement
            {
                return cmd.ExecuteNonQuery();
            }
        }
    }
}
