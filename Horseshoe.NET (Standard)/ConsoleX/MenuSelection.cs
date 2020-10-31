using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using Horseshoe.NET.Text;

namespace Horseshoe.NET.ConsoleX
{
    public class MenuSelection<T>
    {
        /// <summary>
        /// The 1-based index of the selected menu item, otherwise 0 if arbitary input was allowed and has been input by the user. 
        /// </summary>
        public int SelectedIndex { get; set; }

        /// <summary>
        /// The 1-based indexes of the selected menu items. 
        /// </summary>
        public IEnumerable<int> SelectedIndexes { get; set; }

        /// <summary>
        /// The selected menu item (the actual object), otherwise null if arbitary input was allowed and has been input by the user.
        /// </summary>
        public T SelectedItem { get; set; }

        /// <summary>
        /// The selected menu items (the actual objects).
        /// </summary>
        public IEnumerable<T> SelectedItems { get; set; }

        /// <summary>
        /// True if 'All' was entered at the prompt for a multi-select menu, false otherwise.
        /// </summary>
        public bool SelectedAll { get; set; }

        /// <summary>
        /// Arbitrary input if allowed and has been input by the user.
        /// </summary>
        public string ArbitraryInput { get; set; }

        /// <summary>
        /// The custom command selected by the user.
        /// </summary>
        public Routine CustomMenuItem { get; set; }
    }

    public static class MenuSelection
    { 
        static readonly Regex NumericRangePattern = new Regex("^[0-9]+(-[0-9]+)?,?", RegexOptions.IgnoreCase);

        public static IEnumerable<int> ParseMultipleIndexes(string input, int menuItemsCount, out bool all)
        {
            input = input.Trim();
            all = false;

            var indexes = new List<int>(); // 1-based indexes
            var except = false;

            if (input.ToUpper().StartsWith("ALL"))
            {
                for (int i = 1; i <= menuItemsCount; i++) indexes.Add(i);
                all = true;
                input = input.Substring(3).Trim();

                if (input.ToUpper().StartsWith("X"))
                {
                    except = true;
                    input = input.Substring(1).Trim();
                }
                else if (input.ToUpper().StartsWith("EXCEPT"))
                {
                    except = true;
                    input = input.Substring(6).Trim();
                }
                else if (input.Length > 0)
                {
                    throw new BenignException("invalid input: " + input);
                }
            }

            input = TextClean.CleanString(input, rules: new TextCleanRules(TextCleanMode.RemoveWhitespace));

            var match = NumericRangePattern.Match(input);

            while (match.Success)
            {
                var token = match.Value.Replace(",", "");
                var ints = token
                    .Split('-')
                    .Select(s => int.Parse(s))
                    .ToArray();

                if (ints.Length == 1)
                {
                    if (ints[0] <= menuItemsCount)
                    {
                        if (except)
                        {
                            indexes.Remove(ints[0]);
                        }
                        else
                        {
                            indexes.Add(ints[0]);
                        }
                    }
                    else throw new BenignException("invalid selection: " + token);
                }
                else
                {
                    if (ints[1] <= menuItemsCount)
                    {
                        for (int i = ints[0]; i <= ints[1]; i++)
                        {
                            if (except)
                            {
                                indexes.Remove(i);
                            }
                            else
                            {
                                indexes.Add(i);
                            }
                        }
                    }
                    else throw new BenignException("invalid range: " + token);
                }

                input = input.Replace(match.Value, "");
                match = NumericRangePattern.Match(input);
            }

            if (input.Length > 0)
            {
                throw new BenignException("invalid input: " + input);
            }

            return indexes;
        }
    }
}
