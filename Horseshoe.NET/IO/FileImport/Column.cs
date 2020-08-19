using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using  Horseshoe.NET.Objects.Clean;

namespace Horseshoe.NET.IO.FileImport
{
    public class Column
    {
        public string Name { get; set; }
        public DataType DataType { get; set; } = DataType.Object;
        public int Width { get; set; }
        public Func<object, object> ValueConverter { get; set; }
        public Func<object, string> Format { get; set; }
        public bool IsNullable => this is NullableColumn;
        public bool IsMappable => !(this is NoMapColumn);

        public override string ToString()
        {
            return ToDataTypeString() + 
                "(" +
                "\"" + Name + "\"" +
                (Width != 0 ? ", " + Width.ToString() : "") +
                ")";
        }

        public string ToDataTypeString()
        {
            if (!IsMappable) return "NoMap";
            return (IsNullable ? "N" : "") + DataType;
        }

        public DataColumn ToDataColumn()
        {
            switch (DataType)
            {
                case DataType.Object:
                case DataType.String:
                default:
                    return new DataColumn(Name);
                case DataType.Byte:
                    return new DataColumn(Name, typeof(byte));
                case DataType.Integer:
                    return new DataColumn(Name, typeof(int));
                case DataType.Decimal:
                    return new DataColumn(Name, typeof(decimal));
                case DataType.DateTime:
                case DataType.Time:
                    return new DataColumn(Name, typeof(DateTime));
                case DataType.Boolean:
                    return new DataColumn(Name, typeof(bool));
            }
        }

        public static Column Object(string name, int width = 0) => new Column { Name = name, Width = width, DataType = DataType.Object };

        public static Column String(string name, int width = 0) => new Column { Name = name, Width = width, DataType = DataType.String, ValueConverter = (obj) => Zap.String(obj) ?? "" };

        public static Column NString(string name, int width = 0) => new NullableColumn { Name = name, Width = width, DataType = DataType.String, ValueConverter = (obj) => Zap.String(obj) };

        public static Column Byte(string name, int width = 0) => new Column { Name = name, Width = width, DataType = DataType.Byte, ValueConverter = (obj) => Zap.Byte(obj) };

        public static Column NByte(string name, int width = 0) => new NullableColumn { Name = name, Width = width, DataType = DataType.Byte, ValueConverter = (obj) => Zap.NByte(obj) };

        public static Column Int(string name, int width = 0) => new Column { Name = name, Width = width, DataType = DataType.Integer, ValueConverter = (obj) => Zap.Int(obj) };

        public static Column NInt(string name, int width = 0) => new NullableColumn { Name = name, Width = width, DataType = DataType.Integer, ValueConverter = (obj) => Zap.NInt(obj) };

        public static Column Decimal(string name, int width = 0) => new Column { Name = name, Width = width, DataType = DataType.Decimal, ValueConverter = (obj) => Zap.Decimal(obj) };

        public static Column NDecimal(string name, int width = 0) => new NullableColumn { Name = name, Width = width, DataType = DataType.Decimal, ValueConverter = (obj) => Zap.NDecimal(obj) };

        public static Column Boolean(string name, int width = 0) => new Column { Name = name, Width = width, DataType = DataType.Boolean, ValueConverter = (obj) => Zap.Bool(obj) };

        public static Column NBoolean(string name, int width = 0) => new NullableColumn { Name = name, Width = width, DataType = DataType.Boolean, ValueConverter = (obj) => Zap.NBool(obj) };

        public static Column Date(string name, int width = 0) => new Column
        {
            Name = name,
            Width = width,
            DataType = DataType.DateTime,
            ValueConverter = (obj) => Zap.DateTime(obj),
            Format = DateFormat
        };

        public static Column NDate(string name, int width = 0) => new NullableColumn
        {
            Name = name,
            Width = width,
            DataType = DataType.DateTime,
            ValueConverter = (obj) => Zap.NDateTime(obj),
            Format = DateFormat
        };

