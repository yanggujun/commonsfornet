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
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using Commons.Collections.Concurrent;
using Xunit;

namespace Test.Commons.Collections
{
	public class ConcurrentListTest
	{
		private readonly object locker = new object();

		[Fact]
		public void TestSingleThreadAdd()
		{
			var list = new ConcurrentSortedList<Order>(new OrderComparer());
			var random = new Random((int) DateTime.Now.Ticks & 0x0000ffff);
			for (var i = 0; i < 10000; i++)
			{
				var number = random.Next();
				var order = new Order {Id = number, Name = number.ToString()};
				list.Add(order);
			}
			AssertList(list, 10000);
		}

		[Fact]
		public void TestMultithreadsAdd()
		{
			var list = new ConcurrentSortedList<Order>(new OrderComparer());
			var tasks = new Task[10000];
			var random = new Random((int) DateTime.Now.Ticks & 0x0000ffff);
			for (var i = 0; i < 10000; i++)
			{
				tasks[i] = Task.Factory.StartNew(() =>
				{
					var id = 0;
					lock (locker)
					{
						id = random.Next();
					}
					list.Add(new Order{Id = id, Name = id.ToString()});
				});
			}
			Task.WaitAll(tasks);
			AssertList(list, 10000);
		}

        [Fact]
        public void TestParallelAdd()
        {
			var list = new ConcurrentSortedList<Order>(new OrderComparer());
			var random = new Random((int) DateTime.Now.Ticks & 0x0000ffff);
            var idList = new List<int>();
			for (var i = 0; i < 10000; i++)
			{
                idList.Add(random.Next());
			}
            Parallel.ForEach(idList, x => list.Add(new Order { Id = x, Name = x.ToString() }));
			AssertList(list, 10000);
        }

        [Fact]
        public void TestSingleThreadContains()
        {
            var list = new ConcurrentSortedList<Order>(new OrderComparer());
            var idList = ConstructIdList(10000);
            Parallel.ForEach(idList, x => list.Add(new Order { Id = x, Name = x.ToString() }));
            foreach (var i in idList)
            {
                Assert.True(list.Contains(new Order { Id = i, Name = i.ToString() }));
            }
            var random = new Random((int)DateTime.Now.Ticks & 0x0000ffff);
            for (var i = 0; i < 20; i++)
            {
                var rand = random.Next();
                if (!idList.Contains(rand))
                {
                    Assert.False(list.Contains(new Order {Id = rand, Name = rand.ToString()}));
                }
            }
        }

        [Fact]
        public void TestSingleThreadRemove()
        {
            var list = new ConcurrentSortedList<Order>(new OrderComparer());
            var idList = ConstructIdList(10000);
            Parallel.ForEach(idList, x => list.Add(new Order { Id = x, Name = x.ToString() }));
            for (var i = 0; i < 1000; i++)
            {
                Assert.True(list.Remove(new Order { Id = idList[i], Name = idList[i].ToString() }));
            }
            Assert.Equal(9000, list.Count);
            for (var i = 0; i < 1000; i++)
            {
                Assert.False(list.Contains(new Order { Id = idList[i], Name = idList[i].ToString() }));
            }
            for (var i = 0; i < 9000; i++)
            {
                Assert.True(list.Contains(new Order { Id = idList[i + 1000], Name = idList[i + 1000].ToString() }));
            }
        }

        [Fact]
        public void TestSingleThreadClear()
        {
            var list = new ConcurrentSortedList<Order>(new OrderComparer());
            var idList = ConstructIdList(10000);
            Parallel.ForEach(idList, x => list.Add(new Order { Id = x, Name = x.ToString() }));
            list.Clear();
            Assert.Equal(0, list.Count);
            for (var i = 0; i < 100; i++)
            {
                Assert.False(list.Contains(new Order { Id = idList[i], Name = idList[i].ToString() }));
            }
        }

        [Fact]
        public void TestSingleThreadRemoveNotExist()
        {
            var list = new ConcurrentSortedList<Order>(new OrderComparer());
            var idList = ConstructIdList(10000);
            Parallel.ForEach(idList, x => list.Add(new Order { Id = x, Name = x.ToString() }));
            var random = new Random((int)DateTime.Now.Ticks & 0x0000ffff);
            for (var i = 0; i < 1000; i++)
            {
                var rand = random.Next();
                if (!idList.Contains(rand))
                {
                    Assert.False(list.Remove(new Order { Id = rand, Name = rand.ToString()}));
                }
            }
        }

        [Fact]
        public void TestSingleThreadAddDuplicate()
        {
            var list = new ConcurrentSortedList<Order>(new OrderComparer());
            var idList = ConstructIdList(10000);
            Parallel.ForEach(idList, x => list.Add(new Order { Id = x, Name = x.ToString() }));
            var tasks = new Task[1000];
            for (var i = 0; i < 1000; i++)
            {
                var id = i;
                tasks[i] = Task.Factory.StartNew(() => list.Add(new Order { Id = id, Name = id.ToString() }));
            }
            Task.WaitAll(tasks);
            Assert.Equal(11000, list.Count);
        }

        [Fact]
        public void TestConcurrentListAndNormalListWithLock()
        {
			var list = new ConcurrentSortedList<Order>(new OrderComparer());
            var idList = ConstructIdList(10000);

            var sw1 = new Stopwatch();
            sw1.Start();
            Parallel.ForEach(idList, x => list.Add(new Order { Id = x, Name = x.ToString() }));
            sw1.Stop();
            Console.WriteLine(sw1.ElapsedMilliseconds);

            var normal = new SortedList<int, Order>();
            var sw2 = new Stopwatch();
            sw2.Start();
            Parallel.ForEach(idList, x =>
                {
                    lock (locker)
                    {
                        normal.Add(x, new Order { Id = x, Name = x.ToString() });
                    }
                });
            sw2.Stop();
            Console.WriteLine(sw2.ElapsedMilliseconds);

            var normal2 = new SortedList<int, Order>();
            var sw3 = new Stopwatch();
            sw3.Start();
            idList.ForEach(x => normal2.Add(x, new Order { Id = x, Name = x.ToString() }));
            sw3.Stop();
            Console.WriteLine(sw3.ElapsedMilliseconds);
        }

        private List<int> ConstructIdList(int number)
        {
			var random = new Random((int) DateTime.Now.Ticks & 0x0000ffff);
            var idList = new List<int>();
			for (var i = 0; i < number; i++)
			{
                idList.Add(random.Next());
			}

            return idList;
        }

		private void AssertList(ConcurrentSortedList<Order> list, int number)
		{
			Assert.Equal(number, list.Count);
			var orders = new Order[number];
			var cursor = 0;
			foreach (var i in list)
			{
				orders[cursor] = i;
				cursor++;
			}
			for (var i = 1; i < number; i++)
			{
				Assert.True(orders[i - 1].Id <= orders[i].Id);
			}
		}
	}
}
