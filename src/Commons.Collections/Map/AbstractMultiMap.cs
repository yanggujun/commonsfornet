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
	public abstract class AbstractMultiMap<K, V> : IMultiMap<K, V>, ICollection<KeyValuePair<K, V>>, ICollection,
		IReadOnlyMultiMap<K, V>, IReadOnlyCollection<KeyValuePair<K, V>>, IEnumerable<KeyValuePair<K, V>>, 
        IEnumerable<KeyValuePair<K, ICollection<V>>>, IEnumerable
	{
		private readonly Equator<V> valueEquator;

		protected IDictionary<K, ICollection<V>> Map { get; private set; }

		protected AbstractMultiMap(IDictionary<K, ICollection<V>> map, IEnumerable<KeyValuePair<K, V>> items, Equator<V> valueEquator)
		{
			this.valueEquator = valueEquator;
			this.Map = map;
			if (items != null)
			{ 
				foreach (var item in items)
				{
					Add(item.Key, item.Value);
				}
			}
		}

		public void Add(K key, V value)
		{
			key.ValidateNotNull("The key is null.");
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

		public bool RemoveItem(K key, V value)
		{
			var removed = false;
			if (Map.ContainsKey(key))
			{
				foreach (var v in Map[key])
				{
					if (valueEquator(v, value))
					{
						removed = Map[key].Remove(v);
					}
				}
			}

			return removed;
		}

		public bool TryGetValue(K key, out ICollection<V> values)
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

			return count;
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

		public void Add(KeyValuePair<K, V> item)
		{
			Add(item.Key, item.Value);
		}

		public void Clear()
		{
			Map.Clear();
		}

		public bool Contains(KeyValuePair<K, V> item)
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

		public int Count
		{
			get
			{
				var count = 0;
				foreach(var kvp in Map)
				{
					foreach(var v in kvp.Value)
					{
						count++;
					}
				}

				return count;
			}
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool Remove(KeyValuePair<K, V> item)
		{
			return RemoveItem(item.Key, item.Value);
		}

		public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
		{
			return SingleItems.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void CopyTo(Array array, int index)
		{
			var theArray = array as KeyValuePair<K, V>[];
			CopyTo(theArray, index);
		}

		public bool IsSynchronized
		{
			get { return false; }
		}

		public object SyncRoot
		{
			get { throw new NotSupportedException("The SyncRoot is not supported in Commons.Collections."); }
		}

		IEnumerator<KeyValuePair<K, ICollection<V>>> IEnumerable<KeyValuePair<K, ICollection<V>>>.GetEnumerator()
		{
			return CollectionItems.GetEnumerator();
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
