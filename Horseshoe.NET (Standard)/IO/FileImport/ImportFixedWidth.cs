using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Horseshoe.NET.Collections.Extensions;
using static Horseshoe.NET.IO.FileImport.ImportUtil;
using static Horseshoe.NET.ObjectClean.Methods;
using Horseshoe.NET.Objects;
using Horseshoe.NET.Text;

namespace Horseshoe.NET.IO.FileImport
{
    internal static class ImportFixedWidth
    {
        internal static IEnumerable<string[]> AsStrings(Stream stream, Column[] columns, AutoTruncate autoTrunc)
        {
            var list = new List<string[]>();
            var lineNum = 0;
            using (var streamReader = new StreamReader(stream))
            {
                var rawRow = streamReader.ReadLine().Trim();
                lineNum++;
                while (rawRow != null)
                {
                    var rowTextValues = ParseStrings(rawRow, columns, lineNum, autoTrunc);

                    // ignore blank rows
                    if (rowTextValues == null)
                    {
                        rawRow = streamReader.ReadLine()?.Trim();
                        lineNum++;
                        continue;
                    }

                    // append items to list
                    list.Add(rowTextValues);

                    // loop next row
                    rawRow = streamReader.ReadLine()?.Trim();
                    lineNum++;
                }
            }
            return list.ToArray();
        }

        internal static IEnumerable<object[]> AsObjects(Stream stream, Column[] columns, AutoTruncate autoTrunc, DataErrorHandlingPolicy dataErrorHandling)
        {
            var allColumnsAreStrings = columns == null;
            var stringArrays = AsStrings(stream, columns, autoTrunc);

            if (allColumnsAreStrings)
            {
                return stringArrays;
            }

            var list = new List<object[]>();
            var _columns = columns.Where(c => c.IsMappable).ToArray();
            stringArrays.Iterate
            (
                (array, index) =>
                {
                    var oArray = new object[_columns.Length];
                    for (int i = 0; i < _columns.Length; i++)
                    {
                        oArray[i] = ProcessDatum(array[i], _columns[i], "data row " + index, dataErrorHandling);
                    }
                    list.Add(oArray);
                }
            );
            return list;
        }

        internal static DataTable AsDataTable(IEnumerable<object[]> objectArrays, Column[] columns)
        {
            var dataTable = new DataTable();

            // build data table structure
            foreach (var col in columns)
            {
                dataTable.Columns.Add(col.ToDataColumn());
            }

            // build data table data
            objectArrays.Iterate
            (
                (array) =>
                {
                    var row = dataTable.NewRow();
                    for (int i = 0; i < columns.Length; i++)
                    {
                        row[i] = array[i];
                    }
                    dataTable.Rows.Add(row);
                }
            );

            return dataTable;
        }

        internal static IEnumerable<E> AsCollection<E>(IEnumerable<object[]> objectArrays, Column[] columns, Func<object[], E> parseFunc, char[] charsToRemove) where E : class, new()
        {
            var list = new List<E>();

            // parse objects with parse function, if applicable... 
            if (parseFunc != null)
            {
                objectArrays.Iterate
                (
                    (array) =>
                    {
                        var e = parseFunc.Invoke(array);
                        list.Add(e);
                    }
                );
            }

            // ...otherwise extrapolate object property names from column names
            else
            {
                // characters to remove (whitespace is removed according to whitespace mode) to convert column names to object properties
                charsToRemove = charsToRemove != null
                    ? charsToRemove.Concat(new[] { '/', '-', '.', ',' }).ToArray()
                    : new[] { '/', '-', '.', ',' };

                objectArrays.Iterate
                (
                    (array) =>
                    {
                        E e = new E();
                        for (int i = 0; i < columns.Length; i++)
                        {
                            var propertyName = ZapString(columns[i].Name, textCleanMode: TextCleanMode.RemoveWhitespace, charsToRemove: charsToRemove);
                            ObjectUtil.SetInstanceProperty(e, propertyName, array[i], ignoreCase: true);
                        }
                        list.Add(e);
                    }
                );
            }

            return list;
        }

        private static readonly List<string> _list = new List<string>();
        private static string _tempValue;
        private static int _pos;

        private static string[] ParseStrings(string rawRow, Column[] columns, int lineNum, AutoTruncate autoTrunc)
        {
            _list.Clear();
            _pos = 0;

            if (rawRow.Trim().Length == 0)
            {
                return null;
            }

            try
            {
                foreach (var column in columns)
                {
                    if (column.IsMappable)
                    {
                        _tempValue = rawRow.Substring(_pos, column.Width);
                        _list.Add(PostParseString(_tempValue, autoTrunc));
                    }
                    _pos += column.Width;
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ValidationException("Not enough data on line " + lineNum);
            }

            if (_list.Count == 1 && Zap(_list[0]) == null)
            {
                return null;
            }

            return _list.ToArray();
        }
    }
}
