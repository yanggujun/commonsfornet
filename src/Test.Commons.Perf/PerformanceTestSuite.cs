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
        public void CompareHash()
        {
            var murmur = new MurmurHash32();
            var result1 = Evaluate(x =>
            {
                foreach (var item in x)
                {
                    var hash = murmur.Hash(Encoding.ASCII.GetBytes(item));
                }
            });
            Console.WriteLine("Murmurhash32: " + result1);

            var fnv = new FnvHash32();
            var result2 = Evaluate(x =>
            {
                foreach (var item in x)
                {
                    var hash = fnv.Hash(Encoding.ASCII.GetBytes(item));
                }
            });
            Console.WriteLine("FNV Hash: " + result2);

            var result3 = Evaluate(x =>
                {
                    foreach(var item in x)
                    {
                        var hash = item.GetHashCode();
                    }
                });
            Console.WriteLine("Plain hash: " + result3);
        }

        [Fact]
        public void TestHashDuplicate()
        {
            var murmur = new MurmurHash32();
            Console.WriteLine("Mumurhash duplicate: " + EvaluateDuplicate(str => (uint)murmur.Hash(str.ToBytes())[0]));
            var fnv = new FnvHash32();
            Console.WriteLine("FNV duplicate: " + EvaluateDuplicate(str => (uint)fnv.Hash(str.ToBytes())[0]));
            Console.WriteLine("Plain hash duplicate: " + EvaluateDuplicate(str => (uint)str.GetHashCode()));
        }

        [Fact]
        public void CompareDictPerf()
        {
            var map = new HashMap<string, string>();
            Console.WriteLine("Hash Map add: " + Test(map));

            var dict = new Dictionary<string, string>();
            Console.WriteLine("Dict add: " + Test(dict));

            var customized = new Customized32HashMap<string, string>(16, x => Encoding.ASCII.GetBytes(x));
            Console.WriteLine("Customized: " + Test(customized));
        }

        private static double Test(IDictionary<string, string> collection)
        {
            return Evaluate(items =>
                {
                    foreach (var item in items)
                    {
                        collection.Add(item, item);
                    }
                });
        }

        private static double Evaluate(Closure<IEnumerable<string>> executor)
        {
            var guids = BuildTestCollection(1000000);

            var start = DateTime.Now;
            executor(guids);
            return (DateTime.Now - start).TotalMilliseconds;
        }

        private static int EvaluateDuplicate(Transformer<string, uint> hash)
        {
            var guids = BuildTestCollection(1000000);

            var duplicated = 0;
            var set = new TreeSet<uint>();
            foreach (var item in guids)
            {
                var code = hash(item);
                code = (code >> 22) ^ (code & 2097151);
                if (!set.Contains(code))
                {
                    set.Add(code);
                }
                else
                {
                    duplicated++;
                }
            }
            return duplicated;
        }

        private static IEnumerable<string> BuildTestCollection(int itemNumber)
        {
            var list = new List<string>();
            for (var i = 0; i < itemNumber; i++)
            {
                list.Add(Guid.NewGuid().ToString() + Guid.NewGuid().ToString() + Guid.NewGuid().ToString());
            }

            return list;
        }

    }
}
