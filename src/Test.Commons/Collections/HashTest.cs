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
using System.Text;
using System.Threading.Tasks;
using Commons.Utils;
using Xunit;

using Commons.Collections.Map;
using Commons.Collections.Set;

namespace Test.Commons.Collections
{
    public class HashTest
    {
        [Fact]
        public void TestMurmurHash32()
        {
            for (var i = 0; i < 1000; i++)
            {
                MurmurHash32 hasher = new MurmurHash32();
                string guid = Guid.NewGuid().ToString();
                string guid2 = guid;
                var bytes1 = new byte[guid.Length * sizeof(char)];
                Buffer.BlockCopy(guid.ToCharArray(), 0, bytes1, 0, bytes1.Length);
                var bytes2 = new byte[guid2.Length * sizeof(char)];
                Buffer.BlockCopy(guid.ToCharArray(), 0, bytes2, 0, bytes2.Length);
                Assert.Equal(hasher.Hash(bytes1)[0], hasher.Hash(bytes2)[0]);
            }
        }

        [Fact]
        public void TestHashAbility()
        {
            var orders = new Customized32HashMap<string, Order>(4);
			orders.HashAbility();
        }

        [Fact]
        public void TestHashMapContains()
        {
            var orders = new Customized32HashMap<string, Order>();
            var key = Guid.NewGuid().ToString();
            var order = new Order();
            orders.Add(key, order);
            Assert.True(orders.ContainsKey(key));
        }

        [Fact]
        public void TestHashMapConstructors()
        {
            var orders = new Customized32HashMap<string, Order>();
			orders.HashAbility();

            var orders2 = new Customized32HashMap<string, Order>(1000);
			orders2.HashAbility();

            var orders3 = new Customized32HashMap<string, Order>(100, x => x.ToBytes());
			orders3.HashAbility();

            var orders4 = new Customized32HashMap<string, Order>(500, x => x.ToBytes(), (x1, x2) => x1 == x2);
			orders4.HashAbility();

            var orders7 = new Customized32HashMap<string, Order>(1000, new MurmurHash32(), x => x.ToBytes(), (x1, x2) => x1 == x2);
			orders7.HashAbility();


            var orders9 = new Customized32HashMap<string, Order>(10000, orders7, new MurmurHash32(), x => x.ToBytes(), (x1, x2) => x1 == x2);
            Assert.NotEmpty(orders9);
            Assert.Equal(orders7.Count, orders9.Count);
        }

        [Fact]
        public void TestHashMapOperations()
        {
            var orders = new Order[1000];
            for (var i = 0; i < orders.Length; i++)
            {
                orders[i] = new Order() { Id = i, Name = Guid.NewGuid().ToString() };
            }

            var bills = new Bill[1000];
            for (var i = 0; i < bills.Length; i++)
            {
                bills[i] = new Bill() { Id = i, Count = i };
            }

            IDictionary<Order, Bill> orderMap = new Customized32HashMap<Order, Bill>(1000, x => x.Name.ToBytes());
            Assert.Equal(0, orderMap.Keys.Count);
            Assert.Equal(0, orderMap.Values.Count);
            for (var i = 0; i < orders.Length; i++)
            {
                orderMap.Add(orders[i], bills[i]);
            }
            Assert.Equal(orders.Length, orderMap.Count);
            var keys = orderMap.Keys;
            Assert.Equal(orders.Length, keys.Count);
            foreach (var key in keys)
            {
                Assert.True(orders.Contains(key, new OrderEqualityComparer()));
            }
            var values = orderMap.Values;
            Assert.Equal(bills.Length, values.Count);
            foreach (var v in values)
            {
                Assert.True(bills.Contains(v, new BillEqualityComparer()));
            }

            var order = new Order() { Id = 564, Name = "000000000000" };
            Bill b;
            Assert.False(orderMap.TryGetValue(order, out b));
            Assert.Equal(null, b);

            for (var i = 0; i < 150; i++)
            {
                Bill bill;
                Assert.Contains(new KeyValuePair<Order, Bill>(orders[200 + i], bills[200 + i]), orderMap);
                Assert.True(orderMap.Contains(new KeyValuePair<Order, Bill>(orders[100 + i], bills[100 + i])));
                Assert.True(orderMap.TryGetValue(orders[500], out bill));
                Assert.Equal(orderMap[orders[500]], bill, new BillEqualityComparer());
            }

            foreach (var item in orderMap)
            {
                Assert.Contains(item.Key, orders, new OrderEqualityComparer());
                Assert.Contains(item.Value, bills, new BillEqualityComparer());
            }

            var kvps1 = new KeyValuePair<Order, Bill>[1010];
            orderMap.CopyTo(kvps1, 5);
            for (var i = 0; i < 1000; i++)
            {
                Assert.Contains(kvps1[i + 5].Key, orders, new OrderEqualityComparer());
                Assert.Contains(kvps1[i + 5].Value, bills, new BillEqualityComparer());
            }

            orderMap.Clear();
            Assert.Equal(0, orderMap.Count);
            Assert.Empty(orderMap);
            foreach (var item in orderMap)
            {
                Assert.Equal(0, 1);
            }
        }

