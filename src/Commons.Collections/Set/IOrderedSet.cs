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

namespace Commons.Collections.Set
{
    /// <summary>
    /// An ordered set is a set where the elements in the set are sequenced by some rule.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the set.</typeparam>
    [CLSCompliant(true)]
    public interface IOrderedSet<T> : IStrictSet<T>, IReadOnlyStrictSet<T>, IEnumerable<T>, ICollection, IEnumerable
    {
        /// <summary>
        /// Element which is on the first of the set.
        /// </summary>
        T First { get; }

        /// <summary>
        /// Element which is on the last of the set.
        /// </summary>
        T Last { get; }

        /// <summary>
        /// Element after a specified <paramref name="item"/>.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        T After(T item);

        /// <summary>
        /// Element before a specified <paramref name="item"/>
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        T Before(T item);

        /// <summary>
        /// Get the element on the index <paramref name="index"/>.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        T GetIndex(int index);
    }
}
