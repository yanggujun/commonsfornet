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
	/// <typeparam name="T">The type of the elements in the list.</typeparam>
    [CLSCompliant(true)]
    public class ConcurrentSortedList<T> : IList<T>, 
#if NET45
        IReadOnlyList<T>, IReadOnlyCollection<T>,
#endif
        ICollection<T>, ICollection, IEnumerable<T>, IEnumerable
    {
        private Node<T> head;
        private Node<T> tail;
        private IComparer<T> comparer;
        private SpinWait spin;

        public ConcurrentSortedList()
        {
            head = new Node<T>();
            tail = new Node<T>();
            head.Next = tail;
            spin = new SpinWait();
            comparer = Comparer<T>.Default;
        }

        private bool Insert(T key)
        {
            var newNode = new Node<T>();
            newNode.Key = key;
            Node<T> rightNode, leftNode;
            do
            {
                rightNode = Search(key, out leftNode);
                newNode.Next = rightNode;
                if (Interlocked.CompareExchange(ref leftNode.Next, newNode, rightNode) == rightNode)
                {
                    return true;
                }
                spin.SpinOnce();
            } while (true);
        }

        private bool Delete(T key)
        {
            Node<T> rightNode, rightNodeNext, leftNode;
            do
            {
                rightNode = Search(key, out leftNode);
                if (rightNode == tail || comparer.Compare(rightNode.Key, key) != 0)
                {
                    return false;
                }
                rightNodeNext = rightNode.Next;
                if (rightNodeNext.Marked == 0)
                {
                    if (Interlocked.CompareExchange(ref rightNode.Next, Mark(rightNodeNext), rightNodeNext) == rightNodeNext)
                    {
                        break;
                    }
                }
            } while (true);

            if (Interlocked.CompareExchange(ref leftNode.Next, rightNodeNext, rightNode) == rightNode)
            {
                rightNode = Search(rightNode.Key, out leftNode);
            }

            return true;
        }

        private bool Find(T key)
        {
            Node<T> rightNode, leftNode;
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
        private bool DoSearch(T key, out Node<T> leftNode, out Node<T> rightNode)
        {
            Node<T> leftNodeNext = null;
            leftNode = null;
            rightNode = null;

            do
            {
                var cursor = head;
                var nextCursor = head.Next;
                // Find left and right node.
                do
                {
                    if (nextCursor.Marked == 0)
                    {
                        leftNode = cursor;
                        leftNodeNext = nextCursor;
                    }
                    cursor = Unmark(nextCursor);
                    if (cursor == tail)
                    {
                        break;
                    }
                    nextCursor = cursor.Next;
                } while (nextCursor.Marked == 1 || comparer.Compare(cursor.Key, key) < 0);

                rightNode = cursor;

                // Check nodes are adjacent.
                if (leftNodeNext == rightNode)
                {
                    if (rightNode != tail && rightNode.Next.Marked == 1)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }

                // Remove one or more marked nodes.
                if (Interlocked.CompareExchange(ref leftNode.Next, rightNode, leftNodeNext) == leftNodeNext)
                {
                    if (rightNode != tail && rightNode.Next.Marked == 1)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }

            } while (true);
        }

        private Node<T> Search(T key, out Node<T> leftNode)
        {
            Node<T> rightNode = null;
            while (DoSearch(key, out leftNode, out rightNode))
            {
                spin.SpinOnce();
            }

            return rightNode;
        }

        private Node<T> Mark(Node<T> node)
        {
            var newNode = new Node<T> { Key = node.Key, Next = node.Next, Marked = 1 };

            return newNode;
        }

        private Node<T> Unmark(Node<T> node)
        {
            var newNode = new Node<T> { Key = node.Key, Next = node.Next, Marked = 0 };
            return newNode;
        }

        public int IndexOf(T item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, T item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public T this[int index]
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void Add(T item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(T item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        public bool Remove(T item)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<T> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        void ICollection.CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        int ICollection.Count
        {
            get { throw new NotImplementedException(); }
        }

        bool ICollection.IsSynchronized
        {
            get { throw new NotImplementedException(); }
        }

        object ICollection.SyncRoot
        {
            get { throw new NotImplementedException(); }
        }
    }
}
