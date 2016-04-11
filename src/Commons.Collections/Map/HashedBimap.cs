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
using Commons.Collections.Set;
using Commons.Utils;

namespace Commons.Collections.Map
{
    [CLSCompliant(true)]
    public class HashedBimap<K, V> :AbstractBimap<K, V>, IBimap<K, V>, IDictionary<K, V>, IReadOnlyBimap<K, V>
#if NET45
        , IReadOnlyDictionary<K, V>
#endif
    {
        private const int DefaultCapacity = 16;
        private readonly IEqualityComparer<K> keyEquator;
        private readonly IEqualityComparer<V> valueEquator;

        public HashedBimap()
            : this(EqualityComparer<K>.Default, EqualityComparer<V>.Default)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyComparer"></param>
        /// <remarks>If the <typeparamref name="K"/> is the same with <typeparamref name="V"/>, need to use named 
        /// arguments to specify it's a <paramref name="keyComparer"/></remarks>
        public HashedBimap(IEqualityComparer<K> keyComparer)
            : this(keyComparer, EqualityComparer<V>.Default)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="valueComparer"></param>
        /// <remarks>If the <typeparamref name="V"/> is the same with <typeparamref name="K"/>, need to use named 
        /// arguments to specify it's a <paramref name="valueComparer"/></remarks>
        public HashedBimap(IEqualityComparer<V> valueComparer) 
            : this(EqualityComparer<K>.Default, valueComparer)
        {
        }

        public HashedBimap(IEqualityComparer<K> keyComparer, IEqualityComparer<V> valueComparer) 
            : base(new HashedMap<K, V>(keyComparer), new HashedMap<V, K>(valueComparer))
        {
            keyEquator = keyComparer;
            valueEquator = valueComparer;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyEquator"></param>
        /// <remarks>If the <typeparamref name="K"/> is the same with <typeparamref name="V"/>, need to use named 
        /// arguments to specify it's a <paramref name="keyEquator"/></remarks>
        public HashedBimap(Equator<K> keyEquator)
            : this(new EquatorComparer<K>(keyEquator), EqualityComparer<V>.Default)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="valueEquator"></param>
        /// <remarks>If the <typeparamref name="V"/> is the same with <typeparamref name="K"/>, need to use named 
        /// arguments to specify it's a <paramref name="valueEquator"/></remarks>
        public HashedBimap(Equator<V> valueEquator) 
            : this(EqualityComparer<K>.Default, new EquatorComparer<V>(valueEquator))
        {
        }

        public HashedBimap(Equator<K> keyEquator, Equator<V> valueEquator)
            : this(new EquatorComparer<K>(keyEquator), new EquatorComparer<V>(valueEquator))
        {
        }

        public HashedBimap(int capacity, IEqualityComparer<K> keyComparer, IEqualityComparer<V> valueComparer)
            : base(new HashedMap<K, V>(capacity, keyComparer), new HashedMap<V, K>(capacity, valueComparer))
        {
            keyEquator = keyComparer;
            valueEquator = valueComparer;
        }

        public HashedBimap(int capacity, Equator<K> keyEquator, Equator<V> valueEquator)
            : this(capacity, new EquatorComparer<K>(keyEquator), new EquatorComparer<V>(valueEquator))
        {
        }

        public HashedBimap(IBimap<K, V> bimap) 
            : this(bimap, EqualityComparer<K>.Default.Equals, EqualityComparer<V>.Default.Equals)
        {
        }

        public HashedBimap(IBimap<K, V> bimap, Equator<K> keyEquator, Equator<V> valueEquator)
            : this(bimap == null ? DefaultCapacity : bimap.Count, new EquatorComparer<K>(keyEquator), new EquatorComparer<V>(valueEquator))
        {
            if (bimap != null)
            {
                foreach (var item in bimap)
                {
                    Add(item.Key, item.Value);
                }
            }
        }

        protected override bool HasItem(KeyValuePair<K, V> item)
        {
            return KeyValue.ContainsKey(item.Key) && valueEquator.Equals(item.Value, ValueOf(item.Key)) 
                && ContainsValue(item.Value) && keyEquator.Equals(item.Key, KeyOf(item.Value));
        }

        public override IBimap<V, K> Inverse()
        {
            var bimap = new HashedBimap<V, K>(valueEquator, keyEquator);
            foreach (var item in ValueKey)
            {
                bimap.Add(item.Key, item.Value);
            }

            return bimap;
        }

        public override IStrictSet<K> KeySet()
        {
            var set = new HashedSet<K>(keyEquator);

            foreach (var item in KeyValue)
            {
                set.Add(item.Key);
            }

            return set;
        }

        public override IStrictSet<V> ValueSet()
        {
            var set = new HashedSet<V>(valueEquator);

            foreach (var item in ValueKey)
            {
                set.Add(item.Key);
            }

            return set;
        }
    }
}
