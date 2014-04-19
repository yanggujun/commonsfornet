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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commons.Collections.Bag
{
    [CLSCompliant(true)]
    public abstract class AbstractMapBag<T> : IBag<T>
    {
        protected IDictionary<T, int> Map { get; private set; }

        protected AbstractMapBag(IDictionary<T, int> map)
        {
            Map = map;
        }

        public virtual int GetCount(T item)
        {
            return Map[item];
        }

        public virtual void Add(T item, int copies = 1)
        {
            if (Map.ContainsKey(item))
            {
                Map[item] += copies;
            }
            else
            {
                Map.Add(item, copies);
            }
        }

        public virtual bool Remove(T item, int copies)
        {
            var removed = false;
            if (Map.ContainsKey(item))
            {
                var count = Map[item];
                if (count > copies)
                {
                    Map[item] -= copies;
                }
                else
                {
                    Remove(item);
                }
                removed = true;
            }
            return removed;
        }

        public virtual bool Remove(T item)
        {
            var removed = false;
            if (Map.ContainsKey(item))
            {
                Map.Remove(item);
                removed = true;
            }
            return removed;
        }

        public virtual bool ContainsAll(ICollection<T> collection)
        {
            var result = true;
            foreach (var item in collection)
            {
                if (!Map.ContainsKey(item))
                {
                    result = false;
                    break;
                }
            }
            return result;
        }

        public virtual bool RemoveAll(ICollection<T> collection)
        {
            var result = false;
            foreach (var item in collection)
            {
                result = result || Remove(item, 1);
            }
            return result;
        }

        public virtual bool RetainAll(ICollection<T> collection)
        {
            var toRemove = new List<T>();
            foreach (var item in Map.Keys)
            {
                if (!collection.Contains(item))
                {
                    toRemove.Add(item);
                }
            }

            return RemoveAll(toRemove);
        }

        public virtual ISet<T> ToUnique()
        {
            return new SortedSet<T>(Map.Keys);
        }

        public virtual void Add(T item)
        {
            Add(item, 1);
        }

        public virtual void Clear()
        {
            Map.Clear();
        }

        public virtual bool Contains(T item)
        {
            return Map.ContainsKey(item);
        }

        public virtual void CopyTo(T[] array, int arrayIndex)
        {
            foreach (var item in this)
            {
                array[arrayIndex++] = item;
            }
        }

        public virtual int Count
        {
            get
            {
                var count = 0;
                foreach (var item in Map)
                {
                    count += item.Value;
                }

                return count;
            }
        }

        public virtual bool IsReadOnly
        {
            get { return false; }
        }

        public virtual IEnumerator<T> GetEnumerator()
        {
            return CreateEnumerator().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        private IEnumerable<T> CreateEnumerator()
        {
            foreach (var key in Map.Keys)
            {
                var copies = Map[key];
                for (var i = 0; i < copies; i++)
                {
                    yield return key;
                }
            }
        }
    }
}
