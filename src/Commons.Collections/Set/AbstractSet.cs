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

using Commons.Utils;

namespace Commons.Collections.Set
{
    [CLSCompliant(true)]
    public abstract class AbstractSet<T> : IStrictSet<T>, IReadOnlyStrictSet<T>
    {
        public IStrictSet<T> Intersect(IStrictSet<T> other)
        {
            other.ValidateNotNull("The other set is null!");
            var itemArray = new T[Count];
            var index = 0;
            foreach (var item in Items)
            {
                if (!other.Contains(item))
                {
                    itemArray[index++] = item;
                }
            }

            foreach (var item in itemArray)
            {
                if (item == null)
                {
                    break;
                }
                Remove(item);
            }

            return this;
        }

        public IStrictSet<T> Union(IStrictSet<T> other)
        {
            other.ValidateNotNull("The other set is null!");
            foreach (var item in other)
            {
                if (!Contains(item))
                {
                    Add(item);
                }
            }

            return this;
        }

        public IStrictSet<T> Differ(IStrictSet<T> other)
        {
            other.ValidateNotNull("The other set is null!");
            foreach (var item in other)
            {
                if (Contains(item))
                {
                    Remove(item);
                }
            }

            return this;
        }

        public bool IsSubsetOf(IStrictSet<T> other)
        {
            other.ValidateNotNull("The other set is null!");
            var isSubset = true;
            foreach (var item in Items)
            {
                if (!other.Contains(item))
                {
                    isSubset = false;
                    break;
                }
            }

            return isSubset;
        }

        public bool IsProperSubsetOf(IStrictSet<T> other)
        {
            var isSubset = IsSubsetOf(other);
	        return isSubset && other.Count > Count;
        }

        public bool IsEqualWith(IStrictSet<T> other)
        {
			return other.IsSubsetOf(this) && IsSubsetOf(other);
        }

        public bool IsDisjointWith(IStrictSet<T> other)
        {
			other.ValidateNotNull("The other set is null!");
			var disjoint = true;
			foreach (var item in other)
			{
				if (Contains(item))
				{
					disjoint = false;
					break;
				}
			}

			return disjoint;
        }

        public IStrictSet<T> Compliment(IStrictSet<T> universe)
        {
			if (!IsSubsetOf(universe))
			{
				throw new InvalidOperationException("Cannot calculate the compliment, as the current set is not the subset of the universe set.");
			}
			Clear();
			foreach(var item in universe)
			{
				Add(item);
			}

            return this;
        }

        public abstract void Add(T item);

        public abstract void Clear();

        public abstract bool Contains(T item);

        public abstract void CopyTo(T[] array, int arrayIndex);

        public abstract int Count { get; }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public abstract bool Remove(T item);

        public abstract IEnumerator<T> GetEnumerator();

        protected abstract IEnumerable<T> Items { get; }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
