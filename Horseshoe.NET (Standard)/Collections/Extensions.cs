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

        public static T[] Concat<T>(this T[] array, IEnumerable<T> collectionToAppend)
        {
            if (collectionToAppend == null) return array;
            return (array as IEnumerable<T>).Concat(collectionToAppend).ToArray();
        }

        public static IEnumerable<T> ConcatIf<T>(this IEnumerable<T> collection, IEnumerable<T> collectionToAppend, bool condition)
        {
            return CollectionUtil.ConcatIf(collection, collectionToAppend, condition);
        }

        public static IEnumerable<T> ConcatIf<T>(this IEnumerable<T> collection, IEnumerable<T> collectionToAppend, Func<bool> condition)
        {
            return CollectionUtil.ConcatIf(collection, collectionToAppend, condition);
        }

        public static T[] ConcatIf<T>(this T[] array, IEnumerable<T> collectionToAppend, bool condition)
        {
            return CollectionUtil.ConcatIf(array, collectionToAppend, condition).ToArray();
        }

        public static T[] ConcatIf<T>(this T[] array, IEnumerable<T> collectionToAppend, Func<bool> condition)
        {
            return CollectionUtil.ConcatIf(array, collectionToAppend, condition).ToArray();
        }

        public static IEnumerable<T> Pad<T>(this IEnumerable<T> collection, int targetSize, CollectionPosition position = CollectionPosition.End, T padWith = default, bool cannotExceedTargetSize = false)
        {
            return CollectionUtil.Pad(collection, targetSize, position: position, padWith: padWith, cannotExceedTargetSize: cannotExceedTargetSize);
        }

        public static IEnumerable<T> PadStart<T>(this IEnumerable<T> collection, int targetSize, T padWith = default, bool cannotExceedTargetSize = false)
        {
            return CollectionUtil.Pad(collection, targetSize, position: CollectionPosition.Start, padWith: padWith, cannotExceedTargetSize: cannotExceedTargetSize);
        }

        public static IEnumerable<T> PadEnd<T>(this IEnumerable<T> collection, int targetSize, T padWith = default, bool cannotExceedTargetSize = false)
        {
            return CollectionUtil.Pad(collection, targetSize, position: CollectionPosition.End, padWith: padWith, cannotExceedTargetSize: cannotExceedTargetSize);
        }

        public static T[] Pad<T>(this T[] collection, int targetSize, CollectionPosition position = CollectionPosition.End, T padWith = default, bool cannotExceedTargetSize = false)
        {
            return CollectionUtil.Pad(collection, targetSize, position: position, padWith: padWith, cannotExceedTargetSize: cannotExceedTargetSize);
        }

        public static T[] PadStart<T>(this T[] collection, int targetSize, T padWith = default, bool cannotExceedTargetSize = false)
        {
            return CollectionUtil.Pad(collection, targetSize, position: CollectionPosition.Start, padWith: padWith, cannotExceedTargetSize: cannotExceedTargetSize);
        }

        public static T[] PadEnd<T>(this T[] collection, int targetSize, T padWith = default, bool cannotExceedTargetSize = false)
        {
            return CollectionUtil.Pad(collection, targetSize, position: CollectionPosition.End, padWith: padWith, cannotExceedTargetSize: cannotExceedTargetSize);
        }

        public static IEnumerable<T> Crop<T>(this IEnumerable<T> collection, int targetSize, CollectionPosition position = CollectionPosition.End)
        {
            return CollectionUtil.Crop(collection, targetSize, position: position);
        }

        public static IEnumerable<T> CropStart<T>(this IEnumerable<T> collection, int targetSize)
        {
            return CollectionUtil.Crop(collection, targetSize, position: CollectionPosition.Start);
        }

        public static IEnumerable<T> CropEnd<T>(this IEnumerable<T> collection, int targetSize)
        {
            return CollectionUtil.Crop(collection, targetSize, position: CollectionPosition.End);
        }

        public static T[] Crop<T>(this T[] collection, int targetSize, CollectionPosition position = CollectionPosition.End)
        {
            return CollectionUtil.Crop(collection, targetSize, position: position);
        }

        public static T[] CropStart<T>(this T[] collection, int targetSize)
        {
            return CollectionUtil.Crop(collection, targetSize, position: CollectionPosition.Start);
        }

        public static T[] CropEnd<T>(this T[] collection, int targetSize)
        {
            return CollectionUtil.Crop(collection, targetSize, position: CollectionPosition.End);
        }

        public static IEnumerable<T> Fit<T>(this IEnumerable<T> collection, int targetSize, CollectionPosition position = CollectionPosition.End, T padWith = default)
        {
            return CollectionUtil.Fit(collection, targetSize, position: position, padWith: padWith);
        }

        public static IEnumerable<T> FitStart<T>(this IEnumerable<T> collection, int targetSize, T padWith = default)
        {
            return CollectionUtil.Fit(collection, targetSize, position: CollectionPosition.Start, padWith: padWith);
        }

        public static IEnumerable<T> FitEnd<T>(this IEnumerable<T> collection, int targetSize, T padWith = default)
        {
            return CollectionUtil.Fit(collection, targetSize, position: CollectionPosition.End, padWith: padWith);
        }

        public static T[] Fit<T>(this T[] array, int targetSize, CollectionPosition position = CollectionPosition.End, T padWith = default)
        {
            return CollectionUtil.Fit(array, targetSize, position: position, padWith: padWith);
        }

        public static T[] FitStart<T>(this T[] array, int targetSize, T padWith = default)
        {
            return CollectionUtil.Fit(array, targetSize, position: CollectionPosition.Start, padWith: padWith);
        }

        public static T[] FitEnd<T>(this T[] array, int targetSize, T padWith = default)
        {
            return CollectionUtil.Fit(array, targetSize, position: CollectionPosition.End, padWith: padWith);
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
