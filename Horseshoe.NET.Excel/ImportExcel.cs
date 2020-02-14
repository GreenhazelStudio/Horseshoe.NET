using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Horseshoe.NET.Collections;
using Horseshoe.NET.IO.FileImport;
using static Horseshoe.NET.IO.FileImport.ImportUtil;
using Horseshoe.NET.Objects;
using Horseshoe.NET.Text;
using static Horseshoe.NET.Text.TextUtil;  // Zap(), etc.

using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

namespace Horseshoe.NET.Excel
{
    public static class ImportExcel
    {
        public static DataTable AsDataTable(string filePath, int sheetNum = 0, bool hasHeaderRow = false, BlankRowMode blankRowMode = default, bool suppressExcelErrors = false)
        {
            return AsDataTable(filePath, null as Column[], sheetNum: sheetNum, hasHeaderRow: hasHeaderRow, blankRowMode: blankRowMode, suppressExcelErrors: suppressExcelErrors);
        }

        public static DataTable AsDataTable(string filePath, Column[] columns, int sheetNum = 0, bool hasHeaderRow = false, bool validateHeaderRowFromColumns = false, BlankRowMode blankRowMode = default, bool suppressExcelErrors = false)
        {
            return AsDataTable(new FileInfo(filePath), columns, sheetNum: sheetNum, hasHeaderRow: hasHeaderRow, validateHeaderRowFromColumns: validateHeaderRowFromColumns, blankRowMode: blankRowMode, suppressExcelErrors: suppressExcelErrors);
        }

        public static DataTable AsDataTable(string filePath, out Column[] columns, int sheetNum = 0, bool hasHeaderRow = false, BlankRowMode blankRowMode = default, bool suppressExcelErrors = false)
        {
            return AsDataTable(new FileInfo(filePath), out columns, sheetNum: sheetNum, hasHeaderRow: hasHeaderRow, blankRowMode: blankRowMode, suppressExcelErrors: suppressExcelErrors);
        }

        public static DataTable AsDataTable(FileInfo file, int sheetNum = 0, bool hasHeaderRow = false, bool validateHeaderRowFromColumns = false, BlankRowMode blankRowMode = default, bool suppressExcelErrors = false)
        {
            return AsDataTable(file, null as Column[], sheetNum: sheetNum, hasHeaderRow: hasHeaderRow, validateHeaderRowFromColumns: validateHeaderRowFromColumns, blankRowMode: blankRowMode, suppressExcelErrors: suppressExcelErrors);
        }

        public static DataTable AsDataTable(FileInfo file, Column[] columns, int sheetNum = 0, bool hasHeaderRow = false, bool validateHeaderRowFromColumns = false, BlankRowMode blankRowMode = default, bool suppressExcelErrors = false)
        {
            var worksheet = GetWorksheet(file, sheetNum);
            var objectArrays = AsObjects(worksheet, ref columns, hasHeaderRow, validateHeaderRowFromColumns, blankRowMode, suppressExcelErrors);
            return AsDataTable(objectArrays, columns);
        }

        public static DataTable AsDataTable(FileInfo file, out Column[] columns, int sheetNum = 0, bool hasHeaderRow = false, BlankRowMode blankRowMode = default, bool suppressExcelErrors = false)
        {
            Column[] _columns = null;
            var worksheet = GetWorksheet(file, sheetNum);
            var objectArrays = AsObjects(worksheet, ref _columns, hasHeaderRow, false, blankRowMode, suppressExcelErrors);

            columns = _columns;
            return AsDataTable(objectArrays, _columns);
        }

        public static DataTable AsDataTable(Stream stream, int sheetNum = 0, bool hasHeaderRow = false, BlankRowMode blankRowMode = default, bool suppressExcelErrors = false)
        {
            return AsDataTable(stream, null as Column[], sheetNum: sheetNum, hasHeaderRow: hasHeaderRow, blankRowMode: blankRowMode, suppressExcelErrors: suppressExcelErrors);
        }

        public static DataTable AsDataTable(Stream stream, Column[] columns, int sheetNum = 0, bool hasHeaderRow = false, bool validateHeaderRowFromColumns = false, BlankRowMode blankRowMode = default, bool suppressExcelErrors = false)
        {
            if (stream is FileStream fileStream)
            {
                switch (Path.GetExtension(fileStream.Name).ToUpper())
                {
                    case ".XLS":
                        return AsDataTableFromXlsStream(stream, columns, sheetNum: sheetNum, hasHeaderRow: hasHeaderRow, validateHeaderRowFromColumns: validateHeaderRowFromColumns, blankRowMode: blankRowMode, suppressExcelErrors: suppressExcelErrors);
                    case ".XLSX":
                        return AsDataTableFromXlsxStream(stream, columns, sheetNum: sheetNum, hasHeaderRow: hasHeaderRow, validateHeaderRowFromColumns: validateHeaderRowFromColumns, blankRowMode: blankRowMode, suppressExcelErrors: suppressExcelErrors);
                }
                throw new UtilityException("At this time we only recognize .xls and .xlsx Excel extensions: " + fileStream.Name + " -- also you can try using AsDataTableFromXlsStream() or AsDataTableFromXlsxStream()");
            }
            throw new UtilityException("Expected System.IO.FileStream, found " + stream.GetType().Name);
        }

