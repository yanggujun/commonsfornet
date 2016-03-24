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
    [CLSCompliant(true)]
    public class MultiValueHashedMap<K, V> : AbstractMultiValueMap<K, V>, IMultiValueMap<K, V>, IReadOnlyMultiValueMap<K, V>
    {
        private readonly HashedMap<K, ICollection<V>> map;
        private const int DefaultCapacity = 16;

        public MultiValueHashedMap() : this(DefaultCapacity)
        {
        }

        public MultiValueHashedMap(int keyCapacity) : this(keyCapacity, EqualityComparer<K>.Default, EqualityComparer<V>.Default)
        {
        }

        public MultiValueHashedMap(int keyCapacity, IEqualityComparer<K> keyCompaer) : this(keyCapacity, keyCompaer, EqualityComparer<V>.Default)
        {
        }

        public MultiValueHashedMap(IEqualityComparer<K> keyComparer, IEqualityComparer<V> valueComparer) 
            : this(DefaultCapacity, keyComparer, valueComparer)
        {
        }

        public MultiValueHashedMap(int keyCapacity, IEqualityComparer<K> keyComparer, IEqualityComparer<V> valueComparer) 
            : base(valueComparer)
        {
			map = new HashedMap<K, ICollection<V>>(keyCapacity, keyComparer);
        }

        public MultiValueHashedMap(Equator<K> keyEquator, Equator<V> valueEquator) 
            : this(DefaultCapacity, keyEquator, valueEquator)
        {
        }

        public MultiValueHashedMap(int keyCapacity, Equator<K> keyEquator, Equator<V> valueEquator)
            : this(keyCapacity, new EquatorComparer<K>(keyEquator), new EquatorComparer<V>(valueEquator))
        {
        }

	    public MultiValueHashedMap(IMultiValueMap<K, V> items) : this(items, EqualityComparer<K>.Default, EqualityComparer<V>.Default)
	    {
	    }

        public MultiValueHashedMap(IMultiValueMap<K, V> items, Equator<K> keyEquator, Equator<V> valueEquator) 
			: this(items, new EquatorComparer<K>(keyEquator), new EquatorComparer<V>(valueEquator)) 
        {
        }

        public MultiValueHashedMap(IMultiValueMap<K, V> items, IEqualityComparer<K> keyComparer, IEqualityComparer<V> valueComparer) : base(valueComparer)
        {
	        map = new HashedMap<K, ICollection<V>>(
				items == null ? DefaultCapacity : items.KeyCount, keyComparer);

            if (items != null)
            {
                foreach (var item in items)
                {
                    Add(item.Key, item.Value);
                }
            }
        }

        protected override IDictionary<K, ICollection<V>> Map
        {
            get { return map; }
        }
    }
}
