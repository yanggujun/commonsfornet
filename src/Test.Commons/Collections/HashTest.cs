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
using System.Diagnostics;

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
            var orders = new Customized32HashedMap<string, Order>(4);
            orders.HashAbility();
        }

        [Fact]
        public void TestHashMapContains()
        {
            var orders = new Customized32HashedMap<string, Order>();
            var key = Guid.NewGuid().ToString();
            var order = new Order();
            orders.Add(key, order);
            Assert.True(orders.ContainsKey(key));
        }

        [Fact]
        public void TestCustomizedHashMapConstructors()
        {
            var orders = new Customized32HashedMap<string, Order>();
            orders.HashAbility();

            var orders2 = new Customized32HashedMap<string, Order>(1000);
            orders2.HashAbility();

            var orders3 = new Customized32HashedMap<string, Order>(100, x => x.ToBytes());
            orders3.HashAbility();

            var orders4 = new Customized32HashedMap<string, Order>(500, x => x.ToBytes(), (x1, x2) => x1 == x2);
            orders4.HashAbility();

            var orders5 = new Customized32HashedMap<string, Order>(500, x => x.ToBytes(), EqualityComparer<string>.Default);
            orders5.HashAbility();

            var orders7 = new Customized32HashedMap<string, Order>(1000, new MurmurHash32(), x => x.ToBytes(), (x1, x2) => x1 == x2);
            orders7.HashAbility();


            var orders9 = new Customized32HashedMap<string, Order>(orders7, new MurmurHash32(), x => x.ToBytes(), (x1, x2) => x1 == x2);
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

            IDictionary<Order, Bill> orderMap = new Customized32HashedMap<Order, Bill>(1000, x => x.Name.ToBytes());
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
            foreach (var o in orders)
            {
                Assert.True(keys.Contains(o, new OrderEqualityComparer()));
            }

            var values = orderMap.Values;
            Assert.Equal(bills.Length, values.Count);
            foreach (var v in values)
            {
                Assert.True(bills.Contains(v, new BillEqualityComparer()));
            }

            foreach (var bill in bills)
            {
                Assert.True(values.Contains(bill, new BillEqualityComparer()));
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
            var map = new Customized32HashedMap<Order, Bill>();
            Bill b;
            Assert.Throws(typeof(ArgumentNullException), () => map.Add(null, null));
            Assert.Throws(typeof(ArgumentNullException), () => map[null] = null);
            Assert.Throws(typeof(ArgumentNullException), () => { var v = map[null]; });
            Assert.Throws(typeof(ArgumentNullException), () => map.Remove(null));
            Assert.Throws(typeof(ArgumentNullException), () => map.TryGetValue(null, out b));
            Assert.Throws(typeof(ArgumentNullException), () => map.ContainsKey(null));
            Assert.Throws(typeof(ArgumentNullException), () => map.CopyTo(null, 0));

            var simpleMap = new Customized32HashedMap<int, string>();
            Assert.DoesNotThrow(() => simpleMap.Add(0, null));
        }

        [Fact]
        public void TestMapArgumentExceptions()
        {
            var map = new Customized32HashedMap<Order, Bill>(5, new MurmurHash32(), x => x.Id.ToString().ToBytes(), (x1, x2) => x1.Id == x2.Id);
            for (var i = 0; i < 5; i++)
            {
                var order = new Order() { Id = i, Name = Guid.NewGuid().ToString() };
                map.Add(order, new Bill() { Id = i, Count = i });
            }

            var notExistingOrder = new Order() { Id = 10, Name = Guid.NewGuid().ToString() };
            Assert.Throws(typeof(KeyNotFoundException), () => { var v = map[notExistingOrder]; });
            Assert.False(map.Remove(notExistingOrder));

            var existingOrder = new Order() { Id = 1, Name = "  " };
            Assert.Throws(typeof(ArgumentException), () => map.Add(existingOrder, new Bill()));
        }

        [Fact]
        public void TestCustomizedHashedMapSetNotExistingValue()
        {
            var map = new Customized32HashedMap<Order, Bill>(1000, new MurmurHash32(), x => x.Id.ToString().ToBytes(), (x1, x2) => x1.Id == x2.Id);
            map.MapSetNotExistingValue();
        }

        [Fact]
        public void TestHashedMapSetNotExistingValue()
        {
            var map = new HashedMap<Order, Bill>(new OrderEqualityComparer());
            map.MapSetNotExistingValue();
        }

        [Fact]
        public void TestPlainHashMapOperations()
        {
            var map1 = new HashedMap<string, Order>();
            map1.HashAbility();

            var map2 = new HashedMap<string, Order>(1000);
            map2.HashAbility();

            var map3 = new HashedMap<string, Order>((x1, x2) => x1 == x2);
            map3.HashAbility();

            var map4 = new HashedMap<string, Order>(EqualityComparer<string>.Default);
            map4.HashAbility();

            var map5 = new HashedMap<string, Order>(1000, EqualityComparer<string>.Default.Equals);
            map5.HashAbility();

            var map6 = new HashedMap<string, Order>(1000, EqualityComparer<string>.Default);
            map6.HashAbility();

            var map7 = new HashedMap<string, Order>(map6, (x1, x2) => x1 == x2);
            Assert.Equal(map6.Count, map7.Count);
            foreach(var item in map6)
            {
                Assert.True(map7.Contains(item));
            }
            foreach(var item in map7)
            {
                Assert.True(map6.Contains(item));
            }

            var map8 = new HashedMap<string, Order>(map7, EqualityComparer<string>.Default);
            foreach(var item in map7)
            {
                Assert.True(map8.Contains(item));
            }
            foreach(var item in map8)
            {
                Assert.True(map7.Contains(item));
            }

            var map9 = new HashedMap<string, Order>(map8);
            foreach (var item in map8)
            {
                Assert.True(map8.Contains(item));
            }

            foreach (var item in map9)
            {
                Assert.True(map8.Contains(item));
            }
        }

		[Fact]
		public void TestHashedMapComplexOperations()
		{
			var map = new HashedMap<string, string>();
			var set = new HashSet<string>();
			var list = new List<string>();
			for (var i = 0; i < 100000; i++)
			{
				var key = Guid.NewGuid().ToString();
				map.Add(key, key);
				set.Add(key);
				list.Add(key);
			}

			Assert.Equal(100000, map.Count);
			foreach(var element in set)
			{
				Assert.True(map.ContainsKey(element));
				Assert.Equal(element, map[element]);
			}

			var count = 0;
			foreach(var element in map)
			{
				count++;
				Assert.True(set.Contains(element.Key));
				Assert.True(set.Contains(element.Value));
			}
			Assert.Equal(100000, count);

			for (var i = 0; i < 20000; i++)
			{
				var element = list[i + 10000];
				Assert.True(map.Remove(element));
				set.Remove(element);
			}
			Assert.Equal(80000, map.Count);

			for (var i = 0; i < 40000; i++)
			{
				var newKey = Guid.NewGuid().ToString();
				Assert.False(map.ContainsKey(newKey));
				map[newKey] = newKey;
				set.Add(newKey);
				list.Add(newKey);
			}
			Assert.Equal(120000, map.Count);
			Assert.Equal(set.Count, map.Count);

			count = 0;
			foreach(var element in map)
			{
				count++;
				Assert.True(set.Contains(element.Key));
				Assert.True(set.Contains(element.Value));
			}
			Assert.Equal(120000, count);

			foreach(var element in set)
			{
				Assert.True(map.ContainsKey(element));
				Assert.Equal(element, map[element]);
			}
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
            var orders = new LinkedHashedMap<string, Order>();
            orders.HashAbility();

            var orders2 = new LinkedHashedMap<string, Order>(10000);
            orders2.HashAbility();

            var orders3 = new LinkedHashedMap<string, Order>(EqualityComparer<string>.Default);
            orders3.HashAbility();

            var orders6 = new LinkedHashedMap<string, Order>((x1, x2) => x1 == x2);
            orders6.HashAbility();

            var orders7 = new LinkedHashedMap<string, Order>(100, EqualityComparer<string>.Default);
            orders7.HashAbility();

            var orders4 = new LinkedHashedMap<string, Order>(500, (x1, x2) => x1 == x2);
            orders4.HashAbility();

            //var orders5 = new LinkedHashedMap<string, Order>(orders2, EqualityComparer<string>.Default);
            //Assert.NotEmpty(orders5);
            //Assert.Equal(orders2.Count, orders5.Count);
            //orders5.Clear();
            //Assert.Equal(0, orders5.Count);

            //var orders9 = new LinkedHashedMap<string, Order>(orders2, (x1, x2) => x1 == x2);
            //Assert.NotEmpty(orders9);
            //Assert.Equal(orders2.Count, orders9.Count);
            //orders9.Clear();
            //Assert.Equal(0, orders9.Count);
        }

        [Fact]
        public void TestLinkedHashedMapRemove()
        {
            var map = new LinkedHashedMap<int, Order>();
            map.Fill(x => new KeyValuePair<int, Order>(x, new Order { Id = x }));
            Assert.Equal(1000, map.Count);

            for (var i = 0; i < 100; i++)
            {
                Assert.True(map.ContainsKey(i));
                Assert.True(map.Remove(i));
                Assert.False(map.ContainsKey(i));
            }
            Assert.Throws(typeof(ArgumentException), () => map.After(0));
            Assert.Throws(typeof(ArgumentException), () => map.Before(100));
            Assert.Throws(typeof(ArgumentException), () => map.After(999));

            Assert.Equal(900, map.Count);

            var index = 100;
            foreach (var item in map)
            {
                Assert.Equal(index, item.Key);
                index++;
            }
            Assert.Equal(1000, index);

            for (var i = 900; i < 1000; i++)
            {
                Assert.True(map.ContainsKey(i));
                Assert.True(map.Remove(i));
                Assert.False(map.ContainsKey(i));
            }
            Assert.Equal(800, map.Count);
            Assert.Throws(typeof(ArgumentException), () => map.After(899));

            index = 100;
            foreach (var item in map)
            {
                Assert.Equal(index, item.Key);
                index++;
            }
            Assert.Equal(900, index);

            for (var i = 300; i < 500; i++)
            {
                Assert.True(map.ContainsKey(i));
                Assert.True(map.Remove(i));
            }
            Assert.Equal(299, map.Before(500).Key);
            Assert.Equal(500, map.After(299).Key);

            Assert.Equal(600, map.Count);
            index = 100;
            foreach (var item in map)
            {
                index = index == 300 ? 500 : index;
                Assert.Equal(index, item.Key);
                index++;
            }

            Assert.Equal(900, index);
        }

        [Fact]
        public void TestOrderedMap()
        {
            var container = new LinkedHashedMap<int, string>(10000);
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
            var orders = new LinkedHashedMap<int, string>();
            orders.Add(1, "1");
            Assert.Equal(1, orders.First.Key);
            Assert.Equal(1, orders.Last.Key);
            Assert.Throws(typeof(ArgumentException), () => orders.Before(1));
            Assert.Throws(typeof(ArgumentException), () => orders.After(1));
        }

        [Fact]
        public void TestEmptyLinkedHashMap()
        {
            var orders = new LinkedHashedMap<string, Order>();
            Assert.Throws(typeof(InvalidOperationException), () => orders.First);
            Assert.Throws(typeof(InvalidOperationException), () => orders.Last);
            Assert.Throws(typeof(InvalidOperationException), () => orders.After("1"));
            Assert.Throws(typeof(InvalidOperationException), () => orders.Before("2"));
            Assert.Throws(typeof(ArgumentException), () => orders.GetIndex(0));
            Assert.Throws(typeof(ArgumentException), () => orders.GetIndex(1));
        }

        [Fact]
        public void TestLinkedSetConstructors()
        {
            var set = new LinkedSet<string>();
            set.TestHashedSetOperations();

            var set2 = new LinkedSet<string>(10000);
            set2.TestHashedSetOperations();

            var set3 = new LinkedSet<string>((x1, x2) => x1 == x2);
            set3.TestHashedSetOperations();

            var set4 = new LinkedSet<string>(EqualityComparer<string>.Default);
            set4.TestHashedSetOperations();

            var set5 = new LinkedSet<string>(10000, EqualityComparer<string>.Default.Equals);
            set5.TestHashedSetOperations();

            var set6 = new LinkedSet<string>(10000, EqualityComparer<string>.Default);
            set6.TestHashedSetOperations();
        }

        [Fact]
        public void TestLinkedSet()
        {
            var container = new LinkedSet<int>(10000);
            Assert.Throws(typeof(InvalidOperationException), () => container.First);
            Assert.Throws(typeof(InvalidOperationException), () => container.Last);
            Assert.Throws(typeof(InvalidOperationException), () => container.After(1));
            Assert.Throws(typeof(InvalidOperationException), () => container.Before(1));
            container.Fill(x => x, 10000);
            Assert.Equal(0, container.First);
            Assert.Equal(9999, container.Last);
            Assert.Equal(0, container.GetIndex(0));
            Assert.Equal(1000, container.GetIndex(1000));
            Assert.Equal(1600, container.GetIndex(1600));
            Assert.Equal(2749, container.GetIndex(2749));
            Assert.Equal(4999, container.GetIndex(4999));
            Assert.Equal(11, container.After(10));
            Assert.Equal(100, container.After(99));
            Assert.Equal(1001, container.After(1000));
            Assert.Equal(5000, container.After(4999));
            Assert.Equal(50, container.Before(51));
            Assert.Equal(500, container.Before(501));
            Assert.Equal(1500, container.Before(1501));
            Assert.Equal(7900, container.Before(7901));
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
            Assert.Equal(0, container.First);
            Assert.Equal(9999, container.Last);
            Assert.Equal(1000, container.After(999));
            Assert.Equal(999, container.Before(1000));
            Assert.Equal(9000, container.After(8999));
            Assert.Equal(8999, container.Before(9000));
            Assert.Equal(8500, container.Before(8501));
            Assert.Equal(5000, container.After(1999));
            Assert.Equal(1999, container.Before(5000));
            
            for (var i = 7000; i < 8000; i++)
            {
                Assert.True(container.Remove(i));
            }
            Assert.Equal(0, container.First);
            Assert.Equal(9999, container.Last);
            Assert.Equal(1000, container.After(999));
            Assert.Equal(999, container.Before(1000));
            Assert.Equal(9000, container.After(8999));
            Assert.Equal(8999, container.Before(9000));
            Assert.Equal(8500, container.Before(8501));
            Assert.Equal(8000, container.After(6999));
            Assert.Equal(6999, container.Before(8000));
        }

        [Fact]
        public void TestOneItemLinkedSet()
        {
            var orders = new LinkedSet<int>();
            orders.Add(1);
            Assert.Equal(1, orders.First);
            Assert.Equal(1, orders.Last);
            Assert.Throws(typeof(ArgumentException), () => orders.Before(1));
            Assert.Throws(typeof(ArgumentException), () => orders.After(1));
        }

        [Fact]
        public void TestEmptyLinkedSet()
        {
            var orders = new LinkedSet<string>();
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
        public void TestReferenceSet()
        {
            var set = new ReferenceSet<Order>();
            set.Fill(x => new Order { Id = x }, 10000);
            Assert.Equal(10000, set.Count);

            var list = new List<Order>();
            list.Fill(x => new Order { Id = x }, 10000);

            foreach (var item in list)
            {
                Assert.False(set.Contains(item));
            }

            for (var i = 1000; i < 3000; i++)
            {
                Assert.False(set.Remove(new Order { Id = i }));
            }
            set.Clear();
            Assert.Equal(0, set.Count);

            var set2 = new ReferenceSet<Order>(10000);
            var list2 = new List<Order>();
            for (var i = 0; i < 10000; i++)
            {
                var order = new Order { Id = i };
                set2.Add(order);
                list2.Add(order);
            }

            for (var i = 0; i < 10000; i++)
            {
                Assert.True(set2.Contains(list2[i]));
            }

            foreach (var item in set2)
            {
                Assert.True(list2.Contains(item));
            }

            for (var i = 1000; i < 3000; i++)
            {
                Assert.True(set2.Remove(list2[i]));
            }

            Assert.Equal(8000, set2.Count);
        }

        [Fact]
        public void TestLruSetConstructor()
        {
            var lru = new LruSet<int>();
            Assert.Equal(100, lru.MaxSize);
            lru.Fill(x => x, 100);
            Assert.Equal(100, lru.Count);
            Assert.True(lru.IsFull);
            for (var i = 100; i < 200; i++)
            {
                lru.Add(i);
            }
            Assert.Equal(100, lru.Count);
            Assert.True(lru.IsFull);
            for (var i = 0; i < 100; i++)
            {
                Assert.False(lru.Contains(i));
            }
            for (var i = 100; i < 200; i++)
            {
                Assert.True(lru.Contains(i));
            }
            Assert.Throws(typeof(ArgumentException), () => lru.Add(100));
            Assert.Throws(typeof(ArgumentException), () => lru.Add(101));

            var lru2 = new LruSet<int>(1000);
            Assert.Equal(1000, lru2.MaxSize);
            lru2.Fill(x => x);
            Assert.Equal(1000, lru2.Count);
            lru2.Fill(x => x + 1000, 100);
            for (var i = 0; i < 100; i++)
            {
                Assert.False(lru2.Contains(i));
            }
            for (var i = 100; i < 1100; i++)
            {
                Assert.True(lru2.Contains(i));
            }

            var lru3 = new LruSet<Order>(10000, new OrderEqualityComparer());
            LruSetConstructor(lru3);

            var lru4 = new LruSet<Order>(10000, (x1, x2) => x1.Id == x2.Id);
            LruSetConstructor(lru4);
        }

        [Fact]
        public void TestLruSetEliminate()
        {
            var lru = new LruSet<int>(10000);
            lru.Fill(x => x, 10000);
            lru.Add(10000);
            Assert.False(lru.Contains(0));
            for (var i = 0; i < 10000; i++)
            {
                Assert.True(lru.Contains(i + 1));
            }
            Assert.True(lru.IsFull);
        }

        [Fact]
        public void TestLruSetRemove()
        {
            var lru = new LruSet<int>(10000);
            lru.Fill(x => x, 10000);
            lru.Fill(x => x + 10000, 5000);
            Assert.True(lru.IsFull);
            Assert.Equal(lru.MaxSize, lru.Count);

            for (var i = 0; i < 5000; i++)
            {
                Assert.True(lru.Remove(i + 5000));
            }

            for (var i = 0; i < 5000; i++)
            {
                Assert.False(lru.Contains(i));
            }

            Assert.Equal(5000, lru.Count);

            for (var i = 0; i < 5000; i++)
            {
                Assert.False(lru.Remove(i));
            }
        }

        [Fact]
        public void TestLruSetBoundary()
        {
            var lru = new LruSet<int>();
            Assert.False(lru.IsFull);

            lru.Add(0);
            Assert.False(lru.IsFull);
            Assert.Equal(1, lru.Count);

            Assert.True(lru.Remove(0));
            Assert.Equal(0, lru.Count);
            Assert.False(lru.IsFull);

            lru.Fill(x => x, 100);
            Assert.Equal(100, lru.Count);
            Assert.True(lru.IsFull);

            for (var i = 0; i < 100; i++)
            {
                Assert.True(lru.Remove(i));
                Assert.False(lru.IsFull);
            }

            Assert.Equal(0, lru.Count);
            lru.Add(0);
            Assert.Equal(1, lru.Count);
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
            LruConstructor(lru3);

            var lru4 = new LruMap<Order, Bill>(10000, new OrderEqualityComparer());
            LruConstructor(lru4);
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

        private void LruSetConstructor(LruSet<Order> lru)
        {
            Assert.False(lru.IsFull);
            lru.Fill(x => new Order { Id = x }, 10000);
            Assert.Equal(10000, lru.Count);
            Assert.True(lru.IsFull);
            lru.Fill(x => new Order { Id = x + 10000 }, 10000);
            Assert.Equal(10000, lru.Count);
            Assert.True(lru.IsFull);
            for (var i = 0; i < 10000; i++)
            {
                Assert.False(lru.Contains(new Order { Id = i }));
            }
            for (var i = 10000; i < 20000; i++)
            {
                Assert.True(lru.Contains(new Order { Id = i }));
            }
        }

        private void LruConstructor(LruMap<Order, Bill> lru)
        {
            for (var i = 0; i < 10000; i++)
            {
                Assert.False(lru.IsFull);
                lru.Add(new Order { Id = i }, new Bill());
            }
            Assert.Equal(10000, lru.Count);
            Assert.True(lru.IsFull);
            for (var i = 10000; i < 20000; i++)
            {
                lru.Add(new Order { Id = i }, new Bill());
            }
            Assert.Equal(10000, lru.Count);
            Assert.True(lru.IsFull);
            for (var i = 0; i < 10000; i++)
            {
                Assert.False(lru.ContainsKey(new Order { Id = i }));
            }
            for (var i = 10000; i < 20000; i++)
            {
                Assert.True(lru.ContainsKey(new Order { Id = i }));
            }
        }
    }
}
