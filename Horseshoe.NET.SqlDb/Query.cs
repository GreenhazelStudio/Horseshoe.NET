using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Horseshoe.NET.Db;
using static Horseshoe.NET.Db.DataUtil;
using Horseshoe.NET.Objects;
using Horseshoe.NET.Text;

namespace Horseshoe.NET.SqlDb
{
    public static class Query
    {
        [SuppressMessage("Design", "CA1034:Nested types should not be visible", Justification = "This is my style")]
        public static class SQL
        {
            public static IEnumerable<E> AsCollection<E>
            (
                string statement,
                AutoSort<E> autoSort = null,
                Func<object[], E> objectParser = null,
                Func<IDataReader, E> readerParser = null,
                SqlConnectionInfo connectionInfo = null,
                int? timeout = null,
                AutoTruncate autoTrunc = default,
                bool suppressErrors = false
            )
            {
                using (var conn = SqlUtil.LaunchConnection(connectionInfo))
                {
                    return AsCollection(conn, statement, autoSort: autoSort, objectParser: objectParser, readerParser: readerParser, timeout: timeout, autoTrunc: autoTrunc, suppressErrors: suppressErrors);
                }
            }

            public static IEnumerable<E> AsCollection<E>
            (
                SqlConnection conn,
                string statement,
                AutoSort<E> autoSort = null,
                Func<object[], E> objectParser = null,
                Func<IDataReader, E> readerParser = null,
                int? timeout = null,
                AutoTruncate autoTrunc = default,
                bool suppressErrors = false
            )
            {
                var list = new List<E>();

                if (objectParser != null)
                {
                    if (readerParser != null) throw new UtilityException("This method accepts a maximum of one parser");
                    using (var reader = AsDataReader(conn, statement, keepOpen: true, timeout: timeout))
                    {
                        var dataColumns = reader.GetDataColumns();
                        object[] objects;
                        while (reader.Read())
                        {
                            objects = GetObjects(reader, dataColumns, autoTrunc: autoTrunc, suppressErrors: suppressErrors);
                            list.Add(objectParser.Invoke(objects));
                        }
                    }
                }
                else if (readerParser != null)
                {
                    using (var reader = AsDataReader(conn, statement, keepOpen: true, timeout: timeout))
                    {
                        while (reader.Read())
                        {
                            list.Add(readerParser.Invoke(reader));
                        }
                    }
                }
                else
                {
                    if (!typeof(E).IsClass || !typeof(E).GetConstructors().Any(c => !c.GetParameters().Any()))
                    {
                        throw new UtilityException("Cannot auto-generate instances of " + typeof(E).FullName + ".  Use only classes with a parameterless constructor.");
                    }
                    E e;
                    var properties = ObjectUtil.GetPublicInstanceProperties(typeof(E));
                    var objectArrays = AsObjects(conn, statement, out DataColumn[] dataColumns, timeout: timeout, autoTrunc: autoTrunc, suppressErrors: suppressErrors);
                    var normalizedColumnNames = dataColumns
                        .Select(c => c.ColumnName.Zap(textCleanMode: TextCleanMode.RemoveWhitespace))
                        .ToArray();
                    foreach (var objects in objectArrays)
                    {
                        e = (E)ObjectUtil.GetInstance(typeof(E));
                        for (int i = 0; i < objects.Length; i++)
                        {
                            ObjectUtil.SetInstanceProperty(e, normalizedColumnNames[i], objects[i], ignoreCase: true, suppressErrors: suppressErrors);
                        }
                        list.Add(e);
                    }
                }

                if (autoSort != null)
                {
                    if (autoSort.Sorter != null)
                    {
                        list = list
                            .OrderBy(autoSort.Sorter)
                            .ToList();
                    }
                    else if (autoSort.Comparer != null)
                    {
                        list.Sort(autoSort.Comparer);
                    }
                    else if (autoSort.Comparison != null)
                    {
                        list.Sort(autoSort.Comparison);
                    }
                    else
                    {
                        list.Sort();
                    }
                }
                return list;
            }

            public static object AsScalar
            (
                string statement,
                SqlConnectionInfo connectionInfo = null,
                int? timeout = null,
                AutoTruncate autoTrunc = default
            )
            {
                using (var conn = SqlUtil.LaunchConnection(connectionInfo))
                {
                    return AsScalar(conn, statement, timeout: timeout, autoTrunc: autoTrunc);
                }
            }

            public static object AsScalar
            (
                SqlConnection conn,
                string statement,
                int? timeout = null,
                AutoTruncate autoTrunc = default
            )
            {
                using (var command = SqlUtil.BuildCommand(conn, CommandType.Text, statement, timeout: timeout))
                {
                    var obj = command.ExecuteScalar();
                    if (ObjectUtil.IsNull(obj)) return null;
                    if (obj is string stringValue)
                    {
                        switch (autoTrunc)
                        {
                            case AutoTruncate.Trim:
                                return stringValue.Trim();
                            case AutoTruncate.Zap:
                                return stringValue.Zap();
                        }
                    }
                    return obj;
                }
            }

            public static E AsScalar<E>
            (
                string statement,
                SqlConnectionInfo connectionInfo = null,
                int? timeout = null
            )
            {
                using (var conn = SqlUtil.LaunchConnection(connectionInfo))
                {
                    return AsScalar<E>(conn, statement, timeout: timeout);
                }
            }

            public static E AsScalar<E>
            (
                SqlConnection conn,
                string statement,
                int? timeout = null
            )
            {
                var obj = AsScalar(conn, statement, timeout: timeout, autoTrunc: AutoTruncate.Zap);
                return obj == null ? default : (E)obj;
            }

            public static IDataReader AsDataReader
            (
                string statement,
                SqlConnectionInfo connectionInfo = null,
                bool keepOpen = false,
                int? timeout = null
            )
            {
                var conn = SqlUtil.LaunchConnection(connectionInfo);
                return AsDataReader(conn, statement, keepOpen: keepOpen, timeout: timeout);
            }

            [SuppressMessage("Code Quality", "IDE0067:Dispose objects before losing scope", Justification = "This method returns an open reader")]
            public static IDataReader AsDataReader
            (
                SqlConnection conn,
                string statement,
                bool keepOpen = false,
                int? timeout = null
            )
            {
                var command = SqlUtil.BuildCommand(conn, CommandType.Text, statement, timeout: timeout);
                var reader = keepOpen
                    ? command.ExecuteReader(CommandBehavior.Default)
                    : command.ExecuteReader(CommandBehavior.CloseConnection);
                return reader;
            }

