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
using Commons.Collections.Collection;
using Commons.Utils;

namespace Commons.Collections.Map
{
    internal class ChainHashedMap<K, V> : AbstractHashedMap<K, V>, IDictionary<K, V>
#if NET45
        , IReadOnlyDictionary<K, V>
#endif
    {
        private const int DefaultCapacity = 16;

        public ChainHashedMap() : this(DefaultCapacity)
        {
        }

        public ChainHashedMap(IEqualityComparer<K> comparer)
            : this(comparer.Equals)
        {
        }

        public ChainHashedMap(Equator<K> equator) : this(DefaultCapacity, equator)
        {
        }

        public ChainHashedMap(int capacity) : this(capacity, EqualityComparer<K>.Default)
        {
        }

        public ChainHashedMap(int capacity, IEqualityComparer<K> comparer)
            : this(capacity, comparer.Equals)
        {
        }

        public ChainHashedMap(int capacity, Equator<K> equator) : base(capacity, new EquatorComparer<K>(equator))
        {
        }

        public ChainHashedMap(IDictionary<K, V> items, IEqualityComparer<K> comparer) : this(items, comparer.Equals)
        {
        }

        public ChainHashedMap(IDictionary<K, V> items) : this(items, EqualityComparer<K>.Default.Equals)
        {
        }

        public ChainHashedMap(IDictionary<K, V> items, Equator<K> equator)
            : base(items == null ? DefaultCapacity : items.Count, new EquatorComparer<K>(equator))
        {
            if (null != items)
            {
                foreach (var item in items)
                {
                    Add(item.Key, item.Value);
                }
            }
        }

        protected override long HashIndex(K key)
        {
            var hash = key.GetHashCode();
            return hash & (Capacity - 1);
        }
    }
}