        public static DataTable AsDataTable(Stream stream, out Column[] columns, int sheetNum = 0, bool hasHeaderRow = false, BlankRowMode blankRowMode = default, bool suppressExcelErrors = false)
        {
            if (stream is FileStream fileStream)
            {
                switch (Path.GetExtension(fileStream.Name).ToUpper())
                {
                    case ".XLS":
                        return AsDataTableFromXlsStream(stream, out columns, sheetNum: sheetNum, hasHeaderRow: hasHeaderRow, blankRowMode: blankRowMode, suppressExcelErrors: suppressExcelErrors);
                    case ".XLSX":
                        return AsDataTableFromXlsxStream(stream, out columns, sheetNum: sheetNum, hasHeaderRow: hasHeaderRow, blankRowMode: blankRowMode, suppressExcelErrors: suppressExcelErrors);
                }
                throw new UtilityException("At this time we only recognize .xls and .xlsx Excel extensions: " + fileStream.Name + " -- also you can try using AsDataTableFromXlsStream() or AsDataTableFromXlsxStream()");
            }
            throw new UtilityException("Expected System.IO.FileStream, found " + stream.GetType().Name);
        }

        public static DataTable AsDataTableFromXlsStream(Stream stream, int sheetNum = 0, bool hasHeaderRow = false, BlankRowMode blankRowMode = default, bool suppressExcelErrors = false)
        {
            return AsDataTableFromXlsStream(stream, null as Column[], sheetNum: sheetNum, hasHeaderRow: hasHeaderRow, blankRowMode: blankRowMode, suppressExcelErrors: suppressExcelErrors);
        }

        public static DataTable AsDataTableFromXlsStream(Stream stream, Column[] columns, int sheetNum = 0, bool hasHeaderRow = false, bool validateHeaderRowFromColumns = false, BlankRowMode blankRowMode = default, bool suppressExcelErrors = false)
        {
            var workbook = new HSSFWorkbook(stream);
            var worksheet = workbook.GetSheetAt(sheetNum);
            var objectArrays = AsObjects(worksheet, ref columns, hasHeaderRow, validateHeaderRowFromColumns, blankRowMode, suppressExcelErrors);

            return AsDataTable(objectArrays, columns);
        }

        public static DataTable AsDataTableFromXlsStream(Stream stream, out Column[] columns, int sheetNum = 0, bool hasHeaderRow = false, BlankRowMode blankRowMode = default, bool suppressExcelErrors = false)
        {
            Column[] _columns = null;
            var workbook = new HSSFWorkbook(stream);
            var worksheet = workbook.GetSheetAt(sheetNum);
            var objectArrays = AsObjects(worksheet, ref _columns, hasHeaderRow, false, blankRowMode, suppressExcelErrors);

            columns = _columns;
            return AsDataTable(objectArrays, _columns);
        }

        public static DataTable AsDataTableFromXlsxStream(Stream stream, int sheetNum = 0, bool hasHeaderRow = false, BlankRowMode blankRowMode = default, bool suppressExcelErrors = false)
        {
            return AsDataTableFromXlsxStream(stream, null as Column[], sheetNum: sheetNum, hasHeaderRow: hasHeaderRow, blankRowMode: blankRowMode, suppressExcelErrors: suppressExcelErrors);
        }

        public static DataTable AsDataTableFromXlsxStream(Stream stream, Column[] columns, int sheetNum = 0, bool hasHeaderRow = false, bool validateHeaderRowFromColumns = false, BlankRowMode blankRowMode = default, bool suppressExcelErrors = false)
        {
            var workbook = new XSSFWorkbook(stream);
            var worksheet = workbook.GetSheetAt(sheetNum);
            var objectArrays = AsObjects(worksheet, ref columns, hasHeaderRow, validateHeaderRowFromColumns, blankRowMode, suppressExcelErrors);

            return AsDataTable(objectArrays, columns);
        }

        public static DataTable AsDataTableFromXlsxStream(Stream stream, out Column[] columns, int sheetNum = 0, bool hasHeaderRow = false, BlankRowMode blankRowMode = default, bool suppressExcelErrors = false)
        {
            Column[] _columns = null;
            var workbook = new XSSFWorkbook(stream);
            var worksheet = workbook.GetSheetAt(sheetNum);
            var objectArrays = AsObjects(worksheet, ref _columns, hasHeaderRow, false, blankRowMode, suppressExcelErrors);

            columns = _columns;
            return AsDataTable(objectArrays, _columns);
        }

        public static IEnumerable<string[]> AsStrings(string filePath, int sheetNum = 0, bool hasHeaderRow = false, BlankRowMode blankRowMode = default, AutoTruncate autoTrunc = AutoTruncate.Trim, bool suppressExcelErrors = false)
        {
            return AsStrings(filePath, null as Column[], sheetNum: sheetNum, hasHeaderRow: hasHeaderRow, blankRowMode: blankRowMode, autoTrunc: autoTrunc, suppressExcelErrors: suppressExcelErrors);
        }

        public static IEnumerable<string[]> AsStrings(string filePath, Column[] columns, int sheetNum = 0, bool hasHeaderRow = false, bool validateHeaderRowFromColumns = false, BlankRowMode blankRowMode = default, AutoTruncate autoTrunc = AutoTruncate.Trim, bool suppressExcelErrors = false)
        {
            return AsStrings(new FileInfo(filePath), columns, sheetNum: sheetNum, hasHeaderRow: hasHeaderRow, validateHeaderRowFromColumns: validateHeaderRowFromColumns, blankRowMode: blankRowMode, autoTrunc: autoTrunc, suppressExcelErrors: suppressExcelErrors);
        }

        public static IEnumerable<string[]> AsStrings(string filePath, out Column[] columns, int sheetNum = 0, bool hasHeaderRow = false, BlankRowMode blankRowMode = default, AutoTruncate autoTrunc = AutoTruncate.Trim, bool suppressExcelErrors = false)
        {
            return AsStrings(new FileInfo(filePath), out columns, sheetNum: sheetNum, hasHeaderRow: hasHeaderRow, blankRowMode: blankRowMode, autoTrunc: autoTrunc, suppressExcelErrors: suppressExcelErrors);
        }

