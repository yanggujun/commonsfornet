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
using Commons.Collections.Map;
using Commons.Utils;

namespace Commons.Collections.Set
{
    [CLSCompliant(true)]
    public class HashedSet<T> : AbstractHashedSet<T>, IStrictSet<T>, IReadOnlyStrictSet<T>
    {
        private readonly HashedMap<T, object> map;

        public HashedSet()
        {
            map = new HashedMap<T, object>();
        }

        public HashedSet(int capacity)
        {
            map = new HashedMap<T, object>(capacity);
        }

        public HashedSet(IEqualityComparer<T> equalityComparer)
        {
            map = new HashedMap<T, object>(equalityComparer);
        }

        public HashedSet(Equator<T> equator)
        {
            map = new HashedMap<T, object>(equator);
        }

        public HashedSet(int capacity, IEqualityComparer<T> equalityComparer)
        {
            map = new HashedMap<T, object>(capacity, equalityComparer);
        }

        public HashedSet(int capacity, Equator<T> equator)
        {
            map = new HashedMap<T, object>(capacity, equator);
        }

        public HashedSet(IEnumerable<T> items, int capacity, Equator<T> equator) : this(capacity, equator)
        {
            if (items != null)
            {
                foreach (var item in items)
                {
                    Add(item);
                }
            }
        }

        protected override IDictionary<T, object> Map
        {
            get
            {
                return map;
            }
        }
    }
}
