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

namespace Commons.Collections.Map
{
    [CLSCompliant(true)]
    public interface IOrderedMap<K, V> : IDictionary<K, V>, ICollection<KeyValuePair<K, V>>, IEnumerable<KeyValuePair<K, V>>, IEnumerable
    {
        KeyValuePair<K, V> First { get; }

        KeyValuePair<K, V> Last { get; }

        KeyValuePair<K, V> After(K key);

        KeyValuePair<K, V> Before(K key);

        /// <summary>
        /// Get the item in the specified index of the ordered map.
        /// </summary>
        /// <param name="index">The index of the item, starting from 0.</param>
        /// <returns>The item on the index.</returns>
        KeyValuePair<K, V> GetIndex(int index);
    }
}
