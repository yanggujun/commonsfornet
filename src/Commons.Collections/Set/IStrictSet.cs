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
    /// The interface defines the operations for a set, with the strict meaning in mathematics.
    /// In mathematics, a set is a collection of distinguishable objects. The value of any element in the set must 
    /// be different from the one of any other elements.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(true)]
    public interface IStrictSet<T> : ICollection<T>, IEnumerable<T>, IEnumerable
    {
        void Intersect(IStrictSet<T> other);

        void Union(IStrictSet<T> other);

        void Differ(IStrictSet<T> other);

        bool IsSubsetOf(IStrictSet<T> other);

        bool IsProperSubsetOf(IStrictSet<T> other);

        bool IsEqualWith(IStrictSet<T> other);

        /// <summary>
        /// If the current set and the set <paramref name="other"/> have no elements in common, 
        /// return true. otherwise false.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        bool IsDisjointWith(IStrictSet<T> other);

        /// <summary>
        /// The current set becomes the compliment of itself under the universe set <paramref name="universe"/>.
        /// The current set must be a subset of <paramref name="universe"/>.
        /// </summary>
        /// <param name="universe"></param>
        /// <param name="other"></param>
        void Compliment(IStrictSet<T> universe);
    }
}
