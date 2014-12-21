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
        private const int MaxLevel = 32;
        private const double P = 0.5;
		private readonly Comparison<K> comparer;
		private LinkedNode header;
        private Random rand;

		public SkipList(Comparison<K> comparer)
		{
            comparer.ValidateNotNull("The comparer is null.");
			this.comparer = comparer;
			header = new LinkedNode(1);
            rand = new Random((int)DateTime.Now.Ticks & 0x0000ffff);
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
            key.ValidateNotNull("The key is null.");
            var cursor = header;
            var update = new LinkedNode[MaxLevel];
            for (var i = cursor.Level; i > 0; i--)
            {
                while (cursor.Forward(i) != null)
                {
                    if (comparer(cursor.Forward(i).Key, key) >= 0)
                    {
                        break;
                    }
                    cursor = cursor.Forward(i);
                    update[i] = cursor;
                }
            }
            cursor = cursor.Forward(1);
            if (comparer(cursor.Key, key) == 0)
            {
                throw new ArgumentException("The key to add already exists.");
            }

            var level = RandomLevel();
            if (level > header.Level)
            {
                for (var i = header.Level + 1; i <= level; i++)
                {
                    update[i] = header;
                }
                header.Level = level;
            }
            var newNode = new LinkedNode(level);
            for (var i = 0; i < level; i++)
            {
                newNode.NewForward(i + 1, update[i].Forward(i + 1));
                update[i].NewForward(i + 1, newNode);
            }
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

        private int RandomLevel()
        {
            var level = 1;
            while (rand.NextDouble() < P && level < MaxLevel)
            {
                level++;
            }
            return level;
        }

		private class LinkedNode
		{
			private LinkedNode[] forwards;

			public LinkedNode(int level)
			{
				forwards = new LinkedNode[MaxLevel];
                Level = level;
			}

			public LinkedNode Forward(int onLevel)
			{
				return forwards[onLevel - 1];
			}

            public void NewForward(int onLevel, LinkedNode newNode)
            {
                forwards[onLevel - 1] = newNode;
            }

			public K Key { get; set; }

			public V Value { get; set; }

            public int Level { get; set; }
		}
	}
}
