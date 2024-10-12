using KhumaloCraft.Helpers;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace KhumaloCraft;

public static class CollectionExtensions
{
    public static bool IsEqualTo<T>(this T[] left, T[] right, bool ignoreSequence = false, EqualityComparer<T> customComparer = null)
    {
        if (ReferenceEquals(left, right))
            return true;

        if (left == null && right == null)
            return true;

        if (left == null)
            return right.IsNullOrEmpty();

        if (right == null)
            return left.IsNullOrEmpty();

        if (left.Length != right.Length)
            return false;

        if (ignoreSequence)
        {
            for (var i = 0; i < left.Length; i++)
            {
                if (!right.Contains(left[i]))
                {
                    return false;
                }
            }

            // is this even necessary, since the lengths are guaranteed to be the same by a previous check?
            for (var i = 0; i < right.Length; i++)
            {
                if (!left.Contains(right[i]))
                {
                    return false;
                }
            }
        }
        else
        {
            var comparer = customComparer ?? EqualityComparer<T>.Default;
            for (var i = 0; i < left.Length; i++)
            {
                if (!comparer.Equals(left[i], right[i]))
                {
                    return false;
                }
            }
        }

        return true;
    }

    public static T[] Clone<T>(this T[] array)
        where T : struct
    {
        if (array == null)
            return null;

        return (T[])array.Clone();
    }
}

