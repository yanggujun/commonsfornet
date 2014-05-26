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
using System.Collections.Generic;

using Commons.Collections.Common;

namespace Commons.Collections.Map
{
    [CLSCompliant(true)]
    public class HashMap<K, V> : AbstractHashMap<K, V>
    {
        private const int DefaultCapacity = 16;

        public HashMap() : this(DefaultCapacity)
        {
        }

        public HashMap(int capacity) : this(capacity, EqualityComparer<K>.Default)
        {
        }

        public HashMap(int capacity, IEqualityComparer<K> comparer) 
            : this(capacity, (x1, x2) => comparer == null ? EqualityComparer<K>.Default.Equals(x1, x1) : comparer.Equals(x1, x2))
        {
        }

        public HashMap(int capacity, Equator<K> equator) : base(capacity, equator)
        {
        }

        public HashMap(IEnumerable<KeyValuePair<K, V>> items, int capacity, Equator<K> equator)
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

        protected override long HashIndex(K key)
        {
            var hash = (uint) key.GetHashCode();
            return hash & (Capacity - 1);
        }
    }
}
