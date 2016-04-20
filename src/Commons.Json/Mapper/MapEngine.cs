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
using System.Linq;
using System.Reflection;
using System.Text;

namespace Commons.Json.Mapper
{
    internal class MapEngine<T> : IMapEngine<T>
    {
        private MapperContainer mappers;
        private T targetObj;
        private TypeCache typeCache;
        private string dateFormat;

        public MapEngine(T target, MapperContainer mappers, TypeCache typeCache, string dateFormat)
        {
            this.mappers = mappers;
            this.targetObj = target;
            this.typeCache = typeCache;
            this.dateFormat = dateFormat;
        }

        public T Map(JValue jsonValue)
        {
            return (T)InternalMap(jsonValue, targetObj, typeof(T));
        }

        public object InternalMap(JValue jsonValue, object obj, Type type)
        {
            if (jsonValue.Is<JString>() || jsonValue.Is<JBoolean>()
                || jsonValue.Is<JInteger>() || jsonValue.Is<JDecimal>())
            {
                return ExtractPrimitiveValue(jsonValue, type);
            }

            if (jsonValue.Is<JNull>())
            {
                return null;
            }

            if (type.IsArray)
            {
                JArray array;
                if (!jsonValue.Is<JArray>(out array))
                {
                    throw new InvalidCastException(Messages.JsonValueTypeNotMatch);
                }
                return ExtractJsonArray(array, type);
            }

            Populate(obj, jsonValue);

            return obj;
        }

        public string Map(T target)
        {
            return Jsonize(target);
        }

        private object ExtractJsonArray(JArray jsonArray, Type arrayType)
        {
            var itemType = arrayType.GetElementType();
            var array = Array.CreateInstance(itemType, jsonArray.Length);
            for (var i = 0; i < jsonArray.Length; i++)
            {
                var jsonValue = GetValueFromJsonArrayItem(jsonArray[i], itemType);
                array.SetValue(jsonValue, i);
            }

            return array;
        }

        private string Jsonize(object target)
        {
            if (target == null)
            {
                return JsonTokens.Null;
            }
            var type = target.GetType();
            var json = string.Empty;
            Type keyType;
            if (type.IsJsonNumber() || type == typeof(bool))
            {
                json = target.ToString();
            }
            else if (type == typeof(string) || type.IsEnum)
            {
                var sb = new StringBuilder();
                sb.Append(JsonTokens.Quoter).Append(target).Append(JsonTokens.Quoter);
                json = sb.ToString();
            }
            else if (type == typeof (DateTime))
            {
                var sb = new StringBuilder();
                var dt = (DateTime) target;
                var time = string.IsNullOrEmpty(dateFormat) ? dt.ToString() : dt.ToString(dateFormat);
                sb.Append(JsonTokens.Quoter).Append(time).Append(JsonTokens.Quoter);
                json = sb.ToString();
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
                            .Append(JsonTokens.Colon).Append(Jsonize(value)).Append(JsonTokens.Comma);
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
                    sb.Append(Jsonize(item)).Append(JsonTokens.Comma);
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
                    sb.Append(Jsonize(item)).Append(JsonTokens.Comma);
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
                var manager = typeCache[type];
                MapperImpl mapper = null;
                if (mappers.ContainsMapper(type))
                {
                    mapper = mappers.GetMapper(type);
                }
                var sb = new StringBuilder();
                sb.Append(JsonTokens.LeftBrace);
                foreach (var prop in manager.Getters)
                {
                    if (mapper != null)
                    {
                        if (mapper.IgnoredProperties.Contains(prop.Name))
                        {
                            continue;
                        }
                    }
                    var propValue = prop.GetValue(target, null);
                    sb.Append(JsonTokens.Quoter);
                    sb.Append(GetJsonKeyFromProperty(prop, mapper));
                    sb.Append(JsonTokens.Quoter);
                    sb.Append(JsonTokens.Colon);
                    sb.Append(Jsonize(propValue));
                    sb.Append(JsonTokens.Comma);
                }
                sb.Remove(sb.Length - 1, 1);
                sb.Append(JsonTokens.RightBrace);
                json = sb.ToString();
            }

            return json;
        }

        private string GetJsonKeyFromProperty(PropertyInfo prop, MapperImpl mapper)
        {
            var key = prop.Name;
            if (mapper != null)
            {
                string k;
                if (mapper.TryGetKey(key, out k))
                {
                    key = k;
                }
            }

            return key;
        }

        private void Populate(object obj, JValue jsonValue)
        {
            JObject jsonObj;
            JArray jsonArray;
            if (jsonValue.Is<JObject>(out jsonObj))
            {
                PopulateJsonObject(obj, jsonObj);
            }
            else if (jsonValue.Is<JArray>(out jsonArray))
            {
                var type = obj.GetType();
                Type itemType;
                if (type.IsList(out itemType))
                {
                    var add = type.GetMethod(Messages.AddMethod);
                    foreach (var value in jsonArray)
                    {
                        var arrayItemValue = GetValueFromJsonArrayItem(value, itemType);
                        add.Invoke(obj, new[] { arrayItemValue });
                    }
                }
            }
        }

        private object GetValueFromJsonArrayItem(JValue jsonValue, Type itemType)
        {
            if (itemType.IsJsonPrimitive())
            {
                return ExtractPrimitiveValue(jsonValue, itemType);
            }
            else if (itemType.IsArray)
            {
                JArray array;
                if (!jsonValue.Is<JArray>(out array))
                {
                    throw new InvalidCastException(Messages.JsonValueTypeNotMatch);
                }
                return ExtractJsonArray(array, itemType);
            }
            else
            {
                var itemValue = typeCache.Instantiate(itemType, mappers);
                Populate(itemValue, jsonValue);
                return itemValue;
            }
        }

