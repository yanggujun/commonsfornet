// Copyright CommonsForNET 2014.
// Licensed to the Apache Software Foundation (ASF) under one or more
// contributor license agreements. See the NOTICE file distributed with
// this work for additional information regarding copyright ownership.
// The ASF licenses this file to You under the Apache License, Version 2.0
// (the "License"); you may not use this file except in compliance with
// the License. You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections;
using System.Collections.Generic;
using Commons.Collections.Set;
using Commons.Utils;

namespace Commons.Collections.Map
{
    [CLSCompliant(true)]
    public abstract class AbstractBimap<K, V> : IBimap<K, V>, IDictionary<K, V>, ICollection<KeyValuePair<K, V>>, IDictionary, 
        ICollection, IReadOnlyBimap<K, V>, IReadOnlyDictionary<K, V>, IReadOnlyCollection<KeyValuePair<K, V>>, IEnumerable<KeyValuePair<K, V>>, IEnumerable
    {
        protected IDictionary<K, V> KeyValue { get; private set; }
        protected IDictionary<V, K> ValueKey { get; private set; }

        protected AbstractBimap(IDictionary<K, V> keyValue, IDictionary<V, K> valueKey)
        {
            KeyValue = keyValue;
            ValueKey = valueKey;
        }

        public void Add(K key, V value)
        {
            key.ValidateNotNull("The key is null!");
            value.ValidateNotNull("The value is null!");
            if (KeyValue.ContainsKey(key))
            {
                throw new ArgumentException("The key already exists in the bimap.");
            }
            if (ValueKey.ContainsKey(value))
            {
                throw new ArgumentException("The value already exists in the bimap.");
            }
            KeyValue.Add(key, value);
            ValueKey.Add(value, key);
        }

        public void Enforce(K key, V value)
        {
            key.ValidateNotNull("The key is null!");
            value.ValidateNotNull("The value is null!");
			if (ContainsKey(key))
			{
				RemoveKey(key);
			}

            if (ContainsValue(value))
            {
				RemoveValue(value);
            }
			Add(key, value);
        }

        public bool RemoveKey(K key)
        {
            key.ValidateNotNull("The key is null!");
            var removed = false;
            if (KeyValue.ContainsKey(key))
            {
                var v = KeyValue[key];
                if (ValueKey.ContainsKey(v))
                {
                    KeyValue.Remove(key);
                    ValueKey.Remove(v);
                    removed = true;
                }
            }

            return removed;
        }

        public bool RemoveValue(V value)
        {
            value.ValidateNotNull("The value is null!");
            var removed = false;
            if (ValueKey.ContainsKey(value))
            {
                var k = ValueKey[value];
                if (KeyValue.ContainsKey(k))
                {
                    ValueKey.Remove(value);
                    KeyValue.Remove(k);
                    removed = true;
                }
            }

            return removed;
        }

        public bool ContainsKey(K key)
        {
            key.ValidateNotNull("The key is null!");
            return KeyValue.ContainsKey(key);
        }

        public bool ContainsValue(V value)
        {
            value.ValidateNotNull("The value is null!");
            return ValueKey.ContainsKey(value);
        }

        public bool TryGetValue(K key, out V value)
        {
            return KeyValue.TryGetValue(key, out value);
        }

        public bool TryGetKey(V value, out K key)
        {
            return ValueKey.TryGetValue(value, out key);
        }

        public V ValueOf(K key)
        {
            var v = default(V);
            if (KeyValue.ContainsKey(key))
            {
                v = KeyValue[key];
            }
            else
            {
                throw new KeyNotFoundException("The key is not found in the bimap.");
            }

            return v;
        }

        public K KeyOf(V value)
        {
            var k = default(K);
            if (ValueKey.ContainsKey(value))
            {
                k = ValueKey[value];
            }
            else
            {
                throw new KeyNotFoundException("The value is not found in the bimap.");
            }

            return k;
        }

        public abstract IBimap<V, K> Inverse();

        public abstract IStrictSet<K> KeySet();

        public abstract IStrictSet<V> ValueSet();

        public void Add(KeyValuePair<K, V> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            KeyValue.Clear();
            ValueKey.Clear();
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		/// <remarks>The method returns true only when key and value of the <paramref name="item"/> exist as a pair in the bimap.</remarks>
		public abstract bool Contains(KeyValuePair<K, V> item);

        public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
        {
            KeyValue.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return KeyValue.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(KeyValuePair<K, V> item)
        {
            var removed = false;
            if (Contains(item))
            {
                removed = RemoveKey(item.Key);
            }

            return removed;
        }

        public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
        {
            return KeyValue.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        void ICollection.CopyTo(Array array, int index)
        {
            array.ValidateNotNull("The input array is null!");
            var itemArray = array as KeyValuePair<K, V>[];
            CopyTo(itemArray, index);
        }

        bool ICollection.IsSynchronized
        {
            get { return false; }
        }

        object ICollection.SyncRoot
        {
            get { throw new NotSupportedException("The SyncRoot is not supported in the Commons.Collections."); }
        }

        IEnumerable<K> IReadOnlyDictionary<K,V>.Keys
        {
            get
            {
                return Keys;
            }
        }

        IEnumerable<V> IReadOnlyDictionary<K, V>.Values
        {
            get
            {
                return Values;
            }
        }

        public V this[K key]
        {
            get { return ValueOf(key); }
            set
            {
				key.ValidateNotNull("The key is null!");
                if (!KeyValue.ContainsKey(key))
                {
                    throw new KeyNotFoundException("The key is not found in the bimap.");
                }
				Enforce(key, value);
            }
        }

        public ICollection<K> Keys
        {
            get { return KeySet(); }
        }

        public bool Remove(K key)
        {
            return RemoveKey(key);
        }

        public ICollection<V> Values
        {
            get { return ValueSet(); }
        }

        void IDictionary.Add(object key, object value)
        {
            key.ValidateNotNull("The key is null!");
            value.ValidateNotNull("The value is null!");
            Add((K)key, (V)value);
        }

        bool IDictionary.Contains(object key)
        {
            key.ValidateNotNull("The key is null!");
            return KeyValue.ContainsKey((K)key);
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return new MapEnumerator<K, V>(this);
        }

        bool IDictionary.IsFixedSize
        {
            get { return false; }
        }

        ICollection IDictionary.Keys
        {
            get { return new List<K>(KeySet()); }
        }

        void IDictionary.Remove(object key)
        {
            key.ValidateNotNull("The key is null!");
            Remove((K)key);
        }

        ICollection IDictionary.Values
        {
            get { return new List<V>(ValueSet()); }
        }

        object IDictionary.this[object key]
        {
            get
            {
                key.ValidateNotNull("The key is null!");
                return this[(K)key];
            }
            set
            {
                key.ValidateNotNull("The key is null!");
                this[(K)key] = (V)value;
            }
        }
    }
}
