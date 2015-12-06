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
using System.Threading;
using Commons.Utils;

namespace Commons.Collections.Concurrent
{
    /// <summary>
    /// Concurrent sorted list.
    /// </summary>
    /// <remarks>This implementation is only for experiment and research use.</remarks>
    /// <typeparam name="T">The type of the elements in the list.</typeparam>
    [CLSCompliant(true)]
    public class ConcurrentSortedList<T> : IEnumerable<T>, IEnumerable
    {
        private readonly Node head;
        private readonly Node tail;
        private readonly IComparer<T> comparer;

        public ConcurrentSortedList() : this(Comparer<T>.Default)
        {
        }

        public ConcurrentSortedList(IComparer<T> comparer)
        {
            head = new Node();
            tail = new Node();
            head.Next = new AtomicMarkableReference<Node>(tail);
            tail.Next = new AtomicMarkableReference<Node>(null);
            this.comparer = comparer;
        }

        public void Add(T key)
        {
            var spin = new SpinWait();
            while (!Insert(key))
            {
                spin.SpinOnce();
            }
        }

        public bool Remove(T key)
        {
            return Delete(key);
        }

        public bool Contains(T key)
        {
            return Find(key);
        }

        public void Clear()
        {
            var headNext = head.Next;
            var newTail = new AtomicMarkableReference<Node>(tail, false);
            var spin = new SpinWait();
            while (!head.Next.CompareExchange(headNext, headNext.IsMarked, newTail, false))
            {
                spin.SpinOnce();
            }
        }

        public int Count
        {
            get
            {
                var count = 0;
                var cursor = head.Next;
                while (!ReferenceEquals(cursor.Value, tail))
                {
                    if (!cursor.IsMarked)
                    {
                        count++;
                    }
                    cursor = cursor.Value.Next;
                }
                return count;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            var cursor = head.Next;
            while (!ReferenceEquals(cursor.Value, tail))
            {
                if (!cursor.IsMarked)
                {
                    yield return cursor.Value.Key;
                }
                cursor = cursor.Value.Next;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private bool Insert(T key)
        {
            var newNode = new Node();
            var spin = new SpinWait();
            newNode.Key = key;
            Node rightNode, leftNode;
            do
            {
                rightNode = Search(key, out leftNode);
                newNode.Next = new AtomicMarkableReference<Node>(rightNode, false);
                if (leftNode.Next.CompareExchange(rightNode, false, newNode, false))
                {
                    return true;
                }
                spin.SpinOnce();
            } while (true);
        }

        private bool Delete(T key)
        {
            Node rightNode, leftNode;
            var spin = new SpinWait();

            do
            {
                rightNode = Search(key, out leftNode);
                if (rightNode == tail || comparer.Compare(rightNode.Key, key) != 0)
                {
                    return false;
                }
                var rightNodeNext = rightNode.Next.Value;
                if (!rightNode.Next.CompareExchange(rightNodeNext, false, rightNodeNext, true))
                {
                    spin.SpinOnce();
                    continue;
                }

                leftNode.Next.CompareExchange(rightNode, false, rightNodeNext, false);
                return true;
            } while (true);
        }

        private bool Find(T key)
        {
            Node rightNode, leftNode;
            rightNode = Search(key, out leftNode);
            if (rightNode == tail || comparer.Compare(rightNode.Key, key) != 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Returns true when another loop is needed, otherwise the search is complete.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="leftNode"></param>
        /// <param name="rightNode"></param>
        /// <returns></returns>
        private bool DoSearch(T key, out Node leftNode, out Node rightNode)
        {
            Node leftNodeNext = null;
            leftNode = null;
            rightNode = null;

            while (true)
            {
                leftNode = head;
                rightNode = leftNode.Next.Value;
                while (true)
                {
                    var isMarked = false;
                    leftNodeNext = rightNode.Next.TryGetValue(out isMarked);
                    while (isMarked)
                    {
                        if (!leftNode.Next.CompareExchange(rightNode, false, leftNodeNext, false))
                        {
                            return true;
                        }
                        rightNode = leftNodeNext;
                        leftNodeNext = rightNode.Next.TryGetValue(out isMarked);
                    }
                    if (ReferenceEquals(rightNode, tail) || comparer.Compare(key, rightNode.Key) <= 0)
                    {
                        return false;
                    }
                    leftNode = rightNode;
                    rightNode = leftNodeNext;
                }
            }
        }

        private Node Search(T key, out Node leftNode)
        {
            Node rightNode = null;
            if (ReferenceEquals(head.Next.Value, tail))
            {
                leftNode = head;
                return tail;
            }
            var spin = new SpinWait();
            while (DoSearch(key, out leftNode, out rightNode))
            {
                spin.SpinOnce();
            }

            return rightNode;
        }

        private class Node
        {
            public T Key;
            public AtomicMarkableReference<Node> Next;
        }
    }
}
