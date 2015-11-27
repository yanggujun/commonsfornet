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
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using Commons.Pool;
using Xunit;

namespace Test.Commons.Pool
{
	public class ObjectPoolTest
	{	
		private const string connString = @"Data Source=(local)\Test;Initial Catalog=Test; UID=; PWD="; 
		private static object locker = new object();

		[Fact]
		public void TestNormalAcquirePoolNotFull()
		{
			var objectPool = new GenericObjectPool<SqlConnection>(0, 5, new DefaultDbConnectionFactory<SqlConnection>(connString));
			var tasks = new Task[3];
			var connections = new List<SqlConnection>();
			for (var i = 0; i < 3; i++)
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
						 command.CommandText = @"select count (*) from [Test]";
						 var count = (int) command.ExecuteScalar();
						 Assert.True(count > 0);
					 }
					 Thread.Sleep(100);
				 });
			}
			foreach (var task in tasks)
			{
				task.Start();
			}
			Task.WaitAll(tasks);
			Assert.Equal(3, connections.Count);
			Assert.Equal(0, objectPool.IdleCount);
			Assert.Equal(3, objectPool.ActiveCount);
			Assert.Equal(0, objectPool.InitialSize);
			Assert.Equal(5, objectPool.Capacity);

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
			Assert.Equal(3, objectPool.IdleCount);
			Assert.Equal(0, objectPool.ActiveCount);
			Assert.Equal(0, objectPool.InitialSize);
			Assert.Equal(5, objectPool.Capacity);
		}
	}
}
