using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;

using Horseshoe.NET.Crypto;

namespace Horseshoe.NET.Odbc
{
    public static class Execute
    {
        public static int StoredProcedure
        (
            string procedureName, 
            IEnumerable<DbParameter> parameters = null, 
            OdbcConnectionInfo connectionInfo = null, 
            int? timeout = null,
            CryptoOptions options = null
        )
        {
            using (var conn = OdbcUtil.LaunchConnection(connectionInfo, options: options))
            {
                conn.Open();
                return StoredProcedure(conn, procedureName, parameters: parameters, timeout: timeout);
            }
        }

        public static int StoredProcedure
        (
            OdbcConnection conn, 
            string procedureName, 
            IEnumerable<DbParameter> parameters = null, 
            int? timeout = null
        )
        {
            using (var cmd = OdbcUtil.BuildCommand(conn, CommandType.StoredProcedure, procedureName, parameters: parameters, timeout: timeout))
            {
                return cmd.ExecuteNonQuery();
            }
        }

        public static int SQL
        (
            string statement, 
            IEnumerable<DbParameter> parameters = null, 
            OdbcConnectionInfo connectionInfo = null, 
            int? timeout = null,
            CryptoOptions options = null
        )
        {
            using (var conn = OdbcUtil.LaunchConnection(connectionInfo, options: options))
            {
                conn.Open();
                return SQL(conn, statement, parameters: parameters, timeout: timeout);
            }
        }

        public static int SQL
        (
            OdbcConnection conn, 
            string statement, 
            IEnumerable<DbParameter> parameters = null, 
            int? timeout = null
        )
        {
            using (var cmd = OdbcUtil.BuildCommand(conn, CommandType.Text, statement, parameters: parameters, timeout: timeout))    // parameters optional here, e.g. may already be included in the SQL statement
            {
                return cmd.ExecuteNonQuery();
            }
        }
    }
}
