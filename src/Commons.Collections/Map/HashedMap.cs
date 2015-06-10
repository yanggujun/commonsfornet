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
    public class HashedMap<K, V> : IDictionary<K, V>, ICollection<KeyValuePair<K, V>>, IDictionary, ICollection,
#if NET45
        IReadOnlyDictionary<K, V>, IReadOnlyCollection<KeyValuePair<K, V>>, 
#endif
		IEnumerable<KeyValuePair<K, V>>, IEnumerable
    {
        private const int DefaultCapacity = 16;
		private const double DefaultLoadFactor = 0.75f;
		private const int MaxCapacity = 1 << 30;

		private readonly IEqualityComparer<K> comparer;

		private int capacity;
		private int threshold;
		private Entry[] entries;

        public HashedMap() : this(DefaultCapacity)
        {
        }

        public HashedMap(IEqualityComparer<K> comparer)
            : this(comparer.Equals)
        {
        }

        public HashedMap(Equator<K> equator) : this(DefaultCapacity, equator)
        {
        }

        public HashedMap(int capacity) : this(capacity, EqualityComparer<K>.Default)
        {
        }

        public HashedMap(int capacity, IEqualityComparer<K> comparer)
        {
			Guarder.CheckNull(comparer);
			if (capacity < 0)
			{
				throw new ArgumentException("The capacity must be larger than zero.");
			}
			this.comparer = comparer;
			this.capacity = CalculateCapacity(capacity);
			Count = 0;
			entries = new Entry[this.capacity];
        }

        public HashedMap(int capacity, Equator<K> equator)
        {
        }

        public HashedMap(IDictionary<K, V> items, IEqualityComparer<K> comparer) : this(items, comparer.Equals)
        {
        }

        public HashedMap(IDictionary<K, V> items) : this(items, EqualityComparer<K>.Default.Equals)
        {
        }

        public HashedMap(IDictionary<K, V> items, Equator<K> equator)
        {
        }

        private int HashIndex(K key)
        {
            var hash = key.GetHashCode();
            return hash & (capacity - 1);
        }

		public void Add(K key, V value)
		{
			Guarder.CheckNull(key);
			Enroute(key, value);
			if (Count > threshold)
			{
				Inflate();
			}
		}

		private void Enroute(K key, V value)
		{
			var index = HashIndex(key);
			var entry = entries[index];
			for (var i = 0; i < entry.Filled; i++)
			{
				if (comparer.Equals(key, entry.Items[i].Key))
				{
					throw new ArgumentException("The element already exists.");
				}
			}
			
			if (entry.Items == null)
			{
				entry.Items = new InnerEntry[1];
			}

			if (entry.Filled >= entry.Items.Length)
			{
				var oldItems = entry.Items;
				entry.Items = new InnerEntry[entry.Filled << 1];
				for (var i = 0; i < entry.Filled; i++)
				{
					entry.Items[i] = oldItems[i];
				}
			}
			entry.Items[entry.Filled].Key = key;
			entry.Items[entry.Filled].Value = value;
			entry.Filled++;
			Count++;
		}

		private void Inflate()
		{
			var oldEntries = entries;
			capacity <<= 1;
			capacity = capacity > MaxCapacity ? MaxCapacity : capacity;
			threshold = Convert.ToInt32(capacity * DefaultLoadFactor);
			entries = new Entry[capacity];
			Count = 0;
			foreach (var element in oldEntries)
			{
				for (var i = 0; i < element.Filled; i++)
				{
					Enroute(element.Items[i].Key, element.Items[i].Value);
				}
			}
		}
		
		public bool ContainsKey(K key)
		{
			var index = HashIndex(key);
			var entry = entries[index];
			for (var i = 0; i < entry.Filled; i++)
			{
				if (comparer.Equals(key, entry.Items[i].Key))
				{
					return true;
				}
			}

			return false;
		}

		public ICollection<K> Keys
		{
			get { throw new NotImplementedException(); }
		}

		public bool Remove(K key)
		{
			throw new NotImplementedException();
		}

		public bool TryGetValue(K key, out V value)
		{
			throw new NotImplementedException();
		}

		public ICollection<V> Values
		{
			get { throw new NotImplementedException(); }
		}

		public V this[K key]
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

		public void Add(KeyValuePair<K, V> item)
		{
			throw new NotImplementedException();
		}

		public void Clear()
		{
			throw new NotImplementedException();
		}

		public bool Contains(KeyValuePair<K, V> item)
		{
			throw new NotImplementedException();
		}

		public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}

		public int Count
		{
			get;
			private set;
		}

		public bool IsReadOnly
		{
			get { throw new NotImplementedException(); }
		}

		public bool Remove(KeyValuePair<K, V> item)
		{
			throw new NotImplementedException();
		}

		public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
		{
			throw new NotImplementedException();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotImplementedException();
		}

		void IDictionary.Add(object key, object value)
		{
			throw new NotImplementedException();
		}

		void IDictionary.Clear()
		{
			throw new NotImplementedException();
		}

		bool IDictionary.Contains(object key)
		{
			throw new NotImplementedException();
		}

		IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			throw new NotImplementedException();
		}

		bool IDictionary.IsFixedSize
		{
			get { throw new NotImplementedException(); }
		}

		bool IDictionary.IsReadOnly
		{
			get { throw new NotImplementedException(); }
		}

		ICollection IDictionary.Keys
		{
			get { throw new NotImplementedException(); }
		}

		void IDictionary.Remove(object key)
		{
			throw new NotImplementedException();
		}

		ICollection IDictionary.Values
		{
			get { throw new NotImplementedException(); }
		}

		object IDictionary.this[object key]
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

		void ICollection.CopyTo(Array array, int index)
		{
			throw new NotImplementedException();
		}

		int ICollection.Count
		{
			get { throw new NotImplementedException(); }
		}

		bool ICollection.IsSynchronized
		{
			get { throw new NotImplementedException(); }
		}

		object ICollection.SyncRoot
		{
			get { throw new NotImplementedException(); }
		}

		bool IReadOnlyDictionary<K, V>.ContainsKey(K key)
		{
			throw new NotImplementedException();
		}

		IEnumerable<K> IReadOnlyDictionary<K, V>.Keys
		{
			get { throw new NotImplementedException(); }
		}

		bool IReadOnlyDictionary<K, V>.TryGetValue(K key, out V value)
		{
			throw new NotImplementedException();
		}

		IEnumerable<V> IReadOnlyDictionary<K, V>.Values
		{
			get { throw new NotImplementedException(); }
		}

		V IReadOnlyDictionary<K, V>.this[K key]
		{
			get { throw new NotImplementedException(); }
		}

		int IReadOnlyCollection<KeyValuePair<K, V>>.Count
		{
			get { throw new NotImplementedException(); }
		}

		IEnumerator<KeyValuePair<K, V>> IEnumerable<KeyValuePair<K, V>>.GetEnumerator()
		{
			throw new NotImplementedException();
		}

		private struct Entry
		{
			public int Filled { get; set; }
			public InnerEntry[] Items { get; set; }
		}

		private struct InnerEntry
		{
			public K Key { get; set; }

			public V Value { get; set; }
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
                threshold = (int)(newCapacity * DefaultLoadFactor);
                while (proposedCapacity > threshold)
                {
                    newCapacity <<= 1;
                    threshold = (int)(newCapacity * DefaultLoadFactor);
                }
                newCapacity = newCapacity > MaxCapacity ? MaxCapacity : newCapacity;
            }

            return newCapacity;
        }
	}
}
