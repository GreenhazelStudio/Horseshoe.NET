using System;
using System.Collections.Generic;
using System.Linq;

using Horseshoe.NET.Text;

namespace Horseshoe.NET.Collections
{
    public static class CollectionUtil
    {
        /// <summary>
        /// Grows a collection to the desired size by adding elements at the specified position (default position is end)
        /// </summary>
        public static void Pad<T>(IList<T> list, int targetSize, CollectionPosition position = CollectionPosition.End, T padWith = default, bool cannotExceedTargetSize = false)
        {
            if (list == null) return;
            if (list.Count > targetSize)
            {
                if (cannotExceedTargetSize) throw new ValidationException("Collection (" + list.Count + ") cannot exceed target size (" + targetSize + ")");
                return;
            }
            switch (position)
            {
                case CollectionPosition.End:
                default:
                    while (list.Count < targetSize)
                    {
                        list.Add(padWith);
                    }
                    break;
                case CollectionPosition.Start:
                    while (list.Count < targetSize)
                    {
                        list.Insert(0, padWith);
                    }
                    break;
            }
        }

        /// <summary>
        /// Returns a duplicate collection of the desired size by adding elements at the specified position (default position is end)
        /// </summary>
        public static IEnumerable<T> Pad<T>(IEnumerable<T> collection, int targetSize, CollectionPosition position = CollectionPosition.End, T padWith = default, bool cannotExceedTargetSize = false)
        {
            if (collection == null) return null;
            var list = collection.ToList();
            Pad(list, targetSize, position: position, padWith: padWith, cannotExceedTargetSize: cannotExceedTargetSize);
            return list;
        }

        /// <summary>
        /// Returns a duplicate collection of the desired size by adding elements at the specified position (default position is end)
        /// </summary>
        public static T[] Pad<T>(T[] array, int targetSize, CollectionPosition position = CollectionPosition.End, T padWith = default, bool cannotExceedTargetSize = false)
        {
            if (array == null) return null;
            var list = array.ToList();
            Pad(list, targetSize, position: position, padWith: padWith, cannotExceedTargetSize: cannotExceedTargetSize);
            return list.ToArray();
        }

        /// <summary>
        /// Shrinks a collection to the desired size by removing elements from the specified position (default position is end)
        /// </summary>
        public static void Crop<T>(IList<T> list, int targetSize, CollectionPosition position = CollectionPosition.End)
        {
            if (list == null) return;
            if (list.Count <= targetSize) return;
            switch (position)
            {
                case CollectionPosition.End:
                default:
                    while (list.Count < targetSize)
                    {
                        list.RemoveAt(list.Count - 1);
                    }
                    break;
                case CollectionPosition.Start:
                    while (list.Count < targetSize)
                    {
                        list.RemoveAt(0);
                    }
                    break;
            }
        }

        /// <summary>
        /// Returns a duplicate collection of the desired size by removing elements from the specified position (default position is end)
        /// </summary>
        public static IEnumerable<T> Crop<T>(IEnumerable<T> collection, int targetSize, CollectionPosition position = CollectionPosition.End)
        {
            if (collection == null) return null;
            var list = collection.ToList();
            Crop(list, targetSize, position: position);
            return list;
        }

        /// <summary>
        /// Returns a duplicate collection of the desired size by removing elements from the specified position (default position is end)
        /// </summary>
        public static T[] Crop<T>(T[] array, int targetSize, CollectionPosition position = CollectionPosition.End)
        {
            if (array == null) return null;
            var list = array.ToList();
            Crop(list, targetSize, position: position);
            return list.ToArray();
        }

        /// <summary>
        /// Grows or shrimks a collection to the desired size by adding / removing elements from the specified position (default position is end)
        /// </summary>
        public static void Fit<T>(IList<T> list, int targetSize, CollectionPosition position = CollectionPosition.Start, T padWith = default)
        {
            if (list == null) return;
            if (list.Count == targetSize) return;
            if (list.Count < targetSize) Pad(list, targetSize, position: FitSwitchCollectionPosition(position), padWith: padWith);
            else Crop(list, targetSize, position: FitSwitchCollectionPosition(position));
        }

