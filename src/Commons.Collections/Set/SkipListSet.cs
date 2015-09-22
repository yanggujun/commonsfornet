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
    public sealed class SkipListSet<T> : AbstractSet<T>, INavigableSet<T>, ISortedSet<T>, IStrictSet<T>, IReadOnlyStrictSet<T>, ICollection<T>, 
#if NET45
		IReadOnlyCollection<T>, 
#endif
		IEnumerable<T>, ICollection, IEnumerable
    {
        private readonly object val = new object();
        private readonly SkipList<T, object> skipList;

        public SkipListSet() :this(Comparer<T>.Default)
        {
        }

        public SkipListSet(IComparer<T> comparer)
            : this(comparer.Compare)
        {
        }

        public SkipListSet(IEnumerable<T> source)
            : this(source, Comparer<T>.Default)
        {
        }

        public SkipListSet(IEnumerable<T> source, IComparer<T> comparer)
            : this(source, comparer.Compare)
        {
        }

        public SkipListSet(IEnumerable<T> source, Comparison<T> comparer)
            : this(comparer)
        {
            if (source != null)
            { 
                foreach (var item in source)
                {
                    Add(item);
                }
            }
        }

        public SkipListSet(Comparison<T> comparer)
        {
            skipList = new SkipList<T, object>(comparer);
        }

        public T Lower(T item)
        {
            return skipList.Lower(item).Key;
        }

        public T Higher(T item)
        {
            return skipList.Higher(item).Key;
        }

        public T Ceiling(T item)
        {
            return skipList.Ceiling(item).Key;
        }

        public T Floor(T item)
        {
            return skipList.Floor(item).Key;
        }

        public override IEnumerator<T> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        void ICollection.CopyTo(Array array, int index)
        {
            array.ValidateNotNull("The array should not be null.");
            var a = (T[])array;
            CopyTo(a, index);
        }

        public override int Count
        {
            get { return skipList.Count; }
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public object SyncRoot
        {
            get { throw new NotSupportedException("SyncRoot is not supported in Commons.Collections."); }
        }

        public void RemoveMin()
        {
            skipList.RemoveMin();
        }

        public void RemoveMax()
        {
            skipList.RemoveMax();
        }

        public T Max
        {
            get { return skipList.Max.Key; }
        }

        public T Min
        {
            get { return skipList.Min.Key; }
        }

        public override void Add(T item)
        {
            skipList.Add(item, val);
        }

        public override void Clear()
        {
            skipList.Clear();
        }

        public override bool Contains(T item)
        {
            return skipList.Contains(item);
        }

        public bool IsEmpty
        {
            get
            {
                return skipList.IsEmpty;
            }
        }

        public override void CopyTo(T[] array, int arrayIndex)
        {
            array.ValidateNotNull("The array should not be null.");
            var index = arrayIndex;
            foreach (var item in skipList)
            {
                array[index++] = item.Key;
            }
        }

        public override bool Remove(T item)
        {
            return skipList.Remove(item);
        }

        protected override IEnumerable<T> Items
        {
            get
            {
                foreach (var item in skipList)
                {
                    yield return item.Key;
                }
            }
        }
    }
}
