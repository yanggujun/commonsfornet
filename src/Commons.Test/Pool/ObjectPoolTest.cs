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
using System.Data;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Commons.Collections.Queue;
using Commons.Pool;
using Moq;
using Xunit;

namespace Commons.Test.Pool
{
    public class ObjectPoolTest
    {    
        private static readonly object locker = new object();
        private MockConnectionFactory mockObjFactory;

        [Fact]
        public void TestNormalAcquirePoolNotFull()
        {
            Setup();
            var poolManager = new PoolManager();
            var objectPool = poolManager.NewPool<IDbConnection>()
                                        .InitialSize(0)
                                        .MaxSize(100)
                                        .WithFactory(mockObjFactory)
                                        .Instance();
            var tasks = new Task[50];
            var connections = new List<IDbConnection>();
            for (var i = 0; i < 50; i++)
            {
                 tasks[i] = new Task(() => Do(connections, objectPool));
            }

            Parallel.ForEach(tasks, x => x.Start());
            Task.WaitAll(tasks);

            Assert.Equal(50, connections.Count);
            Assert.Equal(0, objectPool.IdleCount);
            Assert.Equal(50, objectPool.ActiveCount);
            Assert.Equal(0, objectPool.InitialSize);
            Assert.Equal(100, objectPool.Capacity);

            var returnTasks = new Task[connections.Count];
            for (var i = 0; i < connections.Count; i++)
            {
                var conn = connections[i];
                returnTasks[i] = new Task(() => objectPool.Return(conn)); 
            }

            Parallel.ForEach(returnTasks, x => x.Start());
            Task.WaitAll(returnTasks);

            Assert.Equal(50, objectPool.IdleCount);
            Assert.Equal(0, objectPool.ActiveCount);
            Assert.Equal(0, objectPool.InitialSize);
            Assert.Equal(100, objectPool.Capacity);
            objectPool.Dispose();
        }

        [Fact]
        public void TestAcquireAgain()
        {
            Setup();
            var poolManager = new PoolManager();
            var objectPool = poolManager.NewPool<IDbConnection>()
                                        .InitialSize(0)
                                        .MaxSize(100)
                                        .WithFactory(mockObjFactory)
                                        .Instance();

            var tasks1 = new Task[30];
            var connections = new List<IDbConnection>();
            for (var i = 0; i < 30; i++)
            {
                tasks1[i] = new Task(() => Do(connections, objectPool));
            }
            Parallel.ForEach(tasks1, x => x.Start());
            Task.WaitAll(tasks1);

            Assert.Equal(0, objectPool.IdleCount);
            Assert.Equal(30, objectPool.ActiveCount);
            Assert.Equal(0, objectPool.InitialSize);
            Assert.Equal(100, objectPool.Capacity);

            var tasks2 = new Task[30];
            for (var i = 0; i < 30; i++)
            {
                tasks2[i] = new Task(() => Do(connections, objectPool));
            }
            Parallel.ForEach(tasks2, x => x.Start());
            Task.WaitAll(tasks2);

            Assert.Equal(0, objectPool.IdleCount);
            Assert.Equal(60, objectPool.ActiveCount);


            var tasks3 = new Task[40];
            for (var i = 0; i < 40; i++)
            {
                var conn = connections[i];
                tasks3[i] = new Task(() => objectPool.Return(conn));
            }
            Parallel.ForEach(tasks3, x => x.Start());
            Task.WaitAll(tasks3);

            Assert.Equal(40, objectPool.IdleCount);
            Assert.Equal(20, objectPool.ActiveCount);

            for (var i = 0; i < 20; i++)
            {
                objectPool.Return(connections[i + 40]);
            }
            Assert.Equal(60, objectPool.IdleCount);
            Assert.Equal(0, objectPool.ActiveCount);
            objectPool.Dispose();
        }

