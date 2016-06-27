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
using System.Threading.Tasks;
using System.Threading;

namespace Test.Commons.Collections
{
    public class QueueTest
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

            var rand = new Random((int)DateTime.Now.Ticks & 0x0000ffff);
            var set = new TreeSet<int>();
            var orderSet = new TreeSet<Order>(new OrderComparer());
            var count = 0;
            while (count < 5000)
            {
                var next = rand.Next();
                var order = new Order {Id = next };
                if (!orderSet.Contains(order)) 
                {
                    set.Add(next);
                    orderSet.Add(order);
                    count++;
                }
            }

            var pq4 = new MinPriorityQueue<Order>(orderSet, new OrderComparer().Compare);
            Assert.Equal(5000, pq4.Count);
            Push(pq4, set, rand, 1000);
            Assert.Equal(6000, pq4.Count);
            Pop(pq4, set, 2000);
            Assert.Equal(4000, pq4.Count);
            Push(pq4, set, rand, 10000);
            Assert.Equal(14000, pq4.Count);
        }

        [Fact]
        public void TestMaxPriortyQueueConstructor()
        {
            var pq1 = new MaxPriorityQueue<int>();
            MaxPriorityQueueConstructor(pq1);

            var pq2 = new MaxPriorityQueue<Order>(new OrderComparer());
            Assert.True(pq2.IsEmpty);
            MaxPriorityQueueConstructorWithOrder(pq2);
            Assert.False(pq2.IsEmpty);

            var pq3 = new MaxPriorityQueue<Order>((x1, x2) => x1.Id - x2.Id);
            Assert.True(pq3.IsEmpty);
            MaxPriorityQueueConstructorWithOrder(pq3);
            Assert.False(pq3.IsEmpty);

            var rand = new Random((int)DateTime.Now.Ticks & 0x0000ffff);
            var set = new TreeSet<int>();
            var orderSet = new TreeSet<Order>(new OrderComparer());
            var count = 0;
            while (count < 5000)
            {
                var next = rand.Next();
                var order = new Order {Id = next };
                if (!orderSet.Contains(order)) 
                {
                    set.Add(next);
                    orderSet.Add(order);
                    count++;
                }
            }

            var pq4 = new MaxPriorityQueue<Order>(orderSet, new OrderComparer().Compare);
            Assert.Equal(5000, pq4.Count);
            Push(pq4, set, rand, 1000);
            Assert.Equal(6000, pq4.Count);
            Pop(pq4, set, 2000);
            Assert.Equal(4000, pq4.Count);
            Push(pq4, set, rand, 10000);
            Assert.Equal(14000, pq4.Count);
        }

        [Fact]
        public void TestPriorityQueueEnumerator()
        {
            var pq = new MinPriorityQueue<int>();
            Enumerate(pq);

            var pq2 = new MaxPriorityQueue<int>();
            Enumerate(pq2);
        }

        [Fact]
        public void TestMemQueueOneThread()
        {
            var handler = new DefaultHandler<Order>();
            var memq = new MemQueue<Order>("Test1", handler);
            Assert.Equal("Test1", memq.QueueName);
            Assert.True(memq.IsEmpty);
            memq.Start();

            var tasks = new Task[100];
            for (var i = 0; i < tasks.Length; i++)
            {
                var order = new Order { Id = i };
                tasks[i] = new Task(() => memq.Enqueue(order));
            }

            foreach (var t in tasks)
            {
                t.Start();
            }
            Task.WaitAll(tasks);

            while(!memq.IsEmpty)
            {
                Thread.Sleep(10);
            }

            Thread.Sleep(20);

            Assert.Equal(100, handler.RequestCount);

            memq.Close();

            memq.Enqueue(new Order { Id = 101 });
            memq.Enqueue(new Order { Id = 102 });
            memq.Enqueue(new Order { Id = 103 });

            Assert.Equal(100, handler.RequestCount);
            Assert.True(memq.IsEmpty);
        }

        [Fact]
        public void TestMemQueueOneThreadException()
        {
            var bill = new Bill { Count = 0 };
            var memq = new MemQueue<Order>("Test3", x =>
            {
                if (x.Id % 2 == 0)
                {
                    throw new InvalidOperationException();
                }
                else
                {
                    bill.Count++;
                }
            });

            memq.Start();

            for (var i = 0; i < 100; i++)
            {
                memq.Enqueue(new Order { Id = i } );
            }

            while (!memq.IsEmpty)
            {
            }

            Thread.Sleep(10);
            Assert.Equal(50, bill.Count);
            memq.Close();
        }

        [Fact]
        public void TestMemQueueMultiThread()
        {
            var handler = new ConcurrentHandler<Order>();
            var memq = new MemQueue<Order>("Test2", handler, 4);
            Assert.Equal("Test2", memq.QueueName);
            Assert.True(memq.IsEmpty);

            memq.Start();
            var tasks = new Task[100];
            for (var i = 0; i < tasks.Length; i++)
            {
                var order = new Order { Id = i };
                tasks[i] = new Task(() => memq.Enqueue(order));
            }
            foreach (var t in tasks)
            {
                t.Start();
            }

            Task.WaitAll(tasks);
            while(!memq.IsEmpty)
            {
                Thread.Sleep(10);
            }

            Thread.Sleep(20);
            Assert.Equal(100, handler.RequestCount);

            memq.Close();
            memq.Enqueue(new Order { Id = 101 });
            memq.Enqueue(new Order { Id = 102 });
            memq.Enqueue(new Order { Id = 103 });
            memq.Enqueue(new Order { Id = 104 });

            Assert.Equal(100, handler.RequestCount);
        }

        private void Enumerate(IPriorityQueue<int> pq)
        {
            var set = new TreeSet<int>();
            Fill(pq, set, 5000);
            var count = 0;
            foreach (var item in pq)
            {
                count++;
                Assert.True(set.Contains(item));
            }
            Assert.Equal(5000, count);

            for (var i = 0; i < 2500; i++)
            {
                Assert.True(set.Contains(pq.Pop()));
            }
            Assert.Equal(2500, pq.Count);

            count = 0;
            foreach (var item in pq)
            {
                count++;
                Assert.True(set.Contains(item));
            }
            Assert.Equal(2500, count);

            Fill(pq, set, 1000);
            count = 0;
            foreach(var item in pq)
            {
                count++;
                Assert.True(set.Contains(item));
            }
            Assert.Equal(3500, count);
        }

        private void Fill(IPriorityQueue<int> pq, TreeSet<int> set, int count)
        {
            var rand = new Random((int)DateTime.Now.Ticks & 0x0000ffff);
            var index = 0;
            while (index < count)
            {
                var next = rand.Next();
                if (!set.Contains(next))
                {
                    set.Add(next);
                    pq.Push(next);
                    index++;
                }
            }
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

        private void MaxPriorityQueueConstructorWithOrder(MaxPriorityQueue<Order> pq)
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

        private void Push(MaxPriorityQueue<Order> pq, TreeSet<int> set, Random rand, int count)
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
                    Assert.Equal(set.Max, pq.Top.Id);
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

        private void Pop(MaxPriorityQueue<Order> pq, TreeSet<int> set, int count)
        {
            for (var i = 0; i < count; i++)
            {
                Assert.Equal(set.Max, pq.Top.Id);
                Assert.Equal(set.Max, pq.Pop().Id);
                set.RemoveMax();
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

        private void MaxPriorityQueueConstructor(MaxPriorityQueue<int> pq)
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
                    Assert.Equal(set.Max, pq.Top);
                }
            }
            Assert.False(pq.IsEmpty);
            Assert.Equal(10000, pq.Count);
            for (var i = 0; i < 1000; i++)
            {
                Assert.Equal(set.Max, pq.Top);
                Assert.Equal(set.Max, pq.Pop());
                set.RemoveMax();
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
                Assert.Equal(set.Max, pq.Top);
                Assert.Equal(set.Max, pq.Pop());
                set.RemoveMax();
            }
            Assert.Equal(10000, pq.Count);
        }
    }
}
