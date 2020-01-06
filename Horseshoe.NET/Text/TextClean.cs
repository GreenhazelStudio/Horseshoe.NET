using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Horseshoe.NET.Collections;

namespace Horseshoe.NET.Text
{
    // UniCode ref https://en.wikipedia.org/wiki/List_of_Unicode_characters#Basic_Latin
    public static class TextClean
    {
        public static event TextCleanCharacterIdentified CharacterIdentified;

        public static string CleanString(string text, TextCleanMode textCleanMode = default, object customTextCleanDictionary = null, char[] charsToRemove = null)
        {
            if ((textCleanMode & TextCleanMode.RemoveWhitespace) == TextCleanMode.RemoveWhitespace)
            {
                text = WhitespaceRegex.Replace(text, "");
            }
            else if ((textCleanMode & TextCleanMode.NormalizeAndCombineWhitespace) == TextCleanMode.NormalizeAndCombineWhitespace)
            {
                text = WhitespaceRegex.Replace(text, " ");
            }

            var list = new List<char>(text.ToCharArray());

            if ((textCleanMode & TextCleanMode.NormalizeWhitespace) == TextCleanMode.NormalizeWhitespace || textCleanMode == TextCleanMode.All || textCleanMode == TextCleanMode.AllExtended)
            {
                CleanString(list, Whitespaces, ' ', category: nameof(Whitespaces));
            }

            if ((textCleanMode & TextCleanMode.RemoveNonprintables) == TextCleanMode.RemoveNonprintables || textCleanMode == TextCleanMode.All || textCleanMode == TextCleanMode.AllExtended)
            {
                CleanString(list, Nonprintables, "", category: nameof(Nonprintables));
            }

            if ((textCleanMode & TextCleanMode.NormalizePunctuation) == TextCleanMode.NormalizePunctuation || textCleanMode == TextCleanMode.All || textCleanMode == TextCleanMode.AllExtended)
            {
                CleanString(list, Punctuation, category: nameof(Punctuation));
                CleanString(list, ComplexPunctuation, category: nameof(ComplexPunctuation));
            }

            if ((textCleanMode & TextCleanMode.NormalizeNumbersMathAndProgramming) == TextCleanMode.NormalizeNumbersMathAndProgramming || textCleanMode == TextCleanMode.All || textCleanMode == TextCleanMode.AllExtended)
            {
                CleanString(list, NumbersMathAndProgramming, category: nameof(NumbersMathAndProgramming));
                CleanString(list, ComplexNumbersMathAndProgramming, category: nameof(ComplexNumbersMathAndProgramming));
            }

            if ((textCleanMode & TextCleanMode.NormalizeSuperscriptsAndSubscripts) == TextCleanMode.NormalizeSuperscriptsAndSubscripts || textCleanMode == TextCleanMode.All || textCleanMode == TextCleanMode.AllExtended)
            {
                CleanString(list, SuperscriptsAndSubscripts, category: nameof(SuperscriptsAndSubscripts));
            }

            if ((textCleanMode & TextCleanMode.NormalizeLatin) == TextCleanMode.NormalizeLatin || textCleanMode == TextCleanMode.All)
            {
                CleanString(list, Latin, category: nameof(Latin));
                CleanString(list, ComplexLatin, category: nameof(ComplexLatin));
            }

            if ((textCleanMode & TextCleanMode.NormalizeLatinExtended) == TextCleanMode.NormalizeLatinExtended || textCleanMode == TextCleanMode.AllExtended)
            {
                CleanString(list, LatinExtended, category: nameof(LatinExtended));
                CleanString(list, ComplexLatinExtended, category: nameof(ComplexLatinExtended));
            }

            if ((textCleanMode & TextCleanMode.NormalizeGreek) == TextCleanMode.NormalizeGreek || textCleanMode == TextCleanMode.All)
            {
                CleanString(list, Greek, category: nameof(Greek));
                CleanString(list, ComplexGreek, category: nameof(ComplexGreek));
            }

            if ((textCleanMode & TextCleanMode.NormalizeGreekExtended) == TextCleanMode.NormalizeGreekExtended || textCleanMode == TextCleanMode.AllExtended)
            {
                CleanString(list, GreekExtended, category: nameof(GreekExtended));
                CleanString(list, ComplexGreek, category: nameof(ComplexGreek));                // there is no complex greek extended at this time
            }

            if ((textCleanMode & TextCleanMode.NormalizeCyrillic) == TextCleanMode.NormalizeCyrillic || textCleanMode == TextCleanMode.All || textCleanMode == TextCleanMode.AllExtended)
            {
                CleanString(list, Cyrillic, category: nameof(Cyrillic));
                CleanString(list, ComplexCyrillic, category: nameof(ComplexCyrillic));
            }

            if ((textCleanMode & TextCleanMode.NormalizeSymbols) == TextCleanMode.NormalizeSymbols || textCleanMode == TextCleanMode.All || textCleanMode == TextCleanMode.AllExtended)
            {
                CleanString(list, Symbols, category: nameof(Symbols));
                CleanString(list, ComplexSymbols, category: nameof(ComplexSymbols));
            }

            if (customTextCleanDictionary != null)
            {
                if (customTextCleanDictionary is IDictionary<char, char[]> dictionary)
                {
                    CleanString(list, dictionary, category: null, isCustomDictionary: true);
                }
                else if (customTextCleanDictionary is IDictionary<string, char[]> complexDictionary)
                {
                    CleanString(list, complexDictionary, category: null, isCustomDictionary: true);
                }
                else
                {
                    throw new UtilityException("customDictionary must be either of type IDictionary<char, char[]> or IDictionary<string, char[]>");
                }
            }

            if (charsToRemove != null)
            {
                CleanString(list, charsToRemove, "", category: nameof(charsToRemove), isCustomDictionary: true);
            }

            text = new string(list.ToArray());
            return text;
        }

        public static string CleanString(string text, char[] charsToReplace, char replacement, string category = null, bool isCustomDictionary = false)
        {
            var list = new List<char>(text.ToCharArray());
            CleanString(list, charsToReplace, replacement, category: category, isCustomDictionary: isCustomDictionary);
            return new string(list.ToArray());
        }