        public static IEnumerable<string[]> AsStrings(FileInfo file, int sheetNum = 0, bool hasHeaderRow = false, BlankRowMode blankRowMode = default, AutoTruncate autoTrunc = AutoTruncate.Trim, bool suppressExcelErrors = false)
        {
            return AsStrings(file, null as Column[], sheetNum: sheetNum, hasHeaderRow: hasHeaderRow, blankRowMode: blankRowMode, autoTrunc: autoTrunc, suppressExcelErrors: suppressExcelErrors);
        }

        public static IEnumerable<string[]> AsStrings(FileInfo file, Column[] columns, int sheetNum = 0, bool hasHeaderRow = false, bool validateHeaderRowFromColumns = false, BlankRowMode blankRowMode = default, AutoTruncate autoTrunc = AutoTruncate.Trim, bool suppressExcelErrors = false)
        {
            var worksheet = GetWorksheet(file, sheetNum);
            return AsStrings(worksheet, ref columns, hasHeaderRow, validateHeaderRowFromColumns, blankRowMode, autoTrunc, suppressExcelErrors);
        }

        public static IEnumerable<string[]> AsStrings(FileInfo file, out Column[] columns, int sheetNum = 0, bool hasHeaderRow = false, BlankRowMode blankRowMode = default, AutoTruncate autoTrunc = AutoTruncate.Trim, bool suppressExcelErrors = false)
        {
            var worksheet = GetWorksheet(file, sheetNum);
            Column[] _columns = null;
            var stringArrays = AsStrings(worksheet, ref _columns, hasHeaderRow, false, blankRowMode, autoTrunc, suppressExcelErrors);
            columns = _columns;
            return stringArrays;
        }

        public static IEnumerable<string[]> AsStrings(Stream stream, int sheetNum = 0, bool hasHeaderRow = false, BlankRowMode blankRowMode = default, AutoTruncate autoTrunc = AutoTruncate.Trim, bool suppressExcelErrors = false)
        {
            return AsStrings(stream, null as Column[], sheetNum: sheetNum, hasHeaderRow: hasHeaderRow, blankRowMode: blankRowMode, autoTrunc: autoTrunc, suppressExcelErrors: suppressExcelErrors);
        }

        public static IEnumerable<string[]> AsStrings(Stream stream, Column[] columns, int sheetNum = 0, bool hasHeaderRow = false, bool validateHeaderRowFromColumns = false, BlankRowMode blankRowMode = default, AutoTruncate autoTrunc = AutoTruncate.Trim, bool suppressExcelErrors = false)
        {
            if (stream is FileStream fileStream)
            {
                switch (Path.GetExtension(fileStream.Name).ToUpper())
                {
                    case ".XLS":
                        return AsStringsFromXlsStream(stream, columns, sheetNum: sheetNum, hasHeaderRow: hasHeaderRow, validateHeaderRowFromColumns: validateHeaderRowFromColumns, blankRowMode: blankRowMode, autoTrunc: autoTrunc, suppressExcelErrors: suppressExcelErrors);
                    case ".XLSX":
                        return AsStringsFromXlsxStream(stream, columns, sheetNum: sheetNum, hasHeaderRow: hasHeaderRow, validateHeaderRowFromColumns: validateHeaderRowFromColumns, blankRowMode: blankRowMode, autoTrunc: autoTrunc, suppressExcelErrors: suppressExcelErrors);
                }
                throw new UtilityException("At this time we only recognize .xls and .xlsx Excel extensions: " + fileStream.Name + " -- also you can try using AsDataTableFromXlsStream() or AsDataTableFromXlsxStream()");
            }
            throw new UtilityException("Expected System.IO.FileStream, found " + stream.GetType().Name);
        }

        public static IEnumerable<string[]> AsStrings(Stream stream, out Column[] columns, int sheetNum = 0, bool hasHeaderRow = false, BlankRowMode blankRowMode = default, AutoTruncate autoTrunc = AutoTruncate.Trim, bool suppressExcelErrors = false)
        {
            if (stream is FileStream fileStream)
            {
                switch (Path.GetExtension(fileStream.Name).ToUpper())
                {
                    case ".XLS":
                        return AsStringsFromXlsStream(stream, out columns, sheetNum: sheetNum, hasHeaderRow: hasHeaderRow, blankRowMode: blankRowMode, autoTrunc: autoTrunc, suppressExcelErrors: suppressExcelErrors);
                    case ".XLSX":
                        return AsStringsFromXlsxStream(stream, out columns, sheetNum: sheetNum, hasHeaderRow: hasHeaderRow, blankRowMode: blankRowMode, autoTrunc: autoTrunc, suppressExcelErrors: suppressExcelErrors);
                }
                throw new UtilityException("At this time we only recognize .xls and .xlsx Excel extensions: " + fileStream.Name + " -- also you can try using AsDataTableFromXlsStream() or AsDataTableFromXlsxStream()");
            }
            throw new UtilityException("Expected System.IO.FileStream, found " + stream.GetType().Name);
        }

        public static IEnumerable<string[]> AsStringsFromXlsStream(Stream stream, int sheetNum = 0, bool hasHeaderRow = false, BlankRowMode blankRowMode = default, AutoTruncate autoTrunc = AutoTruncate.Trim, bool suppressExcelErrors = false)
        {
            return AsStringsFromXlsStream(stream, null as Column[], sheetNum: sheetNum, hasHeaderRow: hasHeaderRow, blankRowMode: blankRowMode, autoTrunc: autoTrunc, suppressExcelErrors: suppressExcelErrors);
        }

        public static IEnumerable<string[]> AsStringsFromXlsStream(Stream stream, Column[] columns, int sheetNum = 0, bool hasHeaderRow = false, bool validateHeaderRowFromColumns = false, BlankRowMode blankRowMode = default, AutoTruncate autoTrunc = AutoTruncate.Trim, bool suppressExcelErrors = false)
        {
            var workbook = new HSSFWorkbook(stream);
            var worksheet = workbook.GetSheetAt(sheetNum);
            return AsStrings(worksheet, ref columns, hasHeaderRow, validateHeaderRowFromColumns, blankRowMode, autoTrunc, suppressExcelErrors);
        }

