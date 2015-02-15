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

namespace Commons.Collections.Bag
{
    [CLSCompliant(true)]
    public class TreeBag<T> : AbstractMapBag<T>, ISortedBag<T>
    {
        private readonly Comparison<T> comparer;
        public TreeBag()
            : this(Comparer<T>.Default)
        {
        }

        public TreeBag(Comparison<T> comparer)
            : this(null, comparer)
        {
        }

        public TreeBag(IComparer<T> comparer) : this(comparer.Compare)
        {
        }

        public TreeBag(IEnumerable<T> items) : this (items, Comparer<T>.Default.Compare)
        {
        }

        public TreeBag(IEnumerable<T> items, Comparison<T> comparer)
            : base(items, new TreeMap<T, int>(comparer))
        {
            this.comparer = comparer;
        }

        public T Max
        {
            get
            {
                if (Map.Count <= 0)
                {
                    throw new InvalidOperationException("The bag is empty");
                }
                return ((ISortedMap<T, int>)Map).Max.Key;
            }
        }

        public T Min
        {
            get
            {
                if (Map.Count <= 0)
                {
                    throw new InvalidOperationException("The bag is empty");
                }
                return ((ISortedMap<T, int>)Map).Min.Key;
            }
        }

        public override IStrictSet<T> ToUnique()
        {
            var keys = Map.Keys;
            var set = new TreeSet<T>(keys, comparer);

            return set;
        }
    }
}