        public static void CleanString(List<char> list, char[] charsToReplace, char replacement, string category = null, bool isCustomDictionary = false)
        {
            int pos;
            foreach (char c in charsToReplace)
            {
                if (CharacterIdentified != null)
                {
                    pos = list.IndexOf(c);
                    if (pos > -1)
                    {
                        CharacterIdentified.Invoke(c, new string(new[] { replacement }), pos, category, isCustomDictionary);
                    }
                }

                list.Replace(c, replacement);
            }
        }

        public static string CleanString(string text, char[] charsToReplace, string replacement, string category = null, bool isCustomDictionary = false)
        {
            var list = new List<char>(text.ToCharArray());
            CleanString(list, charsToReplace, replacement, category: category, isCustomDictionary: isCustomDictionary);
            return new string(list.ToArray());
        }

        public static void CleanString(List<char> list, char[] charsToReplace, string replacement, string category = null, bool isCustomDictionary = false)
        {
            int pos;
            var replacements = replacement.ToCharArray();
            foreach (char c in charsToReplace)
            {
                if (CharacterIdentified != null)
                {
                    pos = list.IndexOf(c);
                    if (pos > -1)
                    {
                        CharacterIdentified.Invoke(c, replacement, pos, category, isCustomDictionary);
                    }
                }

                list.Replace(c, replacements);
            }
        }

        public static string CleanString(string text, IDictionary<char, char[]> replacements, string category = null, bool isCustomDictionary = false)
        {
            var list = new List<char>(text.ToCharArray());
            CleanString(list, replacements, category: category, isCustomDictionary: isCustomDictionary);
            return new string(list.ToArray());
        }

        public static void CleanString(List<char> list, IDictionary<char, char[]> replacements, string category = null, bool isCustomDictionary = false)
        {
            foreach (var kvp in replacements)
            {
                CleanString(list, kvp.Value, kvp.Key, category: category, isCustomDictionary: isCustomDictionary);
            }
        }

        public static string CleanString(string text, IDictionary<string, char[]> replacements, string category = null, bool isCustomDictionary = false)
        {
            var list = new List<char>(text.ToCharArray());
            CleanString(list, replacements, category: category, isCustomDictionary: isCustomDictionary);
            return new string(list.ToArray());
        }

        public static void CleanString(List<char> list, IDictionary<string, char[]> replacements, string category = null, bool isCustomDictionary = false)
        {
            foreach (var kvp in replacements)
            {
                CleanString(list, kvp.Value, kvp.Key, category: category, isCustomDictionary: isCustomDictionary);
            }
        }

        static readonly Regex WhitespaceRegex = new Regex(@"\s+");

        public static readonly char[] Whitespaces = new[]
        {//  10    13     9     nbsp
            '\n', '\r', '\t', '\u00A0'
        };

        public static readonly char[] Nonprintables = new[]
        {  
            // Controls (0 - 31)
            '\u0000', '\u0001', '\u0002', '\u0003', '\u0004', '\u0005', '\u0006', '\u0007', '\u0008',                     '\u000B', '\u000C',           '\u000E', '\u000F',
            '\u0010', '\u0011', '\u0012', '\u0013', '\u0014', '\u0015', '\u0016', '\u0017', '\u0018', '\u0019', '\u001A', '\u001B', '\u001C', '\u001D', '\u001E', '\u001F', 

            // Controls (128 - 159)
            '\u0080', '\u0081', '\u0082', '\u0083', '\u0084', '\u0085', '\u0086', '\u0087', '\u0088', '\u0089', '\u008A', '\u008B', '\u008C', '\u008D', '\u008E', '\u008F',
            '\u0090', '\u0091', '\u0092', '\u0093', '\u0094', '\u0095', '\u0096', '\u0097', '\u0098', '\u0099', '\u009A', '\u009B', '\u009C', '\u009D', '\u009E', '\u009F',
        
            /* UnicodeReplacementChar � (65533) */
            '\uFFFD'
        };

        public static readonly IDictionary<char, char[]> Punctuation = new Dictionary<char, char[]>
        {
            { '-', new[] { '–', '—', '―', '‾' } },
            { '_', new[] { '‗' } },
            { '|', new[] { '‖', '⁝', '⁞' } },
            { '/', new[] { '⁄' } },
            { '\'', new[] { '‘', '’', '‚', '‛', '′' } },
            { '\"', new[] { '“', '”', '„', '‟', '«', '»' } },
            { 'o', new[] { '•' } },
            { ';', new[] { '⁏', '⍮' } },
            { '?', new[] { '¿' } },
        }.AsImmutable();

        public static readonly IDictionary<string, char[]> ComplexPunctuation = new Dictionary<string, char[]>
        {
            { "...", new[] { '…' } },
            { "!!", new[] { '‼' } },
            { "??", new[] { '⁇' } },
            { "?!", new[] { '⁈' } },
            { "!?", new[] { '⁉' } },
            { "''", new[] { '″', '‶' } },
            { "'''", new[] { '‴', '‷' } },
            { "''''", new[] { '⁗' } },
        }.AsImmutable();

        public static readonly IDictionary<char, char[]> NumbersMathAndProgramming = new Dictionary<char, char[]>
        {
            { 'A', new[] { '∀' } },
            { 'C', new[] { '∁' } },
            { 'E', new[] { 'ℇ', '∃', '∄', '∈', '∉', '∊', '∋', '∌', '∍' } },
            { 'a', new[] { '⍺', '⍶'} },
            { 'h', new[] { 'ℎ', 'ℏ' } },
            { 'i', new[] { '⍳', '⍸' } },
            { 'o', new[] { '∘' } },       // U+2218 Ring operator
            { 'p', new[] { '⍴' } },
            { 'w', new[] { '⍵', '⍹' } },
            { 'x', new[] { '×', '⋅' } },  // U+00D7 Multiplication sign, U+22C5 Dot operator
            { '0', new[] { '⓪', '⓿', '∅' } },
            { '1', new[] { '①', '⓵' } },
            { '2', new[] { '②', '⓶', '↊' } },
            { '3', new[] { '③', '⓷', '↋' } },
            { '4', new[] { '④', '⓸' } },
            { '5', new[] { '⑤', '⓹' } },
            { '6', new[] { '⑥', '⓺' } },
            { '7', new[] { '⑦', '⓻' } },
            { '8', new[] { '⑧', '⓼' } },
            { '9', new[] { '⑨', '⓽' } },
            { '+', new[] { '∔' } },
            { '*', new[] { '∗' } },
            { '/', new[] { '∕', '√', '÷' } },
            { '\\', new[] { '∖' } },
            { '~', new[] { '∼', '∽', '∿' } },
        }.AsImmutable();

