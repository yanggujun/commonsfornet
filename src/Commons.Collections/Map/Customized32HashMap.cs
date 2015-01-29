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

using System.Collections.Generic;
using System;
using System.Text;
using Commons.Utils;

namespace Commons.Collections.Map
{
    [CLSCompliant(true)]
    public class Customized32HashMap<K, V> : AbstractHashedMap<K, V>, IDictionary<K, V>, IReadOnlyDictionary<K, V>
    {
        private const int DefaultCapacity = 64;

        private readonly Transformer<K, byte[]> transform;

        private readonly IHashStrategy hasher;

        public Customized32HashMap() : this(DefaultCapacity)
        {
        }

        public Customized32HashMap(int capacity)
            : this(capacity, x => Encoding.ASCII.GetBytes(x.ToString()))
        {
        }

        public Customized32HashMap(int capacity, Transformer<K, byte[]> transformer)
            : this(capacity, new MurmurHash32(), transformer, (x1, x2) => x1 == null ? x2 == null : x1.Equals(x2))
        {
        }

        public Customized32HashMap(int capacity, Transformer<K, byte[]> transformer, Equator<K> isEqual)
            : this(capacity, new MurmurHash32(), transformer, isEqual)
        {
        }

        public Customized32HashMap(int capacity, IHashStrategy hasher, Transformer<K, byte[]> transformer, Equator<K> isEqual)
            : this(capacity, null, hasher, transformer, isEqual)
        {
        }

        public Customized32HashMap(int capacity, IEnumerable<KeyValuePair<K, V>> items, IHashStrategy hasher, Transformer<K, byte[]> transformer, Equator<K> isEqual)
            : base(capacity, isEqual)
        {
            Guarder.CheckNull(hasher, transformer, isEqual);
            this.transform = transformer;
            this.hasher = hasher;
            if (null != items)
            {
                foreach (var item in items)
                {
                    Add(item);
                }
            }
        }

        private long Hash(K key)
        {
            var bytes = transform(key);
            var hash = hasher.Hash(bytes);
            return hash[0];
        }

        protected override long HashIndex(K key)
        {
            var hash = Hash(key);
            return hash & (Capacity - 1);
        }

    }
}
