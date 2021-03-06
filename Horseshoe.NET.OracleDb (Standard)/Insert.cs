﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

using Oracle.ManagedDataAccess.Client;

using Horseshoe.NET.Crypto;
using Horseshoe.NET.Db;
using Horseshoe.NET.Text.Extensions;

namespace Horseshoe.NET.OracleDb
{
    public static class Insert
    {
        public static int Table
        (
            string tableName,
            IEnumerable<Column> columns,
            OraConnectionInfo connectionInfo = null,
            int? timeout = null,
            CryptoOptions options = null
        )
        {
            using (var conn = OracleUtil.LaunchConnection(connectionInfo, options: options))
            {
                return Table(conn, tableName, columns, timeout: timeout);
            }
        }

        public static int Table
        (
            OracleConnection conn,
            string tableName,
            IEnumerable<Column> columns,
            int? timeout = null
        )
        {
            foreach (var col in columns.Where(_col => !_col.Product.HasValue))
            {
                col.Product = DbProduct.Oracle;
            }

            var statement = @"
                INSERT INTO " + tableName + " (" + string.Join(", ", columns) + @")
                VALUES (" + string.Join(", ", columns.Select(c => DataUtil.Sqlize(c.Value, DbProduct.Oracle))) + ")";

            statement = statement.MultilineTrim();
            OracleUtil.UsingStatement?.Invoke(statement);

            if (conn == null) return 0;

            return Execute.SQL(conn, statement, timeout: timeout);
        }

        public static int Table
        (
            string tableName,
            IEnumerable<Column> columns,
            out int identity,
            OraConnectionInfo connectionInfo = null,
            int? timeout = null,
            CryptoOptions options = null
        )
        {
            using (var conn = OracleUtil.LaunchConnection(connectionInfo, options: options))
            {
                return Table(conn, tableName, columns, out identity, timeout: timeout);
            }
        }

        public static int Table
        (
            OracleConnection conn,
            string tableName,
            IEnumerable<Column> columns,
            out int identity,
            int? timeout = null
        )
        {
            foreach (var col in columns.Where(_col => !_col.Product.HasValue))
            {
                col.Product = DbProduct.Oracle;
            }

            var statement = @"
                INSERT INTO " + tableName + " (" + string.Join(", ", columns) + @")
                VALUES (" + string.Join(", ", columns.Select(c => DataUtil.Sqlize(c.Value, DbProduct.Oracle))) + @");
                SELECT LAST_INSERT_ID()";

            statement = statement.MultilineTrim();
            OracleUtil.UsingStatement?.Invoke(statement);

            if (conn == null)
            {
                identity = 0;
                return 0;
            }

            identity = Query.SQL.AsScalar<int>(conn, statement, timeout: timeout);
            return 1;
        }
    }
}