        public static IEnumerable<string[]> AsStringsFromXlsStream(Stream stream, out Column[] columns, int sheetNum = 0, bool hasHeaderRow = false, BlankRowMode blankRowMode = default, AutoTruncate autoTrunc = AutoTruncate.Trim, bool suppressExcelErrors = false)
        {
            Column[] _columns = null;
            var workbook = new HSSFWorkbook(stream);
            var worksheet = workbook.GetSheetAt(sheetNum);
            var stringArrays = AsStrings(worksheet, ref _columns, hasHeaderRow, false, blankRowMode, autoTrunc, suppressExcelErrors);

            columns = _columns;
            return stringArrays;
        }

        public static IEnumerable<string[]> AsStringsFromXlsxStream(Stream stream, int sheetNum = 0, bool hasHeaderRow = false, BlankRowMode blankRowMode = default, AutoTruncate autoTrunc = AutoTruncate.Trim, bool suppressExcelErrors = false)
        {
            return AsStringsFromXlsxStream(stream, null as Column[], sheetNum: sheetNum, hasHeaderRow: hasHeaderRow, blankRowMode: blankRowMode, autoTrunc: autoTrunc, suppressExcelErrors: suppressExcelErrors);
        }

        public static IEnumerable<string[]> AsStringsFromXlsxStream(Stream stream, Column[] columns, int sheetNum = 0, bool hasHeaderRow = false, bool validateHeaderRowFromColumns = false, BlankRowMode blankRowMode = default, AutoTruncate autoTrunc = AutoTruncate.Trim, bool suppressExcelErrors = false)
        {
            var workbook = new XSSFWorkbook(stream);
            var worksheet = workbook.GetSheetAt(sheetNum);
            return AsStrings(worksheet, ref columns, hasHeaderRow, validateHeaderRowFromColumns, blankRowMode, autoTrunc, suppressExcelErrors);
        }

        public static IEnumerable<string[]> AsStringsFromXlsxStream(Stream stream, out Column[] columns, int sheetNum = 0, bool hasHeaderRow = false, BlankRowMode blankRowMode = default, AutoTruncate autoTrunc = AutoTruncate.Trim, bool suppressExcelErrors = false)
        {
            Column[] _columns = null;
            var workbook = new XSSFWorkbook(stream);
            var worksheet = workbook.GetSheetAt(sheetNum);
            var stringArrays = AsStrings(worksheet, ref _columns, hasHeaderRow, false, blankRowMode, autoTrunc, suppressExcelErrors);

            columns = _columns;
            return stringArrays;
        }

        public static IEnumerable<object[]> AsObjects(string filePath, int sheetNum = 0, bool hasHeaderRow = false, BlankRowMode blankRowMode = default, bool suppressExcelErrors = false)
        {
            return AsObjects(filePath, null as Column[], sheetNum: sheetNum, hasHeaderRow: hasHeaderRow, blankRowMode: blankRowMode, suppressExcelErrors: suppressExcelErrors);
        }

        public static IEnumerable<object[]> AsObjects(string filePath, Column[] columns, int sheetNum = 0, bool hasHeaderRow = false, bool validateHeaderRowFromColumns = false, BlankRowMode blankRowMode = default, bool suppressExcelErrors = false)
        {
            return AsObjects(new FileInfo(filePath), columns, sheetNum: sheetNum, hasHeaderRow: hasHeaderRow, validateHeaderRowFromColumns: validateHeaderRowFromColumns, blankRowMode: blankRowMode, suppressExcelErrors: suppressExcelErrors);
        }

        public static IEnumerable<object[]> AsObjects(string filePath, out Column[] columns, int sheetNum = 0, bool hasHeaderRow = false, BlankRowMode blankRowMode = default, bool suppressExcelErrors = false)
        {
            return AsObjects(new FileInfo(filePath), out columns, sheetNum: sheetNum, hasHeaderRow: hasHeaderRow, blankRowMode: blankRowMode, suppressExcelErrors: suppressExcelErrors);
        }

        public static IEnumerable<object[]> AsObjects(FileInfo file, int sheetNum = 0, bool hasHeaderRow = false, BlankRowMode blankRowMode = default, bool suppressExcelErrors = false)
        {
            return AsObjects(file, null as Column[], sheetNum: sheetNum, hasHeaderRow: hasHeaderRow, blankRowMode: blankRowMode, suppressExcelErrors: suppressExcelErrors);
        }

        public static IEnumerable<object[]> AsObjects(FileInfo file, Column[] columns, int sheetNum = 0, bool hasHeaderRow = false, bool validateHeaderRowFromColumns = false, BlankRowMode blankRowMode = default, bool suppressExcelErrors = false)
        {
            var worksheet = GetWorksheet(file, sheetNum);
            return AsObjects(worksheet, ref columns, hasHeaderRow, validateHeaderRowFromColumns, blankRowMode, suppressExcelErrors);
        }

        public static IEnumerable<object[]> AsObjects(FileInfo file, out Column[] columns, int sheetNum = 0, bool hasHeaderRow = false, BlankRowMode blankRowMode = default, bool suppressExcelErrors = false)
        {
            Column[] _columns = null;
            var worksheet = GetWorksheet(file, sheetNum);
            var objectArrays = AsObjects(worksheet, ref _columns, hasHeaderRow, false, blankRowMode, suppressExcelErrors);
            columns = _columns;
            return objectArrays;
        }

        public static IEnumerable<object[]> AsObjects(Stream stream, int sheetNum = 0, bool hasHeaderRow = false, BlankRowMode blankRowMode = default, bool suppressExcelErrors = false)
        {
            return AsObjects(stream, null as Column[], sheetNum: sheetNum, hasHeaderRow: hasHeaderRow, blankRowMode: blankRowMode, suppressExcelErrors: suppressExcelErrors);
        }

