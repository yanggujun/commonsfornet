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
            var hashBimap1 = new HashedBimap<Order, Bill>((x1, x2) => x1.Id == x2.Id, (y1, y2) => y1.Id == y2.Id);
            BimapOperation(hashBimap1, x => new Order { Id = x }, y => new Bill { Id = y }, new OrderEqualityComparer(), new BillEqualityComparer());

            var hashBimap2 = new HashedBimap<Order, Bill>(new OrderEqualityComparer(), new BillEqualityComparer());
            BimapOperation(hashBimap2, x => new Order { Id = x }, y => new Bill { Id = y }, new OrderEqualityComparer(), new BillEqualityComparer());

            var hashBimap3 = new HashedBimap<Order, Bill>(10000, new OrderEqualityComparer(), new BillEqualityComparer());
            BimapOperation(hashBimap3, x => new Order { Id = x }, y => new Bill { Id = y }, new OrderEqualityComparer(), new BillEqualityComparer());

            var hashBimap4 = new HashedBimap<Order, Bill>(10000, (x1, x2) => x1.Id == x2.Id, (y1, y2) => y1.Id == y2.Id);
            BimapOperation(hashBimap4, x => new Order { Id = x }, y => new Bill { Id = y }, new OrderEqualityComparer(), new BillEqualityComparer());

            var hashBimap5 = new HashedBimap<int, int>();
            BimapOperation(hashBimap5, x => x, y => y, EqualityComparer<int>.Default, EqualityComparer<int>.Default);

            var hashBimap6 = new HashedBimap<Order, int>((x1, x2) => x1.Id == x2.Id);
            BimapOperation(hashBimap6, x => new Order { Id = x }, y => y, new OrderEqualityComparer(), EqualityComparer<int>.Default);

            var hashBimap7 = new HashedBimap<int, Order>((x1, x2) => x1.Id == x2.Id);
            BimapOperation(hashBimap7, x => x, y => new Order { Id = y }, EqualityComparer<int>.Default, new OrderEqualityComparer());

            var hashBimap8 = new HashedBimap<Order, int>(new OrderEqualityComparer());
            BimapOperation(hashBimap8, x => new Order { Id = x }, y => y, new OrderEqualityComparer(), EqualityComparer<int>.Default);

            var hashBimap9 = new HashedBimap<int, Order>(new OrderEqualityComparer());
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
            var hashBimap = new HashedBimap<Order, Bill>(new OrderEqualityComparer(), new BillEqualityComparer());
            for (var i = 0; i < 10000; i++)
            {
                hashBimap.Add(new Order { Id = i }, new Bill { Id = i });
            }

            var treeBimap = new TreeBimap<Order, Bill>(hashBimap, (x1, x2) => x1.Id - x2.Id, (y1, y2) => y1.Id - y2.Id);
            for (var i = 0; i < 10000; i++)
            {
                Assert.True(treeBimap.ContainsKey(new Order { Id = i }));
                Assert.True(treeBimap.ContainsValue(new Bill { Id = i }));
                Assert.True(treeBimap.Contains(new KeyValuePair<Order, Bill>(new Order { Id = i }, new Bill { Id = i })));
            }

            var bimap2 = new HashedBimap<int, int>();
            for (var i = 0; i < 10000; i++)
            {
                bimap2.Add(i, i + 1);
            }

            var treeBimap2 = new TreeBimap<int, int>(bimap2);
            for (var i = 0; i < 10000; i++)
            {
                Assert.True(treeBimap2.ContainsKey(i));
                Assert.True(treeBimap2.ContainsValue(i + 1));
                Assert.True(treeBimap2.Contains(new KeyValuePair<int, int>(i, i + 1)));
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

            var hashBimap = new HashedBimap<Order, Bill>(treeBimap, (x1, x2) => x1.Id == x2.Id, (y1, y2) => y1.Id == y2.Id);
            for (var i = 0; i < 10000; i++)
            {
                Assert.True(hashBimap.ContainsKey(new Order { Id = i }));
                Assert.True(hashBimap.ContainsValue(new Bill { Id = i }));
                Assert.True(hashBimap.Contains(new KeyValuePair<Order, Bill>(new Order { Id = i }, new Bill { Id = i })));
            }

            var bimap2 = new TreeBimap<int, int>();
            for (var i = 0; i < 10000; i++)
            {
                bimap2.Add(i, i + 1);
            }
            var hashBimap2 = new HashedBimap<int, int>(bimap2);
            for (var i = 0; i < 10000; i++)
            {
                Assert.True(hashBimap2.ContainsKey(i));
                Assert.True(hashBimap2.ContainsValue(i + 1));
                Assert.True(hashBimap2.Contains(new KeyValuePair<int, int>(i, i + 1)));
            }
        }

        [Fact]
        public void TestHashBimapAdd()
        {
            BimapAdd(new HashedBimap<int, int>());
        }

        [Fact]
        public void TestTreeBimapAdd()
        {
            BimapAdd(new TreeBimap<int, int>());
        }

        [Fact]
        public void TestHashBimapEnforce()
        {
            BimapEnforce(new HashedBimap<int, int>());
        }

        [Fact]
        public void TestTreeBimapEnforce()
        {
            BimapEnforce(new TreeBimap<int, int>());
        }

        [Fact]
        public void TestHashBimapRemove()
        {
            BimapRemove(new HashedBimap<int, int>());
        }

        [Fact]
        public void TestTreeBimapRemove()
        {
            BimapRemove(new TreeBimap<int, int>());
        }

        [Fact]
        public void TestHashBimapTryGetValue()
        {
            BimapTryGetValue(new HashedBimap<int, int>());
        }

        [Fact]
        public void TestTreeBimapTryGetValue()
        {
            BimapTryGetValue(new TreeBimap<int, int>());
        }

        [Fact]
        public void TestHashBimapIndexer()
        {
            BimapIndexer(new HashedBimap<int, int>());
        }

        [Fact]
        public void TestTreeBimapIndexer()
        {
            BimapIndexer(new TreeBimap<int, int>());
        }

        [Fact]
        public void TestHashBimapDictionaryIndexer()
        {
            var bimap = new HashedBimap<int, int>();
            Fill(bimap, x => x, y => y);
            DictionaryIndexer(bimap);
            AssertBimapDictionaryIndexer(bimap);
        }

        [Fact]
        public void TestTreeBimapDictionaryIndexer()
        {
            var bimap = new TreeBimap<int, int>();
            Fill(bimap, x => x, y => y);
            DictionaryIndexer(bimap);
            AssertBimapDictionaryIndexer(bimap);
        }

        [Fact]
        public void TestHashedBimapDictionaryIndexerSetNotExistingValue()
        {
            var bimap = new HashedBimap<int, int>();
            Fill(bimap, x => x, y => y);
            BimapDictionaryIndexerSetNotExistingValue(bimap);
        }

        [Fact]
        public void TestTreeBimapDictionaryIndexerSetNotExistingValue()
        {
            var bimap = new TreeBimap<int, int>();
            Fill(bimap, x => x, y => y);
            BimapDictionaryIndexerSetNotExistingValue(bimap);
        }

        private void BimapDictionaryIndexerSetNotExistingValue(IDictionary<int, int> bimap)
        {
            for (var i = 10000; i < 15000; i++)
            {
                bimap[i] = i;
                Assert.Equal(i, bimap[i]);
                Assert.Equal(i + 1, bimap.Count);
            }
        }

        [Fact]
        public void TestHashBimapCollectionRemove()
        {
            var bimap = new HashedBimap<Order, Bill>(new OrderEqualityComparer(), new BillEqualityComparer());
            Fill(bimap, x => new Order { Id = x }, y => new Bill { Id = y });
            CollectionRemove(bimap);
        }

        [Fact]
        public void TestTreeBiampColletionRemove()
        {
            var bimap = new TreeBimap<Order, Bill>(new OrderComparer(), new BillComparer());
            Fill(bimap, x => new Order { Id = x }, y => new Bill { Id = y });
            CollectionRemove(bimap);
        }

        [Fact]
        public void TestHashBimapInverse()
        {
            BimapInverse(new HashedBimap<int, int>());
        }

        [Fact]
        public void TestTreeBimapInverse()
        {
            BimapInverse(new TreeBimap<int, int>());
        }

        [Fact]
        public void TestHashBimapKeySetValueSet()
        {
            BimapKeySetValueSet(new HashedBimap<Order, Bill>(new OrderEqualityComparer(), new BillEqualityComparer()));
        }

        [Fact]
        public void TestTreeBimapKeySetValueSet()
        {
            BimapKeySetValueSet(new TreeBimap<Order, Bill>(new OrderComparer(), new BillComparer()));
        }

        [Fact]
        public void TestHashBimapException()
        {
            BimapException(new HashedBimap<Order, Bill>(new OrderEqualityComparer(), new BillEqualityComparer()));
        }

        public void TestTreeBimapException()
        {
            BimapException(new TreeBimap<Order, Bill>(new OrderComparer(), new BillComparer()));
        }

        private void BimapException(IBimap<Order, Bill> bimap)
        {
            Fill(bimap, x => new Order { Id = x }, y => new Bill { Id = y });
            Assert.Throws(typeof(ArgumentNullException), () => bimap.Add(null, null));
            Assert.Throws(typeof(ArgumentException), () => bimap.Add(new Order { Id = 0 }, new Bill { Id = 10000 }));
            Assert.Throws(typeof(ArgumentException), () => bimap.Add(new Order { Id = 10000 }, new Bill { Id = 0 }));
            Assert.Throws(typeof(KeyNotFoundException), () => bimap.ValueOf(new Order { Id = 10000 }));
            Assert.Throws(typeof(KeyNotFoundException), () => bimap.KeyOf(new Bill { Id = 10000 }));
            Assert.Throws(typeof(KeyNotFoundException), () => bimap.ValueOf(new Order { Id = -1 }));
            Assert.Throws(typeof(KeyNotFoundException), () => bimap.KeyOf(new Bill { Id = -1 }));
            Assert.Throws(typeof(ArgumentNullException), () => bimap.Contains(new KeyValuePair<Order, Bill>(null, null)));
        }

        private void BimapKeySetValueSet(IBimap<Order, Bill> bimap)
        {
            Fill(bimap, x => new Order { Id = x }, y => new Bill { Id = y });
            var keyset = bimap.KeySet();
            var valueset = bimap.ValueSet();
            for (var i = 0; i < 10000; i++)
            {
                Assert.True(keyset.Contains(new Order { Id = i }));
                Assert.True(valueset.Contains(new Bill { Id = i }));
            }
            Assert.False(keyset.Contains(new Order { Id = -1 }));
            Assert.False(keyset.Contains(new Order { Id = 10000 }));
            Assert.Equal(10000, keyset.Count);

            Assert.False(valueset.Contains(new Bill { Id = -1 }));
            Assert.False(valueset.Contains(new Bill { Id = 10000 }));
            Assert.Equal(10000, valueset.Count);
        }

        private void BimapInverse(IBimap<int, int> bimap)
        {
            Fill(bimap, x => x, y => 10000 + y);
            var inversed = bimap.Inverse();
            foreach (var item in inversed)
            {
                Assert.Equal(item.Key, item.Value + 10000);
            }

            for (var i = 0; i < 10000; i++)
            {
                Assert.Equal(inversed.KeyOf(i), 10000 + i);
                Assert.Equal(inversed.ValueOf(i + 10000), i);
            }
        }

        private void CollectionRemove(ICollection<KeyValuePair<Order, Bill>> collection)
        {
            for (var i = 0; i < 1000; i++)
            {
                Assert.True(collection.Remove(new KeyValuePair<Order, Bill>(new Order { Id = i }, new Bill { Id = i })));
            }

            Assert.False(collection.Remove(new KeyValuePair<Order, Bill>(new Order { Id = 2000 }, new Bill { Id = 3000 })));
            Assert.False(collection.Remove(new KeyValuePair<Order, Bill>(new Order { Id = 0 }, new Bill { Id = 1000 })));
            Assert.False(collection.Remove(new KeyValuePair<Order, Bill>(new Order { Id = 1000 }, new Bill { Id = 0 })));
        }

        private void AssertBimapDictionaryIndexer(IBimap<int, int> bimap)
        {
            Assert.Equal(-1, bimap.ValueOf(0));
            Assert.Equal(2, bimap.ValueOf(1));

            Assert.False(bimap.ContainsValue(0));
            Assert.Equal(1, bimap.KeyOf(2));

            Assert.False(bimap.ContainsKey(-1));
            Assert.Equal(3, bimap.KeyOf(3));
        }

        private void DictionaryIndexer(IDictionary<int, int> map)
        {
            map[0] = -1;
            Assert.Equal(10000, map.Count);

            map[1] = 2;
            Assert.Equal(9999, map.Count);
        }

        private void BimapIndexer(IBimap<int, int> bimap)
        {
            Fill(bimap, x => x, y => y);
            for (var i = 0; i < 10000; i++)
            {
                Assert.Equal(bimap.ValueOf(i), i);
                Assert.Equal(bimap.KeyOf(i), i);
            }

            Assert.Throws(typeof(KeyNotFoundException), () => bimap.ValueOf(10000));
            Assert.Throws(typeof(KeyNotFoundException), () => bimap.KeyOf(10000));
            Assert.Throws(typeof(KeyNotFoundException), () => bimap.ValueOf(-1));
            Assert.Throws(typeof(KeyNotFoundException), () => bimap.KeyOf(-1));
        }

        private void BimapTryGetValue(IBimap<int, int> bimap)
        {
            Fill(bimap, x => x, y => y);
            for (var i = 0; i < 1000; i++)
            {
                int v;
                Assert.True(bimap.TryGetValue(i, out v));
                Assert.Equal(i, v);

                int k;
                Assert.True(bimap.TryGetKey(i, out k));
                Assert.Equal(i, k);
            }

            int none;
            Assert.False(bimap.TryGetValue(-1, out none));
            Assert.False(bimap.TryGetKey(-1, out none));
            Assert.False(bimap.TryGetValue(10000, out none));
            Assert.False(bimap.TryGetKey(10000, out none));
        }

        private void BimapRemove(IBimap<int, int> bimap)
        {
            Fill(bimap, x => x, y => y);
            Assert.True(bimap.Remove(new KeyValuePair<int, int>(0, 0)));
            Assert.False(bimap.Contains(new KeyValuePair<int, int>(0, 0)));
            Assert.False(bimap.ContainsKey(0));
            Assert.False(bimap.ContainsValue(0));
            Assert.Equal(9999, bimap.Count);

            Assert.True(bimap.RemoveKey(1));
            Assert.False(bimap.Contains(new KeyValuePair<int, int>(1, 1)));
            Assert.False(bimap.ContainsKey(1));
            Assert.False(bimap.ContainsValue(1));
            Assert.Equal(9998, bimap.Count);

            Assert.True(bimap.RemoveValue(2));
            Assert.False(bimap.Contains(new KeyValuePair<int, int>(2, 2)));
            Assert.False(bimap.ContainsKey(2));
            Assert.False(bimap.ContainsValue(2));
            Assert.Equal(9997, bimap.Count);
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
            bimap.Clear();
            Assert.Equal(0, bimap.Count);
            Fill(bimap, keyGenerator, valueGenerator);
            Assert.Equal(10000, bimap.Count);
            for (var i = 0; i < 1000; i++)
            {
                Assert.True(bimap.ContainsKey(keyGenerator(i)));
                Assert.True(bimap.ContainsValue(valueGenerator(i)));
            }
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
