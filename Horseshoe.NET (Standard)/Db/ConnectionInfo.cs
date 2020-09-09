using System;
using System.Collections.Generic;

namespace Horseshoe.NET.Db
{
    public abstract class ConnectionInfo
    {
        private string _connectionString;

        public virtual string ConnectionString
        {
            get
            {
                if (_connectionString != null)
                {
                    return IsEncryptedPassword
                        ? DataUtil.DecryptInlinePassword(_connectionString)
                        : _connectionString;
                }
                return null;
            }
            set
            {
                _connectionString = value;
            }
        }

        public bool IsEncryptedPassword { get; set; }

        public string DataSource { get; set; }

        public Credential? Credentials { get; set; }

        public IDictionary<string, string> AdditionalConnectionAttributes { get; set; }

        public void AddConnectionAttribute(string key, string value)
        {
            AddConnectionAttributes(new Dictionary<string, string> { { key, value } });
        }

        public void AddConnectionAttributes(IDictionary<string, string> attrs)
        {
            if (AdditionalConnectionAttributes == null)
            {
                AdditionalConnectionAttributes = attrs;
            }
            else
            {
                foreach (var key in attrs.Keys)
                {
                    if (AdditionalConnectionAttributes.ContainsKey(key))
                    {
                        AdditionalConnectionAttributes[key] = attrs[key];
                    }
                    else
                    {
                        AdditionalConnectionAttributes.Add(key, attrs[key]);
                    }
                }
            }
        }
    }
}