            public static DataTable AsDataTable
            (
                string statement,
                SqlConnectionInfo connectionInfo = null,
                int? timeout = null,
                AutoTruncate autoTrunc = default
            )
            {
                using (var conn = SqlUtil.LaunchConnection(connectionInfo))
                {
                    return AsDataTable(conn, statement, timeout: timeout, autoTrunc: autoTrunc);
                }
            }

            public static DataTable AsDataTable
            (
                SqlConnection conn,
                string statement,
                int? timeout = null,
                AutoTruncate autoTrunc = default
            )
            {
                var dataTable = new DataTable();
                using (var command = SqlUtil.BuildCommand(conn, CommandType.Text, statement, timeout: timeout))
                {
                    using (var adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(dataTable);
                    }
                }
                switch (autoTrunc)
                {
                    case AutoTruncate.Trim:
                        TrimDataTable(dataTable);
                        break;
                    case AutoTruncate.Zap:
                        throw new UtilityException("Zap does not work on data tables");
                }
                return dataTable;
            }

            public static IEnumerable<object[]> AsObjects
            (
                string statement,
                SqlConnectionInfo connectionInfo = null,
                int? timeout = null,
                AutoTruncate autoTrunc = default,
                bool suppressErrors = false
            )
            {
                using (var conn = SqlUtil.LaunchConnection(connectionInfo))
                {
                    return AsObjects(conn, statement, timeout: timeout, autoTrunc: autoTrunc, suppressErrors: suppressErrors);
                }
            }

            public static IEnumerable<object[]> AsObjects
            (
                SqlConnection conn,
                string statement,
                int? timeout = null,
                AutoTruncate autoTrunc = default,
                bool suppressErrors = false
            )
            {
                return AsObjects(conn, statement, out _, timeout: timeout, autoTrunc: autoTrunc, suppressErrors: suppressErrors);
            }

            public static IEnumerable<object[]> AsObjects
            (
                string statement,
                out DataColumn[] columns,
                SqlConnectionInfo connectionInfo = null,
                int? timeout = null,
                AutoTruncate autoTrunc = default,
                bool suppressErrors = false
            )
            {
                using (var conn = SqlUtil.LaunchConnection(connectionInfo))
                {
                    return AsObjects(conn, statement, out columns, timeout: timeout, autoTrunc: autoTrunc, suppressErrors: suppressErrors);
                }
            }

            public static IEnumerable<object[]> AsObjects
            (
                SqlConnection conn,
                string statement,
                out DataColumn[] columns,
                int? timeout = null,
                AutoTruncate autoTrunc = default,
                bool suppressErrors = false
            )
            {
                var list = new List<object[]>();
                using (var reader = AsDataReader(conn, statement, keepOpen: true, timeout: timeout))
                {
                    columns = reader.GetDataColumns();
                    while (reader.Read())
                    {
                        var objects = GetObjects(reader, columns, autoTrunc: autoTrunc, suppressErrors: suppressErrors);
                        list.Add(objects);
                    }
                }
                return list;
            }

            public static IEnumerable<string[]> AsStrings
            (
                string statement,
                SqlConnectionInfo connectionInfo = null,
                int? timeout = null,
                AutoTruncate autoTrunc = default,
                bool suppressErrors = false
            )
            {
                using (var conn = SqlUtil.LaunchConnection(connectionInfo))
                {
                    return AsStrings(conn, statement, timeout: timeout, autoTrunc: autoTrunc, suppressErrors: suppressErrors);
                }
            }

            public static IEnumerable<string[]> AsStrings
            (
                SqlConnection conn,
                string statement,
                int? timeout = null,
                AutoTruncate autoTrunc = default,
                bool suppressErrors = false
            )
            {
                return AsStrings(conn, statement, out _, timeout: timeout, autoTrunc: autoTrunc, suppressErrors: suppressErrors);
            }

            public static IEnumerable<string[]> AsStrings
            (
                string statement,
                out DataColumn[] columns,
                SqlConnectionInfo connectionInfo = null,
                int? timeout = null,
                AutoTruncate autoTrunc = default,
                bool suppressErrors = false
            )
            {
                using (var conn = SqlUtil.LaunchConnection(connectionInfo))
                {
                    return AsStrings(conn, statement, out columns, timeout: timeout, autoTrunc: autoTrunc, suppressErrors: suppressErrors);
                }
            }

            public static IEnumerable<string[]> AsStrings
            (
                SqlConnection conn,
                string statement,
                out DataColumn[] columns,
                int? timeout = null,
                AutoTruncate autoTrunc = default,
                bool suppressErrors = false
            )
            {
                var list = new List<string[]>();
                using (var reader = AsDataReader(conn, statement, keepOpen: true, timeout: timeout))
                {
                    columns = reader.GetDataColumns();
                    while (reader.Read())
                    {
                        var objects = GetObjects(reader, columns, autoTrunc: autoTrunc, suppressErrors: suppressErrors);
                        var strings = objects
                            .Select(o => o?.ToString())
                            .ToArray();
                        list.Add(strings);
                    }
                }
                return list;
            }
        }

        [SuppressMessage("Design", "CA1034:Nested types should not be visible", Justification = "This is my style")]
        public static class TableOrView
        {
            public static IEnumerable<E> AsCollection<E>
            (
                string tableOrViewName,
                IEnumerable<string> columns = null,
                Filter where = null,
                IEnumerable<string> groupBy = null,
                IEnumerable<string> orderBy = null,
                AutoSort<E> autoSort = null,
                Func<object[], E> objectParser = null,
                Func<IDataReader, E> readerParser = null,
                SqlConnectionInfo connectionInfo = null,
                int? timeout = null,
                AutoTruncate autoTrunc = default,
                bool suppressErrors = false
            )
            {
                using (var conn = SqlUtil.LaunchConnection(connectionInfo))
                {
                    return AsCollection(conn, tableOrViewName, columns: columns, where: where, groupBy: groupBy, orderBy: orderBy, autoSort: autoSort, objectParser: objectParser, readerParser: readerParser, timeout: timeout, autoTrunc: autoTrunc, suppressErrors: suppressErrors);
                }
            }

