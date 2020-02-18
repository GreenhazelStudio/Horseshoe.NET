using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

using Oracle.ManagedDataAccess.Client;

using Horseshoe.NET.Cryptography;
using Horseshoe.NET.Db;
using Horseshoe.NET.Text;

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
            var statement = @"
                INSERT INTO " + tableName + " (" + string.Join(", ", columns.Select(c => c.ToString(DbProduct.Oracle))) + @")
                VALUES (" + string.Join(", ", columns.Select(c => DataUtil.Sqlize(c.Value, DbProduct.Oracle))) + ")";

            statement = statement.MultilineTrim();
            DataUtil.UsingSqlStatement?.Invoke(statement);

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
            var statement = @"
                INSERT INTO " + tableName + " (" + string.Join(", ", columns.Select(c => c.ToString(DbProduct.Oracle))) + @")
                VALUES (" + string.Join(", ", columns.Select(c => DataUtil.Sqlize(c.Value, DbProduct.Oracle))) + @");
                SELECT LAST_INSERT_ID()";

            statement = statement.MultilineTrim();
            DataUtil.UsingSqlStatement?.Invoke(statement);

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
