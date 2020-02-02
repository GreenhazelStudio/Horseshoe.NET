using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Linq;
using System.Text;

using Horseshoe.NET.Db;
using Horseshoe.NET.Text;

namespace Horseshoe.NET.Odbc
{
    public static class Update
    {
        public static int Table(string tableName, IEnumerable<Column> columns, Filter where = null, OdbcConnectionInfo connectionInfo = null, int? timeout = null, DbProduct? product = null)
        {
            using (var conn = OdbcUtil.LaunchConnection(connectionInfo))
            {
                conn.Open();
                return Table(conn, tableName, columns, where: where, timeout: timeout, product: product);
            }
        }

        public static int Table(OdbcConnection conn, string tableName, IEnumerable<Column> columns, Filter where = null, int? timeout = null, DbProduct? product = null)
        {
            var statement = @"
                UPDATE " + tableName + @"
                SET " + string.Join(", ", columns.Select(c => c.ToDMLString(product)));
            if (where != null)
            {
                statement += @"
                WHERE " + where;
            }

            statement = statement.MultilineTrim();

            DataUtil.UsingSqlStatement?.Invoke(statement);

            if (conn == null) return 0;

            return Execute.SQL(conn, statement, timeout: timeout);
        }
    }
}
