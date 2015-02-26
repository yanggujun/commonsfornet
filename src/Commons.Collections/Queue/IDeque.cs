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
	/// The interface defines the operations for a double ended queue. A double ended queue is a queue, for which elements 
	/// can be added to and removed from both the head and tail of the queue.
	/// </summary>
	/// <typeparam name="T">The item type of the deque.</typeparam>
    [CLSCompliant(true)]
	public interface IDeque<T> : IEnumerable<T>, ICollection, IEnumerable
	{
		/// <summary>
		/// Appends an <paramref name="item"/> to the tail of the deque.
		/// </summary>
		/// <param name="item">The item.</param>
		void Append(T item);

		/// <summary>
		/// Prepends an <paramref name="item"/> to the head of the deque.
		/// </summary>
		/// <param name="item"></param>
		void Prepend(T item);

		/// <summary>
		/// Retrieves the item in the tail of the deque and removes it. If the deque is empty, <see cref="System.InvalidOperationException"/> is thrown.
		/// </summary>
		/// <returns>The item.</returns>
		T Pop();

		/// <summary>
		/// Retrieves the item in the head of the deque and removes it. If the deque is empty, <see cref="System.InvalidOperationException"/> is thrown.
		/// </summary>
		/// <returns>The item.</returns>
		T Shift();

		/// <summary>
		/// Retrieves the item in the head of the deque. If the deque is empty, <see cref="System.InvalidOperationException"/> is thrown.
		/// </summary>
		T First { get; }

		/// <summary>
		/// Retrieves the item in the tail of the deque. If the deque is empty, <see cref="System.InvalidOperationException"/> is thrown.
		/// </summary>
		T Last { get; }
	}
}
