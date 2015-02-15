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
using System.Collections.Generic;
using Commons.Collections.Map;
using Commons.Collections.Set;
using Commons.Utils;

namespace Commons.Collections.Bag
{
    [CLSCompliant(true)]
    public class HashedBag<T> : AbstractMapBag<T>
    {
        private readonly Equator<T> equator;
        public HashedBag()
            : base(new HashedMap<T, int>())
        {
        }

        public HashedBag(IEqualityComparer<T> comparer) : this(comparer.Equals)
        {
        }

        public HashedBag(Equator<T> equator) : this(null, equator)
        {
        }

        public HashedBag(IEnumerable<T> items)
            : this(items, EqualityComparer<T>.Default.Equals)
        {
        }

        public HashedBag(IEnumerable<T> items, Equator<T> equator) : base(items, new HashedMap<T, int>(equator))
        {
            this.equator = equator;
        }

        public override IStrictSet<T> ToUnique()
        {
            var keys = Map.Keys;
            var set = new HashedSet<T>(keys, keys.Count, equator);
            return set;
        }
    }
}