        [Fact]
        public void TestMapNullExceptions()
        {
            var map = new Customized32HashMap<Order, Bill>();
            Bill b;
            Assert.Throws(typeof(ArgumentNullException), () => map.Add(null, null));
            Assert.Throws(typeof(ArgumentNullException), () => map[null] = null);
            Assert.Throws(typeof(ArgumentNullException), () => { var v = map[null]; });
            Assert.Throws(typeof(ArgumentNullException), () => map.Remove(null));
            Assert.Throws(typeof(ArgumentNullException), () => map.TryGetValue(null, out b));
            Assert.Throws(typeof(ArgumentNullException), () => map.ContainsKey(null));
            Assert.Throws(typeof(ArgumentNullException), () => map.CopyTo(null, 0));

            var simpleMap = new Customized32HashMap<int, string>();
            Assert.DoesNotThrow(() => simpleMap.Add(0, null));
        }

        [Fact]
        public void TestMapArgumentExceptions()
        {
            var map = new Customized32HashMap<Order, Bill>(5, new MurmurHash32(), x => x.Id.ToString().ToBytes(), (x1, x2) => x1.Id == x2.Id);
            for (var i = 0; i < 5; i++)
            {
                var order = new Order() { Id = i, Name = Guid.NewGuid().ToString() };
                map.Add(order, new Bill() { Id = i, Count = i });
            }

            var notExistingOrder = new Order() { Id = 10, Name = Guid.NewGuid().ToString() };
            Assert.Throws(typeof(KeyNotFoundException), () => { var v = map[notExistingOrder]; });
            Assert.Throws(typeof(KeyNotFoundException), () => map[notExistingOrder] = new Bill());
            Assert.False(map.Remove(notExistingOrder));

            var existingOrder = new Order() { Id = 1, Name = "  " };
            Assert.Throws(typeof(ArgumentException), () => map.Add(existingOrder, new Bill()));
        }

        [Fact]
        public void TestPlainHashMapOperations()
        {
            var map = new HashedMap<string, Order>();
			map.HashAbility();
        }

        [Fact]
        public void TestPlainHashMapBoundaries()
        {
            var map = new HashedMap<string, string>(100);
            Assert.False(map.Remove("a"));
			Assert.Throws(typeof(ArgumentException), () => new HashedMap<string, string>(0));
			Assert.Throws(typeof(ArgumentException), () => new HashedMap<string, string>(-10));
        }

		[Fact]
		public void TestHashedSetConstructors()
		{
			var set1 = new HashedSet<string>();
			set1.TestHashedSetOperations();

			var set2 = new HashedSet<string>(10000);
			set2.TestHashedSetOperations();

			var set3 = new HashedSet<string>((x1, x2) => x1 == x2);
			set3.TestHashedSetOperations();

			var set4 = new HashedSet<string>(EqualityComparer<string>.Default);
			set4.TestHashedSetOperations();

			var set5 = new HashedSet<string>(10000, EqualityComparer<string>.Default.Equals);
			set5.TestHashedSetOperations();

			var set6 = new HashedSet<string>(10000, EqualityComparer<string>.Default);
			set6.TestHashedSetOperations();

			var items = new List<string>();
			for (var i = 0; i < 10000; i++)
			{
				items.Add(Guid.NewGuid().ToString());
			}
			var set7 = new HashedSet<string>(items, 10000, EqualityComparer<string>.Default.Equals);
			var count = 0;
			foreach (var i in set7)
			{
				count++;
			}

			Assert.Equal(count, set7.Count);
		}

