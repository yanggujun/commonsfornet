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
    /// <summary>
    /// The stub interface of map.
    /// </summary>
    /// <typeparam name="K">Type of the key</typeparam>
    /// <typeparam name="V">Type of the value</typeparam>
    [CLSCompliant(true)]
    public interface ISortedMap<K, V> : IDictionary<K, V>, ICollection<KeyValuePair<K, V>>, IEnumerable<KeyValuePair<K, V>>, IEnumerable
    {
        /// <summary>
        /// Puts the keys of the map to a tree set.
        /// </summary>
        ISortedSet<K> KeySet { get; }

        /// <summary>
        /// Return a key value pair whose key is the maximum in the map.
        /// </summary>
        KeyValuePair<K, V> Max { get; }

        /// <summary>
        /// Return a key value pair whose key is the minimum in the map.
        /// </summary>
        KeyValuePair<K, V> Min { get; }

        /// <summary>
        /// Removes maximum item in the map.
        /// </summary>
        void RemoveMax();

        /// <summary>
        /// Removes minimum item in the map.
        /// </summary>
        void RemoveMin();

        bool IsEmpty { get; }
    }
}
