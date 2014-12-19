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

using Commons.Utils;

namespace Commons.Collections.Map
{
	internal class SkipList<K, V> : IEnumerable<KeyValuePair<K, V>>
	{
		private readonly Comparison<K> comparer;
		private LinkedNode header;

		public SkipList(Comparison<K> comparer)
		{
			this.comparer = comparer;
			header = new LinkedNode(1);
		}

		public bool Contains(K key)
		{
			key.ValidateNotNull("The key to search is null.");
			return FindNode(key) != null;
		}

		public V this[K key]
		{
			get
			{
				return GetNode(key).Value;
			}
			set
			{
				GetNode(key).Value = value;
			}
		}

		public void Add(K key, V value)
		{
		}

		public bool Remove(K key)
		{
			return false;
		}

		public void Clear()
		{
		}

		public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
		{
			throw new NotImplementedException();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotImplementedException();
		}

		private LinkedNode GetNode(K key)
		{
			key.ValidateNotNull("The key is null.");
			var node = FindNode(key);
			node.Validate(x => x != null, new KeyNotFoundException("The key cannot be found."));

			return node;
		}

		private LinkedNode FindNode(K key)
		{
			var cursor = header;
			LinkedNode node = null;
			for (var i = header.Level; i > 0; i--)
			{
				while (cursor.Forward(i) != null)
				{
					if (comparer(cursor.Forward(i).Key, key) >= 0)
					{
						break;
					}
					cursor = cursor.Forward(i);
				}
				if (comparer(cursor.Forward(i).Key, key) == 0)
				{
					node = cursor.Forward(i);
					break;
				}
			}
			if (node == null && comparer(cursor.Forward(1).Key, key) == 0)
			{
				node = cursor.Forward(1);
			}

			return node;
		}

		private class LinkedNode
		{
			private LinkedNode[] forwards;

			public LinkedNode(int level)
			{
				forwards = new LinkedNode[level];
			}

			public LinkedNode Forward(int onLevel)
			{
				forwards.Validate(x => x != null, new ArgumentException("The node is not initialized."));
				onLevel.Validate(x => x < forwards.Length, 
					new ArgumentException(
						string.Format("The level {0} exceeds the max level of the current node.", onLevel)));
				return forwards[onLevel - 1];
			}

			public K Key { get; set; }

			public V Value { get; set; }

			public int Level
			{
				get
				{
					return forwards == null ? 0 : forwards.Length;
				}
			}
		}
	}
}
