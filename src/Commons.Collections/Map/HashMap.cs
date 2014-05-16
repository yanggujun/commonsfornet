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

using System.Collections;
using System.Collections.Generic;
using System;

using Commons.Collections.Common;

namespace Commons.Collections.Map
{
    [CLSCompliant(true)]
    public class HashMap<K, V> : AbstractHashMap<K, V>
    {
        private const int DefaultCapacity = 64;
        private const int MaxCapacity = 1 << 30;
        private const double LoadFactor = 0.75f;

        private readonly Transformer<K, byte[]> transform;

        private IHashStrategy hasher;

        public HashMap() : this(DefaultCapacity)
        {
        }

        public HashMap(int capacity)
            : this(capacity, new MurmurHash32(), x => x.ToString().ToBytes(), EqualityComparer<K>.Default)
        {
        }

        public HashMap(int capacity, Transformer<K, byte[]> transformer)
            : this(capacity, new MurmurHash32(), transformer, EqualityComparer<K>.Default)
        {
        }

        public HashMap(IHashStrategy hasher, Transformer<K, byte[]> transformer)
            : this(DefaultCapacity, hasher, transformer, EqualityComparer<K>.Default)
        {
        }

        public HashMap(IHashStrategy hasher, Transformer<K, byte[]> transformer, Equator<K> isEqual) 
            : this(DefaultCapacity, hasher, transformer, isEqual)
        {
        }

        public HashMap(IHashStrategy hasher, Transformer<K, byte[]> transformer, IEqualityComparer<K> comparer)
            : this(DefaultCapacity, hasher, transformer, comparer)
        { 
        }

        public HashMap(int capacity, IHashStrategy hasher, Transformer<K, byte[]> transformer, Equator<K> isEqual)
            : this(capacity, null, hasher, transformer, isEqual)
        {
        }

        public HashMap(int capacity, IHashStrategy hasher, Transformer<K, byte[]> transformer, IEqualityComparer<K> comparer) 
            : this(capacity, hasher, transformer, (x1, x2) => null == comparer ? EqualityComparer<K>.Default.Equals(x1, x2) : comparer.Equals(x1, x2))
        {
        }

        public HashMap(int capacity, IEnumerable<KeyValuePair<K, V>> items, IHashStrategy hasher, Transformer<K, byte[]> transformer, Equator<K> isEqual)
            : base(capacity, items, isEqual)
        {
            Guarder.CheckNull(hasher, transformer, isEqual);
            this.transform = transformer;
            this.hasher = hasher;
        }

        private uint Hash(K key)
        {
            var bytes = transform(key);
            var hash = hasher.Hash(bytes);
            return (uint)hash[0];
        }

        protected override int CalculateCapacity(int proposedCapacity)
        {
            int newCapacity = 1;
            if (proposedCapacity > MaxCapacity)
            {
                newCapacity = MaxCapacity;
            }
            else
            {
                while (newCapacity < proposedCapacity)
                {
                    newCapacity <<= 1;
                }
                Threshold = (int)(newCapacity * LoadFactor);
                while (proposedCapacity > Threshold)
                {
                    newCapacity <<= 1;
                    Threshold = (int)(newCapacity * LoadFactor);
                }
                newCapacity = newCapacity > MaxCapacity ? MaxCapacity : newCapacity;
            }

            return newCapacity;
        }

        protected override long HashIndex(K key)
        {
            var hash = Hash(key);
            return hash & (Capacity - 1);
        }

    }
}
