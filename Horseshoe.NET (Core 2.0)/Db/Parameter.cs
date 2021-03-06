﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Horseshoe.NET.Db
{
    public class Parameter : DbParameter
    {
        private object _value;

        private DbType? _dbType;

        public override string ParameterName { get; set; }

        public override object Value
        {
            get { return _value ?? DBNull.Value; }
            set { _value = DataUtil.WrangleParameterValue(value); }
        }

        public override DbType DbType
        {
            get { return _dbType ?? DataUtil.CalculateDbType(Value); }
            set { _dbType = value; }
        }

        public bool IsDbTypeSet => _dbType.HasValue;

        public override ParameterDirection Direction { get; set; } = ParameterDirection.Input;

        public override bool IsNullable { get; set; }

        public override int Size { get; set; }

        public override string SourceColumn { get; set; }

        public override bool SourceColumnNullMapping { get; set; }

        public override DataRowVersion SourceVersion { get; set; } = DataRowVersion.Default;

        public Parameter()
        {
        }

        public Parameter(string parameterName, object value)
        {
            ParameterName = parameterName;
            Value = value;
        }

        public override void ResetDbType()
        {
            DbType = default;
        }
    }
}