            public static IEnumerable<E> AsCollection<E>
            (
                SqlConnection conn,
                string tableOrViewName,
                IEnumerable<string> columns = null,
                Filter where = null,
                IEnumerable<string> groupBy = null,
                IEnumerable<string> orderBy = null,
                AutoSort<E> autoSort = null,
                Func<object[], E> objectParser = null,
                Func<IDataReader, E> readerParser = null,
                int? timeout = null,
                AutoTruncate autoTrunc = default,
                bool suppressErrors = false
            )
            {
                var statement = BuildStatement(tableOrViewName, columns, where, groupBy, orderBy);

                if (conn == null) return null;

                var list = new List<E>();

                if (objectParser != null)
                {
                    if (readerParser != null) throw new UtilityException("This method accepts a maximum of one parser");
                    using (var reader = SQL.AsDataReader(conn, statement, keepOpen: true, timeout: timeout))
                    {
                        var dataColumns = reader.GetDataColumns();
                        object[] objects;
                        while (reader.Read())
                        {
                            objects = GetObjects(reader, dataColumns, autoTrunc: autoTrunc, suppressErrors: suppressErrors);
                            list.Add(objectParser.Invoke(objects));
                        }
                    }
                }
                else if (readerParser != null)
                {
                    using (var reader = SQL.AsDataReader(conn, statement, keepOpen: true, timeout: timeout))
                    {
                        while (reader.Read())
                        {
                            list.Add(readerParser.Invoke(reader));
                        }
                    }
                }
                else
                {
                    if (!typeof(E).IsClass || !typeof(E).GetConstructors().Any(c => !c.GetParameters().Any()))
                    {
                        throw new UtilityException("Cannot auto-generate instances of " + typeof(E).FullName + ".  Use only classes with a parameterless constructor.");
                    }
                    E e;
                    var properties = ObjectUtil.GetPublicInstanceProperties(typeof(E));
                    var objectArrays = SQL.AsObjects(conn, statement, out DataColumn[] dataColumns, timeout: timeout, autoTrunc: autoTrunc, suppressErrors: suppressErrors);
                    var normalizedColumnNames = dataColumns
                        .Select(c => c.ColumnName.Zap(textCleanMode: TextCleanMode.RemoveWhitespace))
                        .ToArray();
                    foreach (var objects in objectArrays)
                    {
                        e = (E)ObjectUtil.GetInstance(typeof(E));
                        for (int i = 0; i < objects.Length; i++)
                        {
                            ObjectUtil.SetInstanceProperty(e, normalizedColumnNames[i], objects[i], ignoreCase: true, suppressErrors: suppressErrors);
                        }
                        list.Add(e);
                    }
                }

                if (autoSort != null)
                {
                    if (autoSort.Sorter != null)
                    {
                        list = list
                            .OrderBy(autoSort.Sorter)
                            .ToList();
                    }
                    else if (autoSort.Comparer != null)
                    {
                        list.Sort(autoSort.Comparer);
                    }
                    else if (autoSort.Comparison != null)
                    {
                        list.Sort(autoSort.Comparison);
                    }
                    else
                    {
                        list.Sort();
                    }
                }
                return list;
            }

            static string BuildStatement
            (
                string tableOrViewName,
                IEnumerable<string> columns,
                Filter where,
                IEnumerable<string> groupBy,
                IEnumerable<string> orderBy
            )
            {
                var cols = columns != null
                    ? string.Join(", ", columns)
                    : "*";
                var statement = @"
                    SELECT " + cols + @"
                    FROM " + tableOrViewName;
                if (where != null)
                {
                    statement += @"
                    WHERE " + where;
                }
                if (groupBy != null)
                {
                    statement += @"
                    GROUP BY " + string.Join(", ", groupBy);
                }
                if (orderBy != null)
                {
                    statement += @"
                    ORDER BY " + string.Join(", ", orderBy);
                }

                statement = statement.MultilineTrim();
                UsingSqlStatement?.Invoke(statement);

                return statement;
            }

            public static IDataReader AsDataReader
            (
                string tableOrViewName,
                IEnumerable<string> columns = null,
                Filter where = null,
                IEnumerable<string> groupBy = null,
                IEnumerable<string> orderBy = null,
                SqlConnectionInfo connectionInfo = null,
                bool keepOpen = false,
                int? timeout = null
            )
            {
                var conn = SqlUtil.LaunchConnection(connectionInfo);
                return AsDataReader(conn, tableOrViewName, columns: columns, where: where, groupBy: groupBy, orderBy: orderBy, keepOpen: keepOpen, timeout: timeout);
            }

            [SuppressMessage("Code Quality", "IDE0067:Dispose objects before losing scope", Justification = "This method returns an open reader")]
            public static IDataReader AsDataReader
            (
                SqlConnection conn,
                string tableOrViewName,
                IEnumerable<string> columns = null,
                Filter where = null,
                IEnumerable<string> groupBy = null,
                IEnumerable<string> orderBy = null,
                bool keepOpen = false,
                int? timeout = null
            )
            {
                var statement = BuildStatement(tableOrViewName, columns, where, groupBy, orderBy);

                if (conn == null) return null;

                var command = SqlUtil.BuildCommand(conn, CommandType.Text, statement, timeout: timeout);
                var reader = keepOpen
                    ? command.ExecuteReader(CommandBehavior.Default)
                    : command.ExecuteReader(CommandBehavior.CloseConnection);
                return reader;
            }

            public static DataTable AsDataTable
            (
                string tableOrViewName,
                IEnumerable<string> columns = null,
                Filter where = null,
                IEnumerable<string> groupBy = null,
                IEnumerable<string> orderBy = null,
                SqlConnectionInfo connectionInfo = null,
                int? timeout = null,
                AutoTruncate autoTrunc = default
            )
            {
                using (var conn = SqlUtil.LaunchConnection(connectionInfo))
                {
                    return AsDataTable(conn, tableOrViewName, columns, where: where, groupBy: groupBy, orderBy: orderBy, timeout: timeout, autoTrunc: autoTrunc);
                }
            }

