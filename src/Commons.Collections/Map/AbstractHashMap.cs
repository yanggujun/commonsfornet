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
using System.Collections.Generic;
using System.Diagnostics;
using System.Collections;

using Commons.Collections.Common;

namespace Commons.Collections.Map
{
    /// <summary>
    /// The abstract class provides an alternative skeleton class for dictionary.
    /// The purpose is to enable the alternative hash algorithms.
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    [CLSCompliant(true)]
    public abstract class AbstractHashMap<K, V> : IDictionary<K, V>, IEnumerable<KeyValuePair<K, V>>
    {
        protected int Capacity { get; set; }
        protected int Threshold { get; set; }
        protected HashEntry[] Entries { get; set; }

        protected AbstractHashMap(int capacity, IEnumerable<KeyValuePair<K, V>> items)
        {
            Count = 0;
            this.Capacity = CalculateCapacity(capacity);
            Entries = new HashEntry[this.Capacity];
            if (null != items)
            {
                foreach (var item in items)
                {
                    Add(item.Key, item.Value);
                }
            }
        }

        public void Add(K key, V value)
        {
            var newEntry = new HashEntry(new KeyValuePair<K, V>(key, value));
            // space to optmize?
            Put(Entries, newEntry);
            if (Count > Threshold)
            {
                var newCapacity = CalculateCapacity(Capacity << 1);
                if (newCapacity > Capacity)
                {
                    Capacity = newCapacity;
                    Rehash();
                }
            }
        }

        private void Put(HashEntry[] buckets, HashEntry entry)
        {
            var key = entry.Entry.Key;
            var index = HashIndex(key);

            var item = buckets[index];
            if (null == item)
            {
                buckets[index] = entry;
            }
            else
            {
                HashEntry cursor = null;
                while (null != item)
                {
                    if (item.Entry.Key.Equals(key))
                    {
                        throw new ArgumentException("The key already exists in the map.");
                    }
                    cursor = item;
                    item = item.Next;
                }
                cursor.Next = entry;
            }
            Count++;
        }

        private void Rehash()
        {
            HashEntry[] newEntries = new HashEntry[Capacity];
            Count = 0;
            foreach (var entry in this)
            {
                var item = entry;
                Put(newEntries, new HashEntry(new KeyValuePair<K, V>(entry.Key, entry.Value)));
            }
            Entries = newEntries;
        }

        public bool ContainsKey(K key)
        {
            var index = HashIndex(key);
            var item = Entries[index];
            var contains = false;
            while (null != item)
            {
                if (item.Entry.Key.Equals(key))
                {
                    contains = true;
                    break;
                }
                item = item.Next;
            }

            return contains;
        }

        public ICollection<K> Keys
        {
            get
            {
                List<K> keys = new List<K>();
                foreach (var entry in Entries)
                {
                    var item = entry;
                    while (null != item)
                    {
                        keys.Add(entry.Entry.Key);
                        item = entry.Next;
                    }
                }

                return keys;
            }
        }

        public bool Remove(K key)
        {
            var removed = false;
            var index = HashIndex(key);
            var entry = Entries[index];
            if (null != entry)
            {
                if (entry.Entry.Key.Equals(key))
                {
                    Entries[index] = entry.Next;
                    removed = true;
                }
                else
                {
                    while (null != entry.Next)
                    {
                        var item = entry.Next;
                        if (item.Entry.Key.Equals(key))
                        {
                            entry = item.Next;
                            removed = true;
                            break;
                        }
                        entry = item;
                    }
                }
            }
            if (removed)
            {
                Count--;
            }
            return removed;
        }

        public bool TryGetValue(K key, out V value)
        {
            var contains = false;
            if (ContainsKey(key))
            {
                value = this[key];
                contains = true;
            }
            else
            {
                value = default(V);
            }

            return contains;
        }

        public ICollection<V> Values
        {
            get
            {
                List<V> values = new List<V>();
                foreach (var entry in Entries)
                {
                    var item = entry;
                    while (null != item)
                    {
                        values.Add(item.Entry.Value);
                        item = item.Next;
                    }
                }

                return values;
            }
        }

        public V this[K key]
        {
            get
            {
                if (!ContainsKey(key))
                {
                    throw new ArgumentException("The key does not exist in the map");
                }

                var index = HashIndex(key);
                var entry = Entries[index];
                V v = default(V);
                while (null != entry)
                {
                    if (entry.Entry.Key.Equals(key))
                    {
                        v = entry.Entry.Value;
                        break;
                    }
                    else
                    {
                        entry = entry.Next;
                    }
                }
                return v;
            }
            set
            {
                if (!ContainsKey(key))
                {
                    throw new ArgumentException("The key does not exist in the map");
                }

                var index = HashIndex(key);
                var entry = Entries[index];
                while (null != entry)
                {
                    if (entry.Entry.Key.Equals(key))
                    {
                        entry.Entry = new KeyValuePair<K, V>(key, value);
                        break;
                    }
                    else
                    {
                        entry = entry.Next;
                    }
                }

            }
        }

        public void Add(KeyValuePair<K, V> item)
        {
            this.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            for (var i = 0; i < Entries.Length; i++)
            {
                Entries[i] = null;
            }
        }

        public bool Contains(KeyValuePair<K, V> item)
        {
            return ContainsKey(item.Key) && this[item.Key].Equals(item.Value);
        }

        public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
        {
            var index = 0;
            foreach (var entry in this)
            {
                array[arrayIndex + (index++)] = entry;
            }
        }

        public int Count { get; private set; }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public bool Remove(KeyValuePair<K, V> item)
        {
            var removed = false;
            if (Contains(item))
            {
                removed = Remove(item.Key);
            }
            return removed;
        }

        public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
        {
            return CreateEnumerator().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        private IEnumerable<KeyValuePair<K, V>> CreateEnumerator()
        {
            foreach (var entry in Entries)
            {
                var item = entry;
                while (null != item)
                {
                    yield return item.Entry;
                    item = item.Next;
                }
            }
        }

        protected abstract int CalculateCapacity(int proposedCapacity);

        protected abstract long HashIndex(K key);

#if DEBUG
        [DebuggerDisplay("Entry = {Output}")]
#endif
        protected class HashEntry
        {
            public HashEntry(KeyValuePair<K, V> entry)
            {
                this.Entry = entry;
            }
            public KeyValuePair<K, V> Entry { get; set; }
            public HashEntry Next { get; set; }

#if DEBUG
            public string Output
            {
                get
                {
                    return this.Entry.Key + (this.Next == null ? string.Empty : " Next: " + this.Next.Output);
                }
            }
#endif
        }
    }
}
