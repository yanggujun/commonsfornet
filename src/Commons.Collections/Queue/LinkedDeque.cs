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
	/// <summary>
	/// A LinkedDeque is a deque with its entries doubly linked with their next and previous siblings.
	/// </summary>
	/// <typeparam name="T">The type of the item in the deque.</typeparam>
	public class LinkedDeque<T> : IDeque<T>, IEnumerable<T>, ICollection, IEnumerable
	{
		public void Append(T item)
		{
			throw new System.NotImplementedException();
		}

		public void Prepend(T item)
		{
			throw new System.NotImplementedException();
		}

		public T Pop()
		{
			throw new System.NotImplementedException();
		}

		public T Shift()
		{
			throw new System.NotImplementedException();
		}

		public T First
		{
			get { throw new System.NotImplementedException(); }
		}

		public T Last
		{
			get { throw new System.NotImplementedException(); }
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
