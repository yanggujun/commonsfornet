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

using System.Collections.Generic;
using Commons.Utils;

namespace Commons.Collections.Map
{
	public class MultiHashMap<K, V> : AbstractMultiMap<K, V>, IMultiMap<K, V>
	{
		public MultiHashMap() : base(new HashMap<K, ICollection<V>>(), null, EqualityComparer<V>.Default.Equals)
		{
		}

		public MultiHashMap(int keyCapacity) : this(keyCapacity, EqualityComparer<K>.Default, EqualityComparer<V>.Default)
		{
		}

		public MultiHashMap(int keyCapacity, IEqualityComparer<K> keyCompaer) : this(keyCapacity, keyCompaer, EqualityComparer<V>.Default)
		{
		}

		public MultiHashMap(IEqualityComparer<K> keyComparer, IEqualityComparer<V> valueComparer) 
			: base(new HashMap<K, ICollection<V>>(keyComparer.Equals), null, valueComparer.Equals)
		{
		}

		public MultiHashMap(int keyCapacity, IEqualityComparer<K> keyComparer, IEqualityComparer<V> valueComparer) 
			: this(null, keyCapacity, keyComparer.Equals, valueComparer.Equals)
		{
		}
		public MultiHashMap(IEnumerable<KeyValuePair<K, V>> items, int keyCapacity, Equator<K> keyEquator, Equator<V> valueEquator) 
			: base(new HashMap<K, ICollection<V>>(keyCapacity, keyEquator), items, valueEquator)
		{
		}
	}
}
