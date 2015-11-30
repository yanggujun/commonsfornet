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

		[Fact]
		public void TestNormalAcquirePoolNotFull()
		{
			var mockFactory = new Mock<IPooledObjectFactory<IDbConnection>>();
			var mockConnection = new Mock<IDbConnection>();
			var mockCommand = new Mock<IDbCommand>();
			mockFactory.Setup(x => x.Create()).Returns(mockConnection.Object);
			mockConnection.Setup(x => x.CreateCommand()).Returns(mockCommand.Object);
			mockCommand.Setup(x => x.ExecuteScalar()).Callback(() => Thread.Sleep(100));
			var objectPool = new GenericObjectPool<IDbConnection>(0, 100, mockFactory.Object);
			var tasks = new Task[50];
			var connections = new List<IDbConnection>();
			for (var i = 0; i < 50; i++)
			{
				 tasks[i] = new Task(() =>
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
				 });
			}
			foreach (var task in tasks)
			{
				task.Start();
			}
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

			foreach (var task in returnTasks)
			{
				task.Start();
			}
			Task.WaitAll(returnTasks);
			Assert.Equal(50, objectPool.IdleCount);
			Assert.Equal(0, objectPool.ActiveCount);
			Assert.Equal(0, objectPool.InitialSize);
			Assert.Equal(100, objectPool.Capacity);
		}
	}
}
