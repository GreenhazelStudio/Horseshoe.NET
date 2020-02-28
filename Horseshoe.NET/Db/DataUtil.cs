using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security;
using System.Text.RegularExpressions;

using Horseshoe.NET.Collections;
using Horseshoe.NET.Crypto;
using Horseshoe.NET.Events;
using Horseshoe.NET.Text;

namespace Horseshoe.NET.Db
{
    public static class DataUtil
    {
        /* * * * * * * * * * * * * * * * * * * * 
         *   EVENTS                            *
         * * * * * * * * * * * * * * * * * * * */

        public static EasyNotifier<string, string> UsingConnectionString;

        public static EasyNotifier<string, string, string> UsingCredentials;

        public static EasyNotifier<string> UsingSqlStatement;

        public static EasyNotifier<string> ColumnSearchedByValue;

        /* * * * * * * * * * * * * * * * * * * * 
         *   CONNECTION STRINGS                *
         * * * * * * * * * * * * * * * * * * * */

        public static string DecryptInlinePassword(string connStrWithEcryptedPassword, CryptoOptions options = null)
        {
            var cipherText = ParseConnectionStringValue(ConnectionStringPart.Password, connStrWithEcryptedPassword);
            var plainText = Decrypt.String(cipherText, options: options);
            var reconstitutedConnStr = connStrWithEcryptedPassword.Replace(cipherText, plainText);
            return reconstitutedConnStr;
        }

        public static string HideInlinePassword(string connectionString)
        {
            var plainText = ParseConnectionStringValue(ConnectionStringPart.Password, connectionString);
            if (plainText == null) return connectionString;
            var reconstitutedConnStr = connectionString.Replace(plainText, "******");
            return reconstitutedConnStr;
        }

        public static string ParseConnectionStringValue(string key, string connectionString)
        {
            var match = new Regex("(?<=" + key + "=)[^;]+", RegexOptions.IgnoreCase).Match(connectionString);
            return TextUtil.Zap(match.Value);
        }

        public static string ParseConnectionStringValue(ConnectionStringPart part, string connectionString)
        {
            switch (part)
            {
                case ConnectionStringPart.DataSource:
                    return ParseConnectionStringValue("Data Source", connectionString) ?? ParseConnectionStringValue("Server", connectionString) ?? ParseConnectionStringValue("DSN", connectionString);
                case ConnectionStringPart.InitialCatalog:
                    return ParseConnectionStringValue("Initial Catalog", connectionString) ?? ParseConnectionStringValue("Database", connectionString);
                case ConnectionStringPart.UserId:
                    return ParseConnectionStringValue("User ID", connectionString) ?? ParseConnectionStringValue("UID", connectionString);
                case ConnectionStringPart.Password:
                    return ParseConnectionStringValue("Password", connectionString) ?? ParseConnectionStringValue("PWD", connectionString);
            }
            throw new UtilityException("Unknown connection string part: " + part);  // this should never happen
        }

        public static string Sqlize(object o, DbProduct? product = null)
        {
            if (o == null || o is DBNull) return "NULL";
            if (o is bool boolValue) o = boolValue ? 1 : 0;
            if (o is byte || o is short || o is int || o is long || o is float || o is double || o is decimal) return o.ToString();
            if (o is DateTime dateTimeValue)
            {
                string dateTimeFormat;
                switch (product ?? DbProduct.Neutral)
                {
                    case DbProduct.SqlServer:
                        dateTimeFormat = "yyyy-MM-dd HH:mm:ss.fff";  // format inspired by Microsoft Sql Server Managament Studio
                        break;
                    case DbProduct.Oracle:
                        dateTimeFormat = "MM/dd/yyyy hh:mm:ss tt";   // format inspired by dbForge for Oracle
                        var oracleDateFormat = "mm/dd/yyyy hh:mi:ss am";
                        return "TO_DATE('" + dateTimeValue.ToString(dateTimeFormat) + "', '" + oracleDateFormat + "')";  // example: TO_DATE('02/02/2014 3:35:57 PM', 'mm/dd/yyyy hh:mi:ss am')
                    default:
                        dateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                        break;
                }
                return "'" + dateTimeValue.ToString(dateTimeFormat) + "'";
            }
            if (o is RawSql rawSql)
            {
                if (rawSql is SystemValue systemValue)
                {
                    return systemValue.Formatter.Invoke(product, systemValue.Expression);
                }
                return rawSql.Expression ?? "";
            }
            var returnValue = "'" + o.ToString().Replace("'", "''") + "'";
            if (product == DbProduct.SqlServer)
            {
                return "N" + returnValue;
            }
            return returnValue;
        }