        public static readonly IDictionary<string, char[]> ComplexNumbersMathAndProgramming = new Dictionary<string, char[]>
        {
            { "(1)", new[] { '⑴' } },
            { "(2)", new[] { '⑵' } },
            { "(3)", new[] { '⑶' } },
            { "(4)", new[] { '⑷' } },
            { "(5)", new[] { '⑸' } },
            { "(6)", new[] { '⑹' } },
            { "(7)", new[] { '⑺' } },
            { "(8)", new[] { '⑻' } },
            { "(9)", new[] { '⑼' } },
            { "(10)", new[] { '⑽' } },
            { "(11)", new[] { '⑾' } },
            { "(12)", new[] { '⑿' } },
            { "(13)", new[] { '⒀' } },
            { "(14)", new[] { '⒁' } },
            { "(15)", new[] { '⒂' } },
            { "(16)", new[] { '⒃' } },
            { "(17)", new[] { '⒄' } },
            { "(18)", new[] { '⒅' } },
            { "(19)", new[] { '⒆' } },
            { "(20)", new[] { '⒇' } },
            { "1.", new[] { '⒈' } },
            { "2.", new[] { '⒉' } },
            { "3.", new[] { '⒊' } },
            { "4.", new[] { '⒋' } },
            { "5.", new[] { '⒌' } },
            { "6.", new[] { '⒍' } },
            { "7.", new[] { '⒎' } },
            { "8.", new[] { '⒏' } },
            { "9.", new[] { '⒐' } },
            { "10.", new[] { '⒑' } },
            { "11.", new[] { '⒒' } },
            { "12.", new[] { '⒓' } },
            { "13.", new[] { '⒔' } },
            { "14.", new[] { '⒕' } },
            { "15.", new[] { '⒖' } },
            { "16.", new[] { '⒗' } },
            { "17.", new[] { '⒘' } },
            { "18.", new[] { '⒙' } },
            { "19.", new[] { '⒚' } },
            { "20.", new[] { '⒛' } },
            { "10", new[] { '⓾' } },
            { "0/00", new[] { '‰' } },
            { "0/000", new[] { '‱' } },
            { "0/3", new[] { '↉' } },
            { "1/", new[] { '⅟' } },
            { "1/2", new[] { '½' } },
            { "1/3", new[] { '⅓' } },
            { "1/4", new[] { '¼' } },
            { "1/5", new[] { '⅕' } },
            { "1/6", new[] { '⅙' } },
            { "1/7", new[] { '⅐' } },
            { "1/8", new[] { '⅛' } },
            { "1/9", new[] { '⅑' } },
            { "1/10", new[] { '⅒' } },
            { "2/3", new[] { '⅔' } },
            { "2/5", new[] { '⅖' } },
            { "3/", new[] { '¾' } },
            { "3/4", new[] { '∛' } },
            { "3/5", new[] { '⅗' } },
            { "3/8", new[] { '⅜' } },
            { "4/", new[] { '∜' } },
            { "4/5", new[] { '⅘' } },
            { "5/6", new[] { '⅚' } },
            { "5/8", new[] { '⅝' } },
            { "7/8", new[] { '⅞' } },
            { "+/-", new[] { '∓' } },
            { "...", new[] { '⋰', '⋱' } },
            { ":", new[] { '∴', '∵', '∶' } },
            { "::", new[] { '∷' } },
        }.AsImmutable();

        public static readonly IDictionary<char, char[]> SuperscriptsAndSubscripts = new Dictionary<char, char[]>
        {
            { '0', new[] { '⁰', '₀' } },
            { '1', new[] { '¹', '₁' } },
            { '2', new[] { '²', '₂' } },
            { '3', new[] { '³', '₃' } },
            { '4', new[] { '⁴', '₄' } },
            { '5', new[] { '⁵', '₅' } },
            { '6', new[] { '⁶', '₆' } },
            { '7', new[] { '⁷', '₇' } },
            { '8', new[] { '⁸', '₈' } },
            { '9', new[] { '⁹', '₉' } },
            { 'a', new[] { 'ª', 'ₐ' } },
            { 'e', new[] { 'ₑ', 'ₔ' } },
            { 'h', new[] { 'ʰ', 'ʱ', 'ₕ' } },
            { 'i', new[] { 'ⁱ' } },
            { 'j', new[] { 'ʲ' } },
            { 'k', new[] { 'ₖ' } },
            { 'l', new[] { 'ˡ', 'ₗ' } },
            { 'm', new[] { 'ⁿ', 'ₘ' } },
            { 'n', new[] { 'ⁿ', 'ₙ' } },
            { 'o', new[] { 'º', 'ₒ' } },
            { 'p', new[] { 'ₚ' } },
            { 'R', new[] { 'ʶ' } },
            { 'r', new[] { 'ʳ' , 'ʴ', 'ʵ' } },
            { 's', new[] { 'ˢ', 'ₛ' } },
            { 't', new[] { 'ₜ' } },
            { 'w', new[] { 'ʷ' } },
            { 'x', new[] { '˟', 'ˣ', 'ₓ' } },
            { 'y', new[] { 'ʸ' } },
        }.AsImmutable();

