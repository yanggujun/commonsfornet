// Copyright CommonsForNET.  // Licensed to the Apache Software Foundation (ASF) under one or more
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

namespace Commons.Collections.Map
{
    public class SortedBimap<K, V> : AbstractBimap<K, V>, IBimap<K, V>, INavigableMap<K, V>, 
        ISortedMap<K, V>, IDictionary<K, V>, IReadOnlyBimap<K, V>, IReadOnlyDictionary<K, V>
    {
        private readonly Comparison<K> keyComparer;
        private readonly Comparison<V> valueComparer;

        public SortedBimap()
            : this(Comparer<K>.Default.Compare, Comparer<V>.Default.Compare)
        {
        }

        public SortedBimap(Comparison<K> keyComparer)
            : this(keyComparer, Comparer<V>.Default.Compare)
        {
        }

        public SortedBimap(IComparer<K> keyComparer)
            : this(keyComparer, Comparer<V>.Default)
        {
        }

        public SortedBimap(IComparer<K> keyComparer, IComparer<V> valueComparer)
            : this(keyComparer.Compare, valueComparer.Compare)
        {
        }

        public SortedBimap(Comparison<K> keyComparer, Comparison<V> valueComparer)
            : base(new TreeMap<K, V>(keyComparer), new TreeMap<V, K>(valueComparer))
        {
            this.keyComparer = keyComparer;
            this.valueComparer = valueComparer;
        }

        public SortedBimap(Comparison<K> keyComparer, Comparison<V> valueComparer, IBimap<K, V> bimap) 
            : this(keyComparer, valueComparer)
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
            var bimap = new SortedBimap<V, K>(valueComparer, keyComparer);
            foreach (var item in ValueKey)
            {
                bimap.Add(item);
            }

            return bimap;
        }

        public override Set.IStrictSet<K> KeySet()
        {
            var set = new Set.TreeSet<K>(keyComparer);
            foreach (var item in KeyValue)
            {
                set.Add(item.Key);
            }

            return set;
        }

        public override Set.IStrictSet<V> ValueSet()
        {
            var set = new Set.TreeSet<V>(valueComparer);
            foreach (var item in ValueKey)
            {
                set.Add(item.Key);
            }

            return set;
        }

        public KeyValuePair<K, V> Lower(K key)
        {
            return ((TreeMap<K, V>)KeyValue).Lower(key);
        }

        public KeyValuePair<K, V> Higher(K key)
        {
            return ((TreeMap<K, V>)KeyValue).Higher(key);
        }

        public KeyValuePair<K, V> Ceiling(K key)
        {
            return ((TreeMap<K, V>)KeyValue).Ceiling(key);
        }

        public KeyValuePair<K, V> Floor(K key)
        {
            return ((TreeMap<K, V>)KeyValue).Floor(key);
        }

        public Set.INavigableSet<K> NavigableKeySet
        {
            get { return ((TreeMap<K, V>)KeyValue).NavigableKeySet; }
        }

        public KeyValuePair<K, V> Max
        {
            get { return ((TreeMap<K, V>)KeyValue).Max; }
        }

        public KeyValuePair<K, V> Min
        {

            get { return ((TreeMap<K, V>)KeyValue).Min; }
        }

        public void RemoveMax()
        {
            var map = (TreeMap<K, V>)KeyValue;
            var reversed = (TreeMap<V, K>)ValueKey;
            var max = map.Max;
            map.RemoveMax();
            reversed.Remove(max.Value);
        }

        public void RemoveMin()
        {
            var map = (TreeMap<K, V>)KeyValue;
            var reversed = (TreeMap<V, K>)ValueKey;
            var min = map.Min;
            map.RemoveMin();
            reversed.Remove(min.Value);
        }

        public bool IsEmpty
        {
            get { return ((TreeMap<K, V>)KeyValue).IsEmpty; }
        }

        public Set.ISortedSet<K> SortedKeySet
        {
            get
            {
                return ((TreeMap<K, V>)KeyValue).SortedKeySet;
            }
        }
    }
}