        /* * * * * * * * * * * * * * * * * * * * 
         *         TABLES AND COLUMNS          *
         * * * * * * * * * * * * * * * * * * * */
        static readonly Regex SimpleColumnNamePattern = new Regex("^[A-Z0-9_]+$", RegexOptions.IgnoreCase);

        public static string RenderColumnName(string name, DbProduct? product = null)
        {
            if (SimpleColumnNamePattern.IsMatch(name))
            {
                return name;
            }
            switch (product)
            {
                case DbProduct.SqlServer:
                    return "[" + name + "]";
                default:
                    return "\"" + name + "\"";
            }
        }

        public static void TrimDataTable(DataTable dataTable)
        {
            var fieldTypes = dataTable.Columns
                .Cast<DataColumn>()
                .Select(dc => dc.DataType)
                .ToArray();
            foreach (DataRow dataRow in dataTable.Rows)
            {
                if (dataRow.ItemArray.Length != fieldTypes.Length)
                {
                    throw new UtilityException("Row items do not match field types: " + dataRow.ItemArray.Length + ", " + fieldTypes.Length);
                }
                for (int i = 0; i < dataRow.ItemArray.Length; i++)
                {
                    if (dataRow[i] is string stringValue)
                    {
                        dataRow[i] = stringValue.Trim();
                    }
                }
            }
        }

        /* * * * * * * * * * * * * * * * * * * * 
         *         RESULT SET HELPERS          *
         * * * * * * * * * * * * * * * * * * * */
        public static object[] GetObjects(IDataReader reader, DataColumn[] columns, AutoTruncate autoTrunc = default, bool suppressErrors = false)
        {
            var items = new object[reader.FieldCount];
            if (items.Length != columns.Length)
            {
                throw new UtilityException("Row items do not match field types: " + items.Length + ", " + columns.Length);
            }
            try
            {
                reader.GetValues(items);
            }
            catch (Exception)
            {
                if (!suppressErrors) throw;
            }
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] is DBNull)
                {
                    items[i] = null;
                }
                else if (items[i] is string stringValue)
                {
                    switch (autoTrunc)
                    {
                        case AutoTruncate.Trim:
                            items[i] = stringValue.Trim();
                            break;
                        case AutoTruncate.Zap:
                            items[i] = stringValue.Zap();
                            break;
                    }
                }
            }

            return items;
        }

        /* * * * * * * * * * * * * * * * * * * * 
         *           MISCELLANEOUS             *
         * * * * * * * * * * * * * * * * * * * */
        internal static DbType CalculateDbType(object value)
        {
            if (value is string) return DbType.String;
            if (value is byte) return DbType.Byte;
            if (value is short) return DbType.Int16;
            if (value is int) return DbType.Int32;
            if (value is long) return DbType.Int64;
            if (value is decimal) return DbType.Decimal;
            if (value is double) return DbType.Double;
            if (value is bool) return DbType.Boolean;
            if (value is DateTime) return DbType.DateTime;
            if (value is Guid) return DbType.Guid;
            return DbType.Object;
        }

        internal static object WrangleParameterValue(object value)
        {
            if (value == null) return DBNull.Value;
            if (value.GetType().IsEnum)
            {
                return value.ToString();
            }
            return value;
        }

        public static IDictionary<string, string> ParseAdditionalConnectionAttributes(string text)
        {
            if (string.IsNullOrEmpty(text)) return null;
            var dict = new Dictionary<string, string>();
            var list = text.Split('|').ZapAndPrune();
            foreach (var attr in list)
            {
                var attrParts = attr.Split('=').Trim().ToArray();
                if (attrParts.Length == 2)
                {
                    dict.Add(attrParts[0], attrParts[1]);
                }
                else
                {
                    throw new UtilityException("Invalid additional connection string attribute: " + attr);
                }
            }
            return dict;
        }
    }
}
