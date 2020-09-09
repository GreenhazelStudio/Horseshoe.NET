using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Horseshoe.NET.Collections.Extensions;
using Horseshoe.NET.Objects;
using Horseshoe.NET.Text;

namespace Horseshoe.NET.IO.FileImport
{
    public static class Import
    {
        [SuppressMessage("Design", "CA1034:Nested types should not be visible", Justification = "This is my style")]
        public static class CommaDelimited
        {
            public static DataTable AsDataTable(string filePath, bool hasHeaderRow = false)
            {
                using (var stream = File.OpenRead(filePath))
                {
                    return AsDataTable(stream, hasHeaderRow: hasHeaderRow);
                }
            }

            public static DataTable AsDataTable(string filePath, Column[] columns, bool hasHeaderRow = false)
            {
                using (var stream = File.OpenRead(filePath))
                {
                    return AsDataTable(stream, columns, hasHeaderRow: hasHeaderRow);
                }
            }

            public static DataTable AsDataTable(string filePath, out Column[] columns, bool hasHeaderRow = false)
            {
                using (var stream = File.OpenRead(filePath))
                {
                    return AsDataTable(stream, out columns, hasHeaderRow: hasHeaderRow);
                }
            }

            public static DataTable AsDataTable(string text, Encoding encoding, bool hasHeaderRow = false)
            {
                using (var stream = new MemoryStream((encoding ?? Encoding.ASCII).GetBytes(text)))
                {
                    return AsDataTable(stream, hasHeaderRow: hasHeaderRow);
                }
            }

            public static DataTable AsDataTable(string text, Encoding encoding, Column[] columns, bool hasHeaderRow = false)
            {
                using (var stream = new MemoryStream((encoding ?? Encoding.ASCII).GetBytes(text)))
                {
                    return AsDataTable(stream, columns, hasHeaderRow: hasHeaderRow);
                }
            }

            public static DataTable AsDataTable(string text, Encoding encoding, out Column[] columns, bool hasHeaderRow = false)
            {
                using (var stream = new MemoryStream((encoding ?? Encoding.ASCII).GetBytes(text)))
                {
                    return AsDataTable(stream, out columns, hasHeaderRow: hasHeaderRow);
                }
            }

            public static DataTable AsDataTable(Stream stream, bool hasHeaderRow = false)
            {
                return Delimited.AsDataTable(stream, ',', hasHeaderRow: hasHeaderRow);
            }

            public static DataTable AsDataTable(Stream stream, Column[] columns, bool hasHeaderRow = false)
            {
                return Delimited.AsDataTable(stream, ',', columns, hasHeaderRow: hasHeaderRow);
            }

            public static DataTable AsDataTable(Stream stream, out Column[] columns, bool hasHeaderRow = false)
            {
                return Delimited.AsDataTable(stream, ',', out columns, hasHeaderRow: hasHeaderRow);
            }

            public static IEnumerable<string[]> AsStrings(string filePath, bool hasHeaderRow = false, AutoTruncate autoTrunc = AutoTruncate.Trim)
            {
                using (var stream = File.OpenRead(filePath))
                {
                    return AsStrings(stream, hasHeaderRow: hasHeaderRow, autoTrunc: autoTrunc);
                }
            }

            public static IEnumerable<string[]> AsStrings(string filePath, Column[] columns, bool hasHeaderRow = false, AutoTruncate autoTrunc = AutoTruncate.Trim)
            {
                using (var stream = File.OpenRead(filePath))
                {
                    return AsStrings(stream, columns, hasHeaderRow: hasHeaderRow, autoTrunc: autoTrunc);
                }
            }

            public static IEnumerable<string[]> AsStrings(string filePath, out Column[] columns, bool hasHeaderRow = false, AutoTruncate autoTrunc = AutoTruncate.Trim)
            {
                using (var stream = File.OpenRead(filePath))
                {
                    return AsStrings(stream, out columns, hasHeaderRow: hasHeaderRow, autoTrunc: autoTrunc);
                }
            }