public static class DictionaryExtensions
{
    /// <summary>
    /// Adds the key/value pair if the key doesn't already exist, or uses the supplied function to update the key's current value with the supplied value.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="this"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="updater"></param>
    /// <returns>TRUE if the key already existed in the dictionary and its value was updated, FALSE if it didn't exist and was added.</returns>
    public static bool AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> @this, TKey key, TValue value, Func<TValue, TValue, TValue> updater)
    {
        if (!@this.TryGetValue(key, out TValue currentValue))
        {
            @this.Add(key, value);

            return false;
        }

        if (updater == null)
        {
            throw new ArgumentNullException(nameof(updater));
        }

        @this[key] = updater(currentValue, value);

        return true;
    }

    /// <summary>
    /// Complements <c>List&lt;T>.AsReadOnly()</c>.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="this"></param>
    /// <returns></returns>
    public static IReadOnlyDictionary<TKey, TValue> AsReadOnly<TKey, TValue>(this Dictionary<TKey, TValue> @this)
    {
        return new ReadOnlyDictionary<TKey, TValue>(@this);
    }

    public static TValue TryGetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> @this, TKey key)
    {
        return @this.TryGetValue(key, out var value)
            ? value
            : default;
    }

    // https://github.com/dotnet/corefx/issues/1942
    public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> @this, TKey key, Func<TValue> valueFactory)
    {
        if (@this.TryGetValue(key, out TValue value))
        {
            return value;
        }

        if (valueFactory == null)
        {
            throw new ArgumentNullException(nameof(valueFactory));
        }

        value = valueFactory();

        @this.Add(key, value);

        return value;
    }

    /// <summary>
    /// Inverts, or "flips", a dictionary so that its keys become its values and vice versa.
    /// Note that this will  throw an exception if the values in the supplied dictionary are not unique.
    /// </summary>
    public static IDictionary<TValue, TKey> Invert<TKey, TValue>(this IDictionary<TKey, TValue> @this)
    {
        return @this.ToDictionary(k => k.Value, v => v.Key);
    }

    /// <summary>
    /// Inverts, or "flips", a dictionary so that its keys become its values and vice versa.
    /// Note that this will  throw an exception if the values in the supplied dictionary are not unique.
    /// </summary>
    public static Dictionary<TValue, TKey> Invert<TKey, TValue>(this Dictionary<TKey, TValue> @this)
    {
        return @this.ToDictionary(k => k.Value, v => v.Key);
    }

    /// <summary>
    /// Removes all keys that match the specified predicate function from the dictionary. Complements the <c>List&lt;T>.RemoveAll</c> method.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="dictionary"></param>
    /// <param name="predicate"></param>
    /// <returns>The number of items that were removed.</returns>
    public static int RemoveAll<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, Func<KeyValuePair<TKey, TValue>, bool> predicate)
    {
        if (predicate == null)
        {
            throw new ArgumentNullException(nameof(predicate));
        }

        // ToList is vital to prevent deferred execution and hence "collection was modified" exception
        var pairsToRemove = dictionary.Where(kvp => predicate(kvp)).ToList();

        foreach (var keyToRemove in pairsToRemove)
        {
            dictionary.Remove(keyToRemove.Key);
        }

        return pairsToRemove.Count;
    }

    // Source: http://stackoverflow.com/questions/1287567/is-using-random-and-orderby-a-good-shuffle-algorithm/1287572#1287572
    public static IList<T> Shuffle<T>(this IList<T> source, string seed = null)
    {
        Random random;

        Func<int, int> GetNext;
        if (seed != null)
        {
            random = new Random(seed.GetHashCode());
            GetNext = random.Next;
        }
        else
        {
            GetNext = RandomHelper.Next;
        }

        for (var i = source.Count - 1; i >= 0; i--)
        {
            // Swap element "i" with a random earlier element (or itself)
            var swapIndex = GetNext(i + 1);
            var tmp = source[i];
            source[i] = source[swapIndex];
            source[swapIndex] = tmp;
        }

        return source;
    }

    public static IDictionary<string, object> ToDictionary(this object @this, params object[] args)
    {
        if (@this == null)
        {
            throw new ArgumentNullException(nameof(@this));
        }

        if (args == null)
        {
            throw new ArgumentNullException(nameof(args));
        }

        var result = new Dictionary<string, object>();

        foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(@this))
        {
            result.Add(property.Name, property.GetValue(@this));
        }

        foreach (var arg in args)
        {
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(arg))
            {
                result[property.Name] = property.GetValue(arg);
            }
        }

        return result;
    }

    // https://github.com/dotnet/corefx/issues/1942
    /// <summary>
    /// Adds the specified value to the dictionary only if its key is not already present, and returns a value indicating whether the key existed before the operation.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="this"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns>TRUE if the key did not exist and the value was newly added, FALSE otherwise.</returns>
    /// <remarks>After execution of this method, the specified dictionary is guaranteed to contain the specified key.</remarks>
    public static bool TryAdd<TKey, TValue>(this IDictionary<TKey, TValue> @this, TKey key, TValue value)
    {
        if (@this.ContainsKey(key))
        {
            return false;
        }

        @this.Add(key, value);

        return true;
    }

    public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> @this, TKey key)
    {
        if (@this.TryGetValue(key, out TValue value))
        {
            return value;
        }

        return default;
    }

    public static void AddRange<T>(this ICollection<T> target, IEnumerable<T> source)
    {
        ArgumentNullException.ThrowIfNull(target);

        ArgumentNullException.ThrowIfNull(source);

        foreach (var element in source)
        {
            target.Add(element);
        }
    }
}

public static class EnumerableExtensions
{
    public static IEnumerable<T> Prepend<T>(this IEnumerable<T> first, params T[] second)
    {
        return second.Concat(first);
    }

    public static IEnumerable Prepend(this IEnumerable first, params object[] second)
    {
        return second.Concat(first.OfType<object>());
    }

    public static IEnumerable<T> Append<T>(this IEnumerable<T> first, params T[] second)
    {
        return first.Concat(second);
    }

    public static IEnumerable Append(this IEnumerable first, params object[] second)
    {
        return first.OfType<object>().Concat(second);
    }

    public static IEnumerable<T> Distinct<T, TKey>(this IEnumerable<T> source, Func<T, TKey> selector)
    {
        return source.GroupBy(selector).Select(x => x.First());
    }

    public static T? FirstOrNull<T>(this IEnumerable<T> source)
        where T : struct
    {
        return source.FirstOrNull(p => true);
    }

    public static T? FirstOrNull<T>(this IEnumerable<T> source, Func<T, bool> predicate = null)
        where T : struct
    {
        if (source == null)
        {
            return null;
        }

        var result = source.FirstOrDefault(predicate);

        return EqualityComparer<T>.Default.Equals(result, default) ? (T?)null : result;
    }

