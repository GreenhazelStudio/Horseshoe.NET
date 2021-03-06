﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

using Horseshoe.NET.Crypto;

namespace Horseshoe.NET.SqlDb
{
    public static class Execute
    {
        public static int StoredProcedure
        (
            string procedureName, 
            IEnumerable<DbParameter> parameters = null, 
            SqlConnectionInfo connectionInfo = null, 
            int? timeout = null,
            CryptoOptions options = null
        )
        {
            using (var conn = SqlUtil.LaunchConnection(connectionInfo, options: options))
            {
                return StoredProcedure(conn, procedureName, parameters: parameters, timeout: timeout);
            }
        }

        public static int StoredProcedure
        (
            SqlConnection conn, 
            string procedureName, 
            IEnumerable<DbParameter> parameters = null, 
            int? timeout = null
        )
        {
            using (var cmd = SqlUtil.BuildCommand(conn, CommandType.StoredProcedure, procedureName, parameters: parameters, timeout: timeout))
            {
                return cmd.ExecuteNonQuery();
            }
        }

        public static int SQL
        (
            string statement, 
            IEnumerable<DbParameter> parameters = null, 
            SqlConnectionInfo connectionInfo = null, 
            int? timeout = null,
            CryptoOptions options = null
        )
        {
            using (var conn = SqlUtil.LaunchConnection(connectionInfo, options: options))
            {
                return SQL(conn, statement, parameters: parameters, timeout: timeout);
            }
        }

        public static int SQL
        (
            SqlConnection conn, 
            string statement, 
            IEnumerable<DbParameter> parameters = null, 
            int? timeout = null
        )
        {
            using (var cmd = SqlUtil.BuildCommand(conn, CommandType.Text, statement, parameters: parameters, timeout: timeout))    // parameters optional here, e.g. may already be included in the SQL statement
            {
                return cmd.ExecuteNonQuery();
            }
        }
    }
}
