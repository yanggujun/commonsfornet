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

using Commons.Utils;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Reflection;
using System.Text;

namespace Commons.Json.Mapper
{
    internal sealed class JsonBuilder
    {
        private readonly MapperContainer mappers;
        private readonly TypeContainer types;
        private readonly ConfigContainer config;
        private readonly ConcurrentDictionary<Type, Action<object, StringBuilder>> serializers;
        private readonly DictReflector dictReflector;

        public JsonBuilder(MapperContainer mappers, TypeContainer types, ConfigContainer config, 
            ConcurrentDictionary<Type, Action<object, StringBuilder>> serializerMapper, DictReflector dictReflector)
        {
            serializers = serializerMapper;
            this.mappers = mappers;
            this.types = types;
            this.config = config;
            this.dictReflector = dictReflector;
        }

        public string Build(object target)
        {
            var buffer = new StringBuilder();
            var serializer = GetSerializer(target);
            serializer(target, buffer);
            return buffer.ToString();
        }

        private Action<object, StringBuilder> GetSerializer(object target)
        {
            if (target == null)
            {
                return SerializeNull;
            }

            var type = target.GetType();
            Action<object, StringBuilder> serializer;
            if (serializers.TryGetValue(type, out serializer))
            {
                return serializer;
            }
            
            if (type == typeof(bool))
            {
                serializer = SerializeBoolean;
            }
            else if (type.IsJsonNumber())
            {
                serializer = SerializeNumber;
            }
            else if (type == typeof(string) || type == typeof(Guid) || type == typeof(char))
            {
                serializer = SerializeString;
            }
            else if (type.IsEnum())
            {
                if (type.HasAttribute(typeof(FlagsAttribute)))
                {
                    serializer = SerializeFlagEnum;
                }
                else
                {
                    serializer = SerializeString;
                }
            }
            else if (type == typeof(DateTime))
            {
                serializer = SerializeTime;
            }
            else if (type == typeof(byte[]))
            {
                serializer = SerializeByteArray;
            }
            else if (type.IsDictionary())
            {
                serializer = SerializeDict;
            }
            else if (type.IsArray)
            {
                serializer = SerializeArray;
            }
            else if (typeof(IList).IsAssignableFrom(type))
            {
                serializer = SerializeList;
            }
            else if (target is IEnumerable)
            {
                serializer = SerializeEnumerable;
            }
            else
            {
                serializer = SerializeObject;
            }

            serializers[type] = serializer;

            return serializer;
        }

        private void SerializeByteArray(object target, StringBuilder localBuffer)
        {
            var bytes = (byte[])target;
            localBuffer.Append(JsonTokens.Quoter).Append(bytes.Base64Encode()).Append(JsonTokens.Quoter);
        }

        private void SerializeObject(object target, StringBuilder localBuffer)
        {
            var type = target.GetType();
            var mapper = mappers.GetMapper(type);
            if (mapper.Serializer != null)
            {
                localBuffer.Append(mapper.Serializer(target).ToString());
            }
            else
            {
                var manager = types[type];
                localBuffer.Append(JsonTokens.LeftBrace);
                var getters = manager.Getters;
                for (var i = 0; i < getters.Count; i++)
                {
                    var prop = getters[i];
                    var itemName = prop.Item1.Name;
                    if (mapper.IsPropertyIgnored(itemName))
                    {
                        continue;
                    }
                    var propValue = prop.Item2(target);
                    localBuffer.Append(JsonTokens.Quoter);
                    localBuffer.Append(mapper.GetKey(itemName));
                    localBuffer.Append(JsonTokens.Quoter);
                    localBuffer.Append(JsonTokens.Colon);
                    var serializer = GetSerializer(propValue);
                    serializer(propValue, localBuffer);
                    localBuffer.Append(JsonTokens.Comma);
                }
                localBuffer.Remove(localBuffer.Length - 1, 1);
                localBuffer.Append(JsonTokens.RightBrace);
            }
        }

