using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

using Horseshoe.NET.Db;
using Horseshoe.NET.Text;

using Oracle.ManagedDataAccess.Client;

namespace Horseshoe.NET.OracleDb
{
    public static class Update
    {
        public static int Table(string tableName, IEnumerable<Column> columns, Filter where = null, OraConnectionInfo connectionInfo = null, int? timeout = null)
        {
            using (var conn = OracleUtil.LaunchConnection(connectionInfo as OraConnectionInfo))
            {
                return Table(conn, tableName, columns, where: where, timeout: timeout);
            }
        }

        public static int Table(OracleConnection conn, string tableName, IEnumerable<Column> columns, Filter where = null, int? timeout = null)
        {
            var statement = @"
                UPDATE " + tableName + @"
                SET " + string.Join(", ", columns.Select(c => c.ToDMLString(DbProduct.Oracle)));
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
