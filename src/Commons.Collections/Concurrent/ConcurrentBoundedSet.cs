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
using System.Collections.Concurrent;
using System.Collections.Generic;
using Commons.Collections.Collection;
using Commons.Utils;

namespace Commons.Collections.Concurrent
{
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class ConcurrentBoundedSet<T> : IProducerConsumerCollection<T>, IEnumerable<T>, IEnumerable
	{
		private Node head;
		private Node tail;
		private IEqualityComparer<T> comparer;
		private int maxSize;

		public ConcurrentBoundedSet(int maxSize) : this(maxSize, EqualityComparer<T>.Default)
		{
		}

		public ConcurrentBoundedSet(int maxSize, Equator<T> equator) : this(maxSize, new EquatorComparer<T>(equator))
		{
		}

		public ConcurrentBoundedSet(int maxSize, IEqualityComparer<T> comparer)
		{
			this.comparer = comparer;
			head = new Node();
			tail = new Node();
			head.Next = AtomicReference<Node>.From(tail);
			this.maxSize = maxSize;
		}

		public IEnumerator<T> GetEnumerator()
		{
			throw new NotImplementedException();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotImplementedException();
		}

		public void CopyTo(T[] array, int index)
		{
			throw new NotImplementedException();
		}

		public T[] ToArray()
		{
			throw new NotImplementedException();
		}

		public bool TryAdd(T item)
		{
			throw new NotImplementedException();
		}

		public bool TryTake(out T item)
		{
			throw new NotImplementedException();
		}

		void ICollection.CopyTo(Array array, int index)
		{
			throw new NotImplementedException();
		}

		public int Count
		{
			get { throw new NotImplementedException(); }
		}

		bool ICollection.IsSynchronized
		{
			get { return false; }
		}

		object ICollection.SyncRoot
		{
			get { throw new NotSupportedException(Messages.SyncRootNotSupportError); }
		}

		private class Node
		{
			public T Value;
			public bool Occupied;
			public AtomicReference<Node> Next;
		}
	}
}
