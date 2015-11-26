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

namespace Commons.Pool
{
	/// <summary>
	/// The abstract factory is called by the object pool to create and destroy the objects which are pooled.
	/// </summary>
	/// <typeparam name="T">The object type.</typeparam>
	public interface IPooledObjectFactory<T>
	{
		/// <summary>
		/// Creates an object to be pooled.
		/// </summary>
		/// <returns></returns>
		T Create();

		/// <summary>
		/// Destroys the object in the pool. When overriding this method, exceptions shall be caught, as it will break the 
        /// process and remaining pooled object cannot be destroyed.
		/// </summary>
		/// <param name="obj"></param>
		void Destroy(T obj);
	}
}
