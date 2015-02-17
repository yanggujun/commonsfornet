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
using Commons.Utils;

namespace Commons.Collections.Map
{
    /// <summary>
    /// The abstract class provides an alternative skeleton class for dictionary.
    /// The purpose is to enable the alternative hash algorithms.
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    [CLSCompliant(true)]
    public abstract class AbstractHashedMap<K, V> : IDictionary<K, V>, ICollection<KeyValuePair<K, V>>, IDictionary, ICollection,
        IReadOnlyDictionary<K, V>, IReadOnlyCollection<KeyValuePair<K, V>>, IEnumerable<KeyValuePair<K, V>>, IEnumerable
    {
        private const int MaxCapacity = 1 << 30;
        private const double LoadFactor = 0.75f;

        protected int Capacity { get; set; }
        protected int Threshold { get; set; }
        protected HashEntry[] Entries { get; set; }
        protected readonly Equator<K> IsEqual;

        protected AbstractHashedMap(int capacity, Equator<K> isEqual)
        {
            capacity.Validate(x => x > 0, new ArgumentException("Capacity must be larger than 0."));
            Guarder.CheckNull(isEqual);
            Count = 0;
            this.Capacity = CalculateCapacity(capacity);
            this.IsEqual = isEqual;
            Entries = new HashEntry[this.Capacity];
        }

        public virtual void Add(K key, V value)
        {
            Guarder.CheckNull(key);
            var newEntry = CreateEntry(key, value);
            // space to optimize?
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

        public virtual bool ContainsKey(K key)
        {
            Guarder.CheckNull(key);
            var entry = GetEntry(key);

            return entry != null;
        }

        public virtual ICollection<K> Keys
        {
            get
            {
                List<K> keys = new List<K>();
                foreach (var entry in Entries)
                {
                    var cursor = entry;
                    while (null != cursor)
                    {
                        keys.Add(cursor.Key);
                        cursor = cursor.Next;
                    }
                }

                return keys;
            }
        }

        public virtual bool Remove(K key)
        {
            Guarder.CheckNull(key);
            var removed = false;
            var index = HashIndex(key);
            var entry = Entries[index];
            if (null != entry)
            {
                if (IsEqual(entry.Key, key))
                {
                    Entries[index] = entry.Next;
                    removed = true;
                }
                else
                {
                    while (null != entry.Next)
                    {
                        var item = entry.Next;
                        if (IsEqual(item.Key, key))
                        {
                            entry.Next = item.Next;
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

        public virtual bool TryGetValue(K key, out V value)
        {
            Guarder.CheckNull(key);
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

        public virtual ICollection<V> Values
        {
            get
            {
                var values = new List<V>();
                foreach (var entry in Entries)
                {
                    var item = entry;
                    while (null != item)
                    {
                        values.Add(item.Value);
                        item = item.Next;
                    }
                }

                return values;
            }
        }

        public virtual V this[K key]
        {
            get
            {
                return Get(key);
            }
            set
            {
                Set(key, value);
            }
        }

        public void Add(KeyValuePair<K, V> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            for (var i = 0; i < Entries.Length; i++)
            {
                //TODO: possible memory leak
                Entries[i] = null;
            }
            Count = 0;
        }

        public virtual bool Contains(KeyValuePair<K, V> item)
        {
            return ContainsKey(item.Key) && this[item.Key].Equals(item.Value);
        }

        public virtual void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
        {
            Guarder.CheckNull(array);
            var index = 0;
            foreach (var entry in this)
            {
                array[arrayIndex + (index++)] = entry;
            }
        }

        public int Count { get; protected set; }

        public virtual bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public virtual bool Remove(KeyValuePair<K, V> item)
        {
            var removed = false;
            if (Contains(item))
            {
                removed = Remove(item.Key);
            }
            return removed;
        }

        public virtual IEnumerator<KeyValuePair<K, V>> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        protected IEnumerable<KeyValuePair<K, V>> Items
        {
            get
            {
                foreach (var entry in Entries)
                {
                    var item = entry;
                    while (null != item)
                    {
                        yield return new KeyValuePair<K, V>(item.Key, item.Value);
                        item = item.Next;
                    }
                }
            }
        }

        protected virtual V Get(K key)
        { 
            Guarder.CheckNull(key);
            var entry = GetEntry(key);
            entry.Validate(x => x != null, new KeyNotFoundException(string.Format("The key {0} does not exist in the map. ", key)));
            return entry.Value;
        }

        protected virtual void Set(K key, V v)
        {
            Guarder.CheckNull(key);
            var entry = GetEntry(key);
            if (entry == null)
            {
                Add(key, v);
            }
            else
            {
                entry.Value = v;
            }
        }

        protected int CalculateCapacity(int proposedCapacity)
        {
            int newCapacity = 1;
            if (proposedCapacity > MaxCapacity)
            {
                newCapacity = MaxCapacity;
            }
            else
            {
                while (newCapacity < proposedCapacity)
                {
                    newCapacity <<= 1;
                }
                Threshold = (int)(newCapacity * LoadFactor);
                while (proposedCapacity > Threshold)
                {
                    newCapacity <<= 1;
                    Threshold = (int)(newCapacity * LoadFactor);
                }
                newCapacity = newCapacity > MaxCapacity ? MaxCapacity : newCapacity;
            }

            return newCapacity;
        }

        protected abstract long HashIndex(K key);

        protected virtual HashEntry CreateEntry(K key, V value)
        {
            return new HashEntry(key, value);
        }

        protected HashEntry GetEntry(K key)
        {
            var index = HashIndex(key);
            var entry = Entries[index];
            HashEntry target = null;
            while (null != entry)
            {
                if (IsEqual(entry.Key, key))
                {
                    target = entry;
                    break;
                }
                entry = entry.Next;
            }
            return target;
        }

        protected void Put(HashEntry[] buckets, HashEntry entry)
        {
            var key = entry.Key;
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
                    if (IsEqual(item.Key, key))
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

        protected virtual void Rehash()
        {
            var newEntries = new HashEntry[Capacity];
            Count = 0;
            foreach (var entry in Entries)
            {
                var item = entry;
                while (item != null)
                { 
                    Put(newEntries, CreateEntry(item.Key, item.Value));
                    item = item.Next;
                }
            }
            Entries = newEntries;
        }


        void IDictionary.Add(object key, object value)
        {
            Add((K)key, (V)value);
        }

        void IDictionary.Clear()
        {
            Clear();
        }

        bool IDictionary.Contains(object key)
        {
            return ContainsKey((K)key);
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return new MapEnumerator<K, V>(this);
        }

        bool IDictionary.IsFixedSize
        {
            get { return false; }
        }

        bool IDictionary.IsReadOnly
        {
            get { return false; }
        }

        ICollection IDictionary.Keys
        {
            get
            {
                var keys = new List<K>();
                foreach (var k in Keys)
                {
                    keys.Add(k);
                }
                return keys;
            }
        }

        void IDictionary.Remove(object key)
        {
            this.Remove((K)key);
        }

        ICollection IDictionary.Values
        {
            get
            {
                var values = new List<V>();
                foreach (var v in Values)
                {
                    values.Add(v);
                }
                return values;
            }
        }

        object IDictionary.this[object key]
        {
            get
            {
                return this[(K)key];
            }
            set
            {
                this[(K)key] = (V)value;
            }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            var items = (KeyValuePair<K, V>[])array;
            this.CopyTo(items, index);
        }

        int ICollection.Count
        {
            get { return this.Count; }
        }

        bool ICollection.IsSynchronized
        {
            get { return false; }
        }

        object ICollection.SyncRoot
        {
            get { throw new NotSupportedException("The sync root is not supported in Commons.Collections"); }
        }

        bool IReadOnlyDictionary<K, V>.ContainsKey(K key)
        {
            return this.ContainsKey(key);
        }

        IEnumerable<K> IReadOnlyDictionary<K, V>.Keys
        {
            get { return this.Keys; }
        }

        bool IReadOnlyDictionary<K, V>.TryGetValue(K key, out V value)
        {
            return this.TryGetValue(key, out value);
        }

        IEnumerable<V> IReadOnlyDictionary<K, V>.Values
        {
            get { return this.Values; }
        }

        V IReadOnlyDictionary<K, V>.this[K key]
        {
            get { return this[key]; }
        }

        int IReadOnlyCollection<KeyValuePair<K, V>>.Count
        {
            get { return this.Count; }
        }

        IEnumerator<KeyValuePair<K, V>> IEnumerable<KeyValuePair<K, V>>.GetEnumerator()
        {
            return this.GetEnumerator();
        }

#if DEBUG
        [DebuggerDisplay("Entry = {Output}")]
#endif
        protected class HashEntry
        {
            public HashEntry(KeyValuePair<K, V> entry)
            {
                Key = entry.Key;
                Value = entry.Value;
            }
            public HashEntry(K key, V value)
            {
                Key = key;
                Value = value;
            }
            public K Key { get; set; }
            public V Value { get; set; }
            public HashEntry Next { get; set; }

#if DEBUG
            public string Output
            {
                get
                {
                    return this.Key + (this.Next == null ? string.Empty : " Next: " + this.Next.Output);
                }
            }
#endif
        }
    }
}
