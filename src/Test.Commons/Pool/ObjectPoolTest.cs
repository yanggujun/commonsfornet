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
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using Commons.Pool;
using Xunit;

namespace Test.Commons.Pool
{
	public class ObjectPoolTest
	{
		[Fact]
		public void TestNormalAcquire()
		{
			var conn = @"Data Source=(local);Initial Catalog=Test; UID=admin; PWD=111111";
			var objectPool = new GenericObjectPool<SqlConnection>(0, 5, new DefaultDbConnectionFactory<SqlConnection>(conn));
			var tasks = new Task[6];
			for (var i = 0; i < 6; i++)
			{
				 tasks[i] = new Task(() =>
				 {
					 var connection = objectPool.Acquire();
					 using (var command = connection.CreateCommand())
					 {
						 command.CommandText = @"select top 1 * from [Test]";
						 using (var reader = command.ExecuteReader())
						 {
							 if (reader.Read())
							 {
								 Console.WriteLine(reader[0].ToString());
							 }
						 }
					 }
					 Thread.Sleep(100);
				 });
			}
			foreach (var task in tasks)
			{
				task.Start();
			}
			Task.WaitAll(tasks);
		}
	}
}
