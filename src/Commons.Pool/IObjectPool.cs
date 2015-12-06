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

namespace Commons.Pool
{
    /// <summary>
    /// The interface defines the operations for an object pool.
    /// </summary>
    /// <typeparam name="T">The type of the pooled object.</typeparam>
    [CLSCompliant(true)]
    public interface IObjectPool<T> : IDisposable where T : class
    {
        /// <summary>
        /// Acquires an object from the pool. 
        /// If there is any object available in the pool, one of the idle objects is returned
        /// from the pool. 
        /// If there is not any idle object in the pool, and the pool size is less than the max value,
        /// the object pool attempts to create an object and returns the object if successful. Pool size is increased.
        /// If there is not any idle object in the pool and the pool size has reached the max value, it waits infinitively.
        /// </summary>
        /// <returns>The object</returns>
        T Acquire();

        /// <summary>
        /// Acquires an object from the pool. If there is no object available in the pool, it waits until timeout. If the time out is less than or equal to 0, 
        /// it waits for the object infinitively.
        /// </summary>
        /// <param name="timeout">The timeout in milli-seconds.</param>
        /// <param name="obj">The object from the pool.</param>
        /// <returns>True if any idle object is retrieved from the pool, otherwise false.</returns>
        bool TryAcquire(int timeout, out T obj);

        /// <summary>
        /// Returns an <paramref name="obj"/> to the pool. The object becomes idle when it's returned to the pool.
        /// </summary>
        /// <param name="obj">The returned object</param>
        void Return(T obj);

        /// <summary>
        /// The number of the idle objects which can be acquired from the pool.
        /// </summary>
        int IdleCount { get; }

        /// <summary>
        /// The number of the active objects, which means the objects acquired out of the pool.
        /// </summary>
        int ActiveCount { get; }

        /// <summary>
        /// The maximum number of all the objects created in the pool. The capacity must be larger than or equal to <see cref="InitialSize"/>.
        /// </summary>
        int Capacity { get; }

        /// <summary>
        /// The initial size of the pool. If the pool grows, its size eventually reaches the <see cref="Capacity"/>;
        /// </summary>
        int InitialSize { get; }
    }
}
