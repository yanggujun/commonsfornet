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

using Xunit;

using Commons.Collections.Queue;
using System;
using Commons.Collections.Set;

namespace Test.Commons.Collections
{
    public class PriorityQueueTest
    {
        [Fact]
        public void TestMinPriortyQueueConstructor()
        {
            var pq1 = new MinPriorityQueue<int>();
            MinPriorityQueueConstructor(pq1);

            var pq2 = new MinPriorityQueue<Order>(new OrderComparer());
            Assert.True(pq2.IsEmpty);
            MinPriorityQueueConstructorWithOrder(pq2);
            Assert.False(pq2.IsEmpty);

            var pq3 = new MinPriorityQueue<Order>((x1, x2) => x1.Id - x2.Id);
            Assert.True(pq3.IsEmpty);
            MinPriorityQueueConstructorWithOrder(pq3);
            Assert.False(pq3.IsEmpty);
        }

        [Fact]
        public void TestPriorityQueueBoundary()
        {
            var pq1 = new MinPriorityQueue<int>();
            PriorityQueueBoundary(pq1);

            var pq2 = new MaxPriorityQueue<int>();
            PriorityQueueBoundary(pq2);
        }

        private void PriorityQueueBoundary(IPriorityQueue<int> pq)
        {
            Assert.True(pq.IsEmpty);
            pq.Push(1000);
            Assert.False(pq.IsEmpty);
            Assert.Equal(1, pq.Count);
            Assert.Equal(1000, pq.Top);
            Assert.Equal(1000, pq.Pop());
            Assert.Equal(0, pq.Count);
            Assert.True(pq.IsEmpty);
        }

        private void MinPriorityQueueConstructorWithOrder(MinPriorityQueue<Order> pq)
        {
            Assert.True(pq.IsEmpty);
            var rand = new Random((int)DateTime.Now.Ticks & 0x0000ffff);
            var set = new TreeSet<int>();
            Push(pq, set, rand, 10000);
            Assert.Equal(10000, pq.Count);
            Pop(pq, set, 6000);
            Assert.Equal(4000, pq.Count);
            Push(pq, set, rand, 20000);
            Assert.Equal(24000, pq.Count);
            Pop(pq, set, 14000);
            Assert.Equal(10000, pq.Count);
            Pop(pq, set, 5000);
            Assert.Equal(5000, pq.Count);
            Push(pq, set, rand, 1000);
            Assert.Equal(6000, pq.Count);
            Push(pq, set, rand, 1000);
            Assert.Equal(7000, pq.Count);
        }

        private void Push(MinPriorityQueue<Order> pq, TreeSet<int> set, Random rand, int count)
        {
            var index = 0;
            while (index < count)
            {
                var next = rand.Next();
                if (!set.Contains(next))
                {
                    var order = new Order { Id = next };
                    pq.Push(order);
                    set.Add(next);
                    index++;
                    Assert.Equal(set.Min, pq.Top.Id);
                }
            }
        }

        private void Pop(MinPriorityQueue<Order> pq, TreeSet<int> set, int count)
        {
            for (var i = 0; i < count; i++)
            {
                Assert.Equal(set.Min, pq.Top.Id);
                Assert.Equal(set.Min, pq.Pop().Id);
                set.RemoveMin();
            }
        }

        private void MinPriorityQueueConstructor(MinPriorityQueue<int> pq)
        {
            Assert.True(pq.IsEmpty);
            var rand = new Random((int)DateTime.Now.Ticks & 0x0000ffff);
            var set = new TreeSet<int>();
            var count = 0; 
            while (count < 10000)
            {
                var next = rand.Next() % 100000;
                if (!set.Contains(next))
                {
                    pq.Push(next);
                    set.Add(next);
                    count++;
                    Assert.Equal(set.Min, pq.Top);
                }
            }
            Assert.False(pq.IsEmpty);
            Assert.Equal(10000, pq.Count);
            for (var i = 0; i < 1000; i++)
            {
                Assert.Equal(set.Min, pq.Top);
                Assert.Equal(set.Min, pq.Pop());
                set.RemoveMin();
            }
            Assert.Equal(9000, pq.Count);

            var newCount = 0;
            while (newCount < 5000)
            {
                var next = rand.Next() % 100000;
                if (!set.Contains(next))
                {
                    pq.Push(next);
                    set.Add(next);
                    newCount++;
                }
            }
            Assert.Equal(14000, pq.Count);
            for (var i = 0; i < 4000; i++)
            {
                Assert.Equal(set.Min, pq.Top);
                Assert.Equal(set.Min, pq.Pop());
                set.RemoveMin();
            }
            Assert.Equal(10000, pq.Count);
        }
    }
}
