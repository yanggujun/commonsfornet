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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

using Xunit;

using Commons.Collections;
using Commons.Collections.Set;
using Commons.Collections.Queue;
using Commons.Collections.Collection;
using Commons.Collections.Json;

namespace Test.Commons.Collections
{
    public class CollectionTest
    {
        [Fact]
        public void TestBoundedQueueNoAbsorb()
        {
            BoundedQueue<int> queue = new BoundedQueue<int>(10);
            for (int i = 0; i < 10; i++)
            {
                Assert.False(queue.IsFull);
                queue.Enqueue(i);
            }
            Assert.Equal(queue.Count, 10);
            Assert.True(queue.Contains(5));
            Assert.True(queue.IsFull);
            Assert.Equal(queue.Peek(), 0);
            Assert.Equal(queue.Dequeue(), 0);
            Assert.Equal(queue.Count, 9);
            Assert.False(queue.IsFull);
        }

        [Fact]
        public void TestBoundedQueueNoAbsorbExceedLimit()
        {
            Assert.Throws<InvalidOperationException>(
                () =>
                {
                    BoundedQueue<int> queue = new BoundedQueue<int>(10);
                    for (int i = 0; i < 10; i++)
                    {
                        queue.Enqueue(i);
                    }

                    queue.Enqueue(11);
                });
        }

        [Fact]
        public void TestBoundedQueueInvalidMaxSize()
        {
            Assert.Throws<ArgumentException>(
                () =>
                {
                    BoundedQueue<int> queue = new BoundedQueue<int>(-10);
                });
        }

        [Fact]
        public void TestBoundedQueueConstructedWithEnumrable()
        {
            BoundedQueue<int> queue = new BoundedQueue<int>(Enumerable.Range(0, 10), 5);
            Assert.True(queue.Count == 5);
            int[] array = new int[10];
            queue.CopyTo(array, 1);
            Assert.True(array[0] == 0);
            for (int i = 0; i < 5; i++)
            {
                Assert.True(array[i+1] == i);
            }
        }

        [Fact]
        public void TestBoundedQueueCopyToNullArray()
        {
            Assert.Throws<ArgumentNullException>(
                () =>
                {
                    BoundedQueue<int> queue = new BoundedQueue<int>(Enumerable.Range(0, 10), 5);
                    queue.CopyTo(null, 0);
                });
        }
        
        [Fact]
        public void TestBoundedQueueAbsorb()
        {
            BoundedQueue<int> queue = new BoundedQueue<int>(Enumerable.Range(0, 10), 6, true);
            Assert.True(queue.Count == 6);
            Assert.True(queue.Peek() == 0);
            queue.Enqueue(7);
            Assert.True(queue.Count == 6);
            Assert.True(queue.Peek() == 1);
        }

        [Fact]
        public void TestBoundedQueueAbsorbWithMaxsizeOne()
        {
            BoundedQueue<int> queue = new BoundedQueue<int>(1, true);
            queue.Enqueue(0);
            queue.Enqueue(10);
            Assert.True(queue.Count == 1);
            Assert.True(queue.Dequeue() == 10);
            Assert.True(queue.Count == 0);
        }

        [Fact]
        public void TestCompositeCollectionEmptyConstructor()
        {
            CompositeCollection<int> comp = new CompositeCollection<int>();
            comp.Add(1);
            comp.Add(2);
            Assert.Equal(comp.Count, 2);
            comp.AddAll(Enumerable.Range(3, 5).ToList());
            Assert.Equal(comp.Count, 7);
        }

        [Fact]
        public void TestCompositeCollectionMultiparamConstructor()
        {
            List<int> list1 = Enumerable.Range(0, 10).ToList();
            List<int> list2 = Enumerable.Range(8, 10).ToList();

            CompositeCollection<int> comp = new CompositeCollection<int>(list1, list2);
            Assert.Equal(comp.Count, 20);
            comp.Add(30);
            Assert.Equal(comp.Count, 21);
            Assert.True(comp.Remove(9));
            Assert.False(comp.Contains(9));
            Assert.True(comp.Contains(8));
            comp.Clear();
            Assert.Equal(comp.Count, 0);
        }

