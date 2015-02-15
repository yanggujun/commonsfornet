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
    [CLSCompliant(true)]
    public class Deque<T> : ICollection, IEnumerable<T>, IEnumerable
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
                actual <<= 1;
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
                head = capacity / 2 - 1;
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
                head = capacity / 2 - 1;
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

                return items[tail];
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

                return items[head];
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void CopyTo(Array array, int index)
        {
            Guarder.CheckNull(array);
            var theArray = (T[])array;
            Guarder.CheckNull(theArray);
            var cursor = 0;
            for (var i = head; i <= tail; i++)
            {
                theArray[index + (cursor++)] = items[i];
            }
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
            get { throw new NotSupportedException("The sync root is not supported in Commons.Collections"); }
        }

        private IEnumerable<T> Items
        {
            get
            {
                if (head != EmptyPointer && tail != EmptyPointer)
                { 
                    for (var i = head; i <= tail; i++)
                    {
                        yield return items[i];
                    }
                }
            }
        }

        private void Resize()
        {
            capacity <<= 1;
            var newItems = new T[capacity];
            var newHead = capacity / 2 - count / 2;
            var cursor = newHead;
            for (var i = head; i <= tail; i++)
            {
                newItems[cursor] = items[i];
                cursor++;
            }
            head = newHead;
            tail = cursor - 1;
            items = newItems;
        }
    }
}
