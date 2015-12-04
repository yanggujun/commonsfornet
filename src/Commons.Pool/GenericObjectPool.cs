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

namespace Commons.Pool
{
	/// <summary>
	/// The generic object pool. The pool is used when the objects are identical, for example, the connection pool.
	/// </summary>
	/// <typeparam name="T">The type of the pooled object.</typeparam>
	[CLSCompliant(true)]
    public class GenericObjectPool<T> : IObjectPool<T> where T : class
	{
		private IPooledObjectFactory<T> factory;
        private ConcurrentQueue<T> objQueue;
		private ReaderWriterLockSlim locker;
		private int initialSize;
		private int maxSize;
        private int createdCount;
        private AutoResetEvent objectReturned;
		private ReferenceSet<T> idleObjects; 

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
				throw new ArgumentException("Max size is smaller than initialSize.");
			}
			if (factory == null)
			{
				throw new ArgumentNullException("factory");
			}
			this.initialSize = initialSize;
			this.maxSize = maxSize;
			this.factory = factory;
			createdCount = 0;
            objectReturned = new AutoResetEvent(false);
			locker = new ReaderWriterLockSlim();
            objQueue = new ConcurrentQueue<T>();
			idleObjects = new ReferenceSet<T>();
			for (var i = 0; i < initialSize; i++)
			{
                objQueue.Enqueue(factory.Create());
                createdCount++;
			}
		}

		/// <summary>
		/// Acquires an object from the pool. The method is blocked when there is no object available 
		/// in the pool. When there is no object, it waits until any object is returned to the pool.
		/// The method is recommended to use when the objects are sufficient.
		/// </summary>
		/// <returns>The pooled object</returns>
		public T Acquire()
		{
			T obj;
			var spin = new SpinWait();
            while (!TryAcquire(-1, out obj))
			{
				spin.SpinOnce();
			}

			return obj;
		}

		/// <summary>
		/// Tries to acquire an object in the pool. The method waits until <param name="timeout"></param> expires.
		/// When no object is available when waiting times out, it returns false.
		/// When <param name="timeout"></param> is set to 0, it only tests whether there is any
		/// object in the pool and return immediately. If <param name="timeout"></param> is set to 
		/// a negative number, it waits infinitely. But it does not guarantee that an object is 
		/// acquired. The return result still can be false. If you want to wait infinitely and expect 
		/// a object is returned anyway, use <see cref="Acquire"/> method.
		/// </summary>
		/// <param name="timeout">The time to wait for an object available in the pool.</param>
		/// <param name="obj">The object acquired.</param>
		/// <returns>True if an object is acquired, otherwise false.</returns>
        public bool TryAcquire(int timeout, out T obj)
        {
	        var acquired = false;
            var localTimeout = timeout < 0 ? -1 : timeout;
	        if (objQueue.TryDequeue(out obj))
	        {
		        acquired = true;
		        locker.EnterWriteLock();
		        try
		        {
			        idleObjects.Remove(obj);
		        }
		        finally
		        {
			        locker.ExitWriteLock();
		        }
	        }
	        else
	        {
		        locker.EnterWriteLock();
		        try
		        {
			        if (createdCount < maxSize)
			        {
					    obj = factory.Create();
					    createdCount++;
				        acquired = true;
			        }
		        }
		        finally
		        {
			        locker.ExitWriteLock();
		        }
		        if (!acquired)
		        {
			        if (objectReturned.WaitOne(localTimeout))
			        {
				        locker.EnterWriteLock();
				        try
				        {
					        acquired = objQueue.TryDequeue(out obj);
					        if (acquired)
					        {
						        idleObjects.Remove(obj);
					        }
				        }
				        finally
				        {
					        locker.ExitWriteLock();
				        }
			        }
		        }
			}

	        return acquired;
        }

		/// <summary>
		/// Returns the object to the pool. If the object is already returned to the pool, <exception cref="InvalidOperationException"></exception> is thrown.
		/// </summary>
		/// <param name="obj">The object to return.</param>
        public void Return(T obj)
        {
			locker.EnterUpgradeableReadLock();
	        try
	        {
		        if (idleObjects.Contains(obj))
		        {
			        throw new InvalidOperationException("The object is already returned to the pool.");
		        }
		        locker.EnterWriteLock();
		        try
		        {
			        objQueue.Enqueue(obj);
			        idleObjects.Add(obj);
		        }
		        finally
		        {
			        locker.ExitWriteLock();
		        }
	        }
	        finally
	        {
				locker.ExitUpgradeableReadLock();
	        }
	        objectReturned.Set();
        }

		/// <summary>
		/// The number of objects available in the pool.
		/// </summary>
        public int IdleCount
        {
            get
            {
                var count = 0;
                locker.EnterReadLock();
                try
                {
                    count = objQueue.Count;
                }
                finally
                {
                    locker.ExitReadLock();
                }
                return count;
            }
        }

		/// <summary>
		/// The number of objects which are actively used by pool consumers. 
		/// Those objects are already acquired.
		/// </summary>
        public int ActiveCount
        {
            get
            {
	            var created = 0;
	            var idleCount = 0;
	            locker.EnterReadLock();
	            try
	            {
		            created = createdCount;
		            idleCount = objQueue.Count;
	            }
	            finally
	            {
					locker.ExitReadLock();
	            }
	            return created - idleCount;
            }
        }

		/// <summary>
		/// The size of the pool.
		/// </summary>
        public int Capacity
        {
            get
            {
                return maxSize;
            }
        }

		/// <summary>
		/// The initial size of the pool.
		/// </summary>
        public int InitialSize
        {
            get { return initialSize; }
        }

		/// <summary>
		/// Dispose the pool.
		/// </summary>
        public void Dispose()
        {
            locker.EnterWriteLock();
            try
            {
                foreach(var element in objQueue)
                {
                    factory.Destroy(element);
                }
				objectReturned.Dispose();
            }
            finally
            {
                locker.ExitWriteLock();
            }
        }
    }
}
