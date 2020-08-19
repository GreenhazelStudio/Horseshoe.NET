using System;
using System.Data;

using Horseshoe.NET.Objects.Clean;

namespace Horseshoe.NET.Db
{
    public static class ScalarReaderParser
    {
        public static Func<IDataReader, string> String { get; } = (IDataReader reader) => Zap.String(reader[0]);
        public static Func<IDataReader, int> Int { get; } = (IDataReader reader) => Zap.Int(reader[0]);
        public static Func<IDataReader, int?> NInt { get; } = (IDataReader reader) => Zap.NInt(reader[0]);
        public static Func<IDataReader, decimal> Decimal { get; } = (IDataReader reader) => Zap.Decimal(reader[0]);
        public static Func<IDataReader, decimal?> NDecimal { get; } = (IDataReader reader) => Zap.NDecimal(reader[0]);
        public static Func<IDataReader, DateTime> DateTime { get; } = (IDataReader reader) => Zap.DateTime(reader[0]);
        public static Func<IDataReader, DateTime?> NDateTime { get; } = (IDataReader reader) => Zap.NDateTime(reader[0]);
    }
}
