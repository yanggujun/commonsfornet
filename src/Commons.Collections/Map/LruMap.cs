// Copyright CommonsForNET.
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
using Commons.Collections.Collection;
using Commons.Utils;

namespace Commons.Collections.Map
{
    [CLSCompliant(true)]
    public class LruMap<K, V> : AbstractLinkedMap<K, V>, IDictionary<K, V>, 
#if NET45
		IReadOnlyDictionary<K, V>, 
#endif
		IBoundedCollection
    {
        private const int DefaultFullSize = 100;

        public int MaxSize { get; private set; }

        public LruMap() : this(DefaultFullSize)
        {
        }

        public LruMap(int fullSize) : this(fullSize, EqualityComparer<K>.Default)
        {
        }

        public LruMap(int fullSize, IEqualityComparer<K> comparer) : this(fullSize, comparer.Equals)
        {
        }

        public LruMap(int fullSize, Equator<K> equator) : base(fullSize, equator)
        {
            MaxSize = fullSize;
        }

        public override void Add(K key, V value)
        {
            base.Add(key, value);
            if (Count > MaxSize)
            {
                if (!ReferenceEquals(Header, Header.Before))
                {
                    Remove(Header.Before.Key);
                }
            }
        }

        public bool IsFull
        {
            get { return Count >= MaxSize; }
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
            if (entry == null)
            {
                Add(key, v);
            }
            else
            {
                entry.Value = v;
                MoveToFirst(entry);
            }
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
