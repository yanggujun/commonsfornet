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

namespace Commons.Collections
{
    /// <summary>
    /// Tree set implemented as a left leaning red black tree. A left leaning red black tree is a red black tree 
    /// in which the red nodes are always leaning to the left.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TreeSet<T> : ITreeSet<T>
    {
        LlrbTree<T, T> llrbTree;
        public TreeSet()
        {
            llrbTree = new LlrbTree<T, T>();
        }

        public TreeSet(IComparer<T> comparer)
        {
        }

        public TreeSet(IEnumerable<T> items)
        {
        }

        public TreeSet(IEnumerable<T> items, IComparer<T> comparer)
        {
            IComparer<T> theComparer = comparer;
            if (null == comparer)
            {
                theComparer = Comparer<T>.Default;
            }
            llrbTree = new LlrbTree<T, T>(theComparer);
            if (null != items)
            {
                foreach (var i in items)
                {
                    Add(i);
                }
            }
        }

        public void Add(T item)
        {
            llrbTree.Add(item, item);
        }

        public void Clear()
        {
            llrbTree.Clear();
        }

        public bool Contains(T item)
        {
            return llrbTree.Contains(item);
        }

        public T Max
        {
            get
            {
                return llrbTree.Max.Key;
            }
        }

        public T Min
        {
            get
            {
                return llrbTree.Min.Key;
            }
        }

        public void RemoveMin()
        {
            llrbTree.RemoveMin();
        }

        public void RemoveMax()
        {
            llrbTree.RemoveMax();
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            var index = 0;
            foreach (var i in llrbTree)
            {
                array[index++] = i.Key;
            }
        }

        public int Count
        {
            get
            {
                return llrbTree.Count;
            }
        }

        public bool IsReadOnly
        {
            get { return llrbTree.IsReadOnly; }
        }

        public bool Remove(T item)
        {
            return llrbTree.Remove(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return CreateEnumerator().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public void CopyTo(Array array, int index)
        {
            T[] a = (T[])array;
            CopyTo(a, index);
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public object SyncRoot
        {
            get { return null; }
        }

        public IEnumerable<T> CreateEnumerator()
        {
            foreach (var item in llrbTree)
            {
                yield return item.Key;
            }
        }
    }
}
