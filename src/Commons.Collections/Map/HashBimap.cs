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
    [CLSCompliant(true)]
    public class HashBimap<K, V> :AbstractBimap<K, V>, IBimap<K, V>, IDictionary<K, V>, IReadOnlyBimap<K, V>, IReadOnlyDictionary<K, V>
    {
        private readonly Equator<K> keyEquator;
        private readonly Equator<V> valueEquator;
        public HashBimap()
            : this(EqualityComparer<K>.Default.Equals, EqualityComparer<V>.Default.Equals)
        {
        }

        public HashBimap(IEqualityComparer<K> keyComparer)
            : this(keyComparer.Equals, EqualityComparer<V>.Default.Equals)
        {
        }

        public HashBimap(IEqualityComparer<K> keyComparer, IEqualityComparer<V> valueComparer)
            : this(keyComparer.Equals, valueComparer.Equals)
        {
        }

        public HashBimap(Equator<K> keyEquator)
            : this(keyEquator, EqualityComparer<V>.Default.Equals)
        {
        }

        public HashBimap(Equator<K> keyEquator, Equator<V> valueEquator)
            : base(new HashMap<K, V>(keyEquator), new HashMap<V, K>(valueEquator))
        {
            this.keyEquator = keyEquator;
            this.valueEquator = valueEquator;
        }

        public HashBimap(int capacity, IEqualityComparer<K> keyComparer, IEqualityComparer<V> valueComparer)
            : this(capacity, keyComparer.Equals, valueComparer.Equals)
        {
        }

        public HashBimap(int capacity, Equator<K> keyEquator, Equator<V> valueEquator)
            : base(new HashMap<K, V>(capacity, keyEquator), new HashMap<V, K>(capacity, valueEquator))
        {
            this.keyEquator = keyEquator;
            this.valueEquator = valueEquator;
        }

        public HashBimap(int capacity, Equator<K> keyEquator, Equator<V> valueEquator, IBimap<K, V> bimap) 
            : this(capacity, keyEquator, valueEquator)
        {
            if (bimap != null)
            {
                foreach (var item in bimap)
                {
                    Add(item);
                }
            }
        }

        public override IBimap<V, K> Inverse()
        {
            var bimap = new HashBimap<V, K>(valueEquator, keyEquator);
            foreach (var item in ValueKey)
            {
                bimap.Add(item);
            }

            return bimap;
        }

        public override Set.IStrictSet<K> KeySet()
        {
            var set = new Set.HashSet<K>(keyEquator);

            foreach (var item in KeyValue)
            {
                set.Add(item.Key);
            }

            return set;
        }

        public override Set.IStrictSet<V> ValueSet()
        {
            var set = new Set.HashSet<V>(valueEquator);

            foreach (var item in ValueKey)
            {
                set.Add(item.Key);
            }

            return set;
        }
    }
}