        /// <summary>
        /// Returns a duplicate collection of the desired size by adding / removing elements from the specified position (default position is end)
        /// </summary>
        public static IEnumerable<T> Fit<T>(IEnumerable<T> collection, int targetSize, CollectionPosition position = CollectionPosition.Start, T padWith = default)
        {
            if (collection == null) return null;
            var list = collection.ToList();
            if (list.Count < targetSize) Pad(list, targetSize, position: FitSwitchCollectionPosition(position), padWith: padWith);
            else if (list.Count > targetSize) Crop(list, targetSize, position: FitSwitchCollectionPosition(position));
            return list;
        }

        /// <summary>
        /// Returns a duplicate collection of the desired size by adding / removing elements from the specified position (default position is end)
        /// </summary>
        public static T[] Fit<T>(T[] collection, int targetSize, CollectionPosition position = CollectionPosition.Start, T padWith = default)
        {
            if (collection == null) return null;
            var list = collection.ToList();
            if (list.Count < targetSize) Pad(list, targetSize, position: FitSwitchCollectionPosition(position), padWith: padWith);
            else if (list.Count > targetSize) Crop(list, targetSize, position: FitSwitchCollectionPosition(position));
            return list.ToArray();
        }

        static CollectionPosition FitSwitchCollectionPosition(CollectionPosition position)
        {
            switch (position)
            {
                case CollectionPosition.Start:
                    return CollectionPosition.End;
                case CollectionPosition.End:
                default:
                    return CollectionPosition.Start;
            }
        }

        /// <summary>
        /// Returns a collection of the desired size containing a single repeated value
        /// </summary>
        public static T[] Fill<T>(int targetSize, T fillWith = default)
        {
            var list = new List<T>();
            for (int i = 0; i < targetSize; i++)
            {
                list.Add(fillWith);
            }
            return list.ToArray();
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
        /// Condititionally appends two collections
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="collectionToAppend"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static IEnumerable<T> Concat<T>(IEnumerable<T> collection, IEnumerable<T> collectionToAppend)
        {
            if (collection == null) return collectionToAppend;
            if (collectionToAppend == null) return collection;
            return collection.Concat(collectionToAppend);
        }

        /// <summary>
        /// Condititionally appends two collections
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="collectionToAppend"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static IEnumerable<T> ConcatIf<T>(IEnumerable<T> collection, IEnumerable<T> collectionToAppend, bool condition)
        {
            if (condition) return Concat(collection, collectionToAppend);
            return collection;
        }

        /// <summary>
        /// Condititionally appends two collections
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="collectionToAppend"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static IEnumerable<T> ConcatIf<T>(IEnumerable<T> collection, IEnumerable<T> collectionToAppend, Func<bool> condition)
        {
            if (condition.Invoke()) return Concat(collection, collectionToAppend);
            return collection;
        }

        /// <summary>
        /// Trims all the strings in a collection and returns a new collection
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
        /// Zaps all the strings in a collection and returns a new collections
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static IEnumerable<string> Zap(IEnumerable<string> collection, TextCleanMode textCleanMode = default, object customTextCleanDictionary = null, char[] charsToRemove = null)
        {
            if (collection == null) return null;
            collection = collection
                .Select(str => Objects.Clean.Zap.String(str, textCleanMode: textCleanMode, customTextCleanDictionary: customTextCleanDictionary, charsToRemove: charsToRemove))
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
                var ucaseControlCollection = distinctControlCollection.Select(item => Objects.Clean.Zap.String(item)?.ToUpper()).ToList();
                var ucaseCompareCollection = distinctCompareCollection.Select(item => Objects.Clean.Zap.String(item)?.ToUpper()).ToList();
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
                var ucaseControlCollection = controlCollection.Select(item => Objects.Clean.Zap.String(item)?.ToUpper()).ToList();
                var ucaseCompareCollection = compareCollection.Select(item => Objects.Clean.Zap.String(item)?.ToUpper()).ToList();
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
    }
}