        public static readonly IDictionary<char, char[]> Latin = new Dictionary<char, char[]>
        {
            { 'A', new[] { 'À', 'Á', 'Â', 'Ã', 'Ä', 'Å'  } },
            { 'C', new[] { 'Ç' } },
            { 'E', new[] { 'È', 'É', 'Ê', 'Ë' } },
            { 'I', new[] { 'Ì', 'Í', 'Î', 'Ï' } },
            { 'N', new[] { 'Ñ' } },
            { 'O', new[] { 'Ò', 'Ó', 'Ô', 'Õ', 'Ö', 'Ø' } },
            { 'U', new[] { 'Ù', 'Ú', 'Û', 'Ü' } },
            { 'Y', new[] { 'Ý' } },
            { 'a', new[] { 'à', 'á', 'â', 'ã', 'ä', 'å' } },
            { 'c', new[] { 'ç' } },
            { 'e', new[] { 'è', 'é', 'ê', 'ë' } },
            { 'i', new[] { 'ì', 'í', 'î', 'ï' } },
            { 'n', new[] { 'ñ' } },
            { 'o', new[] { 'ò', 'ó', 'ô', 'õ', 'ö', 'ø' } },
            { 'u', new[] { 'ù', 'ú', 'û', 'ü' } },
            { 'y', new[] { 'ý' } },
        }.AsImmutable();

        public static IDictionary<char, char[]> LatinExtended => new Dictionary<char, char[]>
        (
            Latin.Merge
            (
                new Dictionary<char, char[]>
                {
                    { 'A', new[] { 'Ā', 'Ă', 'Ą', 'Ǎ', 'Ǟ', 'Ǡ', 'Ǻ', 'Ȁ', 'Ȃ', 'Ȧ', 'Ⱥ' } },
                    { 'B', new[] { 'Ɓ', 'Ƃ', 'Ƀ', 'Ḃ' } },
                    { 'C', new[] { 'Ć', 'Ĉ', 'Ċ', 'Č', 'Ƈ', 'Ȼ' } },
                    { 'D', new[] { 'Ď', 'Đ', 'Ɖ', 'Ɗ', 'Ƌ', 'Ḋ' } },
                    { 'E', new[] { 'Ē', 'Ĕ', 'Ė', 'Ę', 'Ě', 'Ǝ', 'Ə', 'Ɛ', 'Ʃ', 'Ȅ', 'Ȇ', 'Ȩ', 'Ɇ' } },
                    { 'F', new[] { 'Ƒ', 'Ḟ' } },
                    { 'G', new[] { 'Ĝ', 'Ğ', 'Ġ', 'Ģ', 'Ɠ', 'Ǥ', 'Ǧ', 'Ǵ' } },
                    { 'H', new[] { 'Ĥ', 'Ħ', 'Ȟ' } },
                    { 'I', new[] { 'Ĩ', 'Ī', 'Ĭ', 'Į', 'İ', 'Ɩ', 'Ɨ', 'Ǐ', 'Ȉ', 'Ȋ' } },
                    { 'J', new[] { 'Ĵ', 'Ɉ' } },
                    { 'K', new[] { 'Ķ', 'Ƙ', 'Ǩ' } },
                    { 'L', new[] { 'Ĺ', 'Ļ', 'Ľ', 'Ŀ', 'Ł', 'Ƚ' } },
                    { 'M', new[] { 'Ɯ', 'Ṁ' } },
                    { 'N', new[] { 'Ń', 'Ņ', 'Ň', 'Ŋ', 'Ɲ', 'Ǹ', 'Ƞ' } },
                    { 'O', new[] { 'Ō', 'Ŏ', 'Ő', 'Ɔ', 'Ɵ', 'Ơ', 'Ǒ', 'Ǫ', 'Ǭ', 'Ǿ', 'Ȍ', 'Ȏ', 'Ȫ', 'Ȭ', 'Ȯ', 'Ȱ' } },
                    { 'P', new[] { 'Ƥ', 'Ṗ' } },
                    { 'Q', new[] { 'Ɋ' } },
                    { 'R', new[] { 'Ŕ', 'Ŗ', 'Ř', 'Ȑ', 'Ȓ', 'Ɍ' } },
                    { 'S', new[] { 'Ś', 'Ŝ', 'Ş', 'Š', 'Ƨ', 'Ș', 'Ṡ' } },
                    { 'T', new[] { 'Ţ', 'Ť', 'Ŧ', 'Ƭ', 'Ʈ', 'Ț', 'Ⱦ', 'Ṫ' } },
                    { 'U', new[] { 'Ũ', 'Ū', 'Ŭ', 'Ů', 'Ű', 'Ų', 'Ư', 'Ʊ', 'Ǔ', 'Ǖ', 'Ǘ', 'Ǚ', 'Ǜ', 'Ȕ', 'Ȗ', 'Ʉ' } },
                    { 'V', new[] { 'Ʋ', 'Ʌ' } },
                    { 'W', new[] { 'Ŵ', 'Ẁ', 'Ẃ', 'Ẅ' } },
                    { 'Y', new[] { 'Ŷ', 'Ÿ', 'Ɣ', 'Ƴ', 'Ȳ', 'Ɏ', 'Ỳ' } },
                    { 'Z', new[] { 'Ź', 'Ż', 'Ž', 'Ƶ', 'Ȥ' } },
                    { 'a', new[] { 'ā', 'ă', 'ą', 'ǎ', 'ǟ', 'ǡ', 'ǻ', 'ȁ', 'ȃ', 'ȧ' } },
                    { 'b', new[] { 'ƀ', 'ƃ', 'ḃ' } },
                    { 'c', new[] { 'ć', 'ĉ', 'ċ', 'č', 'ƈ', 'ȼ' } },
                    { 'd', new[] { 'ď', 'đ', 'ƌ', 'ȡ', 'ḋ' } },
                    { 'e', new[] { 'ē', 'ĕ', 'ė', 'ę', 'ě', 'ǝ', 'ȅ', 'ȇ', 'ȩ', 'ɇ', 'ə' } },
                    { 'f', new[] { 'ƒ', 'ḟ' } },
                    { 'g', new[] { 'ĝ', 'ğ', 'ġ', 'ģ', 'ǥ', 'ǧ', 'ǵ' } },
                    { 'h', new[] { 'ĥ', 'ħ', 'ȟ' } },
                    { 'i', new[] { 'ĩ', 'ī', 'ĭ', 'į', 'ı', 'ǐ', 'ȉ', 'ȋ' } },
                    { 'j', new[] { 'ĵ', 'ȷ', 'ɉ' } },
                    { 'k', new[] { 'ķ', 'ĸ', 'ƙ', 'ǩ' } },
                    { 'l', new[] { 'ĺ', 'ļ', 'ľ', 'ŀ', 'ł', 'ƚ', 'ȴ' } },
                    { 'm', new[] { 'ṁ' } },
                    { 'n', new[] { 'ń', 'ņ', 'ň', 'ŉ', 'ŋ', 'ƞ', 'ǹ', 'ȵ' } },
                    { 'o', new[] { 'ō', 'ŏ', 'ő', 'ơ', 'ǒ', 'ǫ', 'ǭ', 'ǿ', 'ȍ', 'ȏ', 'ȫ', 'ȭ', 'ȯ', 'ȱ' } },
                    { 'p', new[] { 'ƥ', 'ṗ' } },
                    { 'q', new[] { 'ɋ' } },
                    { 'r', new[] { 'ŕ', 'ŗ', 'ř', 'ȑ', 'ȓ', 'ɍ', 'ɼ' } },
                    { 's', new[] { 'ś', 'ŝ', 'ş', 'š', 'ſ', 'ƨ', 'ș', 'ȿ', 'ṡ', 'ẛ' } },
                    { 't', new[] { 'ţ', 'ť', 'ŧ', 'ƫ', 'ƭ', 'ț', 'ȶ', 'ṫ' } },
                    { 'u', new[] { 'ũ', 'ū', 'ŭ', 'ů', 'ű', 'ų', 'ư', 'ǔ', 'ǖ', 'ǘ', 'ǚ', 'ǜ', 'ȕ', 'ȗ' } },
                    { 'w', new[] { 'ŵ', 'ẁ', 'ẃ', 'ẅ' } },
                    { 'y', new[] { 'ŷ', 'ƴ', 'ȳ', 'ɏ', 'ỳ' } },
                    { 'z', new[] { 'ź', 'ż', 'ž', 'ƶ', 'ȥ', 'ɀ' } },
                },
                (latinCharArray, extendedLatinCharArray) => latinCharArray.ConcatIf(extendedLatinCharArray).ToArray()
            )
        );

