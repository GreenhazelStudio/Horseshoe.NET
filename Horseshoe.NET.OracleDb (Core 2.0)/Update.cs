using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

using Horseshoe.NET.Crypto;
using Horseshoe.NET.Db;
using Horseshoe.NET.Text.Extensions;

using Oracle.ManagedDataAccess.Client;

namespace Horseshoe.NET.OracleDb
{
    public static class Update
    {
        public static int Table
        (
            string tableName, 
            IEnumerable<Column> columns, 
            Filter where = null, 
            OraConnectionInfo connectionInfo = null, 
            int? timeout = null,
            CryptoOptions options = null
        )
        {
            using (var conn = OracleUtil.LaunchConnection(connectionInfo, options: options))
            {
                return Table(conn, tableName, columns, where: where, timeout: timeout);
            }
        }

        public static int Table(OracleConnection conn, string tableName, IEnumerable<Column> columns, Filter where = null, int? timeout = null)
        {
            foreach (var col in columns.Where(_col => !_col.Product.HasValue))
            {
                col.Product = DbProduct.Oracle;
            }

            var statement = @"
                UPDATE " + tableName + @"
                SET " + string.Join(", ", columns.Select(c => c.ToDMLString()));
            if (where != null)
            {
                statement += @"
                WHERE " + where;
            }

            statement = statement.MultilineTrim();

            OracleUtil.UsingStatement?.Invoke(statement);

            if (conn == null) return 0;

            return Execute.SQL(conn, statement, timeout: timeout);
        }
    }
}
