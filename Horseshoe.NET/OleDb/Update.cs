﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Linq;
using System.Text;

using Horseshoe.NET.Crypto;
using Horseshoe.NET.Db;
using Horseshoe.NET.Text.Extensions;

namespace Horseshoe.NET.OleDb
{
    public static class Update
    {
        public static int Table
        (
            string tableName, 
            IEnumerable<Column> columns, 
            Filter where = null, 
            OleDbConnectionInfo connectionInfo = null, 
            int? timeout = null, 
            DbProduct? product = null,
            CryptoOptions options = null
        )
        {
            using (var conn = OleDbUtil.LaunchConnection(connectionInfo, options: options))
            {
                conn.Open();
                return Table(conn, tableName, columns, where: where, timeout: timeout, product: product);
            }
        }

        public static int Table(OleDbConnection conn, string tableName, IEnumerable<Column> columns, Filter where = null, int? timeout = null, DbProduct? product = null)
        {
            foreach (var col in columns.Where(_col => !_col.Product.HasValue))
            {
                col.Product = product;
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

            OleDbUtil.UsingStatement?.Invoke(statement);

            if (conn == null) return 0;

            return Execute.SQL(conn, statement, timeout: timeout);
        }
    }
}