    /// <summary>
    /// Checks if the enumerable is not null and has atleast the number of elements specified by Count
    /// </summary>
    /// <param name="count">The Number of elements to check source for</param>
    public static bool HasMinCount<T>(this IEnumerable<T> source, int count)
    {
        if (source == null) return false;

        return source.Count() >= count;
    }

    public static bool IsNullOrEmpty<T>(this ICollection<T> collection)
    {
        return collection == null || collection.Count == 0;
    }

    public static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
    {
        return source == null || !source.Any();
    }

    // unused <T> is there to prevent method overload ambiguity
    public static bool IsNullOrEmpty<T>(this ICollection collection)
    {
        return collection == null || collection.Count == 0;
    }

    // http://stackoverflow.com/a/27191938/70345
    public static bool IsNullOrEmpty(this IEnumerable source)
    {
        return source == null || !source.GetEnumerator().MoveNext();
    }

    public static IEnumerable<T> Merge<T>(this IEnumerable<T> list1, IEnumerable<T> list2)
    {
        if (list1 == null && list2 == null)
        {
            return null;
        }

        if (list2 == null) return list1;
        if (list1 == null) return list2;

        if (list1 != null && list2 != null)
        {
            foreach (var item in list2.Where(item => !list1.Contains(item)))
            {
                list1.Append(item);
            }
        }

        return list1;
    }

    // http://stackoverflow.com/a/22323356/70345
    public static IEnumerable<T> OrderByAlphaNumeric<T>(this IEnumerable<T> source, Func<T, string> selector)
    {
        var numericRegex = new Regex(@"\d+", RegexOptions.Compiled);

        var maxDigits = source
                      .SelectMany(i => numericRegex.Matches(selector(i)).Cast<Match>().Select(digitChunk => (int?)digitChunk.Value.Length))
                      .Max() ?? 0;

        return source.OrderBy(i => numericRegex.Replace(selector(i), match => match.Value.PadLeft(maxDigits, '0')), StringComparer.CurrentCulture);
    }

    // http://www.velir.com/blog/2011/02/17/ilist-sorting-better-way
    public static IOrderedEnumerable<T> OrderBy<T>(this IEnumerable<T> source, Comparison<T> comparison)
    {
        return source.OrderBy(t => t, Comparer<T>.Create(comparison));
    }

    public static IOrderedEnumerable<T> OrderByDescending<T>(this IEnumerable<T> source, Comparison<T> comparison)
    {
        return source.OrderByDescending(t => t, Comparer<T>.Create(comparison));
    }

    public static IEnumerable<T> Page<T>(this IEnumerable<T> ienumerable, int pageSize, int pageNumber)
    {
        return ienumerable.Skip(PageHelper.CalculateSkip(pageSize, pageNumber)).Take(pageSize);
    }

    public static IEnumerable<IEnumerable<T>> Paginate<T>(this IEnumerable<T> @this, int itemsPerPage)
    {
        for (var skip = 0; skip < @this.Count(); skip += itemsPerPage)
        {
            yield return @this.Skip(skip).Take(itemsPerPage);
        }
    }

    public static IEnumerable<T> RateLimit<T>(this IEnumerable<T> ienumerable, int itemsPerSecond, TimeSpan delay)
    {
        var timer = Stopwatch.StartNew();
        var itemsCount = 0;

        foreach (var item in ienumerable)
        {
            while (timer.Elapsed.TotalSeconds * itemsPerSecond < itemsCount)
            {
                Thread.Sleep((int)Math.Ceiling(delay.TotalMilliseconds));
            }

            yield return item;

            itemsCount++;
        }
    }

    public static IEnumerable<T> RemoveAll<T>(this IEnumerable<T> value, Func<T, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);

        if (value == null)
        {
            return value;
        }