        public static readonly IDictionary<string, char[]> ComplexLatin = new Dictionary<string, char[]>
        {
            { "AE", new[] { 'Æ' } },
            { "C", new[] { 'Ⅽ' } },
            { "D", new[] { 'Ⅾ' } },
            { "I", new[] { 'Ⅰ' } },
            { "II", new[] { 'Ⅱ' } },
            { "III", new[] { 'Ⅲ' } },
            { "IV", new[] { 'Ⅳ' } },
            { "IX", new[] { 'Ⅸ' } },
            { "L", new[] { 'Ⅼ' } },
            { "M", new[] { 'Ⅿ' } },
            { "V", new[] { 'Ⅴ' } },
            { "VI", new[] { 'Ⅵ' } },
            { "VII", new[] { 'Ⅶ' } },
            { "VIII", new[] { 'Ⅷ' } },
            { "X", new[] { 'Ⅹ' } },
            { "XI", new[] { 'Ⅺ' } },
            { "XII", new[] { 'Ⅻ' } },
            { "YR", new[] { 'Ʀ' }},
            { "ae", new[] { 'æ' } },
            { "c", new[] { 'ⅽ' } },
            { "d", new[] { 'ⅾ' } },
            { "ff", new[] { 'ﬀ' } },
            { "fi", new[] { 'ﬁ' } },
            { "fl", new[] { 'ﬂ' } },
            { "ffi", new[] { 'ﬃ' } },
            { "ffl", new[] { 'ﬄ' } },
            { "ft", new[] { 'ﬅ' } },
            { "hv", new[] { 'ƕ' } },
            { "i", new[] { 'ⅰ' } },
            { "ii", new[] { 'ⅱ' } },
            { "iii", new[] { 'ⅲ' } },
            { "iv", new[] { 'ⅳ' } },
            { "ix", new[] { 'ⅸ' } },
            { "m", new[] { 'ⅿ' } },
            { "st", new[] { 'ﬆ' } },
            { "v", new[] { 'ⅴ' } },
            { "vi", new[] { 'ⅵ' } },
            { "vii", new[] { 'ⅶ' } },
            { "viii", new[] { 'ⅷ' } },
            { "x", new[] { 'ⅹ' } },
            { "xi", new[] { 'ⅺ' } },
            { "xii", new[] { 'ⅻ' } },
        }.AsImmutable();

        public static IDictionary<string, char[]> ComplexLatinExtended => new Dictionary<string, char[]>
        (
            ComplexLatin.Merge
            (
                new Dictionary<string, char[]>
                {
                    { "AE", new[] { 'Ǣ', 'Ǽ' } },
                    { "DZ", new[] { 'Ǆ', 'Ǳ' } },
                    { "Dz", new[] { 'ǅ', 'ǲ' } },
                    { "IJ", new[] { 'Ĳ' } },
                    { "LJ", new[] { 'Ǉ' } },
                    { "Lj", new[] { 'ǈ' } },
                    { "NJ", new[] { 'Ǌ' } },
                    { "Nj", new[] { 'ǋ' } },
                    { "OE", new[] { 'Œ' } },
                    { "OI", new[] { 'Ƣ' } },
                    { "OU", new[] { 'Ȣ' } },
                    { "ae", new[] { 'ǣ', 'ǽ' } },
                    { "db", new[] { 'ȸ' } },
                    { "dz", new[] { 'ǆ', 'ǳ' } },
                    { "ij", new[] { 'ĳ' } },
                    { "lj", new[] { 'ǉ' } },
                    { "nj", new[] { 'ǌ' } },
                    { "oe", new[] { 'œ' } },
                    { "oi", new[] { 'ƣ' } },
                    { "ou", new[] { 'ȣ' } },
                    { "qp", new[] { 'ȹ' } },
                },
                (latinCharArray, extendedLatinCharArray) => latinCharArray.ConcatIf(extendedLatinCharArray).ToArray()
            )
        );

