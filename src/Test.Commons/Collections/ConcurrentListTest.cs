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
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting;
using System.Threading;
using System.Threading.Tasks;
using Commons.Collections.Concurrent;
using Commons.Collections.Set;
using Commons.Utils;
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
            AssertOrderList(list, 10000);
        }

        [Fact]
        public void TestMultithreadAdd()
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
            AssertOrderList(list, 10000);
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
            AssertOrderList(list, 10000);
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
            AssertOrderList(list, 9000);
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
            AssertOrderList(list, 11000);
        }

        [Fact]
        public void TestMultithreadAddAndReomove()
        {
            var list = new ConcurrentSortedList<int>();
            var idList = Enumerable.Range(0, 10000).ToList();
            var newIdList = Enumerable.Range(10000, 2000).ToList();
            Parallel.ForEach(idList, x => list.Add(x));
            var tasks = new Task[3000];
            for (var i = 0; i < 2000; i++)
            {
                var id = newIdList[i];
                tasks[i] = new Task(() => list.Add(id));
            }
            for (var i = 0; i < 1000; i++)
            {
                var id = idList[i];
                tasks[i + 2000] = new Task(() => list.Remove(id));
            }
            Parallel.ForEach(tasks, x => x.Start());
            Task.WaitAll(tasks);
            Assert.Equal(11000, list.Count);
            for (var i = 0; i < 2000; i++)
            {
                Assert.True(list.Contains(newIdList[i]));
            }

            for (var i = 0; i < 1000; i++)
            {
                Assert.False(list.Contains(idList[i]));
            }

            for (var i = 0; i < 9000; i++)
            {
                Assert.True(list.Contains(idList[i + 1000]));
            }
            AssertIntList(list, 11000);
        }

        [Fact]
        public void TestParallelMixedAddAndRemove()
        {
            var list = new ConcurrentSortedList<int>();
            var prepare = Enumerable.Range(0, 10000).Select(x => x * 2).ToList();
            Parallel.ForEach(prepare, x => list.Add(x));
            var tasks = new Task[4000];
            for (var i = 0; i < 2000; i++)
            {
                var id = i;
                tasks[i] = new Task(() => Assert.True(list.Remove(id * 2)));
            }
            for (var i = 0; i < 2000; i++)
            {
                var id = i;
                tasks[i + 2000] = new Task(() => list.Add(id * 2 + 1));
            }
            Parallel.ForEach(tasks, x => x.Start());
            Task.WaitAll(tasks);
            Assert.Equal(10000, list.Count);
            foreach (var i in list)
            {
                if (i < 4000)
                {
                    Assert.Equal(1, i % 2);
                }
                else
                {
                    Assert.Equal(0, i % 2);
                }
            }
            AssertIntList(list, 10000);
        }

        [Fact]
        public void TestMultithreadRemove()
        {
            var list = new ConcurrentSortedList<int>();
            var idList = Enumerable.Range(0, 10000).ToList();
            var removeList = Enumerable.Range(0, 5000).ToList();
            Parallel.ForEach(idList, x => list.Add(x));
            Parallel.ForEach(removeList, x => Assert.True(list.Remove(x)));
            Assert.Equal(5000, list.Count);
            for (var i = 0; i < 5000; i++)
            {
                Assert.False(list.Contains(i));
            }
            for (var i = 0; i < 5000; i++)
            {
                Assert.True(list.Contains(i + 5000));
            }
            AssertIntList(list, 5000);
        }

        [Fact]
        public void TestMultithreadRemoveAndContains()
        {
            var list = new ConcurrentSortedList<int>();
            var idList = Enumerable.Range(0, 10000).ToList();
            Parallel.ForEach(idList, x => list.Add(x));
            Parallel.ForEach(idList, x =>
                {
                    if (x < 5000)
                    {
                        Assert.True(list.Remove(x));
                    }
                    else
                    {
                        Assert.True(list.Contains(x));
                    }
                });
            Assert.Equal(5000, list.Count);

            for (var i = 0; i < 5000; i++)
            {
                Assert.False(list.Contains(idList[i]));
            }
            AssertIntList(list, 5000);
        }

        [Fact]
        public void TestMultithreadAddAndContains()
        {
            var list = new ConcurrentSortedList<int>();
            var idList = Enumerable.Range(0, 5000).ToList();
            Parallel.ForEach(idList, x => list.Add(x));
            var newIdList = Enumerable.Range(5000, 5000).ToList();
            var tasks = new Task[10000];
            for (var i = 0; i < 5000; i++)
            {
                var id = newIdList[i];
                tasks[i] = new Task(() => list.Add(id));
            }
            for (var i = 0; i < 5000; i++)
            {
                var id = i;
                tasks[i + 5000] = new Task(() => Assert.True(list.Contains(id)));
            }
            Parallel.ForEach(tasks, x => x.Start());
            Task.WaitAll(tasks);
            Assert.Equal(10000, list.Count);
            AssertIntList(list, 10000);
        }

        [Fact]
        public void TestMultithreadRemoveAndThenEnumerate()
        {
            var list = new ConcurrentSortedList<int>();
            var idList = Enumerable.Range(0, 10000).ToList();
            var removeList = Enumerable.Range(0, 5000).Select(x => x * 2).ToList();
            Parallel.ForEach(idList, x => list.Add(x));
            Parallel.ForEach(removeList, x => list.Remove(x));
            Assert.Equal(5000, list.Count);
            foreach (var i in list)
            {
                Assert.Equal(1, i % 2);
            }
            AssertIntList(list, 5000);
        }

        [Fact]
        public void TestConcurrentListBasedMapAddSingleThread()
        {
            var map = new ConcurrentListBasedMap<int, int>();
            var idList = Enumerable.Range(0, 10000).ToList();
            foreach (var id in idList)
            {
                Assert.True((map.TryAdd(id, id)));
            }

            Assert.Equal(10000, map.Count);
            foreach (var id in idList)
            {
                Assert.True(map.ContainsKey(id));
            }
        }

        [Fact]
        public void TestConcurrentListBasedMapTryAddFailsSingleThread()
        {
            var map = new ConcurrentListBasedMap<int, int>();
            var idList = Enumerable.Range(0, 10000).ToList();
            foreach (var id in idList)
            {
                Assert.True(map.TryAdd(id, id));
            }

            foreach (var id in idList)
            {
                Assert.False(map.TryAdd(id, id));
            }

            Assert.Equal(10000, map.Count);
        }

        [Fact]
        public void TestConcurrentListBasedMapRemoveSomeSingleThread()
        {
            var map = new ConcurrentListBasedMap<int, int>();
            var idList = Enumerable.Range(0, 10000).ToList();
            foreach (var id in idList)
            {
                Assert.True(map.TryAdd(id, id));
            }

            for (var i = 0; i < 5000; i++)
            {
                Assert.True(map.TryRemove(idList[i + 2000]));
            }
            Assert.Equal(5000, map.Count);
            for (var i = 0; i < 2000; i++)
            {
                Assert.True(map.ContainsKey(idList[i]));
            }
            for (var i = 0; i < 5000; i++)
            {
                Assert.False(map.ContainsKey(idList[i + 2000]));
            }
        }

        [Fact]
        public void TestConcurrentListBasedMapRemoveAllSingleThread()
        {
            var map = new ConcurrentListBasedMap<int, int>();
            var idList = Enumerable.Range(0, 10000).ToList();
            foreach (var id in idList)
            {
                Assert.True(map.TryAdd(id, id));
            }

            foreach (var id in idList)
            {
                Assert.True(map.TryRemove(id));
            }
            Assert.Equal(0, map.Count);
            foreach (var id in idList)
            {
                Assert.False(map.ContainsKey(id));
            }
        }

        [Fact]
        public void TestConcurrentListBasedMapRemoveEmpty()
        {
            var map = new ConcurrentListBasedMap<int, int>();
            Assert.False(map.TryRemove(1));
            Assert.False(map.TryRemove(2));
            Assert.False(map.TryRemove(200));
            Assert.False(map.TryRemove(3000));
        }

        [Fact]
        public void TestConcurrentListBasedMapAddMultithread()
        {
            var map = new ConcurrentListBasedMap<int, int>();
            var idList = Enumerable.Range(0, 10000).ToList();
            Parallel.ForEach(idList, x => Assert.True(map.TryAdd(x, x)));
            Assert.Equal(0, map.Min.Key);
            Assert.Equal(9999, map.Max.Key);
            Assert.Equal(10000, map.Count);
        }

        [Fact]
        public void TestConcurrentListBasedMapAddMultithreadOrder()
        {
            var map = new ConcurrentListBasedMap<int, int>();
            var rand = new Random((int) (0x0000ffff & DateTime.Now.Ticks));
            var idList = new List<int>();
	        var set = new HashedSet<int>();
	        var n = 0;
			while (n < 10000)
			{
				var v = rand.Next();
				if (!set.Contains(v))
				{
					idList.Add(v);
					set.Add(v);
					n++;
				}
			}
            Parallel.ForEach(idList, x => Assert.True((map.TryAdd(x, x))));
            AssertListBasedMapOrder(map);
        }

        [Fact]
        public void TestConcurrentListBasedMapAddAndRemoveMultithread()
        {
            var map = new ConcurrentListBasedMap<int, int>();
            var idList = new List<int>();
            var newList = new List<int>();
            var rand = new Random((int) (0x0000ffff & DateTime.Now.Ticks));
	        var set = new HashedSet<int>();
	        var items = 0;
			while (items < 10000)
            {
	            var n = rand.Next();
				if (!set.Contains(n))
	            {
					idList.Add(rand.Next());
		            set.Add(n);
		            items++;
	            }
            }
			set.Clear();
	        items = 0;
			while (items < 5000)
            {
	            var n = rand.Next();
	            if (!set.Contains(n))
	            {
					newList.Add(n);
					set.Add(n);
		            items++;
	            }
            }
            Parallel.ForEach(idList, x => Assert.True(map.TryAdd(x, x)));
            var remove = new Task(() =>
            {
                for (var i = 0; i < 5000; i++)
                {
                    Assert.True(map.TryRemove(idList[i]));
                }
            });
            var add = new Task(() =>
            {
                foreach (var element in newList)
                {
                    Assert.True(map.TryAdd(element, element));
                }
            });
            add.Start();
            remove.Start();
            Task.WaitAll(add, remove);
            Assert.Equal(10000, map.Count);
            AssertListBasedMapOrder(map);
        }

        [Fact]
        public void TestConcurrentListBasedMapTryGetValue()
        {
            var map = new ConcurrentListBasedMap<int, int>();
            var idList = new List<int>();
            var rand = new Random((int)(0x0000ffff & DateTime.Now.Ticks));
            for (var i = 0; i < 10000; i++)
            {
                idList.Add(rand.Next());
            }

            var newList = idList.ToList();

            Parallel.ForEach(idList, x => map.TryAdd(x, x));

            var remove = new Task(() =>
            { 
                for (var i = 0; i < 5000; i++)
                {
                    Assert.True(map.TryRemove(idList[i]));
                }
            });

            var tryGet = new Task(() =>
            {
                for (var i = 0; i < 5000; i++)
                {
                    var v = 0;
                    Assert.True(map.TryGetValue(newList[i + 5000], out v));
                    Assert.Equal(v, newList[i + 5000]);
                }
            });

            tryGet.Start();
            remove.Start();
            Task.WaitAll(remove, tryGet);
            Assert.Equal(5000, map.Count);

            for (var i = 0; i < 5000; i++)
            {
                Assert.False(map.ContainsKey(idList[i]));
            }

            for (var i = 0; i < 5000; i++)
            {
                var v = 0;
                Assert.False(map.TryGetValue(idList[i], out v));
            }
        }

        [Fact]
        public void TestConcurrentListBasedMapIndexer()
        {
            var map = new ConcurrentListBasedMap<int, int>();
            var idList = new List<int>();
            var newList = new List<int>();
            var rand = new Random((int)(0x0000ffff & DateTime.Now.Ticks));
            for (var i = 0; i < 5000; i++)
            {
                idList.Add(rand.Next());
                newList.Add(rand.Next());
            }

            var add = new Task(() =>
            {
                foreach (var id in idList)
                {
                    Assert.True(map.TryAdd(id, id));
                }
            });

            var set = new Task(() =>
            {
                foreach (var id in newList)
                {
                    map[id] = id;
                }
            });

            add.Start();
            set.Start();
            Task.WaitAll(add, set);
            foreach(var id in idList)
            {
                Assert.Equal(id, map[id]);
            }

            foreach(var id in newList)
            {
                Assert.Equal(id, map[id]);
            }
        }

        [Fact]
        public void TestConcurrentListBasedMapIndexerGet()
        {
            var map = new ConcurrentListBasedMap<int, int>();
            var idList = new List<int>();
            var newList = new List<int>();
            var rand = new Random((int)(0x0000ffff & DateTime.Now.Ticks));
            for (var i = 0; i < 5000; i++)
            {
                idList.Add(rand.Next());
                newList.Add(rand.Next());
            }
            Parallel.ForEach(idList, x => map.TryAdd(x, x));
            Parallel.ForEach(idList, x => Assert.Equal(x, map[x]));
            Parallel.ForEach(newList, x => Assert.Throws(typeof(KeyNotFoundException), () => map[x]));
        }

        [Fact]
        public void TestConcurrentListBasedMapUpdate()
        {
            var map = new ConcurrentListBasedMap<int, int>();
            var idList = Enumerable.Range(0, 5000).ToList();
            Parallel.ForEach(idList, x => map.TryAdd(x, x));
            Parallel.ForEach(idList, x => 
            {
                if (x % 2 == 0)
                {
                    map[x] = x + 1;
                }
                else
                {
                    Assert.True(map.TryRemove(x));
                }
            });
            Assert.Equal(2500, map.Count);
            foreach (var element in map)
            {
                Assert.Equal(element.Key + 1, element.Value);
                Assert.True(element.Key % 2 == 0);
            }
            for (var i = 0; i < 2500; i++)
            {
                Assert.False(map.ContainsKey(i * 2 + 1));
            }
        }

        [Fact]
        public void TestConcurrentListBasedMapRemoveMultithread()
        {
            var map = new ConcurrentListBasedMap<int, int>();
            var idList = new List<int>();
            var rand = new Random((int)(0x0000ffff & DateTime.Now.Ticks));
            for (var i = 0; i < 5000; i++)
            {
                idList.Add(rand.Next());
            }
            Parallel.ForEach(idList, x => map.TryAdd(x, x));
            Parallel.ForEach(idList, x => Assert.True(map.TryRemove(x)));
            Assert.Equal(0, map.Count);
            Parallel.ForEach(idList, x => Assert.False(map.TryRemove(x)));
        }

        [Fact]
        public void TestConcurrentListBasedMapClear()
        {
            var map = new ConcurrentListBasedMap<int, int>();
            var idList = new List<int>();
            var rand = new Random((int)(0x0000ffff & DateTime.Now.Ticks));
            for (var i = 0; i < 5000; i++)
            {
                idList.Add(rand.Next());
            }
			// TODO: problem
            Parallel.ForEach(idList, x => map.TryAdd(x, x));
            Assert.Equal(5000, map.Count);
            map.Clear();
            Assert.Equal(0, map.Count);
        }

        [Fact]
        public void TestConcurrentListBasedMapEmptyMin()
        {
            var map = new ConcurrentListBasedMap<int, int>();
            Assert.Throws(typeof(InvalidOperationException), () => map.Min);
        }

        [Fact]
        public void TestConcurrentListBasedMapEmptyMax()
        {
            var map = new ConcurrentListBasedMap<int, int>();
            Assert.Throws(typeof(InvalidOperationException), () => map.Max);
        }

        [Fact]
        public void TestConcurrentListBasedMapOneItemMinMax()
        {
            var map = new ConcurrentListBasedMap<int, int>();
            map.TryAdd(30, 30);
            Assert.Equal(30, map.Min.Key);
            Assert.Equal(30, map.Min.Value);
            Assert.Equal(30, map.Max.Key);
            Assert.Equal(30, map.Max.Value);
        }

        [Fact]
        public void TestConcurrentListBasedMapMinMaxMultithread()
        {
            var map = new ConcurrentListBasedMap<int, int>();
            map.TryAdd(-1, -1);
            map.TryAdd(10000, 10000);
            var idList = Enumerable.Range(0, 5000).ToList();
            var add = new Task(() =>
            {
                foreach (var element in idList)
                {
                    map.TryAdd(element, element);
                }
            });
            var getMin = new Task(() =>
            {
                for (var i = 0; i < 1000; i++)
                {
                    Assert.Equal(-1, map.Min.Key);
                    Thread.Sleep(0);
                }
            });
            var getMax = new Task(() =>
            {
                for (var i = 0; i < 1000; i++)
                {
                    Assert.Equal(10000, map.Max.Key);
                    Thread.Sleep(0);
                }

            });
            add.Start();
            getMin.Start();
            getMax.Start();
            Task.WaitAll(add, getMin, getMax);
            Assert.Equal(5002, map.Count);
        }

        private void AssertListBasedMapOrder(ConcurrentListBasedMap<int, int> map)
        {
            var index = 0;
            var previous = 0;
            foreach (var element in map)
            {
                if (index != 0)
                {
                    Assert.True(element.Key > previous);
                }
                previous = element.Key;
                index++;
            }
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

        private void AssertOrderList(ConcurrentSortedList<Order> list, int number)
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

        private void AssertIntList(ConcurrentSortedList<int> list, int number)
        {
            Assert.Equal(number, list.Count);
            var items = new int[number];
            var cursor = 0;
            foreach (var i in list)
            {
                items[cursor] = i;
                cursor++;
            }
            for (var i = 1; i < number; i++)
            {
                Assert.True(items[i - 1] <= items[i]);
            }
        }
    }
}
