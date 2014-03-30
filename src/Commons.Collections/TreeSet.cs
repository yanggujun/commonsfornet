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
        LlrbTreeSet<T> llrbTree;
        public TreeSet()
        {
            llrbTree = new LlrbTreeSet<T>();
        }

        public TreeSet(IComparer<T> comparer)
        {
            llrbTree = new LlrbTreeSet<T>(comparer);
        }

        public TreeSet(IEnumerable<T> items)
        {
            llrbTree = new LlrbTreeSet<T>(items);
        }

        public TreeSet(IEnumerable<T> items, IComparer<T> comparer)
        {
            llrbTree = new LlrbTreeSet<T>(items, comparer);
        }

        public void Add(T item)
        {
            llrbTree.Add(item);
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
                return llrbTree.Max;
            }
        }

        public T Min
        {
            get
            {
                return llrbTree.Min;
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
            llrbTree.CopyTo(array, arrayIndex);
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
            return llrbTree.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public void CopyTo(Array array, int index)
        {
            llrbTree.CopyTo(array, index);
        }

        public bool IsSynchronized
        {
            get { return llrbTree.IsSynchronized; }
        }

        public object SyncRoot
        {
            get { return llrbTree.SyncRoot; }
        }
    }
}
