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
    /// A tree set with a left leaning red black tree implementation. The Left-Leaning RB tree is introduced by 
    /// Dr. Robert Sedgewick in a paper in Feb. 2008.
    /// The LL RB tree is designated for less code compared with the standard RB tree implementation. The efficiency of insert
    /// and removal is said to be faster than the standard version.
    /// The code here is the implementation of c#, according to the java code which was presented in Dr. Sedgewick's paper.
    /// The depth of the tree is 2logn
    /// The time complexity of search is O(lgn), of insert is O(lgn), of delete if O(lgn)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LlTreeSet<T> : ITreeSet<T>
    {
        private const bool RED = true;
        private const bool BLACK = false;

        private TreeNode root;

        private IComparer<T> comparer;

        public LlTreeSet() : this(null, Comparer<T>.Default)
        {
        }

        public LlTreeSet(IComparer<T> comparer) : this(null, comparer)
        {

        }

        public LlTreeSet(IEnumerable<T> items) : this(items, Comparer<T>.Default)
        {

        }

        public LlTreeSet(IEnumerable<T> items, IComparer<T> comparer)
        {
            if (null != items)
            {
                foreach (var i in items)
                {
                    Add(i);
                }
            }

            if (null != comparer)
            {
                this.comparer = comparer;
            }
            else
            {
                this.comparer = Comparer<T>.Default;
            }
        }

        public bool Add(T item)
        {
            var result = true;
            try
            {
                root = Insert(root, item);
                root.Color = BLACK;
            }
            catch
            {
                result = false;
            }

            return result;
        }


        public void ExceptWith(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public void IntersectWith(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public bool IsSubsetOf(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public bool IsSupersetOf(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public bool Overlaps(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public bool SetEquals(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public void UnionWith(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        void ICollection<T>.Add(T item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(T item)
        {
            var found = false;
            var node = root;

            while (null != node)
            {
                var cmp = comparer.Compare(item, node.Item);
                if (cmp == 0)
                {
                    found = true;
                    break;
                }
                else if (cmp < 0)
                {
                    node = node.Left;
                }
                else // cmp > 0
                {
                    node = node.Right;
                }
            }

            return found;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        public bool Remove(T item)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<T> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public bool IsSynchronized
        {
            get { throw new NotImplementedException(); }
        }

        public object SyncRoot
        {
            get { throw new NotImplementedException(); }
        }

        private TreeNode Insert(TreeNode node, T item)
        {
            if (null == node)
            {
                return new TreeNode(item);
            }

            if (IsRed(node.Left) && IsRed(node.Right))
            {
                FlipColor(node);
            }

            int result = this.comparer.Compare(item, node.Item);
            if (result == 0)
            {
                throw new ArgumentException("The item to add has already existed");
            }
            else if (result < 0)
            {
                node.Left = Insert(node.Left, item);
            }
            else
            {
                node.Right = Insert(node.Right, item);
            }

            if (IsRed(node.Right) && !IsRed(node.Left))
            {
                node = RotateLeft(node);
            }

            if (IsRed(node.Left) && !IsRed(node.Right))
            {
                node = RotateRight(node);
            }

            return node;
        }

        private TreeNode RotateRight(TreeNode node)
        {
            TreeNode x = node.Left;
            node.Left = x.Right;
            x.Right = node;
            x.Color = node.Color;
            node.Color = RED;
            return x;
        }

        private TreeNode RotateLeft(TreeNode node)
        {
            TreeNode x = node.Right;
            node.Right = x.Left;
            x.Left = node;
            x.Color = node.Color;
            node.Color = RED;
            return x;
        }

        private void FlipColor(TreeNode node)
        {
            node.Color = !node.Color;
            if (null != node.Left)
            {
                node.Left.Color = !node.Left.Color;
            }
            if (null != node.Right)
            {
                node.Right.Color = !node.Right.Color;
            }
        }

        private static bool IsRed(TreeNode node)
        {
            var isRed = false;

            if (null != node)
            {
                isRed = node.Color == RED;
            }
            else
            {
                isRed = true;
            }
            return isRed;
        }

        private class TreeNode
        {
            public T Item { get; set; }
            public TreeNode Left { get; set; }
            public TreeNode Right { get; set; }
            public bool Color { get; set; }

            public TreeNode(T item)
            {
                Item = item;
                Color = RED;
            }
        }
    }
}
