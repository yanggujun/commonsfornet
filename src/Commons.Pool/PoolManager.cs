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
using System.Collections.Concurrent;
using Commons.Utils;

namespace Commons.Pool
{
    /// <summary>
    /// The class maintains the object pools created in the application.
    /// For generic object pool, the pool manager does not track it in the runtime. The client 
    /// shall maintain the instance by itself.
    /// For keyed object pool, the client can access it by just providing the key.
    /// When the pool manager is disposed, all of the objects in all the pools are disposed, even if the object is
    /// not returned to the pool yet. 
    /// It is recommended that there is only one pool manager instance for one application, although pool manager itself is not 
    /// singleton and multiple instances can be created for one application. By achieving this, if you are using 
    /// an IoC container, you can register the pool manager object into the container by using singleton option, which can be easily
    /// achieved in most of the modern IoC containers.
    /// Path forward, the pool manager could be integrated with some popular IoC containers.
    /// </summary>
    [CLSCompliant(true)]
    public class PoolManager : IPoolManager
    {
        private readonly ConcurrentBag<IDisposable> genericPools = new ConcurrentBag<IDisposable>();
        private readonly ConcurrentDictionary<string, object> keyedPools = new ConcurrentDictionary<string, object>();

        public void Destroy(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("The key is invalid");
            }

            object poolObj;
            if (keyedPools.TryGetValue(key, out poolObj))
            {
                if (keyedPools.TryRemove(key, out poolObj))
                {
                    // if remove does not succeed, it could be removed by another thread.
                    ((IDisposable) poolObj).Dispose();
                }
            }
            else
            {
                throw new ArgumentException("The key does not represent a valid object pool.");
            }
        }

        public void Dispose()
        {
            foreach (var pool in genericPools)
            {
                pool.Swallow(x => x.Dispose());
            }

            foreach (var keyedPool in keyedPools)
            {
                ((IDisposable) keyedPool.Value).Swallow(x => x.Dispose());
            }
            keyedPools.Clear();
        }

        public IPoolDescriptor<T> NewPool<T>() where T : class
        {
            return new GenericPoolDescriptor<T>(this);
        }

        public IObjectPool<T> GetPool<T>(string key) where T : class
        {
            object pool;
            if (keyedPools.TryGetValue(key, out pool))
            {
                return (IObjectPool<T>) pool;
            }
            return null;
        }

        internal void AddPool<T>(IObjectPool<T> pool) where T : class
        {
            genericPools.Add(pool);
        }

        internal void AddKeyedPool<T>(string key, IObjectPool<T> pool) where T : class
        {
            if (!keyedPools.TryAdd(key, pool))
            {
                throw new InvalidOperationException("The keyed object pool cannot be added to the pool manager.");
            }
        }
    }
}
