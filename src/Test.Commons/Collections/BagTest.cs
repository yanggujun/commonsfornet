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
using System.Globalization;
using Commons.Collections.Bag;
using Xunit;

namespace Test.Commons.Collections
{
	public class BagTest
	{
		[Fact]
		public void TestHashBagOperation()
		{
			var bag = new HashedBag<Order>((x1, x2) => x1.Id == x2.Id);
			TestBagOperation(bag);
		}

		[Fact]
		public void TestTreeBagOperation()
		{
			var bag = new TreeBag<Order>((x1, x2) => x1.Id - x2.Id);
			TestBagOperation(bag);
		}

		[Fact]
		public void TestTreeMapSpecialOperation()
		{
			var bag = new TreeBag<Order>((x1, x2) => x1.Id - x2.Id);
			for (var i = 0; i < 1000; i++)
			{
				bag.Add(new Order { Id = i + 1 });
			}
			Assert.Equal(1, bag.Min.Id);
			Assert.Equal(1000, bag.Max.Id);
		}

		[Fact]
		public void TestBagCollectionOperation()
		{
			var bag = new HashedBag<Order>((x1, x2) => x1.Id == x2.Id);
			for (var i = 0; i < 1000; i++)
			{
				for (var j = 0; j < i + 1; j++)
				{
					bag.Add(new Order { Id = i + 1 });
				}
				Assert.Equal(i + 1, bag[new Order { Id = i + 1 }]);
			}

			for (var i = 300; i < 400; i++)
			{
				bag.Remove(new Order { Id = i });
				Assert.False(bag.Contains(new Order { Id = i }));
			}
		}

		[Fact]
		public void TestBagNotExistItem()
		{
			TestBagItemNotExist(new HashedBag<string>());
			TestBagItemNotExist(new TreeBag<string>());
		}

		[Fact]
		public void TestBagNoItem()
		{
			var bag = new TreeBag<string>();
			Assert.Throws(typeof(InvalidOperationException), () => bag.Max);
			Assert.Throws(typeof(InvalidOperationException), () => bag.Min);
		}

		[Fact]
		public void TestHashedBagConstructors()
		{
			var bag1 = new HashedBag<string>();
			Fill(bag1, x => x.ToString(CultureInfo.InvariantCulture));
			BagConstructorOperations(bag1);

			var bag2 = new HashedBag<Order>((x1, x2) => x1.Id == x2.Id);
			Fill(bag2, x => new Order { Id = x });
			BagConstructorOperations(bag2);

			var bag3 = new HashedBag<string>(bag1);
			BagConstructorOperations(bag3);

			var bag4 = new HashedBag<Order>(new OrderEqualityComparer());
            Fill(bag4, x => new Order { Id = x });
			BagConstructorOperations(bag4);

			var bag5 = new HashedBag<Order>(bag4, new OrderEqualityComparer().Equals);
			BagConstructorOperations(bag5);
		}

		[Fact]
		public void TestTreeBagConstructors()
		{
			var bag1 = new TreeBag<string>();
			Fill(bag1, x => x.ToString(CultureInfo.InvariantCulture));
			BagConstructorOperations(bag1);

			var bag2 = new TreeBag<Order>((x1, x2) => x1.Id - x2.Id);
			Fill(bag2, x => new Order { Id = x });
			BagConstructorOperations(bag2);

			var bag3 = new TreeBag<string>(bag1);
			BagConstructorOperations(bag3);

			var bag4 = new TreeBag<Order>(new OrderComparer());
			Fill(bag4, x => new Order { Id = x });
			BagConstructorOperations(bag4);

			var bag5 = new TreeBag<Order>(bag4, new OrderComparer().Compare);
			BagConstructorOperations(bag5);
		}

		private void BagConstructorOperations<T>(IBag<T> bag)
		{
			var total = 0;
			for (var i = 0; i < 1000; i++)
			{
				total += (i + 1);
			}
			Assert.Equal(total, bag.Count);
		}

		private static void Fill<T>(IBag<T> bag, Func<int, T> itemGen, int count = 1000)
		{
			for (var i = 0; i < count; i++)
			{
				bag.Add(itemGen(i), i + 1);
			}
		}

		private static void TestBagItemNotExist(IBag<string> bag)
		{
			for (var i = 0; i < 100; i++)
			{
				for (var j = 0; j < 10; j++)
				{ 
					bag.Add(i.ToString());
				}
			}
			Assert.True(bag.Contains("50"));
			Assert.False(bag.Contains("200"));
			Assert.Throws(typeof(ArgumentException), () => bag["150"]);
			Assert.False(bag.Remove("199"));
			Assert.True(bag.Remove("99"));
		}

		private static void TestBagOperation(IBag<Order> bag)
		{
			var total = 0;
			for (var i = 0; i < 1000; i++)
			{
				bag.Add(new Order {Id = i + 1}, i + 1);
				total += i + 1;
			}

			var itemNumber = 0;
			foreach (var item in bag)
			{
				itemNumber++;
			}
			Assert.Equal(itemNumber, bag.Count);

			Assert.Equal(total, bag.Count);

			for (var i = 0; i < 1000; i++)
			{
				var count = bag[new Order {Id = i + 1}];
				Assert.Equal(count, i + 1);
				Assert.True(bag.Contains(new Order { Id = i + 1 }));
			}

			for (var i = 2000; i < 3000; i++)
			{
				Assert.False(bag.Contains(new Order { Id = i }));
			}

			Assert.Equal(1000, bag.ToUnique().Count);

			for (var i = 400; i < 600; i++)
			{
				Assert.True(bag.Remove(new Order {Id = i}, 100));
				total -= 100;
				Assert.Equal(total, bag.Count);
			}

			Assert.Equal(1000, bag.ToUnique().Count);

			bag.Clear();
			Assert.Equal(0, bag.Count);
		}
	}
}
