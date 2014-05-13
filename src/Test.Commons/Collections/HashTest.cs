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

using Xunit;

using Commons.Collections.Map;
using Commons.Collections.Common;
using Commons.Collections.Extension;

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
            var orders = new HashMap<string, Order>(4);
            HashAbility(orders);
        }
        private void HashAbility(HashMap<string, Order> orders)
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
                orders.Add(key, order);
                orderDict.Add(key, order);
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

        [Fact]
        public void TestHashMapContains()
        {
            var orders = new HashMap<string, Order>();
            var key = Guid.NewGuid().ToString();
            var order = new Order();
            orders.Add(key, order);
            Assert.True(orders.ContainsKey(key));
        }

        [Fact]
        public void TestHashMapConstructors()
        {
            var orders = new HashMap<string, Order>();
            HashAbility(orders);

            var orders2 = new HashMap<string, Order>(1000);
            HashAbility(orders2);

            var orders3 = new HashMap<string, Order>(new MurmurHash32(), x => x.ToBytes());
            HashAbility(orders3);

            var orders4 = new HashMap<string, Order>(new MurmurHash32(), x => x.ToBytes(), (x1, x2) => x1 == x2);
            HashAbility(orders4);

            var orders5 = new HashMap<string, Order>(new MurmurHash32(), x => x.ToBytes(), EqualityComparer<string>.Default);
            HashAbility(orders5);

            var orders6 = new HashMap<string, Order>(1000, new MurmurHash32(), x => x.ToBytes(), EqualityComparer<string>.Default);
            HashAbility(orders6);

            var orders7 = new HashMap<string, Order>(1000, new MurmurHash32(), x => x.ToBytes(), (x1, x2) => x1 == x2);
            HashAbility(orders7);

            var orders8 = new HashMap<string, Order>(1000, new MurmurHash32(), x => x.ToBytes(), (IEqualityComparer<string>)null);
            HashAbility(orders8);
        }
    }
}
