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

using Commons.Collections.Set;

namespace Commons.Collections.Bag
{
    /// <summary>
    /// A bag is a collection which accept duplicate elements.
    /// </summary>
    /// <typeparam name="T">Element type.</typeparam>
    [CLSCompliant(true)]
    public interface IBag<T> : ICollection<T>, IEnumerable<T>, IEnumerable
    {
        /// <summary>
        /// Retrieves the number of the elemenet.
        /// </summary>
        /// <param name="item">The element.</param>
        /// <returns></returns>
        int this[T item] { get; }

        /// <summary>
        /// Add an element with number of copies.
        /// </summary>
        /// <param name="item">The element to add.</param>
        /// <param name="copies">Number of the element copies.</param>
        void Add(T item, int copies);

        /// <summary>
        /// Remove an element with number of copies.
        /// </summary>
        /// <param name="item">The element to remove.</param>
        /// <param name="copies">Number of the element to remove.</param>
        /// <returns>True if remove succeeds.</returns>
        bool Remove(T item, int copies);

        /// <summary>
        /// Retrieves a unique set of the elements in the bag.
        /// </summary>
        /// <returns></returns>
        IStrictSet<T> ToUnique();
    }
}
