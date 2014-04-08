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
    public class TreeMap<TKey, TValue> : IMap<TKey, TValue>
    {
        private LlrbTree<TKey, TValue> llrbTree;

        private IComparer<TKey> comparer;

        public TreeMap() : this(null, Comparer<TKey>.Default)
        {
        }

        public TreeMap(IComparer<TKey> comparer) : this(null, comparer)
        {

        }

        public TreeMap(IDictionary<TKey, TValue> source) : this(source, Comparer<TKey>.Default)
        {
        }

        public TreeMap(IDictionary<TKey, TValue> source, IComparer<TKey> comparer)
        {
            if (null == comparer)
            {
                this.comparer = Comparer<TKey>.Default;
            }
            else
            {
                this.comparer = comparer;
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

        public void Add(TKey key, TValue value)
        {
            llrbTree.Add(key, value);
        }

        public bool ContainsKey(TKey key)
        {
            return llrbTree.Contains(key);
        }

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

        public bool Remove(TKey key)
        {
            return llrbTree.Remove(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            var hasValue = false;
            if (llrbTree.Contains(key))
            {
                hasValue = true;
                value = llrbTree[key];
            }
            value = default(TValue);
            return hasValue;
        }

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

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            llrbTree.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            llrbTree.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            var contains = false;
            if (llrbTree.Contains(item.Key))
            {
                var v = llrbTree[item.Key];
                if (v != null && v.Equals(item.Key))
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
            return llrbTree.Remove(item.Key);
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
