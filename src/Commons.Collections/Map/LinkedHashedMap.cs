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
    public class LinkedHashedMap<K, V> : AbstractLinkedMap<K, V>, IOrderedMap<K, V>, IDictionary<K, V>, IReadOnlyDictionary<K, V>
    {
        private const int DefaultCapacity = 16;

        public LinkedHashedMap()
            : this(DefaultCapacity)
        {
        }

        public LinkedHashedMap(int capacity)
            : this(capacity, (x1, x2) => x1 == null ? x2 == null : x1.Equals(x2))
        {
        }

        public LinkedHashedMap(int capacity, IEnumerable<KeyValuePair<K, V>> items)
            : this(capacity, items, (x1, x2) => x1 == null ? x2 == null : x1.Equals(x2))
        {
        }

        public LinkedHashedMap(int capacity, Equator<K> equator)
            : this(capacity, null, equator)
        {
        }

        public LinkedHashedMap(int capacity, IEnumerable<KeyValuePair<K, V>> items, Equator<K> equator)
            : base(capacity, equator)
        {
            if (null != items)
            {
                foreach (var item in items)
                {
                    Add(item);
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