        [Fact]
        public void TestAcquireAndReturnAtTheSameTime()
        {
            Setup();
            for (var j = 0; j < 10; j++)
            {
            var poolManager = new PoolManager();
            var objectPool = poolManager.NewPool<IDbConnection>()
                                        .InitialSize(0)
                                        .MaxSize(100)
                                        .WithFactory(mockObjFactory)
                                        .Instance();
                var tasks1 = new Task[60];
                var connections = new List<IDbConnection>();
                for (var i = 0; i < 60; i++)
                {
                    tasks1[i] = new Task(() => Do(connections, objectPool));
                }

                Parallel.ForEach(tasks1, x => x.Start());
                Task.WaitAll(tasks1);

                var tasks2 = new Task[40];
                for (var i = 0; i < 20; i++)
                {
                    tasks2[i] = new Task(() => Do(connections, objectPool));
                }
                for (var i = 20; i < 40; i++)
                {
                    var conn = connections[i - 20];
                    tasks2[i] = new Task(() => objectPool.Return(conn));
                }

                Parallel.ForEach(tasks2, x => x.Start());
                Task.WaitAll(tasks2);

                Assert.True(objectPool.IdleCount >= 0);
                Assert.True(objectPool.IdleCount <= 20);
                Assert.True(objectPool.ActiveCount == 60);
            }
        }

        [Fact]
        public void TestAcquireExceedsCapacity()
        {
            Setup();
            for (var j = 0; j < 10; j++)
            {
            var poolManager = new PoolManager();
            var connectionPool = poolManager.NewPool<IDbConnection>()
                                        .InitialSize(0)
                                        .MaxSize(10)
                                        .WithFactory(mockObjFactory)
                                        .Instance();
                var connectTasks = new Task[200];
                var results = new List<bool>();
                for (var i = 0; i < 200; i++)
                {
                    connectTasks[i] = new Task(() =>
                    {
                        try
                        {
                            var spin = new SpinWait();
                            IDbConnection connection = null;
                            while (!connectionPool.TryAcquire(1000, out connection))
                            {
                                spin.SpinOnce();
                            }

                            connection.Open();
                            connectionPool.Return(connection);
                            lock (locker)
                            {
                                results.Add(true);
                            }
                        }
                        catch
                        {
                            lock (locker)
                            {
                                results.Add(false);
                            }
                        }
                    });
                }
                Parallel.ForEach(connectTasks, x => x.Start());
                Task.WaitAll(connectTasks);

                Assert.Equal(200, results.Count);
                foreach (var r in results)
                {
                    Assert.True(r);
                }
                Assert.True(connectionPool.IdleCount <= 10);
                Assert.Equal(0, connectionPool.ActiveCount);
                Assert.Equal(10, connectionPool.Capacity);
            }
        }

        [Fact]
        public void TestAcquireTimeout()
        {
            Setup();
            var poolManager = new PoolManager();
            var connectionPool =
                poolManager.NewPool<IDbConnection>().InitialSize(0).MaxSize(10).WithFactory(mockObjFactory).Instance();
            OperateOnPool(connectionPool);
            Assert.Equal(0, connectionPool.IdleCount);
            Assert.Equal(10, connectionPool.ActiveCount);
            IDbConnection anotherConnection = null;
            var result = connectionPool.TryAcquire(1000, out anotherConnection);
            Assert.False(result);
            Assert.Null(anotherConnection);
        }

        [Fact]
        public void TestPoolInitialSize()
        {
            Setup();
            var poolManager = new PoolManager();
            var pool =
                poolManager.NewPool<IDbConnection>()
                    .InitialSize(5)
                    .MaxSize(10)
                    .WithFactory(new MockConnectionFactory())
                    .Instance();
            OperateOnPool(pool);
            Assert.Equal(5, pool.InitialSize);
            Assert.Equal(10, pool.ActiveCount);
        }

