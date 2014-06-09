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

using Commons.Collections.Common;
using Commons.Collections.Map;
using Commons.Collections.Set;

using Xunit;

namespace Test.Commons.Perf
{
    public class PerformanceTestSuite
    {
        [Fact]
        public void CompareStringHashPerf()
        {
            var guids = BuildTestCollection(1000000, () => Guid.NewGuid().ToString()).ToList();
			var items = new List<byte[]>();
			foreach (var i in guids)
			{
				items.Add(Encoding.ASCII.GetBytes(i));
			}
            var murmur = new MurmurHash32();
            var result1 = Evaluate(items, x =>
            {
                foreach (var item in x)
                {
					var hash = murmur.Hash(item);
                }
            });
            Console.WriteLine("Murmurhash32: " + result1);

            var fnv = new FnvHash32();
            var result2 = Evaluate(items, x =>
            {
                foreach (var item in x)
                {
					var hash = fnv.Hash(item);
                }
            });
            Console.WriteLine("FNV Hash: " + result2);

            var result3 = Evaluate(guids, x =>
                {
                    foreach(var item in x)
                    {
                        var hash = item.GetHashCode();
                    }
                });
            Console.WriteLine("Plain hash: " + result3);
        }

        [Fact]
        public void CompareStringHashCollision()
        {
            var guids = BuildTestCollection(1000000, () => Guid.NewGuid().ToString()).ToList();
            var murmur = new MurmurHash32();
            Console.WriteLine("Mumurhash duplicate: " + EvaluateCollision(guids, str => (int)murmur.Hash(Encoding.ASCII.GetBytes(str))[0]));
            var fnv = new FnvHash32();
            Console.WriteLine("FNV duplicate: " + EvaluateCollision(guids, str => (int)fnv.Hash(Encoding.ASCII.GetBytes(str))[0]));
            Console.WriteLine("Plain hash duplicate: " + EvaluateCollision(guids, str => str.GetHashCode()));
        }

        [Fact]
        public void CompareStringDictPerf()
        {
            var guids = BuildTestCollection(1000000, () => Guid.NewGuid().ToString()).ToList();
            var map = new HashMap<string, string>(1000000);
            Console.WriteLine("Hash Map add: " + TestMapAddPerf(guids, map));

            var dict = new Dictionary<string, string>(1000000);
            Console.WriteLine("Dict add: " + TestMapAddPerf(guids, dict));

            var customized = new Customized32HashMap<string, string>(1000000, x => Encoding.ASCII.GetBytes(x));
            Console.WriteLine("Customized: " + TestMapAddPerf(guids, customized));
        }

        [Fact]
        public void CompareStringDictRemovePerf()
        {
            var items = BuildTestCollection(1000000, () => Guid.NewGuid().ToString()).ToList();
            var map = new HashMap<string, string>(1000000);
            Console.WriteLine("Hash Map remove: " + TestMapRemovePerf(items, map));
            var dict = new Dictionary<string, string>(1000000);
            Console.WriteLine("Dict remove: " + TestMapRemovePerf(items, dict));
            var customized = new Customized32HashMap<string, string>(1000000, x => Encoding.ASCII.GetBytes(x));
            Console.WriteLine("Customized Murmur remove: " + TestMapRemovePerf(items, customized));
            var customizedFnv = new Customized32HashMap<string, string>(1000000, new FnvHash32(), x => Encoding.ASCII.GetBytes(x), EqualityComparer<string>.Default);
            Console.WriteLine("Customized FNV remove: " + TestMapRemovePerf(items, customizedFnv));
        }

        [Fact]
        public void CompareStringDictUpdatePerf()
        {
            var items = BuildTestCollection(1000000, () => Guid.NewGuid().ToString()).ToList();
            var map = new HashMap<string, string>(1000000);
            Console.WriteLine("Hash Map update: " + TestMapUpdatePerf(items, map));
            var dict = new Dictionary<string, string>(1000000);
            Console.WriteLine("Dict update: " + TestMapUpdatePerf(items, dict));
            var customized = new Customized32HashMap<string, string>(1000000, x => Encoding.ASCII.GetBytes(x));
            Console.WriteLine("Customized update: " + TestMapUpdatePerf(items, customized));
            var customizedFnv = new Customized32HashMap<string, string>(1000000, new FnvHash32(), x => Encoding.ASCII.GetBytes(x), EqualityComparer<string>.Default);
            Console.WriteLine("Customized FNV update: " + TestMapUpdatePerf(items, customizedFnv));
        }


        [Fact]
        public void CompareIntMapPerf()
        {
            var rand = new Random((int)(DateTime.Now.Ticks & 0x0000ffff));
            var numbers = BuildTestCollection(1000000, () => rand.Next()).ToList();
            var dict = new Dictionary<int, int>(1000000);
            Console.WriteLine("Int Dict: " + TestMapAddPerf(numbers, dict));
            var map = new HashMap<int, int>(1000000);
            Console.WriteLine("Int Hash Map: " + TestMapAddPerf(numbers, map));
            var customizedMap = new Customized32HashMap<int, int>(1000000, x => ConvertIntToBytes(x));
            Console.WriteLine("Customized Int Hash Map: " + TestMapAddPerf(numbers, customizedMap));
        }

