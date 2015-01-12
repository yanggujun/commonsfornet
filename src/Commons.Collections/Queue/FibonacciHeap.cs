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

namespace Commons.Collections.Queue
{
    /// <summary>
    /// 
    /// </summary>
    internal class FibonacciHeap<T> : IEnumerable<T>
    {
        private const double FiboRatio = 1.618;
        private readonly Func<T, T, bool> comparer;
        private readonly Func<T, T, bool> reverseComparer;
        private FiboNode TopNode { get; set; }
        public FibonacciHeap(Func<T, T, bool> comparer, Func<T, T, bool> reverseComparer)
        {
            comparer.ValidateNotNull("The comparer is null!");
            this.comparer = comparer;
            this.reverseComparer = reverseComparer;
        }

        public void Add(T item)
        {
            item.ValidateNotNull("The item is null!");
            var node = new FiboNode { Value = item };
            if (TopNode == null)
            {
                node.Left = node.Right = node;
                TopNode = node;
            }
            else
            {
                AddNode(node, TopNode);
                if (comparer(node.Value, TopNode.Value))
                {
                    TopNode = node;
                }
            }
            Count++;
        }

        public T Top
        {
            get
            {
                TopNode.Validate(x => x != null, new ArgumentException("The collection is empty!"));
                return TopNode.Value;
            }
        }

        public T ExtractTop()
        {
			TopNode.Validate(x => x != null, new ArgumentException("The collection is empty."));
            var top = TopNode;
            if (top != null)
            {
                if (top.Child != null)
                {
                    foreach (var childNode in top.Child.Siblings)
                    {
                        AddNode(childNode, TopNode);
						childNode.Parent = null;
                    }
                }
                RemoveNode(top);
                if (ReferenceEquals(top, top.Right))
                {
                    TopNode = null;
                }
                else
                {
                    TopNode = TopNode.Right;
                    Consolidate();
                }
                Count--;
            }

            return top.Value;
        }

        public int Count { get; private set; }

		public bool IsEmpty { get { return TopNode != null; } }

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
				if (TopNode != null)
				{
					foreach(var node in TopNode.Relatives)
					{
						yield return node.Value;
					}
				}
			}
		}

		/// <summary>
		/// Adds a node <paramref name="node"/> to <paramref name="header"/>
		/// </summary>
		/// <param name="node">The node to add.</param>
		/// <param name="header">The header.</param>
        private static void AddNode(FiboNode node, FiboNode header)
        {
			node.Right = header;
            node.Left = header.Left;
            header.Left.Right = node;
            header.Left = node;
        }

		/// <summary>
		/// Removes the <paramref name="node"/> from its list.
		/// </summary>
		/// <param name="node">The node.</param>
        private static void RemoveNode(FiboNode node)
        {
            node.Left.Right = node.Right;
            node.Right.Left = node.Left;
        }

        private void Consolidate()
        {
            var upperBound = Convert.ToInt32(Math.Floor(Math.Log(Count, FiboRatio)));
            var nodeArray = new FiboNode[upperBound];
            var rootNodes = new List<FiboNode>();
            foreach (var node in TopNode.Siblings)
            {
                rootNodes.Add(node);
            }
            foreach (var node in rootNodes)
            {
                var degree = node.Degree;
                while (nodeArray[degree] != null)
                {
                    var another = nodeArray[degree];
                    if (reverseComparer(node.Value, another.Value))
                    {
                        var temp = node.Value;
                        node.Value = another.Value;
                        another.Value = temp;
                    }
					RemoveNode(another);
					MakeChild(another, node);
					another.Mark = false;
					nodeArray[degree] = null;
                    degree++;
                }
				nodeArray[degree] = node;
            }
			TopNode = null;
			foreach (var node in nodeArray)
			{
				if (node != null)
				{
					if (TopNode == null)
					{
						node.Left = node.Right = node;
						TopNode = node;
					}
					else
					{
						AddNode(node, TopNode);
						if (comparer(node.Value, TopNode.Value))
						{
							TopNode = node;
						}
					}
				}
			}
        }

		/// <summary>
		/// Makes the <paramref name="child"/> to its new <param name="parent"/>
		/// </summary>
		/// <param name="child">The child.</param>
		/// <param name="parent">The parent.</param>
        private void MakeChild(FiboNode child, FiboNode parent)
        {
            if (parent.Child == null)
            {
                parent.Child = child;
                parent.Child.Right = parent.Child.Left = child;
            }
            else
            {
                AddNode(child, parent.Child);
            }
			child.Parent = parent;
			parent.Degree++;
        }

        private class FiboNode
        {
            public T Value { get; set; }

            public FiboNode Parent { get; set; }

            public FiboNode Left { get; set; }

            public FiboNode Right { get; set; }

            public FiboNode Child { get; set; }

            public int Degree { get; set; }

            public bool Mark { get; set; }

            /// <summary>
            /// 
            /// </summary>
            /// <remarks>The siblings include the current itself.</remarks>
            public IEnumerable<FiboNode> Siblings
            {
                get
                {
                    var cursor = this;
                    do
                    {
                        yield return cursor;
                        cursor = cursor.Right;
                    } while (!ReferenceEquals(cursor, this));
                }
            }

			public IEnumerable<FiboNode> Relatives
			{
				get
				{
					var cursor = this;
					do
					{
						yield return cursor;
						if (cursor.Child != null)
						{
							foreach (var child in cursor.Child.Relatives)
							{
								yield return child;
							}
						}
						cursor = cursor.Right;
					} while (!ReferenceEquals(cursor, this));
				}
			}
        }
    }
}
