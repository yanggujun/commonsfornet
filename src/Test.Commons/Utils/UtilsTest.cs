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

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Commons.Utils;
using Xunit;

namespace Test.Commons.Utils
{
    public class UtilsTest
    {
        [Fact]
        public void TestAtomicBool()
        {
            var b = AtomicBool.From(true);
            Assert.True(b.Value);
            Assert.True(b.CompareExchange(false, true));
            Assert.False(b.Value);
            Assert.True(b.Exchange(true));
            Assert.True(b.Value);
            Assert.True(b);
        }

        [Fact]
        public void TestAtomicBoolDefaultCtor()
        {
            var ab = new AtomicBool();
            Assert.False(ab.Value);
        }

        [Fact]
        public void TestAtomicDouble()
        {
            var d = AtomicDouble.From(0.1);
            Assert.Equal(0.1, d.Value);
            Assert.True(d.CompareExchange(0.2, 0.1));
            Assert.Equal(0.2, d.Value);
            Assert.True(d.Exchange(0.3));
            Assert.Equal(0.3, d.Value);
            Assert.Equal(0.3, d);
        }

        [Fact]
        public void TestAtomicDoubleDefaultCtor()
        {
            var ad = new AtomicDouble();
            Assert.Equal(0.0, ad.Value);
        }

        [Fact]
        public void TestAtomicFloat()
        {
            var d = AtomicFloat.From(0.1f);
            Assert.Equal(0.1f, d.Value);
            Assert.True(d.CompareExchange(0.2f, 0.1f));
            Assert.Equal(0.2f, d.Value);
            Assert.True(d.Exchange(0.3f));
            Assert.Equal(0.3f, d.Value);
            Assert.Equal(0.3f, d);
        }

        [Fact]
        public void TestAtomicFloatDefaultCtor()
        {
            var af = new AtomicFloat();
            Assert.Equal(0.0f, af.Value);
        }

        [Fact]
        public void TestAtomicInt32()
        {
            var d = AtomicFloat.From(10);
            Assert.Equal(10, d.Value);
            Assert.True(d.CompareExchange(20, 10));
            Assert.Equal(20, d.Value);
            Assert.True(d.Exchange(30));
            Assert.Equal(30, d.Value);
            Assert.Equal(30, (int)d);
        }

        [Fact]
        public void TestAtomicInt32DefaultCtor()
        {
            //var ai = new AtomicInt32();
            //Assert.Equal(0, ai.Value);
        }

        [Fact]
        public void TestAtomicInt64()
        {
            var d = AtomicFloat.From(10);
            Assert.Equal(10, d.Value);
            Assert.True(d.CompareExchange(20, 10));
            Assert.Equal(20, d.Value);
            Assert.True(d.Exchange(30));
            Assert.Equal(30, d.Value);
            Assert.Equal(30, (long)d);
        }

        [Fact]
        public void TestAtomicInt64DefaultCtor()
        {
            var al = new AtomicInt64();
            Assert.Equal(0, al.Value);
        }

        [Fact]
        public void TestAtomicReference()
        {
            var o = new Order { Id = 1, Name = "1"};
            var r = AtomicReference<Order>.From(o);
            Assert.Same(o, r.Value);
            Assert.Equal(1, r.Value.Id);
            Assert.Equal("1", r.Value.Name);
        }

        [Fact]
        public void TestAtomicReferenceDefaultCtor()
        {
            var ar = new AtomicReference<Order>();
            Assert.Null(ar.Value);
            var newOrder = new Order {Id = 2, Name = "2"};
            Assert.True(ar.CompareExchange(newOrder, null));
            Assert.Same(newOrder, ar.Value);
        }

        [Fact]
        public void TestAtomicReferenceCas()
        {
            var o = new Order{ Id = 1, Name = "1"};
            var r = AtomicReference<Order>.From(o);

            var n = new Order { Id = 2, Name = "2" };
            Assert.True(r.CompareExchange(n, o));
            Assert.Same(n, r.Value);
            Assert.Equal(2, r.Value.Id);
            Assert.Equal("2", r.Value.Name);
        }

        [Fact]
        public void TestAtomicReferenceExchange()
        {
            var o = new Order{ Id = 1, Name = "1"};
            var r = AtomicReference<Order>.From(o);
            var n1 = new Order { Id = 3, Name = "3" };
            Assert.True(r.Exchange(n1));
            Assert.Same(n1, r.Value);
            Assert.Equal(3, r.Value.Id);
            Assert.Equal("3", r.Value.Name);
            var c = (Order)r;
            Assert.Equal(3, c.Id);
            Assert.Equal("3", c.Name);
        }

