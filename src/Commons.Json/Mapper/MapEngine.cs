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
using System.Collections;
using System.Globalization;
using System.Reflection;
using Commons.Utils;

namespace Commons.Json.Mapper
{
    internal class MapEngine : IMapEngine
    {
        private readonly TypeContainer types;
        private readonly MapperContainer mappers;
        private readonly ConfigContainer configuration;
        private readonly CollectionBuilder colBuilder;

        public MapEngine(TypeContainer types, MapperContainer mappers, ConfigContainer configuration, CollectionBuilder colBuilder)
        {
            this.types = types;
            this.mappers = mappers;
            this.configuration = configuration;
            this.colBuilder = colBuilder;
        }

        public object Map(Type type, JValue jsonValue)
        {
            JObject jsonObj;
            JArray jsonArray;
            JInteger jsonInteger;
            JDecimal jsonDecimal;
            JString jsonString;
            JBoolean jsonBool;

            if (jsonValue.Is<JNull>())
            {
                if (!type.IsClass() && !type.IsNullable())
                {
                    throw new InvalidCastException(Messages.CannotAssignNullToStruct);
                }
                return null;
            }

            var mapper = mappers.GetMapper(type);
            if (mapper.ManualCreate != null)
            {
                return mapper.ManualCreate(jsonValue);
            }

            if (type.IsArray)
            {
                return BuildArray(type.GetElementType(), jsonValue);
            }

            if (type.IsCollection() && !type.IsDictionary())
            {
                return BuildEnumerable(type, jsonValue);
            }

            if (jsonValue.Is<JObject>(out jsonObj))
            {
                return BuildObject(type, jsonObj);
            }

            if (jsonValue.Is<JArray>(out jsonArray))
            {
                if (type.IsArray)
                {
                    var itemType = type.GetElementType();
                    return BuildArray(itemType, jsonArray);
                }
                else if (type.IsCollection() && !type.IsDictionary())
                {
                    return BuildEnumerable(type, jsonArray);
                }
                else
                {
                    throw new InvalidCastException(Messages.JsonValueTypeNotMatch);
                }
            }
            
            if (jsonValue.Is<JString>(out jsonString))
            {
                return BuildString(type, jsonString);
            }

            if (jsonValue.Is<JInteger>(out jsonInteger))
            {
                return BuildInteger(type, jsonInteger);
            }

            if (jsonValue.Is<JDecimal>(out jsonDecimal))
            {
                return BuildDecimal(type, jsonDecimal);
            }

            if (jsonValue.Is<JBoolean>(out jsonBool))
            {
                return BuildBool(type, jsonBool);
            }

            return null;
        }

        private object BuildEnumerable(Type targetType, JValue jsonValue)
        {
            //TODO: how about two type arguments?
            var itemType = targetType.GetGenericArguments()[0];
            //TODO: do not use reflection.
            IEnumerable raw;
            var mapper = mappers.GetMapper(targetType);

            if (mapper.Create != null)
            {
                raw = mapper.Create() as IEnumerable;
            }
            else
            {
                raw = colBuilder.Construct(targetType) as IEnumerable;
            }

            JArray jsonArray;
            if (jsonValue.Is<JArray>(out jsonArray))
            {
                for (var i = 0; i < jsonArray.Count; i++)
                {
                    var item = jsonArray[i];
                    var itemValue = Map(itemType, item);
                    colBuilder.Build(raw, itemValue);
                }
            }
            else
            {
                var itemValue = Map(itemType, jsonValue);
                colBuilder.Build(raw, itemValue);
            }

            return raw;
        }

        private object BuildBool(Type type, JBoolean jsonBool)
        {
            if (type != typeof (bool))
            {
                throw new InvalidCastException(Messages.JsonValueTypeNotMatch);
            }
            bool v = jsonBool;
            return v;
        }
        
        private object BuildDecimal(Type actualType, JDecimal jsonDecimal)
        {
            object result;
            if (actualType != typeof (float) && actualType != typeof (double) && actualType != typeof (decimal))
            {
                throw new InvalidCastException(Messages.JsonValueTypeNotMatch);
            }
            if (actualType == typeof (float))
            {
                result = jsonDecimal.AsSingle();
            }
            else if (actualType == typeof (double))
            {
                result = jsonDecimal.AsDouble();
            }
            else
            {
                result = jsonDecimal.AsDecimal();
            }
            return result;
        }

        private object BuildInteger(Type type, JInteger jsonInteger)
        {
            if (!type.IsJsonNumber())
            {
                throw new InvalidCastException(Messages.JsonValueTypeNotMatch);
            }
            return GetIntegerPropertyValue(type, jsonInteger);
        }

