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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commons.Pool
{
    /// <summary>
    /// Object pool where the object can be retrieved by its key.
    /// </summary>
    /// <typeparam name="T">The object type.</typeparam>
    internal interface IKeyedObjectPool<K, T> : ICountable, IDisposable where T : class
    {
        /// <summary>
        /// Acquires an object from the pool by the key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <returns>An object if it's avaialable.</returns>
        T Acquire(K key);

        /// <summary>
        /// Tries to acquire an object from the pool by the key.
        /// </summary>
        /// <param name="timeout">Time to wait for the object. If the value is less than or equal to 0, it waits infinitively if there is never an object returned to pool.</param>
        /// <param name="key">The key of the object.</param>
        /// <param name="obj">The object retrieved from pool.</param>
        /// <returns>True if the object is retrieved from the pool. Otherwise false.</returns>
        bool TryAcquire(long timeout, K key, out T obj);

        /// <summary>
        /// Returns the object with its key to the pool. If the key does not exit/never created by the pool or the object
        /// already exist in the pool, <exception cref="InvalidOperationException"></exception> is thrown. 
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="obj">The object reference.</param>
        void Return(K key, T obj);
    }
}