        public static IEnumerable<object[]> AsObjects(Stream stream, Column[] columns, int sheetNum = 0, bool hasHeaderRow = false, bool validateHeaderRowFromColumns = false, BlankRowMode blankRowMode = default, bool suppressExcelErrors = false)
        {
            if (stream is FileStream fileStream)
            {
                switch (Path.GetExtension(fileStream.Name).ToUpper())
                {
                    case ".XLS":
                        return AsObjectsFromXlsStream(stream, columns, sheetNum: sheetNum, hasHeaderRow: hasHeaderRow, validateHeaderRowFromColumns: validateHeaderRowFromColumns, blankRowMode: blankRowMode, suppressExcelErrors: suppressExcelErrors);
                    case ".XLSX":
                        return AsObjectsFromXlsxStream(stream, columns, sheetNum: sheetNum, hasHeaderRow: hasHeaderRow, validateHeaderRowFromColumns: validateHeaderRowFromColumns, blankRowMode: blankRowMode, suppressExcelErrors: suppressExcelErrors);
                }
                throw new UtilityException("At this time we only recognize .xls and .xlsx Excel extensions: " + fileStream.Name + " -- also you can try using AsDataTableFromXlsStream() or AsDataTableFromXlsxStream()");
            }
            throw new UtilityException("Expected System.IO.FileStream, found " + stream.GetType().Name);
        }

        public static IEnumerable<object[]> AsObjects(Stream stream, out Column[] columns, int sheetNum = 0, bool hasHeaderRow = false, BlankRowMode blankRowMode = default, bool suppressExcelErrors = false)
        {
            if (stream is FileStream fileStream)
            {
                switch (Path.GetExtension(fileStream.Name).ToUpper())
                {
                    case ".XLS":
                        return AsObjectsFromXlsStream(stream, out columns, sheetNum: sheetNum, hasHeaderRow: hasHeaderRow, blankRowMode: blankRowMode, suppressExcelErrors: suppressExcelErrors);
                    case ".XLSX":
                        return AsObjectsFromXlsxStream(stream, out columns, sheetNum: sheetNum, hasHeaderRow: hasHeaderRow, blankRowMode: blankRowMode, suppressExcelErrors: suppressExcelErrors);
                }
                throw new UtilityException("At this time we only recognize .xls and .xlsx Excel extensions: " + fileStream.Name + " -- also you can try using AsDataTableFromXlsStream() or AsDataTableFromXlsxStream()");
            }
            throw new UtilityException("Expected System.IO.FileStream, found " + stream.GetType().Name);
        }

        public static IEnumerable<object[]> AsObjectsFromXlsStream(Stream stream, int sheetNum = 0, bool hasHeaderRow = false, BlankRowMode blankRowMode = default, bool suppressExcelErrors = false)
        {
            return AsObjectsFromXlsStream(stream, null as Column[], sheetNum: sheetNum, hasHeaderRow: hasHeaderRow, blankRowMode: blankRowMode, suppressExcelErrors: suppressExcelErrors);
        }

        public static IEnumerable<object[]> AsObjectsFromXlsxStream(Stream stream, int sheetNum = 0, bool hasHeaderRow = false, BlankRowMode blankRowMode = default, bool suppressExcelErrors = false)
        {
            return AsObjectsFromXlsxStream(stream, null as Column[], sheetNum: sheetNum, hasHeaderRow: hasHeaderRow, blankRowMode: blankRowMode, suppressExcelErrors: suppressExcelErrors);
        }

        public static IEnumerable<object[]> AsObjectsFromXlsStream(Stream stream, Column[] columns, int sheetNum = 0, bool hasHeaderRow = false, bool validateHeaderRowFromColumns = false, BlankRowMode blankRowMode = default, bool suppressExcelErrors = false)
        {
            var workbook = new HSSFWorkbook(stream);
            var worksheet = workbook.GetSheetAt(sheetNum);
            return AsObjects(worksheet, ref columns, hasHeaderRow, validateHeaderRowFromColumns, blankRowMode, suppressExcelErrors);
        }

        public static IEnumerable<object[]> AsObjectsFromXlsxStream(Stream stream, Column[] columns, int sheetNum = 0, bool hasHeaderRow = false, bool validateHeaderRowFromColumns = false, BlankRowMode blankRowMode = default, bool suppressExcelErrors = false)
        {
            var workbook = new XSSFWorkbook(stream);
            var worksheet = workbook.GetSheetAt(sheetNum);
            return AsObjects(worksheet, ref columns, hasHeaderRow, validateHeaderRowFromColumns, blankRowMode, suppressExcelErrors);
        }

        public static IEnumerable<object[]> AsObjectsFromXlsStream(Stream stream, out Column[] columns, int sheetNum = 0, bool hasHeaderRow = false, BlankRowMode blankRowMode = default, bool suppressExcelErrors = false)
        {
            Column[] _columns = null;
            var workbook = new HSSFWorkbook(stream);
            var worksheet = workbook.GetSheetAt(sheetNum);
            var objectArrays = AsObjects(worksheet, ref _columns, hasHeaderRow, false, blankRowMode, suppressExcelErrors);

            columns = _columns;
            return objectArrays;
        }

        public static IEnumerable<object[]> AsObjectsFromXlsxStream(Stream stream, out Column[] columns, int sheetNum = 0, bool hasHeaderRow = false, BlankRowMode blankRowMode = default, bool suppressExcelErrors = false)
        {
            Column[] _columns = null;
            var workbook = new XSSFWorkbook(stream);
            var worksheet = workbook.GetSheetAt(sheetNum);
            var objectArrays = AsObjects(worksheet, ref _columns, hasHeaderRow, false, blankRowMode, suppressExcelErrors);

            columns = _columns;
            return objectArrays;
        }