        private void SerializeEnumerable(object target, StringBuilder localBuffer)
        {
            localBuffer.Append(JsonTokens.LeftBracket);
            var hasValue = false;
            var items = target as IEnumerable;
            foreach (var item in items)
            {
                var itemSerializer = GetSerializer(item);
                itemSerializer(item, localBuffer);
                localBuffer.Append(JsonTokens.Comma);
                hasValue = true;
            }
            if (hasValue)
            {
                localBuffer.Remove(localBuffer.Length - 1, 1);
            }
            localBuffer.Append(JsonTokens.RightBracket);
        }

        private void SerializeList(object target, StringBuilder localBuffer)
        {
            localBuffer.Append(JsonTokens.LeftBracket);
            var hasValue = false;
            var items = target as IList;
            for (var i = 0; i < items.Count; i++)
            {
                var item = items[i];
                var serializer = GetSerializer(item);
                serializer(item, localBuffer);
                localBuffer.Append(JsonTokens.Comma);
                hasValue = true;
            }

            if (hasValue)
            {
                localBuffer.Remove(localBuffer.Length - 1, 1);
            }
            localBuffer.Append(JsonTokens.RightBracket);
        }

        private void SerializeNull(object target, StringBuilder localBuffer)
        {
            localBuffer.Append(JsonTokens.Null);
        }

        private void SerializeDict(object target, StringBuilder localBuffer)
        {
            localBuffer.Append(JsonTokens.LeftBrace);
            var type = target.GetType();
            var keyType = type.GetGenericArguments()[0];
            var valueType = type.GetGenericArguments()[1];
            if (keyType == typeof(string))
            {
                var dict = (IEnumerable)target;
                var hasValue = false;
                var tuple = dictReflector.GetReadDelegate(type, keyType, valueType);
                var getKey = tuple.Item1;
                var getValue = tuple.Item2;
                foreach (var element in dict)
                {
                    var key = getKey(element);
                    var value = getValue(element);
                    localBuffer.Append(JsonTokens.Quoter).Append(key).Append(JsonTokens.Quoter)
                        .Append(JsonTokens.Colon);
                    var serializer = GetSerializer(value);
                    serializer(value, localBuffer);
                    localBuffer.Append(JsonTokens.Comma);
                    hasValue = true;
                }
                if (hasValue)
                {
                    localBuffer.Remove(localBuffer.Length - 1, 1);
                }
            }
            else
            {
                throw new InvalidOperationException(Messages.InvalidDictionary);
            }
            localBuffer.Append(JsonTokens.RightBrace);
        }

        private void SerializeBoolean(object target, StringBuilder localBuffer)
        {
            var val = (bool)target;
            if (val)
            {
                localBuffer.Append(JsonTokens.True);
            }
            else
            {
                localBuffer.Append(JsonTokens.False);
            }
        }

        private void SerializeNumber(object target, StringBuilder localBuffer)
        {
            localBuffer.Append(target);
        }

        private void SerializeString(object target, StringBuilder localBuffer)
        {
            localBuffer.Append(JsonTokens.Quoter).Append(target).Append(JsonTokens.Quoter);
        }

        private void SerializeFlagEnum(object target, StringBuilder localBuffer)
        {
            localBuffer.Append((long)target);
        }

        private void SerializeTime(object target, StringBuilder localBuffer)
        {
            var dt = (DateTime) target;
            var time = string.IsNullOrEmpty(config.DateFormat) ? dt.FastToStringInvariantCulture() : dt.ToString(config.DateFormat);
            localBuffer.Append(JsonTokens.Quoter).Append(time).Append(JsonTokens.Quoter);
        }

        private void SerializeArray(object target, StringBuilder localBuffer)
        {
            var array = (Array)target;
            localBuffer.Append(JsonTokens.LeftBracket);
            var hasValue = false;
            for (var i = 0; i < array.Length; i++)
            {
                var item = array.GetValue(i);
                var serializer = GetSerializer(item);
                serializer(item, localBuffer);
                localBuffer.Append(JsonTokens.Comma);
                hasValue = true;
            }
            if (hasValue)
            {
                localBuffer.Remove(localBuffer.Length - 1, 1);
            }
            localBuffer.Append(JsonTokens.RightBracket);
        }
    }

}
