// Copyright CommonsForNET. Author: Gujun Yang. email: gujun.yang@gmail.com
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commons.Collections
{
    /// <summary>
    /// A tree map with the left leaning red black tree implementation. A tree map is a sorted dictionary.
    /// </summary>
    /// <typeparam name="TKey">The key type</typeparam>
    /// <typeparam name="TValue">The value type</typeparam>
    [CLSCompliant(true)]
    public sealed class TreeMap<TKey, TValue> : ITreeMap<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, 
        IDictionary, ICollection, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable
    {
        private LlrbTree<TKey, TValue> llrbTree;

        private Comparison<TKey> comparer;

        /// <summary>
        /// Constructs an empty map, with the default comparer of the key.
        /// </summary>
        public TreeMap() : this(null, Comparer<TKey>.Default)
        {
        }

        /// <summary>
        /// Constructs an empty map with the specified comparer of the key.
        /// </summary>
        /// <param name="comparer">The comparer of the key.</param>
        public TreeMap(IComparer<TKey> comparer) : this(null, comparer)
        {
        }

        /// <summary>
        /// Constructs a map with the items in the specified dictionary and using the default comparer of the key.
        /// </summary>
        /// <param name="source">The source dictionary.</param>
        public TreeMap(IDictionary<TKey, TValue> source) : this(source, Comparer<TKey>.Default)
        {
        }

        /// <summary>
        /// Constructs a mp with the items in the specified dictionary, by using the specified comparer of the key.
        /// </summary>
        /// <param name="source">The source dictionary.</param>
        /// <param name="comparer">The comparer of the key.</param>
        public TreeMap(IDictionary<TKey, TValue> source, IComparer<TKey> comparer) : this(source, (k1, k2) => comparer.Compare(k1, k2))
        {
        }

        public TreeMap(Comparison<TKey> comparison) : this(null, comparison)
        {
        }

        public TreeMap(IDictionary<TKey, TValue> source, Comparison<TKey> comparison)
        {
            if (null == comparison)
            {
                this.comparer = (k1, k2) => Comparer<TKey>.Default.Compare(k1, k2);
            }
            else
            {
                this.comparer = comparison;
            }
            llrbTree = new LlrbTree<TKey, TValue>(this.comparer);

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
        public void Add(TKey key, TValue value)
        {
            llrbTree.Add(key, value);
        }

        /// <summary>
        /// Checks whether the specified key exists in the map.
        /// </summary>
        /// <param name="key">The key to search.</param>
        /// <returns>True if the key exists. Otherwise false.</returns>
        public bool ContainsKey(TKey key)
        {
            return llrbTree.Contains(key);
        }

        /// <summary>
        /// Returns a collection of keys in the map.
        /// </summary>
        public ICollection<TKey> Keys
        {
            get
            {
                var keys = new List<TKey>();
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
        public bool Remove(TKey key)
        {
            return llrbTree.Remove(key);
        }

        /// <summary>
        /// Try get value of the specified key. If the key does not exist in the map, it does not throw exception. It only returns false.The value is retrieved in the value param.
        /// </summary>
        /// <param name="key">The key of the value.</param>
        /// <param name="value">The value to retrieved.</param>
        /// <returns>True if key exists in the map. Otherwise false.</returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            var hasValue = false;
            if (llrbTree.Contains(key))
            {
                hasValue = true;
                value = llrbTree[key];
            }
            else
            {
                value = default(TValue);
            }
            return hasValue;
        }

        /// <summary>
        /// Returns a collection of values in the map.
        /// </summary>
        public ICollection<TValue> Values
        {
            get
            {
                var values = new List<TValue>();
                foreach (var item in llrbTree)
                {
                    values.Add(item.Value);
                }
                return values;
            }
        }

        public TValue this[TKey key]
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
        public void Add(KeyValuePair<TKey, TValue> item)
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
        public bool Contains(KeyValuePair<TKey, TValue> item)
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

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
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

        public bool Remove(KeyValuePair<TKey, TValue> item)
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

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return llrbTree.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return llrbTree.GetEnumerator();
        }

        public void Add(object key, object value)
        {
            llrbTree.Add((TKey)key, (TValue)value);
        }

        public bool Contains(object key)
        {
            return llrbTree.Contains((TKey)key);
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return new MapEnumerator(this);
        }

        public bool IsFixedSize
        {
            get { return false; }
        }

        ICollection IDictionary.Keys
        {
            get
            {
                List<TKey> list = new List<TKey>();
                foreach (var item in this.Keys)
                {
                    list.Add(item);
                }
                return list;
            }
        }

        public void Remove(object key)
        {
            llrbTree.Remove((TKey)key);
        }

        ICollection IDictionary.Values
        {
            get
            {
                List<TValue> list = new List<TValue>();
                foreach (var item in this.Values)
                {
                    list.Add(item);
                }
                return list;
            }
        }

        public object this[object key]
        {
            get
            {
                return llrbTree[(TKey)key];
            }
            set
            {
                llrbTree[(TKey)key] = (TValue)value;
            }
        }

        public void CopyTo(Array array, int index)
        {
            KeyValuePair<TKey, TValue>[] kvps = (KeyValuePair<TKey, TValue>[])array;
            this.CopyTo(kvps, index);
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public object SyncRoot
        {
            get { return null; }
        }

        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys
        {
            get { return this.Keys; }
        }

        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values
        {
            get { return this.Values; }
        }

        public ITreeSet<TKey> KeySet
        {
            get
            {
                TreeSet<TKey> keySet = new TreeSet<TKey>(this.comparer);
                foreach(var item in this)
                {
                    keySet.Add(item.Key);
                }
                return keySet;
            }
        }

        public KeyValuePair<TKey, TValue> Max
        {
            get
            {
                return llrbTree.Max;
            }
        }

        public KeyValuePair<TKey, TValue> Min
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

        private class MapEnumerator : IDictionaryEnumerator
        {
            private TreeMap<TKey, TValue> map;
            private IEnumerator<KeyValuePair<TKey, TValue>> iter;
            public MapEnumerator(TreeMap<TKey, TValue> map)
            {
                this.map = map;
                iter = this.map.GetEnumerator();
            }
            public DictionaryEntry Entry
            {
                get
                {
                    DictionaryEntry entry = new DictionaryEntry();
                    entry.Key = iter.Current.Key;
                    entry.Value = iter.Current.Value;
                    return entry;
                }
            }

            public object Key
            {
                get
                {
                    return Entry.Key;
                }
            }

            public object Value
            {
                get
                {
                    return Entry.Value;
                }
            }

            public object Current
            {
                get
                {
                    return Entry;
                }
            }

            public bool MoveNext()
            {
                return iter.MoveNext();
            }

            public void Reset()
            {
                iter.Reset();
            }
        }
    }
}
