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

namespace Commons.Collections.Map
{
	public class ListMap<K, V> : IOrderedMap<K, V>, INavigableMap<K, V>, IDictionary<K, V>, IEnumerable<KeyValuePair<K, V>>, IEnumerable, ICollection
	{
		private readonly SkipList<K, V> skipList;

		public ListMap() : this (Comparer<K>.Default)
		{
		}

		public ListMap(Comparer<K> comparer) : this (comparer.Compare)
		{
		}

		public ListMap(Comparison<K> comparer)
		{
			skipList = new SkipList<K, V>(comparer);
		}

		public KeyValuePair<K, V> Lower(K key)
		{
			throw new System.NotImplementedException();
		}

		public KeyValuePair<K, V> Higher(K key)
		{
			throw new System.NotImplementedException();
		}

		public KeyValuePair<K, V> Ceiling(K key)
		{
			throw new System.NotImplementedException();
		}

		public KeyValuePair<K, V> Floor(K key)
		{
			throw new System.NotImplementedException();
		}

		public Set.ISortedSet<K> KeySet
		{
			get { throw new System.NotImplementedException(); }
		}

		public KeyValuePair<K, V> Max
		{
			get { throw new System.NotImplementedException(); }
		}

		public KeyValuePair<K, V> Min
		{
			get { throw new System.NotImplementedException(); }
		}

		public void RemoveMax()
		{
			throw new System.NotImplementedException();
		}

		public void RemoveMin()
		{
			throw new System.NotImplementedException();
		}

		public void Add(K key, V value)
		{
			skipList.Add(key, value);
		}

		public bool ContainsKey(K key)
		{
			return skipList.Contains(key);
		}

		public ICollection<K> Keys
		{
			get { throw new System.NotImplementedException(); }
		}

		public bool Remove(K key)
		{
			throw new System.NotImplementedException();
		}

		public bool TryGetValue(K key, out V value)
		{
			throw new System.NotImplementedException();
		}

		public ICollection<V> Values
		{
			get { throw new System.NotImplementedException(); }
		}

		public V this[K key]
		{
			get
			{
				return skipList[key];
			}
			set
			{
				skipList[key] = value;
			}
		}

		public void Add(KeyValuePair<K, V> item)
		{
			throw new System.NotImplementedException();
		}

		public void Clear()
		{
			throw new System.NotImplementedException();
		}

		public bool Contains(KeyValuePair<K, V> item)
		{
			throw new System.NotImplementedException();
		}

		public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
		{
			throw new System.NotImplementedException();
		}

		public int Count
		{
			get { throw new System.NotImplementedException(); }
		}

		public bool IsReadOnly
		{
			get { throw new System.NotImplementedException(); }
		}

		public bool Remove(KeyValuePair<K, V> item)
		{
			throw new System.NotImplementedException();
		}

		public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
		{
			throw new System.NotImplementedException();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new System.NotImplementedException();
		}

		public KeyValuePair<K, V> First
		{
			get { throw new System.NotImplementedException(); }
		}

		public KeyValuePair<K, V> Last
		{
			get { throw new System.NotImplementedException(); }
		}

		public KeyValuePair<K, V> After(K key)
		{
			throw new System.NotImplementedException();
		}

		public KeyValuePair<K, V> Before(K key)
		{
			throw new System.NotImplementedException();
		}

		public KeyValuePair<K, V> GetIndex(int index)
		{
			throw new System.NotImplementedException();
		}

		public void CopyTo(System.Array array, int index)
		{
			throw new System.NotImplementedException();
		}

		public bool IsSynchronized
		{
			get { throw new System.NotImplementedException(); }
		}

		public object SyncRoot
		{
			get { throw new System.NotImplementedException(); }
		}
	}
}