            public static IEnumerable<string[]> AsStrings(string text, Encoding encoding, bool hasHeaderRow = false, AutoTruncate autoTrunc = AutoTruncate.Trim)
            {
                using (var stream = new MemoryStream((encoding ?? Encoding.ASCII).GetBytes(text)))
                {
                    return AsStrings(stream, hasHeaderRow: hasHeaderRow, autoTrunc: autoTrunc);
                }
            }

            public static IEnumerable<string[]> AsStrings(string text, Encoding encoding, Column[] columns, bool hasHeaderRow = false, AutoTruncate autoTrunc = AutoTruncate.Trim)
            {
                using (var stream = new MemoryStream((encoding ?? Encoding.ASCII).GetBytes(text)))
                {
                    return AsStrings(stream, columns, hasHeaderRow: hasHeaderRow, autoTrunc: autoTrunc);
                }
            }

            public static IEnumerable<string[]> AsStrings(string text, Encoding encoding, out Column[] columns, bool hasHeaderRow = false, AutoTruncate autoTrunc = AutoTruncate.Trim)
            {
                using (var stream = new MemoryStream((encoding ?? Encoding.ASCII).GetBytes(text)))
                {
                    return AsStrings(stream, out columns, hasHeaderRow: hasHeaderRow, autoTrunc: autoTrunc);
                }
            }

            public static IEnumerable<string[]> AsStrings(Stream stream, bool hasHeaderRow = false, AutoTruncate autoTrunc = AutoTruncate.Trim)
            {
                return Delimited.AsStrings(stream, ',', hasHeaderRow: hasHeaderRow, autoTrunc: autoTrunc);
            }

            public static IEnumerable<string[]> AsStrings(Stream stream, Column[] columns, bool hasHeaderRow = false, AutoTruncate autoTrunc = AutoTruncate.Trim)
            {
                return Delimited.AsStrings(stream, ',', columns, hasHeaderRow: hasHeaderRow, autoTrunc: autoTrunc);
            }

            public static IEnumerable<string[]> AsStrings(Stream stream, out Column[] columns, bool hasHeaderRow = false, AutoTruncate autoTrunc = AutoTruncate.Trim)
            {
                return Delimited.AsStrings(stream, ',', out columns, hasHeaderRow: hasHeaderRow, autoTrunc: autoTrunc);
            }

            public static IEnumerable<object[]> AsObjects(string filePath, bool hasHeaderRow = false)
            {
                using (var stream = File.OpenRead(filePath))
                {
                    return AsObjects(stream, hasHeaderRow: hasHeaderRow);
                }
            }

            public static IEnumerable<object[]> AsObjects(string filePath, Column[] columns, bool hasHeaderRow = false)
            {
                using (var stream = File.OpenRead(filePath))
                {
                    return AsObjects(stream, columns, hasHeaderRow: hasHeaderRow);
                }
            }

            public static IEnumerable<object[]> AsObjects(string filePath, out Column[] columns, bool hasHeaderRow = false)
            {
                using (var stream = File.OpenRead(filePath))
                {
                    return AsObjects(stream, out columns, hasHeaderRow: hasHeaderRow);
                }
            }

            public static IEnumerable<object[]> AsObjects(string text, Encoding encoding, bool hasHeaderRow = false)
            {
                using (var stream = new MemoryStream((encoding ?? Encoding.ASCII).GetBytes(text)))
                {
                    return AsObjects(stream, hasHeaderRow: hasHeaderRow);
                }
            }

            public static IEnumerable<object[]> AsObjects(string text, Encoding encoding, Column[] columns, bool hasHeaderRow = false)
            {
                using (var stream = new MemoryStream((encoding ?? Encoding.ASCII).GetBytes(text)))
                {
                    return AsObjects(stream, columns, hasHeaderRow: hasHeaderRow);
                }
            }

