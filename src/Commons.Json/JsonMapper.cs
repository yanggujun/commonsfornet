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
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Contexts;
using Commons.Collections.Map;
using Commons.Json.Mapper;

namespace Commons.Json
{
	// TODO: 1. control characters
	//       2. large numbers.
	//       3. array converter
	//       4. object converter
	//       5. date format
	//       6. culture
	//       7. dictionary key 
	//       8. multi context
	[CLSCompliant(true)]
	public static class JsonMapper
	{
		private const string DefaultContext = "default";
		private static HashedMap<string, MapContext> contexts = new HashedMap<string, MapContext>();

		public static IMapContext UseDateFormat(string format)
		{
			var context = GetContext(DefaultContext);
			context.DateFormat = format;
			return context;
		}

		public static IJsonObjectMapper<T> For<T>()
		{
			var context = GetContext(DefaultContext);
			return context.For<T>();
		}

		public static T To<T>(string json)
		{
			var context = GetContext(DefaultContext);
			return context.To<T>(json);
		}

		public static string ToJson<T>(T target)
		{
			var context = GetContext(DefaultContext);
			return context.ToJson<T>(target);
		}

		public static dynamic Parse(string json)
		{
			return JsonParser.Parse(json);
		}

		private static MapContext GetContext(string contextName)
		{
			if (!contexts.ContainsKey(contextName))
			{
				var ctx = new MapContext(contextName);
				contexts.Add(DefaultContext, ctx);
			}
			var context = contexts[contextName];

			return context;
		}
	}
}