        [Fact]
        public void TestCompositeCollectionUniqueList()
        {
            List<int> list1 = Enumerable.Range(0, 10).ToList();
            List<int> list2 = Enumerable.Range(8, 10).ToList();
            CompositeCollection<int> comp = new CompositeCollection<int>(list1, list2);

            IList<int> result = comp.ToUnique();
            Assert.Equal(result.Count, 18);
            for (int i = 0; i < 18; i++)
            {
                Assert.Equal(result[i], i);
            }
        }

        [Fact]
        public void TestCompositeCollectionEnumrator()
        {
            List<int> list1 = Enumerable.Range(0, 10).ToList();
            List<int> list2 = Enumerable.Range(10, 10).ToList();
            CompositeCollection<int> comp = new CompositeCollection<int>(list1, list2);
            int i = 0;
            foreach (var item in comp)
            {
                Assert.Equal(item, i);
                i++;
            }
            Assert.Equal(i, 20);
            int[] array = new int[25];
            comp.CopyTo(array, 1);
            Assert.Equal(0, array[0]);
            for (int j = 1; j < 21; j++)
            {
                Assert.Equal(j - 1, array[j]);
            }
        }

        [Fact]
        public void TestCompositeCollectionFuncComparer()
        {
            List<Order> orders1 = new List<Order>() { new Order() { Id = 1, Name = "Name1" }, new Order { Id = 2, Name = "Name2" }, new Order { Id = 3, Name = "Name3" } };
            List<Order> orders2 = new List<Order>() { new Order() { Id = 3, Name = "Name3" }, new Order { Id = 4, Name = "Name4" }, new Order { Id = 5, Name = "Name5" } };
            List<Order> orders3 = new List<Order>() { new Order() { Id = 6, Name = "Name6" }, new Order { Id = 7, Name = "Name7" }, new Order { Id = 8, Name = "Name8" } };
            CompositeCollection<Order> orders = new CompositeCollection<Order>(orders1, orders2, orders3);
            Order o1 = new Order() { Id = 1, Name = "whatever" };
            Assert.True(orders.Contains(o1, (i1, i2) => i1.Id == i2.Id));
            Order o2 = new Order() { Id = 1111, Name = "whatever" };
            Assert.False(orders.Contains(o2, (i1, i2) => i1.Id == i2.Id));
            var list = orders.ToUnique((i1, i2) => i1.Id == i2.Id);
            Assert.Equal(list.Count, 8);

            Assert.True(orders.Remove(o1, (i1, i2) => i1.Id == i2.Id));
            Assert.False(orders.Remove(o2, (i1, i2) => i1.Id == i2.Id));
        }

        [Fact]
        public void TestCompositeCollectionEqualityComparer()
        {
            List<Order> orders1 = new List<Order>() { new Order() { Id = 1, Name = "Name1" }, new Order { Id = 2, Name = "Name2" }, new Order { Id = 3, Name = "Name3" } };
            List<Order> orders2 = new List<Order>() { new Order() { Id = 3, Name = "Name3" }, new Order { Id = 4, Name = "Name4" }, new Order { Id = 5, Name = "Name5" } };
            List<Order> orders3 = new List<Order>() { new Order() { Id = 6, Name = "Name6" }, new Order { Id = 7, Name = "Name7" }, new Order { Id = 8, Name = "Name8" } };
            CompositeCollection<Order> orders = new CompositeCollection<Order>(orders1, orders2, orders3);
            Order o1 = new Order() { Id = 1, Name = "whatever" };
            Assert.True(orders.Contains(o1, new OrderEqualityComparer()));
            Order o2 = new Order() { Id = 1111, Name = "whatever" };
            Assert.False(orders.Contains(o2, new OrderEqualityComparer()));
            var list = orders.ToUnique(new OrderEqualityComparer());
            Assert.Equal(list.Count, 8);

            Assert.True(orders.Remove(o1, new OrderEqualityComparer()));
            Assert.False(orders.Remove(o2, new OrderEqualityComparer()));
        }