            public static IEnumerable<object[]> AsObjects(string text, Encoding encoding, out Column[] columns, bool hasHeaderRow = false)
            {
                using (var stream = new MemoryStream((encoding ?? Encoding.ASCII).GetBytes(text)))
                {
                    return AsObjects(stream, out columns, hasHeaderRow: hasHeaderRow);
                }
            }

            public static IEnumerable<object[]> AsObjects(Stream stream, bool hasHeaderRow = false)
            {
                return Delimited.AsObjects(stream, ',', hasHeaderRow: hasHeaderRow);
            }

            public static IEnumerable<object[]> AsObjects(Stream stream, Column[] columns, bool hasHeaderRow = false)
            {
                return Delimited.AsObjects(stream, ',', columns, hasHeaderRow: hasHeaderRow);
            }

            public static IEnumerable<object[]> AsObjects(Stream stream, out Column[] columns, bool hasHeaderRow = false)
            {
                return Delimited.AsObjects(stream, ',', out columns, hasHeaderRow: hasHeaderRow);
            }

            public static IEnumerable<E> AsCollection<E>(string filePath, Column[] columns, bool hasHeaderRow = false, char[] charsToRemove = null) where E : class, new()
            {
                using (var stream = File.OpenRead(filePath))
                {
                    return AsCollection<E>(stream, columns, hasHeaderRow: hasHeaderRow, charsToRemove: charsToRemove);
                }
            }

            public static IEnumerable<E> AsCollection<E>(string filePath, Func<string[], E> parseFunc, bool hasHeaderRow = false)
            {
                using (var stream = File.OpenRead(filePath))
                {
                    return AsCollection<E>(stream, parseFunc, hasHeaderRow: hasHeaderRow);
                }
            }

            public static IEnumerable<E> AsCollection<E>(string text, Encoding encoding, Column[] columns, bool hasHeaderRow = false, char[] charsToRemove = null) where E : class, new()
            {
                using (var stream = new MemoryStream((encoding ?? Encoding.ASCII).GetBytes(text)))
                {
                    return AsCollection<E>(stream, columns, hasHeaderRow: hasHeaderRow, charsToRemove: charsToRemove);
                }
            }

            public static IEnumerable<E> AsCollection<E>(string text, Encoding encoding, Func<string[], E> parseFunc, bool hasHeaderRow = false)
            {
                using (var stream = new MemoryStream((encoding ?? Encoding.ASCII).GetBytes(text)))
                {
                    return AsCollection<E>(stream, parseFunc, hasHeaderRow: hasHeaderRow);
                }
            }

            public static IEnumerable<E> AsCollection<E>(Stream stream, Column[] columns, bool hasHeaderRow = false, char[] charsToRemove = null) where E : class, new()
            {
                return Delimited.AsCollection<E>(stream, ',', columns, hasHeaderRow: hasHeaderRow, charsToRemove: charsToRemove);
            }

            public static IEnumerable<E> AsCollection<E>(Stream stream, Func<string[], E> parseFunc, bool hasHeaderRow = false)
            {
                return Delimited.AsCollection<E>(stream, ',', parseFunc, hasHeaderRow: hasHeaderRow);
            }
        }

        [SuppressMessage("Design", "CA1034:Nested types should not be visible", Justification = "This is my style")]
        public static class Delimited
        {
            public static DataTable AsDataTable(string filePath, char delimiter, bool hasHeaderRow = false)
            {
                using (var stream = File.OpenRead(filePath))
                {
                    return AsDataTable(stream, delimiter, hasHeaderRow: hasHeaderRow);
                }
            }

            public static DataTable AsDataTable(string filePath, char delimiter, Column[] columns, bool hasHeaderRow = false)
            {
                using (var stream = File.OpenRead(filePath))
                {
                    return AsDataTable(stream, delimiter, columns, hasHeaderRow: hasHeaderRow);
                }
            }

            public static DataTable AsDataTable(string filePath, char delimiter, out Column[] columns, bool hasHeaderRow = false)
            {
                using (var stream = File.OpenRead(filePath))
                {
                    return AsDataTable(stream, delimiter, out columns, hasHeaderRow: hasHeaderRow);
                }
            }

