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
using System.Reflection;
using System.Text;

namespace Commons.Json.Mapper
{
	internal class MapEngine<T> : IMapEngine<T>
	{
		private MapperContainer mappers;
        private T target;
        private TypeCache typeCache;

		public MapEngine(T target, MapperContainer mappers, TypeCache typeCache)
		{
			this.mappers = mappers;
            this.target = target;
            this.typeCache = typeCache;
		}

		public T Map(JValue jsonValue)
		{
			if (jsonValue.Is<JString>() || jsonValue.Is<JBoolean>()
			    || jsonValue.Is<JInteger>() || jsonValue.Is<JDecimal>())
			{
				return (T)ExtractPrimitiveValue(jsonValue, typeof (T));
			}

			if (jsonValue.Is<JNull>())
			{
				return default(T);
			}

			Populate(target, jsonValue);

            return target;
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
            if (type.IsJsonNumber() || type == typeof(bool))
            {
                json = target.ToString();
            }
            else if (type == typeof(string) || type == typeof(DateTime))
            {
                var sb = new StringBuilder();
                sb.Append(JsonTokens.Quoter).Append(target).Append(JsonTokens.Quoter);
                json = sb.ToString();
            }
            else if (type.IsDictionary())
            {
            }
            else if (type.InstanceOf(typeof(IEnumerable)))
            {
            }
            else
            {
                var manager = typeCache[type];
                var sb = new StringBuilder();
                sb.Append(JsonTokens.LeftBrace);
                foreach (var prop in manager.Properties)
                {
                    var propValue = prop.GetValue(target);
                    sb.Append(JsonTokens.Quoter);
                    sb.Append(prop.Name);
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

        private void Populate(object target, JValue jsonValue)
        {
            JObject jsonObj;
            JArray jsonArray;
            if (jsonValue.Is<JObject>(out jsonObj))
            {
	            PopulateJsonObject(target, jsonObj);
            }
			else if (jsonValue.Is<JArray>(out jsonArray))
			{
				var type = target.GetType();
				Type itemType;
				if (!type.IsList(out itemType))
				{
					throw new InvalidCastException(Messages.JsonValueTypeNotMatch);
				}
                var add = type.GetMethod(Messages.AddMethod);
				foreach (var value in jsonArray)
				{
					var arrayItemValue = GetValueFromJsonArrayItem(value, itemType);
                    add.Invoke(target, new [] {arrayItemValue});
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
				var itemValue = typeCache.Instantiate(itemType);
				Populate(itemValue, jsonValue);
				return itemValue;
			}
		}

		private void PopulateJsonObject(object target, JObject jsonObj)
		{
			var type = target.GetType();
			var properties = typeCache[type].Properties;
			foreach (var prop in properties)
			{
				if (jsonObj.ContainsKey(prop.Name))
				{
					var propertyType = prop.PropertyType;
					if (propertyType.IsJsonPrimitive())
					{
						PopulateJsonPrimitive(target, jsonObj, prop);
					}
					else
					{
						var propValue = prop.GetValue(target);
						Populate(propValue, jsonObj[prop.Name]);
					}
				}
			}
		}

		private static void PopulateJsonPrimitive(object target, JObject jsonObj, PropertyInfo prop)
		{
			var propertyType = prop.PropertyType;
			var name = prop.Name;

			var value = jsonObj[name];
			var valueType = value.GetType();
			if (valueType == typeof (JArray) || valueType == typeof (JObject))
			{
				throw new InvalidCastException(Messages.JsonValueTypeNotMatch);
			}

			var propertyValue = ExtractPrimitiveValue(value, propertyType);

			prop.SetValue(target, propertyValue);
		}

		private static object ExtractPrimitiveValue(JValue value, Type type)
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
					if (DateTime.TryParse(str, out dt))
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
