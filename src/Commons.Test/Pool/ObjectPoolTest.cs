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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using Moq;
using Xunit;

using Commons.Pool;

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
            DoTestAcquireAgain(objectPool);
        }


        [Fact]
        public void TestAcquireAgainWithFluentConfig()
        {
            Setup();
            var poolManager = new PoolManager();
            var objectPool = poolManager.NewPool<IDbConnection>(x => 
                {
                    x.InitialSize = 0;
                    x.MaxSize = 100;
                    x.ByFactory(mockObjFactory);
                });
            DoTestAcquireAgain(objectPool);
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
            Assert.Throws<ArgumentException>(() =>
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
            Assert.Throws<ArgumentException>(
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
            Assert.Throws<ArgumentException>(
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
            Assert.Throws<InvalidOperationException>(() => pool.Return(fake));
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
            Assert.Throws<ArgumentException>(
                () => poolManager.NewPool<IDbConnection>().MaxSize(1).InitialSize(5).WithFactory(mockObjFactory).Instance());
        }

        [Fact]
        public void TestWithoutFactory()
        {
            var poolManager = new PoolManager();
            Assert.Throws<InvalidOperationException>(
                () => poolManager.NewPool<IDbConnection>().InitialSize(0).MaxSize(10).Instance());
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
            Assert.Equal(ConnectionState.Open, connection.State);
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
            Assert.Equal(ConnectionState.Executing, connection.State);
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
            }).WithDestroyer(x => x.Dispose()).Instance();
            var connection = pool.Acquire();
            Assert.NotNull(poolManager.GetPool<IDbConnection>("mock"));
            poolManager.Destroy("mock");
            Assert.Null(poolManager.GetPool<IDbConnection>("mock"));
            mock.Verify(x => x.Dispose());
        }

        [Fact]
        public void TestDestroyWithFluentConfig()
        {
            var poolManager = new PoolManager();
            var mock = new Mock<IDbConnection>();
            var pool = poolManager.NewPool<IDbConnection>(x =>
            {
                x.Key = "mock";
                x.InitialSize = 0;
                x.MaxSize = 10;
                x.CreateWith(() =>
                {
                    mock.Setup(y => y.State).Returns(ConnectionState.Open);
                    return mock.Object;
                });
                x.DestroyWith(y => y.Dispose());
            });
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
            Assert.Throws<ArgumentException>(() => poolManager.Destroy("mock1"));
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
            Assert.Throws<ArgumentException>(() => poolManager.Destroy("  "));
        }

        [Fact]
        public void TestInvalidateOutsider()
        {
            using (var poolManager = new PoolManager())
            {
                var pool = poolManager.NewPool<string>()
                .OfKey("invalidatorPool")
                .InitialSize(0)
                .MaxSize(5)
                .WithCreator(() => "Hello World")
                .WithDestroyer((obj) => { /* No-Op*/ })
                .Instance();

                Assert.Throws<InvalidOperationException>(() => pool.Invalidate("Outsider"));
            }
        }

        [Fact]
        public void TestInvalidateAlreadyReturnedObject()
        {
            using (var poolManager = new PoolManager())
            {
                var pool = poolManager.NewPool<string>()
                .OfKey("invalidatorPool")
                .InitialSize(0)
                .MaxSize(5)
                .WithCreator(() => Guid.NewGuid().ToString())
                .WithDestroyer((obj) => { /* No-Op*/ })
                .Instance();

                string acquiredObj = pool.Acquire();
                pool.Return(acquiredObj);
                pool.Invalidate(acquiredObj);

                Assert.Equal(0, pool.ActiveCount);
            }
        }

        [Fact]
        public void TestInvalidateNull()
        {
            using (var poolManager = new PoolManager())
            {
                var pool = poolManager.NewPool<string>()
                .OfKey("invalidatorPool")
                .InitialSize(0)
                .MaxSize(5)
                .WithCreator(() => "Hello World")
                .WithDestroyer((obj) => { /* No-Op*/ })
                .Instance();

                Assert.Throws<ArgumentNullException>(() => pool.Invalidate(null));
            }
        }

        [Fact]
        public void TestInvalidateTwice()
        {
            using (var poolManager = new PoolManager())
            {
                var pool = poolManager.NewPool<string>()
                    .OfKey("invalidatorPool")
                    .InitialSize(0)
                    .MaxSize(5)
                    .WithCreator(() => Guid.NewGuid().ToString())
                    .WithDestroyer((obj) => { /* No-Op*/ })
                    .Instance();

                string acquiredObj = pool.Acquire();
                pool.Invalidate(acquiredObj);

                Assert.Throws<InvalidOperationException>(() => pool.Invalidate(acquiredObj));
            }
        }

        [Fact]
        public void TestInvalidateWithAcquireAndReturn()
        {
            using (var poolManager = new PoolManager())
            {
                var objectPool = poolManager.NewPool<string>()
                    .InitialSize(0)
                    .MaxSize(100)
                    .WithCreator(() => Guid.NewGuid().ToString())
                    .WithDestroyer((obj) => { /* No-Op*/ })
                    .Instance();

                const int tasksCount = 50;
                var objectsBag = new ConcurrentBag<string>();
                Task[] acquireTasks = CreateAcquireAndStoreTasks(objectPool, tasksCount, objectsBag);

                Parallel.ForEach(acquireTasks, x => x.Start());
                Task.WaitAll(acquireTasks);

                Assert.Equal(tasksCount, objectsBag.Count);
                Assert.Equal(0, objectPool.IdleCount);
                Assert.Equal(tasksCount, objectPool.ActiveCount);

                var returnAndInvalidateTasks = new Task[objectsBag.Count];
                var objectsArr = objectsBag.ToArray();
                for (var i = 0; i < objectsBag.Count; i++)
                {
                    var obj = objectsArr[i];
                    returnAndInvalidateTasks[i] = (i % 2 == 0) ? new Task(() => objectPool.Return(obj)) : new Task(() => objectPool.Invalidate(obj));
                }

                Parallel.ForEach(returnAndInvalidateTasks, x => x.Start());
                Task.WaitAll(returnAndInvalidateTasks);

                Assert.Equal(25, objectPool.IdleCount);
                Assert.Equal(0, objectPool.ActiveCount);
            }
        }

        [Fact]
        public void TestNullValidatorShouldBehaveAsDefault()
        {
            using (var poolManager = new PoolManager())
            {
                var objectPool = poolManager.NewPool<string>()
                    .InitialSize(0)
                    .MaxSize(10)
                    .WithCreator(() => Guid.NewGuid().ToString())
                    .WithDestroyer((obj) => { /* No-Op*/ })
                    .WithValidator(null)
                    .Instance();


                const int tasksCount = 10;
                var objectsBag = new ConcurrentBag<string>();
                Task[] acquireTasks = CreateAcquireAndStoreTasks(objectPool, tasksCount, objectsBag);

                Parallel.ForEach(acquireTasks, x => x.Start());
                Task.WaitAll(acquireTasks);

                Assert.Equal(tasksCount, objectsBag.Count);
                Assert.Equal(0, objectPool.IdleCount);
                Assert.Equal(tasksCount, objectPool.ActiveCount);
            }
        }

        [Fact]
        public void TestNeverValidatorShouldBehaveAsDefault()
        {
            using (var poolManager = new PoolManager())
            {
                var objectPool = poolManager.NewPool<string>()
                    .InitialSize(0)
                    .MaxSize(10)
                    .WithCreator(() => Guid.NewGuid().ToString())
                    .WithDestroyer((obj) => { /* No-Op*/ })
                    .WithValidator(new NeverValidator())
                    .Instance();


                const int tasksCount = 10;
                var objectsBag = new ConcurrentBag<string>();
                Task[] acquireTasks = CreateAcquireAndStoreTasks(objectPool, tasksCount, objectsBag);

                Parallel.ForEach(acquireTasks, x => x.Start());
                Task.WaitAll(acquireTasks);

                Assert.Equal(tasksCount, objectsBag.Count);
                Assert.Equal(0, objectPool.IdleCount);
                Assert.Equal(tasksCount, objectPool.ActiveCount);
            }
        }

        [Fact]
        public void TestValidateOnAcquireExhausted()
        {
            using (var poolManager = new PoolManager())
            {
                var objectPool = poolManager.NewPool<string>()
                    .InitialSize(0)
                    .MaxSize(10)
                    .WithCreator(() => Guid.NewGuid().ToString())
                    .WithDestroyer((obj) => { /* No-Op*/ })
                    .WithValidator(new StringValidator(validateOnAcquire: true, validateOnReturn: false, validateDelegate: (obj) => false))
                    .Instance();

                var theException = Assert.Throws<InvalidOperationException>(() => objectPool.Acquire());

                const int defaultMaxAttempts = 10;
                Assert.Contains($"after {defaultMaxAttempts} attempts", theException.Message);
            }
        }

        [Fact]
        public void TestValidateOnAcquireExhaustedWithCustomLimit()
        {
            using (var poolManager = new PoolManager())
            {
                const int maxAttempts = 5;

                var objectPool = poolManager.NewPool<string>()
                    .InitialSize(0)
                    .MaxSize(10)
                    .WithCreator(() => Guid.NewGuid().ToString())
                    .WithDestroyer((obj) => { /* No-Op*/ })
                    .WithValidator(new StringValidator(validateOnAcquire: true, validateOnReturn: false, validateDelegate: (obj) => false))
                    .AcquiredInvalidLimit(maxAttempts)
                    .Instance();

                var theException = Assert.Throws<InvalidOperationException>(() => objectPool.Acquire());

                Assert.Contains($"after {maxAttempts} attempts", theException.Message);
            }
        }

        [Fact]
        public void TestValidateOnAcquireAllValid()
        {
            using (var poolManager = new PoolManager())
            {
                var objectPool = poolManager.NewPool<string>()
                    .InitialSize(0)
                    .MaxSize(50)
                    .WithCreator(() => Guid.NewGuid().ToString())
                    .WithDestroyer((obj) => { /* No-Op*/ })
                    .WithValidator(new StringValidator(validateOnAcquire: true, validateOnReturn: false, validateDelegate: (obj) => true))
                    .Instance();

                const int tasksCount = 50;
                var objectsBag = new ConcurrentBag<string>();
                Task[] acquireTasks = CreateAcquireAndStoreTasks(objectPool, tasksCount, objectsBag);

                Parallel.ForEach(acquireTasks, x => x.Start());
                Task.WaitAll(acquireTasks);

                Assert.Equal(tasksCount, objectsBag.Count);
                Assert.Equal(0, objectPool.IdleCount);
                Assert.Equal(tasksCount, objectPool.ActiveCount);
            }
        }

        [Fact]
        public void TestValidateOnAcquireValidByPattern()
        {
            using (var poolManager = new PoolManager())
            {
                const string validPattern = "^[a-z].*$";
                var objectPool = poolManager.NewPool<string>()
                    .InitialSize(0)
                    .MaxSize(50)
                    .WithCreator(() => Guid.NewGuid().ToString())
                    .WithDestroyer((obj) => { /* No-Op*/ })
                    .WithValidator(new StringValidator(validateOnAcquire: true, validateOnReturn: false, validateDelegate: (obj) =>
                        {
                            return Regex.IsMatch(obj, validPattern);
                        }))
                    .AcquiredInvalidLimit(20)
                    .Instance();

                const int tasksCount = 50;
                var acquireTasks = new Task[tasksCount];
                var objects = new ConcurrentBag<string>();
                for (var i = 0; i < tasksCount; i++)
                {
                    acquireTasks[i] = new Task(() =>
                    {
                        string obj = objectPool.Acquire();
                        objects.Add(obj);
                    });
                }

                Parallel.ForEach(acquireTasks, x => x.Start());
                Task.WaitAll(acquireTasks);

                Assert.True(objects.All(obj => Regex.IsMatch(obj, validPattern)));
            }
        }

        [Fact]
        public void TestValidateOnReturn()
        {
            using (var poolManager = new PoolManager())
            {
                const string validPattern = "^[a-z].*$";
                var objectPool = poolManager.NewPool<string>()
                    .InitialSize(0)
                    .MaxSize(50)
                    .WithCreator(() => Guid.NewGuid().ToString())
                    .WithDestroyer((obj) => { /* No-Op*/ })
                    .WithValidator(new StringValidator(validateOnAcquire: false, validateOnReturn: true, validateDelegate: (obj) =>
                    {
                        return Regex.IsMatch(obj, validPattern);
                    }))
                    .AcquiredInvalidLimit(10)
                    .Instance();

                const int tasksCount = 50;
                var objectsBag = new ConcurrentBag<string>();
                Task[] acquireTasks = CreateAcquireAndStoreTasks(objectPool, tasksCount, objectsBag);
                Parallel.ForEach(acquireTasks, x => x.Start());
                Task.WaitAll(acquireTasks);

                Task[] returnTasks = CreateReturnTasks(objectPool, objectsBag);
                Parallel.ForEach(returnTasks, x => x.Start());
                Task.WaitAll(returnTasks);

                objectsBag = new ConcurrentBag<string>();
                var actualIdleCount = objectPool.IdleCount;
                acquireTasks = CreateAcquireAndStoreTasks(objectPool, actualIdleCount, objectsBag);
                Parallel.ForEach(acquireTasks, x => x.Start());
                Task.WaitAll(acquireTasks);

                Assert.True(objectsBag.All(obj => Regex.IsMatch(obj, validPattern)));
            }
        }

        [Fact]
        public void TestValidateOnAcquireAndReturn()
        {
            object createLock = new object();
            object returnLock = new object();
            Random rnd = new Random();

            using (var poolManager = new PoolManager())
            {
                var objectPool = poolManager.NewPool<SelfValidatingPoco>()
                    .InitialSize(0)
                    .MaxSize(100)
                    .WithCreator(() => {
                        lock (createLock) {
                            var val = rnd.NextDouble();
                            return new SelfValidatingPoco(isValid: (val >= 0.5d));
                        }
                    })
                    .WithDestroyer((obj) => { /* No-Op*/ })
                    .WithValidator(new PocoValidator(validateOnAcquire: true, validateOnReturn: true))
                    .AcquiredInvalidLimit(10)
                    .Instance();

                const int tasksCount = 100;
                var objectsBag = new ConcurrentBag<SelfValidatingPoco>();
                Task[] acquireTasks = CreateAcquireAndStoreTasks(objectPool, tasksCount, objectsBag);
                Parallel.ForEach(acquireTasks, x => x.Start());
                Task.WaitAll(acquireTasks);

                Task[] returnTasks = CreateReturnTasks(objectPool, objectsBag, (obj) => 
                {
                    lock (returnLock)
                    {
                        double val = rnd.NextDouble();

                        if (val >= 0.5d)
                        {
                            obj.MakeMeValid();
                        }
                        else
                        {
                            obj.MakeMeInvalid();
                        }
                    }
                });
                Parallel.ForEach(returnTasks, x => x.Start());
                Task.WaitAll(returnTasks);

                objectsBag = new ConcurrentBag<SelfValidatingPoco>();
                var actualIdleCount = objectPool.IdleCount;
                acquireTasks = CreateAcquireAndStoreTasks(objectPool, actualIdleCount, objectsBag);
                Parallel.ForEach(acquireTasks, x => x.Start());
                Task.WaitAll(acquireTasks);

                Assert.True(objectsBag.All(obj => obj.IsValid));
            }
        }

        [Fact]
        public void TestInvalidateValidateOnAcquireAndReturnMixed()
        {
            const int tasksCount = 500;
            Random rnd = new Random();
            ConcurrentQueue<int> flags = new ConcurrentQueue<int>();
            for (int f = 0; f < tasksCount * 5; f++) {
                flags.Enqueue(rnd.Next(4));
            }

            using (var poolManager = new PoolManager())
            {
                var objectPool = poolManager.NewPool<SelfValidatingPoco>()
                    .InitialSize(0)
                    .MaxSize(250)
                    .WithCreator(() => new SelfValidatingPoco(isValid: (ExtractFlag(flags) >= 2)))
                    .WithDestroyer((obj) => { /* No-Op*/ })
                    .WithValidator(new PocoValidator(validateOnAcquire: true, validateOnReturn: true))
                    .AcquiredInvalidLimit(250)
                    .Instance();

                var objectsBag = new ConcurrentBag<SelfValidatingPoco>();
                Task[] acquireTasks = new Task[tasksCount];

                for (int i = 0; i < tasksCount; i++) {
                    acquireTasks[i] = new Task(() =>
                    {
                        SelfValidatingPoco obj;
                        switch (ExtractFlag(flags))
                        {
                            case 0: //Acquire, Store
                                obj = objectPool.Acquire();
                                objectsBag.Add(obj);
                                break;
                            case 1: //Acquire, Wait, Return
                                obj = objectPool.Acquire();
                                Thread.Sleep(ExtractFlag(flags) * 100);
                                objectPool.Return(obj);
                                break;
                            case 2: //Acquire, Wait, Invalidate
                                obj = objectPool.Acquire();
                                Thread.Sleep(ExtractFlag(flags) * 100);
                                objectPool.Invalidate(obj);
                                break;
                            case 3: //Acquire, Wait, Random State Change, Return
                                obj = objectPool.Acquire();
                                Thread.Sleep(ExtractFlag(flags) * 100);

                                if (ExtractFlag(flags) >= 2)
                                {
                                    obj.MakeMeInvalid();
                                }

                                objectPool.Return(obj);
                                break;
                        }
                    });
                }

                Parallel.ForEach(acquireTasks, x => x.Start());
                Task.WaitAll(acquireTasks);

                Assert.True(objectsBag.Count > 0);
                Assert.True(objectPool.ActiveCount > 0);
                Assert.True(objectPool.IdleCount > 0);
            }
        }

        [Fact]
        public void TestAcquiredInvalidLimitInvalid()
        {
            using (var poolManager = new PoolManager())
            {
                var descriptor = poolManager.NewPool<string>()
                    .InitialSize(0)
                    .MaxSize(10)
                    .WithCreator(() => Guid.NewGuid().ToString())
                    .WithDestroyer((obj) => { /* No-Op*/ })
                    .WithValidator(new StringValidator(validateOnAcquire: true, validateOnReturn: false, validateDelegate: (obj) => false))
                    .AcquiredInvalidLimit(-5);

                Assert.Throws<ArgumentException>(() => descriptor.Instance());
            }
        }

        [Fact]
        public void TestNullDestroyerShouldInvokeDiposeOnIdleDisposables()
        {
            using (var poolManager = new PoolManager())
            {
                var objectPool = poolManager.NewPool<DisposablePoco>()
                    .InitialSize(0)
                    .MaxSize(10)
                    .WithCreator(() => new DisposablePoco())
                    .WithDestroyer(null)
                    .Instance();

                DisposablePoco obj = objectPool.Acquire();
                objectPool.Return(obj);

                objectPool.Dispose();

                Assert.True(obj.Disposed);
            }
        }

        [Fact]
        public void TestNullDestroyerShouldInvokeDiposeOnActiveDisposables()
        {
            using (var poolManager = new PoolManager())
            {
                var objectPool = poolManager.NewPool<DisposablePoco>()
                    .InitialSize(0)
                    .MaxSize(10)
                    .WithCreator(() => new DisposablePoco())
                    .WithDestroyer(null)
                    .Instance();

                DisposablePoco obj = objectPool.Acquire();

                objectPool.Dispose();

                Assert.True(obj.Disposed);
            }
        }

        [Fact]
        public void TestNullDestroyerShouldInvokeDisposeOnAllActiveAndIdle()
        {
            using (var poolManager = new PoolManager())
            {
                var objectPool = poolManager.NewPool<DisposablePoco>()
                    .InitialSize(0)
                    .MaxSize(10)
                    .WithCreator(() => new DisposablePoco())
                    .WithDestroyer(null)
                    .Instance();

                List<DisposablePoco> activeInstances = new List<DisposablePoco>();
                List<DisposablePoco> idleInstances = new List<DisposablePoco>();

                for (int i = 0; i < 5; i++) {
                    activeInstances.Add(objectPool.Acquire());
                    idleInstances.Add(objectPool.Acquire());
                }

                foreach (DisposablePoco toReturn in idleInstances) {
                    objectPool.Return(toReturn);
                }

                objectPool.Dispose();

                Assert.True(activeInstances.TrueForAll(obj => obj.Disposed));
                Assert.True(idleInstances.TrueForAll(obj => obj.Disposed));
            }
        }

        private static int ExtractFlag(ConcurrentQueue<int> flags)
        {
            int flag;
            while (!flags.TryDequeue(out flag))
            {
                /* try again */
            }

            return flag;
        }

        private static Task[] CreateReturnTasks<T>(IObjectPool<T> objectPool, ConcurrentBag<T> objectsBag) where T : class
        {
            return CreateReturnTasks(objectPool, objectsBag, null);
        }

        private static Task[] CreateReturnTasks<T>(IObjectPool<T> objectPool, ConcurrentBag<T> objectsBag, Action<T> preReturnAction) where T : class
        {
            var objectsArray = objectsBag.ToArray();
            var returnTasks = new Task[objectsArray.Length];
            for (var i = 0; i < returnTasks.Length; i++)
            {
                T obj = objectsArray[i];
                returnTasks[i] = new Task(() =>
                {
                    preReturnAction?.Invoke(obj);
                    objectPool.Return(obj);
                });
            }

            return returnTasks;
        }

        private static Task[] CreateAcquireAndStoreTasks<T>(IObjectPool<T> objectPool, int tasksCount, ConcurrentBag<T> objectsBag) where T : class
        {
            var acquireTasks = new Task[tasksCount];
            for (var i = 0; i < tasksCount; i++)
            {
                acquireTasks[i] = new Task(() =>
                {
                    T obj = objectPool.Acquire();
                    objectsBag.Add(obj);
                });
            }

            return acquireTasks;
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

        private void DoTestAcquireAgain(IObjectPool<IDbConnection> objectPool)
        {
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

    class NeverValidator : IPooledObjectValidator<string>
    {
        public bool ValidateOnAcquire => false;

        public bool ValidateOnReturn => false;

        public bool Validate(string obj)
        {
            throw new InvalidOperationException("I must never be called.");
        }
    }

    public class StringValidator : IPooledObjectValidator<string>
    {

        private readonly bool _validateOnAcquire;
        private readonly bool _validateOnReturn;
        private readonly Func<string, bool> _validateDelegate;

        public bool ValidateOnAcquire => _validateOnAcquire;
        public bool ValidateOnReturn => _validateOnReturn;

        public StringValidator(bool validateOnAcquire, bool validateOnReturn, Func<string, bool> validateDelegate)
        {
            _validateOnAcquire = validateOnAcquire;
            _validateOnReturn = validateOnReturn;
            _validateDelegate = validateDelegate;
        }

        public bool Validate(string obj) => _validateDelegate(obj);
    }

    public sealed class SelfValidatingPoco
    {
        public SelfValidatingPoco(bool isValid) {
            IsValid = isValid;
        }

        public bool IsValid { get; private set; }

        public void MakeMeInvalid() => IsValid = false;

        public void MakeMeValid() => IsValid = true;
    }

    public class PocoValidator : IPooledObjectValidator<SelfValidatingPoco>
    {
        private readonly bool _validateOnAcquire;
        private readonly bool _validateOnReturn;

        public bool ValidateOnAcquire => _validateOnAcquire;
        public bool ValidateOnReturn => _validateOnReturn;

        public PocoValidator(bool validateOnAcquire, bool validateOnReturn)
        {
            _validateOnAcquire = validateOnAcquire;
            _validateOnReturn = validateOnReturn;
        }

        public bool Validate(SelfValidatingPoco obj) => obj.IsValid;
    }

    public sealed class DisposablePoco : IDisposable {

        public bool Disposed => disposedValue;

        #region IDisposable Support
        private bool disposedValue = false;

        void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    /* Do Nothing */
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion

    }
}
