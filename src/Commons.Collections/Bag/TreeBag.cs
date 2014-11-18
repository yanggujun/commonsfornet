// Copyright CommonsForNET. Author: Gujun Yang. email: gujun.yang@gmail.com
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

namespace Commons.Collections.Bag
{
    [CLSCompliant(true)]
    public class TreeBag<T> : AbstractMapBag<T>, ISortedBag<T>
    {
        public TreeBag()
            : base(new TreeMap<T, int>())
        {
        }

        public TreeBag(Comparison<T> comp)
            : base(new TreeMap<T, int>(comp))
        {
        }

        public TreeBag(IEnumerable<T> items, Comparison<T> comp)
            : base(items, new TreeMap<T, int>(comp))
        {
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
    }
}
