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
using Commons.Collections.Map;
using Xunit;
namespace Test.Commons.Collections
{
	public class BimapTest
	{
		[Fact]
		public void TestHashBimapContructor()
		{
			var hashBimap1 = new HashBimap<Order, Bill>((x1, x2) => x1.Id == x2.Id, (y1, y2) => y1.Id == y2.Id);
			BimapOperation(hashBimap1, x => new Order { Id = x }, y => new Bill { Id = y }, new OrderEqualityComparer(), new BillEqualityComparer());

			var hashBimap2 = new HashBimap<Order, Bill>(new OrderEqualityComparer(), new BillEqualityComparer());
			BimapOperation(hashBimap2, x => new Order { Id = x }, y => new Bill { Id = y }, new OrderEqualityComparer(), new BillEqualityComparer());

			var hashBimap3 = new HashBimap<Order, Bill>(10000, new OrderEqualityComparer(), new BillEqualityComparer());
			BimapOperation(hashBimap3, x => new Order { Id = x }, y => new Bill { Id = y }, new OrderEqualityComparer(), new BillEqualityComparer());

			var hashBimap4 = new HashBimap<Order, Bill>(10000, (x1, x2) => x1.Id == x2.Id, (y1, y2) => y1.Id == y2.Id);
			BimapOperation(hashBimap4, x => new Order { Id = x }, y => new Bill { Id = y }, new OrderEqualityComparer(), new BillEqualityComparer());

			var hashBimap5 = new HashBimap<int, int>();
			BimapOperation(hashBimap5, x => x, y => y, EqualityComparer<int>.Default, EqualityComparer<int>.Default);

			var hashBimap6 = new HashBimap<Order, int>((x1, x2) => x1.Id == x2.Id);
			BimapOperation(hashBimap6, x => new Order { Id = x }, y => y, new OrderEqualityComparer(), EqualityComparer<int>.Default);

			var hashBimap7 = new HashBimap<int, Order>((x1, x2) => x1.Id == x2.Id);
			BimapOperation(hashBimap7, x => x, y => new Order { Id = y }, EqualityComparer<int>.Default, new OrderEqualityComparer());

			var hashBimap8 = new HashBimap<Order, int>(new OrderEqualityComparer());
			BimapOperation(hashBimap8, x => new Order { Id = x }, y => y, new OrderEqualityComparer(), EqualityComparer<int>.Default);

			var hashBimap9 = new HashBimap<int, Order>(new OrderEqualityComparer());
			BimapOperation(hashBimap9, x => x, y => new Order { Id = y }, EqualityComparer<int>.Default, new OrderEqualityComparer());
		}

		[Fact]
		public void TestTreeBimapConstructor()
		{
			var treeBimap1 = new TreeBimap<Order, Bill>(new OrderComparer(), new BillComparer());
			BimapOperation(treeBimap1, x => new Order { Id = x }, y => new Bill { Id = y }, new OrderEqualityComparer(), new BillEqualityComparer());

			var treeBimap2 = new TreeBimap<Order, Bill>((x1, x2) => x1.Id - x2.Id, (y1, y2) => y1.Id - y2.Id);
			BimapOperation(treeBimap2, x => new Order { Id = x }, y => new Bill { Id = y }, new OrderEqualityComparer(), new BillEqualityComparer());

			var treeBimap3 = new TreeBimap<int, int>();
			BimapOperation(treeBimap3, x => x, y => y, EqualityComparer<int>.Default, EqualityComparer<int>.Default);

			var treeBimap4 = new TreeBimap<Order, int>((x1, x2) => x1.Id - x2.Id);
			BimapOperation(treeBimap4, x => new Order { Id = x }, y => y, new OrderEqualityComparer(), EqualityComparer<int>.Default);

			var treeBimap5 = new TreeBimap<int, Order>((x1, x2) => x1.Id - x2.Id);
			BimapOperation(treeBimap5, x => x, y => new Order { Id = y }, EqualityComparer<int>.Default, new OrderEqualityComparer());

			var treeBimap6 = new TreeBimap<Order, int>(new OrderComparer());
			BimapOperation(treeBimap6, x => new Order { Id = x }, y => y, new OrderEqualityComparer(), EqualityComparer<int>.Default);

			var treeBimap7 = new TreeBimap<int, Order>(new OrderComparer());
			BimapOperation(treeBimap7, x => x, y => new Order { Id = y }, EqualityComparer<int>.Default, new OrderEqualityComparer());
		}

		[Fact]
		public void TestTreeBimapComplexConstructor()
		{
			var hashBimap = new HashBimap<Order, Bill>(new OrderEqualityComparer(), new BillEqualityComparer());
			for (var i = 0; i < 10000; i++)
			{
				hashBimap.Add(new Order { Id = i }, new Bill { Id = i });
			}

			var treeBimap = new TreeBimap<Order, Bill>((x1, x2) => x1.Id - x2.Id, (y1, y2) => y1.Id - y2.Id, hashBimap);
			for (var i = 0; i < 10000; i++)
			{
				Assert.True(treeBimap.ContainsKey(new Order { Id = i }));
				Assert.True(treeBimap.ContainsValue(new Bill { Id = i }));
				Assert.True(treeBimap.Contains(new KeyValuePair<Order, Bill>(new Order { Id = i }, new Bill { Id = i })));
			}
		}

