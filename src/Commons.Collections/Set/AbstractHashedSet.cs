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
    public abstract class AbstractHashedSet<T> : AbstractSet<T>, IStrictSet<T>, IReadOnlyStrictSet<T>, ICollection<T>, IReadOnlyCollection<T>, IEnumerable<T>, ICollection, IEnumerable
    {
        protected readonly object val = new object();

        protected abstract IDictionary<T, object> Map { get; }

        public override void Add(T item)
        {
            Map.Add(item, val);
        }

        public override void Clear()
        {
            Map.Clear();
        }

        public override bool Contains(T item)
        {
            return Map.ContainsKey(item);
        }

        public override void CopyTo(T[] array, int arrayIndex)
        {
            array.ValidateNotNull("The input array is null!.");
            var index = arrayIndex;
            foreach (var item in Map)
            {
                array[index++] = item.Key;
            }
        }

        public override int Count
        {
            get { return Map.Count; }
        }

        public override bool Remove(T item)
        {
            return Map.Remove(item);
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
                foreach (var item in Map)
                {
                    yield return item.Key;
                }
            }
        }
    }
}
