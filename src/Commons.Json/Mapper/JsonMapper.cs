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

namespace Commons.Json.Mapper
{
	[CLSCompliant(true)]
	public static class JsonMapper
	{
		private static MapperContainer mapperContainer = new MapperContainer();

		public static IJsonObjectMapper<T> For<T>()
		{
			IJsonObjectMapper<T> mapper;
			if (mapperContainer.ContainsMapper<T>())
			{
				mapper = mapperContainer.GetMapper<T>();
			}
			else
			{
				mapper = new JsonObjectMapper<T>();
				mapperContainer.PushMapper(mapper);
			}
			return mapper;
		}

		public static T ToObject<T>(string json)
		{
			var parseEngine = new JsonParseEngine();
			var jsonValue = parseEngine.Parse(json);
			var mapEngine = new MapEngine<T>(For<T>());
			return mapEngine.Map(jsonValue);
		}

		public static void FillWith<T>(string json, ref T obj)
		{
		}

		public static string ToJson<T>(T target)
		{
			return null;
		}

        public static dynamic ToDynamic(string json)
        {
            return null;
        }
	}
}
