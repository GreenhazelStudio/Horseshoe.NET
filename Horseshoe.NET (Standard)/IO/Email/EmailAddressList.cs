using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Horseshoe.NET.Collections;

namespace Horseshoe.NET.IO.Email
{
    public class EmailAddressList : List<string>
    {
        public EmailAddressList() { }
        public EmailAddressList(IEnumerable<string> addresses) : base(addresses) { }

        public static implicit operator EmailAddressList(string address) => new EmailAddressList(address?.Split(',', ';').ZapAndPrune());
        public static implicit operator EmailAddressList(string[] addresses) => new EmailAddressList(addresses?.ZapAndPrune());
    }
}
