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
using System.Linq;
using Xunit;
using Commons.Collections.Set;
using Commons.Collections.Queue;
using Commons.Collections.Collection;
using Commons.Collections.Bag;
using Commons.Collections.Map;
using System.Collections;

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
            ISortedSet<int> set = new TreeSet<int>();
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
                ISortedSet<int> set = new TreeSet<int>();

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
        public void TestDequeConstructor()
        {
            var deque = new Deque<string>();
            TestDeque(deque);
            var deque2 = new Deque<string>();
            TestDeque(deque2);
        }

        [Fact]
        public void TestLinkedDequeConstructor()
        {
            var deque = new LinkedDeque<string>();
            TestDeque(deque);
        }

        [Fact]
        public void TestLinkedDequeOneItem()
        {
            var deque = new LinkedDeque<int>();

            deque.Append(1);
            Assert.False(deque.IsEmpty);
            foreach (var item in deque)
            {
                Assert.Equal(1, item);
            }
            Assert.Equal(1, deque.First);
            Assert.Equal(1, deque.Last);
            Assert.Equal(1, deque.Pop());
            foreach (var item in deque)
            {
                Assert.True(false);
            }
            Assert.True(deque.IsEmpty);
            Assert.Equal(0, deque.Count);

            deque.Prepend(1);
            Assert.False(deque.IsEmpty);
            foreach (var item in deque)
            {
                Assert.Equal(1, item);
            }
            Assert.Equal(1, deque.First);
            Assert.Equal(1, deque.Last);
            Assert.Equal(1, deque.Shift());
            foreach (var item in deque)
            {
                Assert.True(false);
            }
            Assert.True(deque.IsEmpty);
            Assert.Equal(0, deque.Count);
        }

        [Fact]
        public void TestLinkedDequeReduceToZero()
        {
            var deque = new LinkedDeque<int>();
            for (var i = 0; i < 100; i++)
            {
                deque.Append(i);
            }
            for (var i = 100; i < 200; i++)
            {
                deque.Prepend(i);
            }
            Assert.False(deque.IsEmpty);
            Assert.Equal(200, deque.Count);

            for (var i = 0; i < 100; i++)
            {
                Assert.Equal(99 - i, deque.Pop());
            }

            for (var i = 0; i < 100; i++)
            {
                Assert.Equal(199 - i, deque.Shift());
            }
            Assert.True(deque.IsEmpty);
            Assert.Equal(0, deque.Count);
        }

        [Fact]
        public void TestDequeOperations()
        {
            var deque = new Deque<string>();
            DequeOperations(deque);
        }

        [Fact]
        public void TestLinkedDequeOperations()
        {
            var deque = new LinkedDeque<string>();
            DequeOperations(deque);
        }

        [Fact]
        public void TestDequeAppendAndShift()
        {
            var deque = new Deque<int>();
            DequeAppendAndShift(deque);
        }

        [Fact]
        public void TestLinkedDequeAppendAndShift()
        {
            var deque = new LinkedDeque<int>();
            DequeAppendAndShift(deque);
        }

        [Fact]
        public void TestDequeAppendAndPop()
        {
            var deque = new Deque<int>();
            DequeAppendAndPop(deque);
        }

        [Fact]
        public void TestLinkedDequeAppendAndPop()
        {
            var deque = new LinkedDeque<int>();
            DequeAppendAndPop(deque);
        }

        [Fact]
        public void TestDequePrependAndShift()
        {
            var deque = new Deque<int>();
            DequePrependAndShift(deque);
        }

        [Fact]
        public void TestLinkedDequePrependAndShift()
        {
            var deque = new LinkedDeque<int>();
            DequePrependAndShift(deque);
        }

        [Fact]
        public void TestDequePrependAndPop()
        {
            var deque = new Deque<int>();
            DequePrependAndPop(deque);
        }

        [Fact]
        public void TestLinkedDequePrependAndPop()
        {
            var deque = new LinkedDeque<int>();
            DequePrependAndPop(deque);
        }

        [Fact]
        public void TestDequeExceptions()
        {
            var deque = new Deque<int>();
            DequeExceptions(deque);
        }

        [Fact]
        public void TestLinkedDequeExceptions()
        {
            var deque = new LinkedDeque<int>();
            DequeExceptions(deque);
        }
        
        [Fact]
        public void TestDequeCopyTo()
        {
            var deque = new Deque<int>();
            DequeCopyTo(deque);
        }

        [Fact]
        public void TestLinkedDequeCopyTo()
        {
            var deque = new LinkedDeque<int>();
            DequeCopyTo(deque);
        }

        [Fact]
        public void TestDequeEnumerator()
        {
            var deque = new Deque<int>();
            DequeEnumerator(deque);
        }

        [Fact]
        public void TestLinkedDequeEnumerator()
        {
            var deque = new LinkedDeque<int>();
            DequeEnumerator(deque);
        }

        [Fact]
        public void TestStrictSetIntersect()
        {
            StrictSetIntersect<HashedSet<int>, HashedSet<int>>();
            StrictSetIntersect<HashedSet<int>, TreeSet<int>>();
            StrictSetIntersect<TreeSet<int>, HashedSet<int>>();
            StrictSetIntersect<TreeSet<int>, TreeSet<int>>();
        }

        [Fact]
        public void TestStrictSetUnion()
        {
            StrictSetUnion<HashedSet<int>, HashedSet<int>>();
            StrictSetUnion<HashedSet<int>, TreeSet<int>>();
            StrictSetUnion<TreeSet<int>, HashedSet<int>>();
            StrictSetUnion<TreeSet<int>, TreeSet<int>>();
        }

        [Fact]
        public void TestStrictSetDiffer()
        {
            StrictSetDiffer<HashedSet<int>, HashedSet<int>>();
            StrictSetDiffer<HashedSet<int>, TreeSet<int>>();
            StrictSetDiffer<TreeSet<int>, HashedSet<int>>();
            StrictSetDiffer<TreeSet<int>, TreeSet<int>>();
        }

        [Fact]
        public void TestStrictSetSubset()
        {
            StrictSetSubset<HashedSet<int>, HashedSet<int>>();
            StrictSetSubset<HashedSet<int>, TreeSet<int>>();
            StrictSetSubset<TreeSet<int>, HashedSet<int>>();
            StrictSetSubset<TreeSet<int>, TreeSet<int>>();
        }

        [Fact]
        public void TestStrictSetEqual()
        {
            StrictSetEqual<HashedSet<int>, HashedSet<int>>();
            StrictSetEqual<HashedSet<int>, TreeSet<int>>();
            StrictSetEqual<TreeSet<int>, HashedSet<int>>();
            StrictSetEqual<TreeSet<int>, TreeSet<int>>();
        }

        [Fact]
        public void TestStrictSetDisjoint()
        {
            StrictSetDisjoint<HashedSet<int>, HashedSet<int>>();
            StrictSetDisjoint<HashedSet<int>, TreeSet<int>>();
            StrictSetDisjoint<TreeSet<int>, HashedSet<int>>();
            StrictSetDisjoint<TreeSet<int>, TreeSet<int>>();
        }

        [Fact]
        public void TestStrictSetCompliment()
        {
            StrictSetCompliment<HashedSet<int>, HashedSet<int>>();
            StrictSetCompliment<HashedSet<int>, TreeSet<int>>();
            StrictSetCompliment<TreeSet<int>, HashedSet<int>>();
            StrictSetCompliment<TreeSet<int>, TreeSet<int>>();
        }

        [Fact]
        public void TestHasedBagCollectionOperations()
        {
            var bag = new HashedBag<Order>(new OrderEqualityComparer());
            bag.Fill(x => new Order { Id = x });
            bag.CollectionOperations<Order>(bag.Count);
        }

        [Fact]
        public void TestTreeBagCollectionOperations()
        {
            var bag = new TreeBag<Order>(new OrderComparer());
            bag.Fill(x => new Order { Id = x });
            bag.CollectionOperations<Order>(bag.Count);
        }

        [Fact]
        public void TestCompositeCollectionOperations()
        {
            var collection = new CompositeCollection<Order>();
            var list1 = new List<Order>();
            list1.Fill(x => new Order { Id = x });
            var list2 = new List<Order>();
            list2.Fill(x => new Order { Id = x }, 100);
            var list3 = new List<Order>();
            list3.Fill(x => new Order { Id = x }, 5);
            collection.AddAll(list1);
            collection.AddAll(list2);
            collection.AddAll(list3);
            collection.CollectionOperations<Order>(1105);
        }

        [Fact]
        public void TestHashedBimapCollectionOperations()
        {
            var bimap = Instantiate<HashedBimap<int, Order>>();
            bimap.CollectionOperations<KeyValuePair<int, Order>>(1000);
        }

        [Fact]
        public void TestTreeBimapCollectionOperations()
        {
            var bimap = new TreeBimap<int, Order>(new OrderComparer());
            bimap.Fill(x => new KeyValuePair<int, Order>(x, new Order { Id = x }));
            bimap.CollectionOperations<KeyValuePair<int, Order>>(1000);
        }

        [Fact]
        public void TestHashedMapCollectionOperations()
        {
            var map = Instantiate<HashedMap<int, Order>>();
            map.CollectionOperations<KeyValuePair<int, Order>>(1000);
        }

        [Fact]
        public void TestTreeMapCollectionOperations()
        {
            var map = Instantiate<TreeMap<int, Order>>();
            map.CollectionOperations<KeyValuePair<int, Order>>(1000);
        }

        [Fact]
        public void  TestSkipListMapCollectionOperations()
        {
            var map = Instantiate<SkipListMap<int, Order>>();
            map.CollectionOperations<KeyValuePair<int, Order>>(1000);
        }

        [Fact]
        public void TestCustomized32HashedMapCollectionOperations()
        {
            var map = Instantiate<Customized32HashedMap<int, Order>>();
            map.CollectionOperations<KeyValuePair<int, Order>>(1000);
        }

        [Fact]
        public void TestLinkedHashedMapCollectionOperations()
        {
            var map = Instantiate<LinkedHashedMap<int, Order>>();
            map.CollectionOperations<KeyValuePair<int, Order>>(1000);
        }

        [Fact]
        public void TestLruMapCollectionOperations()
        {
            var lru = new LruMap<int, Order>(1000);
            lru.Fill(x => new KeyValuePair<int, Order>(x, new Order { Id = x }));
            lru.CollectionOperations<KeyValuePair<int, Order>>(1000);
        }

        [Fact]
        public void TestMultiValueHashedMapCollectionOperations()
        {
            var mvMap = FillMultiValueMap<MultiValueHashedMap<int, Order>>();
            mvMap.CollectionOperations<KeyValuePair<int, Order>>(5000);
        }

        [Fact]
        public void TestMultiValueTreeMapCollectionOperations()
        {
            var mvMap = FillMultiValueMap<MultiValueTreeMap<int, Order>>();
            mvMap.CollectionOperations<KeyValuePair<int, Order>>(5000);
        }

        [Fact]
        public void TestReferenceMapCollectionOperations()
        {
            var map = new ReferenceMap<Order, Bill>();
            map.Fill(x => new KeyValuePair<Order, Bill>(new Order { Id = x }, new Bill { Id = x }));
            map.CollectionOperations<KeyValuePair<Order, Bill>>(1000);
        }

        [Fact]
        public void TestBoundedQueueCollectionOperations()
        {
            var queue = new BoundedQueue<int>(2000);
            for (var i = 0; i < 1000; i++)
            {
                queue.Enqueue(i + 1);
            }
            queue.CollectionOperations<int>(1000);
        }

        [Fact]
        public void TestDequeCollectionOperations()
        {
            var deque = new Deque<int>(2000);
            DequeCollectionOperations(deque);
        }

        [Fact]
        public void TestLinkedDequeCollectionOpertions()
        {
            var deque = new LinkedDeque<int>();
            DequeCollectionOperations(deque);
        }

        [Fact]
        public void TestMaxPriorityQueueCollectionOperations()
        {
            var pq = new MaxPriorityQueue<int>();
            for (var i = 0; i < 1500; i++)
            {
                pq.Push(i + 1);
            }
            for (var i = 0; i < 500; i++)
            {
                pq.Pop();
            }
            pq.CollectionOperations<int>(1000);
        }

        [Fact]
        public void TestMinPriorityQueueCollectionOperations()
        {
            var pq = new MinPriorityQueue<int>();
            for (var i = 0; i < 1500; i++)
            {
                pq.Push(i + 1);
            }

            for (var i = 0; i < 500; i++)
            {
                pq.Pop();
            }
            pq.CollectionOperations<int>(1000);
        }

        [Fact]
        public void TestHashedSetCollectionOperations()
        {
            var set = new HashedSet<int>();
            set.Fill(x => x + 1);
            set.CollectionOperations<int>(1000);
        }

        [Fact]
        public void TestReferenceSetCollectionOperations()
        {
            var set = new ReferenceSet<Order>();
            set.Fill(x => new Order { Id = x + 1 });
            set.CollectionOperations<Order>(1000);
        }

        [Fact]
        public void TestLruSetCollectionOperations()
        {
            var set = new LruSet<Order>(10000);
            set.Fill(x => new Order { Id = x }, 10000);
            set.CollectionOperations<Order>(10000);
        }

        public void TestLinkedSetCollectionOperations()
        {
            var set = new LinkedSet<Order>(10000, new OrderEqualityComparer());
            set.Fill(x => new Order { Id = x });
            set.CollectionOperations<Order>(10000);
        }

        [Fact]
        public void TestTreeSetCollectionOperations()
        {
            var set = new TreeSet<int>();
            set.Fill(x => x + 1);
            set.CollectionOperations<int>(1000);
        }

        [Fact]
        public void TestSkipListSetCollectionOperations()
        {
            var set = new SkipListSet<int>();
            set.Fill(x => x + 1);
            set.CollectionOperations<int>(1000);
        }

        [Fact]
        public void TestHashedBimapDictionaryOperations()
        {
            var bimap = new HashedBimap<Order, Bill>(new OrderEqualityComparer(), new BillEqualityComparer());
            bimap.DictionaryOperations(x => new Order {Id = x}, y => new Bill {Id = y }, new BillEqualityComparer().Equals);
        }

        [Fact]
        public void TestTreeBimapDictionaryOperations()
        {
            var bimap = new TreeBimap<Order, Bill>(new OrderComparer(), new BillComparer());
            bimap.DictionaryOperations<Order, Bill>(x => new Order { Id = x }, y => new Bill { Id = y }, new BillEqualityComparer().Equals);
        }

        [Fact]
        public void TestHashedMapDictionaryOperations()
        {
            var map = new HashedMap<Order, Bill>(new OrderEqualityComparer());
            map.DictionaryOperations(x => new Order { Id = x }, y => new Bill { Id = y }, new BillEqualityComparer().Equals);
        }

        [Fact]
        public void TestLinkedHashedMapDictionaryOperations()
        {
            var map = new LinkedHashedMap<Order, Bill>(new OrderEqualityComparer());
            map.DictionaryOperations(x => new Order { Id = x }, y => new Bill { Id = y }, new BillEqualityComparer().Equals);
        }

        [Fact]
        public void TestTreeMapDictionaryOperations()
        {
            var map = new TreeMap<Order, Bill>(new OrderComparer());
            map.DictionaryOperations(x => new Order { Id = x }, y => new Bill { Id = y }, new BillEqualityComparer().Equals);
        }

        [Fact]
        public void TestSkipListMapDictionaryOperations()
        {
            var map = new SkipListMap<Order, Bill>(new OrderComparer());
            map.DictionaryOperations(x => new Order { Id = x }, y => new Bill { Id = y }, new BillEqualityComparer().Equals);
        }

        [Fact]
        public void TestLruMapDictionaryOperations()
        {
            var map = new LruMap<Order, Bill>(2000 ,new OrderEqualityComparer());
            map.DictionaryOperations(x => new Order { Id = x }, y => new Bill { Id = y }, new BillEqualityComparer().Equals);
        }

        [Fact]
        public void TestReferenceMapDictionaryOperations()
        {
            var map = new ReferenceMap<Order, Bill>();
            ReferenceMapDictionaryOperations(map);
        }

        [Fact]
        public void TestDefaultComparer()
        {
            var map = new TreeMap<Employee, int>();
            map.DictionaryOperations(x => new Employee { Id = x, Mark = x * 5 }, y => y, EqualityComparer<int>.Default.Equals);
        }

        [Fact]
        public void TestDefaultEqualityComparer()
        {
            var map = new HashedMap<Employee, int>();
            map.DictionaryOperations(x => new Employee { Id = x, Mark = x * 5 }, y => y, EqualityComparer<int>.Default.Equals);
        }

        private void ReferenceMapDictionaryOperations(IDictionary source)
        {
            var list = new List<Order>();
            for (var i = 0; i < 1000; i++)
            {
                var order = new Order { Id = i };
                var bill = new Bill { Id = i };
                source.Add(order, bill);
                list.Add(order);
            }

            Assert.Equal(1000, source.Count);

            Assert.False(source.Contains(new Order {Id = 1}));
            Assert.False(source.Contains(new Order { Id = 2 }));

            for (var i = 0; i < 200; i++)
            {
                Assert.True(source.Contains(list[i]));
                source.Remove(list[i]);
            }
            Assert.Equal(800, source.Count);
            for (var i = 0; i < 200; i++)
            {
                Assert.False(source.Contains(list[i]));
            }

            for (var i = 500; i < 700; i++)
            {
                var v = (Bill)source[list[i]];
                Assert.Equal(i, v.Id);
            }

            var keys = source.Keys;
            Assert.Equal(800, keys.Count);
            var values = source.Values;
            Assert.Equal(800, values.Count);
            var currentCount = 0;
            foreach (var item in source)
            {
                currentCount++;
            }
            Assert.Equal(800, currentCount);
            Assert.Equal(currentCount, source.Count);

            source.Clear();
            Assert.Equal(0, source.Count);
        
        }

        private T FillMultiValueMap<T>() where T : IMultiValueMap<int, Order>, new()
        {
            var mvMap = new T();
            for (var i = 0; i < 1000; i++)
            {
                for (var j = 0; j < 5; j++)
                { 
                    mvMap.Add(i, new Order { Id = j });
                }
            }

            return mvMap;
        }

        private T Instantiate<T>() where T : IDictionary<int, Order>, new()
        {
            var bimap = new T();
            bimap.Fill(x => new KeyValuePair<int, Order>(x, new Order { Id = x }));
            return bimap;
        }

        private void StrictSetCompliment<S1, S2>() where S1 : IStrictSet<int>, new() where S2 : IStrictSet<int>, new()
        {
            var s1 = new S1();
            var s2 = new S2();
            Fill(s1, 0, 1000);
            Fill(s2, 0, 2000);
            var result = s1.Compliment(s2);
            for (var i = 0; i < 1000; i++)
            {
                Assert.False(s1.Contains(i));
                Assert.False(result.Contains(i));
            }
            for (var i = 1000; i < 2000; i++)
            {
                Assert.True(s1.Contains(i));
                Assert.True(result.Contains(i));
            }

            var s3 = new S1();
            var s4 = new S2();
            Fill(s4, 0, 1000);
            var result2 = s3.Compliment(s4);
            Assert.True(s3.IsEqualWith(s4));
            Assert.True(result2.IsEqualWith(s4));
            for (var i = 0; i < 1000; i++)
            {
                Assert.True(s3.Contains(i));
                Assert.True(result2.Contains(i));
            }

            var s5 = new S1();
            var s6 = new S2();
            Fill(s5, 0, 1000);
            Fill(s6, 500, 1000);
            Assert.Throws(typeof(InvalidOperationException), () => s5.Compliment(s6));

            var s7 = new S1();
            var s8 = new S2();
            var result3 = s7.Compliment(s8);
            Assert.True(s7.IsEqualWith(new S1()));
            Assert.True(s7.IsEqualWith(new S2()));
            Assert.True(result3.IsEqualWith(new S1()));
            Assert.True(result3.IsEqualWith(new S2()));
        }

        private void StrictSetDisjoint<S1, S2>() where S1 : IStrictSet<int>, new() where S2 : IStrictSet<int>, new()
        {
            var s1 = new S1();
            var s2 = new S2();
            Fill(s1, 0, 1000);
            Fill(s2, 1000, 2000);
            Assert.True(s1.IsDisjointWith(s2));

            var s3 = new S1();
            var s4 = new S2();
            Fill(s3, 0, 2000);
            Fill(s4, 1000, 2000);
            Assert.False(s3.IsDisjointWith(s4));

            var s5 = new S1();
            var s6 = new S2();
            Assert.True(s5.IsDisjointWith(s6));

            var s7 = new S1();
            var s8 = new S2();
            Fill(s7, 0, 1000);
            Assert.True(s7.IsDisjointWith(s8));
            Assert.True(s8.IsDisjointWith(s7));

        }
        
        private void StrictSetEqual<S1, S2>() where S1 : IStrictSet<int>, new() where S2 : IStrictSet<int>, new()
        {
            var s1 = new S1();
            var s2 = new S2();
            Fill(s1, 0, 10000);
            Fill(s2, 0, 5000);
            Assert.False(s1.IsEqualWith(s2));

            var s3 = new S1();
            var s4 = new S2();

            Fill(s3, 0, 1000);
            Fill(s4, 0, 1000);
            Assert.True(s3.IsEqualWith(s4));

            var s5 = new S1();
            var s6 = new S2();
            Fill(s5, 0, 1000);
            Fill(s6, 1000, 1000);
            Assert.False(s5.IsEqualWith(s6));

            var s7 = new S1();
            var s8 = new S2();
            Fill(s7, 0, 1000);
            Fill(s8, 0, 1999);
            Assert.False(s7.IsEqualWith(s8));

            var s9 = new S1();
            var s10 = new S2();
            Assert.True(s9.IsEqualWith(s10));
        }

        private void StrictSetSubset<S1, S2>()
            where S1 : IStrictSet<int>, new()
            where S2 : IStrictSet<int>, new()
        {
            var s1 = new S1();
            var s2 = new S2();
            Fill(s1, 0, 5000);
            Fill(s2, 4000, 5000);
            Assert.False(s1.IsSubsetOf(s2));
            Assert.False(s2.IsSubsetOf(s1));
            Assert.False(s1.IsProperSubsetOf(s2));
            Assert.False(s2.IsProperSubsetOf(s1));


            var s3 = new S1();
            var s4 = new S2();
            Fill(s3, 0, 5000);
            Fill(s4, 1000, 2000);
            Assert.False(s3.IsSubsetOf(s4));
            Assert.True(s4.IsSubsetOf(s3));
            Assert.False(s3.IsProperSubsetOf(s4));
            Assert.True(s4.IsProperSubsetOf(s3));

            var s5 = new S1();
            var s6 = new S2();
            Fill(s5, 1000, 2000);
            Fill(s6, 0, 5000);
            Assert.True(s5.IsSubsetOf(s6));
            Assert.False(s6.IsSubsetOf(s5));
            Assert.True(s5.IsProperSubsetOf(s6));
            Assert.False(s6.IsProperSubsetOf(s5));

            var s7 = new S1();
            var s8 = new S2();
            Fill(s7, 0, 1000);
            Fill(s8, 2000, 1000);
            Assert.False(s7.IsSubsetOf(s8));
            Assert.False(s8.IsSubsetOf(s7));
            Assert.False(s7.IsProperSubsetOf(s8));
            Assert.False(s8.IsProperSubsetOf(s7));

            var s9 = new S2();
            Fill(s9, 0, 1000);
            Assert.True(s7.IsSubsetOf(s9));
            Assert.True(s9.IsSubsetOf(s7));
            Assert.False(s7.IsProperSubsetOf(s9));
            Assert.False(s9.IsProperSubsetOf(s7));
            Assert.True(s7.IsSubsetOf(s7));
            Assert.False(s7.IsProperSubsetOf(s7));

            var s10 = new S1();
            var s11 = new S2();
            Assert.True(s10.IsSubsetOf(s11));
            Assert.True(s11.IsSubsetOf(s10));
            Assert.False(s10.IsProperSubsetOf(s11));
            Assert.False(s11.IsProperSubsetOf(s10));
        }

        private void StrictSetDiffer<S1, S2>()
            where S1 : IStrictSet<int>, new()
            where S2 : IStrictSet<int>, new()
        {
            var origin = new S1();
            var other = new S2();
            Fill(origin, 0, 5000);
            Fill(other, 4000, 5000);
            var result = origin.Differ(other);
            for (var i = 0; i < 4000; i++)
            {
                Assert.True(origin.Contains(i));
                Assert.True(result.Contains(i));
            }
            for (var i = 4000; i < 5000; i++)
            {
                Assert.False(origin.Contains(i));
                Assert.False(result.Contains(i));
            }
        }

        private void StrictSetUnion<S1, S2>()
            where S1 : IStrictSet<int>, new()
            where S2 : IStrictSet<int>, new()
        {
            var origin = new S1();
            var other = new S2();
            Fill(origin, 0, 5000);
            Fill(other, 4000, 5000);
            var result = origin.Union(other);
            Assert.Equal(9000, origin.Count);
            Assert.Equal(9000, result.Count);
            for (var i = 0; i < 9000; i++)
            {
                Assert.True(origin.Contains(i));
                Assert.True(result.Contains(i));
            }
        }

        private void Fill(IStrictSet<int> set, int start, int count)
        {
            foreach (var i in Enumerable.Range(start, count))
            {
                set.Add(i);
            }
        }

        private void StrictSetIntersect<S1, S2>() where S1 : IStrictSet<int>, new() where S2 : IStrictSet<int>, new()
        {
            var origin = new S1();
            var other = new S2();
            Fill(origin, 0, 5000);
            Fill(other, 4000, 5000);
            var result = origin.Intersect(other);
            Assert.Equal(1000, origin.Count);
            Assert.Equal(1000, result.Count);
            for(var i = 0; i < 4000; i++)
            {
                Assert.False(origin.Contains(i));
                Assert.False(result.Contains(i));
            }

            for (var i = 4000; i < 5000; i++)
            {
                Assert.True(origin.Contains(i));
                Assert.True(result.Contains(i));
            }

            for (var i = 5000; i < 9000; i++)
            {
                Assert.False(origin.Contains(i));
                Assert.False(result.Contains(i));
            }

            var third = new S1();
            foreach (var i in Enumerable.Range(5000, 1000))
            {
                third.Add(i);
            }

            origin.Intersect(third);

            Assert.Equal(0, origin.Count);
        }

        private void TestDeque(IDeque<string> deque)
        {
            for (var i = 0; i < 1000; i++)
            {
                deque.Prepend(Guid.NewGuid().ToString());
            }
            Assert.Equal(1000, deque.Count);
            for (var i = 0; i < 1000; i++)
            {
                deque.Append(Guid.NewGuid().ToString());
            }
            Assert.Equal(2000, deque.Count);
        }

        private void DequeOperations(IDeque<string> deque)
        {
            var lastPrepend = string.Empty;
            var lastAppend = string.Empty;
            var testNumber = 10000;
            for (var i = 0; i < testNumber; i++)
            {
                lastPrepend = Guid.NewGuid().ToString();
                lastAppend = Guid.NewGuid().ToString();
                deque.Prepend(lastPrepend);
                deque.Append(lastAppend);
            }
            Assert.Equal(testNumber * 2, deque.Count);
            Assert.Equal(lastPrepend, deque.First);
            Assert.Equal(lastAppend, deque.Last);
            Assert.Equal(lastPrepend, deque.Shift());
            Assert.Equal(testNumber * 2 - 1, deque.Count);
            Assert.Equal(lastAppend, deque.Pop());
            Assert.Equal(testNumber * 2 - 2, deque.Count);

            for (var i = 0; i < (testNumber * 2 - 2); i++)
            {
                Assert.False(string.IsNullOrEmpty(deque.Pop()));
            }
            Assert.Equal(0, deque.Count);
        }

        private void DequeAppendAndShift(IDeque<int> deque)
        {
            for (var i = 0; i < 100000; i++)
            {
                deque.Append(i);
            }

            for (var i = 0; i < 100000; i++)
            {
                Assert.Equal(i, deque.First);
                Assert.Equal(99999, deque.Last);
                Assert.Equal(i, deque.Shift());
            }
            Assert.Equal(0, deque.Count);
        }

        private void DequeAppendAndPop(IDeque<int> deque)
        {
            for (var i = 0; i < 100000; i++)
            {
                deque.Append(i);
            }

            for (var i = 100000; i > 0; i--)
            {
                Assert.Equal(0, deque.First);
                Assert.Equal(i - 1, deque.Last);
                Assert.Equal(i - 1, deque.Pop());
            }
            Assert.Equal(0, deque.Count);
        }

        private void DequePrependAndShift(IDeque<int> deque)
        {
            for (var i = 0; i < 100000; i++)
            {
                deque.Prepend(i);
            }

            for (var i = 0; i < 100000; i++)
            {
                Assert.Equal(100000 - i - 1, deque.First);
                Assert.Equal(0, deque.Last);
                Assert.Equal(100000 - i - 1, deque.Shift());
            }
            Assert.Equal(0, deque.Count);
        }

        private void DequePrependAndPop(IDeque<int> deque)
        {
            for (var i = 0; i < 100000; i++)
            {
                deque.Prepend(i);
            }

            for (var i = 0; i < 100000; i++)
            {
                Assert.Equal(99999, deque.First);
                Assert.Equal(i, deque.Last);
                Assert.Equal(i, deque.Pop());
            }
            Assert.Equal(0, deque.Count);
        }

        private void DequeExceptions(IDeque<int> deque)
        {
            Assert.Throws(typeof(InvalidOperationException), () => deque.Pop());
            Assert.Throws(typeof(InvalidOperationException), () => deque.Shift());
            Assert.Throws(typeof(InvalidOperationException), () => deque.First);
            Assert.Throws(typeof(InvalidOperationException), () => deque.Last);
        }

        private void DequeCopyTo(IDeque<int> deque)
        {
            for (var i = 0; i < 1000; i++)
            {
                deque.Append(i);
            }
            var items = new int[1003];
            ((ICollection)deque).CopyTo(items, 3);

            for (var i = 3; i < 1003; i++)
            {
                Assert.Equal(items[i], i - 3);
            }
        }

        private void DequeEnumerator(IDeque<int> deque)
        {
            var cursor1 = 0;
            foreach (var item in deque)
            {
                cursor1++;
            }
            Assert.Equal(0, cursor1);

            for (var i = 0; i < 100000; i++)
            {
                deque.Append(i);
            }
            var cursor2 = 0;
            foreach (var item in deque)
            {
                Assert.Equal(cursor2, item);
                cursor2++;

            }
            for (var i = 1; i < 100001; i++)
            {
                deque.Prepend(-i);
            }
            var cursor3 = -100000;
            foreach (var item in deque)
            {
                Assert.Equal(cursor3, item);
                cursor3++;
            }
        }

        private void DequeCollectionOperations(IDeque<int> deque)
        {
            for (var i= 0; i < 1000; i++)
            {
                deque.Append(i + 1);
            }
            deque.CollectionOperations<int>(1000);
        }
    }
}
