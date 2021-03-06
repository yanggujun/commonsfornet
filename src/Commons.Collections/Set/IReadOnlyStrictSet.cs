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

namespace Commons.Collections.Set
{
    [CLSCompliant(true)]
    public interface IReadOnlyStrictSet<T> : 
#if NET45
        IReadOnlyCollection<T>, 
#endif
        IEnumerable<T>, IEnumerable
    {
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

        bool Contains(T item);
    }
}
