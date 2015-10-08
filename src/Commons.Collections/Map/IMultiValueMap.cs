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

namespace Commons.Collections.Map
{
    /// <summary>
    /// A multi value map provides a table view for a set of data with unfixed columns. 
    /// </summary>
    /// <typeparam name="K">The type of the key.</typeparam>
    /// <typeparam name="V">The type of the value.</typeparam>
    [CLSCompliant(true)]
    public interface IMultiValueMap<K, V> : ICollection<KeyValuePair<K, V>>, IEnumerable<KeyValuePair<K, V>>, IEnumerable
    {
        /// <summary>
        /// Add a value to the row identified by the key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void Add(K key, V value);

        /// <summary>
        /// Add a collection of values to the row identified by the key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="values"></param>
        void Add(K key, ICollection<V> values);

        /// <summary>
        /// Removes a row identified by a key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool Remove(K key);

        /// <summary>
        /// Removes specific value in the row key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool RemoveValue(K key, V value);

        /// <summary>
        /// Whether containin the row of the key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool ContainsKey(K key);

        /// <summary>
        /// Whether a value is in the row of the key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool ContainsValue(K key, V value);

        /// <summary>
        /// Tries to get all the values from the row of key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        bool TryGetValue(K key, out List<V> values);

        /// <summary>
        /// The count of the values in the row of key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        int CountOf(K key);

        /// <summary>
        /// The count of the keys.
        /// </summary>
        int KeyCount { get; }

        /// <summary>
        /// Retrieves a collection of values of the row of the key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        ICollection<V> this[K key] { get; }

        /// <summary>
        /// All the keys in the multi value map.
        /// </summary>
        ICollection<K> Keys { get; }

        /// <summary>
        /// All the values in the multi value map.
        /// </summary>
        ICollection<V> Values { get; }
    }
}
