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
    public class AbstractLinkedMap<K, V> : AbstractHashedMap<K, V>
    {
        protected LinkedHashEntry Header { get; set; }

        public AbstractLinkedMap(int capacity, Equator<K> isEqual)
            : base(capacity, isEqual)
        {
        }

        protected override long HashIndex(K key)
        {
            return key.GetHashCode() & (Capacity - 1);
        }

        public override bool Remove(K key)
        {
            Guarder.CheckNull(key);
            var removed = false;
			var entry = GetEntry(key) as LinkedHashEntry;
            if (entry != null)
            {
                if (base.Remove(key))
                {
                    if (ReferenceEquals(entry, Header))
                    {
                        Header = entry.After;
                    }
                    entry.Before.After = entry.After;
                    entry.After.Before = entry.Before;
                    removed = true;
                }
            }
            return removed;
        }

        public override IEnumerator<KeyValuePair<K, V>> GetEnumerator()
        {
			return LinkedItems.GetEnumerator();
        }

        protected override HashEntry CreateEntry(K key, V value)
        {
            var linkedEntry = new LinkedHashEntry(key, value);
            if (Header == null)
            {
                Header = linkedEntry;
                Header.Before = Header.After = Header;
            }
            else
            {
                linkedEntry.After = Header;
                linkedEntry.Before = Header.Before;
                Header.Before.After = linkedEntry;
                Header.Before = linkedEntry;
            }

            return linkedEntry;
        }

		private IEnumerable<KeyValuePair<K, V>> LinkedItems
		{
			get
			{
				var cursor = Header;
				if (null != cursor)
				{
					do
					{
						yield return new KeyValuePair<K, V>(cursor.Key, cursor.Value);
						cursor = cursor.After;
					} while (!ReferenceEquals(cursor, Header));
				}
			}
		}

        protected class LinkedHashEntry : HashEntry
        {
            public LinkedHashEntry(K key, V value) : base(key, value)
            {
            }
            public LinkedHashEntry Before { get; set; }
            public LinkedHashEntry After { get; set; }
        }
    }
}