            public static DataTable AsDataTable(string text, Encoding encoding, char delimiter, bool hasHeaderRow = false, DataErrorHandlingPolicy dataErrorHandling = DataErrorHandlingPolicy.Throw)
            {
                using (var stream = new MemoryStream((encoding ?? Encoding.ASCII).GetBytes(text)))
                {
                    return AsDataTable(stream, delimiter, hasHeaderRow: hasHeaderRow, dataErrorHandling: dataErrorHandling);
                }
            }

            public static DataTable AsDataTable(string text, Encoding encoding, char delimiter, Column[] columns, bool hasHeaderRow = false)
            {
                using (var stream = new MemoryStream((encoding ?? Encoding.ASCII).GetBytes(text)))
                {
                    return AsDataTable(stream, delimiter, columns, hasHeaderRow: hasHeaderRow);
                }
            }

            public static DataTable AsDataTable(string text, Encoding encoding, char delimiter, out Column[] columns, bool hasHeaderRow = false)
            {
                using (var stream = new MemoryStream((encoding ?? Encoding.ASCII).GetBytes(text)))
                {
                    return AsDataTable(stream, delimiter, out columns, hasHeaderRow: hasHeaderRow);
                }
            }

            public static DataTable AsDataTable(Stream stream, char delimiter, bool hasHeaderRow = false, DataErrorHandlingPolicy dataErrorHandling = DataErrorHandlingPolicy.Throw)
            {
                Column[] columns = null;

                // get imported data - ensures columns is not null
                var objectArrays = ImportDelimited.AsObjects(stream, delimiter, ref columns, hasHeaderRow, dataErrorHandling);

                return ImportDelimited.AsDataTable(objectArrays, columns);
            }

            public static DataTable AsDataTable(Stream stream, char delimiter, Column[] columns, bool hasHeaderRow = false, DataErrorHandlingPolicy dataErrorHandling = DataErrorHandlingPolicy.Throw)
            {
                // get imported data - ensures columns is not null
                var objectArrays = ImportDelimited.AsObjects(stream, delimiter, ref columns, hasHeaderRow, dataErrorHandling);

                return ImportDelimited.AsDataTable(objectArrays, columns);
            }

            public static DataTable AsDataTable(Stream stream, char delimiter, out Column[] columns, bool hasHeaderRow = false, DataErrorHandlingPolicy dataErrorHandling = DataErrorHandlingPolicy.Throw)
            {
                Column[] _columns = null;

                // get imported data - ensures columns is not null
                var objectArrays = ImportDelimited.AsObjects(stream, delimiter, ref _columns, hasHeaderRow, dataErrorHandling);

                columns = _columns;
                return ImportDelimited.AsDataTable(objectArrays, _columns);
            }

            public static IEnumerable<string[]> AsStrings(string filePath, char delimiter, bool hasHeaderRow = false, AutoTruncate autoTrunc = AutoTruncate.Trim)
            {
                using (var stream = File.OpenRead(filePath))
                {
                    return AsStrings(stream, delimiter, hasHeaderRow: hasHeaderRow, autoTrunc: autoTrunc);
                }
            }

            public static IEnumerable<string[]> AsStrings(string filePath, char delimiter, Column[] columns, bool hasHeaderRow = false, AutoTruncate autoTrunc = AutoTruncate.Trim)
            {
                using (var stream = File.OpenRead(filePath))
                {
                    return AsStrings(stream, delimiter, columns, hasHeaderRow: hasHeaderRow, autoTrunc: autoTrunc);
                }
            }

            public static IEnumerable<string[]> AsStrings(string filePath, char delimiter, out Column[] columns, bool hasHeaderRow = false, AutoTruncate autoTrunc = AutoTruncate.Trim)
            {
                using (var stream = File.OpenRead(filePath))
                {
                    return AsStrings(stream, delimiter, out columns, hasHeaderRow: hasHeaderRow, autoTrunc: autoTrunc);
                }
            }

