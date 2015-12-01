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
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Commons.Pool;
using Moq;
using Xunit;

namespace Test.Commons.Pool
{
	public class ObjectPoolTest
	{	
		private static object locker = new object();
        private Mock<IPooledObjectFactory<IDbConnection>> mockFactory;
        private Mock<IDbConnection> mockConnection;
        private Mock<IDbCommand> mockCommand;

		[Fact]
		public void TestNormalAcquirePoolNotFull()
		{
            Setup();
			var objectPool = new GenericObjectPool<IDbConnection>(0, 100, mockFactory.Object);
			var tasks = new Task[50];
			var connections = new List<IDbConnection>();
			for (var i = 0; i < 50; i++)
			{
				 tasks[i] = new Task(() => Do(connections, objectPool));
			}

            tasks.ToList().ForEach(x => x.Start());

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

            returnTasks.ToList().ForEach(x => x.Start());

			Task.WaitAll(returnTasks);
			Assert.Equal(50, objectPool.IdleCount);
			Assert.Equal(0, objectPool.ActiveCount);
			Assert.Equal(0, objectPool.InitialSize);
			Assert.Equal(100, objectPool.Capacity);
		}

        [Fact]
        public void TestAcquireAgain()
        {
            Setup();
            var objectPool = new GenericObjectPool<IDbConnection>(0, 100, mockFactory.Object);

            var tasks1 = new Task[30];
            var connections = new List<IDbConnection>();
            for (var i = 0; i < 30; i++)
            {
                tasks1[i] = new Task(() => Do(connections, objectPool));
            }
            tasks1.ToList().ForEach(x => x.Start());

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
            tasks2.ToList().ForEach(x => x.Start());
            Task.WaitAll(tasks2);

            Assert.Equal(0, objectPool.IdleCount);
            Assert.Equal(60, objectPool.ActiveCount);

            var returnTasks = new Task[40];

            for (var i = 0; i < 40; i++)
            {
                var conn = connections[i];
                returnTasks[i] = new Task(() => objectPool.Return(conn));
            }

            returnTasks.ToList().ForEach(x => x.Start());
            Task.WaitAll(returnTasks);

            Assert.Equal(40, objectPool.IdleCount);
            Assert.Equal(20, objectPool.ActiveCount);

            for (var i = 0; i < 20; i++)
            {
                objectPool.Return(connections[i + 40]);
            }
            Assert.Equal(60, objectPool.IdleCount);
            Assert.Equal(0, objectPool.ActiveCount);
        }

        private void Do(IList<IDbConnection> connections, IObjectPool<IDbConnection> objectPool)
        {
            var connection = objectPool.Acquire();
            Assert.NotNull(connection);
            lock (locker)
            {
                connections.Add(connection);
            }
            using (var command = connection.CreateCommand())
            {
                command.ExecuteScalar();
            }
        }

        private void Setup()
        {
            mockFactory = new Mock<IPooledObjectFactory<IDbConnection>>();
            mockConnection = new Mock<IDbConnection>();
            mockCommand = new Mock<IDbCommand>();
			mockFactory.Setup(x => x.Create()).Returns(mockConnection.Object);
			mockConnection.Setup(x => x.CreateCommand()).Returns(mockCommand.Object);
			mockCommand.Setup(x => x.ExecuteScalar()).Callback(() => Thread.Sleep(100));
        }
	}
}
