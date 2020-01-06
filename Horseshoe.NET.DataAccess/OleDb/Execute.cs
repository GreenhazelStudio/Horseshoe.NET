using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;

namespace Horseshoe.NET.DataAccess.OleDb
{
    public static class Execute
    {
        public static int StoredProcedure(string procedureName, IEnumerable<DbParameter> parameters = null, OleDbConnectionInfo connectionInfo = null, int? timeout = null)
        {
            using (var conn = OleDbUtil.LaunchConnection(connectionInfo))
            {
                conn.Open();
                return StoredProcedure(conn, procedureName, parameters: parameters, timeout: timeout);
            }
        }

        public static int StoredProcedure(OleDbConnection conn, string procedureName, IEnumerable<DbParameter> parameters = null, int? timeout = null)
        {
            using (var cmd = OleDbUtil.BuildCommand(conn, CommandType.StoredProcedure, procedureName, parameters: parameters, timeout: timeout))
            {
                return cmd.ExecuteNonQuery();
            }
        }

        public static int SQL(string statement, IEnumerable<DbParameter> parameters = null, OleDbConnectionInfo connectionInfo = null, int? timeout = null)
        {
            using (var conn = OleDbUtil.LaunchConnection(connectionInfo))
            {
                conn.Open();
                return SQL(conn, statement, parameters: parameters, timeout: timeout);
            }
        }

        public static int SQL(OleDbConnection conn, string statement, IEnumerable<DbParameter> parameters = null, int? timeout = null)
        {
            using (var cmd = OleDbUtil.BuildCommand(conn, CommandType.Text, statement, parameters: parameters, timeout: timeout))  // parameters optional here, e.g. may already be included in the SQL statement
            {
                return cmd.ExecuteNonQuery();
            }
        }
    }
}
