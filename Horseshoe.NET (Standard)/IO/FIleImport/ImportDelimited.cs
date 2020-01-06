using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Horseshoe.NET.Collections;
using static Horseshoe.NET.IO.FileImport.ImportUtil;
using Horseshoe.NET.Text;
using static Horseshoe.NET.Text.TextUtil;  // Zap(), etc.

namespace Horseshoe.NET.IO.FileImport
{
    internal static class ImportDelimited
    {
        internal static IEnumerable<string[]> AsStrings(Stream stream, char delimiter, ref Column[] columns, bool hasHeaderRow, AutoTruncate autoTrunc)
        {
            var list = new List<string[]>();
            var lineNum = 0;
            var firstRowProcessed = false;
            using (var streamReader = new StreamReader(stream))
            {
                var rawRow = streamReader.ReadLine().Trim();
                lineNum++;
                while (rawRow != null)
                {
                    var rowTextValues = ParseStrings(rawRow, delimiter, lineNum, autoTrunc);

                    // ignore blank rows
                    if (rowTextValues == null)
                    {
                        rawRow = streamReader.ReadLine()?.Trim();
                        lineNum++;
                        continue;
                    }

                    if (!firstRowProcessed)
                    {
                        // prepare column names if necessary, otherwise validate user-supplied column info
                        if (columns == null)
                        {
                            columns = new Column[rowTextValues.Length];
                            for (int i = 0; i < columns.Length; i++)
                            {
                                columns[i] = Column.String(hasHeaderRow ? rowTextValues[i] : "Col " + i);
                            }
                        }
                        else if (rowTextValues.Length != columns.Length)
                        {
                            throw new ValidationException("The number of columns / data values on line " + lineNum + " did not match: " + columns.Length + " / " + rowTextValues.Length);
                        }

                        firstRowProcessed = true;

                        // if the first row is a header row skip to the first data row
                        if (hasHeaderRow)
                        {
                            rawRow = streamReader.ReadLine()?.Trim();
                            lineNum++;
                            continue;
                        }
                    }
                    else if (rowTextValues.Length != columns.Length)
                    {
                        throw new ValidationException("The number of columns / data values on line " + lineNum + " did not match: " + columns.Length + " / " + rowTextValues.Length);
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

        internal static IEnumerable<object[]> AsObjects(Stream stream, char delimiter, ref Column[] columns, bool hasHeaderRow)
        {
            var allColumnsAreStrings = columns == null;
            var stringArrays = AsStrings(stream, delimiter, ref columns, hasHeaderRow, AutoTruncate.Zap);

            if (allColumnsAreStrings)
            {
                return stringArrays;
            }

            var list = new List<object[]>();
            var _columns = columns;   // non-ref variable for use in lambda expression
            stringArrays.Iterate
            (
                (array, index) =>
                {
                    var oArray = new object[_columns.Length];
                    for (int i = 0; i < _columns.Length; i++)
                    {
                        oArray[i] = ConvertDataElement(array[i], _columns[i], "data row " + index);
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

        private static readonly List<string> _list = new List<string>();
        private static readonly StringBuilder _valueTemp = new StringBuilder();

        private static string[] ParseStrings(string rawRow, char delimiter, int lineNum, AutoTruncate autoTrunc)
        {
            _list.Clear();
            _valueTemp.Clear();
            var inQuotes = false;
            foreach (char c in rawRow)
            {
                if (c == delimiter && !inQuotes)
                {
                    _list.Add(PostParseString(_valueTemp.ToString(), autoTrunc));
                    _valueTemp.Clear();
                }
                else if (c == '"' && delimiter != '"')
                {
                    inQuotes = !inQuotes;
                }
                else
                {
                    _valueTemp.Append(c);
                }
            }
            if (inQuotes)
            {
                throw new UtilityException("Unclosed quotation marks detected on line " + lineNum);
            }
            _list.Add(PostParseString(_valueTemp.ToString(), autoTrunc));
            _valueTemp.Clear();

            if (_list.Count == 1 && Zap(_list[0]) == null)
            {
                return null;
            }

            return _list.ToArray();
        }
    }
}