        public static Column Time(string name, int width = 0) => new Column
        {
            Name = name,
            Width = width,
            DataType = DataType.DateTime,
            ValueConverter = (obj) => TimeConverter(obj) ?? DateTime.MinValue,
            Format = TimeFormat
        };

        public static Column NTime(string name, int width = 0) => new NullableColumn
        {
            Name = name,
            Width = width,
            DataType = DataType.DateTime,
            ValueConverter = TimeConverter,
            Format = TimeFormat
        };

        public static Column NoMap(string name, int width = 0) => new NoMapColumn { Name = name, Width = width };

        static string DateFormat(object obj)
        {
            if (obj is DateTime dateTimeValue)
            {
                if (dateTimeValue.Hour > 0 || dateTimeValue.Minute > 0)
                {
                    if (dateTimeValue.Millisecond > 0)
                    {
                        return dateTimeValue.ToString("M/d/yyyy hh:mm:ss.fff tt");
                    }
                    else if (dateTimeValue.Second > 0)
                    {
                        return dateTimeValue.ToString("M/d/yyyy hh:mm:ss tt");
                    }
                    else
                    {
                        return dateTimeValue.ToString("M/d/yyyy hh:mm tt");
                    }
                }
                return dateTimeValue.ToString("M/d/yyyy");
            }
            return obj?.ToString();
        }

        public static string DateFormatNoMilliseconds(object obj)
        {
            if (obj is DateTime dateTimeValue)
            {
                if (dateTimeValue.Hour > 0 || dateTimeValue.Minute > 0)
                {
                    if (dateTimeValue.Second > 0)
                    {
                        return dateTimeValue.ToString("M/d/yyyy hh:mm:ss tt");
                    }
                    else
                    {
                        return dateTimeValue.ToString("M/d/yyyy hh:mm tt");
                    }
                }
                return dateTimeValue.ToString("M/d/yyyy");
            }
            return obj?.ToString();
        }

        static string TimeFormat(object obj)
        {
            if (obj is DateTime dateTimeValue)
            {
                if (dateTimeValue.Millisecond > 0)
                {
                    return dateTimeValue.ToString("hh:mm:ss.fff tt");
                }
                if (dateTimeValue.Second > 0)
                {
                    return dateTimeValue.ToString("hh:mm:ss tt");
                }
                return dateTimeValue.ToString("hh:mm tt");
            }
            return obj?.ToString();
        }

        static object TimeConverter(object obj)
        {
            if (obj == null) return null;
            var timeValue = DateTime.MinValue;
            if (obj is DateTime dateTimeValue)
            {
                timeValue = timeValue.AddHours(dateTimeValue.Hour);
                timeValue = timeValue.AddMinutes(dateTimeValue.Minute);
                timeValue = timeValue.AddSeconds(dateTimeValue.Second);
                timeValue = timeValue.AddMilliseconds(dateTimeValue.Millisecond);
                return timeValue;
            }
            if (obj is string stringValue)
            {
                //var origStringValue = stringValue;
                stringValue = stringValue.Trim();
                var isPM = false;
                if (stringValue.ToUpper().EndsWith("AM"))
                {
                    stringValue = stringValue.Substring(0, stringValue.Length - 2).Trim();
                }
                else if (stringValue.ToUpper().EndsWith("PM"))
                {
                    isPM = true;
                    stringValue = stringValue.Substring(0, stringValue.Length - 2).Trim();
                }
                var timeParts = stringValue.Replace(".", ":").Split(':');
                switch (timeParts.Length)
                {
                    default:
                        return obj;
                    case 2:
                    case 3:
                    case 4:
                        timeValue = timeValue.AddHours(int.Parse(timeParts[0]) + (isPM ? 12 : 0));
                        timeValue = timeValue.AddMinutes(int.Parse(timeParts[1]));
                        if (timeParts.Length > 2)
                        {
                            timeValue = timeValue.AddSeconds(int.Parse(timeParts[2]));
                        }
                        if (timeParts.Length > 3)
                        {
                            timeValue = timeValue.AddMilliseconds(int.Parse(timeParts[3]));
                        }
                        break;
                }
                return timeValue;
            }
            return obj;
        }
    }
}