        public static IEnumerable<E> AsCollection<E>(string filePath, Func<object[], E> parseFunc, int sheetNum = 0, bool hasHeaderRow = false, BlankRowMode blankRowMode = default, bool suppressExcelErrors = false)
        {
            return AsCollection<E>(new FileInfo(filePath), parseFunc, sheetNum: sheetNum, hasHeaderRow: hasHeaderRow, blankRowMode: blankRowMode, suppressExcelErrors: suppressExcelErrors);
        }

        public static IEnumerable<E> AsCollection<E>(string filePath, Column[] columns, int sheetNum = 0, bool hasHeaderRow = false, bool validateHeaderRowFromColumns = false, BlankRowMode blankRowMode = default, char[] charsToRemove = null, bool suppressExcelErrors = false) where E : class, new()
        {
            return AsCollection<E>(new FileInfo(filePath), columns, sheetNum: sheetNum, hasHeaderRow: hasHeaderRow, validateHeaderRowFromColumns: validateHeaderRowFromColumns, blankRowMode: blankRowMode, charsToRemove: charsToRemove, suppressExcelErrors: suppressExcelErrors);
        }

        public static IEnumerable<E> AsCollection<E>(FileInfo file, Func<object[], E> parseFunc, int sheetNum = 0, bool hasHeaderRow = false, BlankRowMode blankRowMode = default, bool suppressExcelErrors = false)
        {
            var worksheet = GetWorksheet(file, sheetNum);
            return AsCollection<E>(worksheet, parseFunc, hasHeaderRow: hasHeaderRow, blankRowMode: blankRowMode, suppressExcelErrors: suppressExcelErrors);
        }

        public static IEnumerable<E> AsCollection<E>(FileInfo file, Column[] columns, int sheetNum = 0, bool hasHeaderRow = false, bool validateHeaderRowFromColumns = false, BlankRowMode blankRowMode = default, char[] charsToRemove = null, bool suppressExcelErrors = false) where E : class, new()
        {
            var worksheet = GetWorksheet(file, sheetNum);
            return AsCollection<E>(worksheet, columns, hasHeaderRow: hasHeaderRow, validateHeaderRowFromColumns: validateHeaderRowFromColumns, blankRowMode: blankRowMode, charsToRemove: charsToRemove, suppressExcelErrors: suppressExcelErrors);
        }

        public static IEnumerable<E> AsCollection<E>(Stream stream, Func<object[], E> parseFunc, int sheetNum = 0, bool hasHeaderRow = false, BlankRowMode blankRowMode = default, bool suppressExcelErrors = false)
        {
            if (stream is FileStream fileStream)
            {
                switch (Path.GetExtension(fileStream.Name).ToUpper())
                {
                    case ".XLS":
                        return AsCollectionFromXlsStream<E>(stream, parseFunc, sheetNum: sheetNum, hasHeaderRow: hasHeaderRow, blankRowMode: blankRowMode, suppressExcelErrors: suppressExcelErrors);
                    case ".XLSX":
                        return AsCollectionFromXlsxStream<E>(stream, parseFunc, sheetNum: sheetNum, hasHeaderRow: hasHeaderRow, blankRowMode: blankRowMode, suppressExcelErrors: suppressExcelErrors);
                }
                throw new UtilityException("At this time we only recognize .xls and .xlsx Excel extensions: " + fileStream.Name + " -- also you can try using AsDataTableFromXlsStream() or AsDataTableFromXlsxStream()");
            }
            throw new UtilityException("Expected System.IO.FileStream, found " + stream.GetType().Name);
        }

        public static IEnumerable<E> AsCollection<E>(Stream stream, Column[] columns, int sheetNum = 0, bool hasHeaderRow = false, bool validateHeaderRowFromColumns = false, BlankRowMode blankRowMode = default, char[] charsToRemove = null, bool suppressExcelErrors = false) where E : class, new()
        {
            if (stream is FileStream fileStream)
            {
                switch (Path.GetExtension(fileStream.Name).ToUpper())
                {
                    case ".XLS":
                        return AsCollectionFromXlsStream<E>(stream, columns, sheetNum: sheetNum, hasHeaderRow: hasHeaderRow, validateHeaderRowFromColumns: validateHeaderRowFromColumns, blankRowMode: blankRowMode, charsToRemove: charsToRemove, suppressExcelErrors: suppressExcelErrors);
                    case ".XLSX":
                        return AsCollectionFromXlsxStream<E>(stream, columns, sheetNum: sheetNum, hasHeaderRow: hasHeaderRow, validateHeaderRowFromColumns: validateHeaderRowFromColumns, blankRowMode: blankRowMode, charsToRemove: charsToRemove, suppressExcelErrors: suppressExcelErrors);
                }
                throw new UtilityException("At this time we only recognize .xls and .xlsx Excel extensions: " + fileStream.Name + " -- also you can try using AsDataTableFromXlsStream() or AsDataTableFromXlsxStream()");
            }
            throw new UtilityException("Expected System.IO.FileStream, found " + stream.GetType().Name);
        }

        public static IEnumerable<E> AsCollectionFromXlsStream<E>(Stream stream, Func<object[], E> parseFunc, int sheetNum = 0, bool hasHeaderRow = false, BlankRowMode blankRowMode = default, bool suppressExcelErrors = false)
        {
            var workbook = new HSSFWorkbook(stream);
            var worksheet = workbook.GetSheetAt(sheetNum);
            return AsCollection<E>(worksheet, parseFunc, hasHeaderRow: hasHeaderRow, blankRowMode: blankRowMode, suppressExcelErrors: suppressExcelErrors);
        }

