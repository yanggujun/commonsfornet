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
using System.Data;

namespace Commons.Pool
{
    /// <summary>
    /// A default database connection factory. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(true)]
    public class DefaultDbConnectionFactory<T> : IPooledObjectFactory<T> where T : IDbConnection, new()
    {
        protected string ConnectionString { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connString"></param>
        public DefaultDbConnectionFactory(string connString)
        {
            ConnectionString = connString;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual T Create()
        {
            var connection = new T {ConnectionString = ConnectionString};
            connection.Open();
            return connection;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        public virtual void Destroy(T obj)
        {
            obj.Close();
        }
    }
}
