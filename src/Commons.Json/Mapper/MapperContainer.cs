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
using Commons.Collections.Map;

namespace Commons.Json.Mapper
{
	internal class MapperContainer
	{
		private ReferenceMap<Type, object> mappers = new ReferenceMap<Type, object>();

		public bool ContainsMapper<T>()
		{
			var type = typeof (T);
			return mappers.ContainsKey(type);
		}

		public void PushMapper<T>(IJsonObjectMapper<T> mapper)
		{
			var type = typeof (T);
			mappers[type] = mapper;
		}

		public IJsonObjectMapper<T> GetMapper<T>()
		{
			var type = typeof (T);
			if (mappers.ContainsKey(type))
			{
				return (IJsonObjectMapper<T>) mappers[type];
			}
			return null;
		}
	}
}
