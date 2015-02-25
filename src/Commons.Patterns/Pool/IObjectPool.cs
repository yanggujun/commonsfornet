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

namespace Commons.Patterns.Pool
{
	/// <summary>
	/// The interface defines the operations for an object pool.
	/// </summary>
	/// <typeparam name="T">The type of the pooled object.</typeparam>
    [CLSCompliant(true)]
	public interface IObjectPool<T> : IDisposable
	{
		/// <summary>
		/// Acquires an object from the pool. 
		/// If there is any object available in the pool, one of the idle objects is returned
		/// from the pool. And the idle object is marked as allocated.
		/// If there is not any idle object in the pool, and the pool size is less than the max value,
		/// the object pool attempts to create an object and returns the object if successful. Pool size is increased.
		/// If there is not any idle object in the pool and the pool size has reached the max value, 
		/// <see cref="ResourceUnavailableException"/> is thrown.
		/// </summary>
		/// <returns>The object</returns>
		T Acquire();

        /// <summary>
        /// Returns an <paramref name="obj"/> to the pool. The object becomes idle when it's returned to the pool.
        /// </summary>
        /// <param name="obj">The returned object</param>
		void Return(T obj);

        /// <summary>
        /// Asks the pool to create a new object instance and add the object into the pool.
        /// If the pool has already reaches the maximum size, <see cref="ExceedLimitException"/> is thrown.
        /// </summary>
        void AddObject();

        /// <summary>
        /// When the operations on the object fail, need to call this method to tell the 
        /// pool that the object state is invalidated. And the object shall be abandoned.
        /// </summary>
        /// <param name="obj">The object to invalidate.</param>
        void Invalidate(T obj);

        /// <summary>
        /// The number of the idle objects which can be acquired from the pool.
        /// </summary>
        int IdleCount { get; }

        /// <summary>
        /// The number of the active objects, which means the objects acquired out of the pool.
        /// </summary>
        int ActiveCount { get; }

        /// <summary>
        /// The maximum number of all the objects created in the pool.
        /// </summary>
        int MaxSize { get; }

        /// <summary>
        /// The configuration for the pool.
        /// </summary>
        PoolConfig PoolConfiguration { get; }

        /// <summary>
        /// Clears the pool. And diposes the objects in the pool.
        /// </summary>
        void Clear();
	}
}
