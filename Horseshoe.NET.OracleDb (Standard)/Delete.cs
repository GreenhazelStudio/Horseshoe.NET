﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

using Oracle.ManagedDataAccess.Client;

using Horseshoe.NET.Crypto;
using Horseshoe.NET.Db;
using Horseshoe.NET.Text.Extensions;

namespace Horseshoe.NET.OracleDb
{
    public static class Delete
    {
        public static int Table
        (
            string tableName, 
            Filter where = null, 
            bool truncate = false, 
            OraConnectionInfo connectionInfo = null, 
            int? timeout = null,
            CryptoOptions options = null
        )
        {
            using (var conn = OracleUtil.LaunchConnection(connectionInfo, options: options))
            {
                return Table(conn, tableName, where: where, truncate: truncate, timeout: timeout);
            }
        }

        public static int Table
        (
            OracleConnection conn, 
            string tableName, 
            Filter where = null, 
            bool truncate = false, 
            int? timeout = null
        )
        {
            string statement;
            if (where != null)
            {
                statement = @"
                    DELETE FROM " + tableName + @"
                    WHERE " + where;
                statement = statement.MultilineTrim();
            }
            else if (truncate)
            {
                statement = "TRUNCATE TABLE " + tableName;
            }
            else
            {
                statement = "DELETE FROM " + tableName;
            }

            OracleUtil.UsingStatement?.Invoke(statement);

            if (conn == null) return 0;

            return Execute.SQL(conn, statement, timeout: timeout);
        }
    }
}
