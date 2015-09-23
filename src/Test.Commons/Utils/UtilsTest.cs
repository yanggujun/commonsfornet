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
using Commons.Utils;
using Xunit;

namespace Test.Commons.Utils
{
    public class UtilsTest
    {
        [Fact]
        public void TestAtomicReferenceCompareExchange()
        {
            var order = new Order();
            var atomic = Atomic<Order>.From(order);
            var newOrder = new Order();
            Assert.True(atomic.CompareExchange(newOrder));
            Assert.True(ReferenceEquals(atomic.Value, newOrder));
        }

        [Fact]
        public void TestAtomicReferenceExchange()
        {
            var order = new Order();
            var atomic = Atomic<Order>.From(order);
            var newOrder = new Order();
            Assert.True(atomic.Exchange(newOrder));
            Assert.True(ReferenceEquals(atomic.Value, newOrder));
        }

        [Fact]
        public void TestAtomicStructWithDefaultConstructor()
        {
            var order = new Atomic<Order>();
            var newOrder = new Order();
            var result = order.CompareExchange(newOrder);
            Assert.True(result);
            Assert.True(ReferenceEquals(newOrder, order.Value));
        }

        [Fact]
        public void TestAtomicNullArgument()
        {
            var order = Atomic<Order>.From(new Order());
            var result = order.CompareExchange(null);
            Assert.True(result);
            Assert.Equal(null, order.Value);
        }

        [Fact]
        public void TestAtomicNullReference()
        {
            var order = Atomic<Order>.From(null);
            var result = order.CompareExchange(new Order());
            Assert.True(result);
        }
    }
}
