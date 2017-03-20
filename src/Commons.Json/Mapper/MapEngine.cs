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
using System.Collections.Concurrent;
using System.Globalization;
using System.Reflection;
using Commons.Utils;

namespace Commons.Json.Mapper
{
    internal sealed class MapEngine
    {
        private readonly TypeContainer types;
        private readonly MapperContainer mappers;
        private readonly ConfigContainer configuration;
        private readonly CollectionBuilder colBuilder;
		// objectType -> deserializer
		private readonly ConcurrentDictionary<Type, Func<Type, JValue, object>> deserializers;
        private readonly EnumCache enumCache;
		private readonly DictReflector dictReflector;

        public MapEngine(TypeContainer types, MapperContainer mappers, ConfigContainer configuration, 
			CollectionBuilder colBuilder, ConcurrentDictionary<Type, Func<Type, JValue, object>> deserializers, EnumCache enumCache, DictReflector dictReflector)
        {
            this.types = types;
            this.mappers = mappers;
            this.configuration = configuration;
            this.colBuilder = colBuilder;
			this.deserializers = deserializers;
			this.dictReflector = dictReflector;
            this.enumCache = enumCache;
        }

		public object Map(Type type, JValue jsonValue)
		{
			var deserializer = GetDeserializer(type);
			return deserializer(type, jsonValue); 
		}

		private Func<Type, JValue, object> GetDeserializer(Type type)
		{
			Func<Type, JValue, object> deserializer;
			if (deserializers.TryGetValue(type, out deserializer))
			{
				return deserializer;
			}


			var mapper = mappers.GetMapper(type);
            // for manual create now, assuming underlying type == type
            if (mapper.ManualCreate != null)
            {
                deserializer = (t, v) => mapper.ManualCreate(v);
            }
            else if ((type.IsInterface() || type.IsAbstract()) && mapper.Create == null)
            {
                deserializer = BuildNull;
            }
            else if (type == typeof(bool))
            {
                deserializer = BuildBool;
            }
            else if (type == typeof(bool?))
            {
                deserializer = BuildNullableBool;
            }
            else if (type == typeof(double))
            {
                deserializer = BuildDouble;
            }
            else if (type == typeof(double?))
            {
                deserializer = BuildNullableDouble;
            }
            else if (type == typeof(decimal))
            {
                deserializer = BuildDecimal;
            }
            else if (type == typeof(decimal?))
            {
                deserializer = BuildNullableDecimal;
            }
            else if (type == typeof(float))
            {
                deserializer = BuildFloat;
            }
            else if (type == typeof(float?))
            {
                deserializer = BuildNullableFloat;
            }
            else if (type == typeof(int))
            {
                deserializer = BuildInt;
            }
            else if (type == typeof(int?))
            {
                deserializer = BuildNullableInt;
            }
            else if (type == typeof(long))
            {
                deserializer = BuildLong;
            }
            else if (type == typeof(long?))
            {
                deserializer = BuildNullableLong;
            }
            else if (type == typeof(short))
            {
                deserializer = BuildShort;
            }
            else if (type == typeof(short?))
            {
                deserializer = BuildNullableShort;
            }
            else if (type == typeof(uint))
            {
                deserializer = BuildUint;
            }
            else if (type == typeof(uint?))
            {
                deserializer = BuildNullableUint;
            }
            else if (type == typeof(ulong))
            {
                deserializer = BuildUlong;
            }
            else if (type == typeof(ulong?))
            {
                deserializer = BuildNullableUlong;
            }
            else if (type == typeof(ushort))
            {
                deserializer = BuildUshort;
            }
            else if (type == typeof(ushort?))
            {
                deserializer = BuildNullableUshort;
            }
            else if (type == typeof(byte))
            {
                deserializer = BuildByte;
            }
            else if (type == typeof(byte?))
            {
                deserializer = BuildNullableByte;
            }
            else if (type == typeof(sbyte))
            {
                deserializer = BuildSbyte;
            }
            else if (type == typeof(sbyte?))
            {
                deserializer = BuildNullableSbyte;
            }
            else if (type.IsEnum())
            {
                deserializer = BuildEnum;
            }
            else if (type == typeof(Guid))
            {
                deserializer = BuildGuid;
            }
            else if (type == typeof(Guid?))
            {
                deserializer = BuildNullableGuid;
            }
            else if (type == typeof(char))
            {
                deserializer = BuildChar;
            }
            else if (type == typeof(char?))
            {
                deserializer = BuildNullableChar;
            }
            else if (type == typeof(string))
            {
                deserializer = BuildString;
            }
            else if (type == typeof(DateTime))
            {
                deserializer = BuildTime;
            }
            else if (type == typeof(DateTime?))
            {
                deserializer = BuildNullableTime;
            }
            else if (type == typeof(byte[]))
            {
                deserializer = BuildByteArray;
            }
            else if (type.IsDictionary())
            {
                deserializer = BuildDict;
            }
            else if (type.IsArray)
            {
                deserializer = BuildArray;
            }
            else if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                deserializer = BuildEnumerable;
            }
            else if (type.IsClass())
            {
                deserializer = BuildObject;
            }
            else
            {
                var underlying = Nullable.GetUnderlyingType(type);
                if (underlying != null)
                {
                    if (underlying.IsEnum())
                    {
                        deserializer = BuildNullableEnum;
                    }
                    else
                    {
                        deserializer = BuildObject;
                    }
                }
                else
                {
                    deserializer = BuildStruct;
                }
            }