        public static IEnumerable<E> AsCollectionFromXlsStream<E>(Stream stream, Column[] columns, int sheetNum = 0, bool hasHeaderRow = false, bool validateHeaderRowFromColumns = false, BlankRowMode blankRowMode = default, char[] charsToRemove = null, bool suppressExcelErrors = false) where E : class, new()
        {
            var workbook = new HSSFWorkbook(stream);
            var worksheet = workbook.GetSheetAt(sheetNum);
            return AsCollection<E>(worksheet, columns, hasHeaderRow: hasHeaderRow, validateHeaderRowFromColumns: validateHeaderRowFromColumns, blankRowMode: blankRowMode, charsToRemove: charsToRemove, suppressExcelErrors: suppressExcelErrors);
        }

        public static IEnumerable<E> AsCollectionFromXlsxStream<E>(Stream stream, Func<object[], E> parseFunc, int sheetNum = 0, bool hasHeaderRow = false, BlankRowMode blankRowMode = default, bool suppressExcelErrors = false)
        {
            var workbook = new XSSFWorkbook(stream);
            var worksheet = workbook.GetSheetAt(sheetNum);
            return AsCollection<E>(worksheet, parseFunc, hasHeaderRow: hasHeaderRow, blankRowMode: blankRowMode, suppressExcelErrors: suppressExcelErrors);
        }

        public static IEnumerable<E> AsCollectionFromXlsxStream<E>(Stream stream, Column[] columns, int sheetNum = 0, bool hasHeaderRow = false, bool validateHeaderRowFromColumns = false, BlankRowMode blankRowMode = default, char[] charsToRemove = null, bool suppressExcelErrors = false) where E : class, new()
        {
            var workbook = new XSSFWorkbook(stream);
            var worksheet = workbook.GetSheetAt(sheetNum);
            return AsCollection<E>(worksheet, columns, hasHeaderRow: hasHeaderRow, validateHeaderRowFromColumns: validateHeaderRowFromColumns, blankRowMode: blankRowMode, charsToRemove: charsToRemove, suppressExcelErrors: suppressExcelErrors);
        }

        static IEnumerable<E> AsCollection<E>(ISheet worksheet, Func<object[], E> parseFunc, bool hasHeaderRow = false, BlankRowMode blankRowMode = default, bool suppressExcelErrors = false)
        {
            Column[] columns = null;
            var list = new List<E>();

            // get imported data and populate columns
            var objectArrays = AsObjects(worksheet, ref columns, hasHeaderRow, false, blankRowMode, suppressExcelErrors);

            objectArrays.Iterate
            (
                (array) =>
                {
                    var e = parseFunc.Invoke(array);
                    list.Add(e);
                }
            );

            return list;
        }

