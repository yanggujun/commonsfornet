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
    public class MultiValueTreeMap<K, V> : AbstractMultiValueMap<K, V>, ISortedMultiValueMap<K, V>, IMultiValueMap<K, V>, IReadOnlyMultiValueMap<K, V>
    {
        private readonly TreeMap<K, ICollection<V>> map;
        public MultiValueTreeMap() : this(Comparer<K>.Default)
        {
        }

        public MultiValueTreeMap(IComparer<K> keyComparer) : this(keyComparer, EqualityComparer<V>.Default)
        {
        }

        public MultiValueTreeMap(Comparison<K> keyComparer) : this(keyComparer, EqualityComparer<V>.Default.Equals)
        {
        }

        public MultiValueTreeMap(IComparer<K> keyComparer, IEqualityComparer<V> valueComparer)
            : this(keyComparer.Compare, valueComparer.Equals)
        {
        }

        public MultiValueTreeMap(Comparison<K> keyComparer, Equator<V> valueEquator) 
            : this(null, new TreeMap<K, ICollection<V>>(keyComparer), valueEquator)
        {
        }

        public MultiValueTreeMap(IMultiValueMap<K, V> mvMap) 
            : this(mvMap, Comparer<K>.Default.Compare, EqualityComparer<V>.Default.Equals)
        {
        }

        public MultiValueTreeMap(IMultiValueMap<K, V> mvMap, Comparison<K> keyComparer, Equator<V> valueEquator) 
            : this (mvMap, new TreeMap<K, ICollection<V>>(keyComparer), valueEquator)
        {
        }

        private MultiValueTreeMap(IMultiValueMap<K, V> items, TreeMap<K, ICollection<V>> map, Equator<V> valueEquator) 
            : base(new EquatorComparer<V>(valueEquator))
        {
            this.map = map;

            if (items != null)
            {
                foreach (var item in items)
                {
                    Add(item.Key, item.Value);
                }
            }
        }

        public K Max
        {
            get { return map.Max.Key; }
        }

        public K Min
        {
            get { return map.Min.Key; }
        }

        public void RemoveMax()
        {
            map.RemoveMax();
        }

        public void RemoveMin()
        {
            map.RemoveMin();
        }

        public bool IsEmpty
        {
            get { return map.IsEmpty; }
        }

        protected override IDictionary<K, ICollection<V>> Map
        {
            get { return map; }
        }
    }
}
