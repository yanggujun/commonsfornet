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
    public class HashedSet<T> : AbstractSet<T>, IStrictSet<T>, IReadOnlyStrictSet<T>, ICollection<T>, IReadOnlyCollection<T>, IEnumerable<T>, ICollection, IEnumerable
    {
        private readonly object val = new object();
        private readonly HashedMap<T, object> map;

        public HashedSet()
        {
            map = new HashedMap<T, object>();
        }

        public HashedSet(int capacity) : this(capacity, EqualityComparer<T>.Default.Equals)
        {
        }

        public HashedSet(IEqualityComparer<T> equalityComparer) : this(equalityComparer.Equals)
        {
        }

        public HashedSet(Equator<T> equator)
        {
            map = new HashedMap<T, object>(equator);
        }

        public HashedSet(int capacity, IEqualityComparer<T> equalityComparer)
            : this(capacity, equalityComparer.Equals)
        {
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
                    map.Add(item, item);
                }
            }
        }
		public override void Add(T item)
		{
            map.Add(item, val);
		}

		public override void Clear()
		{
            map.Clear();
		}

		public override bool Contains(T item)
		{
            return map.ContainsKey(item);
		}

		public override void CopyTo(T[] array, int arrayIndex)
		{
            array.ValidateNotNull("The input array is null!.");
            var index = arrayIndex;
            foreach (var item in map)
            {
                array[index++] = item.Key;
            }
		}

		public override int Count
		{
            get { return map.Count; }
		}

		public override bool Remove(T item)
		{
            return map.Remove(item);
		}

		public override IEnumerator<T> GetEnumerator()
		{
            return Items.GetEnumerator();
		}

		void ICollection.CopyTo(Array array, int index)
		{
            array.ValidateNotNull("The array is null!");
            var itemArray = array as T[];
			itemArray.Validate(x => x != null, new ArgumentException("The type of the array is not correct."));
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

        protected override IEnumerable<T> Items
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
