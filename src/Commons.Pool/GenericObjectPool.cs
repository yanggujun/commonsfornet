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
using Commons.Collections.Queue;

namespace Commons.Pool
{
	/// <summary>
	/// The generic object pool. The pool is used when the objects are identical, for example, the connection pool.
	/// </summary>
	/// <typeparam name="T">The type of the pooled object.</typeparam>
	[CLSCompliant(true)]
    public class GenericObjectPool<T> : IObjectPool<T>
	{
		private IPooledObjectFactory<T> factory;
        private LinkedDeque<T> idleObjects;
		private ReaderWriterLockSlim locker;
		private int initialSize;
		private int maxSize;
        private int createdCount;
        private AutoResetEvent objectReturned;

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
            this.createdCount = 0;
            objectReturned = new AutoResetEvent(false);
			locker = new ReaderWriterLockSlim();
            idleObjects = new LinkedDeque<T>();
			for (var i = 0; i < initialSize; i++)
			{
                idleObjects.Prepend(factory.Create());
                createdCount++;
			}
		}

		/// <summary>
		/// Acquires an object from the pool. 
		/// </summary>
		/// <returns></returns>
		public T Acquire()
		{
			T obj;
            TryAcquire(-1, out obj);

			return obj;
		}

        public bool TryAcquire(int timeout, out T obj)
        {
	        var acquired = false;
            obj = default(T);
			locker.EnterWriteLock();
	        try
	        {
                if (idleObjects.IsEmpty)
                {
                    if (createdCount < maxSize)
                    {
                        obj = factory.Create();
                        createdCount++;
                        idleObjects.Prepend(obj);
                        acquired = true;
                    }
                    else
                    {
                        if (objectReturned.WaitOne(timeout))
                        {
                            obj = idleObjects.Pop();
                            acquired = true;
                        }
                    }
                }
                else
                {
                    obj = idleObjects.Pop();
                    acquired = true;
                }
	        }
	        finally
	        {
				locker.ExitWriteLock();
	        }

	        return acquired;
        }

        public void Return(T obj)
        {
            locker.EnterWriteLock();
            try
            {
                idleObjects.Prepend(obj);
                objectReturned.Set();
            }
            finally
            {
                locker.ExitWriteLock();
            }
        }

        public int IdleCount
        {
            get
            {
                var count = 0;
                locker.EnterReadLock();
                try
                {
                    count = idleObjects.Count;
                }
                finally
                {
                    locker.ExitWriteLock();
                }
                return count;
            }
        }

        public int ActiveCount
        {
            get
            {
                var created = createdCount;
                var idleCount = IdleCount;
                return created - idleCount;
            }
        }

        public int Capacity
        {
            get
            {
                return maxSize;
            }
        }

        public int InitialSize
        {
            get { return initialSize; }
        }

        public void Dispose()
        {
            locker.EnterWriteLock();
            try
            {
                foreach(var element in idleObjects)
                {
                    factory.Destroy(element);
                }
            }
            finally
            {
                locker.ExitWriteLock();
            }
        }
    }
}
