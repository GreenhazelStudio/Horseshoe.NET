using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using System.Linq;
using System.Text;

using Horseshoe.NET.Crypto;
using Horseshoe.NET.Db;
using Horseshoe.NET.Text;

namespace Horseshoe.NET.Odbc
{
    public static class Insert
    {
        public static int Table
        (
            string tableName,
            IEnumerable<Column> columns,
            OdbcConnectionInfo connectionInfo = null,
            int? timeout = null,
            DbProduct? product = null,
            CryptoOptions options = null
        )
        {
            using (var conn = OdbcUtil.LaunchConnection(connectionInfo, options: options))
            {
                return Table(conn, tableName, columns, timeout: timeout, product: product);
            }
        }

        public static int Table
        (
            OdbcConnection conn,
            string tableName,
            IEnumerable<Column> columns,
            int? timeout = null,
            DbProduct? product = null
        )
        {
            foreach (var col in columns.Where(_col => !_col.Product.HasValue))
            {
                col.Product = product;
            }

            var statement = @"
                INSERT INTO " + tableName + " (" + string.Join(", ", columns) + @")
                VALUES (" + string.Join(", ", columns.Select(c => DataUtil.Sqlize(c.Value, product))) + ")";

            statement = statement.MultilineTrim();
            OdbcUtil.UsingStatement?.Invoke(statement);

            if (conn == null) return 0;

            return Execute.SQL(conn, statement, timeout: timeout);
        }

        public static int Table
        (
            string tableName,
            IEnumerable<Column> columns,
            out int identity,
            OdbcConnectionInfo connectionInfo = null,
            int? timeout = null,
            DbProduct? product = null,
            CryptoOptions options = null
        )
        {
            using (var conn = OdbcUtil.LaunchConnection(connectionInfo, options: options))
            {
                return Table(conn, tableName, columns, out identity, timeout: timeout, product: product);
            }
        }

        public static int Table
        (
            OdbcConnection conn,
            string tableName,
            IEnumerable<Column> columns,
            out int identity,
            int? timeout = null,
            DbProduct? product = null
        )
        {
            foreach (var col in columns.Where(_col => !_col.Product.HasValue))
            {
                col.Product = product;
            }

            var statement = @"
                INSERT INTO " + tableName + " (" + string.Join(", ", columns) + @")
                VALUES (" + string.Join(", ", columns.Select(c => DataUtil.Sqlize(c.Value, product))) + ")";
            switch (product)
            {
                case DbProduct.SqlServer:
                    statement += @"
                        SELECT CONVERT(int, SCOPE_IDENTITY())";
                    break;
                case DbProduct.Oracle:
                    statement += @";
                        SELECT LAST_INSERT_ID();";
                    break;
                case null:
                    throw new UtilityException("Identifying the identity value is a product-specific technique. Product not supplied.");
                default:
                    throw new UtilityException("Identifying the identity value is a product-specific technique. Unsupported product: " + product);
            }

            statement = statement.MultilineTrim();
            OdbcUtil.UsingStatement?.Invoke(statement);

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
