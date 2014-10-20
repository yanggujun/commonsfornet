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
	}
}
