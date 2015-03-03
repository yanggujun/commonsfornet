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
using Commons.Collections.Set;
using Commons.Utils;
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

        public static void TestHashedSetOperations(this AbstractHashedSet<string> orders)
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
                orders.Add(key);
            }
            Assert.Equal(keys.Length, orders.Count);
            foreach (var key in keys)
            {
                Assert.True(orders.Contains(key));
            }

            for (var i = 0; i < 3000; i++)
            {
                Assert.True(orders.Remove(keys[3000 + i]));
                Assert.False(orders.Contains(keys[3000 + i]));
            }
            Assert.Equal(keys.Length - 3000, orders.Count);
            var total = 0;
            foreach (var o in orders)
            {
                total++;
            }
            Assert.Equal(total, orders.Count);
        }

        public static void DictionaryOperations<K, V>(this IDictionary source, Func<int, K> keyGen, Func<int, V> valueGen, Equator<V> valueEquator, int count = 1000)
        {
            for (var i = 0; i < count; i++)
            {
                source.Add(keyGen(i), valueGen(i));
            }
            Assert.Equal(count, source.Count);

            Assert.False(source.Contains(keyGen(count)));
            Assert.False(source.Contains(keyGen(count + 1)));

            for (var i = 0; i < 200; i++)
            {
                Assert.True(source.Contains(keyGen(i)));
                source.Remove(keyGen(i));
            }
            Assert.Equal(count - 200, source.Count);
            for (var i = 0; i < 200; i++)
            {
                Assert.False(source.Contains(keyGen(i)));
            }

            for (var i = 500; i < 700; i++)
            {
                var v = (V)source[keyGen(i)];
                Assert.True(valueEquator(v, valueGen(i)));
            }

            var keys = source.Keys;
            Assert.Equal(count - 200, keys.Count);
            var values = source.Values;
            Assert.Equal(count - 200, values.Count);
            var currentCount = 0;
            foreach (var item in source)
            {
                currentCount++;
            }
            Assert.Equal(count - 200, currentCount);
            Assert.Equal(currentCount, source.Count);

            source.Clear();
            Assert.Equal(0, source.Count);
        }

        public static void Fill<T>(this ICollection<T> source, Transformer<int, T> generator, int itemCount = 1000)
        {
            for (var i = 0; i < itemCount; i++)
            {
                source.Add(generator(i));
            }
        }

        public static void CollectionOperations<T>(this ICollection source, int count)
        {
            Assert.Equal(count, source.Count);
            source.AssertCopyTo<T>(count);

            var actualCount = 0;
            foreach (var item in source)
            {
                actualCount++;
                Assert.IsAssignableFrom<T>(item);
            }

            Assert.Equal(count, actualCount);
        }

        public static void AssertCopyTo<T>(this ICollection source, int count)
        {
            var array = new T[count + 3];
            source.CopyTo(array, 2);
            Assert.Equal(default(T), array[0]);
            Assert.Equal(default(T), array[1]);

            for (var i = 2; i < count + 2; i++)
            {
                Assert.NotEqual(default(T), array[i]);
            }

            Assert.Equal(default(T), array[count + 2]);
        }

        public static void MapSetNotExistingValue(this IDictionary<Order, Bill> map)
        {
            for (var i = 0; i < 1000; i++)
            {
                var order = new Order() { Id = i, Name = Guid.NewGuid().ToString() };
                map.Add(order, new Bill() { Id = i, Count = i });
            }

            for (var i = 1000; i < 1100; i++)
            {
                var newOrder = new Order { Id = i };
                map[newOrder] = new Bill { Id = i };
                Assert.Equal(i + 1, map.Count);
                Assert.Equal(new Bill { Id = i }, map[newOrder], new BillEqualityComparer());
            }
        }
    }
}
