﻿// Copyright CommonsForNET.
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
using Commons.Json.Mapper;
using Commons.Utils;

namespace Commons.Json
{
    [CLSCompliant(true)]
    public static class JsonMapper
    {
        private const string DefaultContext = "default";
        private static readonly IMapContext defaultContext = new MapContext(DefaultContext);

        public static IMapContext UseDateFormat(string format)
        {
            defaultContext.DateFormat = format;
            return defaultContext;
        }

        public static IJsonObjectMapper<T> For<T>()
        {
            return defaultContext.For<T>();
        }

        public static T To<T>(string json)
        {
            return (T)defaultContext.To(typeof(T), json);
        }

        public static object To(Type type, string json)
        {
            return defaultContext.To(type, json);
        }

        public static string ToJson<T>(T target)
        {
            return defaultContext.ToJson<T>(target);
        }

        public static T To<T>(string json, Transformer<JValue, T> transform)
        {
            Guarder.CheckNull(transform, "transform");
            var engine = new JsonParseEngine();
            var value = engine.Parse(json);
            return transform(value);
        }

        public static T To<T>(string json, IJsonConverter<T> converter)
        {
            Guarder.CheckNull(converter, "converter");
            var engine = new JsonParseEngine();
            var value = engine.Parse(json);
            return converter.Convert(value);
        }

        public static string ToJson<T>(T target, Transformer<T, JValue> transform)
        {
            Guarder.CheckNull(transform, "transform");
            Guarder.CheckNull(target, "target");
            var value = transform(target);
            return value.ToString();
        }

        public static string ToJson<T>(T target, IObjectConverter<T> converter)
        {
            Guarder.CheckNull(converter, "converter");
            Guarder.CheckNull(target, "target");
            var value = converter.Convert(target);
            return value.ToString();
        }

        public static dynamic Parse(string json)
        {
            Guarder.CheckNull(json, "json");
            return JsonParser.Parse(json);
        }

    }
}
