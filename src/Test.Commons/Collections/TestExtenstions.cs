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
using System.Collections;
using System.Collections.Generic;

using Xunit;

namespace Test.Commons.Collections
{
	internal static class TestExtenstions
	{
		public static void HashAbility(this IDictionary<string, Order> orders)
		{
			var keys = new string[10000];
			for (var i = 0; i < keys.Length; i++)
			{
				keys[i] = Guid.NewGuid().ToString();
			}

			var orderDict = new Dictionary<string, Order>();
			var idIndex = 0;
			foreach (var key in keys)
			{
				Order order = new Order();
				order.Id = idIndex++;
				order.Name = Guid.NewGuid().ToString();
				orderDict.Add(key, order);
				orders.Add(key, order);
			}
			Assert.Equal(keys.Length, orders.Count);
			foreach (var key in keys)
			{
				Assert.True(orders.ContainsKey(key));
			}
			foreach (var key in orderDict.Keys)
			{
				Assert.Equal(orderDict[key], orders[key]);
			}

			for (var i = 0; i < 3000; i++)
			{
				Assert.True(orders.Remove(keys[3000 + i]));
				Assert.False(orders.ContainsKey(keys[3000 + i]));
			}
			Assert.Equal(keys.Length - 3000, orders.Count);
			var total = 0;
			foreach (var o in orders)
			{
				total++;
			}
			Assert.Equal(total, orders.Count);
		}

		public static void AssertCopyTo<T>(ICollection<T> source)
		{
		}

		public static void AssertCopyTo<T>(ICollection source)
		{
		}
	}
}
