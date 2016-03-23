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
using System.Collections.Generic;
using Commons.Utils;

namespace Commons.Collections.Map
{
    [CLSCompliant(true)]
    public class LinkedHashedMap<K, V> : AbstractLinkedMap<K, V>, IOrderedMap<K, V>, IDictionary<K, V>
#if NET45
        , IReadOnlyDictionary<K, V>
#endif
    {
        private const int DefaultCapacity = 16;

        public LinkedHashedMap()
            : this(DefaultCapacity)
        {
        }

        public LinkedHashedMap(int capacity)
            : this(capacity, EqualityComparer<K>.Default)
        {
        }

        public LinkedHashedMap(IEqualityComparer<K> comparer) 
            : this(DefaultCapacity, comparer)
        {
        }

        public LinkedHashedMap(Equator<K> equator) 
            : this(DefaultCapacity, equator)
        {
        }

        public LinkedHashedMap(int capacity, Equator<K> equator)
            : base(capacity, equator)
        {
        }

        public LinkedHashedMap(int capacity, IEqualityComparer<K> comparer) 
            : this(capacity, comparer.Equals)
        {
        }

        /// <summary>
        /// Currently disable the constructor with a predefined dictionary, as the order is not defined in the dictionary.
        /// This may cause some confusion.
        /// </summary>
        /// <param name="items"></param>
        /// <param name="comparer"></param>
        private LinkedHashedMap(IDictionary<K, V> items, IEqualityComparer<K> comparer) : this(items, comparer.Equals)
        {
        }

        private LinkedHashedMap(IDictionary<K, V> items, Equator<K> equator)
            : this(items == null ? DefaultCapacity : items.Count, equator)
        {
            if (null != items)
            {
                foreach (var item in items)
                {
                    Add(item.Key, item.Value);
                }
            }
        }

        public KeyValuePair<K, V> First
        {
            get
            {
                CheckEmpty("get first item");
                return new KeyValuePair<K, V>(Header.Key, Header.Value);
            }
        }

        public KeyValuePair<K, V> Last
        {
            get
            {
                CheckEmpty("get last item");
                return new KeyValuePair<K, V>(Header.Before.Key, Header.Before.Value);
            }
        }

        public KeyValuePair<K, V> After(K key)
        {
            Guarder.CheckNull(key);
            CheckEmpty("get After item");

            var entry = GetEntry(key) as LinkedHashEntry;
            if (entry == null)
            {
                throw new ArgumentException("The key does not exist.");
            }
            if (IsEqual(entry.Key, Header.Before.Key))
            {
                throw new ArgumentException("There is no more items after the searched key.");
            }

            return new KeyValuePair<K, V>(entry.After.Key, entry.After.Value);
        }

        public KeyValuePair<K, V> Before(K key)
        {
            Guarder.CheckNull(key);
            CheckEmpty("get Before item");
            var entry = GetEntry(key) as LinkedHashEntry;
            if (entry == null)
            {
                throw new ArgumentException("The key does not exist.");
            }
            if (IsEqual(entry.Key, Header.Key))
            {
                throw new ArgumentException("There is no item before the searched key.");
            }

            return new KeyValuePair<K, V>(entry.Before.Key, entry.Before.Value);
        }
 
        public KeyValuePair<K, V> GetIndex(int index)
        {
            if (index < 0)
            {
                throw new ArgumentException("The order cannot be less than zero");
            }
            if (index >= Count)
            {
                throw new ArgumentException("The order is larger than the item count");
            }
            CheckEmpty("get item from index");

            var cursor = Header;
            if (index < (Count / 2))
            {
                for (var i = 0; i < index; i++)
                {
                    cursor = cursor.After;
                }
            }
            else
            {
                for (var i = Count; i > Count - index; i--)
                {
                    cursor = cursor.Before;
                }
            }

            return new KeyValuePair<K, V>(cursor.Key, cursor.Value);
        }

        protected override void Rehash()
        {
            if (Header != null)
            {
                var newEntries = new HashEntry[Capacity];
                Count = 0;
                var cursor = Header;
                var oldHeader = Header;
                Header = null;
                Put(newEntries, CreateEntry(cursor.Key, cursor.Value));
                cursor = cursor.After;
                while (!ReferenceEquals(cursor, oldHeader))
                {
                    Put(newEntries, CreateEntry(cursor.Key, cursor.Value));
                    cursor = cursor.After;
                }
                Entries = newEntries;
            }
        }

        private void CheckEmpty(string message)
        {
            Header.Validate(x => x != null, 
                new InvalidOperationException(string.Format("Invalid to {0}, the map is empty", message)));
        }
    }
}
