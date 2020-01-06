using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

using Horseshoe.NET.Text;

namespace Horseshoe.NET.DataAccess.Sql
{
    public static class Insert
    {
        public static int Table
        (
            string tableName,
            IEnumerable<Column> columns,
            SqlConnectionInfo connectionInfo = null,
            int? timeout = null
        )
        {
            using (var conn = SqlUtil.LaunchConnection(connectionInfo))
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
            var statement = @"
                INSERT INTO " + tableName + " (" + string.Join(", ", columns.Select(c => c.ToString(DbProduct.SqlServer))) + @")
                VALUES (" + string.Join(", ", columns.Select(c => DataUtil.Sqlize(c.Value, DbProduct.SqlServer))) + ")";

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
            SqlConnectionInfo connectionInfo = null,
            int? timeout = null
        )
        {
            using (var conn = SqlUtil.LaunchConnection(connectionInfo))
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
            var statement = @"
                INSERT INTO " + tableName + " (" + string.Join(", ", columns.Select(c => c.ToString(DbProduct.SqlServer))) + @")
                VALUES (" + string.Join(", ", columns.Select(c => DataUtil.Sqlize(c.Value, DbProduct.SqlServer))) + @")
                SELECT CONVERT(int, SCOPE_IDENTITY())";

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
