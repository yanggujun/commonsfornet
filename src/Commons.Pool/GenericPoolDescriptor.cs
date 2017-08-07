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
    internal class GenericPoolDescriptor<T> : IPoolDescriptor<T> where T : class
    {
        private int initialSize;
        private int maxSize;
        private string key;
        private readonly PoolManager poolManager;
        private Func<T> creator;
        private Action<T> destroyer;
        private IPooledObjectFactory<T> objectFactory;
        private IPooledObjectValidator<T> validator;
        private int acquiredInvalidLimit = 10;

        public GenericPoolDescriptor(PoolManager poolManager)
        {
            this.poolManager = poolManager;
            maxSize = -1;
        }

        public IPoolDescriptor<T> OfKey(string key)
        {
            this.key = key;
            return this;
        }

        public IPoolDescriptor<T> InitialSize(int initialSize)
        {
            if (initialSize < 0)
            {
                throw new ArgumentException("The initial size value is invalid.");
            }
            this.initialSize = initialSize;
            return this;
        }

        public IPoolDescriptor<T> MaxSize(int maxSize)
        {
            if (maxSize < -1)
            {
                throw new ArgumentException("The max size value is invalid.");
            }
            if (maxSize < initialSize)
            {
                throw new ArgumentException("The maximum size of the pool shall not be smaller than the initial size.");
            }
            this.maxSize = maxSize;
            return this;
        }

        public IPoolDescriptor<T> WithCreator(Func<T> creator)
        {
            this.creator = creator;
            return this;
        }

        public IPoolDescriptor<T> WithDesctroyer(Action<T> destroyer)
        {
            this.destroyer = destroyer;
            return this;
        }

        public IPoolDescriptor<T> WithFactory(IPooledObjectFactory<T> factory)
        {
            objectFactory = factory;
            return this;
        }

        public IPoolDescriptor<T> WithValidator(IPooledObjectValidator<T> validator)
        {
            this.validator = validator;
            return this;
        }

        public IPoolDescriptor<T> AcquiredInvalidLimit(int acquiredInvalidLimit)
        {
            this.acquiredInvalidLimit = acquiredInvalidLimit;
            return this;
        }

        public IObjectPool<T> Instance()
        {
            if (objectFactory == null)
            {
                if (creator == null)
                {
                    throw new InvalidOperationException("The object pool cannot be instantiated as the object creation method is not defined.");
                }

                objectFactory = new ObjectFactory<T>
                {
                    Creator = creator,
                    Destroyer = destroyer ?? new Action<T>((obj) => { (obj as IDisposable)?.Dispose(); })
                };
            }

            if (maxSize > 0 && maxSize < initialSize)
            {
                throw new ArgumentException("The maximum size of the pool shall not be smaller than its initial size.");
            }

            if (acquiredInvalidLimit < 0)
            {
                throw new ArgumentException($"The limit of acquired invalid objects must not be negative. Actual value is {acquiredInvalidLimit}.");
            }

            var newPool = new GenericObjectPool<T>(initialSize, maxSize, objectFactory, validator, acquiredInvalidLimit);
            if (string.IsNullOrWhiteSpace(key))
            {
                poolManager.AddPool(newPool);
            }
            else
            {
                poolManager.AddKeyedPool(key, newPool);
            }

            return newPool;
        }
    }
}
