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
using Commons.Collections.Set;
using Commons.Utils;

namespace Commons.Collections.Bag
{
    [CLSCompliant(true)]
    public abstract class AbstractMapBag<T> : IBag<T>, ICollection<T>, 
#if NET45
        IReadOnlyCollection<T>, 
#endif
        ICollection, IEnumerable<T>, IEnumerable
    {
        protected IDictionary<T, int> Map { get; private set; }

        protected AbstractMapBag(IDictionary<T, int> map)
        {
            Map = map;
        }

        protected AbstractMapBag(IEnumerable<T> items, IDictionary<T, int> map)
        {
            Map = map;
            if (items != null)
            { 
                foreach (var item in items)
                {
                    if (Map.ContainsKey(item))
                    {
                        Map[item]++;
                    }
                    else
                    {
                        Map.Add(item, 1);
                    }
                }
            }
        }

        public virtual int this[T item]
        {
            get
            {
                Guarder.CheckNull(item);
                if (!Map.ContainsKey(item))
                {
                    throw new ArgumentException("The item does not exist in the bag");
                }
                return Map[item];
            }
        }

        public virtual void Add(T item, int copies)
        {
            copies.Validate(x => x > 0, new ArgumentException("The copies must be larger than 0!"));
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
            Guarder.CheckNull(item);
            copies.Validate(x => x > 0, new ArgumentException("The copies must be larger than 0!"));
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
            Guarder.CheckNull(item);
            var removed = false;
            if (Map.ContainsKey(item))
            {
                Map.Remove(item);
                removed = true;
            }
            return removed;
        }

        public abstract IStrictSet<T> ToUnique();

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
            array.ValidateNotNull("The array is null!");
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
            return Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private IEnumerable<T> Items
        {
            get
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

        void ICollection.CopyTo(Array array, int index)
        {
            var itemArray = array as T[];
            CopyTo(itemArray, index);
        }

        int ICollection.Count
        {
            get { return Count; }
        }

        bool ICollection.IsSynchronized
        {
            get { return false; }
        }

        object ICollection.SyncRoot
        {
            get { throw new NotSupportedException("The SyncRoot is not supported in the Commons.Collections."); }
        }
    }
}