        [Fact]
        public void TestAcquireImmediatelyFailure()
        {
            Setup();
            var poolManager = new PoolManager();
            var pool =
                poolManager.NewPool<IDbConnection>()
                    .InitialSize(5)
                    .MaxSize(10)
                    .WithFactory(new MockConnectionFactory())
                    .Instance();
            var tasks = new Task[10];
            var connections = new List<IDbConnection>();
            for (var i = 0; i < 10; i++)
            {
                tasks[i] = new Task(() =>
                {
                    var connection = pool.Acquire();
                    Assert.NotNull(connection);
                    lock (locker)
                    {
                        connections.Add(connection);
                    }
                    connection.Open();
                });
            }
            Parallel.ForEach(tasks, x => x.Start());
            Task.WaitAll(tasks);
            IDbConnection another = null;
            var result = pool.TryAcquire(0, out another);
            Assert.False(result);
            Assert.Null(another);
        }

        [Fact]
        public void TestSequenceOperations()
        {
            Setup();
            var poolManager = new PoolManager();
            var pool =
                poolManager.NewPool<IDbConnection>()
                    .InitialSize(5)
                    .MaxSize(10)
                    .WithFactory(new MockConnectionFactory())
                    .Instance();
            var connections = new List<IDbConnection>();
            for (var i = 0; i < 10; i++)
            {
                connections.Add(pool.Acquire());
            }
            Assert.Equal(10, connections.Count);
            Assert.Equal(0, pool.IdleCount);
            Assert.Equal(10, pool.ActiveCount);
            Assert.Equal(10, pool.Capacity);
            foreach (var connection in connections)
            {
                pool.Return(connection);
            }
            Assert.Equal(10, pool.IdleCount);
            Assert.Equal(0, pool.ActiveCount);
            Assert.Equal(10, pool.Capacity);
        }

        [Fact]
        public void TestHungrySituation()
        {
            Setup();
            var poolManager = new PoolManager();
            var pool =
                poolManager.NewPool<IDbConnection>()
                    .InitialSize(0)
                    .MaxSize(20)
                    .WithFactory(new MockConnectionFactory())
                    .Instance();
            var connectTasks = new Task[200];
            var results = new List<bool>();
            var connections = new List<IDbConnection>();
            for (var i = 0; i < 200; i++)
            {
                connectTasks[i] = new Task(() =>
                {
                    IDbConnection conn = null;
                    var result = pool.TryAcquire(50, out conn);
                    lock (locker)
                    {
                        results.Add(result);
                    }
                    if (result)
                    {
                        Assert.NotNull(conn);
                        lock (locker)
                        {
                            connections.Add(conn);
                        }
                        conn.Open();

                    }
                });
            }
            Parallel.ForEach(connectTasks, x => x.Start());
            Task.WaitAll(connectTasks);
            Assert.Equal(0, pool.IdleCount);
            Assert.Equal(20, pool.ActiveCount);
            var acquiredCount = 0;
            foreach (var r in results)
            {
                if (r)
                {
                    acquiredCount++;
                }
            }
            Assert.Equal(20, acquiredCount);

            results.Clear();
            var mixedTasks = new Task[120];
            for (var i = 0; i < 20; i++)
            {
                var conn = connections[i];
                mixedTasks[i] = new Task(() => pool.Return(conn));
            }
            connections.Clear();
            for (var i = 0; i < 100; i++)
            {
                mixedTasks[i + 20] = new Task(() =>
                    {
                        IDbConnection conn = null;
                        var result = pool.TryAcquire(50, out conn);
                        results.Add(result);
                        if (result)
                        {
                            Assert.NotNull(conn);
                            lock (locker)
                            {
                                connections.Add(conn);
                            }
                            conn.Open();
                        }
                    });
            }
            Parallel.ForEach(mixedTasks, x => x.Start());
            Task.WaitAll(mixedTasks);
            Assert.Equal(20, connections.Count);
            Assert.Equal(20, pool.ActiveCount);
            Assert.Equal(0, pool.IdleCount);
            acquiredCount = 0;
            foreach (var r in results)
            {
                if (r)
                {
                    acquiredCount++;
                }
            }
            Assert.Equal(20, acquiredCount);
        }

        [Fact]
        public void TestInitialSizeLessThanZero()
        {
            Setup();
            Assert.Throws(typeof (ArgumentException), () =>
            {
                var poolManager = new PoolManager();
                var pool =
                    poolManager.NewPool<IDbConnection>()
                        .InitialSize(-1)
                        .MaxSize(10)
                        .WithFactory(new MockConnectionFactory())
                        .Instance();
            });
        }

