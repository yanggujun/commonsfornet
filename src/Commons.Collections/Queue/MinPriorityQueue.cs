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
using System.Collections;
using System.Collections.Generic;

using Commons.Utils;

namespace Commons.Collections.Queue
{
    /// <summary>
    /// 
    /// </summary>
    public class MinPriorityQueue<T> : IPriorityQueue<T>, ICollection, IEnumerable<T>, IEnumerable
    {
		private readonly FibonacciHeap<T> heap;

        public MinPriorityQueue()
            : this(Comparer<T>.Default)
        {
        }

        public MinPriorityQueue(IComparer<T> comparer)
            : this(comparer.Compare)
        {
        }

        public MinPriorityQueue(Comparison<T> comparer) : this (null, comparer)
        {
        }

		public MinPriorityQueue(IEnumerable<T> items, Comparison<T> comparer)
		{
			heap = new FibonacciHeap<T>((x1, x2) => comparer(x1, x2) < 0);
			if (items != null)
			{
				foreach (var item in items)
				{
					Push(item);
				}
			}
		}

        public bool IsEmpty
        {
            get { return heap.IsEmpty; }
        }

        public T Top
        {
            get { return heap.Top; }
        }

        public void Push(T item)
        {
            heap.Add(item);
        }

        public T Pop()
        {
            return heap.ExtractTop();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return heap.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void CopyTo(Array array, int index)
        {
            var items = array as T[];
            items.Validate(x => x != null, new ArgumentException(string.Format("The array is not of type {0}", typeof(T))));
            var arrayIndex = index;
            foreach (var item in heap)
            {
                items[arrayIndex++] = item;
            }
        }

        public int Count
        {
            get { return heap.Count; }
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public object SyncRoot
        {
            get { throw new NotSupportedException("The SyncRoot is not supported in Commons.Collections."); }
        }
    }
}
