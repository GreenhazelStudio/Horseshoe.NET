using System;
using System.Collections.Generic;
using System.Linq;

using Horseshoe.NET.Text;

namespace Horseshoe.NET.Collections
{
    public static class CollectionUtil
    {
        /// <summary>
        /// Grows a collection to the desired length by adding elements at the start
        /// </summary>
        public static IEnumerable<T> PadStart<T>(IEnumerable<T> collection, T padWith, int targetLength, TruncatePolicy truncPolicy = default)
        {
            if (targetLength <= 0) throw new ArgumentException("The target length must be greater than 0");
            if (collection.Count() == targetLength) return collection;
            if (collection.Count() > targetLength)
            {
                switch (truncPolicy)
                {
                    case TruncatePolicy.None:
                    default:
                        break;
                    case TruncatePolicy.Simple:
                        collection = collection.Skip(collection.Count() - targetLength).ToList();
                        break;
                    case TruncatePolicy.Exception:
                        throw new ArgumentException("The collection exceeds the target length: " + collection.Count() + "/" + targetLength);
                }
            }
            else
            {
                var list = collection.ToList();
                while (list.Count < targetLength)
                {
                    list.Insert(0, padWith);
                }
                collection = list;
            }
            return collection;
        }

        /// <summary>
        /// Grows a collection to the desired length by adding elements at the end
        /// </summary>
        public static IEnumerable<T> PadEnd<T>(IEnumerable<T> collection, T padWith, int targetLength, TruncatePolicy truncPolicy = default)
        {
            if (targetLength <= 0) throw new ArgumentException("The target length must be greater than 0");
            if (collection.Count() == targetLength) return collection;
            if (collection.Count() > targetLength)
            {
                switch (truncPolicy)
                {
                    case TruncatePolicy.None:
                    default:
                        break;
                    case TruncatePolicy.Simple:
                        collection = collection.Take(targetLength).ToList();
                        break;
                    case TruncatePolicy.Exception:
                        throw new ArgumentException("The collection exceeds the target length: " + collection.Count() + "/" + targetLength);
                }
            }
            else
            {
                var list = collection.ToList();
                while (list.Count < targetLength)
                {
                    list.Add(padWith);
                }
                collection = list;
            }
            return collection;
        }

        /// <summary>
        /// Removes elements from an array returning a second array with the removed elements
        /// </summary>
        public static T[] Scoop<T>(ref T[] array, int startIndex, int length = -1)
        {
            if (length == -1)
            {
                length = array.Length - startIndex;
            }
            var newArray = array
                    .Skip(startIndex)
                    .Take(length)
                    .ToArray();
            array = array
                    .Take(startIndex)
                    .Concat(
                        array
                            .Skip(startIndex + length)
                            .Take(array.Length)
                    )
                    .ToArray();
            return newArray;
        }

        /// <summary>
        /// Removes elements from the end of an array returning a second array with the removed elements
        /// </summary>
        public static T[] ScoopOffTheEnd<T>(ref T[] array, int length)
        {
            return Scoop(ref array, array.Length - length);
        }

        /// <summary>
        /// Trims all the strings in a collection.
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static IEnumerable<string> Trim(IEnumerable<string> collection)
        {
            if (collection == null) return null;
            collection = collection
                .Select(str => str?.Trim())
                .ToList();
            return collection;
        }

        /// <summary>
        /// Zaps all the strings in a collection.
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static IEnumerable<string> Zap(IEnumerable<string> collection, TextCleanMode textCleanMode = default, object customTextCleanDictionary = null, char[] charsToRemove = null)
        {
            if (collection == null) return null;
            collection = collection
                .Select(str => str?.Zap(textCleanMode: textCleanMode, customTextCleanDictionary: customTextCleanDictionary, charsToRemove: charsToRemove))
                .ToList();
            return collection;
        }

        /// <summary>
        /// Zaps all the strings in a collection and removes null results.
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static IEnumerable<string> ZapAndPrune(IEnumerable<string> collection, TextCleanMode textCleanMode = default, object customTextCleanDictionary = null, char[] charsToRemove = null, PrunePolicy prunePolicy = default)
        {
            var list = Zap(collection, textCleanMode: textCleanMode, customTextCleanDictionary: customTextCleanDictionary, charsToRemove: charsToRemove)
                ?.ToList();

            if (list == null || !list.Any()) return list;

            switch (prunePolicy)
            {
                case PrunePolicy.All:
                    list = list
                        .Where(s => s != null)
                        .ToList();
                    break;
                case PrunePolicy.Leading:
                    while (true)
                    {
                        if (!list.Any()) return list;
                        if (list[0] == null)
                        {
                            list.RemoveAt(0);
                        }
                        else
                        {
                            break;
                        }
                    }
                    break;
                case PrunePolicy.Trailing:
                    while (true)
                    {
                        if (!list.Any()) return list;
                        if (list[list.Count - 1] == null)
                        {
                            list.RemoveAt(list.Count - 1);
                        }
                        else
                        {
                            break;
                        }
                    }
                    break;
                case PrunePolicy.LeadingAndTrailing:
                    while (true)
                    {
                        if (!list.Any()) return list;
                        if (list[0] == null)
                        {
                            list.RemoveAt(0);
                        }
                        else
                        {
                            break;
                        }
                    }
                    while (true)
                    {
                        if (!list.Any()) return list;
                        if (list[list.Count - 1] == null)
                        {
                            list.RemoveAt(list.Count - 1);
                        }
                        else
                        {
                            break;
                        }
                    }
                    break;
            }
            return list;
        }

