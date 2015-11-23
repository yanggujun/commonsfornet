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
using System.Threading;
using Commons.Collections.Set;
using Commons.Utils;

namespace Commons.Pool
{
	/// <summary>
	/// The generic object pool. The pool is used when the objects are identical, for example, the connection pool.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	[CLSCompliant(true)]
    public class GenericObjectPool<T> : IObjectPool<T>
	{
		private IPooledObjectFactory<T> factory;
		private ConcurrentBag<T> pooledObjects;
		private ReaderWriterLockSlim locker;
		private int initialSize;
		private int maxSize;

		/// <summary>
		/// Constructor with the intialSize and maxSize. <see cref="ArgumentException"/> is thrown when <param name="initialSize"/> is larger than <param name="maxSize"/>
		/// <see cref="IPooledObjectFactory{T}.Create"/> is called to create objects with the number of <param name="initialSize"></param>. If it returns the object with null, it is not 
		/// added to the pool.
		/// </summary>
		/// <param name="initialSize">The initial object number of the pool.</param>
		/// <param name="maxSize">The max object number of the pool.</param>
		/// <param name="factory">The factory create and destroy the pooled object.</param>
		public GenericObjectPool(int initialSize, int maxSize, IPooledObjectFactory<T> factory)
		{
			if (initialSize < 0)
			{
				throw new ArgumentException("The initial size is negtive.");
			}
			if (maxSize < initialSize)
			{
				throw new ArgumentException("Mas size is smaller than initialSize.");
			}
			if (factory == null)
			{
				throw new ArgumentNullException("factory");
			}
			this.initialSize = initialSize;
			this.maxSize = maxSize;
			this.factory = factory;
			locker = new ReaderWriterLockSlim();
			pooledObjects = new ConcurrentBag<T>();
			for (var i = 0; i < initialSize; i++)
			{
				pooledObjects.Add(this.factory.Create());
			}
		}

		/// <summary>
		/// Acquires an object from the pool. 
		/// </summary>
		/// <returns></returns>
		public T Acquire()
		{
			T obj;
			locker.EnterWriteLock();
			try
			{
				if (!pooledObjects.TryTake(out obj))
				{
				}
			}
			finally
			{
				locker.ExitWriteLock();
			}

			return obj;
		}

        public bool TryAcquire(long timeout, out T obj)
        {
	        var result = false;
			locker.EnterWriteLock();
	        try
	        {
		        if (pooledObjects.TryTake(out obj))
		        {
			        result = true;
		        }
		        else
		        {
			        if (pooledObjects.Count < maxSize)
			        {
			        }
		        }
	        }
	        finally
	        {
				locker.ExitWriteLock();
	        }

	        return result;
        }

        public void Return(T obj)
        {
            throw new NotImplementedException();
        }

        public int IdleCount
        {
            get { throw new NotImplementedException(); }
        }

        public int ActiveCount
        {
            get { throw new NotImplementedException(); }
        }

        public int Capacity
        {
            get { throw new NotImplementedException(); }
        }

        public int InitialSize
        {
            get { throw new NotImplementedException(); }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

		public event EventHandler<T> ObjectCreated;

		public event EventHandler<T> ObjectObtained;

		public event EventHandler<T> ObjectRetracted;

		public event EventHandler<T> ObjectDestroying;

		protected T NewObject()
		{
			return factory.Create();
		}
    }
}
