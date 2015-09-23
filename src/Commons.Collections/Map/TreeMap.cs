// Copyright CommonsForNET.
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
    /// <summary>
    /// A tree map with the left leaning red black tree implementation. A tree map is a sorted dictionary.
    /// </summary>
    /// <typeparam name="K">The key type</typeparam>
    /// <typeparam name="V">The value type</typeparam>
    [CLSCompliant(true)]
    public sealed class TreeMap<K, V> : INavigableMap<K, V>, ISortedMap<K, V>, IDictionary<K, V>, ICollection<KeyValuePair<K, V>>, IDictionary, ICollection,
#if NET45
        IReadOnlyDictionary<K, V>, IReadOnlyCollection<KeyValuePair<K, V>>, 
#endif
        IEnumerable<KeyValuePair<K, V>>, IEnumerable
    {
        private readonly LlrbTree<K, V> llrbTree;

        /// <summary>
        /// Constructs an empty map, with the default comparer of the key.
        /// </summary>
        public TreeMap() : this(null, Comparer<K>.Default)
        {
        }

        /// <summary>
        /// Constructs an empty map with the specified comparer of the key.
        /// </summary>
        /// <param name="comparer">The comparer of the key.</param>
        public TreeMap(IComparer<K> comparer) : this(comparer.Compare)
        {
        }

        public TreeMap(Comparison<K> comparison)
        {
            llrbTree = new LlrbTree<K, V>(comparison);
        }

        /// <summary>
        /// Constructs a map with the items in the specified dictionary and using the default comparer of the key.
        /// </summary>
        /// <param name="source">The source dictionary.</param>
        public TreeMap(IDictionary<K, V> source) : this(source, Comparer<K>.Default)
        {
        }

        /// <summary>
        /// Constructs a mp with the items in the specified dictionary, by using the specified comparer of the key.
        /// </summary>
        /// <param name="source">The source dictionary.</param>
        /// <param name="comparer">The comparer of the key.</param>
        public TreeMap(IDictionary<K, V> source, IComparer<K> comparer) : this(source, comparer.Compare)
        {
        }

        public TreeMap(IDictionary<K, V> source, Comparison<K> comparison) : this(comparison)
        {
            comparison.ValidateNotNull("The comparison func should not be null.");

            if (null != source)
            {
                foreach (var key in source.Keys)
                {
                    Add(key, source[key]);
                }
            }
        }

        /// <summary>
        /// Adds a key value paire to the map. If the map already contains the key, an exception is thrown.
        /// </summary>
        /// <param name="key">The key to add.</param>
        /// <param name="value">The value to add.</param>
        public void Add(K key, V value)
        {
            llrbTree.Add(key, value);
        }

        /// <summary>
        /// Checks whether the specified key exists in the map.
        /// </summary>
        /// <param name="key">The key to search.</param>
        /// <returns>True if the key exists. Otherwise false.</returns>
        public bool ContainsKey(K key)
        {
            return llrbTree.Contains(key);
        }

        /// <summary>
        /// Returns a collection of keys in the map.
        /// </summary>
        public ICollection<K> Keys
        {
            get
            {
                var keys = new List<K>();
                foreach (var item in llrbTree)
                {
                    keys.Add(item.Key);
                }

                return keys;
            }
        }

        /// <summary>
        /// Removes the item identified by the key.
        /// </summary>
        /// <param name="key">The key used to search the item to remove.</param>
        /// <returns>True if the item identified by the key is removed. Otherwise false.</returns>
        public bool Remove(K key)
        {
            return llrbTree.Remove(key);
        }

        /// <summary>
        /// Try get value of the specified key. If the key does not exist in the map, it does not throw exception. It only returns false.The value is retrieved in the value param.
        /// </summary>
        /// <param name="key">The key of the value.</param>
        /// <param name="value">The value to retrieved.</param>
        /// <returns>True if key exists in the map. Otherwise false.</returns>
        public bool TryGetValue(K key, out V value)
        {
            Guarder.CheckNull(key);
            var hasValue = false;
            if (llrbTree.Contains(key))
            {
                hasValue = true;
                value = llrbTree[key];
            }
            else
            {
                value = default(V);
            }
            return hasValue;
        }

        /// <summary>
        /// Returns a collection of values in the map.
        /// </summary>
        public ICollection<V> Values
        {
            get
            {
                var values = new List<V>();
                foreach (var item in llrbTree)
                {
                    values.Add(item.Value);
                }
                return values;
            }
        }

        public V this[K key]
        {
            get
            {
                return llrbTree[key];
            }
            set
            {
                llrbTree[key] = value;
            }
        }

        /// <summary>
        /// Adds a key value pair to the map.
        /// </summary>
        /// <param name="item">The key value pair.</param>
        public void Add(KeyValuePair<K, V> item)
        {
            llrbTree.Add(item.Key, item.Value);
        }

        /// <summary>
        /// Clears the map by removing everything in it.
        /// </summary>
        public void Clear()
        {
            llrbTree.Clear();
        }

        /// <summary>
        /// Checks whether the key value pair exists in the map. Only if an item with the same key and value is considered existing.
        /// </summary>
        /// <param name="item">The key value paire to search.</param>
        /// <returns>True if key and value exist. Otherwise false.</returns>
        public bool Contains(KeyValuePair<K, V> item)
        {
            var contains = false;
            if (llrbTree.Contains(item.Key))
            {
                var v = llrbTree[item.Key];
                if (v != null && v.Equals(item.Value))
                {
                    contains = true;
                }
            }
            return contains;
        }

        public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
        {
            Guarder.CheckNull(array);
            llrbTree.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return llrbTree.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(KeyValuePair<K, V> item)
        {
            var removed = false;
            if (llrbTree.Contains(item.Key))
            {
                var v = llrbTree[item.Key];
                if (null != v && v.Equals(item.Value))
                {
                    llrbTree.Remove(item.Key);
                    removed = true;
                }
            }

            return removed;
        }

        public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
        {
            return llrbTree.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        void IDictionary.Add(object key, object value)
        {
            llrbTree.Add((K)key, (V)value);
        }

        bool IDictionary.Contains(object key)
        {
            return llrbTree.Contains((K)key);
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
            get
            {
                List<K> list = new List<K>();
                foreach (var item in this.Keys)
                {
                    list.Add(item);
                }
                return list;
            }
        }

        void IDictionary.Remove(object key)
        {
            llrbTree.Remove((K)key);
        }

        ICollection IDictionary.Values
        {
            get
            {
                List<V> list = new List<V>();
                foreach (var item in this.Values)
                {
                    list.Add(item);
                }
                return list;
            }
        }

        object IDictionary.this[object key]
        {
            get
            {
                return llrbTree[(K)key];
            }
            set
            {
                llrbTree[(K)key] = (V)value;
            }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            KeyValuePair<K, V>[] kvps = (KeyValuePair<K, V>[])array;
            this.CopyTo(kvps, index);
        }

        bool ICollection.IsSynchronized
        {
            get { return false; }
        }

        object ICollection.SyncRoot
        {
            get { throw new NotSupportedException("The sync root is not supported in Commons.Collections"); }
        }

#if NET45
        IEnumerable<K> IReadOnlyDictionary<K, V>.Keys
        {
            get { return this.Keys; }
        }

        IEnumerable<V> IReadOnlyDictionary<K, V>.Values
        {
            get { return Values; }
        }
#endif

        public ISortedSet<K> SortedKeySet
        {
            get
            {
                return NavigableKeySet;
            }
        }

        public INavigableSet<K> NavigableKeySet
        {
            get
            {
                var keySet = new TreeSet<K>(llrbTree.Comparer);
                foreach(var item in llrbTree)
                {
                    keySet.Add(item.Key);
                }
                return keySet;
            }
        }

        public KeyValuePair<K, V> Max
        {
            get
            {
                return llrbTree.Max;
            }
        }

        public KeyValuePair<K, V> Min
        {
            get
            {
                return llrbTree.Min;
            }
        }

        public void RemoveMax()
        {
            llrbTree.RemoveMax();
        }

        public void RemoveMin()
        {
            llrbTree.RemoveMin();
        }

        public bool IsEmpty
        {
            get
            {
                return llrbTree.IsEmpty;
            }
        }

        public KeyValuePair<K, V> Lower(K key)
        {
            return llrbTree.Lower(key);
        }

        public KeyValuePair<K, V> Higher(K key)
        {
            return llrbTree.Higher(key);
        }

        public KeyValuePair<K, V> Ceiling(K key)
        {
            return llrbTree.Ceiling(key);
        }

        public KeyValuePair<K, V> Floor(K key)
        {
            return llrbTree.Floor(key);
        }
    }
}
