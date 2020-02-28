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
    public static class Update
    {
        public static int Table
        (
            string tableName, 
            IEnumerable<Column> columns, 
            Filter where = null, 
            SqlConnectionInfo connectionInfo = null,
            int? timeout = null,
            CryptoOptions options = null
        )
        {
            using (var conn = SqlUtil.LaunchConnection(connectionInfo, options: options))
            {
                return Table(conn, tableName, columns, where: where, timeout: timeout);
            }
        }

        public static int Table
        (
            SqlConnection conn, 
            string tableName, 
            IEnumerable<Column> columns, 
            Filter where = null, 
            int? timeout = null
        )
        {
            foreach (var col in columns.Where(_col => !_col.Product.HasValue))
            {
                col.Product = DbProduct.SqlServer;
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

            DataUtil.UsingSqlStatement?.Invoke(statement);

            if (conn == null) return 0;

            return Execute.SQL(conn, statement, timeout: timeout);
        }
    }
}
