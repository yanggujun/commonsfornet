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
using System.Reflection;
using System.Text;
using System.Collections;
using Commons.Utils;

namespace Commons.Json.Mapper
{
    internal class JsonBuilder : IJsonBuilder
    {
        private readonly MapperContainer mappers;
        private readonly TypeContainer types;
        private readonly ConfigContainer config;
        private readonly StringBuilder localBuffer;

        public JsonBuilder(MapperContainer mappers, TypeContainer types, ConfigContainer config)
        {
            localBuffer = new StringBuilder();
            this.mappers = mappers;
            this.types = types;
            this.config = config;
        }

        public string Build(object target)
        {
            if (target == null)
            {
                return JsonTokens.Null;
            }
            var type = target.GetType();
            var json = string.Empty;
            Type keyType;
            if (type == typeof(bool))
            {
                var val = (bool)target;
                if (val)
                {
                    json = Messages.True;
                }
                else
                {
                    json = Messages.False;
                }
            }
            else if (type.IsJsonNumber())
            {
                json = target.ToString();
            }
            else if (type == typeof(string) || type.IsEnum() || type == typeof(Guid) || type == typeof(char))
            {
                localBuffer.Length = 0;
                localBuffer.Append(JsonTokens.Quoter).Append(target).Append(JsonTokens.Quoter);
                json = localBuffer.ToString();
            }
            else if (type == typeof (DateTime))
            {
                localBuffer.Length = 0;
                var dt = (DateTime) target;
                object dateFormat;
                var time = config.TryGetValue(Messages.DateFormat, out dateFormat) ? dt.ToString((string)dateFormat) : dt.FastToStringInvariantCulture();
                localBuffer.Append(JsonTokens.Quoter).Append(time).Append(JsonTokens.Quoter);
                json = localBuffer.ToString();
            }
            else if (type.IsDictionary(out keyType))
            {
                var sb = new StringBuilder();
                sb.Append(JsonTokens.LeftBrace);
                if (keyType == typeof(string))
                {
                    var dict = (IEnumerable)target;
                    var hasValue = false;
                    foreach (var element in dict)
                    {
                        var key = element.GetType().GetProperty("Key").GetValue(element, null) as string;
                        var value = element.GetType().GetProperty("Value").GetValue(element, null);
                        sb.Append(JsonTokens.Quoter).Append(key).Append(JsonTokens.Quoter)
                            .Append(JsonTokens.Colon).Append(Build(value)).Append(JsonTokens.Comma);
                        hasValue = true;
                    }
                    if (hasValue)
                    {
                        sb.Remove(sb.Length - 1, 1);
                    }
                }
                sb.Append(JsonTokens.RightBrace);
                json = sb.ToString();
            }
            else if (type.IsArray)
            {
                var array = (Array)target;
                var sb = new StringBuilder();
                sb.Append(JsonTokens.LeftBracket);
                var hasValue = false;
                foreach(var item in array)
                {
                    sb.Append(Build(item)).Append(JsonTokens.Comma);
                    hasValue = true;
                }
                if (hasValue)
                {
                    sb.Remove(sb.Length - 1, 1);
                }
                sb.Append(JsonTokens.RightBracket);
                json = sb.ToString();
            }
            else if (typeof(IEnumerable).IsInstanceOfType(target))
            {
                var sb = new StringBuilder();
                sb.Append(JsonTokens.LeftBracket);
                var hasValue = false;
                foreach (var item in ((IEnumerable)target))
                {
                    sb.Append(Build(item)).Append(JsonTokens.Comma);
                    hasValue = true;
                }
                if (hasValue)
                {
                    sb.Remove(sb.Length - 1, 1);
                }
                sb.Append(JsonTokens.RightBracket);
                json = sb.ToString();
            }
            else
            {
                var mapper = mappers.GetMapper(type);
                if (mapper.Serializer != null)
                {
                    json = mapper.Serializer(target).ToString();
                }
                else
                {
                    var manager = types[type];
                    var sb = new StringBuilder();
                    sb.Append(JsonTokens.LeftBrace);
                    foreach (var prop in manager.Getters)
                    {
                        if (mapper.IsPropertyIgnored(prop.Key.Name))
                        {
                            continue;
                        }
                        var propValue = prop.Value(target);
                        sb.Append(JsonTokens.Quoter);
                        sb.Append(mapper.GetKey(prop.Key.Name));
                        sb.Append(JsonTokens.Quoter);
                        sb.Append(JsonTokens.Colon);
                        sb.Append(Build(propValue));
                        sb.Append(JsonTokens.Comma);
                    }
                    sb.Remove(sb.Length - 1, 1);
                    sb.Append(JsonTokens.RightBrace);
                    json = sb.ToString();
                }
            }

            return json;
        }

    }
}
