using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Commons.Collections.Collection;
using Commons.Collections.Map;
using Commons.Json;
using Commons.Utils;

namespace Commons.Messaging.Cache
{
    public class SimpleCache<K, T> : ICache<K, T>
    {
        private readonly ConcurrentDictionary<K, T> map;

        public SimpleCache(string name) : this(name, EqualityComparer<K>.Default)
        {
        }
        
        public SimpleCache(string name, IEqualityComparer<K> equator)
        {
            Name = name;
            map = new ConcurrentDictionary<K, T>(equator);
        }

        public SimpleCache(string name, Equator<K> equator) : this(name, new EquatorComparer<K>(equator))
        {
        }

        public bool IsEmpty
        {
            get
            {
                return map.Count == 0;
            }
        }

        public string Name
        {
            get; private set;
        }

        public void Add(K key, T val)
        {
            if (!map.ContainsKey(key))
            {
                map[key] = val;
            }
        }

        public void Clear()
        {
            map.Clear();
        }

        public bool Contains(K key)
        {
            return map.ContainsKey(key);
        }

        public T From(K key)
        {
            return map[key];
        }

        public IEnumerator<KeyValuePair<K, T>> GetEnumerator()
        {
            return map.GetEnumerator();
        }

        public void Remove(K key)
        {
            if (map.ContainsKey(key))
            {
                T val;
                map.TryRemove(key, out val);
            }
        }

        public byte[] ToBson()
        {
            throw new NotImplementedException();
        }

        public string ToJson()
        {
            return JsonMapper.ToJson(map);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