            public static IEnumerable<string[]> AsStrings(string text, Encoding encoding, char delimiter, bool hasHeaderRow = false, AutoTruncate autoTrunc = AutoTruncate.Trim)
            {
                using (var stream = new MemoryStream((encoding ?? Encoding.ASCII).GetBytes(text)))
                {
                    return AsStrings(stream, delimiter, hasHeaderRow: hasHeaderRow, autoTrunc: autoTrunc);
                }
            }

            public static IEnumerable<string[]> AsStrings(string text, Encoding encoding, char delimiter, Column[] columns, bool hasHeaderRow = false, AutoTruncate autoTrunc = AutoTruncate.Trim)
            {
                using (var stream = new MemoryStream((encoding ?? Encoding.ASCII).GetBytes(text)))
                {
                    return AsStrings(stream, delimiter, columns, hasHeaderRow: hasHeaderRow, autoTrunc: autoTrunc);
                }
            }

            public static IEnumerable<string[]> AsStrings(string text, Encoding encoding, char delimiter, out Column[] columns, bool hasHeaderRow = false, AutoTruncate autoTrunc = AutoTruncate.Trim)
            {
                using (var stream = new MemoryStream((encoding ?? Encoding.ASCII).GetBytes(text)))
                {
                    return AsStrings(stream, delimiter, out columns, hasHeaderRow: hasHeaderRow, autoTrunc: autoTrunc);
                }
            }

            public static IEnumerable<string[]> AsStrings(Stream stream, char delimiter, bool hasHeaderRow = false, AutoTruncate autoTrunc = AutoTruncate.Trim)
            {
                Column[] columns = null;

                // get imported data - ensures columns is not null
                var stringArrays = ImportDelimited.AsStrings(stream, delimiter, ref columns, hasHeaderRow, autoTrunc);

                return stringArrays;
            }

            public static IEnumerable<string[]> AsStrings(Stream stream, char delimiter, Column[] columns, bool hasHeaderRow = false, AutoTruncate autoTrunc = AutoTruncate.Trim)
            {
                return ImportDelimited.AsStrings(stream, delimiter, ref columns, hasHeaderRow, autoTrunc);
            }

            public static IEnumerable<string[]> AsStrings(Stream stream, char delimiter, out Column[] columns, bool hasHeaderRow = false, AutoTruncate autoTrunc = AutoTruncate.Trim)
            {
                Column[] _columns = null;

                // get imported data - ensures columns is not null
                var stringArrays = ImportDelimited.AsStrings(stream, delimiter, ref _columns, hasHeaderRow, autoTrunc);

                columns = _columns;
                return stringArrays;
            }

            public static IEnumerable<object[]> AsObjects(string filePath, char delimiter, bool hasHeaderRow = false)
            {
                using (var stream = File.OpenRead(filePath))
                {
                    return AsObjects(stream, delimiter, hasHeaderRow: hasHeaderRow);
                }
            }

            public static IEnumerable<object[]> AsObjects(string filePath, char delimiter, Column[] columns, bool hasHeaderRow = false)
            {
                using (var stream = File.OpenRead(filePath))
                {
                    return AsObjects(stream, delimiter, columns, hasHeaderRow: hasHeaderRow);
                }
            }

            public static IEnumerable<object[]> AsObjects(string filePath, char delimiter, out Column[] columns, bool hasHeaderRow = false)
            {
                using (var stream = File.OpenRead(filePath))
                {
                    return AsObjects(stream, delimiter, out columns, hasHeaderRow: hasHeaderRow);
                }
            }

            public static IEnumerable<object[]> AsObjects(string text, Encoding encoding, char delimiter, bool hasHeaderRow = false)
            {
                using (var stream = new MemoryStream((encoding ?? Encoding.ASCII).GetBytes(text)))
                {
                    return AsObjects(stream, delimiter, hasHeaderRow: hasHeaderRow);
                }
            }