		[Fact]
		public void TestHashedSetOperations()
		{
            var orders = new Order[1000];
            for (var i = 0; i < orders.Length; i++)
            {
                orders[i] = new Order() { Id = i, Name = Guid.NewGuid().ToString() };
            }

			var orderSet = new HashedSet<Order>(1000, (x1, x2) => x1.Id == x2.Id);
			Assert.Equal(0, orderSet.Count);
            for (var i = 0; i < orders.Length; i++)
            {
				orderSet.Add(orders[i]);
            }
            Assert.Equal(orders.Length, orderSet.Count);
            foreach (var key in orderSet)
            {
                Assert.True(orders.Contains(key, new OrderEqualityComparer()));
            }

            for (var i = 0; i < 150; i++)
            {
				Assert.True(orderSet.Contains(orders[200 + i]));
            }

            foreach (var item in orderSet)
            {
                Assert.Contains(item, orders, new OrderEqualityComparer());
            }

            orderSet.Clear();
            Assert.Equal(0, orderSet.Count);
            Assert.Empty(orderSet);
            foreach (var item in orderSet)
            {
                Assert.Equal(0, 1);
            }
        
		}

		[Fact]
		public void TestHashedSetBoundaries()
		{
			var set = new HashedSet<string>();
			Assert.False(set.Remove("s"));

			Assert.Throws(typeof(ArgumentException), () => new HashedSet<string>(0));
			Assert.Throws(typeof(ArgumentException), () => new HashedSet<string>(-1));
		}

		[Fact]
		public void TestLinkedHashMapConstructors()
		{
			var orders = new LinkedHashMap<string, Order>();
			orders.HashAbility();

			var orders2 = new LinkedHashMap<string, Order>(10000);
			orders2.HashAbility();

			var orders4 = new LinkedHashMap<string, Order>(500, (x1, x2) => x1 == x2);
			orders4.HashAbility();

			var orders5 = new LinkedHashMap<string, Order>(1000, orders2);
			Assert.NotEmpty(orders5);
			Assert.Equal(orders2.Count, orders5.Count);

			var orders9 = new LinkedHashMap<string, Order>(1000, orders2, (x1, x2) => x1 == x2);
			Assert.NotEmpty(orders9);
			Assert.Equal(orders2.Count, orders9.Count);
		}

