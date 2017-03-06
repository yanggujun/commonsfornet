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

namespace Commons.Json.Mapper
{
    public class DictReflector
    {
		private readonly ConcurrentDictionary<Type, Tuple<Func<object, string>, Func<object, object>>> serializeMapper = 
			new ConcurrentDictionary<Type, Tuple<Func<object, string>, Func<object, object>>>();
		private readonly ConcurrentDictionary<Type, Tuple<Func<object>, Action<object, string, object>>> deserializeMapper = new ConcurrentDictionary<Type, Tuple<Func<object>, Action<object, string, object>>>();

		public Tuple<Func<object, string>, Func<object, object>> GetReadDelegate(Type dictType, Type keyType, Type valueType)
		{
			if (serializeMapper.ContainsKey(dictType))
			{
				return serializeMapper[dictType];
			}
			var kvpGenericType = typeof(KeyValuePair<,>);
			var kvpType = kvpGenericType.MakeGenericType(keyType, valueType);
			
			var getKey = kvpType.CreateGetKeyDelegate();
			var getValue = kvpType.CreateGetValueDelegate();
			var tuple = new Tuple<Func<object, string>, Func<object, object>>(getKey, getValue);
			serializeMapper[dictType] = tuple;
			return tuple;
		}

		public Tuple<Func<object>, Action<object, string, object>> GetDeserializeDelegates(Type dictType, Type valueType)
		{
			if (deserializeMapper.ContainsKey(dictType))
			{
				return deserializeMapper[dictType];
			}
			var add = dictType.CreateDictAddDelegate(valueType);
			var create = dictType.CreateNewDelegate();
			var tuple = new Tuple<Func<object>, Action<object, string, object>>(create, add);
			return tuple;
		}
    }
}
