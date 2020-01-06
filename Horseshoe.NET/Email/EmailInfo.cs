using System.Collections.Generic;

using Horseshoe.NET.Collections;

namespace Horseshoe.NET.Email
{
    public class EmailInfo
    {
        public string From { get; set; }

        public string To
        {
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    var tos = value.Replace(",", ";");
                    Tos = tos.Split(';').Trim();
                }
                else
                {
                    Tos = null;
                }
            }
        }

        public IEnumerable<string> Tos { get; set; }

        public string CC
        {
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    var ccs = value.Replace(",", ";");
                    CCs = ccs.Split(';').Trim();
                }
                else
                {
                    CCs = null;
                }
            }
        }

        public IEnumerable<string> CCs { get; set; }

        public string BCC
        {
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    var bccs = value.Replace(",", ";");
                    BCCs = bccs.Split(';').Trim();
                }
                else
                {
                    BCCs = null;
                }
            }
        }

        public IEnumerable<string> BCCs { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public bool IsHtml { get; } = false;

        /// <summary>
        /// Optional disclaimers, contact information, etc. to append at the bottom of the email body
        /// </summary>
        public string FooterText { get; set; }

        public string Attachment
        {
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    var attachments = value.Replace(":", "|");
                    Attachments = attachments.Split('|').Trim();
                }
                else
                {
                    Attachments = null;
                }
            }
        }

        public IEnumerable<string> Attachments { get; set; }
    }
}
