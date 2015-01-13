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

using Xunit;

using Commons.Collections.Queue;
using System;
using Commons.Collections.Set;

namespace Test.Commons.Collections
{
    public class PriorityQueueTest
    {
        [Fact]
        public void TestPriortyQueueConstructor()
        {
            var pq = new MinPriorityQueue<int>();
            var rand = new Random((int)DateTime.Now.Ticks & 0x0000ffff);
            var set = new TreeSet<int>();
            var count = 0; 
            while (count < 10000)
            {
                var next = rand.Next() % 10000;
                if (!set.Contains(next))
                {
                    pq.Push(next);
                    set.Add(next);
                    count++;
                }
            }
            for (var i = 0; i < 1000; i++)
            {
                Assert.Equal(set.Min, pq.Top);
                Assert.Equal(set.Min, pq.Pop());
                set.RemoveMin();
            }
        }

        [Fact]
        public void SimpleTest()
        {
            var pq = new MinPriorityQueue<int>();
            for (var i = 0; i < 20; i++)
            {
                pq.Push(20 - i - 1);
            }

            for (var i = 0; i < 20; i++)
            {
                Assert.Equal(i, pq.Top);
                Assert.Equal(i, pq.Pop());
				Assert.Equal(20 - i - 1, pq.Count);
            }
        }
    }
}
