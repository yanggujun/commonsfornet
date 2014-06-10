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

using Commons.Collections.Common;

namespace Commons.Collections.Set
{
    /// <summary>
    /// Tree set implemented as a left leaning red black tree. A left leaning red black tree is a red black tree 
    /// in which the red nodes are always leaning to the left.
    /// </summary>
    /// <typeparam name="T">Type of the items in the set.</typeparam>
    [CLSCompliant(true)]
    public sealed class TreeSet<T> : ISortedSet<T>
    {
        LlrbTree<T, T> llrbTree;
        /// <summary>
        /// Constructs an empty tree set using the default item comparer.
        /// </summary>
        public TreeSet() : this(null, Comparer<T>.Default)
        {
        }

        /// <summary>
        /// Constructs an empty tree set with the specified comparer.
        /// </summary>
        /// <param name="comparer">The item comparer.</param>
        public TreeSet(IComparer<T> comparer) : this(null, comparer)
        {
        }

        /// <summary>
        /// Constructs a tree set with the items in the enumerable object, using the default item comparer.
        /// </summary>
        /// <param name="items">enumerable items.</param>
        public TreeSet(IEnumerable<T> items) : this(items, Comparer<T>.Default)
        {
        }

        /// <summary>
        /// Constructs a tree set with the items in the enumerable object, using the specified comparer.
        /// </summary>
        /// <param name="items">The enumerable items.</param>
        /// <param name="comparer">The item comparer.</param>
        public TreeSet(IEnumerable<T> items, IComparer<T> comparer) : this(items, (k1, k2) => comparer.Compare(k1, k2))
        {
        }

        public TreeSet(Comparison<T> comparison) : this(null, comparison)
        {
        }

        public TreeSet(IEnumerable<T> items, Comparison<T> comparison)
        {
            Comparison<T> comp = comparison;
            if (null == comp)
            {
                comp = (k1, k2) => Comparer<T>.Default.Compare(k1, k2);
            }
            llrbTree = new LlrbTree<T, T>(comp);
            if (null != items)
            {
                foreach (var i in items)
                {
                    Add(i);
                }
            }
        }

        /// <summary>
        /// Adds an item to the set.
        /// If the item already exists, an exception is thrown.
        /// </summary>
        /// <param name="item">The item to add.</param>
        public void Add(T item)
        {
            llrbTree.Add(item, item);
        }

        /// <summary>
        /// Clears the set.
        /// </summary>
        public void Clear()
        {
            llrbTree.Clear();
        }

        /// <summary>
        /// Checks whether the item exists in the set by using the item comparer.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns>True if the item exists. Otherwise false.</returns>
        public bool Contains(T item)
        {
            return llrbTree.Contains(item);
        }

        /// <summary>
        /// Retrieves the item with max value in the set.
        /// If there is no item in the tree, an exception is thrown.
        /// </summary>
        public T Max
        {
            get
            {
                return llrbTree.Max.Key;
            }
        }

        /// <summary>
        /// Retrieves the item with minimum value in the set.
        /// If there is no item in the tree, an exception is thrown.
        /// </summary>
        public T Min
        {
            get
            {
                return llrbTree.Min.Key;
            }
        }

        /// <summary>
        /// Removes the minimum value in the set.
        /// If there is no item in the tree, an exception is thrown.
        /// </summary>
        public void RemoveMin()
        {
            llrbTree.RemoveMin();
        }

        /// <summary>
        /// Removes the maximum value in the set.
        /// If there is no item in the tree, an exception is thrown.
        /// </summary>
        public void RemoveMax()
        {
            llrbTree.RemoveMax();
        }

        /// <summary>
        /// Copies the items in the set to an array, start from the arrayIndex.
        /// </summary>
        /// <param name="array">The array to which items are copied.</param>
        /// <param name="arrayIndex">The index to start to copy the items.</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            Guarder.CheckNull(array);
            var index = arrayIndex;
            foreach (var i in llrbTree)
            {
                array[index++] = i.Key;
            }
        }

        /// <summary>
        /// Retrieves the item count of the set.
        /// </summary>
        public int Count
        {
            get
            {
                return llrbTree.Count;
            }
        }

        /// <summary>
        /// The tree set is not read only.
        /// </summary>
        public bool IsReadOnly
        {
            get { return llrbTree.IsReadOnly; }
        }

        /// <summary>
        /// Remove the item from the tree set. 
        /// If there is no such item existing in the set, an exception is thrown.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <returns>True if the item is removed. Otherwise false.</returns>
        public bool Remove(T item)
        {
            return llrbTree.Remove(item);
        }

        /// <summary>
        /// Retrieves the enumerator of the tree set.
        /// </summary>
        /// <returns>The enumerator of the items in the tree set.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return CreateEnumerator().GetEnumerator();
        }

        /// <summary>
        /// Retrieves the enumerator of the tree set.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Copies the items in the set to the array from the index.
        /// </summary>
        /// <param name="array">The array to which the items are copied.</param>
        /// <param name="index">The start index of the array.</param>
        public void CopyTo(Array array, int index)
        {
            Guarder.CheckNull(array);
            T[] a = (T[])array;
            CopyTo(a, index);
        }

        /// <summary>
        /// The tree set is not synchornized.
        /// </summary>
        public bool IsSynchronized
        {
            get { return false; }
        }

        /// <summary>
        /// The sync root object is not set.
        /// </summary>
        public object SyncRoot
        {
            get { return null; }
        }

        private IEnumerable<T> CreateEnumerator()
        {
            foreach (var item in llrbTree)
            {
                yield return item.Key;
            }
        }
    }
}
