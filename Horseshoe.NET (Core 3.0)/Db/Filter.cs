using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using static Horseshoe.NET.Db.DataUtil;
using Horseshoe.NET.Objects;

namespace Horseshoe.NET.Db
{
    public class Filter
    {
        public string ColumnName { get; }
        public FilterMode FilterMode { get; }
        public object[] Values { get; }
        public DbProduct? Vendor { get; set; }

        internal Filter() { }

        public Filter(string columnName, FilterMode filterMode, DbProduct? product = null) :
            this(columnName, filterMode, null, product: product)
        { 
        }

        public Filter(string columnName, FilterMode filterMode, object[] values, DbProduct? product = null)
        {
            ColumnName = Zap.String(columnName);
            FilterMode = filterMode;
            Values = values;
            Vendor = product;
            Validate();
        }

        private void Validate()
        {

            if (ColumnName == null)
            {
                throw new UtilityException("missing required parameter: column name");
            }
            if (ColumnName.StartsWith("["))
            {
                if (!ColumnName.EndsWith("]"))
                {
                    throw new UtilityException("malformed column name: " + ColumnName);
                }
            }
            else if (ColumnName.EndsWith("]"))
            {
                throw new UtilityException("malformed column name: " + ColumnName);
            }
            if (!(FilterMode == FilterMode.IsNull || FilterMode == FilterMode.IsNotNull))
            {
                if (Values == null || Values.Length == 0)
                {
                    throw new UtilityException("required value(s) not supplied: " + (Values?.Length.ToString() ?? "null"));
                }
                switch (FilterMode)
                {
                    case FilterMode.Equals:
                    case FilterMode.NotEquals:
                    case FilterMode.Contains:
                    case FilterMode.NotContains:
                    case FilterMode.StartsWith:
                    case FilterMode.NotStartsWith:
                    case FilterMode.EndsWith:
                    case FilterMode.NotEndsWith:
                    case FilterMode.GreaterThan:
                    case FilterMode.GreaterThanOrEquals:
                    case FilterMode.LessThan:
                    case FilterMode.LessThanOrEquals:
                        if (Values.Length != 1)
                        {
                            throw new UtilityException("Comparison filters require exactly 1 value, found: " + Values.Length);
                        }
                        break;
                    case FilterMode.In:
                    case FilterMode.NotIn:
                        if (Values.Length == 0)
                        {
                            throw new UtilityException("'In' filters require at least 1 value, found: 0");
                        }
                        break;
                    case FilterMode.Between:
                    case FilterMode.NotBetween:
                    case FilterMode.BetweenExclusive:
                    case FilterMode.NotBetweenInclusive:
                    case FilterMode.BetweenExclusiveLowerBoundOnly:
                    case FilterMode.NotBetweenExclusiveLowerBoundOnly:
                    case FilterMode.BetweenExclusiveUpperBoundOnly:
                    case FilterMode.NotBetweenExclusiveUpperBoundOnly:
                        if (Values.Length != 2)
                        {
                            throw new UtilityException("'Between' filters require exactly 2 values, found: " + Values.Length);
                        }
                        break;
                }
            }
        }

        public override string ToString()
        {
            return Render();
        }

