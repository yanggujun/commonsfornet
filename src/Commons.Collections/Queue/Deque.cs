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

namespace Commons.Collections.Queue
{
    public class Deque<T> : IEnumerable<T>, IEnumerable, ICollection
    {
		private const int DefaultCapacity = 32;
		private const int EmptyPointer = -1;
		private T[] items;
		private int head;
		private int tail;
		private int capacity;
		private int count;

		public Deque() : this(DefaultCapacity)
		{
		}

		public Deque(int initialCapacity)
		{
			var actual = 1;
			while (actual < initialCapacity)
			{
				actual <<= 2;
			}
			capacity = actual;
			items = new T[capacity];
			head = EmptyPointer;
			tail = EmptyPointer;
			count = 0;
		}

        public void Append(T item)
        {
			if (head == EmptyPointer && tail == EmptyPointer)
			{
				head = capacity / 2;
				items[head] = item;
				tail = head;
			}
			else
			{
				if ((tail + 1) >= capacity)
				{
					Resize();
				}
				++tail;
				items[tail] = item;
			}
			++count;
        }

        public void Prepend(T item)
        {
			if (head == EmptyPointer && tail == EmptyPointer)
			{
				head = capacity / 2;
				items[head] = item;
				tail = head;
			}
			else
			{
				if (head - 1 < 0)
				{
					Resize();
				}
				--head;
				items[head] = item;
			}
			++count;
        }

        public T Pop()
        {
			if (count <= 0)
			{
				throw new InvalidOperationException("The deque is empty");
			}
			var item = items[tail];
			items[tail] = default(T);
			--tail;
			--count;
			if (count <= 0)
			{
				head = tail = EmptyPointer;
			}
			return item;
        }

        public T Shift()
        {
			if (count <= 0)
			{
				throw new InvalidOperationException("The deque is empty");
			}

			var item = items[head];
			items[head] = default(T);
			++head;
			--count;
			if (count <= 0)
			{
				head = tail = EmptyPointer;
			}

			return item;
        }

        public T Last
        {
            get
            {
				if (count <= 0)
				{
					throw new InvalidOperationException("The deque is empty");
				}

				return items[head];
            }
        }

        public T First
        {
            get
            {
				if (count <= 0)
				{
					throw new InvalidOperationException("The deque is empty");
				}

				return items[tail];
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { return count; }
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public object SyncRoot
        {
            get { return this; }
        }

		private void Resize()
		{
			throw new NotImplementedException();
		}
    }
}
