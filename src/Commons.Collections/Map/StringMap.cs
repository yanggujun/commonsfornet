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
using System.Diagnostics;

using Commons.Collections.Common;

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
    public sealed class StringMap<T> : AbstractHashMap<string ,T>, IEnumerable, IDictionary, ICollection, ICollection<KeyValuePair<string, T>>, IReadOnlyDictionary<string, T>
    {
        private const int DefaultCapacity = 128;
        private const int MaxCapacity = 1 << 30;
        private const double LoadFactor = 0.75f;

        private IHashStrategy hasher;
        public StringMap() : this(DefaultCapacity)
        {
        }

        public StringMap(int capacity) : this(capacity, null)
        {
        }

        public StringMap(IEnumerable<KeyValuePair<string, T>> items)
            : base(DefaultCapacity, items)
        {
        }

        public StringMap(int capacity, IEnumerable<KeyValuePair<string, T>> items) : base(capacity, items)
        {
            hasher = new MurmurHash32();
        }

        private uint Hash(string key)
        {
            var bytes = new byte[key.Length * sizeof(char)];
            Buffer.BlockCopy(key.ToCharArray(), 0, bytes, 0, bytes.Length);
            var hash = hasher.Hash(bytes);
            return (uint)hash[0];
        }

        protected override int CalculateCapacity(int proposedCapacity)
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

        protected override long HashIndex(string key)
        {
            var hash = Hash(key);
            return hash & (Capacity - 1);
        }

        void IDictionary.Add(object key, object value)
        {
            this.Add((string)key, (T)value);
        }

        void IDictionary.Clear()
        {
            this.Clear();
        }

        bool IDictionary.Contains(object key)
        {
            return this.ContainsKey((string)key);
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            throw new NotImplementedException();
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
                var keys = new List<string>();
                foreach (var k in Keys)
                {
                    keys.Add(k);
                }
                return keys;
            }
        }

        void IDictionary.Remove(object key)
        {
            this.Remove((string)key);
        }

        ICollection IDictionary.Values
        {
            get
            {
                var values = new List<T>();
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
                return this[(string)key];
            }
            set
            {
                this[(string)key] = (T)value;
            }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            var items = (KeyValuePair<string, T>[])array;
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
            get { return null; }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        bool IReadOnlyDictionary<string, T>.ContainsKey(string key)
        {
            return this.ContainsKey(key);
        }

        IEnumerable<string> IReadOnlyDictionary<string, T>.Keys
        {
            get { return this.Keys; }
        }

        bool IReadOnlyDictionary<string, T>.TryGetValue(string key, out T value)
        {
            return this.TryGetValue(key, out value);
        }

        IEnumerable<T> IReadOnlyDictionary<string, T>.Values
        {
            get { return this.Values; }
        }

        T IReadOnlyDictionary<string, T>.this[string key]
        {
            get { return this[key]; }
        }

        int IReadOnlyCollection<KeyValuePair<string, T>>.Count
        {
            get { return this.Count; }
        }

        IEnumerator<KeyValuePair<string, T>> IEnumerable<KeyValuePair<string, T>>.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