        public virtual string Render()
        {
            var colName = RenderColumnName(ColumnName, Vendor);
            var sb = new StringBuilder(colName);
            try
            {
                switch (FilterMode)
                {
                    case FilterMode.Equals:
                        sb.Append(" = ").Append(Sqlize(Values[0]));
                        break;
                    case FilterMode.NotEquals:
                        sb.Append(" <> ").Append(Sqlize(Values[0]));
                        break;
                    case FilterMode.Contains:
                        sb.Append(" LIKE ").Append(Sqlize("%" + Values[0] + "%"));
                        break;
                    case FilterMode.NotContains:
                        sb.Append(" NOT LIKE ").Append(Sqlize("%" + Values[0] + "%"));
                        break;
                    case FilterMode.StartsWith:
                        sb.Append(" LIKE ").Append(Sqlize(Values[0] + "%"));
                        break;
                    case FilterMode.NotStartsWith:
                        sb.Append(" NOT LIKE ").Append(Sqlize(Values[0] + "%"));
                        break;
                    case FilterMode.EndsWith:
                        sb.Append(" LIKE ").Append(Sqlize("%" + Values[0]));
                        break;
                    case FilterMode.NotEndsWith:
                        sb.Append(" NOT LIKE ").Append(Sqlize("%" + Values[0]));
                        break;
                    case FilterMode.GreaterThan:
                        sb.Append(" > ").Append(Sqlize(Values[0]));
                        break;
                    case FilterMode.GreaterThanOrEquals:
                        sb.Append(" >= ").Append(Sqlize(Values[0]));
                        break;
                    case FilterMode.LessThan:
                        sb.Append(" < ").Append(Sqlize(Values[0]));
                        break;
                    case FilterMode.LessThanOrEquals:
                        sb.Append(" <= ").Append(Sqlize(Values[0]));
                        break;
                    case FilterMode.In:
                        sb.Append(" IN(").Append(string.Join(", ", Values.Select(v => Sqlize(v)))).Append(")");
                        break;
                    case FilterMode.NotIn:
                        sb.Append(" NOT IN(").Append(string.Join(", ", Values.Select(v => Sqlize(v)))).Append(")");
                        break;
                    case FilterMode.Between:
                        sb.Append(" BETWEEN ").Append(Sqlize(Values[0])).Append(" AND ").Append(Sqlize(Values[1]));
                        break;
                    case FilterMode.NotBetween:
                        sb.Append(" NOT BETWEEN ").Append(Sqlize(Values[0])).Append(" AND ").Append(Sqlize(Values[1]));
                        break;
                    case FilterMode.BetweenExclusive:
                        sb.Insert(0, "(")
                            .Append(" > ").Append(Sqlize(Values[0]))
                            .Append(" AND ")
                            .Append(colName)
                            .Append(" < ").Append(Sqlize(Values[1]))
                            .Append(")");
                        break;
                    case FilterMode.NotBetweenInclusive:
                        sb.Insert(0, "(")
                            .Append(" <= ").Append(Sqlize(Values[0]))
                            .Append(" AND ")
                            .Append(colName)
                            .Append(" >= ").Append(Sqlize(Values[1]))
                            .Append(")");
                        break;
                    case FilterMode.BetweenExclusiveLowerBoundOnly:
                        sb.Insert(0, "(")
                            .Append(" > ").Append(Sqlize(Values[0]))
                            .Append(" AND ")
                            .Append(colName)
                            .Append(" <= ").Append(Sqlize(Values[1]))
                            .Append(")");
                        break;
                    case FilterMode.NotBetweenExclusiveLowerBoundOnly:
                        sb.Insert(0, "(")
                            .Append(" < ").Append(Sqlize(Values[0]))
                            .Append(" AND ")
                            .Append(colName)
                            .Append(" >= ").Append(Sqlize(Values[1]))
                            .Append(")");
                        break;
                    case FilterMode.BetweenExclusiveUpperBoundOnly:
                        sb.Insert(0, "(")
                            .Append(" >= ").Append(Sqlize(Values[0]))
                            .Append(" AND ")
                            .Append(colName)
                            .Append(" < ").Append(Sqlize(Values[1]))
                            .Append(")");
                        break;
                    case FilterMode.NotBetweenExclusiveUpperBoundOnly:
                        sb.Insert(0, "(")
                            .Append(" <= ").Append(Sqlize(Values[0]))
                            .Append(" AND ")
                            .Append(colName)
                            .Append(" > ").Append(Sqlize(Values[1]))
                            .Append(")");
                        break;
                    case FilterMode.IsNull:
                        sb.Append(" IS NULL");
                        break;
                    case FilterMode.IsNotNull:
                        sb.Append(" IS NOT NULL");
                        break;
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new UtilityException("Invalid value(s): " + string.Join(", ", Values.Select(v => v?.ToString() ?? "[null]")), ex);
            }
        }

        public static Filter Equals(string columnName, object value, DbProduct? product = null)
        {
            return value != null
                ? new Filter(columnName, FilterMode.Equals, new[] { value }, product: product)
                : new Filter(columnName, FilterMode.IsNull, product: product);
        }

        public static Filter NotEquals(string columnName, object value, bool columnIsNullable = false, DbProduct? product = null)
        {
            if (value != null)
            {
                return columnIsNullable
                    ? Or
                      (
                          IsNull(columnName),
                          new Filter(columnName, FilterMode.NotEquals, new[] { value }, product: product)
                      )
                    : new Filter(columnName, FilterMode.NotEquals, new[] { value }, product: product);

            }
            return new Filter(columnName, FilterMode.IsNotNull, product: product);
        }

        public static Filter NotEqualsOrNull(string columnName, object value, DbProduct? product = null)
        {
            return value != null
                ? new Filter(columnName, FilterMode.NotEquals, new[] { value }, product: product)
                : new Filter(columnName, FilterMode.IsNotNull, product: product);
        }

        public static Filter Contains(string columnName, string value, DbProduct? product = null)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new UtilityException("May produce unexpected results: no text for 'like' comparison");
            return new Filter(columnName, FilterMode.Contains, new object[] { value }, product: product);
        }

        public static Filter StartsWith(string columnName, string value, DbProduct? product = null)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new UtilityException("May produce unexpected results: no text for 'like' comparison");
            return new Filter(columnName, FilterMode.StartsWith, new object[] { value }, product: product);
        }

