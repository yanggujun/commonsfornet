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
    public class HashSet<T> : IStrictSet<T>, IReadOnlyStrictSet<T>, ICollection<T>, IReadOnlyCollection<T>, IEnumerable<T>, ICollection, IEnumerable
    {
        private readonly object val = new object();
        private readonly HashMap<T, object> map;

        public HashSet()
        {
            map = new HashMap<T, object>();
        }

        public HashSet(int capacity) : this(capacity, EqualityComparer<T>.Default.Equals)
        {
        }

        public HashSet(IEqualityComparer<T> equalityComparer) : this(equalityComparer.Equals)
        {
        }

        public HashSet(Equator<T> equator)
        {
            map = new HashMap<T, object>(equator);
        }

        public HashSet(int capacity, IEqualityComparer<T> equalityComparer)
            : this(capacity, equalityComparer.Equals)
        {
        }

        public HashSet(int capacity, Equator<T> equator)
            : this(null, capacity, equator)
        {
        }

        public HashSet(IEnumerable<T> items, int capacity, Equator<T> equator)
        {
            map = new HashMap<T, object>(capacity, equator);
            if (items != null)
            {
                foreach (var item in items)
                {
                    map.Add(item, item);
                }
            }
        }

        public void Intersect(IStrictSet<T> other)
        {
            other.ValidateNotNull("The other set is null!");
        }

        public void Union(IStrictSet<T> other)
        {
            throw new NotImplementedException();
        }

        public void Differ(IStrictSet<T> other)
        {
            throw new NotImplementedException();
        }

        public bool IsSubsetOf(IStrictSet<T> other)
        {
            throw new NotImplementedException();
        }

        public bool IsProperSubsetOf(IStrictSet<T> other)
        {
            throw new NotImplementedException();
        }

        public bool IsEqualWith(IStrictSet<T> other)
        {
            throw new NotImplementedException();
        }

        public bool IsDisjointWith(IStrictSet<T> other)
        {
            throw new NotImplementedException();
        }

        public void Compliment(IStrictSet<T> universe)
        {
            throw new NotImplementedException();
        }

		public void Add(T item)
		{
            map.Add(item, val);
		}

		public void Clear()
		{
            map.Clear();
		}

		public bool Contains(T item)
		{
            return map.ContainsKey(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
            array.ValidateNotNull("The input array is null!.");
            var index = arrayIndex;
            foreach (var item in map)
            {
                array[index++] = item.Key;
            }
		}

		public int Count
		{
            get { return map.Count; }
		}

		public bool IsReadOnly
		{
            get { return false; }
		}

		public bool Remove(T item)
		{
            return map.Remove(item);
		}

		public IEnumerator<T> GetEnumerator()
		{
            return Items.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
            return GetEnumerator();
		}

		public void CopyTo(Array array, int index)
		{
            array.ValidateNotNull("The array is null!");
            var itemArray = array as T[];
            itemArray.Validate(x => x != null, new ArgumentException("The array type is not correct."));
            CopyTo(itemArray, index);
		}

		public bool IsSynchronized
		{
            get { return false; }
		}

		public object SyncRoot
		{
            get { throw new NotSupportedException("The SyncRoot is not supported in Commons.Collections."); }
		}

        private IEnumerable<T> Items
        {
            get
            {
                foreach (var item in map)
                {
                    yield return item.Key;
                }
            }
        }
    }
}