        [Fact]
        public void TestMaxSizeLessThanZero()
        {
            Setup();
            Assert.Throws(typeof (ArgumentException),
                () =>
                    new PoolManager().NewPool<IDbConnection>()
                        .InitialSize(0)
                        .MaxSize(-100)
                        .WithFactory(new MockConnectionFactory())
                        .Instance());
        }

        [Fact]
        public void TestMaxSizeLessThanInitialSize()
        {
            Setup();
            Assert.Throws((typeof (ArgumentException)),
                () =>
                    new PoolManager().NewPool<IDbConnection>()
                        .InitialSize(10)
                        .MaxSize(1)
                        .WithFactory(new MockConnectionFactory())
                        .Instance());
        }

        [Fact]
        public void TestReturnObjectNotBelongToThePool()
        {
            Setup();
            var poolManager = new PoolManager();
            var pool = poolManager.NewPool<IDbConnection>().InitialSize(0).MaxSize(10).WithFactory(mockObjFactory).Instance();
            var tasks = new Task[4];
            for (var i = 0; i < 4; i++)
            {
                tasks[i] = new Task(() =>
                {
                    var connection = pool.Acquire();
                    connection.Open();
                });
            }
            Parallel.ForEach(tasks, x => x.Start());
            Task.WaitAll(tasks);
            var fake = new Mock<IDbConnection>().Object;
            Assert.Throws(typeof (InvalidOperationException), () => pool.Return(fake));
        }

        [Fact]
        public void TestReturnSameObjectMoreThanOnce()
        {
            Setup();
            var pool =
                new PoolManager().NewPool<IDbConnection>()
                    .InitialSize(10)
                    .MaxSize(20)
                    .WithFactory(new MockConnectionFactory())
                    .Instance();
            var connectTasks = new Task[20];
            var connections = new List<IDbConnection>();
            for (var i = 0; i < 20; i++)
            {
                connectTasks[i] = new Task(() =>
                {
                    var connection = pool.Acquire();
                    lock (locker)
                    {
                        connections.Add(connection);
                    }
                    connection.Open();
                });
            }
            Parallel.ForEach(connectTasks, x => x.Start());
            Task.WaitAll(connectTasks);

            var returnTasks = new Task[22];
            for (var i = 0; i < 19; i++)
            {
                var connection = connections[i];
                returnTasks[i] = new Task(() =>
                {
                    pool.Return((connection));
                });
            }

            var last = connections[19];
            var results = new List<bool>();
            for (var i = 0; i < 3; i++)
            {
                returnTasks[i + 19] = new Task(() =>
                {
                    try
                    {
                        pool.Return(last);
                        lock (locker)
                        {
                            results.Add(true);
                        }
                    }
                    catch (InvalidOperationException)
                    {
                        lock (locker)
                        {
                            results.Add(false);
                        }
                    }
                });
            }
            Parallel.ForEach(returnTasks, x => x.Start());
            Task.WaitAll(returnTasks);
            var returnedCount = 0;
            var failureCount = 0;
            foreach (var r in results)
            {
                if (r)
                {
                    returnedCount++;
                }
                else
                {
                    failureCount++;
                }
            }

            Assert.Equal(1, returnedCount);
            Assert.Equal(2, failureCount);
        }