        /// <summary>
        /// Removes and returns a value from a dictionary, like Array.pop() in JavaScript
        /// </summary>
        public static TValue Extract<TKey, TValue>(IDictionary<TKey, TValue> dictionary, TKey key)
        {
            if (dictionary.ContainsKey(key))
            {
                var value = dictionary[key];
                dictionary.Remove(key);
                return value;
            }
            return default;
        }

        public static bool StartsWith<E>(IEnumerable<E> controlCollection, IEnumerable<E> compareCollection, bool ignoreCase = false)
        {
            if (controlCollection == null || compareCollection == null)
            {
                return false;
            }

            var controlCount = controlCollection.Count();
            var compareCount = compareCollection.Count();

            if (controlCount < compareCount)
            {
                return false;
            }

            if (controlCount > compareCount)
            {
                return Identical(controlCollection.Take(compareCount), compareCollection, ignoreCase: ignoreCase);
            }

            return Identical(controlCollection, compareCollection, ignoreCase: ignoreCase);
        }

        public static bool ContainsAll<E>(IEnumerable<E> controlCollection, IEnumerable<E> compareCollection, bool ignoreCase = false) where E : IComparable<E>
        {
            if (controlCollection == null || compareCollection == null)
            {
                return false;
            }

            var distinctControlCollection = controlCollection.Distinct().ToList();
            var distinctCompareCollection = compareCollection.Distinct().ToList();

            if (distinctCompareCollection.Count > distinctControlCollection.Count)
            {
                return false;
            }

            if (typeof(E) == typeof(string) && ignoreCase)
            {
                var ucaseControlCollection = distinctControlCollection.Select(item => TextUtil.Zap(item)?.ToUpper()).ToList();
                var ucaseCompareCollection = distinctCompareCollection.Select(item => TextUtil.Zap(item)?.ToUpper()).ToList();
                return !ucaseCompareCollection.Any(item => !ucaseControlCollection.Contains(item));
            }
            return !distinctCompareCollection.Any(item => !distinctControlCollection.Contains(item));
        }

        public static bool Identical<E>(IEnumerable<E> controlCollection, IEnumerable<E> compareCollection, bool ignoreCase = false)
        {
            if (controlCollection == null || compareCollection == null || compareCollection.Count() != controlCollection.Count())
            {
                return false;
            }

            if (typeof(E) == typeof(string) && ignoreCase)
            {
                var ucaseControlCollection = controlCollection.Select(item => TextUtil.Zap(item)?.ToUpper()).ToList();
                var ucaseCompareCollection = compareCollection.Select(item => TextUtil.Zap(item)?.ToUpper()).ToList();
                for (int i = 0; i < ucaseControlCollection.Count; i++)
                {
                    if (!string.Equals(ucaseControlCollection[i], ucaseCompareCollection[i]))
                    {
                        return false;
                    }
                }
                return true;
            }

            for (int i = 0, count = controlCollection.Count(); i < count; i++)
            {
                if (!Equals(controlCollection.ElementAt(i), compareCollection.ElementAt(i)))
                {
                    return false;
                }
            }
            return true;
        }

        public static IDictionary<TKey, TValue> Merge<TKey, TValue>(IDictionary<TKey, TValue> dictionary, IDictionary<TKey, TValue> dictionaryToMerge, bool overwrite = true)
        {
            dictionary = new Dictionary<TKey, TValue>(dictionary);
            foreach (var kvp in dictionaryToMerge)
            {
                if (dictionary.ContainsKey(kvp.Key))
                {
                    if (overwrite)
                    {
                        dictionary[kvp.Key] = kvp.Value;
                    }
                }
                else
                {
                    dictionary.Add(kvp);
                }
            }
            return dictionary;
        }

        public static IDictionary<TKey, TValue> Merge<TKey, TValue>(IDictionary<TKey, TValue> dictionary, IDictionary<TKey, TValue> dictionaryToMerge, Func<TValue, TValue, TValue> mergeValuesFunc)
        {
            dictionary = new Dictionary<TKey, TValue>(dictionary);
            foreach (var kvp in dictionaryToMerge)
            {
                if (dictionary.ContainsKey(kvp.Key))
                {
                    dictionary[kvp.Key] = mergeValuesFunc(dictionary[kvp.Key], kvp.Value);
                }
                else
                {
                    dictionary.Add(kvp);
                }
            }
            return dictionary;
        }

        public static IEnumerable<T> ConcatIf<T>(IEnumerable<T> collection, IEnumerable<T> collectionToAppend)
        {
            if (collection == null) return collectionToAppend;
            if (collectionToAppend == null) return collection;
            return collection
                .Concat(collectionToAppend);
        }
    }
}
