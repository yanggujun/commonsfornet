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

namespace Commons.Collections.Collection
{
    /// <summary>
    /// Provides a collection same with how System.Linq.Enumerable.Concat method provides
    /// a concatenated collection.
    /// The composited collections are not formed into a new collection. They are just logically linked together.
    /// And it has the capability to add and remove items from the composite collection.
    /// When two collections are needed to be concatenated and new items are required to be added to the new collection,
    /// the CompositeCollection will be faster than System.Linq.Enumerable.Concat. 
    /// The items in this collection is unordered. The items in the composite collection are not necessarily unique, like the result of 
    /// System.Linq.Enumerable.Concat.
    /// </summary>
    [CLSCompliant(true)]
    public class CompositeCollection<T> : ICollection<T>, IReadOnlyCollection<T>, ICollection, IEnumerable<T>, IEnumerable
    {
        private readonly List<ICollection<T>> allCollections = new List<ICollection<T>>();

        /// <summary>
        /// A collection of collections containing the items of type T.
        /// </summary>
        protected List<ICollection<T>> AllCollections { get { return allCollections; } }

        /// <summary>
        /// Composites a list of collections.
        /// </summary>
        /// <param name="collections"></param>
        public CompositeCollection(params ICollection<T>[] collections)
        {
            AddAll(collections);
        }

        /// <summary>
        /// Adds a item to the composite collection. 
        /// The adding uses the default operation. It adds the item to the last collection 
        /// of the composite collection. If no collection already exists in this composite, 
        /// it creates a list, adds the item to the list and adds the list to the composite.
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            Add(item, (collection, i) =>
            {
                var last = AllCollections.Count > 0 ? AllCollections[AllCollections.Count - 1] : null;
                if (null != last)
                { 
                    last.Add(i);
                }
                else
                {
                    last = new List<T>();
                    last.Add(i);
                    allCollections.Add(last);
                }
            });
        }

        /// <summary>
        /// Adds an item to the composite with a customized the mutator for the current collection.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <param name="mutator">The user defined mutator. The first param is always the current composite collection. 
        /// And the second param is the item to add.</param>
        private void Add(T item, Action<CompositeCollection<T>, T> mutator)
        {
            mutator(this, item);
        }

        /// <summary>
        /// Add a collection of items to the composite collection.
        /// </summary>
        /// <param name="collections"></param>
        public void AddAll(params ICollection<T>[] collections)
        {
			foreach (var collection in collections)
			{
				allCollections.Add(collection);
			}
        }

        public void Clear()
        {
            AllCollections.Clear();
        }

        public bool Contains(T item)
        {
            return Contains(item, (i1, i2) => i1.Equals(i2));
        }

        public bool Contains(T item , IEqualityComparer<T> comparer)
        {
			Guarder.CheckNull(item, comparer);
			return Contains(item, comparer.Equals);
        }

        public bool Contains(T item, Func<T, T, bool> isEqual)
        {
			Guarder.CheckNull(item, isEqual);
            bool hasItem = false;
            foreach (var c in AllCollections)
            {
                foreach (var i in c)
                {
                    hasItem = isEqual(item, i);
                    if (hasItem)
                    {
                        return hasItem;
                    }
                }
            }
            return hasItem;
        }

        /// <inheritdoc/>
        public void CopyTo(T[] array, int arrayIndex)
        {
			Guarder.CheckNull(array);
            if (arrayIndex < 0)
            {
                throw new ArgumentException("The arrayIndex is invalid");
            }
            int n = 0;
            AllCollections.ForEach(c => 
            {
                foreach (var i in c)
                {
                    array[arrayIndex + n++] = i;
                }
            });
        }

        /// <inheritdoc/>
        public int Count
        {
            get
            {
                int count = 0;
                AllCollections.ForEach(c => count += c.Count);
                return count;
            }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <inheritdoc/>
        public bool Remove(T item)
        {
            return Remove(item, (t1, t2) => t1.Equals(t2));
        }

        /// <summary>
        /// Removes the item with a customzied equality comparer.
        /// There can be multiple items with the same value in the composite. Those items are all removed.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <param name="comparer">The equality comparer to compare the item equality.</param>
        /// <returns>True if any item is removed.</returns>
        public bool Remove(T item, IEqualityComparer<T> comparer)
        {
            return Remove(item, (i1, i2) => comparer.Equals(i1, i2));
        }

        /// <summary>
        /// Removes the items with a customized delegate.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="isEqual"></param>
        /// <returns></returns>
        public bool Remove(T item, Func<T, T, bool> isEqual)
        {
            bool removed = false;
            List<T> removeItems = new List<T>();
            foreach (var c in AllCollections)
            {
                foreach (var i in c)
                {
                    if (isEqual(i, item))
                    {
                        removeItems.Add(i);
                    }
                }

                foreach (var i in removeItems)
                {
                    c.Remove(i);
                    removed = true;
                }
            }

            return removed;
        }

        /// <summary>
        /// Creates a list of the items in the composite collection.
        /// The duplicated items are removed.
        /// </summary>
        /// <returns>A list of the items.</returns>
        public IList<T> ToUnique()
        {
            return ToUnique((t1, t2) => t1.Equals(t2));
        }

        public IList<T> ToUnique(IEqualityComparer<T> comparer)
        {
            return ToUnique((t1, t2) => comparer.Equals(t1, t2));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isEqual"></param>
        /// <returns></returns>
        /// TODO: may be in low efficiency.
        public IList<T> ToUnique(Func<T, T, bool> isEqual)
        {
            List<T> list = new List<T>();
            foreach (var c in AllCollections)
            {
                foreach (var i in c)
                {
                    bool notIn = true;
                    foreach (var r in list)
                    {
                        if (isEqual(r, i))
                        {
                            notIn = false;
                            break;
                        }
                    }
                    if (notIn)
                    {
                        list.Add(i);
                    }
                }
            }
            return list;
        }

        public IEnumerator<T> GetEnumerator()
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
				foreach (var c in AllCollections)
				{
					var iter = c.GetEnumerator();
					while (iter.MoveNext())
					{
						yield return iter.Current;
					}
				}
			}
        }

		public void CopyTo(Array array, int index)
		{
			var itemArray = array as T[];
			CopyTo(itemArray, index);
		}

		public bool IsSynchronized
		{
			get { return false; }
		}

		public object SyncRoot
		{
			get { throw new NotSupportedException("The SyncRoot is not supported in the Commons.Collections"); }
		}
	}
}
