// Copyright CommonsForNET // Licensed to the Apache Software Foundation (ASF) under one or more
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

using Commons.Utils;
using Commons.Collections.Map;

namespace Commons.Collections.Set
{
    [CLSCompliant(true)]
    public class LinkedSet<T> : AbstractHashedSet<T>, IOrderedSet<T>, IStrictSet<T>, IReadOnlyStrictSet<T>
    {
        private readonly LinkedHashedMap<T, object> map;

        public LinkedSet()
        {
            map = new LinkedHashedMap<T, object>();
        }

        public LinkedSet(int capacity)    
        {
            map = new LinkedHashedMap<T, object>(capacity);
        }

        public LinkedSet(IEqualityComparer<T> comparer)
        {
            map = new LinkedHashedMap<T, object>(comparer);
        }

        public LinkedSet(Equator<T> equator)
        {
            map = new LinkedHashedMap<T, object>(equator);
        }

        public LinkedSet(int capacity, IEqualityComparer<T> comparer)
        { 
            map = new LinkedHashedMap<T,object>(capacity, comparer);
        }

        public LinkedSet(int capacity, Equator<T> equator)
        {
            map = new LinkedHashedMap<T, object>(capacity, equator);
        }

        protected override IDictionary<T, object> Map
        {
            get
            {
                return map;
            }
        }

        public T First
        {
            get { return map.First.Key; }
        }

        public T Last
        {
            get { return map.Last.Key; }
        }

        public T After(T item)
        {
            return map.After(item).Key;
        }

        public T Before(T item)
        {
            return map.Before(item).Key;
        }

        public T GetIndex(int index)
        {
            return map.GetIndex(index).Key;
        }
    }
}