            public static IEnumerable<object[]> AsObjects(string text, Encoding encoding, char delimiter, Column[] columns, bool hasHeaderRow = false)
            {
                using (var stream = new MemoryStream((encoding ?? Encoding.ASCII).GetBytes(text)))
                {
                    return AsObjects(stream, delimiter, columns, hasHeaderRow: hasHeaderRow);
                }
            }

            public static IEnumerable<object[]> AsObjects(string text, Encoding encoding, char delimiter, out Column[] columns, bool hasHeaderRow = false)
            {
                using (var stream = new MemoryStream((encoding ?? Encoding.ASCII).GetBytes(text)))
                {
                    return AsObjects(stream, delimiter, out columns, hasHeaderRow: hasHeaderRow);
                }
            }

            public static IEnumerable<object[]> AsObjects(Stream stream, char delimiter, bool hasHeaderRow = false, DataErrorHandlingPolicy dataErrorHandling = DataErrorHandlingPolicy.Throw)
            {
                Column[] columns = null;

                // get imported data - ensures columns is not null
                var objectArrays = ImportDelimited.AsObjects(stream, delimiter, ref columns, hasHeaderRow, dataErrorHandling);

                return objectArrays;
            }

            public static IEnumerable<object[]> AsObjects(Stream stream, char delimiter, Column[] columns, bool hasHeaderRow = false, DataErrorHandlingPolicy dataErrorHandling = DataErrorHandlingPolicy.Throw)
            {
                return ImportDelimited.AsObjects(stream, delimiter, ref columns, hasHeaderRow, dataErrorHandling);
            }

            public static IEnumerable<object[]> AsObjects(Stream stream, char delimiter, out Column[] columns, bool hasHeaderRow = false, DataErrorHandlingPolicy dataErrorHandling = DataErrorHandlingPolicy.Throw)
            {
                Column[] _columns = null;

                // get imported data - ensures columns is not null
                var objectArrays = ImportDelimited.AsObjects(stream, delimiter, ref _columns, hasHeaderRow, dataErrorHandling);

                columns = _columns;
                return objectArrays;
            }

            public static IEnumerable<E> AsCollection<E>(string filePath, char delimiter, Column[] columns, bool hasHeaderRow = false, char[] charsToRemove = null) where E : class, new()
            {
                using (var stream = File.OpenRead(filePath))
                {
                    return AsCollection<E>(stream, delimiter, columns, hasHeaderRow: hasHeaderRow, charsToRemove: charsToRemove);
                }
            }

            public static IEnumerable<E> AsCollection<E>(string filePath, char delimiter, Func<string[], E> parseFunc, bool hasHeaderRow = false)
            {
                using (var stream = File.OpenRead(filePath))
                {
                    return AsCollection<E>(stream, delimiter, parseFunc, hasHeaderRow: hasHeaderRow);
                }
            }

            public static IEnumerable<E> AsCollection<E>(string text, Encoding encoding, char delimiter, Column[] columns, bool hasHeaderRow = false, char[] charsToRemove = null) where E : class, new()
            {
                using (var stream = new MemoryStream((encoding ?? Encoding.ASCII).GetBytes(text)))
                {
                    return AsCollection<E>(stream, delimiter, columns, hasHeaderRow: hasHeaderRow, charsToRemove: charsToRemove);
                }
            }

            public static IEnumerable<E> AsCollection<E>(string text, Encoding encoding, char delimiter, Func<string[], E> parseFunc, bool hasHeaderRow = false)
            {
                using (var stream = new MemoryStream((encoding ?? Encoding.ASCII).GetBytes(text)))
                {
                    return AsCollection<E>(stream, delimiter, parseFunc, hasHeaderRow: hasHeaderRow);
                }
            }