		[Fact]
		public void TestHashBimapComplexConstructor()
		{
			var treeBimap = new TreeBimap<Order, Bill>(new OrderComparer(), new BillComparer());
			for (var i = 0; i < 10000; i++)
			{
				treeBimap.Add(new Order { Id = i }, new Bill { Id = i });
			}

			var hashBimap = new HashBimap<Order, Bill>(10000, (x1, x2) => x1.Id == x2.Id, (y1, y2) => y1.Id == y2.Id, treeBimap);
			for (var i = 0; i < 10000; i++)
			{
				Assert.True(hashBimap.ContainsKey(new Order { Id = i }));
				Assert.True(hashBimap.ContainsValue(new Bill { Id = i }));
				Assert.True(hashBimap.Contains(new KeyValuePair<Order, Bill>(new Order { Id = i }, new Bill { Id = i })));
			}
		}

		[Fact]
		public void TestHashBimapAdd()
		{
			BimapAdd(new HashBimap<int, int>());
		}

		[Fact]
		public void TestTreeBimapAdd()
		{
			BimapAdd(new TreeBimap<int, int>());
		}

		[Fact]
		public void TestHashBimapEnforce()
		{
			BimapEnforce(new HashBimap<int, int>());
		}

		[Fact]
		public void TestTreeBimapEnforce()
		{
			BimapEnforce(new TreeBimap<int, int>());
		}

		private void BimapEnforce(IBimap<int, int> bimap)
		{
			Fill(bimap, x => x, y => y);
			bimap.Enforce(0, 1);
			Assert.Equal(9999, bimap.Count);
			Assert.True(bimap.ContainsKey(0));
			Assert.False(bimap.ContainsKey(1));
			Assert.True(bimap.ContainsValue(1));
			Assert.False(bimap.ContainsValue(0));
			Assert.Equal(bimap.ValueOf(0), 1);
			Assert.Equal(bimap.KeyOf(1), 0);

			bimap.Enforce(-1, 1);
			Assert.Equal(9999, bimap.Count);
			Assert.True(bimap.ContainsKey(-1));
			Assert.False(bimap.ContainsKey(0));
			Assert.True(bimap.ContainsValue(1));
			Assert.Equal(bimap.ValueOf(-1), 1);
			Assert.Equal(bimap.KeyOf(1), -1);

			bimap.Enforce(-1, -1);
			Assert.Equal(9999, bimap.Count);
			Assert.True(bimap.ContainsKey(-1));
			Assert.True(bimap.ContainsValue(-1));
			Assert.False(bimap.ContainsValue(1));
			Assert.Equal(bimap.ValueOf(-1), -1);
			Assert.Equal(bimap.KeyOf(-1), -1);
		}

		private void BimapAdd(IBimap<int ,int> bimap)
		{
			for (var i = 0; i < 10000; i++)
			{
				bimap.Add(i, i);
			}
			Assert.Equal(10000, bimap.Count);
			Assert.Throws(typeof(ArgumentException), () => bimap.Add(0, 10000));
			Assert.Throws(typeof(ArgumentException), () => bimap.Add(10000, 0));
		}

		private void BimapOperation<K, V>(IBimap<K, V> bimap, Func<int, K> keyGenerator, 
			Func<int, V> valueGenerator, IEqualityComparer<K> keyComparer, IEqualityComparer<V> valueComparer)
		{
			for (var i = 0; i < 10000; i++)
			{
				var key = keyGenerator(i);
				var value = valueGenerator(i);
				bimap.Add(key, value);
			}

			Assert.Equal(10000, bimap.Count);
			for(var i = 0; i < 10000; i++)
			{
				var key = keyGenerator(i);
				var value = valueGenerator(i);
				Assert.True(bimap.ContainsKey(key));
				Assert.True(bimap.ContainsValue(value));
				Assert.True(bimap.Contains(new KeyValuePair<K, V>(key, value)));
				Assert.Equal(bimap.ValueOf(key), value, valueComparer);
				Assert.Equal(bimap.KeyOf(value), key, keyComparer);
			}
			var notExistOrder = keyGenerator(10000);
			var notExistBill = valueGenerator(10000);
			Assert.False(bimap.ContainsKey(notExistOrder));
			Assert.False(bimap.ContainsValue(notExistBill));

			for (var i = 5000; i < 7500; i++)
			{
				var key = keyGenerator(i);
				Assert.True(bimap.RemoveKey(key));
				Assert.False(bimap.ContainsKey(key));
				Assert.False(bimap.ContainsValue(valueGenerator(i)));
			}
			Assert.Equal(7500, bimap.Count);

			for (var i = 7500; i < 10000; i++)
			{
				var value = valueGenerator(i);
				Assert.True(bimap.RemoveValue(value));
				Assert.False(bimap.ContainsKey(keyGenerator(i)));
				Assert.False(bimap.ContainsValue(valueGenerator(i)));
			}
			Assert.Equal(5000, bimap.Count);

			var newKey = keyGenerator(0);
			var newValue = valueGenerator(1);
			bimap.Enforce(newKey, newValue);
			Assert.Equal(bimap.ValueOf(newKey), newValue, valueComparer);
			Assert.Equal(bimap.KeyOf(newValue), newKey, keyComparer);
		}

		private void Fill<K, V>(IBimap<K, V> bimap, Func<int, K> keyGenerator, Func<int, V> valueGenerator)
		{
			for (var i = 0; i < 10000; i++)
			{
				bimap.Add(keyGenerator(i), valueGenerator(i));
			}
		}
	}
}