        public static readonly IDictionary<char, char[]> Greek = new Dictionary<char, char[]>
        {
            { 'A', new[] { 'Ά', 'Α' } },
            { 'B', new[] { 'Β', 'ϐ' } },
            { 'C', new[] { 'Ϲ', 'Ͻ', 'Ͼ', 'Ͽ' } },
            { 'E', new[] { 'Έ', 'Ε', 'Σ' } },
            { 'F', new[] { 'Ϝ' } },
            { 'H', new[] { 'Ή', 'Η' } },
            { 'I', new[] { 'Ί', 'Ι', 'Ϊ' } },
            { 'J', new[] { 'Ϳ' } },
            { 'K', new[] { 'Κ', 'Ϗ' } },
            { 'M', new[] { 'Μ', 'Ϻ' } },
            { 'N', new[] { 'Ͷ', 'Ν' } },
            { 'O', new[] { 'Ό', 'Θ', 'Ο', 'Φ', 'ϴ' } },
            { 'P', new[] { 'Ρ' } },
            { 'Q', new[] { 'Ϙ' } },
            { 'T', new[] { 'Ͳ', 'Τ' } },
            { 'X', new[] { 'Χ' } },
            { 'Y', new[] { 'Ύ', 'Υ', 'Ϋ', 'ϒ', 'ϓ', 'ϔ' } },
            { 'Z', new[] { 'Ζ' } },
            { 'a', new[] { 'ά', 'α' } },
            { 'b', new[] { 'β' } },
            { 'c', new[] { 'ς' } },
            { 'e', new[] { 'έ', 'ε', 'ϵ', '϶' } },
            { 'f', new[] { 'ϝ' } },
            { 'i', new[] { 'ΐ', 'ί', 'ι', 'ϊ' } },
            { 'k', new[] { 'κ' } },
            { 'm', new[] { 'ϻ' } },
            { 'n', new[] { 'ͷ', 'ή', 'η' } },
            { 'o', new[] { 'θ', 'ο', 'σ', 'ό' } },
            { 'p', new[] { 'ρ' } },
            { 'q', new[] { 'ϙ' } },
            { 't', new[] { 'ͳ', 'τ' } },
            { 'u', new[] { 'ΰ', 'υ', 'ϋ', 'ύ' } },
            { 'v', new[] { 'ν' } },
            { 'w', new[] { 'ω', 'ώ' } },
            { 'x', new[] { 'χ' } },
            { 'y', new[] { 'γ' } },
            { 'z', new[] { 'ζ' } },
        }.AsImmutable();

        public static IDictionary<char, char[]> GreekExtended => new Dictionary<char, char[]>
        (
            Greek.Merge
            (
                new Dictionary<char, char[]>
                {
                    { 'A', new[] { 'Ἀ', 'Ἁ', 'Ἂ', 'Ἃ', 'Ἄ', 'Ἅ', 'Ἆ', 'Ἇ', 'ᾈ', 'ᾉ', 'ᾊ', 'ᾋ', 'ᾌ', 'ᾍ', 'ᾎ', 'ᾏ', 'Ᾰ', 'Ᾱ', 'Ὰ', 'Ά', 'ᾼ' } },
                    { 'E', new[] { 'Ἐ', 'Ἑ', 'Ἒ', 'Ἓ', 'Ἔ', 'Ἕ', 'Ὲ', 'Έ' } },
                    { 'H', new[] { 'Ἠ', 'Ἡ', 'Ἢ', 'Ἣ', 'Ἤ', 'Ἥ', 'Ἦ', 'Ἧ', 'ᾘ', 'ᾙ', 'ᾚ', 'ᾛ', 'ᾜ', 'ᾝ', 'ᾞ', 'ᾟ', 'Ὴ', 'Ή', 'ῌ' } },
                    { 'I', new[] { 'Ἰ', 'Ἱ', 'Ἲ', 'Ἳ', 'Ἴ', 'Ἵ', 'Ἶ', 'Ἷ', 'Ῐ', 'Ῑ', 'Ὶ', 'Ί' } },
                    { 'O', new[] { 'Ὀ', 'Ὁ', 'Ὂ', 'Ὃ', 'Ὄ', 'Ὅ', 'Ὸ', 'Ό' } },
                    { 'P', new[] { 'Ῥ' } },
                    { 'Y', new[] { 'Ὑ', 'Ὓ', 'Ὕ', 'Ὗ', 'Ῠ', 'Ῡ', 'Ὺ', 'Ύ' } },
                    { 'a', new[] { 'ἀ', 'ἁ', 'ἂ', 'ἃ', 'ἄ', 'ἅ', 'ἆ', 'ἇ', 'ὰ', 'ά', 'ᾀ', 'ᾁ', 'ᾂ', 'ᾃ', 'ᾄ', 'ᾅ', 'ᾆ', 'ᾇ', 'ᾰ', 'ᾱ', 'ᾲ', 'ᾳ', 'ᾴ', 'ᾶ', 'ᾷ' } },
                    { 'e', new[] { 'ἐ', 'ἑ', 'ἒ', 'ἓ', 'ἔ', 'ἕ', 'ὲ', 'έ' } },
                    { 'i', new[] { 'ἰ', 'ἱ', 'ἲ', 'ἳ', 'ἴ', 'ἵ', 'ἶ', 'ἷ', 'ὶ', 'ί', 'ῐ', 'ῑ', 'ῒ', 'ΐ', 'ῖ', 'ῗ' } },
                    { 'n', new[] { 'ἠ', 'ἡ', 'ἢ', 'ἣ', 'ἤ', 'ἥ', 'ἦ', 'ἧ', 'ὴ', 'ή', 'ᾐ', 'ᾑ', 'ᾒ', 'ᾓ', 'ᾔ', 'ᾕ', 'ᾖ', 'ᾗ', 'ῂ', 'ῃ', 'ῄ', 'ῆ', 'ῇ' } },
                    { 'o', new[] { 'ὀ', 'ὁ', 'ὂ', 'ὃ', 'ὄ', 'ὅ', 'ὸ', 'ό' } },
                    { 'p', new[] { 'ῤ', 'ῥ' } },
                    { 'u', new[] { 'ὐ', 'ὑ', 'ὒ', 'ὓ', 'ὔ', 'ὕ', 'ὖ', 'ὗ', 'ὺ', 'ύ', 'ῠ', 'ῡ', 'ῢ', 'ΰ', 'ῦ', 'ῧ' } },
                    { 'w', new[] { 'ὠ', 'ὡ', 'ὢ', 'ὣ', 'ὤ', 'ὥ', 'ὦ', 'ὧ', 'ὼ', 'ώ', 'ᾠ', 'ᾡ', 'ᾢ', 'ᾣ', 'ᾤ', 'ᾥ', 'ᾦ', 'ᾧ', 'ῲ', 'ῳ', 'ῴ', 'ῶ', 'ῷ' } },
                },
                (latinCharArray, extendedLatinCharArray) => latinCharArray.ConcatIf(extendedLatinCharArray).ToArray()
            )
        );

