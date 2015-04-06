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
            Assert.Throws(typeof(InvalidOperationException), () => order.CompareExchange(newOrder));
            Assert.Throws(typeof(InvalidOperationException), () => order.Exchange(newOrder));
        }

        [Fact]
        public void TestAtomicNullArgument()
        {
            var order = Atomic<Order>.From(new Order());
            Assert.Throws(typeof(ArgumentNullException), () => order.CompareExchange(null));
            Assert.Throws(typeof(ArgumentNullException), () => order.Exchange(null));
        }
    }
}
