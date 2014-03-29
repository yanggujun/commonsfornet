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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commons.Collections
{
    /// <summary>
    /// A tree set with a left leaning red black tree implementation. The Left-Leaning RB tree is introduced by 
    /// Dr. Robert Sedgewick in a paper in Feb. 2008.
    /// The LL RB tree is designated to reduce code compared with the standard RB tree implementation. 
    /// The code here is the implementation of c#, according to the java code which was presented in Dr. Sedgewick's paper.
    /// The depth of the tree is 2logn
    /// The time complexity of search is O(lgn), of insert is O(lgn), of delete if O(lgn)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LlrbTreeSet<T> : ITreeSet<T>
    {
        private const bool RED = true;
        private const bool BLACK = false;

        private TreeNode root;

        private IComparer<T> comparer;

        public LlrbTreeSet() : this(null, Comparer<T>.Default)
        {
        }

        public LlrbTreeSet(IComparer<T> comparer) : this(null, comparer)
        {

        }

        public LlrbTreeSet(IEnumerable<T> items) : this(items, Comparer<T>.Default)
        {

        }

        public LlrbTreeSet(IEnumerable<T> items, IComparer<T> comparer)
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

        public void Add(T item)
        {
            root = Insert(root, item);
            root.Color = BLACK;
        }

        public void Clear()
        {
            root = null;
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

        public T Max
        {
            get
            {
                var maxNode = Maximum(root);
                if (null == maxNode)
                {
                    throw new InvalidOperationException("The set is empty");
                }
                return maxNode.Item;
            }
        }

        public T Min
        {
            get
            {
                var minNode = Minimum(root);
                if (null == minNode)
                {
                    throw new InvalidOperationException("The set is empty");
                }
                return minNode.Item;
            }
        }

        public void RemoveMin()
        {
            if (null == root)
            {
                throw new InvalidOperationException("The set is empty");
            }
            root = DeleteMin(root);
            if (null != root)
            { 
                root.Color = BLACK;
            }
        }

        public void RemoveMax()
        {
            if (null == root)
            {
                throw new InvalidOperationException("The set is empty");
            }
            root = DeleteMax(root);
            if (null != root)
            {
                root.Color = BLACK;
            }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            foreach (var item in this)
            {
                array[arrayIndex++] = item;
            }
        }

        public int Count
        {
            get
            {
                return GetCount(root);
            }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(T item)
        {
            var removed = false;
            if (Contains(item))
            {
                root = Delete(root, item);
                root.Color = BLACK;
                removed = true;
            }

            return removed;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return CreateEnumerator(root).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public void CopyTo(Array array, int index)
        {
            T[] items = array as T[];
            CopyTo(items, index);
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public object SyncRoot
        {
            get { return null; }
        }

        private static IEnumerable<T> CreateEnumerator(TreeNode node)
        {
            if (null != node)
            {
                Stack<TreeNode> nodes = new Stack<TreeNode>();
                nodes.Push(node);
                while (nodes.Count != 0)
                {
                    TreeNode current = nodes.Pop();
                    if (null != current.Right)
                    {
                        nodes.Push(current.Right);
                    }
                    yield return current.Item;
                    if (null != current.Left)
                    {
                        nodes.Push(current.Left);
                    }
                }
            }
        }

        private TreeNode Insert(TreeNode node, T item)
        {
            if (null == node)
            {
                return new TreeNode(item);
            }
            // whether it is a 4-node.
            if (IsRed(node.Left) && IsRed(node.Right))
            {
                // splits a 4-node.
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

            // left lean the 3-node
            if (IsRed(node.Right) && !IsRed(node.Left))
            {
                node = RotateLeft(node);
            }
            // balance the node to a 4-node.
            if (IsRed(node.Left) && IsRed(node.Left.Left))
            {
                node = RotateRight(node);
            }

            return node;
        }

        private static int GetCount(TreeNode node)
        {
            int count = 0;
            if (null != node)
            {
                count++;
                count += GetCount(node.Right);
                count += GetCount(node.Left);
            }

            return count;
        }

        private TreeNode Delete(TreeNode node, T item)
        {
            if (comparer.Compare(item, node.Item) < 0)
            {
                // since before delete, it already checks the item exists, so here it can guarrantee that node.left exits.
                if (!IsRed(node.Left) && !IsRed(node.Left.Left))
                {
                    node = MoveRedLeft(node);
                }
                node.Left = Delete(node.Left, item);
            }
            else
            {
                if (IsRed(node.Left))
                {
                    node = RotateRight(node);
                }
                
                if ((comparer.Compare(item, node.Item) == 0) && (node.Right == null))
                {
                    return null;
                }

                if (!IsRed(node.Right) && !IsRed(node.Right.Left))
                {
                    node = MoveRedRight(node);
                }
                if (comparer.Compare(item, node.Item) == 0)
                {
                    node.Item = Minimum(node.Right).Item;
                    node.Right = DeleteMin(node.Right);
                }
                else
                {
                    node.Right = Delete(node.Right, item);
                }
            }
            return FixUp(node);
        }

        private static TreeNode Minimum(TreeNode root)
        {
            TreeNode node = root;
            TreeNode minimum = node;

            while (null != node)
            {
                minimum = node;
                node = node.Left;
            }
            return minimum;
        }

        private static TreeNode Maximum(TreeNode root)
        {
            TreeNode node = root;
            TreeNode maximum = node;

            while (null != node)
            {
                maximum = node;
                node = node.Right;
            }

            return maximum;
        }

        private static TreeNode DeleteMin(TreeNode node)
        {
            if (null == node.Left)
            {
                return null;
            }

            //Converts the 2-node to a 3-node.
            if (!IsRed(node.Left) && !IsRed(node.Left.Left))
            {
                node = MoveRedLeft(node);
            }

            node.Left = DeleteMin(node.Left);

            return FixUp(node);
        }

        private static TreeNode DeleteMax(TreeNode node)
        {
            if (IsRed(node.Left))
            {
                node = RotateRight(node);
            }

            if (null == node.Right)
            {
                return null;
            }

            if (!IsRed(node.Right) && !IsRed(node.Right.Left))
            {
                node = MoveRedRight(node);
            }

            node.Right = DeleteMax(node.Right);

            return FixUp(node);
        }

        private static TreeNode FixUp(TreeNode node)
        {
            if (IsRed(node.Right))
            {
                node = RotateLeft(node);
            }
            if (IsRed(node.Left) && IsRed(node.Left.Left))
            {
                node = RotateRight(node);
            }
            if (IsRed(node.Left) && IsRed(node.Right))
            {
                FlipColor(node);
            }

            // make sure no right leaning red nodes.
            if (null != node.Left && IsRed(node.Left.Right) && !IsRed(node.Left.Left))
            {
                node.Left = RotateLeft(node.Left);
                if (IsRed(node.Left))
                {
                    node = RotateRight(node);
                }
            }

            return node;
        }

        private static TreeNode RotateRight(TreeNode node)
        {
            TreeNode x = node.Left;
            node.Left = x.Right;
            x.Right = node;
            x.Color = node.Color;
            node.Color = RED;
            return x;
        }

        private static TreeNode RotateLeft(TreeNode node)
        {
            TreeNode x = node.Right;
            node.Right = x.Left;
            x.Left = node;
            x.Color = node.Color;
            node.Color = RED;
            return x;
        }

        private static void FlipColor(TreeNode node)
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

        private static TreeNode MoveRedLeft(TreeNode node)
        {
            FlipColor(node);
            if (IsRed(node.Right.Left))
            {
                node.Right = RotateRight(node.Right);
                node = RotateLeft(node);
                FlipColor(node);
                if (IsRed(node.Right.Right))
                {
                    node.Right = RotateLeft(node.Right);
                }
            }
            return node;
        }

        private static TreeNode MoveRedRight(TreeNode node)
        {
            FlipColor(node);
            if ((null != node.Left) && IsRed(node.Left.Left))
            {
                node = RotateRight(node);
                FlipColor(node);
            }
            return node;
        }

        private static bool IsRed(TreeNode node)
        {
            return node == null ? false : node.Color == RED;
        }

#if DEBUG
        private static void WriteNodeHtml(TreeNode node)
        {
            using (FileStream fs = new FileStream(@"c:\misc\node.html", FileMode.Create))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.Write(NodeHtmlDoc(node));
                    sw.Flush();
                    sw.Close();
                }
            }
        }

        private void WriteToHtml()
        {
            using (FileStream fs = new FileStream(@"c:\misc\tree.html", FileMode.Create))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.Write(this.HtmlDoc);
                    sw.Flush();
                    sw.Close();
                }
            }
        }

        private static string NodeHtmlDoc(TreeNode node)
        {
            return
                    "<html>" +
                    "<body>" +
                        (null != node ? node.HtmlFragment : "[null]") +
                    "</body>" +
                "</html>";

        }

        private string HtmlDoc
        {
            get
            {
                return 
                    "<html>" +
                    "<body>" +
                        (null != root ? root.HtmlFragment : "[null]") +
                    "</body>" +
                "</html>";

            }
        }

        [DebuggerDisplay("Item = {Item}, Color = {Color}")]
#endif
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

#if DEBUG
            public string HtmlFragment
            {
                get
                {
                    return
                            "<table border='1'>" +
                                "<tr>" +
                                    "<td colspan='2' align='center' bgcolor='" + (Color ? "red" : "grey") + "'>" + Item + "</td>" +
                                "</tr>" +
                                "<tr>" +
                                    "<td valign='top'>" + (null != Left ? Left.HtmlFragment : "[null]") + "</td>" +
                                    "<td valign='top'>" + (null != Right ? Right.HtmlFragment : "[null]") + "</td>" +
                                "</tr>" +
                            "</table>";

                }
            }
#endif
        }
    }
}