        public static readonly IDictionary<string, char[]> ComplexGreek = new Dictionary<string, char[]>
        {
            { "PI", new[] { 'Π' } },
            { "pi", new[] { 'π' } },
            { "mu", new[] { 'μ' } },
        }.AsImmutable();

        public static readonly IDictionary<char, char[]> Cyrillic = new Dictionary<char, char[]>
        {
            { 'A', new[] { 'А', 'Ѧ' } },
            { 'B', new[] { 'Б', 'В' } },
            { 'C', new[] { 'С', 'Ҫ' } },
            { 'E', new[] { 'Ѐ', 'Ё', 'Є', 'Е', 'Э', 'Ҽ', 'Ҿ', 'Ӭ' } },
            { 'H', new[] { 'Н', 'Ң', 'Ҥ', 'Һ' } },
            { 'I', new[] { 'І', 'Ї', 'Ӏ' } },
            { 'J', new[] { 'Ј' } },
            { 'K', new[] { 'Ќ', 'К', 'Қ', 'Ҝ', 'Ҟ', 'Ҡ' } },
            { 'M', new[] { 'М' } },
            { 'N', new[] { 'Ѝ', 'И', 'Й', 'Ҋ', 'Ӣ', 'Ӥ' } },
            { 'O', new[] { 'О', 'Ф', 'Ѳ', 'Ѻ', 'Ӧ', 'Ө', 'Ӫ' } },
            { 'P', new[] { 'Р', 'Ҏ' } },
            { 'R', new[] { 'Я' } },
            { 'S', new[] { 'Ѕ' } },
            { 'T', new[] { 'Т', 'Ҭ' } },
            { 'V', new[] { 'Ѵ', 'Ѷ' } },
            { 'W', new[] { 'Ш', 'Щ', 'Ѡ', 'Ѽ', 'Ѿ' } },
            { 'X', new[] { 'Х', 'Ҳ', 'Җ', 'Ӂ', 'Ӽ', 'Ӿ' } },
            { 'Y', new[] { 'У', 'Ү', 'Ұ', 'Ӯ', 'Ӱ', 'Ӳ' } },
            { 'a', new[] { 'а', 'ѧ' } },
            { 'b', new[] { 'б', 'в' } },
            { 'c', new[] { 'с', 'ҫ' } },
            { 'e', new[] { 'е', 'э', 'ѐ', 'ё', 'є', 'ҽ', 'ҿ', 'ӭ' } },
            { 'h', new[] { 'н', 'ң', 'ҥ', 'һ' } },
            { 'i', new[] { 'і', 'ї' } },
            { 'j', new[] { 'ј' } },
            { 'k', new[] { 'к', 'ќ', 'қ', 'ҝ', 'ҟ', 'ҡ' } },
            { 'm', new[] { 'м' } },
            { 'n', new[] { 'и', 'й', 'ѝ', 'ҋ', 'ӣ', 'ӥ' } },
            { 'o', new[] { 'о', 'ѳ', 'ѻ', 'ӧ', 'ө', 'ӫ' } },
            { 'p', new[] { 'р', 'ҏ' } },
            { 'r', new[] { 'я' } },
            { 's', new[] { 'ѕ' } },
            { 't', new[] { 'т', 'ҭ' } },
            { 'v', new[] { 'ѵ', 'ѷ' } },
            { 'w', new[] { 'ш', 'щ', 'ѡ', 'ѽ', 'ѿ' } },
            { 'x', new[] { 'х', 'ҳ', 'җ', 'ӂ', 'ӽ', 'ӿ' } },
            { 'y', new[] { 'Ў', 'у', 'ў', 'ү', 'ұ', 'ӯ', 'ӱ', 'ӳ' } },
            { '3', new[] { 'З', 'з', 'Ҙ', 'ҙ', 'Ӟ', 'ӟ', 'Ӡ', 'ӡ'  } },
        }.AsImmutable();

        public static readonly IDictionary<string, char[]> ComplexCyrillic = new Dictionary<string, char[]>
        {
            { "Bl", new[] { 'Ӹ' } },
            { "IA", new[] { 'Ѩ' } },
            { "Oy", new[] { 'Ѹ' } },
            { "bl", new[] { 'ӹ' } },
            { "ia", new[] { 'ѩ' } },
            { "oy", new[] { 'ѹ' } },
        }.AsImmutable();

