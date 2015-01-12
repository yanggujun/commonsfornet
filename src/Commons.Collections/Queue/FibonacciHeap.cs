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
        private readonly Comparison<T> comparer;
        private FiboNode RootHeader { get; set; }
        private FiboNode MinNode { get; set; }
        public FibonacciHeap(Comparison<T> comparer)
        {
            comparer.ValidateNotNull("The comparer is null!");
            this.comparer = comparer;
        }

        public void Add(T item)
        {
            item.ValidateNotNull("The item is null!");
            var node = new FiboNode { Value = item };
            if (RootHeader == null)
            {
                node.Left = node.Right = node;
                RootHeader = node;
                MinNode = RootHeader;
            }
            else
            {
                AddNode(node, RootHeader);
                if (comparer(node.Value, MinNode.Value) < 0)
                {
                    MinNode = node;
                }
            }
            Count++;
        }

        public T Min
        {
            get
            {
                RootHeader.Validate(x => x != null, new ArgumentException("The collection is empty!"));
                return MinNode.Value;
            }
        }

        public T ExtractMin()
        {
			MinNode.Validate(x => x != null, new ArgumentException("The collection is empty."));
            if (MinNode != null)
            {
                if (MinNode.Child != null)
                {
                    foreach (var childNode in MinNode.Child.Siblings)
                    {
                        AddNode(childNode, RootHeader);
						childNode.Parent = null;
                    }
                }
                Remove(MinNode);
                if (ReferenceEquals(MinNode, MinNode.Right))
                {
                    MinNode = null;
                }
                else
                {
                    MinNode = MinNode.Right;
                    Consolidate();
                }
                Count--;
            }

            return MinNode.Value;
        }

        public int Count { get; private set; }

		public bool IsEmpty { get { return MinNode != null; } }

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
				if (RootHeader != null)
				{
					foreach(var node in RootHeader.Relatives)
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
        private static void Remove(FiboNode node)
        {
            node.Left.Right = node.Right;
            node.Right.Left = node.Left;
        }

        private void Consolidate()
        {
            var upperBound = Convert.ToInt32(Math.Floor(Math.Log(Count, FiboRatio)));
            var nodeArray = new FiboNode[upperBound];
            foreach (var node in RootHeader.Siblings)
            {
                var degree = node.Degree;
                while (nodeArray[degree] != null)
                {
                    var another = nodeArray[degree];
                    if (comparer(node.Value, another.Value) > 0)
                    {
                        var temp = node.Value;
                        node.Value = another.Value;
                        another.Value = temp;
                    }
					Remove(another);
					MakeChild(another, node);
					another.Mark = false;
					nodeArray[degree] = null;
					node.Degree++;
                }
				nodeArray[node.Degree] = node;
            }
			MinNode = null;
			foreach (var node in nodeArray)
			{
				if (node != null)
				{
					if (MinNode == null)
					{
						node.Left = node.Right = node;
						RootHeader = node;
						MinNode = RootHeader;
					}
					else
					{
						AddNode(node, RootHeader);
						if (comparer(node.Value, MinNode.Value) < 0)
						{
							MinNode = node;
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
            }
			AddNode(child, parent.Child);
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
                    } while (ReferenceEquals(cursor, this));
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
					} while (ReferenceEquals(cursor, this));
				}
			}
        }
    }
}
