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
    /// The interface maintains the pools used in the application. Ideally the one application shall 
    /// only have one pool manager. And different pools can be created/destroyed by the pool manager.
    /// It is recommended that the manager is registered to the IoC container of the application as an singleton instance 
    /// and can be accessed in the application to create and destroy different object pools.
    /// </summary>
    [CLSCompliant(true)]
    public interface IPoolManager : IDisposable
    {

        /// <summary>
        /// Attempts to instantiate a new object pool by creating a <see cref="IPoolDescriptor{T}"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IPoolDescriptor<T> NewPoolOf<T>() where T : class;

        /// <summary>
        /// Retrieves an object pool with the <paramref name="key"/>. When the key does not exist in the pool manager,
        /// it returns null.
        /// </summary>
        /// <typeparam name="T">The type of the pooled object.</typeparam>
        /// <param name="key">The key of the object pool.</param>
        /// <returns>The an existing generic object pool.</returns>
        IObjectPool<T> GetPoolOf<T>(string key) where T : class;

        /// <summary>
        /// Destroys an object pool with the key. When the key does not exist in the pool manager, <exception cref="ArgumentException"></exception> is thrown.
        /// </summary>
        /// <param name="key">The key of the object pool.</param>
        void Destroy(string key);
    }
}
