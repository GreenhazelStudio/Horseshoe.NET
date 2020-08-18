using System;
using System.Data;

using static Horseshoe.NET.ObjectClean.Methods;

namespace Horseshoe.NET.Db
{
    public static class ScalarReaderParser
    {
        public static Func<IDataReader, string> String { get; } = (IDataReader reader) => ZapString(reader[0]);
        public static Func<IDataReader, int> Int { get; } = (IDataReader reader) => ZapInt(reader[0]);
        public static Func<IDataReader, int?> NInt { get; } = (IDataReader reader) => ZapNInt(reader[0]);
        public static Func<IDataReader, decimal> Decimal { get; } = (IDataReader reader) => ZapDecimal(reader[0]);
        public static Func<IDataReader, decimal?> NDecimal { get; } = (IDataReader reader) => ZapNDecimal(reader[0]);
        public static Func<IDataReader, DateTime> DateTime { get; } = (IDataReader reader) => ZapDateTime(reader[0]);
        public static Func<IDataReader, DateTime?> NDateTime { get; } = (IDataReader reader) => ZapNDateTime(reader[0]);
    }
}
