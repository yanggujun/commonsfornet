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
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Commons.Json.Mapper
{
	internal class MapEngine<T> : IMapEngine<T>
	{
		private MapperContainer mappers;
        private T target;
        private TypeCache typeCache;
		private string dateFormat;

		public MapEngine(T target, MapperContainer mappers, TypeCache typeCache, string dateFormat)
		{
			this.mappers = mappers;
            this.target = target;
            this.typeCache = typeCache;
			this.dateFormat = dateFormat;
		}

		public T Map(JValue jsonValue)
		{
            return (T)InternalMap(jsonValue, target, typeof(T));
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

			Populate(obj, jsonValue);

            return obj;
		}

		public string Map(T target)
		{
            return Jsonize(target);
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
            else if (type == typeof(string))
            {
                var sb = new StringBuilder();
                sb.Append(JsonTokens.Quoter).Append(target).Append(JsonTokens.Quoter);
                json = sb.ToString();
            }
			else if (type == typeof (DateTime))
			{
				var sb = new StringBuilder();
				var dt = (DateTime) target;
				sb.Append(JsonTokens.Quoter).Append(dt.ToString(dateFormat)).Append(JsonTokens.Quoter);
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
                        var key = element.GetType().GetProperty("Key").GetValue(element) as string;
                        var value = element.GetType().GetProperty("Value").GetValue(element);
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
                    var propValue = prop.GetValue(target);
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
                    if (jsonObj.ContainsKey(GetJsonKeyFromProperty(prop, mapper)))
                    {
                        var propertyType = prop.PropertyType;
                        if (propertyType.IsJsonPrimitive())
                        {
                            PopulateJsonPrimitive(target, jsonObj, prop, mapper);
                        }
                        else
                        {
                            var propValue = prop.GetValue(target);
                            Populate(propValue, jsonObj[prop.Name]);
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

			prop.SetValue(target, propertyValue);
		}

		private object ExtractPrimitiveValue(JValue value, Type type)
		{
			JString str;
			JInteger integer;
			JBoolean boolean;
			JDecimal floating;
			object propertyValue;
			if (value.Is<JString>(out str))
			{
				if (type != typeof (string) && type != typeof (DateTime))
				{
					throw new InvalidCastException(Messages.JsonValueTypeNotMatch);
				}
				if (type == typeof (DateTime))
				{
					DateTime dt;
					if (DateTime.TryParseExact(str, dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
					{
						propertyValue = dt;
					}
					else
					{
						throw new InvalidCastException(Messages.JsonValueTypeNotMatch);
					}
				}
				else
				{
					string v = str;
					propertyValue = v;
				}
			}
			else if (value.Is<JInteger>(out integer))
			{
				if (!type.IsJsonNumber())
				{
					throw new InvalidCastException(Messages.JsonValueTypeNotMatch);
				}
				propertyValue = GetIntegerPropertyValue(type, integer);
			}
			else if (value.Is<JBoolean>(out boolean))
			{
				if (type != typeof (bool))
				{
					throw new InvalidCastException(Messages.JsonValueTypeNotMatch);
				}
				bool v = boolean;
				propertyValue = v;
			}
			else if (value.Is<JDecimal>(out floating))
			{
				if (type != typeof (float) && type != typeof (double) && type != typeof (decimal))
				{
					throw new InvalidCastException(Messages.JsonValueTypeNotMatch);
				}
				if (type == typeof (float))
				{
					propertyValue = floating.AsFloat();
				}
				else if (type == typeof (double))
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
				integerObj = integer.AsLong();
			}
			else if (propertyType == typeof (int))
			{
				integerObj = integer.AsInt();
			}
			else if (propertyType == typeof (byte))
			{
				integerObj = integer.AsByte();
			}
			else if (propertyType == typeof (sbyte))
			{
				integerObj = integer.AsSbyte();
			}
			else if (propertyType == typeof (short))
			{
				integerObj = integer.AsShort();
			}
			else if (propertyType == typeof (double))
			{
				integerObj = integer.AsDouble();
			}
			else if (propertyType == typeof (float))
			{
				integerObj = integer.AsFloat();
			}
			else if (propertyType == typeof (decimal))
			{
				integerObj = integer.AsDecimal();
			}
			else if (propertyType == typeof (ulong))
			{
				integerObj = integer.AsULong();
			}
			else if (propertyType == typeof (uint))
			{
				integerObj = integer.AsUInt();
			}
			else if (propertyType == typeof (ushort))
			{
				integerObj = integer.AsUShort();
			}

			return integerObj;
		}
	}
}