        [Fact]
        public void TestAcquireAlways()
        {
            Setup();
            var pool =
                new PoolManager().NewPool<IDbConnection>()
                    .InitialSize(0)
                    .MaxSize(10)
                    .WithFactory(new MockConnectionFactory())
                    .Instance();
            var connectTasks = new Task[10];
            var connections = new List<IDbConnection>();
            for (var i = 0; i < connectTasks.Length; i++)
            {
                connectTasks[i] = new Task(() =>
                {
                    var connection = pool.Acquire();
                    lock (locker)
                    {
                        connections.Add(connection);
                    }
                    connection.Open();
                });
            }
            Parallel.ForEach(connectTasks, x => x.Start());
            Task.WaitAll(connectTasks);

            var newConnectTasks = new Task[2];
            var newConnections = new List<IDbConnection>();
            for (var i = 0; i < newConnectTasks.Length; i++)
            {
                newConnectTasks[i] = new Task(() =>
                {
                    var connection = pool.Acquire();
                    lock (locker)
                    {
                        newConnections.Add(connection);
                    }
                    connection.Open();
                });
            }
            Parallel.ForEach(newConnectTasks, x => x.Start());
            Thread.Sleep(1000);
            var connection0 = connections[0];
            var connection1 = connections[1];
            Task.Factory.StartNew(() => pool.Return(connection0));
            Task.Factory.StartNew(() => pool.Return(connection1));
            Task.WaitAll(newConnectTasks);
            Assert.Equal(0, pool.IdleCount);
            Assert.Equal(10, pool.ActiveCount);
            var acquired0 = false;
            var acquired1 = false;
            foreach (var c in newConnections)
            {
                if (ReferenceEquals(c, connection0))
                {
                    acquired0 = true;
                }
                if (ReferenceEquals(c, connection1))
                {
                    acquired1 = true;
                }
            }
            Assert.True(acquired0);
            Assert.True(acquired1);
        }

        [Fact]
        public void TestKeyedObjectPool()
        {
            var poolManager = new PoolManager();
            var keyedPool = poolManager.NewPool<IDbConnection>().OfKey("mock").InitialSize(0).MaxSize(10).WithCreator(() =>
            {
                var mock = new Mock<IDbConnection>();
                mock.Setup(x => x.Open()).Callback(() => Thread.Sleep(50));
                return mock.Object;
            }).Instance();

            var existingPool = poolManager.GetPool<IDbConnection>("mock");
            Assert.True(ReferenceEquals(keyedPool, existingPool));
            poolManager.Destroy("mock");
            Assert.Null(poolManager.GetPool<IDbConnection>("mock"));
        }

        [Fact]
        public void TestPoolManagerMaxSizeLessThanInitialSize()
        {
            Setup();
            var poolManager = new PoolManager();
            Assert.Throws(typeof (ArgumentException), () =>
                poolManager.NewPool<IDbConnection>().MaxSize(1).InitialSize(5).WithFactory(mockObjFactory).Instance());
        }

        [Fact]
        public void TestWithoutFactory()
        {
            var poolManager = new PoolManager();
            Assert.Throws(typeof (InvalidOperationException), () =>
                poolManager.NewPool<IDbConnection>().InitialSize(0).MaxSize(10).Instance());
        }

        [Fact]
        public void TestWithCreator()
        {
            Setup();
            var poolManager = new PoolManager();
            var pool = poolManager.NewPool<IDbConnection>().InitialSize(0).MaxSize(10).WithCreator(() =>
            {
                var mock = new Mock<IDbConnection>();
                mock.Setup(x => x.State).Returns(ConnectionState.Open);
                return mock.Object;
            }).Instance();
            var connection = pool.Acquire();
            Assert.Equal(connection.State, ConnectionState.Open);
        }

        [Fact]
        public void TestFactoryOverrideCreator()
        {
            Setup();
            var poolManager = new PoolManager();
            var pool = poolManager.NewPool<IDbConnection>().InitialSize(0).MaxSize(10).WithCreator(() =>
            {
                var mock = new Mock<IDbConnection>();
                mock.Setup(x => x.State).Returns(ConnectionState.Open);
                return mock.Object;
            }).WithFactory(mockObjFactory).Instance();
            var connection = pool.Acquire();
            Assert.Equal(connection.State, ConnectionState.Executing);
        }

        [Fact]
        public void TestDestroy()
        {
            var poolManager = new PoolManager();
            var mock = new Mock<IDbConnection>();
            var pool = poolManager.NewPool<IDbConnection>().OfKey("mock").InitialSize(0).MaxSize(10).WithCreator(() =>
            {
                mock.Setup(x => x.State).Returns(ConnectionState.Open);
                return mock.Object;
            }).WithDesctroyer(x => x.Dispose()).Instance();
            var connection = pool.Acquire();
            Assert.NotNull(poolManager.GetPool<IDbConnection>("mock"));
            poolManager.Destroy("mock");
            Assert.Null(poolManager.GetPool<IDbConnection>("mock"));
            mock.Verify(x => x.Dispose());
        }

