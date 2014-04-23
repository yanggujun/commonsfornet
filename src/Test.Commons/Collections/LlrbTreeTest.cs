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

using Xunit;

using Commons.Collections;
using Commons.Collections.Map;
using Commons.Collections.Set;

namespace Test.Commons.Collections
{
    public class LlrbTreeTest
    {
        [Fact]
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
                Assert.True(orderSet.Contains(item));
            }

            TreeSet<int> simpleSet = new TreeSet<int>(Enumerable.Range(0, 100));
            for (var i = 0; i < 100; i++)
            {
                Assert.True(simpleSet.Contains(i));
            }
            Assert.False(simpleSet.Contains(101));
        }

        [Fact]
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

            Assert.Equal(1000, orderSet.Count);

            foreach (var item in orderList)
            {
                Assert.True(orderSet.Contains(item));
            }
            orderSet.Clear();
            Assert.Equal(0, orderSet.Count);
        }

        [Fact]
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
                Assert.True(orderSet.Remove(orderlist[i]));
                Assert.False(orderSet.Contains(orderlist[i]));
            }
            Assert.Equal(500, orderSet.Count);
            var notExist = new Order();
            notExist.Id = 1;
            notExist.Name = "not exist";
            Assert.False(orderSet.Remove(notExist));
        }

        [Fact]
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
                Assert.True(orderSet.Contains(maxOrder));
                var comparer = new OrderComparer();
                Assert.True(comparer.Compare(maxOrder, orderSet.Max) == 0);
                orderSet.RemoveMax();
                Assert.False(orderSet.Contains(maxOrder));
                Assert.True(comparer.Compare(maxOrder, orderSet.Max) > 0);
            }

            Assert.Equal(150, orderSet.Count);
        }

        [Fact]
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
                Assert.True(orderSet.Contains(minOrder));
                var comparer = new OrderComparer();
                Assert.True(comparer.Compare(minOrder, orderSet.Min) == 0);
                orderSet.RemoveMin();
                Assert.False(orderSet.Contains(minOrder));
                Assert.True(comparer.Compare(minOrder, orderSet.Min) < 0);
            }

            Assert.Equal(150, orderSet.Count);
        }

        [Fact]
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
                Assert.True(orderSet.Contains(orders[i]));
            }
            for (var i = 0; i < 3; i++)
            {
                Assert.Null(orders[i]);
            }
        }

        [Fact]
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
                Assert.True(orderlist.Contains(item, new OrderEqualityComparer()));
            }
            Assert.Equal(total, orderSet.Count);
        }

        [Fact]
        public void TestTreeSetNoItem()
        {
            TreeSet<int> set = new TreeSet<int>();
            var index = 0;
            foreach (var item in set)
            {
                index++;
            }
            Assert.Equal(0, index);
            Assert.Empty(set);
            Assert.False(set.Contains(5));
            Assert.False(set.Remove(6));
            var array = new int[5] { 0 , 0, 0, 0, 0 };
            set.CopyTo(array, 0);
            Assert.Equal(0, array[0]);
        }

        [Fact]
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
                Assert.True(orderMap.ContainsKey(item));
            }

            Dictionary<int, string> simpleDict = new Dictionary<int, string>();
            for (int i = 0; i < 100; i++)
            {
                simpleDict.Add(i, Guid.NewGuid().ToString());
            }
            TreeMap<int, string> simpleMap = new TreeMap<int, string>(simpleDict);
            Assert.Equal(simpleDict.Count, simpleMap.Count);
            foreach (var k in simpleDict.Keys)
            {
                Assert.True(simpleMap.ContainsKey(k));
            }
        }

        [Fact]
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

            Assert.Equal(1000, orderMap.Count);

            foreach (var item in orderList)
            {
                Assert.True(orderMap.ContainsKey(item));
            }
            orderMap.Clear();
            Assert.Equal(0, orderMap.Count);
        }

        [Fact]
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
                Assert.True(orderMap.Remove(orderList[i]));
                Assert.False(orderMap.Contains(orderList[i]));
            }
            Assert.Equal(500, orderMap.Count);
            var notExist = new Order();
            notExist.Id = 1;
            notExist.Name = "not exist";
            Assert.False(orderMap.Remove(notExist));
        }

        [Fact]
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
                Assert.True(orderMap.Contains(maxOrder));
                var comparer = new OrderComparer();
                Assert.True(comparer.Compare(maxOrder, orderMap.Max.Key) == 0);
                orderMap.RemoveMax();
                Assert.False(orderMap.ContainsKey(maxOrder));
                Assert.True(comparer.Compare(maxOrder, orderMap.Max.Key) > 0);
            }

            Assert.Equal(150, orderMap.Count);
        }

        [Fact]
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
                Assert.True(orderMap.Contains(minOrder));
                var comparer = new OrderComparer();
                Assert.True(comparer.Compare(minOrder, orderMap.Min.Key) == 0);
                orderMap.RemoveMin();
                Assert.False(orderMap.ContainsKey(minOrder));
                Assert.True(comparer.Compare(minOrder, orderMap.Min.Key) < 0);
            }

            Assert.Equal(150, orderMap.Count);
        }

        [Fact]
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
                Assert.True(orderMap.Contains(kvps[i]));
            }
            for (var i = 0; i < 3; i++)
            {
                Assert.Null(kvps[i].Key);
                Assert.Null(kvps[i].Value);
            }
        }

        [Fact]
        public void TestMapEnumerator()
        {
            Random r = new Random((int)(DateTime.Now.Ticks & 0x0000FFFF));
            TreeMap<Order, Bill> orderMap = new TreeMap<Order, Bill>(new OrderComparer());
            List<KeyValuePair<Order, Bill>> orderList = new List<KeyValuePair<Order, Bill>>();
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
                    var os = new Order();
                    os.Id = o.Id;
                    os.Name = o.Name;
                    var bs = new Bill();
                    bs.Id = bill.Id;
                    bs.Count = bill.Count;
                    orderList.Add(new KeyValuePair<Order, Bill>(os, bs));
                    orderMap.Add(o, bill);
                    i++;
                }
            }

            var total = 0;
            foreach (var kvp in orderMap)
            {
                total++;
                Assert.True(orderList.Contains(kvp, new OrderBillEqualityComparer()));
            }
            Assert.Equal(1000, total);
        }

        [Fact]
        public void TestMapIndexer()
        {
            TreeMap<Order, Bill> orderMap = null;
            Dictionary<Order, Bill> orderDict = null;
            InitializeMap(out orderMap, out orderDict);
            foreach (var key in orderDict.Keys)
            {
                Assert.True(orderMap.ContainsKey(key));
                Assert.Equal(orderDict[key].Id, orderMap[key].Id);
                Assert.Equal(orderDict[key].Count, orderMap[key].Count);
            }
        }

        [Fact]
        public void TestMapKeys()
        {
            TreeMap<Order, Bill> orderMap = null;
            Dictionary<Order, Bill> orderDict = null;
            InitializeMap(out orderMap, out orderDict);

            foreach (var key in orderMap.Keys)
            {
                Assert.True(orderDict.ContainsKey(key));
                Assert.Equal(orderDict[key].Id, orderMap[key].Id);
                Assert.Equal(orderDict[key].Count, orderMap[key].Count);
            }
        }

        [Fact]
        public void TestMapKeySet()
        {
            TreeMap<Order, Bill> orderMap = null;
            Dictionary<Order, Bill> orderDict = null;
            InitializeMap(out orderMap, out orderDict);

            var set = orderMap.KeySet;
            foreach (var item in set)
            {
                Assert.True(orderDict.ContainsKey(item));
            }
        }

        [Fact]
        public void TestMapValues()
        {
            TreeMap<Order, Bill> orderMap = null;
            Dictionary<Order, Bill> orderDict = null;
            InitializeMap(out orderMap, out orderDict);

            var values = orderDict.Values;
            foreach (var v in orderMap.Values)
            {
                values.Contains(v, new BillEqualityComparer());
            }
        }

        [Fact]
        public void TestMapTryGetValue()
        {
            TreeMap<Order, Bill> orderMap = null;
            Dictionary<Order, Bill> orderDict = null;
            InitializeMap(out orderMap, out orderDict);
            foreach (var key in orderDict.Keys)
            {
                Bill bill = null;
                Assert.True(orderMap.TryGetValue(key, out bill));
                Assert.Equal(orderDict[key].Id, bill.Id);
                Assert.Equal(orderDict[key].Count, bill.Count);
            }

            var notExist = new Order();

            Bill b = null;
            Assert.False(orderMap.TryGetValue(notExist, out b));
            Assert.Null(b);
        }

        [Fact]
        public void TestMapAddRemoveKvp()
        {
            TreeMap<Order, Bill> orderMap = null;
            Dictionary<Order, Bill> orderDict = null;
            InitializeMap(out orderMap, out orderDict);
            TreeMap<Order, Bill> newMap = new TreeMap<Order, Bill>(new OrderComparer());
            foreach (var kvp in orderMap)
            {
                newMap.Add(kvp);
            }
            Assert.Equal(orderMap.Count, newMap.Count);

            var count = 0;
            foreach (var kvp in orderMap)
            {
                if (count++ < 500)
                {
                    Assert.True(newMap.Remove(kvp));
                }
            }

            Assert.Equal(500, newMap.Count);
            var kvps = new KeyValuePair<Order, Bill>[500];
            newMap.CopyTo(kvps, 0);
            var item = new KeyValuePair<Order, Bill>(kvps[250].Key, new Bill());
            Assert.True(newMap.ContainsKey(item.Key));
            Assert.False(newMap.Remove(item));
        }

        [Fact]
        public void TestMapNoItem()
        {
            TreeMap<int, string> map = new TreeMap<int, string>();
            Assert.Equal(0, map.Count);
            Assert.Empty(map);
            var count = 0;
            foreach (var kvp in map)
            {
                count++;
            }
            Assert.Equal(0, count);
            Assert.False(map.ContainsKey(0));
            Assert.False(map.Remove(0));
        }

        private void InitializeMap(out TreeMap<Order, Bill> map, out Dictionary<Order, Bill> dict)
        {
            Random r = new Random((int)(DateTime.Now.Ticks & 0x0000FFFF));
            TreeMap<Order, Bill> orderMap = new TreeMap<Order, Bill>(new OrderComparer());
            Dictionary<Order, Bill> orderDict = new Dictionary<Order, Bill>(new OrderEqualityComparer());
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
                    var os = new Order();
                    os.Id = o.Id;
                    os.Name = o.Name;
                    var bs = new Bill();
                    bs.Id = bill.Id;
                    bs.Count = bill.Count;
                    orderDict.Add(os, bs);
                    orderMap.Add(o, bill);
                    i++;
                }
            }

            map = orderMap;
            dict = orderDict;
        }
        
        private class OrderBillEqualityComparer : IEqualityComparer<KeyValuePair<Order, Bill>>
        {

            public bool Equals(KeyValuePair<Order, Bill> x, KeyValuePair<Order, Bill> y)
            {
                return (x.Key.Id == y.Key.Id) && (x.Key.Name == y.Key.Name) 
                    && (x.Value.Id == y.Value.Id) && (x.Value.Count == y.Value.Count);
            }

            public int GetHashCode(KeyValuePair<Order, Bill> obj)
            {
                return 0;
            }
        }

        private class BillEqualityComparer : IEqualityComparer<Bill>
        {
            public bool Equals(Bill x, Bill y)
            {
                return (x.Id == y.Id) && (x.Count == y.Count);
            }

            public int GetHashCode(Bill obj)
            {
                return 0;
            }
        }
    }
} 