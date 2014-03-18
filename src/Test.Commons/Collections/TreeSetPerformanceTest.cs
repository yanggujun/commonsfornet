// Copyright CommonsForNET. Author: Gujun Yang. email: gujun.yang@gmail.com
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

using NUnit.Framework;
using Commons.Collections;

namespace Test.Commons.Collections
{
    [TestFixture]
    public class TreeSetPerformanceTest
    {
        [Test]
        public void TestTreeSetInsert()
        {
            Insert(100000);
        }

        private void Insert(int count)
        {
            Random value = new Random((int)DateTime.Now.Ticks & 0x0000ffff);
            List<int> list = new List<int>();
            for (int i = 0; i < count; i++)
            {
                list.Add(value.Next());
            }
            SortedSet<int> sortedSet = new SortedSet<int>();
            var result2 = DoInsert(sortedSet, list);
            Console.WriteLine("Sorted set: " + result2);
            LlrbTreeSet<int> set = new LlrbTreeSet<int>();
            var result = DoInsert(set, list);
            Console.WriteLine("Tree set: " + result);

            int removeCount = count / 10;
            List<int> removeList = new List<int>();
            for (int i = 0; i < removeCount; i++)
            {
                var index = value.Next() % count;
                removeList.Add(list[index]);
            }

            result2 = DoRemove(sortedSet, removeList);
            Console.WriteLine("Sorted set remove: " + result2);
            result = DoRemove(set, removeList);
            Console.WriteLine("Tree set remove: " + result);
        }

        private double DoInsert(ICollection<int> set, List<int> list)
        {
            DateTime start = DateTime.Now;
            foreach (var v in list)
            {
                if (!set.Contains(v))
                {
                    set.Add(v);
                }
            }
            TimeSpan span = DateTime.Now - start;
            return span.TotalMilliseconds;
        }

        private double DoRemove(ICollection<int> set, List<int> list)
        {
            DateTime start = DateTime.Now;
            foreach (var v in list)
            {
                if (set.Contains(v))
                {
                    set.Remove(v);
                }
            }
            TimeSpan span = DateTime.Now - start;
            return span.TotalMilliseconds;
        }
    }
}