        [Fact]
        public void CompareIntHashPerf()
        {
            var rand = new Random((int)(DateTime.Now.Ticks & 0x0000ffff));
            var numbers = BuildTestCollection(1000000, () => rand.Next()).ToList();
            var murmur = new MurmurHash32();
            var murmurResult = Evaluate(numbers, x => x.ToList().ForEach(y => murmur.Hash(ConvertIntToBytes(y))));
            Console.WriteLine("Murmur Hash int: " + murmurResult);
            var fnv = new FnvHash32();
            var fnvResult = Evaluate(numbers, x => x.ToList().ForEach(y => fnv.Hash(ConvertIntToBytes(y))));
            Console.WriteLine("FNV Hash int: " + fnvResult);
            var plainResult = Evaluate(numbers, x => x.ToList().ForEach(y => y.GetHashCode()));
            Console.WriteLine("Plain hash int: " + plainResult);
        }

        [Fact]
        public void CompareGuidHashPerf()
        {
            var items = BuildTestCollection(1000000, () => Guid.NewGuid()).ToList();
            var murmur = new MurmurHash32();
            var murmurResult = Evaluate(items, x => x.ToList().ForEach(y => murmur.Hash(y.ToByteArray())));
            Console.WriteLine("Murmur Hash GUID: " + murmurResult);
            var fnv = new FnvHash32();
            var fnvResult = Evaluate(items, x => x.ToList().ForEach(y => fnv.Hash(y.ToByteArray())));
            Console.WriteLine("FNV Hash GUID: " + fnvResult);
            var plainResult = Evaluate(items, x => x.ToList().ForEach(y => y.GetHashCode()));
            Console.WriteLine("Plain Hash GUID: " + plainResult);
        }

        [Fact]
        public void CompareIntHashCollision()
        {
            var rand = new Random((int)(DateTime.Now.Ticks & 0x0000ffff));
            var numbers = BuildTestCollection(1000000, () => rand.Next()).ToList();
            var murmur = new MurmurHash32();
            Console.WriteLine("Murmurhash int collision: " + EvaluateCollision(numbers, x => (int)murmur.Hash(ConvertIntToBytes(x))[0]));
            var fnv = new FnvHash32();
            Console.WriteLine("Fnv int collision: " + EvaluateCollision(numbers, x => (int)fnv.Hash(ConvertIntToBytes(x))[0]));
            Console.WriteLine("Dict int collision: " + EvaluateCollision(numbers, x => x.GetHashCode()));
        }

        public byte[] ConvertIntToBytes(int number32)
        {
            var bytes = new byte[4];
            bytes[0] = (byte)(number32 >> 24);
            bytes[1] = (byte)(number32 >> 16);
            bytes[2] = (byte)(number32 >> 8);
            bytes[3] = (byte)number32;

            return bytes;

        }

        private static double TestMapAddPerf<T>(IList<T> source, IDictionary<T, T> collection)
        {
            return Evaluate(source, items =>
                {
                    foreach (var item in items)
                    {
                        collection.Add(item, item);
                    }
                });
        }

        private static double TestMapUpdatePerf<T>(IList<T> source, IDictionary<T, T> collection)
        {
            foreach (var item in source)
            {
                collection.Add(item, item);
            }
            return Evaluate(source, items =>
                {
                    var index = 0;
                    foreach (var item in items)
                    {
                        collection[item] = item;
                    }
                });
        }

        private static double TestMapRemovePerf<T>(IList<T> source, IDictionary<T, T> collection)
        {
            foreach (var item in source)
            {
                collection.Add(item, item);
            }
            return Evaluate(source, items =>
                {
                    foreach (var item in items)
                    {
                        collection.Remove(item);
                    }
                });
        }

        private static double Evaluate<T>(IList<T> source, Closure<IEnumerable<T>> executor)
        {
            var start = DateTime.Now;
            executor(source);
            return (DateTime.Now - start).TotalMilliseconds;
        }

        private static int EvaluateCollision<T>(IList<T> guids, Transformer<T, int> hash)
        {
            var duplicated = 0;
            var map = new HashMap<int, int>();
            foreach (var item in guids)
            {
                var code = hash(item);
                if (!map.ContainsKey(code))
                {
                    map.Add(code, 1);
                }
                else
                {
                    duplicated++;
                    map[code]++;
                }
            }
            return duplicated;
        }

        private static IEnumerable<T> BuildTestCollection<T>(int itemNumber, Factory<T> create)
        {
            var set = new TreeSet<T>();
            var index = 0;
            while (index < itemNumber)
            {
                T item = create();
                if (!set.Contains(item))
                {
                    set.Add(item);
                    index++;
                }
            }

            return set;
        }

    }
}
