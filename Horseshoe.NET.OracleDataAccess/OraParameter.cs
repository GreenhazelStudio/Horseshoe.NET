using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Horseshoe.NET.DataAccess;

using Oracle.ManagedDataAccess.Client;

namespace Horseshoe.NET.OracleDataAccess
{
    public class OraParameter : Parameter
    {
        private OracleDbType? _oraDbType; 

        public OracleDbType OracleDbType
        {
            get { return _oraDbType ?? OracleDbType.Char; }
            set { _oraDbType = value; }
        }

        public bool IsOracleDbTypeSet => _oraDbType.HasValue;

        public OraParameter() : base()
        {
        }

        public OraParameter(string parameterName, object value) : base(parameterName, value)
        {
        }

        public static OraParameter BuildRefCursor(string parameterName)
        {
            return new OraParameter
            {
                ParameterName = parameterName,
                OracleDbType = OracleDbType.RefCursor,
                Direction = ParameterDirection.Output
            };
        }

        public static OraParameter RefCursor { get; } = BuildRefCursor(null);
    }
}
