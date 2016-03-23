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
using Commons.Utils;

namespace Commons.Collections.Map
{
    [CLSCompliant(true)]
    public abstract class AbstractMultiValueMap<K, V> : IMultiValueMap<K, V>, ICollection<KeyValuePair<K, V>>, ICollection,
        IReadOnlyMultiValueMap<K, V>, 
#if NET45
        IReadOnlyCollection<KeyValuePair<K, V>>, 
#endif
        IEnumerable<KeyValuePair<K, V>>, 
        IEnumerable<KeyValuePair<K, ICollection<V>>>, IEnumerable
    {
        private readonly Equator<V> valueEquator;

        protected abstract IDictionary<K, ICollection<V>> Map { get; }

        protected AbstractMultiValueMap(Equator<V> valueEquator)
        {
            this.valueEquator = valueEquator;
        }

        public void Add(K key, V value)
        {
            key.ValidateNotNull("The key is null!");
            value.ValidateNotNull("The value is null!");
            if (Map.ContainsKey(key))
            {
                Map[key].Add(value);
            }
            else
            {
                Map.Add(key, new List<V> { value });
            }
        }

        public void Add(K key, ICollection<V> values)
        {
            key.ValidateNotNull("The key is null.");
            values.ValidateNotNull("The value collection is null.");
            foreach (var value in values)
            {
                Add(key, value);
            }
        }

        public bool Remove(K key)
        {
            return Map.Remove(key);
        }

        public bool ContainsKey(K key)
        {
            return Map.ContainsKey(key);
        }

        public bool RemoveValue(K key, V value)
        {
            var removed = false;
            V toRemove = default(V);
            if (Map.ContainsKey(key))
            {
                foreach (var v in Map[key])
                {
                    if (valueEquator(v, value))
                    {
                        removed = true;
                        toRemove = v;
                        break;
                    }
                }
                if (removed)
                { 
                    Map[key].Remove(toRemove);
                }
            }

            return removed;
        }

        public bool TryGetValue(K key, out List<V> values)
        {
            var found = false;
            List<V> collection = null;
            if (Map.ContainsKey(key))
            {
                found = true;
                collection = new List<V>();
                foreach (var v in Map[key])
                {
                    collection.Add(v);
                }
            }

            values = collection;
            return found;
        }

        public int CountOf(K key)
        {
            var count = 0;
            if (Map.ContainsKey(key))
            {
                count = Map[key].Count;
            }
            else
            {
                throw new KeyNotFoundException("The key does not exist!");
            }

            return count;
        }

        public int KeyCount
        {
            get
            {
                return Map.Keys.Count;
            }
        }

        public bool ContainsValue(K key, V value)
        {
            var found = false;
            if (Map.ContainsKey(key))
            {
                foreach(var v in Map[key])
                {
                    if (valueEquator(v, value))
                    {
                        found = true;
                    }
                }
            }

            return found;
        }

        public ICollection<V> this[K key]
        {
            get { return Map[key]; }
        }

        public ICollection<K> Keys
        {
            get { return Map.Keys; }
        }

        public ICollection<V> Values
        {
            get 
            {
                var valueList = new List<V>();

                foreach(var kvp in Map)
                {
                    foreach(var v in kvp.Value)
                    {
                        valueList.Add(v);
                    }
                }

                return valueList;
            }
        }

        void ICollection<KeyValuePair<K, V>>.Add(KeyValuePair<K, V> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            Map.Clear();
        }

        bool ICollection<KeyValuePair<K, V>>.Contains(KeyValuePair<K, V> item)
        {
            return ContainsValue(item.Key, item.Value);
        }

        public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
        {
            array.ValidateNotNull("The array is null.");
            var index = arrayIndex;
            foreach(var kvp in Map)
            {
                foreach(var v in kvp.Value)
                {
                    array[index++] = new KeyValuePair<K, V>(kvp.Key, v);
                }
            }
        }

        /// <summary>
        /// The return value represents the count of all the values in this multi value map.
        /// It represents how many key value pairs of [<typeparam name="K"/>, <typeparam name="V"/>] in the multi value map.
        /// </summary>
        public int Count
        {
            get
            {
                var count = 0;
                foreach(var kvp in Map)
                {
                    count += kvp.Value.Count;
                }

                return count;
            }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        bool ICollection<KeyValuePair<K, V>>.Remove(KeyValuePair<K, V> item)
        {
            return RemoveValue(item.Key, item.Value);
        }

        public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
        {
            return SingleItems.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        void ICollection.CopyTo(Array array, int index)
        {
            var theArray = array as KeyValuePair<K, V>[];
            CopyTo(theArray, index);
        }

        bool ICollection.IsSynchronized
        {
            get { return false; }
        }

        object ICollection.SyncRoot
        {
            get { throw new NotSupportedException("The SyncRoot is not supported in Commons.Collections."); }
        }

        IEnumerator<KeyValuePair<K, ICollection<V>>> IEnumerable<KeyValuePair<K, ICollection<V>>>.GetEnumerator()
        {
            return CollectionItems.GetEnumerator();
        }

        IEnumerable<K> IReadOnlyMultiValueMap<K, V>.Keys
        {
            get { return Keys; }
        }

        IEnumerable<V> IReadOnlyMultiValueMap<K, V>.Values
        {
            get { return Values; }
        }

        private IEnumerable<KeyValuePair<K, V>> SingleItems
        {
            get
            {
                foreach (var kvp in Map)
                {
                    foreach(var v in kvp.Value)
                    {
                        yield return new KeyValuePair<K, V>(kvp.Key, v);
                    }
                }
            }
        }

        private IEnumerable<KeyValuePair<K, ICollection<V>>> CollectionItems
        {
            get
            {
                foreach (var kvp in Map)
                {
                    yield return kvp;
                }
            }
        }
    }
}
