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
using System.Threading.Tasks;
using Commons.Collections.Concurrent;
using Xunit;

namespace Test.Commons.Collections
{
	public class ConcurrentListTest
	{
		private readonly object locker = new object();
		[Fact]
		public void TestSingleThreadAdd()
		{
			var list = new ConcurrentSortedList<Order>(new OrderComparer());
			var random = new Random((int) DateTime.Now.Ticks & 0x0000ffff);
			for (var i = 0; i < 10000; i++)
			{
				var number = random.Next();
				var order = new Order {Id = number, Name = number.ToString()};
				list.Add(order);
			}
			AssertList(list, 10000);
		}

		private void AssertList(ConcurrentSortedList<Order> list, int number)
		{
			Assert.Equal(number, list.Count);
			var orders = new Order[number];
			var cursor = 0;
			foreach (var i in list)
			{
				orders[cursor] = i;
				cursor++;
			}
			for (var i = 1; i < number; i++)
			{
				Assert.True(orders[i - 1].Id <= orders[i].Id);
			}
		}

		[Fact]
		public void TestMultithreadsAdd()
		{
			var list = new ConcurrentSortedList<Order>(new OrderComparer());
			var tasks = new Task[10000];
			var random = new Random((int) DateTime.Now.Ticks & 0x0000ffff);
			for (var i = 0; i < 10000; i++)
			{
				tasks[i] = Task.Factory.StartNew(() =>
				{
					var id = 0;
					lock (locker)
					{
						id = random.Next();
					}
					list.Add(new Order{Id = id, Name = id.ToString()});
				});
			}
			Task.WaitAll(tasks);
			AssertList(list, 10000);
		}
	}
}
