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

namespace Commons.Json.Mapper
{
    internal class MapContext : IMapContext
    {
        private string dateFormat = string.Empty;
        public MapContext(string name)
        {
            Name = name;
            Mappers = new MapperContainer();
            Types = new TypeCache();
            MapEngineFactory = new MapEngineFactory();
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
            var jsonObjMapper = new JsonObjectMapper<T>(mapper);
            return jsonObjMapper;
        }

        public string ToJson<T>(T target)
        {
            var mapEngine = MapEngineFactory.CreateMapEngine(target, Mappers, Types, dateFormat);
            return mapEngine.Map(target);
        }

        public T To<T>(string json)
        {
            var parseEngine = new JsonParseEngine();
            var jsonValue = parseEngine.Parse(json);

            T target = default(T);
            var targetType = typeof (T);
            if (!targetType.IsJsonPrimitive() && !targetType.IsArray)
            {
                target = Types.Instantiate<T>(Mappers);
            }

            var mapEngine = MapEngineFactory.CreateMapEngine(target, Mappers, Types, dateFormat);
            return mapEngine.Map(jsonValue);
        }

        public MapperContainer Mappers { get; private set; }

        public TypeCache Types { get; private set; }

        public IMapEngineFactory MapEngineFactory { get; private set; }

        public string DateFormat
        {
            get { return dateFormat; } 
            internal set { dateFormat = value; }
        }
    }
}
