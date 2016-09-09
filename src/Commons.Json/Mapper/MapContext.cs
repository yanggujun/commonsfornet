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
    internal class MapContext : IMapContext
    {
        private string dateFormat = string.Empty;
        public MapContext(string name)
        {
            Name = name;
            Mappers = new MapperContainer();
            MapEngineFactory = new MapEngineFactory();
            Configuration = new ConfigContainer();
            Types = new TypeContainer();
            CollBuilder = new CollectionBuilder();
            Launcher = new Launcher(Mappers, Types);
            JsonBuilder = new JsonBuilder(Mappers, Types, Configuration);
            MapEngine = 
                MapEngineFactory.CreateMapEngine(JsonBuilder, CollBuilder,
                                                 Mappers, Types, Configuration);

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

        public string ToJson<T>(T target)
        {
            return MapEngine.Map(target);
        }

        public T To<T>(string json)
        {
            return (T)To(typeof(T), json);
        }

        public object To(Type type, string json)
        {
            var parseEngine = new JsonParseEngine();
            var jsonValue = parseEngine.Parse(json);

            object target = null;
            if (!type.IsJsonPrimitive() && !type.IsArray)
            {
                target = Launcher.Launch(type);
            }

            return MapEngine.Map(target, type, jsonValue);
        }

        public MapperContainer Mappers { get; private set; }

        public IMapEngineFactory MapEngineFactory { get; private set; }

        public CollectionBuilder CollBuilder { get; private set; }

        public ILauncher Launcher { get; private set; }
        
        public ConfigContainer Configuration { get; private set; }

        public TypeContainer Types { get; private set; }

        public IJsonBuilder JsonBuilder { get; private set; }

        public IMapEngine MapEngine { get; private set; }

        public string DateFormat
        {
            set
            {
                Configuration.Add(Messages.DateFormat, value);
            }
        }

    }
}