        return value.Where(x => !predicate(x));
    }

    public static IEnumerable<T> NotNull<T>(this IEnumerable<T?> items) where T : struct
    {
        return items
            .Where(item => item.HasValue)
            .Select(item => item.Value);
    }

    public static HashSet<TDestination> ConvertAllToHashSet<TSource, TDestination>(this IEnumerable<TSource> enumerable, Converter<TSource, TDestination> converter)
    {
        var hashSet = new HashSet<TDestination>();
        hashSet.AddRange(enumerable.Select(item => converter(item)));
        return hashSet;
    }

    public static IEnumerable<T> Fill<T>(this IEnumerable<T> source, int length, T defaultValue = default)
    {
        int i = 0;
        foreach (var item in source.Take(length))
        {
            yield return item;
            i++;
        }
        for (; i < length; i++)
            yield return defaultValue;
    }
}

public static class HashSetExtensions
{
    public static void AddRange<T>(this HashSet<T> hash, IEnumerable<T> range)
    {
        if (hash == null) return;

        if (range.IsNullOrEmpty())
        {
            return;
        }

        foreach (var item in range)
        {
            hash.Add(item);
        }
    }

    public static T[] SafeToArray<T>(this HashSet<T> hashSet)
    {
        if (hashSet == null)
            return null;

        return hashSet.ToArray();
    }
}

public static class ListExtensions
{
    public static IEnumerable<List<T>> BatchProcess<T>(this IEnumerable<T> ienumerable, int batchSize)
    {
        var result = new List<T>();

        foreach (var item in ienumerable)
        {
            result.Add(item);

            if (result.Count == batchSize)
            {
                yield return result;

                result = new List<T>();
            }
        }

        if (result.Count != 0)
        {
            yield return result;
        }
    }

    /// <summary>
    /// Partitions a list so that it is split up into columns as lists.
    /// If the elements cannot be exactly divided into the columns, the first few columns will have more elements.
    /// Fills the matrix a row at a time.
    /// </summary>
    public static List<List<T>> ToBalancedMatrixHorizontal<T>(this IEnumerable<T> ienumerable, int columns)
    {
        var result = new List<List<T>>(columns);

        for (var i = 0; i < columns; i++)
        {
            result.Add(new List<T>());
        }

        var index = 0;

        foreach (var item in ienumerable)
        {
            if (index == columns)
                index = 0;

            result[index].Add(item);
            index++;
        }

        return result;
    }

    /// <summary>
    /// Partitions a list so that it is split up into columns as lists.
    /// Fills the matrix a column at a time.
    /// </summary>
    public static List<List<T>> ToBalancedMatrixVertically<T>(this IEnumerable<T> ienumerable, int columns)
    {
        var count = ienumerable.Count();
        var result = new List<List<T>>(columns);
        var lengthPerColumn = (int)Math.Ceiling(count / (decimal)columns);

        for (var i = 0; i < columns; i++)
        {
            result.Add(new List<T>());
        }

        var index = 0;

        for (var i = 0; i < count; i++)
        {
            if (i == lengthPerColumn * (index + 1))
                index++;

            result[index].Add(ienumerable.ElementAt(i));
        }

        return result;
    }
}

public static class ArrayExtensions
{
    public static bool AreEqual(this Array left, Array right)
    {
        if (left?.Length != right?.Length)
        {
            return false;
        }

        var leftLength = left?.Length ?? 0;
        var rightLength = right?.Length ?? 0;

        if (leftLength != rightLength)
        {
            return false;
        }

        for (var i = 0; i < leftLength; i++)
        {
            if (!object.Equals(left.GetValue(i), right.GetValue(i)))
            {
                return false;
            }
        }

        return true;
    }

    public static int GetHashCode(this Array array)
    {
        if (array.IsNullOrEmpty())
        {
            return 0;
        }

        var hash = 0;

        foreach (var value in array)
        {
            if (value != null)
            {
                hash ^= value.GetHashCode();
            }
        }

        return hash;
    }

    public static bool ArrayIsNullOrEmpty(this Array array)
    {
        return (array == null || array.Length == 0);
    }
}

public static class ConcurrentBagExtensions
{
    public static void AddRange<T>(this ConcurrentBag<T> collection, IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            collection.Add(item);
        }
    }
}
