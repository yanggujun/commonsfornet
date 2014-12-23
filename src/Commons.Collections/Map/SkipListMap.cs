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
using Commons.Collections.Set;

namespace Commons.Collections.Map
{
	public class SkipListMap<K, V> : ISortedMap<K, V>, INavigableMap<K, V>, IDictionary<K, V>, IEnumerable<KeyValuePair<K, V>>, IEnumerable, ICollection
	{
		private readonly SkipList<K, V> skipList;

		public SkipListMap() : this (Comparer<K>.Default)
		{
		}

		public SkipListMap(Comparer<K> comparer) : this (comparer.Compare)
		{
		}

		public SkipListMap(Comparison<K> comparer)
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

		public ISortedSet<K> KeySet
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
			return skipList.Remove(key);
		}

		public bool TryGetValue(K key, out V value)
		{
			var exist = false;
			if (skipList.Contains(key))
			{
				value = skipList[key];
				exist = true;
			}
			else
			{
				value = default(V);
			}
			return exist;
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
			skipList.Add(item.Key, item.Value);
		}

		public void Clear()
		{
			skipList.Clear();
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
			get { return skipList.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool Remove(KeyValuePair<K, V> item)
		{
			throw new System.NotImplementedException();
		}

		public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
		{
			return skipList.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
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
