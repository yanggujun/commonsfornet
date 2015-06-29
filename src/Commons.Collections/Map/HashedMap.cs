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
using Commons.Collections.Collection;
using Commons.Collections.Set;
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
            : this(DefaultCapacity, comparer)
        {
        }

        public HashedMap(Equator<K> equator) : this(DefaultCapacity, new EquatorComparer<K>(equator))
        {
        }

        public HashedMap(int capacity) : this(capacity, EqualityComparer<K>.Default)
        {
        }

        public HashedMap(int capacity, IEqualityComparer<K> comparer)
        {
			Guarder.CheckNull(comparer);
			if (capacity <= 0)
			{
				throw new ArgumentException("The capacity must be larger than zero.");
			}
			this.comparer = comparer;
			this.capacity = CalculateCapacity(capacity);
			Count = 0;
			entries = new Entry[this.capacity];
        }

        public HashedMap(int capacity, Equator<K> equator) : this(capacity, new EquatorComparer<K>(equator))
        {
        }

        public HashedMap(IDictionary<K, V> items, IEqualityComparer<K> comparer) : this(DefaultCapacity, comparer)
        {
			if (items != null)
			{ 
				foreach (var item in items)
				{
					Add(item.Key, item.Value);
				}
			}
        }

        public HashedMap(IDictionary<K, V> items) : this(items, EqualityComparer<K>.Default)
        {
        }

        public HashedMap(IDictionary<K, V> items, Equator<K> equator) : this(items, new EquatorComparer<K>(equator))
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
			if (Count >= threshold)
			{
				Inflate();
			}
			Enroute(key, value);
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
                if (element.Occupied)
                {
                    Enroute(element.Key, element.Value);
                }
            }
		}
		
		public bool ContainsKey(K key)
		{
            Guarder.CheckNull(key);
			var index = FindEntry(key);
			var entry = entries[index];

            return entry.Occupied;
		}

		public ICollection<K> Keys
		{
			get
			{
				var set = new HashedSet<K>(comparer);
				foreach (var entry in entries)
				{
                    if (entry.Occupied)
					{
						set.Add(entry.Key);
					}
				}

				return set;
			}
		}

		public bool Remove(K key)
		{
            Guarder.CheckNull(key);
            var index = FindEntry(key);
			var cursor = index;
			if (!entries[index].Occupied)
			{
				return false;
			}
			for (; ; )
			{
				cursor = (cursor + 1) & (capacity - 1);
				if (!entries[cursor].Occupied)
				{
					break;
				}

				var next = entries[cursor].GetHashCode() & (capacity - 1);
				if ((cursor > index && (next <= index || index > cursor)) || (cursor < index && (next <= index || next > cursor)))
				{
					entries[index] = entries[cursor];
					index = cursor;
				}
			}
			entries[index].Occupied = false;
			entries[index].Key = default(K);
			entries[index].Value = default(V);

            return true;
        }

		public bool TryGetValue(K key, out V value)
		{
            Guarder.CheckNull(key);
            var index = FindEntry(key);
			var entry = entries[index];
            value = entry.Value;

            return entry.Occupied;
		}

		public ICollection<V> Values
		{
			get
			{
				var set = new HashedSet<V>();
				foreach (var entry in entries)
				{
                    if (entry.Occupied)
                    {
                        set.Add(entry.Value);
                    }
				}

				return set;
			}
		}

		public V this[K key]
		{
			get
			{
                var index = FindEntry(key);

				var entry = entries[index];
				if (!entry.Occupied)
				{
					throw new KeyNotFoundException("The key does not exist in the map.");
				}

                return entry.Value;
			}
			set
			{
                var index = FindEntry(key);
				var entry = entries[index];
                if (!entry.Occupied)
                {
                    throw new KeyNotFoundException("The key does not exist in the map.");
                }

                entries[index].Value = value;
			}
		}

		public void Add(KeyValuePair<K, V> item)
		{
			Add(item.Key, item.Value);
		}

		public void Clear()
		{
			entries = new Entry[capacity];
			Count = 0;
		}

		public bool Contains(KeyValuePair<K, V> item)
		{
			var contains = false;
			if (ContainsKey(item.Key))
			{
				if (Equals(this[item.Key], item.Value))
				{
					contains = true;
				}
			}

			return contains;
		}

		public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
		{
			Guarder.CheckNull(array);
			var index = 0;
			foreach (var element in entries)
			{
                if (element.Occupied)
				{
					array[arrayIndex + (index++)] = new KeyValuePair<K, V>(element.Key, element.Value);
				}
			}

		}

		public int Count
		{
			get;
			private set;
		}

		public bool IsReadOnly
		{
			get { return false; }
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
		
		private IEnumerable<KeyValuePair<K, V>> Items
		{
			get
			{
				foreach (var element in entries)
				{
                    if (element.Occupied)
					{
						yield return new KeyValuePair<K, V>(element.Key, element.Value);
					}
				}
			}
		}

		public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
		{
			return Items.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
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
				var set = new HashedSet<K>(comparer);
				foreach(var element in Keys)
				{
					set.Add(element);
				}

				return set;
			}
		}

		void IDictionary.Remove(object key)
		{
			Remove((K)key);
		}

		ICollection IDictionary.Values
		{
			get
			{
				var set = new HashedSet<V>();
				foreach(var element in Values)
				{
					set.Add(element);
				}

				return set;
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
			var itemArray = (KeyValuePair<K, V>[])array;
			CopyTo(itemArray, index);
		}

		int ICollection.Count
		{
			get { return Count; }
		}

		bool ICollection.IsSynchronized
		{
			get { return false;}
		}

		object ICollection.SyncRoot
		{
			get { throw new NotSupportedException("The SyncRoot is not supported in Commons.Collections"); }
		}

#if NET45
		bool IReadOnlyDictionary<K, V>.ContainsKey(K key)
		{
			return ContainsKey(key);
		}

		IEnumerable<K> IReadOnlyDictionary<K, V>.Keys
		{
			get { return Keys; }
		}

		bool IReadOnlyDictionary<K, V>.TryGetValue(K key, out V value)
		{
			return TryGetValue(key, out value);
		}

		IEnumerable<V> IReadOnlyDictionary<K, V>.Values
		{
			get { return Values; }
		}

		V IReadOnlyDictionary<K, V>.this[K key]
		{
			get { return this[key]; }
		}

		int IReadOnlyCollection<KeyValuePair<K, V>>.Count
		{
			get { return Count; }
		}
#endif

		IEnumerator<KeyValuePair<K, V>> IEnumerable<KeyValuePair<K, V>>.GetEnumerator()
		{
			return GetEnumerator();
		}

		private struct Entry
		{
			public bool Occupied { get; set; }
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

        private int FindEntry(K key)
        {
            var index = HashIndex(key);
            while (entries[index].Occupied && !comparer.Equals(entries[index].Key, key))
            {
                index = (index + 1) & (capacity - 1);
            }

            return index;
        }

		private void Enroute(K key, V value)
		{
            var index = FindEntry(key);
            if (entries[index].Occupied)
            {
                throw new ArgumentException("The key item already exist in the map.");
            }
            entries[index].Key = key;
            entries[index].Value = value;
            entries[index].Occupied = true;
            Count++;
        }

	}
}
