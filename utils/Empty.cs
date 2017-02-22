namespace Bench
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    public class Empty<T>
    {
        public static readonly T[] Array = new T[0];

        public static readonly IList<T> List = new List<T>().AsReadOnly();
    }

    public class Empty<K, V>
    {
        public static readonly IDictionary<K, V> Dict = 
            new ReadOnlyDictionary<K,V>(new Dictionary<K,V>());
    }

}