        [Fact]
        public void TestPoolManagerAddPoolWithExistingKey()
        {
            Setup();
            var poolManager = new PoolManager();
            var onePool =
                poolManager.NewPool<IDbConnection>()
                    .OfKey("mock")
                    .InitialSize(0)
                    .MaxSize(10)
                    .WithFactory(mockObjFactory)
                    .Instance();
            Assert.Throws(typeof (InvalidOperationException),
                () =>
                    poolManager.NewPool<IDbConnection>()
                        .OfKey("mock")
                        .InitialSize(5)
                        .MaxSize(20)
                        .WithFactory(mockObjFactory)
                        .Instance());
        }

        [Fact]
        public void TestPoolManagerDestroyInvalidKey()
        {
            Setup();
            var poolManager = new PoolManager();
            var pool =
                poolManager.NewPool<IDbConnection>()
                    .OfKey("mock")
                    .InitialSize(0)
                    .MaxSize(10)
                    .WithFactory(mockObjFactory)
                    .Instance();
            Assert.Throws(typeof (ArgumentException), () => poolManager.Destroy("mock1"));
        }

        [Fact]
        public void TestPoolManagerDefaultValue()
        {
            Setup();
            var poolManager = new PoolManager();
            var pool = poolManager.NewPool<IDbConnection>().WithFactory(mockObjFactory).Instance();
            Assert.Equal(0, pool.InitialSize);
            Assert.Equal(-1, pool.Capacity);
        }

        [Fact]
        public void TestPoolManagerConcurrentCreatePool()
        {
            Setup();
            var poolManager = new PoolManager();
            var tasks = new Task[10];
            for (var i = 0; i < 10; i++)
            {
                var id = i;
                tasks[i] =
                    Task.Factory.StartNew(
                        () =>
                            poolManager.NewPool<IDbConnection>()
                                .OfKey("mock" + id)
                                .InitialSize(0)
                                .MaxSize(id + 10)
                                .WithFactory(mockObjFactory)
                                .Instance());
            }
            Task.WaitAll(tasks);
            for (var i = 0; i < 10; i++)
            {
                var pool = poolManager.GetPool<IDbConnection>("mock" + i);
                Assert.Equal(i + 10, pool.Capacity);
            }
        }

        [Fact]
        public void TestPoolManagerDestroyEmptyKey()
        {
            var poolManager = new PoolManager();
            Assert.Throws(typeof (ArgumentException), () => poolManager.Destroy("  "));
        }

        private void OperateOnPool(IObjectPool<IDbConnection> pool)
        {
            var connectTasks = new Task[10];
            var connections = new List<IDbConnection>();
            for (var i = 0; i < 10; i++)
            {
                connectTasks[i] = new Task(() =>
                    {
                        var connection = pool.Acquire();
                        Assert.NotNull(connection);
                        lock (locker)
                        {
                            connections.Add(connection);
                        }
                    });
            }
            Parallel.ForEach(connectTasks, x => x.Start());
            Task.WaitAll(connectTasks);
        }

        private void Do(IList<IDbConnection> connections, IObjectPool<IDbConnection> objectPool)
        {
            var connection = objectPool.Acquire();
            Assert.NotNull(connection);
            lock (locker)
            {
                connections.Add(connection);
            }
            connection.Open();
        }

        private void Setup()
        {
            mockObjFactory = new MockConnectionFactory();
        }
    }

    public class MockConnectionFactory : IPooledObjectFactory<IDbConnection>
    {
        public IDbConnection Create()
        {
            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(x => x.Open()).Callback(() => Thread.Sleep(50));
            mockConnection.Setup(x => x.State).Returns(ConnectionState.Executing);
            return mockConnection.Object;
        }

        public void Destroy(IDbConnection obj)
        {
        }
    }
}
