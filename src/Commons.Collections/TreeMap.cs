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
        private LlrbTreeSet<KeyValuePair<TKey, TValue>> keyValueSet;

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
            if (null != source)
            {
                foreach (var key in source.Keys)
                {
                    Add(key, source[key]);
                }
            }

            if (null == comparer)
            {
                this.comparer = Comparer<TKey>.Default;
            }
            else
            {
                this.comparer = comparer;
            }
            keyValueSet = new LlrbTreeSet<KeyValuePair<TKey, TValue>>(source, new KeyValueComparer(comparer));
        }

        public void Add(TKey key, TValue value)
        {
            KeyValuePair<TKey, TValue> pair = new KeyValuePair<TKey, TValue>(key, value);
            keyValueSet.Add(pair);
        }

        public bool ContainsKey(TKey key)
        {
            return false;
        }

        public ICollection<TKey> Keys
        {
            get
            {
                var keys = new List<TKey>();
                foreach (var item in keyValueSet)
                {
                    keys.Add(item.Key);
                }

                return keys;
            }
        }

        public bool Remove(TKey key)
        {
            return false;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            value = default(TValue);
            return false;
        }

        public ICollection<TValue> Values
        {
            get
            {
                var values = new List<TValue>();
                foreach (var item in keyValueSet)
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
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            keyValueSet.Add(item);
        }

        public void Clear()
        {
            keyValueSet.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return keyValueSet.Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { return keyValueSet.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return keyValueSet.Remove(item);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return keyValueSet.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return keyValueSet.GetEnumerator();
        }

        public void Add(object key, object value)
        {
            throw new NotImplementedException();
        }

        public bool Contains(object key)
        {
            throw new NotImplementedException();
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public bool IsFixedSize
        {
            get { throw new NotImplementedException(); }
        }

        ICollection IDictionary.Keys
        {
            get { throw new NotImplementedException(); }
        }

        public void Remove(object key)
        {
            throw new NotImplementedException();
        }

        ICollection IDictionary.Values
        {
            get { throw new NotImplementedException(); }
        }

        public object this[object key]
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public bool IsSynchronized
        {
            get { throw new NotImplementedException(); }
        }

        public object SyncRoot
        {
            get { throw new NotImplementedException(); }
        }


        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys
        {
            get { throw new NotImplementedException(); }
        }

        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values
        {
            get { throw new NotImplementedException(); }
        }

        private class KeyValueComparer : IComparer<KeyValuePair<TKey, TValue>>
        {
            IComparer<TKey> keyComparer;
            public KeyValueComparer(IComparer<TKey> comparer)
            {
                this.keyComparer = comparer;
            }
            public int Compare(KeyValuePair<TKey, TValue> x, KeyValuePair<TKey, TValue> y)
            {
                return keyComparer.Compare(x.Key, y.Key);
            }
        }
    }
}