        private void PopulateJsonObject(object target, JObject jsonObj)
        {
            var type = target.GetType();
            Type keyType;
            Type valueType;
            if (type.IsDictionary(out keyType, out valueType))
            {
                if (keyType != typeof(string))
                {
                    throw new InvalidCastException(Messages.JsonValueTypeNotMatch);
                }
                var method = type.GetMethod(Messages.AddMethod);
                foreach (var kvp in jsonObj)
                {
                    var key = kvp.Key;
                    object value = null;
                    if (!valueType.IsJsonPrimitive())
                    {
                        value = typeCache.Instantiate(valueType, mappers);
                    }
                    value = InternalMap(kvp.Value, value, valueType);
                    method.Invoke(target, new[] { key, value });
                }
            }
            else
            {
                var properties = typeCache[type].Setters;
                MapperImpl mapper = null;
                if (mappers.ContainsMapper(type))
                {
                    mapper = mappers.GetMapper(type);
                }
                foreach (var prop in properties)
                {
                    if (mapper != null)
                    {
                        if (mapper.IgnoredProperties.Contains(prop.Name))
                        {
                            continue;
                        }
                    }
                    var key = GetJsonKeyFromProperty(prop, mapper);
                    if (jsonObj.ContainsKey(key))
                    {
                        var propertyType = prop.PropertyType;
                        if (propertyType.IsJsonPrimitive())
                        {
                            PopulateJsonPrimitive(target, jsonObj, prop, mapper);
                        }
                        else if (propertyType.IsArray)
                        {
                            JArray array;
                            if (!jsonObj[key].Is<JArray>(out array))
                            {
                                throw new InvalidCastException(Messages.JsonValueTypeNotMatch);
                            }
                            var value = ExtractJsonArray(array, propertyType);
                            prop.SetValue(target, value, null);
                        }
                        else
                        {
                            var propValue = prop.GetValue(target, null);
                            Populate(propValue, jsonObj[key]);
                            if (propertyType.IsNullable() && !propertyType.IsNullablePrimitive())
                            {
                                prop.SetValue(target, propValue);
                            }
                        }
                    }
                }
            }
        }

        private void PopulateJsonPrimitive(object target, JObject jsonObj, PropertyInfo prop, MapperImpl mapper)
        {
            var propertyType = prop.PropertyType;
            var name = GetJsonKeyFromProperty(prop, mapper);

            var value = jsonObj[name];
            var valueType = value.GetType();
            if (valueType == typeof (JArray) || valueType == typeof (JObject))
            {
                throw new InvalidCastException(Messages.JsonValueTypeNotMatch);
            }

            var propertyValue = ExtractPrimitiveValue(value, propertyType);

            prop.SetValue(target, propertyValue, null);
        }

        private object ExtractPrimitiveValue(JValue value, Type type)
        {
            JString str;
            JInteger integer;
            JBoolean boolean;
            JDecimal floating;
            object propertyValue;
            Type actualType;
            if (!type.IsNullable(out actualType))
            {
                actualType = type;
            }
            if (value.Is<JString>(out str))
            {
                if (actualType != typeof (string) && actualType != typeof (DateTime) && !actualType.IsEnum)
                {
                    throw new InvalidCastException(Messages.JsonValueTypeNotMatch);
                }
                if (actualType == typeof (DateTime))
                {
                    DateTime dt;
                    if (TryParseDate(str, out dt))
                    {
                        propertyValue = dt;
                    }
                    else
                    {
                        throw new InvalidCastException(Messages.InvalidDateFormat);
                    }
                }
                else if (actualType.IsEnum)
                {
                    propertyValue = Enum.Parse(actualType, str);
                }
                else
                {
                    string v = str;
                    propertyValue = v;
                }
            }
            else if (value.Is<JInteger>(out integer))
            {
                if (!actualType.IsJsonNumber())
                {
                    throw new InvalidCastException(Messages.JsonValueTypeNotMatch);
                }
                propertyValue = GetIntegerPropertyValue(actualType, integer);
            }
            else if (value.Is<JBoolean>(out boolean))
            {
                if (actualType != typeof (bool))
                {
                    throw new InvalidCastException(Messages.JsonValueTypeNotMatch);
                }
                bool v = boolean;
                propertyValue = v;
            }
            else if (value.Is<JDecimal>(out floating))
            {
                if (actualType != typeof (float) && actualType != typeof (double) && actualType != typeof (decimal))
                {
                    throw new InvalidCastException(Messages.JsonValueTypeNotMatch);
                }
                if (actualType == typeof (float))
                {
                    propertyValue = floating.AsSingle();
                }
                else if (actualType == typeof (double))
                {
                    propertyValue = floating.AsDouble();
                }
                else
                {
                    propertyValue = floating.AsDecimal();
                }
            }
            else if (value.Is<JNull>())
            {
                propertyValue = null;
            }
            else
            {
                // Unlikely to happen.
                throw new InvalidCastException(Messages.JsonValueTypeNotMatch);
            }
            return propertyValue;
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

        private bool TryParseDate(string str, out DateTime dt)
        {
            if (string.IsNullOrEmpty(dateFormat))
            {
                return DateTime.TryParse(str, out dt);
            }
            else
            {
                return DateTime.TryParseExact(str, dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out dt);
            }
        }
    }
}
