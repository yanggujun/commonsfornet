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
		IPoolDescriptor<T> NewObjectPool<T>() where T : class;

		/// Creates a generic object pool with the initial pool size set to <paramref name="initSize"/> and maximum pool size set to <paramref name="maxSize"/>.
		/// The object in the pool is created by the <paramref name="factory"/>. After the pool is created, client shall mantain its lifecycle. The pool manager 
		/// destroys the pool only when itself is destoryed.
		/// </summary>
		/// <typeparam name="T">The pooled object type.</typeparam>
		/// <param name="initSize">The initial size of the generic pool.</param>
		/// <param name="maxSize">the maximum size of the generic pool.</param>
		/// <param name="factory">The object factory used to create the pooled object.</param>
		/// <returns>A generic object pool.</returns>
		//IObjectPool<T> NewGenericObjectPool<T>(int initSize, int maxSize, IPooledObjectFactory<T> factory) where T : class;

		/// <summary>
		/// Creates an object pool with the <paramref name="key"/>. The client can retrieve the pool instance whenever a key of the pool is provided.
		/// The client can destroys the pool from the key of the pool.
		/// An <exception cref="InvalidOperationException"/> is thrown when a key already exists for an object pool.
		/// </summary>
		/// <typeparam name="K">The type of the key.</typeparam>
		/// <typeparam name="T">The type of the pooled object.</typeparam>
		/// <param name="key">The key of a pool.</param>
		/// <param name="initSize">The intial size of the object pool.</param>
		/// <param name="maxSize">The maximum size of the object pool.</param>
		/// <param name="factory">The object factory.</param>
		/// <returns>A generic object pool.</returns>
		//IObjectPool<T> NewKeyedObjectPool<K, T>(K key, int initSize, int maxSize, IPooledObjectFactory<T> factory) where T : class;

		/// <summary>
		/// Retrieves an object pool with the <paramref name="key"/>. When the key does not exist in the pool manager, <exception cref="ArgumentException"/> 
		/// is thrown.
		/// </summary>
		/// <typeparam name="T">The type of the pooled object.</typeparam>
		/// <param name="key">The key of the object pool.</param>
		/// <returns>The an existing generic object pool.</returns>
		IObjectPool<T> GetKeyedObjectPool<T>(string key) where T : class;

		/// <summary>
		/// Destroys an object pool with the key. When the key does not exist in the pool manager, <exception cref="ArgumentException"></exception> is thrown.
		/// </summary>
		/// <param name="key">The key of the object pool.</param>
		void Destory(string key);
	}
}
