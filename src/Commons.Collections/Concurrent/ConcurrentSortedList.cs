﻿// Copyright CommonsForNET.
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
    public class ConcurrentSortedList<T> : IList<T>, ICollection<T>, ICollection, IEnumerable<T>, IEnumerable
    {
        private Node<T> head;
        private Node<T> tail;

        public ConcurrentSortedList()
        {
            head = new Node<T>();
            tail = new Node<T>();
            head.Next = tail;
        }

        private bool Insert(T item)
        {
            var newNode = new Node<T>();
            newNode.Key = item;
            Node<T> rightNode, leftNode;
            do
            {
            } while (true);

            return false;
        }

        private Node<T> Search(T item, out Node<T> leftNode)
        {
            Node<T> leftNodeNext, rightNode;
            while (true)
            {
                do
                {
                    var temp = head;
                    var tempNext = head.Next;
                    do
                    {
                    } while (true);
                } while (true);
            }

            return null;
        }

        private Node<T> Mark(Node<T> node)
        {
            return null;
        }

        private Node<T> Unmark(Node<T> node)
        {
            var atomic = Atomic<Node<T>>.From(node);
            while (!atomic.CompareExchange(new Node<T> { Key = node.Key, Marked = 0, Next = node.Next }))
            {
            }

            return null;
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
