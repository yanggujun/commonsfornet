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
using System.Collections;
using System.Collections.Generic;
using Commons.Collections.Set;

namespace Commons.Collections.Map
{
	[CLSCompliant(true)]
	public interface IBimap<K, V> : ICollection<KeyValuePair<K, V>>, IEnumerable<KeyValuePair<K, V>>, IEnumerable
	{
		void Add(K key, V value);

		void RemoveKey(K key);

		void RemoveValue(V value);

		bool ContainsKey(K key);

		bool ContainsValue(V value);

		bool TryGetValue(K key, out V value);

		bool TryGetKey(V value, out K key);

		V ValueOf(K key);

		K KeyOf(V value);

		IBimap<V, K> Inverse();

		IStrictSet<K> KeySet();

		IStrictSet<V> ValueSet();
	}
}
