// Copyright CommonsForNET.  // Licensed to the Apache Software Foundation (ASF) under one or more
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
                AddToRoot(node);
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
            var minNode = MinNode;
            if (minNode != null)
            {
                if (minNode.Child != null)
                {
                    foreach (var childNode in minNode.Child.Siblings)
                    {
                        AddToRoot(childNode);
                    }
                }
                RemoveFromRoot(minNode);
                if (ReferenceEquals(minNode, minNode.Right))
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

        public IEnumerator<T> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        private IEnumerable<FiboNode> Siblings(FiboNode start)
        {
            if (start != null)
            {
                var cursor = start;
                do
                {
                    yield return cursor;
                    cursor = cursor.Right;
                } while (ReferenceEquals(cursor, start));
            }
        }

        private void AddToRoot(FiboNode node)
        {
            node.Right = RootHeader;
            node.Left = RootHeader.Left;
            RootHeader.Left.Right = node;
            RootHeader.Left = node;
            node.Parent = null;
        }

        private void RemoveFromRoot(FiboNode node)
        {
            node.Left.Right = node.Right;
            node.Right.Left = node.Left;
        }

        private void Consolidate()
        {
            int upperBound = Convert.ToInt32(Math.Floor(Math.Log(Count, FiboRatio)));
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
                }
            }
        }

        private void Link(FiboNode another, FiboNode origin)
        {
            RemoveFromRoot(another);
            if (origin.Child == null)
            {
                origin.Child = another;
            }
            origin.Child.Left.Right = another;
            another.Right = origin.Child;
            another.Left = origin.Child.Left;
            origin.Child.Left = another;
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
        }
    }
}
