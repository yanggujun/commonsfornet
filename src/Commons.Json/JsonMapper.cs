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
using System.Reflection;
using Commons.Json.Mapper;

namespace Commons.Json
{
	[CLSCompliant(true)]
	public static class JsonMapper
	{
		private static MapperContainer mapperContainer = new MapperContainer();
        private static TypeCache typeCache = new TypeCache();
		private static IMapEngineFactory mapEngineFactory = new MapEngineFactory();

		public static IJsonObjectMapper<T> For<T>()
		{
            var type = typeof(T);
			if (!mapperContainer.ContainsMapper(type))
			{
                mapperContainer.PushMapper(type);
			}
            var mapper = mapperContainer.GetMapper(type);
            var jsonObjMapper = new JsonObjectMapper<T>(mapper);
            return jsonObjMapper;
		}

		public static T To<T>(string json)
		{
			var parseEngine = new JsonParseEngine();
			var jsonValue = parseEngine.Parse(json);

			T target = default(T);
			if (!typeof (T).IsJsonPrimitive())
			{
				target = typeCache.Instantiate<T>();
			}

			var mapEngine = mapEngineFactory.CreateMapEngine(target, mapperContainer, typeCache);
			return mapEngine.Map(jsonValue);
		}

		public static string ToJson<T>(T target)
		{
            var mapEngine = mapEngineFactory.CreateMapEngine(target, mapperContainer, typeCache);
            return mapEngine.Map(target);
		}

		public static dynamic Parse(string json)
		{
			return JsonParser.Parse(json);
		}
	}
}
