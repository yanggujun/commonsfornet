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
	public class LruMap<K, V> : AbstractLinkedMap<K, V>, IBoundedMap<K, V>, IDictionary<K, V>
	{
		private const int DefaultFullSize = 100;

		private int FullSize { get; set; }

		public LruMap() : this(DefaultFullSize)
		{
		}

		public LruMap(int fullSize) : this(fullSize, (x1, x2) => x1 == null ? x2 == null : x1.Equals(x2))
		{
		}

		public LruMap(int fullSize, Equator<K> equator) : base(fullSize, equator)
		{
			FullSize = fullSize;
		}

		public override void Add(K key, V value)
		{
			base.Add(key, value);
			if (Count > FullSize)
			{
				if (!ReferenceEquals(Header, Header.Before))
				{
					Remove(Header.Before.Key);
				}
			}
		}

		public bool IsFull
		{
			get { return Count >= FullSize; }
		}

		public override IEnumerator<KeyValuePair<K, V>> GetEnumerator()
		{
			return CreateUnorderedEnumerator().GetEnumerator();
		}

		protected override HashEntry CreateEntry(K key, V value)
		{
			var entry = new LinkedHashEntry(key, value);
			if (Header == null)
			{
				Header = entry;
				Header.Before = Header.After = Header;
			}
			else
			{
				entry.Before = Header.Before;
				entry.After = Header;
				Header.Before.After = entry;
				Header.Before = entry;
				Header = entry;
			}

			return entry;
		}

		protected override V Get(K key)
		{
			var entry = (LinkedHashEntry) GetEntry(key);
			entry.Validate(x => x != null, new ArgumentException(string.Format("The key {0} does not exist in the map.", key)));
			MoveToFirst(entry);

			return entry.Value;
		}

		protected override void Set(K key, V v)
		{
			var entry = (LinkedHashEntry) GetEntry(key);
			entry.Validate(x => x != null, new ArgumentException(string.Format("The key {0} does not exist in the map.", key)));
			entry.Value = v;
			MoveToFirst(entry);
		}

		private void MoveToFirst(LinkedHashEntry entry)
		{
			if (!ReferenceEquals(entry, Header))
			{ 
				entry.Before.After = entry.After;
				entry.After.Before = entry.Before;
				entry.After = Header;
				entry.Before = Header.Before;
				Header.Before.After = entry;
				Header.Before = entry;
				Header = entry;
			}
		}
	}
}