        [Fact]
        public void TestTreeSetSimpleOperations()
        {
            ITreeSet<int> set = new TreeSet<int>();
            var count = 0;
            foreach (var item in set)
            {
                count++;
            }
            Assert.True(count == 0);
            Assert.True(set.Count == 0);
            set.Add(10);
            set.Add(20);
            set.Add(30);
            set.Add(5);
            set.Add(1);
            Assert.True(set.Contains(20));
            Assert.False(set.Contains(100));
            Assert.Equal(5, set.Count);
            Assert.Equal(30, set.Max);
            Assert.Equal(1, set.Min);

            var list = new List<int>();

            foreach (var item in set)
            {
                list.Add(item);
            }

            Assert.True(list.Count == set.Count);

            foreach (var item in list)
            {
                Assert.True(set.Contains(item));
            }

            var array = new int[5];
            set.CopyTo(array, 0);
            foreach (var item in array)
            {
                Assert.True(set.Contains(item));
            }

            Assert.True(set.Remove(5));
            Assert.Equal(4, set.Count);
            Assert.False(set.Contains(5));

            set.RemoveMin();
            Assert.Equal(3, set.Count);
            Assert.False(set.Contains(1));

            set.Clear();
            Assert.Equal(0, set.Count);
        }

        [Fact]
        public void TestTreeSetRandomOperations()
        {
            for (int j = 0; j < 10; j++)
            {
                ITreeSet<int> set = new TreeSet<int>();

                Random randomValue = new Random((int)(DateTime.Now.Ticks & 0x0000FFFF));
                List<int> list = new List<int>();
                int testNumber = 1000;
                while (set.Count < testNumber)
                {
                    int v = randomValue.Next();
                    if (!set.Contains(v))
                    {
                        set.Add(v);
                        list.Add(v);
                    }
                }

                int count = 0;
                foreach (var item in set)
                {
                    Assert.True(list.Contains(item));
                    count++;
                }
                Assert.True(count == set.Count);

                Assert.Equal(testNumber, set.Count);

                Random randomIndex = new Random((int)(DateTime.Now.Ticks & 0x0000FFFF));
                for (int i = 0; i < 100; i++)
                {
                    int index = randomIndex.Next();
                    index = index < 0 ? (-index) : index;
                    index %= testNumber;
                    int testValue = list[index];
                    Assert.True(set.Contains(testValue));
                }

                for (int i = 0; i < 100; i++)
                {
                    int min = list.Min();
                    Assert.Equal(min, set.Min);
                    set.RemoveMin();
                    Assert.Equal(testNumber - i - 1, set.Count);
                    Assert.False(set.Contains(min));
                    list.Remove(min);
                }

                testNumber -= 100;
                for (int i = 0; i < 100; i++)
                {
                    int max = list.Max();
                    Assert.Equal(max, set.Max);
                    set.RemoveMax();
                    Assert.Equal(testNumber - i - 1, set.Count);
                    Assert.False(set.Contains(max));
                    list.Remove(max);
                }

                testNumber -= 100;
                for (int i = 0; i < 100; i++)
                {
                    int index = randomIndex.Next();
                    index = index < 0 ? (-index) : index;
                    index %= testNumber - i;
                    int toRemove = list[index];
                    Assert.True(set.Contains(toRemove));
                    Assert.True(set.Remove(toRemove));
                    Assert.False(set.Contains(toRemove));
                    Assert.Equal(testNumber - i - 1, set.Count);
                    list.Remove(toRemove);
                }
            }
        }

        [Fact]
        public void TestJsonObject()
        {
            dynamic json = new JsonObject();
            json.Cities = new string[] { "Shanghai", "Beijing", "Guangzhou" };
			json.Teams = new List<string>() { "Brazil", "Argentina", "France", "Italy" };
			json.Child.Name = "Alan";
			json.Child.Age = 5;
            json.Fruites = new JsonObject();
            json.Fruites.Apple = 5;
            json.Fruites.Orange = 9;
            json.Fruites.Banana = 7;
            Console.WriteLine(json.ToString());
        }
    }
}