		[Fact]
		public void TestOrderedMap()
		{
			var container = new LinkedHashMap<int, string>(10000);
			Assert.Throws(typeof(InvalidOperationException), () => container.First);
			Assert.Throws(typeof(InvalidOperationException), () => container.Last);
			Assert.Throws(typeof(InvalidOperationException), () => container.After(1));
			Assert.Throws(typeof(InvalidOperationException), () => container.Before(1));
			for (var i = 0; i < 10000; i++)
			{
				container.Add(i, i.ToString());
			}
			Assert.Equal(0, container.First.Key);
			Assert.Equal("0", container.First.Value);
			Assert.Equal(9999, container.Last.Key);
			Assert.Equal("9999", container.Last.Value);
			Assert.Equal("0", container.GetIndex(0).Value);
			Assert.Equal("1000", container.GetIndex(1000).Value);
			Assert.Equal("1600", container.GetIndex(1600).Value);
			Assert.Equal("2749", container.GetIndex(2749).Value);
			Assert.Equal("4999", container.GetIndex(4999).Value);
			Assert.Equal("11", container.After(10).Value);
			Assert.Equal("100", container.After(99).Value);
			Assert.Equal("1001", container.After(1000).Value);
			Assert.Equal("5000", container.After(4999).Value);
			Assert.Equal("50", container.Before(51).Value);
			Assert.Equal("500", container.Before(501).Value);
			Assert.Equal("1500", container.Before(1501).Value);
			Assert.Equal("7900", container.Before(7901).Value);
			Assert.Throws(typeof(ArgumentException), () => container.After(9999));
			Assert.Throws(typeof(ArgumentException), () => container.Before(0));
			Assert.Throws(typeof(ArgumentException), () => container.Before(10000));
			Assert.Throws(typeof(ArgumentException), () => container.After(10000));
            Assert.Throws(typeof(ArgumentException), () => container.GetIndex(-100));
            Assert.Throws(typeof(ArgumentException), () => container.GetIndex(20000));

			for (var i = 2000; i < 5000; i++)
			{
				Assert.True(container.Remove(i));
			}
			Assert.Equal("0", container.First.Value);
			Assert.Equal("9999", container.Last.Value);
			Assert.Equal("1000", container.After(999).Value);
			Assert.Equal("999", container.Before(1000).Value);
			Assert.Equal("9000", container.After(8999).Value);
			Assert.Equal("8999", container.Before(9000).Value);
			Assert.Equal("8500", container.Before(8501).Value);
			Assert.Equal("5000", container.After(1999).Value);
			Assert.Equal("1999", container.Before(5000).Value);
			
			for (var i = 7000; i < 8000; i++)
			{
				Assert.True(container.Remove(i));
			}
			Assert.Equal("0", container.First.Value);
			Assert.Equal("9999", container.Last.Value);
			Assert.Equal("1000", container.After(999).Value);
			Assert.Equal("999", container.Before(1000).Value);
			Assert.Equal("9000", container.After(8999).Value);
			Assert.Equal("8999", container.Before(9000).Value);
			Assert.Equal("8500", container.Before(8501).Value);
			Assert.Equal("8000", container.After(6999).Value);
			Assert.Equal("6999", container.Before(8000).Value);
		}

        [Fact]
        public void TestOneItemLinkedHashMap()
        {
            var orders = new LinkedHashMap<int, string>();
            orders.Add(1, "1");
            Assert.Equal(1, orders.First.Key);
            Assert.Equal(1, orders.Last.Key);
            Assert.Throws(typeof(ArgumentException), () => orders.Before(1));
            Assert.Throws(typeof(ArgumentException), () => orders.After(1));
        }

        [Fact]
        public void TestEmptyLinkedHashMap()
        {
            var orders = new LinkedHashMap<string, Order>();
            Assert.Throws(typeof(InvalidOperationException), () => orders.First);
            Assert.Throws(typeof(InvalidOperationException), () => orders.Last);
            Assert.Throws(typeof(InvalidOperationException), () => orders.After("1"));
            Assert.Throws(typeof(InvalidOperationException), () => orders.Before("2"));
            Assert.Throws(typeof(ArgumentException), () => orders.GetIndex(0));
            Assert.Throws(typeof(ArgumentException), () => orders.GetIndex(1));
        }

        [Fact]
        public void TestReferenceMapConstructor()
        {
            var map1 = new ReferenceMap<string, Order>();
            map1.HashAbility();

            var map2 = new ReferenceMap<string, Order>(10000);
            map2.HashAbility();
        }

        [Fact]
        public void TestReferenceMap()
        {
            var orderList = new List<Order>();
            for (var i = 0; i < 10000; i++)
            {
                orderList.Add(new Order { Id = i, Name = i.ToString() });
            }
            var map = new ReferenceMap<Order, Bill>();
            foreach (var order in orderList)
            {
                map.Add(order, new Bill { Id = order.Id, Count = order.Id });
            }

            foreach (var order in orderList)
            {
                Assert.True(map.ContainsKey(order));
            }

            var newOrderList = new List<Order>();
            for (var i = 0; i < 10000; i++)
            {
                orderList.Add(new Order { Id = i, Name = i.ToString() });
            }

            foreach (var order in newOrderList)
            {
                Assert.False(map.ContainsKey(order));
            }
        }

