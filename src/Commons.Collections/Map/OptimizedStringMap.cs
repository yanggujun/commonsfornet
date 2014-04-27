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

using Commons.Collections.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Commons.Collections.Map
{
    /// <summary>
    /// The data structure is a hash map intended for large map whose key is a string. 
    /// The map can be used as the primary data structure to store large JSON object.
    /// Different from the normal hash table which uses the object.GetHashCode() to generate the hash, 
    /// the OptimizedStringMap uses 32bit MurmurHash3 algorithm to generate hash code. The purpose is to reduce
    /// the collision and calculation cost for large string.
    /// The map will be in inefficient when the item number is small.
    /// </summary>
    /// <typeparam name="T">The type of the value item in the map.</typeparam>
    [CLSCompliant(true)]
    public class OptimizedStringMap<T> : IDictionary<string, T>, IEnumerable<KeyValuePair<string, T>>
    {
        private const int DefaultCapacity = 128;
        private const int MaxCapacity = 1 << 30;
        private const double LoadFactor = 0.75f;
        private int capacity = DefaultCapacity;
        private int threshold = (int)(DefaultCapacity * LoadFactor);
        private IHasher hasher;
        private HashEntry[] Entries { get; set; }

        public OptimizedStringMap() : this(DefaultCapacity)
        {
        }

        public OptimizedStringMap(int capacity) : this(capacity, null)
        {
        }

        public OptimizedStringMap(int capacity, IEnumerable<KeyValuePair<string, T>> items)
        {
            Count = 0;
            this.capacity = CalculateCapacity(capacity);
            this.threshold = (int)(this.capacity * LoadFactor);
            Entries = new HashEntry[this.capacity];
            hasher = new MurmurHash32();
            if (null != items)
            {
                foreach (var item in items)
                {
                    Add(item.Key, item.Value);
                }
            }
        }

        public void Add(string key, T value)
        {
            var newEntry = new HashEntry(new KeyValuePair<string, T>(key, value));
            // space to optmize?
            Put(Entries, newEntry);
            if (Count > threshold)
            {
                var newCapacity = CalculateCapacity(capacity << 1);
                if (newCapacity > capacity)
                {
                    capacity = newCapacity;
                    Rehash();
                }
            }
        }

        private void Put(HashEntry[] buckets, HashEntry entry)
        {
            var key = entry.Entry.Key;
            var hash = Hash(key);
            var index = HashIndex(hash);

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

        private uint Hash(string key)
        {
            var bytes = new byte[key.Length * sizeof(char)];
            Buffer.BlockCopy(key.ToCharArray(), 0, bytes, 0, bytes.Length);
            var hash = hasher.Hash(bytes);
            return hash[0];
        }

        private void Rehash()
        {
            HashEntry[] newEntries = new HashEntry[capacity];
            Count = 0;
            foreach (var entry in this)
            {
                var item = entry;
                Put(newEntries, new HashEntry(new KeyValuePair<string, T>(entry.Key, entry.Value)));
            }
            Entries = newEntries;
        }

        public bool ContainsKey(string key)
        {
            var hash = Hash(key);
            var index = HashIndex(hash);
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

        public ICollection<string> Keys
        {
            get
            {
                List<string> keys = new List<string>();
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

        public bool Remove(string key)
        {
            var removed = false;
            var hash = Hash(key);
            var index = HashIndex(hash);
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

        public bool TryGetValue(string key, out T value)
        {
            var contains = false;
            if (ContainsKey(key))
            {
                value = this[key];
                contains = true;
            }
            else
            {
                value = default(T);
            }

            return contains;
        }

        public ICollection<T> Values
        {
            get
            {
                List<T> values = new List<T>();
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

        public T this[string key]
        {
            get
            {
                if (!ContainsKey(key))
                {
                    throw new ArgumentException("The key does not exist in the map");
                }

                var hash = Hash(key);
                var index = HashIndex(hash);
                var entry = Entries[index];
                T v = default(T);
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

                var hash = Hash(key);
                var index = HashIndex(hash);
                var entry = Entries[index];
                while (null != entry)
                {
                    if (entry.Entry.Key.Equals(key))
                    {
                        entry.Entry = new KeyValuePair<string, T>(key, value);
                        break;
                    }
                    else
                    {
                        entry = entry.Next;
                    }
                }

            }
        }

        public void Add(KeyValuePair<string, T> item)
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

        public bool Contains(KeyValuePair<string, T> item)
        {
            return ContainsKey(item.Key) && this[item.Key].Equals(item.Value);
        }

        public void CopyTo(KeyValuePair<string, T>[] array, int arrayIndex)
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

        public bool Remove(KeyValuePair<string, T> item)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<string, T>> GetEnumerator()
        {
            return CreateEnumerator().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        private IEnumerable<KeyValuePair<string, T>> CreateEnumerator()
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

        private int CalculateCapacity(int proposedCapacity)
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
                newCapacity = newCapacity > MaxCapacity ? MaxCapacity : newCapacity;
            }
            threshold = (int) (newCapacity * LoadFactor);

            return newCapacity;
        }

        private uint HashIndex(uint hash)
        {
            return (uint)(hash & capacity - 1);
        }

#if DEBUG
        [DebuggerDisplay("Entry = {Output}")]
#endif
        private class HashEntry
        {
            public HashEntry(KeyValuePair<string, T> entry)
            {
                this.Entry = entry;
            }
            public KeyValuePair<string, T> Entry { get; set; }
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