        [Fact]
        public void TestAmrCasWithSameMark()
        {
            var order = new Order { Id = 1, Name = "1" };
            var amr = new AtomicMarkableReference<Order>(order);
            Assert.Same(order, amr.Value);
            Assert.Equal(1, amr.Value.Id);
            Assert.Equal("1", amr.Value.Name);
            Assert.False(amr.IsMarked);

            var newOrder = new Order { Id = 2, Name = "2" };
            Assert.True(amr.CompareExchange(order, false, newOrder, false));
            Assert.Same(newOrder, amr.Value);
            Assert.Equal(2, amr.Value.Id);
            Assert.Equal("2", amr.Value.Name);
        }

        [Fact]
        public void TestAmrCasWithDifferentMark()
        {
            var order = new Order { Id = 1, Name = "1" };
            var amr = new AtomicMarkableReference<Order>(order, false);
            Assert.Same(order, amr.Value);
            Assert.Equal(1, amr.Value.Id);
            Assert.Equal("1", amr.Value.Name);
            Assert.False(amr.IsMarked);

            var newOrder = new Order { Id = 2, Name = "2" };
            Assert.True(amr.CompareExchange(order, false, newOrder, true));
            Assert.Same(newOrder, amr.Value);
            Assert.Equal(2, amr.Value.Id);
            Assert.Equal("2", amr.Value.Name);
            Assert.True(amr.IsMarked);
        }

        [Fact]
        public void TestAmrExchangeWithSameMark()
        {
            var order = new Order { Id = 1, Name = "1" };
            var amr = new AtomicMarkableReference<Order>(order, false);

            var newOrder = new Order { Id = 2, Name = "2" };
            Assert.True(amr.Exchange(newOrder, false));
            Assert.Same(newOrder, amr.Value);
            Assert.False(amr.IsMarked);
        }

        [Fact]
        public void TestAmrExchangeWithDifferentMark()
        {
            var order = new Order { Id = 1, Name = "1" };
            var amr = new AtomicMarkableReference<Order>(order, true);
            Assert.True(amr.IsMarked);

            var newOrder = new Order { Id = 2, Name = "2" };
            Assert.True(amr.Exchange(newOrder, false));
            Assert.Same(newOrder, amr.Value);
            Assert.False(amr.IsMarked);
        }

        [Fact]
        public void TestAmrTryGetValue()
        {
            var order = new Order { Id = 1, Name = "1" };
            var amr = new AtomicMarkableReference<Order>(order, false);
            bool isMarked = true;
            var theOrder = amr.TryGetValue(out isMarked);
            Assert.False(isMarked);
            Assert.Same(order, theOrder);
        }

        [Fact]
        public void TestAtomicInt32OperatorIncrement()
        {
            for (var j = 0; j < 10; j++)
            {
                var atomic = AtomicInt32.From(0);
                var x = 0;
                var tasks = new Task[100];
                for (var i = 0; i < 100; i++)
                {
                    tasks[i] = Task.Factory.StartNew(() => atomic.Increment());
                }
                Task.WaitAll(tasks);
                Assert.Equal(100, atomic.Value);
            }
        }

        [Fact]
        public void TestAtomicInt32OperatorDecrement()
        {
            for (var j = 0; j < 10; j++)
            {
                var x = AtomicInt32.From(100);
                var tasks = new Task[100];
                for (var i = 0; i < 100; i++)
                {
                    tasks[i] = Task.Factory.StartNew(() => x.Decrement());
                }
                Task.WaitAll(tasks);
                Assert.Equal(0, x.Value);
            }
        }

        [Fact]
        public void TestAtomicInt64OperatorIncrement()
        {
            for (var j = 0; j < 10; j++)
            {
                var atomic = AtomicInt64.From(0);
                var x = 0;
                var tasks = new Task[100];
                for (var i = 0; i < 100; i++)
                {
                    tasks[i] = Task.Factory.StartNew(() => atomic.Increment());
                }
                Task.WaitAll(tasks);
                Assert.Equal(100, atomic.Value);
            }
        }

        [Fact]
        public void TestAtomicInt64OperatorDecrement()
        {
            for (var j = 0; j < 10; j++)
            {
                var x = AtomicInt64.From(100);
                var tasks = new Task[100];
                for (var i = 0; i < 100; i++)
                {
                    tasks[i] = Task.Factory.StartNew(() => x.Decrement());
                }
                Task.WaitAll(tasks);
                Assert.Equal(0, x.Value);
            }
        }

    }
}
