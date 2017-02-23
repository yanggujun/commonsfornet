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

namespace Commons.Json.Mapper
{
	internal class MapContext : IMapContext
	{
		public MapContext(string name)
		{
			Name = name;
			Mappers = new MapperContainer();
			Configuration = new ConfigContainer();
			Types = new TypeContainer();
			CollBuilder = new CollectionBuilder();
			SerializerMapper = new ConcurrentDictionary<Type, Func<object, string>>();
			DeserializerMapper = new ConcurrentDictionary<Type, Func<Type, JValue, object>>();
			MapEngine = new MapEngine(Types, Mappers, Configuration, CollBuilder, DeserializerMapper);
			ParseEngine = new JsonParseEngine();
		}

		public string Name { get; private set; }

		public IJsonObjectMapper<T> For<T>()
		{
			var type = typeof(T);
			if (!Mappers.ContainsMapper(type))
			{
				Mappers.PushMapper(type);
			}
			var mapper = Mappers.GetMapper(type);
			var jsonObjMapper = new JsonObjectMapper<T>(mapper, Configuration);
			return jsonObjMapper;
		}

		// TODO: need to parse the actually typeof(T) to JsonBuilder as sometimes, typeof(T) != target.GetType()
		public string ToJson<T>(T target)
		{
			var jsonBuilder = new JsonBuilder(Mappers, Types, Configuration, SerializerMapper);
			return jsonBuilder.Build(target);
		}

		public T To<T>(string json)
		{
			return (T)To(typeof(T), json);
		}

		public object To(Type type, string json)
		{
            if (json == null)
            {
                throw new ArgumentException(Messages.InvalidValue);
            }
			var jsonValue = ParseEngine.Parse(json);

			return MapEngine.Map(type, jsonValue);
		}

		public MapperContainer Mappers { get; private set; }

		public CollectionBuilder CollBuilder { get; private set; }

		public ConfigContainer Configuration { get; private set; }

		public TypeContainer Types { get; private set; }

		public ConcurrentDictionary<Type, Func<object, string>> SerializerMapper;

		public ConcurrentDictionary<Type, Func<Type, JValue, object>> DeserializerMapper;

		public MapEngine MapEngine { get; private set; }

		public JsonParseEngine ParseEngine { get; private set; }


        public string DateFormat
        {
            set
            {
                Configuration.Add(Messages.DateFormat, value);
            }
        }

    }
}
