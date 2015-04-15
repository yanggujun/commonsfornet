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
    /// A bidirectional map where a key can be retrieved from a value and a value can be retrieved from a key.
    /// </summary>
    /// <typeparam name="K">Type of the key.</typeparam>
    /// <typeparam name="V">Type of the value.</typeparam>
    [CLSCompliant(true)]
    public interface IBimap<K, V> : ICollection<KeyValuePair<K, V>>, IEnumerable<KeyValuePair<K, V>>, IEnumerable
    {
        /// <summary>
        /// Adds a key value mapping to the bimap. The operation is not allowed when the key or value already exits in the bimap.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        void Add(K key, V value);

        /// <summary>
        /// No matter whether the key or value exists in the bimap, the key and value are added to the bimap.
        /// If the key or value already exits in the map, remove the original key or value in the bimap.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        void Enforce(K key, V value);

        /// <summary>
        /// Remove a key value pair from a key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>True if a key value pair is removed.</returns>
        bool RemoveKey(K key);

        /// <summary>
        /// Remove a key value pair from a value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>True if a key value pair is removed.</returns>
        bool RemoveValue(V value);

        /// <summary>
        /// Whether the map contains a key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>True if the key exists.</returns>
        bool ContainsKey(K key);

        /// <summary>
        /// Whether the map contains a value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>True if the value exists.</returns>
        bool ContainsValue(V value);

        /// <summary>
        /// Tries to get a value from its corresponding key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The result value.</param>
        /// <returns>True if the key value pair exists.</returns>
        bool TryGetValue(K key, out V value);

        /// <summary>
        /// Tries to get a key from its corresponding value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="key">The result key.</param>
        /// <returns>True if the key value pair exists.</returns>
        bool TryGetKey(V value, out K key);

        /// <summary>
        /// Retrieves the value of a key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The value corresponding to the key.</returns>
        V ValueOf(K key);

        /// <summary>
        /// Retrieves the key of the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The key corresponding to the value.</returns>
        K KeyOf(V value);

        /// <summary>
        /// Inverse the bidirectional map with its key to be the value of the new bidirectional map 
        /// and its value to be the key of the new bidirectional map.
        /// </summary>
        /// <returns>A new bidirectional map.</returns>
        IBimap<V, K> Inverse();

        /// <summary>
        /// Retrieves a set of the keys.
        /// </summary>
        /// <returns>A set.</returns>
        IStrictSet<K> KeySet();

        /// <summary>
        /// Retrieves a set of the values.
        /// </summary>
        /// <returns>A set.</returns>
        IStrictSet<V> ValueSet();
    }
}