        public static readonly IDictionary<char, char[]> Symbols = new Dictionary<char, char[]>
        {
            { 'A', new[] { 'Ⓐ', 'Å' } },
            { 'B', new[] { 'Ⓑ', 'ℬ', '₿' } },
            { 'C', new[] { 'Ⓒ', 'ℂ', 'ℭ', '₡' } },
            { 'D', new[] { 'Ⓓ', 'ⅅ' } },
            { 'E', new[] { 'Ⓔ', 'ℰ', '€' } },
            { 'F', new[] { 'Ⓕ', 'ℱ', 'Ⅎ', 'ⅎ', '₣' } },
            { 'G', new[] { 'Ⓖ', '⅁' } },
            { 'H', new[] { 'Ⓗ', 'ℋ', 'ℌ', 'ℍ' } },
            { 'I', new[] { 'Ⓘ', 'ℐ', 'ℑ' } },
            { 'J', new[] { 'Ⓙ' } },
            { 'K', new[] { 'Ⓚ', 'K', '₭' } },
            { 'L', new[] { 'Ⓛ', 'ℒ', '⅃', '⅂', '₤' } },
            { 'M', new[] { 'Ⓜ', 'ℳ' } },
            { 'N', new[] { 'Ⓝ', 'ℕ', '₦' } },
            { 'O', new[] { 'Ⓞ' } },
            { 'P', new[] { 'Ⓟ', '♇', 'ℙ', '℘', '℗', '₽' } },
            { 'Q', new[] { 'Ⓠ', '℺', 'ℚ' } },
            { 'R', new[] { 'Ⓡ', '℟', 'ℝ', 'ℜ', 'ℛ' } },
            { 'S', new[] { 'Ⓢ', '₷' } },
            { 'T', new[] { 'Ⓣ', '₸', '₮' } },
            { 'U', new[] { 'Ⓤ' } },
            { 'V', new[] { 'Ⓥ', '℣' } },
            { 'W', new[] { 'Ⓦ', '₩' } },
            { 'X', new[] { 'Ⓧ', '☒', '☓' } },
            { 'Y', new[] { 'Ⓨ', '⅄' } },
            { 'Z', new[] { 'Ⓩ', 'ℤ' } },
            { 'a', new[] { 'ⓐ' } },
            { 'b', new[] { 'ⓑ', '♭' } },
            { 'c', new[] { 'ⓒ' } },
            { 'd', new[] { 'ⓓ', 'ⅆ', '₫' } },
            { 'e', new[] { 'ⓔ', '℮', 'ℯ', 'ⅇ' } },
            { 'f', new[] { 'ⓕ' } },
            { 'g', new[] { 'ⓖ', 'ℊ' } },
            { 'h', new[] { 'ⓗ', 'ℎ', 'ℏ' } },
            { 'i', new[] { 'ⓘ', '℩', 'ℹ', 'ⅈ' } },
            { 'j', new[] { 'ⓙ', 'ⅉ' } },
            { 'k', new[] { 'ⓚ' } },
            { 'l', new[] { 'ⓛ', 'ℓ', 'ˡ' } },
            { 'm', new[] { 'ⓜ', '₥' } },
            { 'n', new[] { 'ⓝ' } },
            { 'o', new[] { 'ⓞ', 'ℴ' } },
            { 'p', new[] { 'ⓟ' } },
            { 'q', new[] { 'ⓠ' } },
            { 'r', new[] { 'ⓡ' } },
            { 's', new[] { 'ⓢ', 'ˢ' } },
            { 't', new[] { 'ⓣ' } },
            { 'u', new[] { 'ⓤ' } },
            { 'v', new[] { 'ⓥ', '˅', '˯' } },
            { 'w', new[] { 'ⓦ' } },
            { 'x', new[] { 'ⓧ', '˟', 'ˣ' } },
            { 'y', new[] { 'ⓨ', 'ˠ' } },
            { 'z', new[] { 'ⓩ' } },
            { '#', new[] { '♯' } },
            { '&', new[] { '⅋' } },
            { '¢', new[] { '₵' } },
            { '/', new[] { '⁄' } },
            { '<', new[] { '˂', '˱' } },
            { '>', new[] { '˃', '˲' } },
            { '^', new[] { '˄', '˰' } },
            { '"', new[] { 'ˮ' } },
            { '´', new[] { 'ˊ', 'ˏ' } },
            { '`', new[] { 'ˋ', 'ˎ', '˴' } },
            { '-', new[] { '˗' } },
            { '_', new[] { 'ˍ' } },
            { ':', new[] { 'ː', '˸' } },
            { '~', new[] { '˜', '˷' } },
            { '=', new[] { '˭' } },
        }.AsImmutable();


        public static readonly IDictionary<string, char[]> ComplexSymbols = new Dictionary<string, char[]>
        {
            { "A/S", new[] { '⅍' } },
            { "CL", new[] { '℄' } },
            { "Cr", new[] { '₢' } },
            { "FAX", new[] { '℻' } },
            { "No", new[] { '№' } },
            { "PL", new[] { '⅊' } },
            { "Pts", new[] { '₧' } },
            { "PX", new[] { '☧' } },
            { "Rs", new[] { '₨' } },
            { "Rx", new[] { '℞' } },
            { "SM", new[] { '℠' } },
            { "TEL", new[] { '℡' } },
            { "TM", new[] { '™' } },
            { "a/c", new[] { '℀' } },
            { "a/s", new[] { '℁' } },
            { "c/o", new[] { '℅' } },
            { "c/u", new[] { '℆' } },
            { "lb", new[] { '℔' } },
            { "(a)", new[] { '⒜' } },
            { "(b)", new[] { '⒝' } },
            { "(c)", new[] { '⒞' } },
            { "(d)", new[] { '⒟' } },
            { "(e)", new[] { '⒠' } },
            { "(f)", new[] { '⒡' } },
            { "(g)", new[] { '⒢' } },
            { "(h)", new[] { '⒣' } },
            { "(i)", new[] { '⒤' } },
            { "(j)", new[] { '⒥' } },
            { "(k)", new[] { '⒦' } },
            { "(l)", new[] { '⒧' } },
            { "(m)", new[] { '⒨' } },
            { "(n)", new[] { '⒩' } },
            { "(o)", new[] { '⒪' } },
            { "(p)", new[] { '⒫' } },
            { "(q)", new[] { '⒬' } },
            { "(r)", new[] { '⒭' } },
            { "(s)", new[] { '⒮' } },
            { "(t)", new[] { '⒯' } },
            { "(u)", new[] { '⒰' } },
            { "(v)", new[] { '⒱' } },
            { "(w)", new[] { '⒲' } },
            { "(x)", new[] { '⒳' } },
            { "(y)", new[] { '⒴' } },
            { "(z)", new[] { '⒵' } },
            { ":(", new[] { '☹' } },
            { ":)", new[] { '☺', '☻' } },
            { "°C", new[] { '℃' } },
            { "°F", new[] { '℉' } },
            { "´´", new[] { '˶' } },
            { "``", new[] { '˵' } },
        }.AsImmutable();
    }
}
