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
    [CLSCompliant(true)]
    public interface IPriorityQueue<T> : IEnumerable<T>, IEnumerable
    {
        /// <summary>
        /// The element count of the priority queue.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Whether the priority queue is empty.
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// Retrieves the element at the top of the priority queue. The element is removed when <see cref="IPriorityQueue.Pop"/>
        /// is called.
        /// </summary>
        T Top { get; }

        /// <summary>
        /// Pushes a new element to the priority queue.
        /// </summary>
        /// <param name="item"></param>
        void Push(T item);

        /// <summary>
        /// Retrieves the element at the top of the priority queue and removes it.
        /// </summary>
        /// <returns>The element at the top of the priority queue.</returns>
        T Pop();
    }
}