        private object BuildString(Type actualType, JString jsonString)
        {
            object result;
            var str = jsonString.Value;
            if (actualType != typeof (string) && actualType != typeof (DateTime) && !actualType.IsEnum() 
                && actualType != typeof(Guid) && actualType != typeof(char) && actualType != typeof(byte[]))
            {
                throw new InvalidCastException(Messages.JsonValueTypeNotMatch);
            }
            if (actualType == typeof (DateTime))
            {
                DateTime dt;
                if (TryParseDate(str, out dt))
                {
                    result = dt;
                }
                else
                {
                    throw new FormatException(Messages.InvalidDateFormat);
                }
            }
            else if (actualType.IsEnum())
            {
                result = Enum.Parse(actualType, str);
            }
            else if (actualType == typeof(Guid))
            {
                Guid guid;
                if(!Guid.TryParse(str, out guid))
                {
                    throw new InvalidCastException(Messages.InvalidDateFormat);
                }
                result = guid;
            }
            else if (actualType == typeof(char))
            {
                result = Convert.ToChar(str);
            }
            else if (actualType == typeof(byte[]))
            {
                result = Convert.FromBase64String(str);
            }
            else
            {
                result = str;
            }

            return result;
        }


        private Array BuildArray(Type itemType, JValue jsonValue)
        {
            JArray jsonArray;
            Array array;
            if (jsonValue.Is<JArray>(out jsonArray))
            {
                array = Array.CreateInstance(itemType, jsonArray.Length);
                for (var i = 0; i < jsonArray.Length; i++)
                {
                    var itemValue = Map(itemType, jsonArray[i]);
                    if (jsonValue != null)
                    {
                        array.SetValue(itemValue, i);
                    }
                }
            }
            else
            {
                array = Array.CreateInstance(itemType, 1);
                var itemValue = Map(itemType, jsonValue);
                array.SetValue(itemValue, 0);
            }

            return array;
        }

        private object BuildObject(Type type, JObject jsonObj)
        {
            var properties = types[type].Setters;
            MapperImpl mapper = mappers.GetMapper(type);
            object target;
            if (mapper.Create != null)
            {
                target = mapper.Create();
            }
            else 
            {
                target = types[type].Launch();
            }

            foreach (var prop in properties)
            {
				var itemName = prop.Item1.Name;
                if (mapper.IsPropertyIgnored(itemName))
                {
                    continue;
                }

                var key = mapper.GetKey(itemName);
                var propertyType = prop.Item1.PropertyType;
                if (jsonObj.ContainsKey(key))
                {
                    var jsonValue = jsonObj[key];
                    var value = Map(propertyType, jsonValue);
                    if (type.IsClass())
                    {
                        prop.Item2(target, value);
                    }
                    else
                    {
                        prop.Item1.SetValue(target, value, null);
                    }
                }
            }

            return target;
        }

        private bool TryParseDate(string str, out DateTime dt)
        {
            object format;
            var hasFormat = configuration.TryGetValue(Messages.DateFormat, out format);
            if (hasFormat && !string.IsNullOrWhiteSpace((string)format))
            {
                return DateTime.TryParseExact(str, (string)format, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out dt);
            }
            else
            {
                return DateTime.TryParse(str, out dt);
            }
        }

        private static object GetIntegerPropertyValue(Type propertyType, JInteger integer)
        {
            object integerObj = null;
            if (propertyType == typeof (long))
            {
                integerObj = integer.AsInt64();
            }
            else if (propertyType == typeof (int))
            {
                integerObj = integer.AsInt32();
            }
            else if (propertyType == typeof (byte))
            {
                integerObj = integer.AsByte();
            }
            else if (propertyType == typeof (sbyte))
            {
                integerObj = integer.AsSByte();
            }
            else if (propertyType == typeof (short))
            {
                integerObj = integer.AsInt16();
            }
            else if (propertyType == typeof (double))
            {
                integerObj = integer.AsDouble();
            }
            else if (propertyType == typeof (float))
            {
                integerObj = integer.AsSingle();
            }
            else if (propertyType == typeof (decimal))
            {
                integerObj = integer.AsDecimal();
            }
            else if (propertyType == typeof (ulong))
            {
                integerObj = integer.AsUInt64();
            }
            else if (propertyType == typeof (uint))
            {
                integerObj = integer.AsUInt32();
            }
            else if (propertyType == typeof (ushort))
            {
                integerObj = integer.AsUInt16();
            }

            return integerObj;
        }
    }
}