            deserializers[type] = deserializer;

			return deserializer;
		}

		private object BuildNull(Type type, JValue jsonValue)
		{
			return null;
		}

        private object BuildEnumerable(Type targetType, JValue jsonValue)
        {
            if (jsonValue == JNull.Null)
            {
                return null;
            }

            //TODO: how about two type arguments?
            var itemType = targetType.GetGenericArguments()[0];
            IEnumerable raw;
            var mapper = mappers.GetMapper(targetType);

            if (mapper.Create != null)
            {
                raw = mapper.Create() as IEnumerable;
            }
            else
            {
				raw = colBuilder.Construct(targetType);
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

        private object BuildBool(Type type, JValue jsonValue)
        {
            if (jsonValue == JNull.Null)
            {
                throw new InvalidCastException(Messages.CannotAssignNullToStruct);
            }
			bool result;
            var bvalue = jsonValue as JNumber;
            if (jsonValue == JBool.True)
            {
                result = true;
            }
            else if (jsonValue == JBool.False)
            {
                result = false;
            }
            else
            {
                throw new InvalidCastException(Messages.JsonValueTypeNotMatch);
            }

            return result;
        }

        private object BuildNullableBool(Type type, JValue jsonValue)
        {
            if (jsonValue == JNull.Null)
            {
                return null;
            }
            return BuildBool(type, jsonValue);
        }

        private object BuildDecimal(Type type, JValue jsonValue)
        {
            if (jsonValue == JNull.Null)
            {
                throw new InvalidCastException(Messages.CannotAssignNullToStruct);
            }
            object result;
			CheckDecimalType(jsonValue);
			try
			{
				result = decimal.Parse(jsonValue.Value);
			}
			catch (Exception e)
			{
				throw new InvalidCastException(Messages.JsonValueTypeNotMatch, e);
			}
            return result;
        }

        private object BuildNullableDecimal(Type type, JValue jsonValue)
        {
            if (jsonValue == JNull.Null)
            {
                return null;
            }
            return BuildDecimal(type, jsonValue);
        }

        private object BuildDouble(Type type, JValue jsonValue)
        {
            if (jsonValue == JNull.Null)
            {
                throw new InvalidCastException(Messages.CannotAssignNullToStruct);
            }
            object result;
			CheckDecimalType(jsonValue);
			try
			{
				result = double.Parse(jsonValue.Value);
			}
			catch (Exception e)
			{
				throw new InvalidCastException(Messages.JsonValueTypeNotMatch, e);
			}
            return result;
        }

        private object BuildNullableDouble(Type type, JValue jsonValue)
        {
            if (jsonValue == JNull.Null)
            {
                return null;
            }
            return BuildDouble(type, jsonValue);
        }

        private object BuildFloat(Type type, JValue jsonValue)
        {
            if (jsonValue == JNull.Null)
            {
                throw new InvalidCastException(Messages.CannotAssignNullToStruct);
            }
            object result;
			CheckDecimalType(jsonValue);
			try
			{
				result = float.Parse(jsonValue.Value);
			}
			catch (Exception e)
			{
				throw new InvalidCastException(Messages.JsonValueTypeNotMatch, e);
			}
            return result;
        }

        private object BuildNullableFloat(Type type, JValue jsonValue)
        {
            if (jsonValue == JNull.Null)
            {
                return null;
            }
            return BuildFloat(type, jsonValue);
        }

		private void CheckDecimalType(JValue jsonValue)
		{
			JNumber prim;
            if (!jsonValue.Is<JNumber>(out prim) || (prim.NumType != NumberType.Decimal && prim.NumType != NumberType.Integer))
            {
                throw new InvalidCastException(Messages.JsonValueTypeNotMatch);
            }
		}
		
        private object BuildInt(Type type, JValue jsonValue)
        {
            if (jsonValue == JNull.Null)
            {
                throw new InvalidCastException(Messages.CannotAssignNullToStruct);
            }
            object result;
			CheckIntegerType(jsonValue);
			try
			{
				result = int.Parse(jsonValue.Value);
			}
			catch (Exception e)
			{
				throw new InvalidCastException(Messages.JsonValueTypeNotMatch, e);
			}
            return result;
        }

        private object BuildNullableInt(Type type, JValue jsonValue)
        {
            if (jsonValue == JNull.Null)
            {
                return null;
            }
            return BuildInt(type, jsonValue);
        }
		
        private object BuildLong(Type type, JValue jsonValue)
        {
            if (jsonValue == JNull.Null)
            {
                throw new InvalidCastException(Messages.CannotAssignNullToStruct);
            }
            object result;
			CheckIntegerType(jsonValue);
			try
			{
				result = long.Parse(jsonValue.Value);
			}
			catch (Exception e)
			{
				throw new InvalidCastException(Messages.JsonValueTypeNotMatch, e);
			}
            return result;
        }

        private object BuildNullableLong(Type type, JValue jsonValue)
        {
            if (jsonValue == JNull.Null)
            {
                return null;
            }
            return BuildLong(type, jsonValue);
        }
		
        private object BuildShort(Type type, JValue jsonValue)
        {
            if (jsonValue == JNull.Null)
            {
                throw new InvalidCastException(Messages.CannotAssignNullToStruct);
            }
            object result;
			CheckIntegerType(jsonValue);
			try
			{
				result = short.Parse(jsonValue.Value);
			}
			catch (Exception e)
			{
				throw new InvalidCastException(Messages.JsonValueTypeNotMatch, e);
			}
            return result;
        }

        private object BuildNullableShort(Type type, JValue jsonValue)
        {
            if (jsonValue == JNull.Null)
            {
                return null;
            }
            return BuildShort(type, jsonValue);
        }
		
        private object BuildUint(Type type, JValue jsonValue)
        {
            if (jsonValue == JNull.Null)
            {
                throw new InvalidCastException(Messages.CannotAssignNullToStruct);
            }
            object result;
			CheckIntegerType(jsonValue);
			try
			{
				result = uint.Parse(jsonValue.Value);
			}
			catch (Exception e)
			{
				throw new InvalidCastException(Messages.JsonValueTypeNotMatch, e);
			}
            return result;
        }

        private object BuildNullableUint(Type type, JValue jsonValue)
        {
            if (jsonValue == JNull.Null)
            {
                return null;
            }
            return BuildUint(type, jsonValue);
        }
		
        private object BuildUlong(Type type, JValue jsonValue)
        {
            if (jsonValue == JNull.Null)
            {
                throw new InvalidCastException(Messages.CannotAssignNullToStruct);
            }
            object result;
			CheckIntegerType(jsonValue);
			try
			{
				result = ulong.Parse(jsonValue.Value);
			}
			catch (Exception e)
			{
				throw new InvalidCastException(Messages.JsonValueTypeNotMatch, e);
			}
            return result;
        }

        private object BuildNullableUlong(Type type, JValue jsonValue)
        {
            if (jsonValue == JNull.Null)
            {
                return null;
            }
            return BuildUlong(type, jsonValue);
        }
		
        private object BuildUshort(Type type, JValue jsonValue)
        {
            if (jsonValue == JNull.Null)
            {
                throw new InvalidCastException(Messages.CannotAssignNullToStruct);
            }
            object result;
			CheckIntegerType(jsonValue);
			try
			{
				result = ushort.Parse(jsonValue.Value);
			}
			catch (Exception e)
			{
				throw new InvalidCastException(Messages.JsonValueTypeNotMatch, e);
			}
            return result;
        }

        private object BuildNullableUshort(Type type, JValue jsonValue)
        {
            if (jsonValue == JNull.Null)
            {
                return null;
            }
            return BuildUshort(type, jsonValue);
        }
		
        private object BuildByte(Type type, JValue jsonValue)
        {
            if (jsonValue == JNull.Null)
            {
                throw new InvalidCastException(Messages.CannotAssignNullToStruct);
            }
            object result;
			CheckIntegerType(jsonValue);
			try
			{
				result = byte.Parse(jsonValue.Value);
			}
			catch (Exception e)
			{
				throw new InvalidCastException(Messages.JsonValueTypeNotMatch, e);
			}
            return result;
        }

        private object BuildNullableByte(Type type, JValue jsonValue)
        {
            if (jsonValue == JNull.Null)
            {
                return null;
            }
            return BuildByte(type, jsonValue);
        }
		
        private object BuildSbyte(Type type, JValue jsonValue)
        {
            if (jsonValue == JNull.Null)
            {
                throw new InvalidCastException(Messages.CannotAssignNullToStruct);
            }
            object result;
			CheckIntegerType(jsonValue);
			try
			{
				result = sbyte.Parse(jsonValue.Value);
			}
			catch (Exception e)
			{
				throw new InvalidCastException(Messages.JsonValueTypeNotMatch, e);
			}
            return result;
        }

        private object BuildNullableSbyte(Type type, JValue jsonValue)
        {
            if (jsonValue == JNull.Null)
            {
                return null;
            }
            return BuildSbyte(type, jsonValue);
        }

		private void CheckIntegerType(JValue jsonValue)
		{
			JNumber prim;
			if (!jsonValue.Is<JNumber>(out prim) || prim.NumType != NumberType.Integer)
			{
				throw new InvalidCastException(Messages.JsonValueTypeNotMatch);
			}
		}

		private object BuildEnum(Type type, JValue jsonValue)
		{
            if (jsonValue == JNull.Null)
            {
                throw new InvalidCastException(Messages.CannotAssignNullToStruct);
            }
			var str = CheckJsonString(jsonValue);
            // TODO: what to do when exception.
            var enumValue = enumCache.GetValue(type, str);
            if (enumValue == null)
            {
                throw new InvalidCastException(Messages.InvalidEnumValue);
            }
            return enumValue;
		}

        private object BuildNullableEnum(Type type, JValue jsonValue)
        {
            if (jsonValue == JNull.Null)
            {
                return null;
            }
            return BuildEnum(type, jsonValue);
        }

		private object BuildGuid(Type type, JValue jsonValue)
		{
            if (jsonValue == JNull.Null)
            {
                throw new InvalidCastException(Messages.CannotAssignNullToStruct);
            }
			Guid guid;
			if (!Guid.TryParse(jsonValue.Value, out guid))
			{
				throw new ArgumentException(Messages.InvalidValue);
			}

			return guid;
		}

        private object BuildNullableGuid(Type type, JValue jsonValue)
        {
            if (jsonValue == JNull.Null)
            {
                return null;
            }
            return BuildGuid(type, jsonValue);
        }

		private object BuildChar(Type type, JValue jsonValue)
		{
            if (jsonValue == JNull.Null)
            {
                throw new InvalidCastException(Messages.CannotAssignNullToStruct);
            }
			var str = CheckJsonString(jsonValue);
			char result;
			if (!char.TryParse(str, out result))
			{
				throw new ArgumentException(Messages.InvalidValue);
			}
			return result;
		}

        private object BuildNullableChar(Type type, JValue jsonValue)
        {
            if (jsonValue == JNull.Null)
            {
                return null;
            }
            return BuildChar(type, jsonValue);
        }

		private object BuildTime(Type type, JValue jsonValue)
		{
            if (jsonValue == JNull.Null)
            {
                throw new InvalidCastException(Messages.CannotAssignNullToStruct);
            }
			DateTime dt;
			if (!TryParseDate(jsonValue.Value, out dt))
			{
				throw new ArgumentException(Messages.InvalidDateFormat);
			}

			return dt;
		}

        private object BuildNullableTime(Type type, JValue jsonValue)
        {
            if (jsonValue == JNull.Null)
            {
                return null;
            }
            return BuildTime(type, jsonValue);
        }

		private object BuildByteArray(Type type, JValue jsonValue)
		{
			JString str;
			byte[] bytes = null;
			if (jsonValue.Is<JString>(out str))
			{
				bytes = Convert.FromBase64String(str.Value);
			}
			else
			{
				JArray jsonArray;
				if (jsonValue.Is<JArray>(out jsonArray))
				{
					bytes = new byte[jsonArray.Count];
					for (var i = 0; i < jsonArray.Count; i++)
					{
						bytes[i] = (byte)Map(typeof(byte), jsonArray[i]);
					}
				}
				else
				{
					throw new InvalidCastException(Messages.JsonValueTypeNotMatch);
				}
			}

			return bytes;
		}

        private object BuildString(Type type, JValue jsonValue)
        {
            if (jsonValue == JNull.Null)
            {
                return null;
            }
			var str = CheckJsonString(jsonValue);
			return str;
        }

		private string CheckJsonString(JValue jsonValue)
		{
			JString str;
			if (!jsonValue.Is<JString>(out str))
			{
				throw new InvalidCastException(Messages.JsonValueTypeNotMatch);
			}

			return str.Value;
		}

        private Array BuildArray(Type arrayType, JValue jsonValue)
        {
            if (jsonValue == JNull.Null)
            {
                return null;
            }
            JArray jsonArray;
            Array array;
			var itemType = arrayType.GetElementType();
			//TODO :do not support array manual create.
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

        private object BuildStruct(Type type, JValue jsonValue)
        {
            if (jsonValue == JNull.Null)
            {
                throw new InvalidCastException(Messages.CannotAssignNullToStruct);
            }

            return BuildObject(type, jsonValue);
        }

        private object BuildObject(Type type, JValue jsonValue)
        {
            if (jsonValue == JNull.Null)
            {
                return null;
            }

			var jsonObj = CheckJsonObject(jsonValue);

            var properties = types[type].Setters;

			object target;
            var mapper = mappers.GetMapper(type);
			if (mapper.Create != null)
			{
				target = mapper.Create();
			}
			else
			{
				target = types[type].Launch();
			}

			for (var i = 0; i < properties.Count; i++)
            {
				var prop = properties[i];
				var itemName = prop.Item1.Name;
                if (mapper.IsPropertyIgnored(itemName))
                {
                    continue;
                }

                var key = mapper.GetKey(itemName);
                var propertyType = prop.Item1.PropertyType;
				JValue jv;
                if (jsonObj.TryGetValue(key, out jv))
                {
                    jv = jsonObj[key];
                    var value = Map(propertyType, jv);
                    prop.Item2(target, value);
                }
            }

            return target;
        }

		private JObject CheckJsonObject(JValue jsonValue)
		{
			JObject jsonObj;
			if (!jsonValue.Is<JObject>(out jsonObj))
			{
				throw new InvalidCastException(Messages.JsonValueTypeNotMatch);
			}

			return jsonObj;
		}

		private object BuildDict(Type dictType, JValue jsonValue)
		{
            if (jsonValue == JNull.Null)
            {
                return null;
            }
			var jsonObj = CheckJsonObject(jsonValue);
			var keyType = dictType.GetGenericArguments()[0];
			var valueType = dictType.GetGenericArguments()[1];
			if (keyType != typeof(string))
			{
				throw new InvalidOperationException(Messages.JsonValueTypeNotMatch);
			}
			object raw;
			var mapper = mappers.GetMapper(dictType);
			var tuple = dictReflector.GetDeserializeDelegates(dictType, valueType);
			if (mapper.Create != null)
			{
				raw = mapper.Create();
			}
			else
			{
				var create = tuple.Item1;
				raw = create();
			}

			var add = tuple.Item2;
            foreach (var kvp in jsonObj)
            {
                var key = kvp.Key;
                object value = null;
                value = Map(valueType, kvp.Value);
				add(raw, key, value);
            }
            return raw;
		}

        private bool TryParseDate(string str, out DateTime dt)
        {
            if (!string.IsNullOrEmpty(configuration.DateFormat))
            {
                return DateTime.TryParseExact(str, configuration.DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out dt);
            }
            else
            {
				var success = false;
				dt = DateTime.MinValue;

				try
				{
					success = str.TryFastParseDateInvariantCulture(out dt);
				}
				catch (Exception)
				{
					success = false;
				}
				return success;
            }
        }
    }
}
