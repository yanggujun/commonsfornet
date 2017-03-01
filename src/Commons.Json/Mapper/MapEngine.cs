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
		// (objectType -> (deserializer, actualType)
		private readonly ConcurrentDictionary<Type, Tuple<Func<Type, JValue, object>, Type>> deserializers;

        public MapEngine(TypeContainer types, MapperContainer mappers, ConfigContainer configuration, 
			CollectionBuilder colBuilder, ConcurrentDictionary<Type, Tuple<Func<Type, JValue, object>, Type>> deserializers)
        {
            this.types = types;
            this.mappers = mappers;
            this.configuration = configuration;
            this.colBuilder = colBuilder;
			this.deserializers = deserializers;
        }

		public object Map(Type type, JValue jsonValue)
		{
            if (jsonValue.Is<JNull>())
            {
                if (!type.IsClass() && !type.IsNullable())
                {
                    throw new InvalidCastException(Messages.CannotAssignNullToStruct);
                }
                return null;
            }

			Type underlyingType;
			var deserializer = GetDeserializer(type, out underlyingType);
			return deserializer(underlyingType, jsonValue); 
		}

		private Func<Type, JValue, object> GetDeserializer(Type type, out Type actualType)
		{
			if (deserializers.ContainsKey(type))
			{
				var tuple = deserializers[type];
				actualType = tuple.Item2;
				return tuple.Item1;
			}
			actualType = GetActualType(type);

			Func<Type, JValue, object> deserializer;

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
			else if (actualType == typeof(bool))
			{
				deserializer = BuildBool;
			}
			else if (actualType == typeof(double))
			{
				deserializer = BuildDouble;
			}
			else if (actualType == typeof(decimal))
			{
				deserializer = BuildDecimal;
			}
			else if (actualType == typeof(float))
			{
				deserializer = BuildFloat;
			}
			else if (actualType == typeof(int))
			{
				deserializer = BuildInt;
			}
			else if (actualType == typeof(long))
			{
				deserializer = BuildLong;
			}
			else if (actualType == typeof(short))
			{
				deserializer = BuildShort;
			}
			else if (actualType == typeof(uint))
			{
				deserializer = BuildUint;
			}
			else if (actualType == typeof(ulong))
			{
				deserializer = BuildUlong;
			}
			else if (actualType == typeof(ushort))
			{
				deserializer = BuildUshort;
			}
			else if (actualType == typeof(byte))
			{
				deserializer = BuildByte;
			}
			else if (actualType == typeof(sbyte))
			{
				deserializer = BuildSbyte;
			}
			else if (actualType.IsEnum())
			{
				deserializer = BuildEnum;
			}
			else if (actualType == typeof(Guid))
			{
				deserializer = BuildGuid;
			}
			else if (actualType == typeof(char))
			{
				deserializer = BuildChar;
			}
			else if (actualType == typeof(string))
			{
				deserializer = BuildString;
			}
			else if (actualType == typeof(DateTime))
			{
				deserializer = BuildTime;
			}
			else if (actualType == typeof(byte[]))
			{
				deserializer = BuildByteArray;
			}
			else if (actualType.IsDictionary())
			{
				deserializer = BuildDict;
			}
			else if (actualType.IsArray)
			{
				deserializer = BuildArray;
			}
			else if (typeof(IEnumerable).IsAssignableFrom(actualType))
			{
				deserializer = BuildEnumerable;
			}
			else
			{
				deserializer = BuildObject;
			}

			deserializers[type] = new Tuple<Func<Type, JValue, object>, Type>(deserializer, actualType);

			return deserializer;
		}

		private object BuildNull(Type type, JValue jsonValue)
		{
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
			bool result;
            var bvalue = jsonValue as JPrimitive;
            if (bvalue.PrimitiveType == PrimitiveType.True)
            {
                result = true;
            }
            else if (bvalue.PrimitiveType == PrimitiveType.False)
            {
                result = false;
            }
            else
            {
                throw new InvalidCastException(Messages.JsonValueTypeNotMatch);
            }

            return result;
        }

        private object BuildDecimal(Type type, JValue jsonValue)
        {
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

        private object BuildDouble(Type type, JValue jsonValue)
        {
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

        private object BuildFloat(Type type, JValue jsonValue)
        {
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

		private void CheckDecimalType(JValue jsonValue)
		{
			JPrimitive prim;
            if (!jsonValue.Is<JPrimitive>(out prim) || (prim.PrimitiveType != PrimitiveType.Decimal && prim.PrimitiveType != PrimitiveType.Integer))
            {
                throw new InvalidCastException(Messages.JsonValueTypeNotMatch);
            }
		}
		
        private object BuildInt(Type type, JValue jsonValue)
        {
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
		
        private object BuildLong(Type type, JValue jsonValue)
        {
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
		
        private object BuildShort(Type type, JValue jsonValue)
        {
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
		
        private object BuildUint(Type type, JValue jsonValue)
        {
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
		
        private object BuildUlong(Type type, JValue jsonValue)
        {
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
		
        private object BuildUshort(Type type, JValue jsonValue)
        {
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
		
        private object BuildByte(Type type, JValue jsonValue)
        {
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
		
        private object BuildSbyte(Type type, JValue jsonValue)
        {
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

		private void CheckIntegerType(JValue jsonValue)
		{
			JPrimitive prim;
			if (!jsonValue.Is<JPrimitive>(out prim) || prim.PrimitiveType != PrimitiveType.Integer)
			{
				throw new InvalidCastException(Messages.JsonValueTypeNotMatch);
			}
		}

		private object BuildEnum(Type type, JValue jsonValue)
		{
			var str = CheckJsonString(jsonValue);
			// TODO: what to do when exception.
			return Enum.Parse(type, str);
		}

		private object BuildGuid(Type type, JValue jsonValue)
		{
			Guid guid;
			if (!Guid.TryParse(jsonValue.Value, out guid))
			{
				throw new ArgumentException(Messages.InvalidValue);
			}

			return guid;
		}

		private object BuildChar(Type type, JValue jsonValue)
		{
			var str = CheckJsonString(jsonValue);
			char result;
			if (!char.TryParse(str, out result))
			{
				throw new ArgumentException(Messages.InvalidValue);
			}
			return result;
		}

		private object BuildTime(Type type, JValue jsonValue)
		{
			DateTime dt;
			if (!TryParseDate(jsonValue.Value, out dt))
			{
				throw new ArgumentException(Messages.InvalidDateFormat);
			}

			return dt;
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

        private object BuildObject(Type type, JValue jsonValue)
        {
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
                if (jsonObj.ContainsKey(key))
                {
                    var jv = jsonObj[key];
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
			var jsonObj = CheckJsonObject(jsonValue);
			var keyType = dictType.GetGenericArguments()[0];
			var valueType = dictType.GetGenericArguments()[1];
			if (keyType != typeof(string))
			{
				throw new InvalidOperationException(Messages.JsonValueTypeNotMatch);
			}
			object raw;
			var mapper = mappers.GetMapper(dictType);
			if (mapper.Create != null)
			{
				raw = mapper.Create();
			}
			else
			{
				raw = Activator.CreateInstance(dictType);
			}

            var method = dictType.GetMethod(Messages.AddMethod);
            foreach (var kvp in jsonObj)
            {
                var key = kvp.Key;
                object value = null;
                value = Map(valueType, kvp.Value);
                method.Invoke(raw, new[] { key, value });
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

		private Type GetActualType(Type type)
		{
			Type actualType;
			if (!type.IsNullable(out actualType))
			{
				actualType = type;
			}
			return actualType;
		}
    }
}
