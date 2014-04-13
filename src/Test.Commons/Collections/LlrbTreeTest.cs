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

using Commons.Collections;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Commons.Collections
{
    [TestFixture]
    public class LlrbTreeTest
    {
        [Test]
        public void TestTreeSetContructor()
        {
            Random randomValue = new Random((int)(DateTime.Now.Ticks & 0x0000FFFF));
            List<Order> orderList = new List<Order>();
            for (var i = 0; i < 100; i++)
            {
                Order order = new Order();
                order.Id = randomValue.Next();
                order.Name = order.Id + "(*^&^%";
                if (!orderList.Contains(order, new OrderEqualityComparer()))
                {
                    orderList.Add(order);
                }
            }
            TreeSet<Order> orderSet = new TreeSet<Order>(orderList, new OrderComparer());
            foreach (var item in orderList)
            {
                Assert.IsTrue(orderSet.Contains(item));
            }

            TreeSet<int> simpleSet = new TreeSet<int>(Enumerable.Range(0, 100));
            for (var i = 0; i < 100; i++)
            {
                Assert.IsTrue(simpleSet.Contains(i));
            }
            Assert.IsFalse(simpleSet.Contains(101));
        }

        [Test]
        public void TestTreeSetAdd()
        {
            Random randomValue = new Random((int)(DateTime.Now.Ticks & 0x0000FFFF));
            List<Order> orderList = new List<Order>();
            TreeSet<Order> orderSet = new TreeSet<Order>(new OrderComparer());

            for (var i = 0; i < 1000; )
            {
                var next = randomValue.Next();
                var orderToSet = new Order();
                orderToSet.Id = next;
                orderToSet.Name = next + "*%%%(*&()*_)(;;";
                if (!orderSet.Contains(orderToSet))
                {
                    var orderToList = new Order();
                    orderToList.Id = next;
                    orderToList.Name = next + "2*%^*((%";
                    orderList.Add(orderToList);
                    orderSet.Add(orderToSet);
                    i++;
                }
            }

            Assert.AreEqual(1000, orderSet.Count);

            foreach (var item in orderList)
            {
                Assert.IsTrue(orderSet.Contains(item));
            }
            orderSet.Clear();
            Assert.AreEqual(0, orderSet.Count);
        }

        [Test]
        public void TestTreeSetRemove()
        {
            Random r = new Random((int)(DateTime.Now.Ticks & 0x0000FFFF));
            List<Order> orderlist = new List<Order>();
            TreeSet<Order> orderSet = new TreeSet<Order>(new OrderComparer());

            for (var i = 0; i < 1000; )
            {
                var next = r.Next();
                var orderToSet = new Order();
                orderToSet.Id = next;
                orderToSet.Name = "(JHOI(*^Y" + next;
                if (!orderSet.Contains(orderToSet))
                {
                    var orderToList = new Order();
                    orderToList.Id = next;
                    orderToList.Name = "(*^TGUHJIO" + next;
                    orderlist.Add(orderToList);
                    orderSet.Add(orderToSet);
                    i++;
                }
            }
            for (var i = 0; i < 500; i++)
            {
                Assert.IsTrue(orderSet.Remove(orderlist[i]));
                Assert.IsFalse(orderSet.Contains(orderlist[i]));
            }
            Assert.AreEqual(500, orderSet.Count);
            var notExist = new Order();
            notExist.Id = 1;
            notExist.Name = "not exist";
            Assert.IsFalse(orderSet.Remove(notExist));
        }

        [Test]
        public void TestTreeSetRemoveMax()
        {
            Random r = new Random((int)(DateTime.Now.Ticks & 0x0000FFFF));
            List<Order> orderlist = new List<Order>();
            TreeSet<Order> orderSet = new TreeSet<Order>(new OrderComparer());

            for (var i = 0; i < 200; )
            {
                var next = r.Next();
                var orderToSet = new Order();
                orderToSet.Id = next;
                orderToSet.Name = "(JHOI(*^Y" + next;
                if (!orderSet.Contains(orderToSet))
                {
                    var orderToList = new Order();
                    orderToList.Id = next;
                    orderToList.Name = "(*^TGUHJIO" + next;
                    orderlist.Add(orderToList);
                    orderSet.Add(orderToSet);
                    i++;
                }
            }

            var orderedList = orderlist.OrderByDescending(o => o.Id).ToList();
            for (var i = 0; i < 50; i++)
            {
                var maxOrder = orderedList[i];
                Assert.IsTrue(orderSet.Contains(maxOrder));
                var comparer = new OrderComparer();
                Assert.That(comparer.Compare(maxOrder, orderSet.Max) == 0);
                orderSet.RemoveMax();
                Assert.IsFalse(orderSet.Contains(maxOrder));
                Assert.That(comparer.Compare(maxOrder, orderSet.Max) > 0);
            }

            Assert.AreEqual(150, orderSet.Count);
        }

        [Test]
        public void TestTreeSetRemoveMin()
        {
            Random r = new Random((int)(DateTime.Now.Ticks & 0x0000FFFF));
            List<Order> orderlist = new List<Order>();
            TreeSet<Order> orderSet = new TreeSet<Order>(new OrderComparer());

            for (var i = 0; i < 200; )
            {
                var next = r.Next();
                var orderToSet = new Order();
                orderToSet.Id = next;
                orderToSet.Name = "(JHOI(*^Y" + next;
                if (!orderSet.Contains(orderToSet))
                {
                    var orderToList = new Order();
                    orderToList.Id = next;
                    orderToList.Name = "(*^TGUHJIO" + next;
                    orderlist.Add(orderToList);
                    orderSet.Add(orderToSet);
                    i++;
                }
            }

            var orderedList = orderlist.OrderBy(o => o.Id).ToList();
            for (var i = 0; i < 50; i++)
            {
                var minOrder = orderedList[i];
                Assert.IsTrue(orderSet.Contains(minOrder));
                var comparer = new OrderComparer();
                Assert.That(comparer.Compare(minOrder, orderSet.Min) == 0);
                orderSet.RemoveMin();
                Assert.IsFalse(orderSet.Contains(minOrder));
                Assert.That(comparer.Compare(minOrder, orderSet.Min) < 0);
            }

            Assert.AreEqual(150, orderSet.Count);
        }

        [Test]
        public void TestTreeSetCopyTo()
        {
            Random r = new Random((int)(DateTime.Now.Ticks & 0x0000FFFF));
            TreeSet<Order> orderSet = new TreeSet<Order>(new OrderComparer());
            for (var i = 0; i < 1000; )
            {
                var o = new Order();
                o.Id = r.Next();
                o.Name = i + " age4356A;";
                if (!orderSet.Contains(o))
                {
                    orderSet.Add(o);
                    i++;
                }
            }
            var orders = new Order[1003];
            orderSet.CopyTo(orders, 3);
            for (var i = 3; i < 1003; i++)
            {
                Assert.IsTrue(orderSet.Contains(orders[i]));
            }
            for (var i = 0; i < 3; i++)
            {
                Assert.IsNull(orders[i]);
            }
        }

        [Test]
        public void TestTreeSetEnumerator()
        {
            Random r = new Random((int)(DateTime.Now.Ticks & 0x0000FFFF));
            List<Order> orderlist = new List<Order>();
            TreeSet<Order> orderSet = new TreeSet<Order>(new OrderComparer());

            for (var i = 0; i < 200; )
            {
                var next = r.Next();
                var orderToSet = new Order();
                orderToSet.Id = next;
                orderToSet.Name = "(JHOI(*^Y" + next;
                if (!orderSet.Contains(orderToSet))
                {
                    var orderToList = new Order();
                    orderToList.Id = next;
                    orderToList.Name = "(*^TGUHJIO" + next;
                    orderlist.Add(orderToList);
                    orderSet.Add(orderToSet);
                    i++;
                }
            }
            var total = 0;
            foreach (var item in orderSet)
            {
                total++;
                Assert.IsTrue(orderlist.Contains(item, new OrderEqualityComparer()));
            }
            Assert.AreEqual(total, orderSet.Count);
        }

        [Test]
        public void TestTreeSetNoItem()
        {
            TreeSet<int> set = new TreeSet<int>();
            var index = 0;
            foreach (var item in set)
            {
                index++;
            }
            Assert.AreEqual(0, index);
            Assert.IsFalse(set.Contains(5));
            Assert.IsFalse(set.Remove(6));
            var array = new int[5] { 0 , 0, 0, 0, 0 };
            set.CopyTo(array, 0);
            Assert.AreEqual(0, array[0]);
        }

        [Test]
        public void TestMapContructor()
        {
            Random randomValue = new Random((int)(DateTime.Now.Ticks & 0x0000FFFF));
            Dictionary<Order, Guid> orderDict = new Dictionary<Order, Guid>();
            for (var i = 0; i < 100; i++)
            {
                Order order = new Order();
                order.Id = randomValue.Next();
                order.Name = order.Id + "(*^&^%";
                if (!orderDict.ContainsKey(order))
                {
                    orderDict.Add(order, Guid.NewGuid());
                }
            }
            TreeMap<Order, Guid> orderMap = new TreeMap<Order, Guid>(orderDict, new OrderComparer());
            foreach (var item in orderDict.Keys)
            {
                Assert.IsTrue(orderMap.ContainsKey(item));
            }

            Dictionary<int, string> simpleDict = new Dictionary<int, string>();
            for (int i = 0; i < 100; i++)
            {
                simpleDict.Add(i, Guid.NewGuid().ToString());
            }
            TreeMap<int, string> simpleMap = new TreeMap<int, string>(simpleDict);
            Assert.AreEqual(simpleDict.Count, simpleMap.Count);
            foreach (var k in simpleDict.Keys)
            {
                Assert.IsTrue(simpleMap.ContainsKey(k));
            }
        }

        [Test]
        public void TestMapAdd()
        {
            Random randomValue = new Random((int)(DateTime.Now.Ticks & 0x0000FFFF));
            List<Order> orderList = new List<Order>();
            TreeMap<Order, Bill> orderMap = new TreeMap<Order, Bill>(new OrderComparer());

            for (var i = 0; i < 1000; )
            {
                var next = randomValue.Next();
                var order = new Order();
                order.Id = next;
                order.Name = next + "*%%%(*&()*_)(;;";
                if (!orderList.Contains(order))
                {
                    var orderToList = new Order();
                    orderToList.Id = next;
                    orderToList.Name = next + "2*%^*((%";
                    orderList.Add(orderToList);
                    Bill bill = new Bill();
                    bill.Id = next;
                    bill.Count = next % 1000;
                    orderMap.Add(order, bill);
                    i++;
                }
            }

            Assert.AreEqual(1000, orderMap.Count);

            foreach (var item in orderList)
            {
                Assert.IsTrue(orderMap.ContainsKey(item));
            }
            orderMap.Clear();
            Assert.AreEqual(0, orderMap.Count);
        }

        [Test]
        public void TestMapRemove()
        {
            Random randomValue = new Random((int)(DateTime.Now.Ticks & 0x0000FFFF));
            List<Order> orderList = new List<Order>();
            TreeMap<Order, Bill> orderMap = new TreeMap<Order, Bill>(new OrderComparer());

            for (var i = 0; i < 1000; ) 
            {
                var next = randomValue.Next();
                var order = new Order();
                order.Id = next;
                order.Name = next + "*%%%(*&()*_)(;;";
                if (!orderList.Contains(order))
                {
                    var orderToList = new Order();
                    orderToList.Id = next;
                    orderToList.Name = next + "2*%^*((%";
                    orderList.Add(orderToList);
                    Bill bill = new Bill();
                    bill.Id = next;
                    bill.Count = next % 1000;
                    orderMap.Add(order, bill);
                    i++;
                }
            }

            for (var i = 0; i < 500; i++)
            {
                Assert.IsTrue(orderMap.Remove(orderList[i]));
                Assert.IsFalse(orderMap.Contains(orderList[i]));
            }
            Assert.AreEqual(500, orderMap.Count);
            var notExist = new Order();
            notExist.Id = 1;
            notExist.Name = "not exist";
            Assert.IsFalse(orderMap.Remove(notExist));
        }

        [Test]
        public void TestMapRemoveMax()
        {
            Random r = new Random((int)(DateTime.Now.Ticks & 0x0000FFFF));
            List<Order> orderlist = new List<Order>();
            TreeMap<Order, Bill> orderMap = new TreeMap<Order, Bill>(new OrderComparer());

            for (var i = 0; i < 200; )
            {
                var next = r.Next();
                var order = new Order();
                order.Id = next;
                order.Name = "(JHOI(*^Y" + next;
                if (!orderMap.ContainsKey(order))
                {
                    var orderToList = new Order();
                    orderToList.Id = next;
                    orderToList.Name = "(*^TGUHJIO" + next;
                    orderlist.Add(orderToList);
                    Bill bill = new Bill();
                    bill.Id = next;
                    bill.Count = next % 1000;
                    orderMap.Add(order, bill);
                    i++;
                }
            }

            var orderedList = orderlist.OrderByDescending(o => o.Id).ToList();
            for (var i = 0; i < 50; i++)
            {
                var maxOrder = orderedList[i];
                Assert.IsTrue(orderMap.Contains(maxOrder));
                var comparer = new OrderComparer();
                Assert.That(comparer.Compare(maxOrder, orderMap.Max.Key) == 0);
                orderMap.RemoveMax();
                Assert.IsFalse(orderMap.ContainsKey(maxOrder));
                Assert.That(comparer.Compare(maxOrder, orderMap.Max.Key) > 0);
            }

            Assert.AreEqual(150, orderMap.Count);
        }

        [Test]
        public void TestMapRemoveMin()
        {
            Random r = new Random((int)(DateTime.Now.Ticks & 0x0000FFFF));
            List<Order> orderlist = new List<Order>();
            TreeMap<Order, Bill> orderMap = new TreeMap<Order, Bill>(new OrderComparer());

            for (var i = 0; i < 200; )
            {
                var next = r.Next();
                var order = new Order();
                order.Id = next;
                order.Name = "(JHOI(*^Y" + next;
                if (!orderMap.ContainsKey(order))
                {
                    var orderToList = new Order();
                    orderToList.Id = next;
                    orderToList.Name = "(*^TGUHJIO" + next;
                    orderlist.Add(orderToList);
                    Bill bill = new Bill();
                    bill.Id = next;
                    bill.Count = next % 1000;
                    orderMap.Add(order, bill);
                    i++;
                }
            }

            var orderedList = orderlist.OrderBy(o => o.Id).ToList();
            for (var i = 0; i < 50; i++)
            {
                var minOrder = orderedList[i];
                Assert.IsTrue(orderMap.Contains(minOrder));
                var comparer = new OrderComparer();
                Assert.That(comparer.Compare(minOrder, orderMap.Min.Key) == 0);
                orderMap.RemoveMin();
                Assert.IsFalse(orderMap.ContainsKey(minOrder));
                Assert.That(comparer.Compare(minOrder, orderMap.Min.Key) < 0);
            }

            Assert.AreEqual(150, orderMap.Count);
        }

        [Test]
        public void TestMapCopyTo()
        {
            Random r = new Random((int)(DateTime.Now.Ticks & 0x0000FFFF));
            TreeMap<Order, Bill> orderMap = new TreeMap<Order, Bill>(new OrderComparer());
            for (var i = 0; i < 1000; )
            {
                var o = new Order();
                o.Id = r.Next();
                o.Name = i + " age4356A;";
                var bill = new Bill();
                bill.Id = o.Id;
                bill.Count = o.Id % 1000;
                if (!orderMap.ContainsKey(o))
                {
                    orderMap.Add(o, bill);
                    i++;
                }
            }
            var kvps = new KeyValuePair<Order, Bill>[1003];
            orderMap.CopyTo(kvps, 3);
            for (var i = 3; i < 1003; i++)
            {
                Assert.IsTrue(orderMap.Contains(kvps[i]));
            }
            for (var i = 0; i < 3; i++)
            {
                Assert.IsNull(kvps[i].Key);
                Assert.IsNull(kvps[i].Value);
            }
        }

        [Test]
        public void TestMapEnumerator()
        {
        }

        [Test]
        public void TestMapNoItem()
        {
        }
    }
} 