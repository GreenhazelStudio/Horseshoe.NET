using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Horseshoe.NET.Text.Extensions;

namespace Horseshoe.NET.Text
{
    public class LRUniformer : List<LRUniformer.Item>
    {
        public int MaxLWidth => this.Max(i => i.L.Length);

        public void Add(string l, string r, bool lIsIndex = false, bool lAlignRight = false, bool rAlignRight = false)
        {
            Add(new Item { L = l, R = r, LIsIndex = lIsIndex, LAlignRight = lAlignRight, RAlignRight = rAlignRight });
        }

        public void Add(int l, string r, bool lIsIndex = false, bool lAlignRight = false, bool rAlignRight = false)
        {
            Add(l.ToString(), r, lIsIndex: lIsIndex, lAlignRight: lAlignRight, rAlignRight: rAlignRight);
        }

        public void Insert(int index, string l, string r, bool lIsIndex = false, bool lAlignRight = false, bool rAlignRight = false)
        {
            Insert(index, new Item { L = l, R = r, LIsIndex = lIsIndex, LAlignRight = lAlignRight, RAlignRight = rAlignRight });
        }

        public void Insert(int index, int l, string r, bool lIsIndex = false, bool lAlignRight = false, bool rAlignRight = false)
        {
            Insert(index, l.ToString(), r, lIsIndex: lIsIndex, lAlignRight: lAlignRight, rAlignRight: rAlignRight);
        }

        public void AddUniqueItem(string l, string r, bool lIsIndex = false, bool lAlignRight = false, bool rAlignRight = false, bool ignoreNullL = false)
        {
            if ((!ignoreNullL || l != null) && this.Any(i => i.L.Equals(l, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ValidationException("Duplicate item found: " + l + " (" + r + ")");
            }
            Add(l, r, lIsIndex: lIsIndex, lAlignRight: lAlignRight, rAlignRight: rAlignRight);
        }

        public void AddUniqueItem(int l, string r, bool lIsIndex = false, bool lAlignRight = false, bool rAlignRight = false, bool ignoreNullL = false)
        {
            if (this.Any(i => i.L.Equals(l.ToString())))
            {
                throw new ValidationException("Duplicate item found: " + l + " (" + r + ")");
            }
            Add(l, r, lIsIndex: lIsIndex, lAlignRight: lAlignRight, rAlignRight: rAlignRight);
        }

        public void InsertUniqueItem(int index, string l, string r, bool lIsIndex = false, bool lAlignRight = false, bool rAlignRight = false, bool ignoreNullL = false)
        {
            if ((!ignoreNullL || l != null) && this.Any(i => i.L.Equals(l, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ValidationException("Duplicate item found: " + l + " (" + r + ")");
            }
            Insert(index, l, r, lIsIndex: lIsIndex, lAlignRight: lAlignRight, rAlignRight: rAlignRight);
        }

        public void InsertUniqueItem(int index, int l, string r, bool lIsIndex = false, bool lAlignRight = false, bool rAlignRight = false, bool ignoreNullL = false)
        {
            if (this.Any(i => i.L.Equals(l.ToString())))
            {
                throw new ValidationException("Duplicate item found: " + l + " (" + r + ")");
            }
            Insert(index, l, r, lIsIndex: lIsIndex, lAlignRight: lAlignRight, rAlignRight: rAlignRight);
        }

        public string RenderLSquareBrackets(int width = 0, string padding = null, string truncateMarker = null)
        {
            var strb = new StringBuilder();
            var lpad = MaxLWidth + 2;
            var rfit = width > lpad + 3 ? width - lpad - 3 : 0;
            foreach (var item in this)
            {
                strb.AppendIf
                    (
                        item.LIsIndex || item.LAlignRight,
                        ("[" + item.L + "]").PadLeft(lpad),
                        ("[" + item.L + "]").PadRight(lpad)
                    )
                    .Append(" ")
                    .AppendLine
                    (
                        rfit > 0
                            ? (item.RAlignRight
                                ? (item.R ?? "").FitRight(rfit, padding: padding, truncateMarker: truncateMarker)
                                : (item.R ?? "").FitLeft(rfit, padding: padding, truncateMarker: truncateMarker)
                              )
                            : item.R
                    );
            }
            return strb.ToString();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1034:Nested types should not be visible", Justification = "Needs to be accessible for potential custom renderers")]
        public class Item
        {
            public string L { get; set; } = "";
            public string R { get; set; }
            public bool LIsIndex { get; set; }
            public bool LAlignRight { get; set; }
            public bool RAlignRight { get; set; }
        }
    }
}
