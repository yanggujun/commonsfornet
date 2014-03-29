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
using System.Threading.Tasks;

using Commons.Collections;

using NUnit;
using NUnit.Framework;
using System.IO;

namespace Test.Commons.Collections
{
    [TestFixture]
    public class CollectionTest
    {
        [SetUp]
        public void Setup()
        {

        }
        [TearDown]
        public void TearDown()
        {

        }

        [Test]
        public void TestBoundedQueueNoAbsorb()
        {
            BoundedQueue<int> queue = new BoundedQueue<int>(10);
            for (int i = 0; i < 10; i++)
            {
                Assert.IsFalse(queue.IsFull);
                queue.Enqueue(i);
            }
            Assert.AreEqual(queue.Count, 10);
            Assert.That(queue.Contains(5));
            Assert.IsTrue(queue.IsFull);
            Assert.AreEqual(queue.Peek(), 0);
            Assert.AreEqual(queue.Dequeue(), 0);
            Assert.AreEqual(queue.Count, 9);
            Assert.IsFalse(queue.IsFull);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestBoundedQueueNoAbsorbExceedLimit()
        {
            BoundedQueue<int> queue = new BoundedQueue<int>(10);
            for (int i = 0; i < 10; i++)
            {
                queue.Enqueue(i);
            }

            queue.Enqueue(11);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestBoundedQueueInvalidMaxSize()
        {
            BoundedQueue<int> queue = new BoundedQueue<int>(-10);
        }

        [Test]
        public void TestBoundedQueueConstructedWithEnumrable()
        {
            BoundedQueue<int> queue = new BoundedQueue<int>(Enumerable.Range(0, 10), 5);
            Assert.That(queue.Count == 5);
            int[] array = new int[10];
            queue.CopyTo(array, 1);
            Assert.That(array[0] == 0);
            for (int i = 0; i < 5; i++)
            {
                Assert.That(array[i+1] == i);
            }
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestBoundedQueueCopyToNullArray()
        {
            BoundedQueue<int> queue = new BoundedQueue<int>(Enumerable.Range(0, 10), 5);
            queue.CopyTo(null, 0);
        }
        
        [Test]
        public void TestBoundedQueueAbsorb()
        {
            BoundedQueue<int> queue = new BoundedQueue<int>(Enumerable.Range(0, 10), 6, true);
            Assert.That(queue.Count == 6);
            Assert.That(queue.Peek() == 0);
            queue.Enqueue(7);
            Assert.That(queue.Count == 6);
            Assert.That(queue.Peek() == 1);
        }

        [Test]
        public void TestBoundedQueueAbsorbWithMaxsizeOne()
        {
            BoundedQueue<int> queue = new BoundedQueue<int>(1, true);
            queue.Enqueue(0);
            queue.Enqueue(10);
            Assert.That(queue.Count == 1);
            Assert.That(queue.Dequeue() == 10);
            Assert.That(queue.Count == 0);
        }

        [Test]
        public void TestCompositeCollectionEmptyConstructor()
        {
            CompositeCollection<int> comp = new CompositeCollection<int>();
            comp.Add(1);
            comp.Add(2);
            Assert.AreEqual(comp.Count, 2);
            comp.AddAll(Enumerable.Range(3, 5).ToList());
            Assert.AreEqual(comp.Count, 7);
        }

        [Test]
        public void TestCompositeCollectionMultiparamConstructor()
        {
            List<int> list1 = Enumerable.Range(0, 10).ToList();
            List<int> list2 = Enumerable.Range(8, 10).ToList();

            CompositeCollection<int> comp = new CompositeCollection<int>(list1, list2);
            Assert.AreEqual(comp.Count, 20);
            comp.Add(30);
            Assert.AreEqual(comp.Count, 21);
            Assert.IsTrue(comp.Remove(9));
            Assert.IsFalse(comp.Contains(9));
            Assert.IsTrue(comp.Contains(8));
            comp.Clear();
            Assert.AreEqual(comp.Count, 0);
        }

        [Test]
        public void TestCompositeCollectionUniqueList()
        {
            List<int> list1 = Enumerable.Range(0, 10).ToList();
            List<int> list2 = Enumerable.Range(8, 10).ToList();
            CompositeCollection<int> comp = new CompositeCollection<int>(list1, list2);

            IList<int> result = comp.ToUnique();
            Assert.AreEqual(result.Count, 18);
            for (int i = 0; i < 18; i++)
            {
                Assert.AreEqual(result[i], i);
            }
        }

        [Test]
        public void TestCompositeCollectionEnumrator()
        {
            List<int> list1 = Enumerable.Range(0, 10).ToList();
            List<int> list2 = Enumerable.Range(10, 10).ToList();
            CompositeCollection<int> comp = new CompositeCollection<int>(list1, list2);
            int i = 0;
            foreach (var item in comp)
            {
                Assert.AreEqual(item, i);
                i++;
            }
            Assert.AreEqual(i, 20);
            int[] array = new int[25];
            comp.CopyTo(array, 1);
            Assert.AreEqual(0, array[0]);
            for (int j = 1; j < 21; j++)
            {
                Assert.AreEqual(j - 1, array[j]);
            }
        }

        [Test]
        public void TestCompositeCollectionFuncComparer()
        {
            List<Order> orders1 = new List<Order>() { new Order() { Id = 1, Name = "Name1" }, new Order { Id = 2, Name = "Name2" }, new Order { Id = 3, Name = "Name3" } };
            List<Order> orders2 = new List<Order>() { new Order() { Id = 3, Name = "Name3" }, new Order { Id = 4, Name = "Name4" }, new Order { Id = 5, Name = "Name5" } };
            List<Order> orders3 = new List<Order>() { new Order() { Id = 6, Name = "Name6" }, new Order { Id = 7, Name = "Name7" }, new Order { Id = 8, Name = "Name8" } };
            CompositeCollection<Order> orders = new CompositeCollection<Order>(orders1, orders2, orders3);
            Order o1 = new Order() { Id = 1, Name = "whatever" };
            Assert.IsTrue(orders.Contains(o1, (i1, i2) => i1.Id == i2.Id));
            Order o2 = new Order() { Id = 1111, Name = "whatever" };
            Assert.IsFalse(orders.Contains(o2, (i1, i2) => i1.Id == i2.Id));
            var list = orders.ToUnique((i1, i2) => i1.Id == i2.Id);
            Assert.AreEqual(list.Count, 8);

            Assert.IsTrue(orders.Remove(o1, (i1, i2) => i1.Id == i2.Id));
            Assert.IsFalse(orders.Remove(o2, (i1, i2) => i1.Id == i2.Id));
        }

        [Test]
        public void TestCompositeCollectionEqualityComparer()
        {
            List<Order> orders1 = new List<Order>() { new Order() { Id = 1, Name = "Name1" }, new Order { Id = 2, Name = "Name2" }, new Order { Id = 3, Name = "Name3" } };
            List<Order> orders2 = new List<Order>() { new Order() { Id = 3, Name = "Name3" }, new Order { Id = 4, Name = "Name4" }, new Order { Id = 5, Name = "Name5" } };
            List<Order> orders3 = new List<Order>() { new Order() { Id = 6, Name = "Name6" }, new Order { Id = 7, Name = "Name7" }, new Order { Id = 8, Name = "Name8" } };
            CompositeCollection<Order> orders = new CompositeCollection<Order>(orders1, orders2, orders3);
            Order o1 = new Order() { Id = 1, Name = "whatever" };
            Assert.IsTrue(orders.Contains(o1, new OrderEqualityComparer()));
            Order o2 = new Order() { Id = 1111, Name = "whatever" };
            Assert.IsFalse(orders.Contains(o2, new OrderEqualityComparer()));
            var list = orders.ToUnique(new OrderEqualityComparer());
            Assert.AreEqual(list.Count, 8);

            Assert.IsTrue(orders.Remove(o1, new OrderEqualityComparer()));
            Assert.IsFalse(orders.Remove(o2, new OrderEqualityComparer()));
        }

        [Test]
        public void TestPredicatedCollection()
        {
            PredicatedCollection<int> pc = new PredicatedCollection<int>(i => i > 0);
            pc.Add(1);
            pc.Add(2);
            Assert.AreEqual(2, pc.Count);
            List<int> list = new List<int>() { 3, 4, 5 };
            pc.AddAll(list);
            Assert.AreEqual(5, pc.Count);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestPredicatedCollectionValidateFail()
        {
            PredicatedCollection<int> pc = new PredicatedCollection<int>(i => i > 0);
            pc.Add(1);
            pc.Add(2);
            pc.Add(-1);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestPredicatedCollectionAddAllValidateFail()
        {
            PredicatedCollection<int> pc = new PredicatedCollection<int>(i => i > 0);
            pc.Add(1);
            pc.Add(2);
            List<int> list = new List<int>() { 3, -1, 5 };
            pc.AddAll(list);
        }

        [Test]
        public void TestTreeSetSimpleOperations()
        {
            ITreeSet<int> set = new LlrbTreeSet<int>();
            var count = 0;
            foreach (var item in set)
            {
                count++;
            }
            Assert.IsTrue(count == 0);
            set.Add(10);
            set.Add(20);
            set.Add(30);
            set.Add(5);
            set.Add(1);
            Assert.IsTrue(set.Contains(20));
            Assert.IsFalse(set.Contains(100));
            Assert.AreEqual(5, set.Count);
            Assert.AreEqual(30, set.Max);
            Assert.AreEqual(1, set.Min);

            var list = new List<int>();

            foreach (var item in set)
            {
                list.Add(item);
            }

            Assert.IsTrue(list.Count == set.Count);

            foreach (var item in list)
            {
                Assert.IsTrue(set.Contains(item));
            }

            var array = new int[5];
            set.CopyTo(array, 0);
            foreach (var item in array)
            {
                Assert.IsTrue(set.Contains(item));
            }

            Assert.IsTrue(set.Remove(5));
            Assert.AreEqual(4, set.Count);
            Assert.IsFalse(set.Contains(5));

            set.RemoveMin();
            Assert.AreEqual(3, set.Count);
            Assert.IsFalse(set.Contains(1));

            set.Clear();
            Assert.AreEqual(0, set.Count);
        }

        [Test]
        public void TestTreeSetRandomOperations()
        {
            for (int j = 0; j < 10; j++)
            {
                ITreeSet<int> set = new LlrbTreeSet<int>();

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
                    Assert.IsTrue(list.Contains(item));
                    count++;
                }
                Assert.IsTrue(count == set.Count);

                Assert.AreEqual(testNumber, set.Count);

                Random randomIndex = new Random((int)(DateTime.Now.Ticks & 0x0000FFFF));
                for (int i = 0; i < 100; i++)
                {
                    int index = randomIndex.Next();
                    index = index < 0 ? (-index) : index;
                    index %= testNumber;
                    int testValue = list[index];
                    Assert.IsTrue(set.Contains(testValue));
                }

                for (int i = 0; i < 100; i++)
                {
                    int min = list.Min();
                    Assert.AreEqual(min, set.Min);
                    set.RemoveMin();
                    Assert.AreEqual(testNumber - i - 1, set.Count);
                    Assert.IsFalse(set.Contains(min));
                    list.Remove(min);
                }

                testNumber -= 100;
                for (int i = 0; i < 100; i++)
                {
                    int max = list.Max();
                    Assert.AreEqual(max, set.Max);
                    set.RemoveMax();
                    Assert.AreEqual(testNumber - i - 1, set.Count);
                    Assert.IsFalse(set.Contains(max));
                    list.Remove(max);
                }

                testNumber -= 100;
                for (int i = 0; i < 100; i++)
                {
                    int index = randomIndex.Next();
                    index = index < 0 ? (-index) : index;
                    index %= testNumber - i;
                    int toRemove = list[index];
                    Assert.IsTrue(set.Contains(toRemove));
                    Assert.IsTrue(set.Remove(toRemove));
                    Assert.IsFalse(set.Contains(toRemove));
                    Assert.AreEqual(testNumber - i - 1, set.Count);
                    list.Remove(toRemove);
                }
            }
        }
    }
}