            public static IEnumerable<E> AsCollection<E>(Stream stream, char delimiter, Column[] columns, bool hasHeaderRow = false, DataErrorHandlingPolicy dataErrorHandling = DataErrorHandlingPolicy.Throw, char[] charsToRemove = null) where E : class, new()
            {
                var list = new List<E>();
                // get imported data
                var objectArrays = ImportDelimited.AsObjects(stream, delimiter, ref columns, hasHeaderRow, dataErrorHandling);

                // characters to remove (whitespace is already handled) to aid in the conversion of column names to object properties
                charsToRemove = charsToRemove != null
                    ? charsToRemove.Concat(new[] { '/', '-', '.', ',' }).ToArray()
                    : new[] { '/', '-', '.', ',' };

                // extrapolate object property names from column names, then set property values
                objectArrays.Iterate
                (
                    (array) =>
                    {
                        E e = new E();
                    for (int i = 0; i < columns.Length; i++)
                    {
                        var propertyName = Zap.String(columns[i].Name, textCleanRules: new TextCleanRules { Mode = TextCleanMode.RemoveWhitespace, CharsToRemove = charsToRemove});
                            ObjectUtil.SetInstanceProperty(e, propertyName, array[i], ignoreCase: true);
                        }
                        list.Add(e);
                    }
                );


                return list;
            }

            public static IEnumerable<E> AsCollection<E>(Stream stream, char delimiter, Func<string[], E> parseFunc, bool hasHeaderRow = false)
            {
                Column[] columns = null;
                var list = new List<E>();

                // get imported data
                var stringArrays = ImportDelimited.AsStrings(stream, delimiter, ref columns, hasHeaderRow, AutoTruncate.Zap);

                stringArrays.Iterate
                (
                    (array) =>
                    {
                        var e = parseFunc.Invoke(array);
                        list.Add(e);
                    }
                );

                return list;
            }
        }

        [SuppressMessage("Design", "CA1034:Nested types should not be visible", Justification = "This is my style")]
        public static class FixedWidth
        {
            public static DataTable AsDataTable(string filePath, Column[] columns, AutoTruncate autoTrunc = AutoTruncate.Trim, DataErrorHandlingPolicy dataErrorHandling = DataErrorHandlingPolicy.Throw)
            {
                using (var stream = File.OpenRead(filePath))
                {
                    return AsDataTable(stream, columns, autoTrunc: autoTrunc, dataErrorHandling: dataErrorHandling);
                }
            }

            public static DataTable AsDataTable(string text, Encoding encoding, Column[] columns, AutoTruncate autoTrunc = AutoTruncate.Trim, DataErrorHandlingPolicy dataErrorHandling = DataErrorHandlingPolicy.Throw)
            {
                using (var stream = new MemoryStream((encoding ?? Encoding.ASCII).GetBytes(text)))
                {
                    return AsDataTable(stream, columns, autoTrunc: autoTrunc, dataErrorHandling: dataErrorHandling);
                }
            }

            public static DataTable AsDataTable(Stream stream, Column[] columns, AutoTruncate autoTrunc = AutoTruncate.Trim, DataErrorHandlingPolicy dataErrorHandling = DataErrorHandlingPolicy.Throw)
            {
                // get imported data - ensures columns is not null
                var objectArrays = ImportFixedWidth.AsObjects(stream, columns, autoTrunc, dataErrorHandling);

                return ImportFixedWidth.AsDataTable(objectArrays, columns.Where(c => c.IsMappable).ToArray());
            }

            public static IEnumerable<string[]> AsStrings(string filePath, Column[] columns, AutoTruncate autoTrunc = AutoTruncate.Trim)
            {
                using (var stream = File.OpenRead(filePath))
                {
                    return AsStrings(stream, columns, autoTrunc: autoTrunc);
                }
            }

            public static IEnumerable<string[]> AsStrings(string text, Encoding encoding, Column[] columns, AutoTruncate autoTrunc = AutoTruncate.Trim)
            {
                using (var stream = new MemoryStream((encoding ?? Encoding.ASCII).GetBytes(text)))
                {
                    return AsStrings(stream, columns, autoTrunc: autoTrunc);
                }
            }

