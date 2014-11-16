// Copyright CommonsForNET 2014.
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
using Commons.Collections.Map;
using Xunit;

namespace Test.Commons.Collections
{
	public class LinkedHashMapTest
	{
		[Fact]
		public void TestHashMapConstructors()
		{
			var orders = new LinkedHashMap<string, Order>();
			orders.HashAbility();

			var orders2 = new LinkedHashMap<string, Order>(1000);
			orders2.HashAbility();

			var orders4 = new LinkedHashMap<string, Order>(500, (x1, x2) => x1 == x2);
			orders4.HashAbility();

			var orders6 = new LinkedHashMap<string, Order>(1000, EqualityComparer<string>.Default);
			orders6.HashAbility();

			var orders8 = new LinkedHashMap<string, Order>(1000, (IEqualityComparer<string>)null);
			orders8.HashAbility();

			var orders9 = new LinkedHashMap<string, Order>(10000, orders8, (x1, x2) => x1 == x2);
			Assert.NotEmpty(orders9);
			Assert.Equal(orders8.Count, orders9.Count);
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
	}
}
