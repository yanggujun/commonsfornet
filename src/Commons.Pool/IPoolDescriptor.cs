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
    /// The interface defines the operations for an object pool configuration descriptor.
    /// The descriptor enables the client to define the key of the object pool, the initial size and max size of the object pool 
    /// and the way to create and destroy the objects contained in the pool.
    /// </summary>
    /// <typeparam name="T">The type of the object contained in the pool.</typeparam>
    [CLSCompliant(true)]
    public interface IPoolDescriptor<T> where T : class
    {
        /// <summary>
        /// Defines the key of the object pool.
        /// An object pool may or may not have a key. If an object pool has a key, it can be 
        /// accessed every time from the pool manager by providing its key. While if an object 
        /// pool does not have a key, the client cannot access it through the pool manager after
        /// the pool is created. 
        /// </summary>
        /// <param name="key">The key of the object pool.</param>
        /// <returns>The pool object descriptor instance.</returns>
        IPoolDescriptor<T> OfKey(string key);

        /// <summary>
        /// Sets the initial size of the object pool. If this method is not called before a pool
        /// is instantiated, it is set to default value 0.
        /// </summary>
        /// <param name="initialSize">The value.</param>
        /// <returns>The pool descriptor with updated initial size.</returns>
        IPoolDescriptor<T> InitialSize(int initialSize);

        /// <summary>
        /// Sets the maximum size of the object pool. If this method is not called before a pool is 
        /// instantiated, the pool descriptor sets it to default value 10 and checks whether it's larger than <see cref="InitialSize"/>.
        /// If the value is invalid, the pool is not instantiated.
        /// </summary>
        /// <param name="maxSize">The value.</param>
        /// <returns>The pool descriptor with updated maximum size.</returns>
        IPoolDescriptor<T> MaxSize(int maxSize);

        /// <summary>
        /// Defines the object creator function for the pooled objects. The value must be set before the pool
        /// is instantiated.
        /// </summary>
        /// <param name="creator">The creator method.</param>
        /// <returns>The pool descriptor with updated creator method.</returns>
        IPoolDescriptor<T> WithCreator(Func<T> creator);

        /// <summary>
        /// Defines the object destroy function for the pooled objects. If the value is not set before the pool is 
        /// instantiated, the value is set to null.
        /// When the object pool is trying to destroy the pooled objects, it will check if the destroy method is defined.
        /// If it's defined, the pool calls the destroy method. If not, the pool checks if the object is an instance of 
        /// <see cref="IDisposable"/> and dispose the object. If neither condition is met, the pool does nothing when it is destroyed.
        /// </summary>
        /// <param name="destroyer">The destroy method.</param>
        /// <returns>The pool descriptor with updated destroyed method.</returns>
        IPoolDescriptor<T> WithDestroyer(Action<T> destroyer);

        /// <summary>
        /// Defines the factory for creating and destroying the pooled objects. When this method is called, the object creator and
        /// destroyer set by <see cref="WithCreator"/> and <see cref="WithDesctroyer"/> are ignored.
        /// </summary>
        /// <param name="factory">The object factory.</param>
        /// <returns>The pool descriptor with updated factory.</returns>
        IPoolDescriptor<T> WithFactory(IPooledObjectFactory<T> factory);

        /// <summary>
        /// Optional, objects validator. If not set, or null a default implementation will be used. The default implementation makes no validation at all.
        /// </summary>
        /// <param name="validator">The validator to use with the configured object pool</param>
        /// <returns>The pool descriptor with updated factory.</returns>
        IPoolDescriptor<T> WithValidator(IPooledObjectValidator<T> validator);

        /// <summary>
        /// Sets the limit of attempts to acquire an if, internally, acquired objects are invalid after tested with the configured <see cref="IPooledObjectValidator{T}"/>.
        /// If this method is not called before a pool is instantiated, the pool descriptor sets it to default value 10.
        /// If the value is invalid (negative), the pool is not instantiated.
        /// </summary>
        /// <param name="acquiredInvalidLimit">The value.</param>
        /// <returns>The pool descriptor with updated maximum size.</returns>
        IPoolDescriptor<T> AcquiredInvalidLimit(int acquiredInvalidLimit);

        /// <summary>
        /// Instantiate the object pool.
        /// </summary>
        /// <returns>The new object pool.</returns>
        IObjectPool<T> Instance();
    }
}
