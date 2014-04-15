// Copyright CommonsForNET. Author: Gujun Yang. email: gujun.yang@gmail.com
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
using System.Collections;
using System.Collections.Generic;

namespace Commons.Collections
{
    /// <summary>
    /// Defines a queue whose size is limited. When the queue is full, if the 
    /// Absorb flag is true, the oldest item is removed from the queue, otherwise it 
    /// throws OperationNotSupportedException.
    /// </summary>
    /// <typeparam name="T">The queued object</typeparam>
    [CLSCompliant(true)]
    public sealed class BoundedQueue<T> : IEnumerable<T>, IEnumerable, ICollection
    {
        /// <summary>
        /// The queue actually stores the data.
        /// </summary>
        private Queue<T> queue = new Queue<T>();

        /// <summary>
        /// Constructs an empty queue with specifying the max size.
        /// </summary>
        /// <param name="maxSize">The max size of the queue.</param>
        /// <param name="absorb">whether to always enable the enqueue operation.</param>
        public BoundedQueue(int maxSize, bool absorb = false)
        {
            if (maxSize < 1)
            {
                throw new ArgumentException("Max size is invalid.");
            }
            MaxSize = maxSize;
            Absorb = absorb;
        }

        /// <summary>
        /// Constructs the queue with a collection, with the max size. If the collection count exceeds 
        /// max size, eliminates the one not enumerated.
        /// </summary>
        /// <param name="collection">The collection</param>
        /// <param name="maxSize">The max size of the queue.</param>
        /// <param name="absorb">whether to always enable the enqueue operation.</param>
        public BoundedQueue(IEnumerable<T> collection, int maxSize, bool absorb = false) : this(maxSize, absorb)
        {
            foreach (var item in collection)
            {
                Enqueue(item);
                if (queue.Count >= MaxSize)
                {
                    break;
                }
            }
        }
        
        /// <summary>
        /// The flag is used in the following case:
        /// When the collection is full and the flag is set to true, 
        /// if a new item is enqueued, the queue will 
        /// enqueue the new one, and dequeue the oldest one.
        /// The value can only be set at construction.
        /// The default value is false.
        /// </summary>
        public bool Absorb { get; private set; }

        /// <summary>
        /// Whether the queue is full.
        /// </summary>
        public bool IsFull
        {
            get { return Count >= MaxSize; }
        }

        /// <summary>
        /// Max size limit of the queue.
        /// </summary>
        public int MaxSize
        {
            get;
            private set;
        }

        /// <summary>
        /// When the queue is full, enqueue a new item will throw an exception.
        /// </summary>
        /// <param name="item">The item to enqueue.</param>
        public void Enqueue(T item)
        {
            if (IsFull && !Absorb)
            {
                throw new InvalidOperationException("The queue has reached the max size.");
            }
            else if (IsFull && Absorb)
            {
                Dequeue();
            }
            queue.Enqueue(item);
        }

        public T Dequeue()
        {
            return queue.Dequeue();
        }

        public T Peek()
        {
            return queue.Peek();
        }

        public void CopyTo(Array array, int index)
        {
            var objArray = array as T[];
            if (null == objArray)
            {
                throw new ArgumentNullException("The input array is invalid.");
            }
            queue.CopyTo(objArray, index);
        }

        public int Count
        {
            get { return queue.Count; }
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public object SyncRoot
        {
            get { return this; }
        }

        public IEnumerator GetEnumerator()
        {
            return queue.GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return queue.GetEnumerator();
        }
    }
}