        public static Filter EndsWith(string columnName, string value, DbProduct? product = null)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new UtilityException("May produce unexpected results: no text for 'like' comparison");
            return new Filter(columnName, FilterMode.EndsWith, new object[] { value }, product: product);
        }

        public static Filter In(string columnName, object[] values, DbProduct? product = null)
        {
            if (values == null || !values.Any()) throw new UtilityException("'in' functions expect at least 1 value, found: " + (values?.Count().ToString() ?? "null"));
            return new Filter(columnName, FilterMode.In, values, product: product);
        }

        public static Filter In<T>(string columnName, T[] values, DbProduct? product = null)
        {
            if (values == null || !values.Any()) throw new UtilityException("'in' functions expect at least 1 value, found: " + (values?.Count().ToString() ?? "null"));
            return new Filter(columnName, FilterMode.In, values.Select(t => (object)t).ToArray(), product: product);
        }

        public static Filter NotIn(string columnName, object[] values, DbProduct? product = null)
        {
            if (values == null || !values.Any()) throw new UtilityException("'in' functions expect at least 1 value, found: " + (values?.Count().ToString() ?? "null"));
            return new Filter(columnName, FilterMode.NotIn, values, product: product);
        }

        public static Filter NotIn<T>(string columnName, T[] values, DbProduct? product = null)
        {
            if (values == null || !values.Any()) throw new UtilityException("'in' functions expect at least 1 value, found: " + (values?.Count().ToString() ?? "null"));
            return new Filter(columnName, FilterMode.NotIn, values.Select(t => (object)t).ToArray(), product: product);
        }

        public static Filter Between(string columnName, object[] values, DbProduct? product = null)
        {
            if (values == null || values.Count() != 2) throw new UtilityException("'between' functions expect exactly 2 values, found: " + (values?.Count().ToString() ?? "null"));
            return new Filter(columnName, FilterMode.Between, values, product: product);
        }

        public static Filter NotBetween(string columnName, object[] values, DbProduct? product = null)
        {
            if (values == null || values.Count() != 2) throw new UtilityException("'between' functions expect exactly 2 values, found: " + (values?.Count().ToString() ?? "null"));
            return new Filter(columnName, FilterMode.NotBetween, values, product: product);
        }

        public static Filter BetweenExclusive(string columnName, object[] values, DbProduct? product = null)
        {
            if (values == null || values.Count() != 2) throw new UtilityException("'between' functions expect exactly 2 values, found: " + (values?.Count().ToString() ?? "null"));
            return new Filter(columnName, FilterMode.BetweenExclusive, values, product: product);
        }

        public static Filter NotBetweenInclusive(string columnName, object[] values, DbProduct? product = null)
        {
            if (values == null || values.Count() != 2) throw new UtilityException("'between' functions expect exactly 2 values, found: " + (values?.Count().ToString() ?? "null"));
            return new Filter(columnName, FilterMode.NotBetweenInclusive, values, product: product);
        }

        public static Filter LessThan(string columnName, object value, DbProduct? product = null)
        {
            return new Filter(columnName, FilterMode.LessThan, new[] { value }, product: product);
        }

        public static Filter LessThanOrEquals(string columnName, object value, DbProduct? product = null)
        {
            return new Filter(columnName, FilterMode.LessThanOrEquals, new[] { value }, product: product);
        }

        public static Filter GreaterThan(string columnName, object value, DbProduct? product = null)
        {
            return new Filter(columnName, FilterMode.GreaterThan, new[] { value }, product: product);
        }

        public static Filter GreaterThanOrEquals(string columnName, object value, DbProduct? product = null)
        {
            return new Filter(columnName, FilterMode.GreaterThanOrEquals, new[] { value }, product: product);
        }

        public static Filter IsNull(string columnName, DbProduct? product = null)
        {
            return new Filter(columnName, FilterMode.IsNull, product: product);
        }

        public static Filter IsNotNull(string columnName, DbProduct? product = null)
        {
            return new Filter(columnName, FilterMode.IsNotNull, product: product);
        }

        public static Filter And(params Filter[] filters)
        {
            return new AndGrouping(filters);
        }

        public static Filter Or(params Filter[] filters)
        {
            return new OrGrouping(filters);
        }

        public class AndGrouping : Filter
        {
            public List<Filter> Filters { get; }

            public AndGrouping(Filter[] filters)
            {
                if (filters == null) throw new UtilityException("filters cannot be null");
                if (filters.Length == 0) throw new UtilityException("filters cannot be empty");
                Filters = new List<Filter>(filters);
            }

            public override string Render()
            {
                return "(" + string.Join(" AND ", Filters) + ")";
            }
        }

        public class OrGrouping : Filter
        {
            public List<Filter> Filters { get; }

            public OrGrouping(Filter[] filters)
            {
                if (filters == null) throw new UtilityException("filters cannot be null");
                if (filters.Length == 0) throw new UtilityException("filters cannot be empty");
                Filters = new List<Filter>(filters);
            }

            public override string Render()
            {
                return "(" + string.Join(" OR ", Filters) + ")";
            }
        }
    }
}
