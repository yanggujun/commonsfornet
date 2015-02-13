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
using Commons.Utils;
namespace Commons.Collections.Map
{
	[CLSCompliant(true)]
	public class MultiValueTreeMap<K, V> : AbstractMultiValueMap<K, V>, IMultiValueMap<K, V>, IReadOnlyMultiValueMap<K, V>
	{
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

		private MultiValueTreeMap(IMultiValueMap<K, V> items, ISortedMap<K, ICollection<V>> map, Equator<V> valueEquator) 
			: base(items, map, valueEquator)
		{
		}
	}
}
