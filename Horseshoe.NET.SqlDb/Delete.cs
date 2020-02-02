using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

using Horseshoe.NET.Db;
using Horseshoe.NET.Text;

namespace Horseshoe.NET.SqlDb
{
    public static class Delete
    {
        public static int Table(string tableName, Filter where = null, bool truncate = false, SqlConnectionInfo connectionInfo = null, int? timeout = null)
        {
            using (var conn = SqlUtil.LaunchConnection(connectionInfo))
            {
                return Table(conn, tableName, where: where, truncate: truncate, timeout: timeout);
            }
        }

        public static int Table(SqlConnection conn, string tableName, Filter where = null, bool truncate = false, int? timeout = null)
        {
            string statement;
            if (where != null)
            {
                statement = @"
                    DELETE FROM " + tableName + @"
                    WHERE " + where;
                statement = statement.MultilineTrim();
            }
            else if (truncate)
            {
                statement = "TRUNCATE TABLE " + tableName;
            }
            else
            {
                statement = "DELETE FROM " + tableName;
            }

            DataUtil.UsingSqlStatement?.Invoke(statement);

            if (conn == null) return 0;

            return Execute.SQL(conn, statement, timeout: timeout);
        }
    }
}