            [SuppressMessage("Code Quality", "IDE0068:Use recommended dispose pattern", Justification = "Datatable exists for arbitrary consumption, disposal not appropriate")]
            public static DataTable AsDataTable
            (
                SqlConnection conn,
                string tableOrViewName,
                IEnumerable<string> columns = null,
                Filter where = null,
                IEnumerable<string> groupBy = null,
                IEnumerable<string> orderBy = null,
                int? timeout = null,
                AutoTruncate autoTrunc = default
            )
            {
                var dataTable = new DataTable(tableOrViewName);
                var statement = BuildStatement(tableOrViewName, columns, where, groupBy, orderBy);

                if (conn == null) return null;

                using (var command = SqlUtil.BuildCommand(conn, CommandType.Text, statement, timeout: timeout))
                {
                    using (var adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(dataTable);
                    }
                }
                switch (autoTrunc)
                {
                    case AutoTruncate.Trim:
                        TrimDataTable(dataTable);
                        break;
                    case AutoTruncate.Zap:
                        throw new UtilityException("Zap does not work on data tables");
                }
                return dataTable;
            }

            public static IEnumerable<object[]> AsObjects
            (
                string tableOrViewName,
                IEnumerable<string> columns = null,
                Filter where = null,
                IEnumerable<string> groupBy = null,
                IEnumerable<string> orderBy = null,
                SqlConnectionInfo connectionInfo = null,
                int? timeout = null,
                AutoTruncate autoTrunc = default,
                bool suppressErrors = false
            )
            {
                using (var conn = SqlUtil.LaunchConnection(connectionInfo))
                {
                    return AsObjects(conn, tableOrViewName, columns: columns, where: where, groupBy: groupBy, orderBy: orderBy, timeout: timeout, autoTrunc: autoTrunc, suppressErrors: suppressErrors);
                }
            }

            public static IEnumerable<object[]> AsObjects
            (
                SqlConnection conn,
                string tableOrViewName,
                IEnumerable<string> columns = null,
                Filter where = null,
                IEnumerable<string> groupBy = null,
                IEnumerable<string> orderBy = null,
                int? timeout = null,
                AutoTruncate autoTrunc = default,
                bool suppressErrors = false
            )
            {
                return AsObjects(conn, out _, tableOrViewName, columns: columns, where: where, groupBy: groupBy, orderBy: orderBy, timeout: timeout, autoTrunc: autoTrunc, suppressErrors: suppressErrors);
            }

            public static IEnumerable<object[]> AsObjects
            (
                out DataColumn[] dataColumns,
                string tableOrViewName,
                IEnumerable<string> columns = null,
                Filter where = null,
                IEnumerable<string> groupBy = null,
                IEnumerable<string> orderBy = null,
                SqlConnectionInfo connectionInfo = null,
                int? timeout = null,
                AutoTruncate autoTrunc = default,
                bool suppressErrors = false
            )
            {
                using (var conn = SqlUtil.LaunchConnection(connectionInfo))
                {
                    return AsObjects(conn, out dataColumns, tableOrViewName, columns: columns, where: where, groupBy: groupBy, orderBy: orderBy, timeout: timeout, autoTrunc: autoTrunc, suppressErrors: suppressErrors);
                }
            }

            public static IEnumerable<object[]> AsObjects
            (
                SqlConnection conn,
                out DataColumn[] dataColumns,
                string tableOrViewName,
                IEnumerable<string> columns = null,
                Filter where = null,
                IEnumerable<string> groupBy = null,
                IEnumerable<string> orderBy = null,
                int? timeout = null,
                AutoTruncate autoTrunc = default,
                bool suppressErrors = false
            )
            {
                var list = new List<object[]>();
                using (var reader = AsDataReader(conn, tableOrViewName, columns: columns, where: where, groupBy: groupBy, orderBy: orderBy, keepOpen: true, timeout: timeout))
                {
                    dataColumns = reader.GetDataColumns();
                    while (reader.Read())
                    {
                        var objects = GetObjects(reader, dataColumns, autoTrunc: autoTrunc, suppressErrors: suppressErrors);
                        list.Add(objects);
                    }
                }
                return list;
            }

            public static IEnumerable<string[]> AsStrings
            (
                string tableOrViewName,
                IEnumerable<string> columns = null,
                Filter where = null,
                IEnumerable<string> groupBy = null,
                IEnumerable<string> orderBy = null,
                SqlConnectionInfo connectionInfo = null,
                int? timeout = null,
                AutoTruncate autoTrunc = default,
                bool suppressErrors = false
            )
            {
                using (var conn = SqlUtil.LaunchConnection(connectionInfo))
                {
                    return AsStrings(conn, tableOrViewName, columns: columns, where: where, groupBy: groupBy, orderBy: orderBy, timeout: timeout, autoTrunc: autoTrunc, suppressErrors: suppressErrors);
                }
            }

            public static IEnumerable<string[]> AsStrings
            (
                SqlConnection conn,
                string tableOrViewName,
                IEnumerable<string> columns = null,
                Filter where = null,
                IEnumerable<string> groupBy = null,
                IEnumerable<string> orderBy = null,
                int? timeout = null,
                AutoTruncate autoTrunc = default,
                bool suppressErrors = false
            )
            {
                return AsStrings(conn, out _, tableOrViewName, columns: columns, where: where, groupBy: groupBy, orderBy: orderBy, timeout: timeout, autoTrunc: autoTrunc, suppressErrors: suppressErrors);
            }

            public static IEnumerable<string[]> AsStrings
            (
                out DataColumn[] dataColumns,
                string tableOrViewName,
                IEnumerable<string> columns = null,
                Filter where = null,
                IEnumerable<string> groupBy = null,
                IEnumerable<string> orderBy = null,
                SqlConnectionInfo connectionInfo = null,
                int? timeout = null,
                AutoTruncate autoTrunc = default,
                bool suppressErrors = false
            )
            {
                using (var conn = SqlUtil.LaunchConnection(connectionInfo))
                {
                    return AsStrings(conn, out dataColumns, tableOrViewName, columns: columns, where: where, groupBy: groupBy, orderBy: orderBy, timeout: timeout, autoTrunc: autoTrunc, suppressErrors: suppressErrors);
                }
            }

            public static IEnumerable<string[]> AsStrings
            (
                SqlConnection conn,
                out DataColumn[] dataColumns,
                string tableOrViewName,
                IEnumerable<string> columns = null,
                Filter where = null,
                IEnumerable<string> groupBy = null,
                IEnumerable<string> orderBy = null,
                int? timeout = null,
                AutoTruncate autoTrunc = default,
                bool suppressErrors = false
            )
            {
                var list = new List<string[]>();
                using (var reader = AsDataReader(conn, tableOrViewName, columns: columns, where: where, groupBy: groupBy, orderBy: orderBy, keepOpen: true, timeout: timeout))
                {
                    dataColumns = reader.GetDataColumns();
                    while (reader.Read())
                    {
                        var objects = GetObjects(reader, dataColumns, autoTrunc: autoTrunc, suppressErrors: suppressErrors);
                        var strings = objects
                            .Select(o => o?.ToString())
                            .ToArray();
                        list.Add(strings);
                    }
                }
                return list;
            }
        }

