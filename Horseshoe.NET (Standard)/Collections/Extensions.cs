using System;
using System.Collections.Generic;
using System.Linq;

using Horseshoe.NET.Text;

namespace Horseshoe.NET.Collections
{
    public static class Extensions
    {
        public static void AddOrReplace<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (dictionary.ContainsKey(key))
            {
                dictionary[key] = value;
            }
            else
            {
                dictionary.Add(key, value);
            }
        }

        public static void AddIfUnique<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (!dictionary.ContainsKey(key))
            {
                dictionary.Add(key, value);
            }
        }

        public static ImmutableDictionary<TKey, TValue> AsImmutable<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
        {
            if (dictionary is ImmutableDictionary<TKey, TValue> immutableDictionary) return immutableDictionary;
            return new ImmutableDictionary<TKey, TValue>(dictionary);
        }

        public static IDictionary<TKey, TValue> Merge<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, IDictionary<TKey, TValue> dictionaryToMerge, bool overwrite = true)
        {
            return CollectionUtil.Merge(dictionary, dictionaryToMerge, overwrite: overwrite);
        }

        public static IDictionary<TKey, TValue> Merge<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, IDictionary<TKey, TValue> dictionaryToMerge, Func<TValue, TValue, TValue> mergeValuesFunc)
        {
            return CollectionUtil.Merge(dictionary, dictionaryToMerge, mergeValuesFunc: mergeValuesFunc);
        }

        public static IEnumerable<T> ConcatIf<T>(this IEnumerable<T> collection, IEnumerable<T> collectionToAppend)
        {
            return CollectionUtil.ConcatIf(collection, collectionToAppend);
        }

        public static IEnumerable<T> PadLeft<T>(this IEnumerable<T> collection, T padWith, int targetLength, TruncatePolicy truncPolicy = default)
        {
            return CollectionUtil.PadStart(collection, padWith, targetLength, truncPolicy: truncPolicy);
        }

        public static IEnumerable<T> PadRight<T>(this IEnumerable<T> collection, T padWith, int targetLength, TruncatePolicy truncPolicy = default)
        {
            return CollectionUtil.PadEnd(collection, padWith, targetLength, truncPolicy: truncPolicy);
        }

        public static string StringDump<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, string equals = " = ", string separator = ", ")
        {
            return string.Join(separator, dictionary.Select(kvp => kvp.Key + equals + kvp.Value));
        }

        public static IEnumerable<string> Trim(this IEnumerable<string> collection)
        {
            return CollectionUtil.Trim(collection);
        }

        public static IEnumerable<string> Zap(this IEnumerable<string> collection, TextCleanMode textCleanMode = default, object customTextCleanDictionary = null, char[] charsToRemove = null)
        {
            return CollectionUtil.Zap(collection, textCleanMode: textCleanMode, customTextCleanDictionary: customTextCleanDictionary, charsToRemove: charsToRemove);
        }

        public static IEnumerable<string> ZapAndPrune(this IEnumerable<string> collection, TextCleanMode textCleanMode = default, object customTextCleanDictionary = null, char[] charsToRemove = null, PrunePolicy prunePolicy = default)
        {
            return CollectionUtil.ZapAndPrune(collection, textCleanMode: textCleanMode, customTextCleanDictionary: customTextCleanDictionary, charsToRemove: charsToRemove, prunePolicy: prunePolicy);
        }

        public static bool StartsWith<E>(this IEnumerable<E> controlCollection, IEnumerable<E> partialCollection)
        {
            return CollectionUtil.StartsWith(controlCollection, partialCollection);
        }

        public static bool StartsWith(this IEnumerable<string> controlCollection, IEnumerable<string> partialCollection, bool ignoreCase = false)
        {
            return CollectionUtil.StartsWith(controlCollection, partialCollection, ignoreCase);
        }

        public static bool Identical<E>(this IEnumerable<E> controlCollection, IEnumerable<E> partialCollection)
        {
            return CollectionUtil.Identical(controlCollection, partialCollection);
        }

        public static bool Identical(this IEnumerable<string> controlCollection, IEnumerable<string> partialCollection, bool ignoreCase = false)
        {
            return CollectionUtil.Identical(controlCollection, partialCollection, ignoreCase);
        }

        public static void Iterate<E>(this IEnumerable<E> collection, Action<E> action)
        {
            foreach (E e in collection)
            {
                try
                {
                    action.Invoke(e);
                }
                catch (IterationException iex)
                {
                    if (iex.Continue)
                    {
                        continue;
                    }
                    if (iex.Break)
                    {
                        break;
                    }
                }
            }
        }

        public static void Iterate<E>(this IEnumerable<E> collection, Action<E, int> action)
        {
            var counter = 0;
            foreach (E e in collection)
            {
                try
                {
                    action.Invoke(e, counter++);
                }
                catch (IterationException iex)
                {
                    if (iex.Continue)
                    {
                        continue;
                    }
                    if (iex.Break)
                    {
                        break;
                    }
                }
            }
        }

        public static void Replace<T>(this List<T> list, T item, T replacement) where T : IEquatable<T>
        {
            if (list == null) return;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] == null)
                {
                    if (item == null)
                    {
                        list[i] = replacement;
                    }
                }
                else if (EqualityComparer<T>.Default.Equals(list[i], item))
                {
                    list[i] = replacement;
                }
            }
        }

        public static void Replace<T>(this List<T> list, T item, T[] replacements) where T : IEquatable<T>
        {
            if (list == null) return;
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (list[i] == null)
                {
                    if (item == null)
                    {
                        switch (replacements.Length)
                        {
                            case 0:
                                list.RemoveAt(i);
                                break;
                            case 1:
                                list[i] = replacements[0];
                                break;
                            default:
                                list.RemoveAt(i);
                                list.InsertRange(i, replacements);
                                break;
                        }
                    }
                }
                else if (EqualityComparer<T>.Default.Equals(list[i], item))
                {
                    switch (replacements.Length)
                    {
                        case 0:
                            list.RemoveAt(i);
                            break;
                        case 1:
                            list[i] = replacements[0];
                            break;
                        default:
                            list.RemoveAt(i);
                            list.InsertRange(i, replacements);
                            break;
                    }
                }
            }
        }
    }
}
