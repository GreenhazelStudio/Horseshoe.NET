using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

using Horseshoe.NET.Crypto;
using Horseshoe.NET.Db;
using Horseshoe.NET.Text;

namespace Horseshoe.NET.SqlDb
{
    public static class Insert
    {
        public static int Table
        (
            string tableName,
            IEnumerable<Column> columns,
            SqlConnectionInfo connectionInfo = null,
            int? timeout = null,
            CryptoOptions options = null
        )
        {
            using (var conn = SqlUtil.LaunchConnection(connectionInfo, options: options))
            {
                return Table(conn, tableName, columns, timeout: timeout);
            }
        }

        public static int Table
        (
            SqlConnection conn,
            string tableName,
            IEnumerable<Column> columns,
            int? timeout = null
        )
        {
            foreach (var col in columns.Where(_col => !_col.Product.HasValue))
            {
                col.Product = DbProduct.SqlServer;
            }

            var statement = @"
                INSERT INTO " + tableName + " (" + string.Join(", ", columns) + @")
                VALUES (" + string.Join(", ", columns.Select(c => DataUtil.Sqlize(c.Value, DbProduct.SqlServer))) + ")";

            statement = statement.MultilineTrim();
            SqlUtil.UsingStatement?.Invoke(statement);

            if (conn == null) return 0;

            return Execute.SQL(conn, statement, timeout: timeout);
        }

        public static int Table
        (
            string tableName,
            IEnumerable<Column> columns,
            out int identity,
            SqlConnectionInfo connectionInfo = null,
            int? timeout = null,
            CryptoOptions options = null
        )
        {
            using (var conn = SqlUtil.LaunchConnection(connectionInfo, options: options))
            {
                return Table(conn, tableName, columns, out identity, timeout: timeout);
            }
        }

        public static int Table
        (
            SqlConnection conn,
            string tableName,
            IEnumerable<Column> columns,
            out int identity,
            int? timeout = null
        )
        {
            foreach (var col in columns.Where(_col => !_col.Product.HasValue))
            {
                col.Product = DbProduct.SqlServer;
            }

            var statement = @"
                INSERT INTO " + tableName + " (" + string.Join(", ", columns) + @")
                VALUES (" + string.Join(", ", columns.Select(c => DataUtil.Sqlize(c.Value, DbProduct.SqlServer))) + @")
                SELECT CONVERT(int, SCOPE_IDENTITY())";

            statement = statement.MultilineTrim();
            SqlUtil.UsingStatement?.Invoke(statement);

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