        [SuppressMessage("Design", "CA1034:Nested types should not be visible", Justification = "This is my style")]
        public static class StoredProcedure
        {
            public static IEnumerable<E> AsCollection<E>
            (
                string procedureName,
                IEnumerable<DbParameter> parameters = null,
                AutoSort<E> autoSort = null,
                Func<object[], E> objectParser = null,
                Func<IDataReader, E> readerParser = null,
                SqlConnectionInfo connectionInfo = null,
                int? timeout = null,
                AutoTruncate autoTrunc = default,
                bool suppressErrors = false
            )
            {
                using (var conn = SqlUtil.LaunchConnection(connectionInfo))
                {
                    return AsCollection(conn, procedureName, parameters: parameters, autoSort: autoSort, objectParser: objectParser, readerParser: readerParser, timeout: timeout, autoTrunc: autoTrunc, suppressErrors: suppressErrors);
                }
            }

            public static IEnumerable<E> AsCollection<E>
            (
                SqlConnection conn,
                string procedureName,
                IEnumerable<DbParameter> parameters = null,
                AutoSort<E> autoSort = null,
                Func<object[], E> objectParser = null,
                Func<IDataReader, E> readerParser = null,
                int? timeout = null,
                AutoTruncate autoTrunc = default,
                bool suppressErrors = false
            )
            {
                var list = new List<E>();

                if (objectParser != null)
                {
                    if (readerParser != null) throw new UtilityException("This method accepts a maximum of one parser");
                    using (var command = SqlUtil.BuildCommand(conn, CommandType.StoredProcedure, procedureName, parameters: parameters, timeout: timeout))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            var dataColumns = reader.GetDataColumns();
                            object[] objects;
                            while (reader.Read())
                            {
                                objects = GetObjects(reader, dataColumns, autoTrunc: autoTrunc, suppressErrors: suppressErrors);
                                list.Add(objectParser.Invoke(objects));
                            }
                        }
                    }
                }
                else if (readerParser != null)
                {
                    using (var command = SqlUtil.BuildCommand(conn, CommandType.StoredProcedure, procedureName, parameters: parameters, timeout: timeout))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                list.Add(readerParser.Invoke(reader));
                            }
                        }
                    }
                }
                else
                {
                    if (!typeof(E).IsClass || !typeof(E).GetConstructors().Any(c => !c.GetParameters().Any()))
                    {
                        throw new UtilityException("Cannot auto-generate instances of " + typeof(E).FullName + ".  Use only classes with a parameterless constructor.");
                    }
                    E e;
                    var properties = ObjectUtil.GetPublicInstanceProperties(typeof(E));
                    using (var command = SqlUtil.BuildCommand(conn, CommandType.StoredProcedure, procedureName, parameters: parameters, timeout: timeout))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            var dataColumns = reader.GetDataColumns();
                            var normalizedColumnNames = dataColumns
                                .Select(c => c.ColumnName.Zap(textCleanMode: TextCleanMode.RemoveWhitespace))
                                .ToArray();
                            object[] objects;
                            while (reader.Read())
                            {
                                objects = GetObjects(reader, dataColumns, autoTrunc: autoTrunc, suppressErrors: suppressErrors);
                                e = (E)ObjectUtil.GetInstance(typeof(E));
                                for (int i = 0; i < objects.Length; i++)
                                {
                                    ObjectUtil.SetInstanceProperty(e, normalizedColumnNames[i], objects[i], ignoreCase: true, suppressErrors: suppressErrors);
                                }
                                list.Add(e);
                            }
                        }
                    }
                }

                if (autoSort != null)
                {
                    if (autoSort.Sorter != null)
                    {
                        list = list
                            .OrderBy(autoSort.Sorter)
                            .ToList();
                    }
                    else if (autoSort.Comparer != null)
                    {
                        list.Sort(autoSort.Comparer);
                    }
                    else if (autoSort.Comparison != null)
                    {
                        list.Sort(autoSort.Comparison);
                    }
                    else
                    {
                        list.Sort();
                    }
                }
                return list;
            }

            public static IDataReader AsDataReader
            (
                string procedureName,
                IEnumerable<DbParameter> parameters = null,
                SqlConnectionInfo connectionInfo = null,
                bool keepOpen = false,
                int? timeout = null
            )
            {
                var conn = SqlUtil.LaunchConnection(connectionInfo);
                return AsDataReader(conn, procedureName, parameters: parameters, keepOpen: keepOpen, timeout: timeout);
            }

            [SuppressMessage("Code Quality", "IDE0067:Dispose objects before losing scope", Justification = "This method returns an open reader")]
            public static IDataReader AsDataReader
            (
                SqlConnection conn,
                string procedureName,
                IEnumerable<DbParameter> parameters = null,
                bool keepOpen = false,
                int? timeout = null
            )
            {
                var command = SqlUtil.BuildCommand(conn, CommandType.StoredProcedure, procedureName, parameters: parameters, timeout: timeout);
                var reader = keepOpen
                    ? command.ExecuteReader(CommandBehavior.Default)
                    : command.ExecuteReader(CommandBehavior.CloseConnection);
                return reader;
            }

            public static DataTable AsDataTable
            (
                string procedureName,
                IEnumerable<DbParameter> parameters = null,
                SqlConnectionInfo connectionInfo = null,
                int? timeout = null,
                AutoTruncate autoTrunc = default
            )
            {
                using (var conn = SqlUtil.LaunchConnection(connectionInfo))
                {
                    return AsDataTable(conn, procedureName, parameters: parameters, timeout: timeout, autoTrunc: autoTrunc);
                }
            }

            public static DataTable AsDataTable
            (
                SqlConnection conn,
                string procedureName,
                IEnumerable<DbParameter> parameters = null,
                int? timeout = null,
                AutoTruncate autoTrunc = default
            )
            {
                var dataTable = new DataTable(procedureName);
                using (var command = SqlUtil.BuildCommand(conn, CommandType.StoredProcedure, procedureName, parameters: parameters, timeout: timeout))
                {
                    using (var adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(dataTable);
                    }
                }
                switch (autoTrunc)
                {
                    case AutoTruncate.Trim:
                        TrimDataTable(dataTable);
                        break;
                    case AutoTruncate.Zap:
                        throw new UtilityException("Zap does not work on data tables");
                }
                return dataTable;
            }

            public static IEnumerable<object[]> AsObjects
            (
                string procedureName,
                IEnumerable<DbParameter> parameters = null,
                SqlConnectionInfo connectionInfo = null,
                int? timeout = null,
                AutoTruncate autoTrunc = default,
                bool suppressErrors = false
            )
            {
                using (var conn = SqlUtil.LaunchConnection(connectionInfo))
                {
                    return AsObjects(conn, procedureName, parameters: parameters, timeout: timeout, autoTrunc: autoTrunc, suppressErrors: suppressErrors);
                }
            }

            public static IEnumerable<object[]> AsObjects
            (
                SqlConnection conn,
                string procedureName,
                IEnumerable<DbParameter> parameters = null,
                int? timeout = null,
                AutoTruncate autoTrunc = default,
                bool suppressErrors = false
            )
            {
                return AsObjects(conn, procedureName, out _, parameters: parameters, timeout: timeout, autoTrunc: autoTrunc, suppressErrors: suppressErrors);
            }

            public static IEnumerable<object[]> AsObjects
            (
                string procedureName,
                out DataColumn[] columns,
                IEnumerable<DbParameter> parameters = null,
                SqlConnectionInfo connectionInfo = null,
                int? timeout = null,
                AutoTruncate autoTrunc = default,
                bool suppressErrors = false
            )
            {
                using (var conn = SqlUtil.LaunchConnection(connectionInfo))
                {
                    return AsObjects(conn, procedureName, out columns, parameters: parameters, timeout: timeout, autoTrunc: autoTrunc, suppressErrors: suppressErrors);
                }
            }

            public static IEnumerable<object[]> AsObjects
            (
                SqlConnection conn,
                string procedureName,
                out DataColumn[] columns,
                IEnumerable<DbParameter> parameters = null,
                int? timeout = null,
                AutoTruncate autoTrunc = default,
                bool suppressErrors = false
            )
            {
                var list = new List<object[]>();
                using (var reader = AsDataReader(conn, procedureName, parameters: parameters, keepOpen: true, timeout: timeout))
                {
                    columns = reader.GetDataColumns();
                    while (reader.Read())
                    {
                        var objects = GetObjects(reader, columns, autoTrunc: autoTrunc, suppressErrors: suppressErrors);
                        list.Add(objects);
                    }
                }
                return list;
            }

            public static IEnumerable<string[]> AsStrings
            (
                string procedureName,
                IEnumerable<DbParameter> parameters = null,
                SqlConnectionInfo connectionInfo = null,
                int? timeout = null,
                AutoTruncate autoTrunc = default,
                bool suppressErrors = false
            )
            {
                using (var conn = SqlUtil.LaunchConnection(connectionInfo))
                {
                    return AsStrings(conn, procedureName, parameters: parameters, timeout: timeout, autoTrunc: autoTrunc, suppressErrors: suppressErrors);
                }
            }

            public static IEnumerable<string[]> AsStrings
            (
                SqlConnection conn,
                string procedureName,
                IEnumerable<DbParameter> parameters = null,
                int? timeout = null,
                AutoTruncate autoTrunc = default,
                bool suppressErrors = false
            )
            {
                return AsStrings(conn, procedureName, out _, parameters: parameters, timeout: timeout, autoTrunc: autoTrunc, suppressErrors: suppressErrors);
            }

            public static IEnumerable<string[]> AsStrings
            (
                string procedureName,
                out DataColumn[] columns,
                IEnumerable<DbParameter> parameters = null,
                SqlConnectionInfo connectionInfo = null,
                int? timeout = null,
                AutoTruncate autoTrunc = default,
                bool suppressErrors = false
            )
            {
                using (var conn = SqlUtil.LaunchConnection(connectionInfo))
                {
                    return AsStrings(conn, procedureName, out columns, parameters: parameters, timeout: timeout, autoTrunc: autoTrunc, suppressErrors: suppressErrors);
                }
            }

            public static IEnumerable<string[]> AsStrings
            (
                SqlConnection conn,
                string procedureName,
                out DataColumn[] columns,
                IEnumerable<DbParameter> parameters = null,
                int? timeout = null,
                AutoTruncate autoTrunc = default,
                bool suppressErrors = false
            )
            {
                var list = new List<string[]>();
                using (var reader = AsDataReader(conn, procedureName, parameters: parameters, keepOpen: true, timeout: timeout))
                {
                    columns = reader.GetDataColumns();
                    while (reader.Read())
                    {
                        var objects = GetObjects(reader, columns, autoTrunc: autoTrunc, suppressErrors: suppressErrors);
                        var strings = objects
                            .Select(o => o?.ToString())
                            .ToArray();
                        list.Add(strings);
                    }
                }
                return list;
            }

            public static object AsScalar
            (
                string procedureName,
                IEnumerable<DbParameter> parameters = null,
                SqlConnectionInfo connectionInfo = null,
                int? timeout = null,
                AutoTruncate autoTrunc = default
            )
            {
                using (var conn = SqlUtil.LaunchConnection(connectionInfo))
                {
                    return AsScalar(conn, procedureName, parameters: parameters, timeout: timeout, autoTrunc: autoTrunc);
                }
            }

            public static object AsScalar
            (
                SqlConnection conn,
                string procedureName,
                IEnumerable<DbParameter> parameters = null,
                int? timeout = null,
                AutoTruncate autoTrunc = default
            )
            {
                using (var command = SqlUtil.BuildCommand(conn, CommandType.StoredProcedure, procedureName, parameters: parameters, timeout: timeout))
                {
                    var obj = command.ExecuteScalar();
                    if (ObjectUtil.IsNull(obj)) return null;
                    if (obj is string stringValue)
                    {
                        switch (autoTrunc)
                        {
                            case AutoTruncate.Trim:
                                return stringValue.Trim();
                            case AutoTruncate.Zap:
                                return stringValue.Zap();
                        }
                    }
                    return obj;
                }
            }

            public static E AsScalar<E>
            (
                string procedureName,
                IEnumerable<DbParameter> parameters = null,
                SqlConnectionInfo connectionInfo = null,
                int? timeout = null
            )
            {
                using (var conn = SqlUtil.LaunchConnection(connectionInfo))
                {
                    return AsScalar<E>(conn, procedureName, parameters: parameters, timeout: timeout);
                }
            }

            public static E AsScalar<E>
            (
                SqlConnection conn,
                string procedureName,
                IEnumerable<DbParameter> parameters = null,
                int? timeout = null
            )
            {
                var obj = AsScalar(conn, procedureName, parameters: parameters, timeout: timeout, autoTrunc: AutoTruncate.Zap);
                return obj == null ? default : (E)obj;
            }
        }

        [SuppressMessage("Design", "CA1034:Nested types should not be visible", Justification = "This is my style")]
        public static class Function
        {
            public static IEnumerable<E> AsCollection<E>
            (
                string functionName,
                IEnumerable<DbParameter> parameters = null,
                AutoSort<E> autoSort = null,
                Func<object[], E> objectParser = null,
                Func<IDataReader, E> readerParser = null,
                SqlConnectionInfo connectionInfo = null,
                int? timeout = null,
                AutoTruncate autoTrunc = default,
                bool suppressErrors = false
            )
            {
                using (var conn = SqlUtil.LaunchConnection(connectionInfo))
                {
                    return AsCollection(conn, functionName, parameters: parameters, autoSort: autoSort, objectParser: objectParser, readerParser: readerParser, timeout: timeout, autoTrunc: autoTrunc, suppressErrors: suppressErrors);
                }
            }

            public static IEnumerable<E> AsCollection<E>
            (
                SqlConnection conn,
                string functionName,
                IEnumerable<DbParameter> parameters = null,
                AutoSort<E> autoSort = null,
                Func<object[], E> objectParser = null,
                Func<IDataReader, E> readerParser = null,
                int? timeout = null,
                AutoTruncate autoTrunc = default,
                bool suppressErrors = false
            )
            {
                string statement;
                if (parameters == null)
                {
                    statement = "SELECT * FROM " + functionName + "()";
                }
                else
                {
                    statement = "SELECT * FROM " + functionName + "(" + string.Join(", ", parameters.Select(p => Sqlize(p.Value, DbProduct.SqlServer))) + ")";
                }

                UsingSqlStatement?.Invoke(statement);

                if (conn == null) return null;

                var list = new List<E>();

                if (objectParser != null)
                {
                    if (readerParser != null) throw new UtilityException("This method accepts a maximum of one parser");
                    using (var reader = AsDataReader(conn, statement, keepOpen: true, timeout: timeout))
                    {
                        var dataColumns = reader.GetDataColumns();
                        object[] objects;
                        while (reader.Read())
                        {
                            objects = GetObjects(reader, dataColumns, autoTrunc: autoTrunc, suppressErrors: suppressErrors);
                            list.Add(objectParser.Invoke(objects));
                        }
                    }
                }
                else if (readerParser != null)
                {
                    using (var reader = AsDataReader(conn, statement, keepOpen: true, timeout: timeout))
                    {
                        while (reader.Read())
                        {
                            list.Add(readerParser.Invoke(reader));
                        }
                    }
                }
                else
                {
                    if (!typeof(E).IsClass || !typeof(E).GetConstructors().Any(c => !c.GetParameters().Any()))
                    {
                        throw new UtilityException("Cannot auto-generate instances of " + typeof(E).FullName + ".  Use only classes with a parameterless constructor.");
                    }
                    E e;
                    var properties = ObjectUtil.GetPublicInstanceProperties(typeof(E));
                    var objectArrays = AsObjects(conn, statement, out DataColumn[] dataColumns, timeout: timeout, autoTrunc: autoTrunc, suppressErrors: suppressErrors);
                    var normalizedColumnNames = dataColumns
                        .Select(c => c.ColumnName.Zap(textCleanMode: TextCleanMode.RemoveWhitespace))
                        .ToArray();
                    foreach (var objects in objectArrays)
                    {
                        e = (E)ObjectUtil.GetInstance(typeof(E));
                        for (int i = 0; i < objects.Length; i++)
                        {
                            ObjectUtil.SetInstanceProperty(e, normalizedColumnNames[i], objects[i], ignoreCase: true, suppressErrors: suppressErrors);
                        }
                        list.Add(e);
                    }
                }

                if (autoSort != null)
                {
                    if (autoSort.Sorter != null)
                    {
                        list = list
                            .OrderBy(autoSort.Sorter)
                            .ToList();
                    }
                    else if (autoSort.Comparer != null)
                    {
                        list.Sort(autoSort.Comparer);
                    }
                    else if (autoSort.Comparison != null)
                    {
                        list.Sort(autoSort.Comparison);
                    }
                    else
                    {
                        list.Sort();
                    }
                }
                return list;
            }

            public static object AsScalar
            (
                string functionName,
                IEnumerable<DbParameter> parameters = null,
                SqlConnectionInfo connectionInfo = null,
                int? timeout = null,
                AutoTruncate autoTrunc = default
            )
            {
                using (var conn = SqlUtil.LaunchConnection(connectionInfo))
                {
                    return AsScalar(conn, functionName, parameters: parameters, timeout: timeout, autoTrunc: autoTrunc);
                }
            }

            public static object AsScalar
            (
                SqlConnection conn,
                string functionName,
                IEnumerable<DbParameter> parameters = null,
                int? timeout = null,
                AutoTruncate autoTrunc = default
            )
            {
                string statement;
                if (parameters == null)
                {
                    statement = "SELECT " + functionName + "()";
                }
                else
                {
                    statement = "SELECT " + functionName + "(" + string.Join(", ", parameters.Select(p => Sqlize(p.Value, DbProduct.SqlServer))) + ")";
                }

                UsingSqlStatement?.Invoke(statement);

                if (conn == null) return null;

                using (var command = SqlUtil.BuildCommand(conn, CommandType.Text, statement, timeout: timeout))  // not including parameters here due to already embedded in the SQL statement
                {
                    var obj = command.ExecuteScalar();
                    if (ObjectUtil.IsNull(obj)) return null;
                    if (obj is string stringValue)
                    {
                        switch (autoTrunc)
                        {
                            case AutoTruncate.Trim:
                                return stringValue.Trim();
                            case AutoTruncate.Zap:
                                return stringValue.Zap();
                        }
                    }
                    return obj;
                }
            }

            public static E AsScalar<E>
            (
                string functionName,
                IEnumerable<DbParameter> parameters = null,
                SqlConnectionInfo connectionInfo = null,
                int? timeout = null
            )
            {
                using (var conn = SqlUtil.LaunchConnection(connectionInfo))
                {
                    return AsScalar<E>(conn, functionName, parameters: parameters, timeout: timeout);
                }
            }

            public static E AsScalar<E>
            (
                SqlConnection conn,
                string functionName,
                IEnumerable<DbParameter> parameters = null,
                int? timeout = null
            )
            {
                var obj = AsScalar(conn, functionName, parameters: parameters, timeout: timeout, autoTrunc: AutoTruncate.Zap);
                return obj == null ? default : (E)obj;
            }

            public static IDataReader AsDataReader
            (
                string functionName,
                IEnumerable<DbParameter> parameters = null,
                SqlConnectionInfo connectionInfo = null,
                bool keepOpen = false,
                int? timeout = null
            )
            {
                using (var conn = SqlUtil.LaunchConnection(connectionInfo))
                {
                    return AsDataReader(conn, functionName, parameters: parameters, keepOpen: keepOpen, timeout: timeout);
                }
            }

            [SuppressMessage("Code Quality", "IDE0067:Dispose objects before losing scope", Justification = "This method returns an open reader")]
            public static IDataReader AsDataReader
            (
                SqlConnection conn,
                string functionName,
                IEnumerable<DbParameter> parameters = null,
                bool keepOpen = false,
                int? timeout = null
            )
            {
                string statement;
                if (parameters == null)
                {
                    statement = "SELECT * FROM " + functionName + "()";
                }
                else
                {
                    statement = "SELECT * FROM " + functionName + "(" + string.Join(", ", parameters.Select(p => Sqlize(p.Value, DbProduct.SqlServer))) + ")";
                }

                UsingSqlStatement?.Invoke(statement);

                if (conn == null) return null;

                var command = SqlUtil.BuildCommand(conn, CommandType.Text, statement, timeout: timeout);  // not including parameters here due to already embedded in the SQL statement
                var reader = keepOpen
                    ? command.ExecuteReader(CommandBehavior.Default)
                    : command.ExecuteReader(CommandBehavior.CloseConnection);
                return reader;
            }

            public static DataTable AsDataTable
            (
                string functionName,
                IEnumerable<DbParameter> parameters = null,
                SqlConnectionInfo connectionInfo = null,
                int? timeout = null,
                AutoTruncate autoTrunc = default
            )
            {
                using (var conn = SqlUtil.LaunchConnection(connectionInfo))
                {
                    return AsDataTable(conn, functionName, parameters: parameters, timeout: timeout, autoTrunc: autoTrunc);
                }
            }

            [SuppressMessage("Code Quality", "IDE0068:Use recommended dispose pattern", Justification = "Not ready to dispose when returned to user")]
            public static DataTable AsDataTable
            (
                SqlConnection conn,
                string functionName,
                IEnumerable<DbParameter> parameters = null,
                int? timeout = null,
                AutoTruncate autoTrunc = default
            )
            {
                var dataTable = new DataTable(functionName);
                string statement;
                if (parameters == null)
                {
                    statement = "SELECT * FROM " + functionName + "()";
                }
                else
                {
                    statement = "SELECT * FROM " + functionName + "(" + string.Join(", ", parameters.Select(p => Sqlize(p.Value, DbProduct.SqlServer))) + ")";
                }

                UsingSqlStatement?.Invoke(statement);

                if (conn == null) return null;

                using (var command = SqlUtil.BuildCommand(conn, CommandType.Text, statement, timeout: timeout))  // not including parameters here due to already embedded in the SQL statement
                {
                    using (var adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(dataTable);
                    }
                }
                switch (autoTrunc)
                {
                    case AutoTruncate.Trim:
                        TrimDataTable(dataTable);
                        break;
                    case AutoTruncate.Zap:
                        throw new UtilityException("Zap does not work on data tables");
                }
                return dataTable;
            }

            public static IEnumerable<object[]> AsObjects
            (
                string functionName,
                IEnumerable<DbParameter> parameters = null,
                SqlConnectionInfo connectionInfo = null,
                int? timeout = null,
                AutoTruncate autoTrunc = default,
                bool suppressErrors = false
            )
            {
                using (var conn = SqlUtil.LaunchConnection(connectionInfo))
                {
                    return AsObjects(conn, functionName, parameters: parameters, timeout: timeout, autoTrunc: autoTrunc, suppressErrors: suppressErrors);
                }
            }

            public static IEnumerable<object[]> AsObjects
            (
                SqlConnection conn,
                string functionName,
                IEnumerable<DbParameter> parameters = null,
                int? timeout = null,
                AutoTruncate autoTrunc = default,
                bool suppressErrors = false
            )
            {
                return AsObjects(conn, functionName, out _, parameters: parameters, timeout: timeout, autoTrunc: autoTrunc, suppressErrors: suppressErrors);
            }

            public static IEnumerable<object[]> AsObjects
            (
                string functionName,
                out DataColumn[] columns,
                IEnumerable<DbParameter> parameters = null,
                SqlConnectionInfo connectionInfo = null,
                int? timeout = null,
                AutoTruncate autoTrunc = default,
                bool suppressErrors = false
            )
            {
                using (var conn = SqlUtil.LaunchConnection(connectionInfo))
                {
                    return AsObjects(conn, functionName, out columns, parameters: parameters, timeout: timeout, autoTrunc: autoTrunc, suppressErrors: suppressErrors);
                }
            }

            public static IEnumerable<object[]> AsObjects
            (
                SqlConnection conn,
                string functionName,
                out DataColumn[] columns,
                IEnumerable<DbParameter> parameters = null,
                int? timeout = null,
                AutoTruncate autoTrunc = default,
                bool suppressErrors = false
            )
            {
                var list = new List<object[]>();
                using (var reader = AsDataReader(conn, functionName, parameters: parameters, keepOpen: true, timeout: timeout))
                {
                    columns = reader.GetDataColumns();
                    while (reader.Read())
                    {
                        var objects = GetObjects(reader, columns, autoTrunc: autoTrunc, suppressErrors: suppressErrors);
                        list.Add(objects);
                    }
                }
                return list;
            }
        }
    }
}
