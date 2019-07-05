
namespace Bench
{
  using System;
  using System.Collections.Generic;
  using System.Linq;

  public static partial class EnumerableUtils
  {
    /// <summary>
    /// convert an IEnumerable to HashSet
    /// </summary>
    /// <typeparam name="T">item type</typeparam>
    /// <param name="array">input array</param>
    /// <returns>output HashSet</returns>
    //public static HashSet<T> ToHashSet<T>(this IEnumerable<T> array)
    //{
    //  return new HashSet<T>(array ?? Enumerable.Empty<T>());
    //}

    /// <summary>
    /// convert an IEnumerable to a dictionary of bools.
    /// </summary>
    /// <typeparam name="T">item type</typeparam>
    /// <param name="array">input array</param>
    /// <returns>output Dictionary</returns>
    public static Dictionary<T, bool> ToHashDictionary<T>(this IEnumerable<T> array)
    {
      array = array ?? Enumerable.Empty<T>();
      Dictionary<T, bool> dict = new Dictionary<T, bool>((array.Count() * 2) + 10);
      foreach (T item in array)
      {
        dict[item] = true;
      }

      return dict;
    }

    /// <summary>
    /// Perform action on each of the items in the enumerable
    /// </summary>
    /// <typeparam name="T">item type</typeparam>
    /// <param name="array">input enumerable</param>
    public static void ForEach<T>(this IEnumerable<T> array, Action<T> action)
    {
      foreach (T item in array)
      {
        action(item);
      }
    }

    /// <summary>
    /// Make an Enumerable from a single item
    /// </summary>
    /// <typeparam name="T">item type</typeparam>
    /// <param name="args">input item</param>
    /// <returns>Enumerable from the item</returns>
    public static IEnumerable<T> ToEnumerable<T>(this T value)
    {
      yield return value;
    }

    /// <summary>
    /// Return a safe array
    /// </summary>
    /// <typeparam name="T">array element type</typeparam>
    /// <param name="array">original array</param>
    /// <returns>safe array</returns>
    public static T[] SafeArray<T>(this T[] array)
    {
      return array ?? Array<T>.Empty;
    }

    public static bool IsNullOrEmpty<T>(this T[] array)
    {
      return array == null || array.Length == 0;
    }

    /// <summary>
    /// Gets value from a dictionary and return default value if not existing.
    /// </summary>
    /// <typeparam name="K">key type</typeparam>
    /// <typeparam name="V">value type</typeparam>
    /// <param name="dict">dictionary object</param>
    /// <param name="key">key value</param>
    /// <returns>stored value if exist otherwise default value</returns>
    public static V SafeGet<K, V>(this IDictionary<K, V> dict, K key)
    {
      V val = default(V);
      dict.TryGetValue(key, out val);
      return val;
    }
  }
}