		[Fact]
		public void TestLruMapConstructor()
		{
			var lru = new LruMap<int, int>();
			for (var i = 0; i < 100; i++)
			{
				Assert.False(lru.IsFull);
				lru.Add(i, i);
			}
			Assert.Equal(100, lru.Count);
			Assert.True(lru.IsFull);
			for (var i = 100; i < 200; i++)
			{
				lru.Add(i, i);
			}
			Assert.Equal(100, lru.Count);
			Assert.True(lru.IsFull);
			for (var i = 0; i < 100; i++)
			{
				Assert.False(lru.ContainsKey(i));
			}
			for (var i = 100; i < 200; i++)
			{
				Assert.True(lru.ContainsKey(i));
			}

			var lru2 = new LruMap<int, int>(10000);
			for (var i = 0; i < 10000; i++)
			{
				Assert.False(lru2.IsFull);
				lru2.Add(i, i);
			}
			Assert.Equal(10000, lru2.Count);
			Assert.True(lru2.IsFull);
			for (var i = 10000; i < 20000; i++)
			{
				lru2.Add(i, i);
			}
			Assert.Equal(10000, lru2.Count);
			Assert.True(lru2.IsFull);
			for (var i = 0 ; i < 10000; i++)
			{
				Assert.False(lru2.ContainsKey(i));
			}
			for (var i = 10000; i < 20000; i++)
			{
				Assert.True(lru2.ContainsKey(i));
			}

			var lru3 = new LruMap<Order, Bill>(10000, (x1, x2) => x1.Id == x2.Id);
			for (var i = 0; i < 10000; i++)
			{
				Assert.False(lru3.IsFull);
				lru3.Add(new Order { Id = i }, new Bill());
			}
			Assert.Equal(10000, lru3.Count);
			Assert.True(lru3.IsFull);
			for (var i = 10000; i < 20000; i++)
			{
				lru3.Add(new Order { Id = i }, new Bill());
			}
			Assert.Equal(10000, lru3.Count);
			Assert.True(lru3.IsFull);
			for (var i = 0; i < 10000; i++)
			{
				Assert.False(lru3.ContainsKey(new Order { Id = i }));
			}
			for (var i = 10000; i < 20000; i++)
			{
				Assert.True(lru3.ContainsKey(new Order { Id = i }));
			}
		}

		[Fact]
		public void TestLruMapEliminate()
		{
			var lru = new LruMap<int, int>(10000);
			for (var i = 0; i < 10000; i++)
			{
				lru.Add(i, i);
			}
			Assert.Equal(0, lru[0]);
			lru.Add(10000, 10000);
			Assert.True(lru.ContainsKey(0));
			Assert.False(lru.ContainsKey(1));
			lru[2] = 3;
			lru.Add(10001, 10001);
			Assert.False(lru.ContainsKey(3));
			Assert.Equal(3, lru[2]);
			Assert.Equal(10000, lru.Count);
			Assert.True(lru.ContainsKey(0));
			Assert.True(lru.ContainsKey(4));
			Assert.Throws(typeof(ArgumentException), () => lru[1]);
			Assert.Throws(typeof(ArgumentException), () => lru[3]);
		}

		[Fact]
		public void TestLruMapHit()
		{
			var lru = new LruMap<int, int>(10000);
			var map = new HashedMap<int, int>(10000);
			for (var i = 0; i < 10000; i++)
			{
				lru.Add(i, i);
				map.Add(i, i);
			}
			foreach (var item in lru)
			{
				Assert.True(map.ContainsKey(item.Key));
			}

			for (var i = 0; i < 10000; i++)
			{
				lru[10000 - 1 - i] = 10000 - 1 - i;
			}

			foreach (var item in lru)
			{
				Assert.True(map.ContainsKey(item.Key));
			}

			for (var i = 0; i < 5000; i++)
			{
				lru.Add(i + 10000, i + 10000);
			}

			for (var i = 0; i < 5000; i++)
			{
				Assert.False(lru.ContainsKey(i + 5000));
			}
		}

		[Fact]
		public void TestLruMapAccessHeader()
		{
			var lru = new LruMap<int, int>(10000);
			for (var i = 0; i < 10000; i++)
			{
				lru.Add(i, i);
			}
			lru[9999] = 9999;
			for (var i = 0; i < 9999; i++)
			{
				lru.Add(i + 10000, i + 10000);
			}
			for (var i = 0; i < 9999; i++)
			{
				Assert.False(lru.ContainsKey(i));
			}
			for (var i = 0; i < 10000; i++)
			{
				Assert.True(lru.ContainsKey(i + 9999));
			}
		}
    }
}