        static IEnumerable<E> AsCollection<E>(ISheet worksheet, Column[] columns, bool hasHeaderRow = false, bool validateHeaderRowFromColumns = false, BlankRowMode blankRowMode = default, char[] charsToRemove = null, bool suppressExcelErrors = false) where E : class, new()
        {
            // get imported data
            var objectArrays = AsObjects(worksheet, ref columns, hasHeaderRow, validateHeaderRowFromColumns, blankRowMode, suppressExcelErrors);
            var list = new List<E>();

            // characters to remove (whitespace is handled separately) to aid in the conversion of column names to object properties
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
                        var propertyName = Zap(columns[i].Name, textCleanMode: TextCleanMode.RemoveWhitespace, charsToRemove: charsToRemove);
                        e.SetInstanceProperty(propertyName, array[i], ignoreCase: true);
                    }
                    list.Add(e);
                }
            );

            return list;
        }

        static ISheet GetWorksheet(FileInfo file, int sheetNum)
        {
            if (!file.Exists) throw new UtilityException("File does not exist: " + file.FullName);
            switch (file.Extension.ToUpper())
            {
                case ".XLS":
                    using (var stream = file.OpenRead())
                    {
                        return new HSSFWorkbook(stream).GetSheetAt(sheetNum);
                    }
                case ".XLSX":
                    using (var stream = file.OpenRead())
                    {
                        return new XSSFWorkbook(stream).GetSheetAt(sheetNum);
                    }
            }
            throw new UtilityException("At this time the only Excel extensions allowed are .xls and .xlsx: " + file.FullName);
        }

        static IEnumerable<string[]> AsStrings(ISheet worksheet, ref Column[] columns, bool hasHeaderRow, bool validateHeaderRowFromColumns, BlankRowMode blankRowMode, AutoTruncate autoTrunc, bool suppressExcelErrors)
        {
            var objectArrays = AsObjectsOrStrings(worksheet, ref columns, true, hasHeaderRow, validateHeaderRowFromColumns, blankRowMode, autoTrunc, suppressExcelErrors);
            return objectArrays
                .Select(o => (string[])o)
                .ToList();
        }

        static IEnumerable<object[]> AsObjects(ISheet worksheet, ref Column[] columns, bool hasHeaderRow, bool validateHeaderRowFromColumns, BlankRowMode blankRowMode, bool suppressExcelErrors)
        {
            return AsObjectsOrStrings(worksheet, ref columns, false, hasHeaderRow, validateHeaderRowFromColumns, blankRowMode, AutoTruncate.Zap, suppressExcelErrors);
        }

        static IEnumerable<object[]> AsObjectsOrStrings(ISheet worksheet, ref Column[] columns, bool stringModeOn, bool hasHeaderRow, bool validateHeaderRowFromColumns, BlankRowMode blankRowMode, AutoTruncate autoTrunc, bool suppressExcelErrors)
        {
            var list = new List<object[]>();
            var rowNum = 0;
            var row = worksheet.GetRow(rowNum);
            var firstRowAsStrings = ParseStrings(row, new int[row.LastCellNum].Select(i => Column.String("")).ToArray(), AutoTruncate.Trim, true);
            object[] array;

            if (firstRowAsStrings == null)
            {
                return list;  // return empty list 
            }

            if (columns == null)
            {
                columns = new Column[firstRowAsStrings.Length];
                if (hasHeaderRow)
                {
                    for (int i = 0; i < firstRowAsStrings.Length; i++)  // use first row to build columns 
                    {
                        columns[i] = stringModeOn
                            ? (autoTrunc == AutoTruncate.Zap ? Column.NString(firstRowAsStrings[i]) : Column.String(firstRowAsStrings[i]))
                            : Column.Object(firstRowAsStrings[i]);
                    }
                }
                else
                {
                    for (int i = 0; i < firstRowAsStrings.Length; i++)
                    {
                        columns[i] = stringModeOn
                            ? (autoTrunc == AutoTruncate.Zap ? Column.NString("Col " + ExcelUtil.GetDisplayColumn(i + 1)) : Column.String("Col " + ExcelUtil.GetDisplayColumn(i + 1)))
                            : Column.Object("Col " + ExcelUtil.GetDisplayColumn(i + 1));
                    }
                }
            }
            else if (hasHeaderRow && validateHeaderRowFromColumns)
            {
                if (firstRowAsStrings.Length != columns.Length)
                {
                    throw new ValidationException("The number of Excel columns does not match the number of supplied columns: " + firstRowAsStrings.Length + ", " + columns.Length);
                }

                for (int i = 0; i < columns.Length; i++)
                {
                    if (!string.Equals(firstRowAsStrings[i], columns[i].Name, StringComparison.OrdinalIgnoreCase))
                    {
                        throw new ValidationException("Excel column \"" + firstRowAsStrings[i] + "\" does not match supplied column\"" + columns[i].Name + "\" at " + ExcelUtil.GetDisplayCellAddress(i + 1, 1));
                    }
                }
            }

            if (hasHeaderRow)
            {
                rowNum++;
            }

            while (true)
            {
                row = worksheet.GetRow(rowNum++);

                if (row == null) break;

                array = stringModeOn
                    ? ParseStrings(row, columns, autoTrunc, suppressExcelErrors)
                    : ParseObjects(row, columns, autoTrunc, suppressExcelErrors);

                if (array == null)
                {
                    switch (blankRowMode)
                    {
                        case BlankRowMode.Allow:
                            // do nothing allowing [null] to be added to the list
                            break;
                        case BlankRowMode.Skip:
                            continue;
                        case BlankRowMode.StopProcessing:
                            return list;
                        case BlankRowMode.ThrowException:
                            throw new Exception("Row " + rowNum + " is blank");
                    }
                }

                list.Add(array);
            }

            switch (blankRowMode)
            {
                case BlankRowMode.Allow:
                    while (list[list.Count - 1] == null)   // trim trailing blank rows
                    {
                        list.RemoveAt(list.Count - 1);
                    }
                    break;
            }

            return list;
        }

        static DataTable AsDataTable(IEnumerable<object[]> objectArrays, Column[] columns)
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

        static string[] ParseStrings(IRow row, Column[] columns, AutoTruncate autoTrunc, bool suppressExcelErrors)
        {
            var objects = ParseObjects(row, columns, autoTrunc, suppressExcelErrors);

            if (objects == null)
            {
                return null;
            }
            //if (objects.Length > columns.Length)
            //{
            //    throw new ValidationException("The number of data elements exceeds the number of columns: " + objects.Length + " / " + columns.Length);
            //}

            var strings = new string[objects.Length];

            for (int i = 0; i < objects.Length; i++)
            {
                strings[i] = i < columns.Length && columns[i].Format != null
                    ? columns[i].Format.Invoke(objects[i])
                    : objects[i]?.ToString();
            }
            return strings;
        }

        static object[] ParseObjects(IRow row, Column[] columns, AutoTruncate autoTrunc, bool suppressExcelErrors)
        {
            if (row.LastCellNum == 0) return null;
            var count = Math.Max(row.LastCellNum, columns.Length);
            var list = new List<object>();
            object value = null;
            ICell cell;
            CellType cellType;
            string cellAddress;

            if (row.IsBlank())
            {
                return null;
            }

            for (int i = 0; i < count; i++)
            {
                cell = row.GetCell(i, MissingCellPolicy.RETURN_BLANK_AS_NULL);

                if (cell == null)
                {
                    list.Add(autoTrunc == AutoTruncate.Zap ? null : "");
                    continue;
                }

                var column = i < columns.Length ? columns[i] : null;
                cellType = cell.CellType == CellType.Formula ? cell.CachedFormulaResultType : cell.CellType;
                cellAddress = ExcelUtil.GetDisplayCellAddress(cell.ColumnIndex + 1, cell.RowIndex + 1);

                switch (cellType)
                {
                    case CellType.String:
                    case CellType.Blank:
                        value = cell.StringCellValue;
                        switch (autoTrunc)
                        {
                            case AutoTruncate.Trim:
                                value = cell.StringCellValue.Trim();
                                break;
                            case AutoTruncate.Zap:
                                value = Zap(cell.StringCellValue);
                                break;
                        }
                        break;
                    case CellType.Numeric:
                        value = cell.NumericCellValue;
                        break;
                    case CellType.Boolean:
                        value = cell.BooleanCellValue;
                        break;
                    case CellType.Error:
                        if (suppressExcelErrors)
                        {
                            list.Add("[Error]");
                            break;
                        }
                        throw new UtilityException("Cell error at " + cellAddress + " (code " + cell.ErrorCellValue + ")");
                    case CellType.Unknown:
                    default:
                        if (suppressExcelErrors)
                        {
                            list.Add("[Unknown]");
                            break;
                        }
                        throw new UtilityException("Unknown cell type at " + cellAddress);
                }
                value = ConvertDataElement(value, column, cellAddress);
                list.Add(value);
            }
            return list.ToArray();
        }
    }
}
