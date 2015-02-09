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
	public class MultiMapTest
	{
		private readonly Func<int, int, Bill> valueGen = (i, j) => new Bill { Id = (i * 50) + 10000 + j };

		[Fact]
		public void TestMultiValueHashedMapAddSingleValue()
		{
			var mvMap = new MultiValueHashedMap<int, Bill>(10000, EqualityComparer<int>.Default, new BillEqualityComparer());
			MultiValueMapAddSingleValue(mvMap);
		}

		[Fact]
		public void TestMultiValueTreeMapAddSingleValue()
		{
			var mvMap = new MultiValueTreeMap<int, Bill>(Comparer<int>.Default, (x1, x2) => x1.Id == x2.Id);
			MultiValueMapAddSingleValue(mvMap);
		}

		[Fact]
		public void TestMultiValueHashedMapAddSingleValueAndCollectionValue()
		{
			var mvMap = new MultiValueHashedMap<int, Bill>(10000, EqualityComparer<int>.Default, new BillEqualityComparer());
			MultiValueMapAddSingleValueAndCollectionValue(mvMap);
		}

		[Fact]
		public void TestMultiValueTreeMapAddSingleValueAndCollectionValue()
		{
			var mvMap = new MultiValueTreeMap<int, Bill>(Comparer<int>.Default, (x1, x2) => x1.Id == x2.Id);
			MultiValueMapAddSingleValueAndCollectionValue(mvMap);
		}

		[Fact]
		public void TestMultiValueHashedMapRemove()
		{
			var mvMap = new MultiValueHashedMap<int, Bill>(1000, EqualityComparer<int>.Default, new BillEqualityComparer());
			MultiValueMapRemove(mvMap);
		}

		[Fact]
		public void TestMultiValueTreeMapRemove()
		{
			var mvMap = new MultiValueTreeMap<int, Bill>(Comparer<int>.Default, (x1, x2) => x1.Id == x2.Id);
			MultiValueMapRemove(mvMap);
		}

		private void MultiValueMapRemove(IMultiValueMap<int, Bill> mvMap)
		{
			Assert.Equal(0, mvMap.Keys.Count);
			Assert.Equal(0, mvMap.Count);
			Fill(mvMap, 10000, 50, x => x, valueGen);

			var total = 0;
			foreach (var item in mvMap)
			{
				total++;
			}
			Assert.Equal(10000 * 50, total);

			List<Bill> billList;
			Assert.False(mvMap.TryGetValue(10000, out billList));
			for (var i = 5000; i < 8000; i++)
			{
				List<Bill> bills;
				Assert.True(mvMap.TryGetValue(i, out bills));
				Assert.Equal(50, bills.Count);
				for (var j = 0; j < 50; j++)
				{
					Assert.True(mvMap.ContainsValue(i, valueGen(i, j)));
				}
			}

			for (var i = 5000; i < 8000; i++)
			{
				Assert.True(mvMap.Remove(i));
			}

			Assert.Equal(7000, mvMap.Keys.Count);
			Assert.Equal(10000 * 50 - 3000 * 50, mvMap.Count);

			for (var i = 5000; i < 8000; i++)
			{
				Assert.Throws(typeof(KeyNotFoundException), () => mvMap[i]);
				Assert.False(mvMap.ContainsKey(i));
				Assert.Throws(typeof(KeyNotFoundException), () => mvMap.CountOf(i));
				for (var j = 0; j < 50; j++)
				{
					Assert.False(mvMap.ContainsValue(i, valueGen(i, j)));
				}
			}

			for (var i = 2000; i < 3000; i++)
			{
				for (var j = 10; j < 20; j++)
				{
					Assert.True(mvMap.RemoveValue(i, valueGen(i, j)));
				}
			}

			for (var i = 2000; i < 3000; i++)
			{
				Assert.Equal(40, mvMap.CountOf(i));
				for (var j = 0; j < 10; j++)
				{
					Assert.True(mvMap.ContainsValue(i, valueGen(i, j)));
				}

				for (var j = 10; j < 20; j++)
				{
					Assert.False(mvMap.ContainsValue(i, valueGen(i, j)));
				}

				for (var j = 20; j < 50; j++)
				{
					Assert.True(mvMap.ContainsValue(i, valueGen(i, j)));
				}
			}

			Assert.Equal(50, mvMap.CountOf(1999));
			Assert.Equal(50, mvMap.CountOf(3000));
		}

		private void MultiValueMapAddSingleValue(IMultiValueMap<int, Bill> mvMap)
		{
			Fill(mvMap, 10000, 50, x => x, valueGen);

			Assert.Equal(10000 * 50, mvMap.Count);
			Assert.Equal(10000, mvMap.Keys.Count);
			Assert.Equal(10000 * 50, mvMap.Values.Count);

			foreach(var item in mvMap.Keys)
			{
				Assert.Equal(50, mvMap[item].Count);
				Assert.Equal(50, mvMap.CountOf(item));
			}

			Assert.False(mvMap.ContainsKey(10000));

			for (var i = 0; i < 10000; i++)
			{
				for (var j = 0; j < 50; j++)
				{
					var value = valueGen(i, j);
					Assert.True(mvMap.ContainsValue(i, value));
				}

				Assert.False(mvMap.ContainsValue(i, new Bill { Id = (i * 50) + 10000 + 51 }));
			}
		}

		private void MultiValueMapAddSingleValueAndCollectionValue(IMultiValueMap<int, Bill> mvMap)
		{
			Fill(mvMap, 10000, 50, x => x, valueGen);

			for (var i = 1000; i < 2000; i++)
			{
				var bills = new List<Bill>();
				for (var j = 0; j < 20; j++)
				{
					bills.Add(new Bill { Id = j });
				}
				mvMap.Add(i, bills);
			}

			for (var i = 0; i < 1000; i++)
			{
				Assert.Equal(50, mvMap.CountOf(i));
			}

			for (var i = 1000; i < 2000; i++)
			{
				Assert.Equal(70, mvMap.CountOf(i));
				Assert.Equal(70, mvMap[i].Count);
			}

			for (var i = 2000; i < 3000; i++)
			{
				Assert.Equal(50, mvMap.CountOf(i));
			}

			for (var i = 1000; i < 2000; i++)
			{
				for (var j = 0; j < 5; j++)
				{
					mvMap.Add(i, new Bill { Id = j + 20 });
				}
			}

			for (var i = 0; i < 1000; i++)
			{
				Assert.Equal(50, mvMap.CountOf(i));
			}

			for (var i = 1000; i < 2000; i++)
			{
				Assert.Equal(75, mvMap.CountOf(i));
				Assert.Equal(75, mvMap[i].Count);
			}

			for (var i = 2000; i < 3000; i++)
			{
				Assert.Equal(50, mvMap.CountOf(i));
			}

			for (var i = 1000; i < 2000; i++)
			{
				for (var j = 0; j < 25; j++)
				{ 
					Assert.True(mvMap.ContainsValue(i, new Bill { Id = j }));
				}
			}
		}

		private static void Fill<K, V>(IMultiValueMap<K, V> mvMap, int keyCount, int valueCount, Func<int, K> keyGen, Func<int, int, V> valueGen)
		{
			for (var i = 0; i < keyCount; i++)
			{
				for (var j = 0; j < valueCount; j++)
				{
					mvMap.Add(keyGen(i), valueGen(i, j));
				}
			}
		}
	}
}