            public static IEnumerable<string[]> AsStrings(Stream stream, Column[] columns, AutoTruncate autoTrunc = AutoTruncate.Trim)
            {
                return ImportFixedWidth.AsStrings(stream, columns, autoTrunc);
            }

            public static IEnumerable<object[]> AsObjects(string filePath, Column[] columns, AutoTruncate autoTrunc = AutoTruncate.Trim, DataErrorHandlingPolicy dataErrorHandling = DataErrorHandlingPolicy.Throw)
            {
                using (var stream = File.OpenRead(filePath))
                {
                    return AsObjects(stream, columns, autoTrunc: autoTrunc, dataErrorHandling: dataErrorHandling);
                }
            }

            public static IEnumerable<object[]> AsObjects(string text, Encoding encoding, Column[] columns, AutoTruncate autoTrunc = AutoTruncate.Trim, DataErrorHandlingPolicy dataErrorHandling = DataErrorHandlingPolicy.Throw)
            {
                using (var stream = new MemoryStream((encoding ?? Encoding.ASCII).GetBytes(text)))
                {
                    return AsObjects(stream, columns, autoTrunc: autoTrunc, dataErrorHandling: dataErrorHandling);
                }
            }

            public static IEnumerable<object[]> AsObjects(Stream stream, Column[] columns, AutoTruncate autoTrunc = AutoTruncate.Trim, DataErrorHandlingPolicy dataErrorHandling = DataErrorHandlingPolicy.Throw)
            {
                return ImportFixedWidth.AsObjects(stream, columns, autoTrunc, dataErrorHandling);
            }

            public static IEnumerable<E> AsCollection<E>(string filePath, Column[] columns, Func<object[], E> parseFunc = null, AutoTruncate autoTrunc = AutoTruncate.Trim, char[] charsToRemove = null) where E : class, new()
            {
                using (var stream = File.OpenRead(filePath))
                {
                    return AsCollection<E>(stream, columns, parseFunc: parseFunc, autoTrunc: autoTrunc, charsToRemove: charsToRemove);
                }
            }

            public static IEnumerable<E> AsCollection<E>(string text, Encoding encoding, Column[] columns, Func<object[], E> parseFunc = null, AutoTruncate autoTrunc = AutoTruncate.Trim, char[] charsToRemove = null) where E : class, new()
            {
                using (var stream = new MemoryStream((encoding ?? Encoding.ASCII).GetBytes(text)))
                {
                    return AsCollection<E>(stream, columns, parseFunc: parseFunc, autoTrunc: autoTrunc, charsToRemove: charsToRemove);
                }
            }

            public static IEnumerable<E> AsCollection<E>(Stream stream, Column[] columns, Func<object[], E> parseFunc = null, AutoTruncate autoTrunc = AutoTruncate.Trim, DataErrorHandlingPolicy dataErrorHandling = DataErrorHandlingPolicy.Throw, char[] charsToRemove = null) where E : class, new()
            {
                // get imported data
                var objectArrays = ImportFixedWidth.AsObjects(stream, columns, autoTrunc, dataErrorHandling);

                return ImportFixedWidth.AsCollection(objectArrays, columns.Where(c => c.IsMappable).ToArray(), parseFunc, charsToRemove);
            }

            public static IEnumerable<E> AsCollection<E>(Stream stream, out Column[] columns, Func<object[], E> parseFunc = null, AutoTruncate autoTrunc = AutoTruncate.Trim, DataErrorHandlingPolicy dataErrorHandling = DataErrorHandlingPolicy.Throw, char[] charsToRemove = null) where E : class, new()
            {
                Column[] _columns = null;

                // get imported data - ensures columns is not null
                var objectArrays = ImportFixedWidth.AsObjects(stream, _columns, autoTrunc, dataErrorHandling);

                columns = _columns;
                return ImportFixedWidth.AsCollection(objectArrays, _columns, parseFunc, charsToRemove);
            }
        }
    }
}
