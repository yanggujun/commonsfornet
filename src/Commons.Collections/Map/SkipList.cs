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
        private const string KeyIsNull = "The key to search is null.";
        private const int MaxLevel = 32;
        private const double P = 0.5;
        private LinkedNode header;
        private Random rand;

        public SkipList(Comparison<K> comparer)
        {
            comparer.ValidateNotNull("The comparer is null.");
            this.Comparer = comparer;
            header = new LinkedNode(1);
            rand = new Random((int)DateTime.Now.Ticks & 0x0000ffff);
        }

        public Comparison<K> Comparer { get; private set; }

        public bool Contains(K key)
        {
            key.ValidateNotNull(KeyIsNull);
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
                var node = FindNode(key);
                if (node == null)
                {
                    Add(key, value);
                }
                else
                {
                    node.Value = value;
                }
            }
        }

        public void Add(K key, V value)
        {
            key.ValidateNotNull("The key to add is null.");
            var update = new LinkedNode[MaxLevel];
            var cursor = GetLeastKeyLargerThanOrEqualTo(key, update);
            if (cursor != null && Comparer(cursor.Key, key) == 0)
            {
                throw new ArgumentException("The key to add already exists.");
            }

            var level = RandomLevel();
            if (level > header.Level)
            {
                for (var i = header.Level; i < level; i++)
                {
                    update[i] = header;
                }
                header.Level = level;
            }
            var newNode = new LinkedNode(level) { Key = key, Value = value };
            for (var i = 0; i < level; i++)
            {
                newNode.SetForward(i + 1, update[i].GetForward(i + 1));
                update[i].SetForward(i + 1, newNode);
            }
        }

        public bool Remove(K key)
        {
            key.ValidateNotNull("The key to remove is null.");
            var update = new LinkedNode[MaxLevel];
            var cursor = GetLeastKeyLargerThanOrEqualTo(key, update);
            var removed = false;
            if (cursor != null && Comparer(cursor.Key, key) == 0)
            {
                for (var i = 0; i < header.Level; i++)
                {
                    if (update[i].GetForward(i + 1) == null || Comparer(update[i].GetForward(i + 1).Key, key) != 0)
                    {
                        break;
                    }
                    update[i].SetForward(i + 1, cursor.GetForward(i + 1));
                }
                TrimHeader();
                removed = true;
            }
            return removed;
        }

        public bool IsEmpty
        {
            get
            {
                return header.GetForward(1) == null;
            }
        }

        public KeyValuePair<K, V> Lower(K key)
        {
            key.ValidateNotNull(KeyIsNull);
            ValidateNotEmpty();
            var node = GetLargestKeyLessThan(key);
            node.Validate(x => x != null, new ArgumentException(string.Format("No item is lower than the key {0}", key)));
            return new KeyValuePair<K, V>(node.Key, node.Value);
        }

        public KeyValuePair<K, V> Higher(K key)
        {
            key.ValidateNotNull(KeyIsNull);
            ValidateNotEmpty();
            var node = GetLeastKeyLargerThan(key);
            node.Validate(x => x != null, new ArgumentException(string.Format("No item is higher than the key {0}", key)));
            return new KeyValuePair<K, V>(node.Key, node.Value);
        }

        public KeyValuePair<K, V> Ceiling(K key)
        {
            key.ValidateNotNull(KeyIsNull);
            ValidateNotEmpty();
            var node = GetLeastKeyLargerThanOrEqualTo(key);
            node.Validate(x => x != null, new ArgumentException(string.Format("No item is the ceiling of the key {0}", key)));
            return new KeyValuePair<K, V>(node.Key, node.Value);
        }

        public KeyValuePair<K, V> Floor(K key)
        {
            key.ValidateNotNull(KeyIsNull);
            ValidateNotEmpty();
            var node = GetLargestKeyLessThanOrEqualTo(key);
            node.Validate(x => x != null, new ArgumentException(string.Format("No item is the floor of the key {0}", key)));
            return new KeyValuePair<K, V>(node.Key, node.Value);
        }

        public KeyValuePair<K, V> Min
        {
            get
            {
                ValidateNotEmpty();
                var min = header.GetForward(1);
                return new KeyValuePair<K, V>(min.Key, min.Value);
            }
        }

        public KeyValuePair<K, V> Max
        {
            get
            {
                ValidateNotEmpty();
                var cursor = header;
                for (var i = header.Level; i > 0; i--)
                {
                    while (cursor.GetForward(i) != null)
                    {
                        cursor = cursor.GetForward(i);
                    }
                }

                return new KeyValuePair<K, V>(cursor.Key, cursor.Value);
            }
        }

        public void RemoveMin()
        {
            ValidateNotEmpty();
            var min = header.GetForward(1);
            for (var i = 0; i < header.Level; i++)
            {
                if (header.GetForward(i + 1) == null || Comparer(min.Key, header.GetForward(i + 1).Key) != 0)
                {
                    break;
                }
                header.SetForward(i + 1, min.GetForward(i + 1));
            }
            TrimHeader();
        }

        public void RemoveMax()
        {
            ValidateNotEmpty();
            Remove(Max.Key);
        }

        public int Count
        {
            get
            {
                var count = 0;
                var cursor = header.GetForward(1);
                while (cursor != null)
                {
                    cursor = cursor.GetForward(1);
                    count++;
                }

                return count;
            }
        }

        public void Clear()
        {
            header = new LinkedNode(1);
        }

        public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private IEnumerable<KeyValuePair<K, V>> Items
        {
            get
            {
                var cursor = header;
                while (cursor.GetForward(1) != null)
                {
                    cursor = cursor.GetForward(1);
                    yield return new KeyValuePair<K, V>(cursor.Key, cursor.Value);
                }
            }
        }

        private void TrimHeader()
        {
            while (header.Level > 1 && header.GetForward(header.Level - 1) == null)
            {
                header.Level--;
            }
        }

        private LinkedNode GetLeastKeyLargerThanOrEqualTo(K key, LinkedNode[] update)
        {
            var cursor = header;
            for (var i = header.Level; i > 0; i--)
            {
                while (cursor.GetForward(i) != null)
                {
                    if (Comparer(cursor.GetForward(i).Key, key) >= 0)
                    {
                        break;
                    }
                    cursor = cursor.GetForward(i);
                }
                update[i - 1] = cursor;
            }
            return cursor.GetForward(1);
        }

        private LinkedNode GetLargestKeyLessThan(K key)
        {
            var cursor = header;
            for (var i = header.Level; i > 0; i--)
            {
                while (cursor.GetForward(i) != null)
                {
                    if (Comparer(cursor.GetForward(i).Key, key) >= 0)
                    {
                        break;
                    }
                    cursor = cursor.GetForward(i);
                }
            }
            return ReferenceEquals(cursor, header) ? null : cursor;
        }

        private LinkedNode GetLargestKeyLessThanOrEqualTo(K key)
        {
            var cursor = header;
            for (var i = header.Level; i > 0; i--)
            {
                while (cursor.GetForward(i) != null)
                {
                    if (Comparer(cursor.GetForward(i).Key, key) > 0)
                    {
                        break;
                    }
                    cursor = cursor.GetForward(i);
                }
            }

            return ReferenceEquals(cursor, header) ? null : cursor;
        }

        private LinkedNode GetLeastKeyLargerThan(K key)
        {
            var cursor = header;
            for (var i = header.Level; i > 0; i--)
            {
                while (cursor.GetForward(i) != null)
                {
                    if (Comparer(cursor.GetForward(i).Key, key) > 0)
                    {
                        break;
                    }
                    cursor = cursor.GetForward(i);
                }
            }

            return cursor.GetForward(1);
        }

        private LinkedNode GetLeastKeyLargerThanOrEqualTo(K key)
        {
            var cursor = header;
            for (var i = header.Level; i > 0; i--)
            {
                while (cursor.GetForward(i) != null)
                {
                    if (Comparer(cursor.GetForward(i).Key, key) >= 0)
                    {
                        break;
                    }
                    cursor = cursor.GetForward(i);
                }
            }

            return cursor.GetForward(1);
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
                while (cursor.GetForward(i) != null)
                {
                    if (Comparer(cursor.GetForward(i).Key, key) >= 0)
                    {
                        break;
                    }
                    cursor = cursor.GetForward(i);
                }
                var forward = cursor.GetForward(i);
                if (forward != null && Comparer(forward.Key, key) == 0)
                {
                    node = cursor.GetForward(i);
                    break;
                }
            }
            if (node != null && Comparer(cursor.GetForward(1).Key, key) == 0)
            {
                node = cursor.GetForward(1);
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

        private void ValidateNotEmpty()
        {
            header.GetForward(1).Validate(x => x != null, new InvalidOperationException("The collection is empty."));
        }

        private class LinkedNode
        {
            private readonly LinkedNode[] forwards;

            public LinkedNode(int level)
            {
                forwards = new LinkedNode[MaxLevel];
                Level = level;
            }

            public LinkedNode GetForward(int onLevel)
            {
                return forwards[onLevel - 1];
            }

            public void SetForward(int onLevel, LinkedNode newNode)
            {
                forwards[onLevel - 1] = newNode;
            }

            public K Key { get; set; }

            public V Value { get; set; }

            public int Level { get; set; }
        }
    }
}
