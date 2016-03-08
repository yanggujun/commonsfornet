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
using System.Collections.Generic;
using System.Reflection;

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
			Populate(target, jsonValue);

            return target;
		}

		public JValue Map(T target)
		{
			throw new NotImplementedException();
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
					var arrayValue = typeCache.Instantiate(itemType);
					Populate(arrayValue, value);
                    add.Invoke(target, new [] {arrayValue});
				}
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

			JString str;
			JInteger integer;
			JBoolean boolean;
			JDecimal floating;
			if (value.Is<JString>(out str))
			{
				if (propertyType != typeof (string) && propertyType != typeof(DateTime))
				{
					throw new InvalidCastException(Messages.JsonValueTypeNotMatch);
				}
				if (propertyType == typeof (DateTime))
				{
					DateTime dt;
					if (DateTime.TryParse(str, out dt))
					{
						prop.SetValue(target, dt);
					}
					else
					{
						throw new InvalidCastException(Messages.JsonValueTypeNotMatch);
					}
				}
				else
				{
					string v = str;
					prop.SetValue(target, v);
				}
			}
			else if (value.Is<JInteger>(out integer))
			{
				if (!propertyType.IsJsonNumber())
				{
					throw new InvalidCastException(Messages.JsonValueTypeNotMatch);
				}
				PopulateInteger(target, prop, propertyType, integer);
			}
			else if (value.Is<JBoolean>(out boolean))
			{
				if (propertyType != typeof (bool))
				{
					throw new InvalidCastException(Messages.JsonValueTypeNotMatch);
				}
				bool v = boolean;
				prop.SetValue(target, v);
			}
			else if (value.Is<JDecimal>(out floating))
			{
				if (propertyType != typeof (float) && propertyType != typeof (double) && propertyType != typeof (decimal))
				{
					throw new InvalidCastException(Messages.JsonValueTypeNotMatch);
				}
				if (propertyType == typeof (float))
				{
					prop.SetValue(target, floating.AsFloat());
				}
				else if (propertyType == typeof (double))
				{
					prop.SetValue(target, floating.AsDouble());
				}
				else
				{
					prop.SetValue(target, floating.AsDecimal());
				}
			}
			else if (value.Is<JNull>())
			{
				prop.SetValue(target, null);
			}
			else
			{
				// Unlikely to happen.
				throw new InvalidCastException(Messages.JsonValueTypeNotMatch);
			}
		}

		private static void PopulateInteger(object target, PropertyInfo prop, Type propertyType, JInteger integer)
		{
			if (propertyType == typeof (long))
			{
				prop.SetValue(target, integer.AsLong());
			}
			else if (propertyType == typeof (int))
			{
				prop.SetValue(target, integer.AsInt());
			}
			else if (propertyType == typeof (byte))
			{
				prop.SetValue(target, integer.AsByte());
			}
			else if (propertyType == typeof (short))
			{
				prop.SetValue(target, integer.AsShort());
			}
			else if (propertyType == typeof (double))
			{
				prop.SetValue(target, integer.AsDouble());
			}
			else if (propertyType == typeof (float))
			{
				prop.SetValue(target, integer.AsFloat());
			}
			else if (propertyType == typeof (decimal))
			{
				prop.SetValue(target, integer.AsDecimal());
			}
			else if (propertyType == typeof (ulong))
			{
				prop.SetValue(target, integer.AsULong());
			}
			else if (propertyType == typeof (uint))
			{
				prop.SetValue(target, integer.AsUInt());
			}
			else if (propertyType == typeof (ushort))
			{
				prop.SetValue(target, integer.AsUShort());
			}
		}
	}
}
