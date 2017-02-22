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
		private const int localBufferSize = 1000;
		private const int objectSize = 200;
        private readonly MapperContainer mappers;
        private readonly TypeContainer types;
        private readonly ConfigContainer config;
        private readonly StringBuilder localBuffer;
		private readonly ConcurrentDictionary<Type, Func<object, string>> serializers;

        public JsonBuilder(MapperContainer mappers, TypeContainer types, ConfigContainer config, 
			ConcurrentDictionary<Type, Func<object, string>> serializerMapper)
        {
	        localBuffer = new StringBuilder(localBufferSize);
			serializers = serializerMapper;
            this.mappers = mappers;
            this.types = types;
            this.config = config;
        }

        public string Build(object target)
        {
			var serializer = GetSerializer(target);
			return serializer(target);
        }

		private Func<object, string> GetSerializer(object target)
		{
			if (target == null)
			{
				return SerializeNull;
			}

			var type = target.GetType();
			if (serializers.ContainsKey(type))
			{
				return serializers[type];
			}
			
			Func<object, string> serializer;
			if (type == typeof(bool))
			{
				serializer = SerializeBoolean;
			}
			else if (type.IsJsonNumber())
			{
				serializer = SerializeNumber;
			}
			else if (type == typeof(string) || type.IsEnum() || type == typeof(Guid) || type == typeof(char))
			{
				serializer = SerializeString;
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

		private string SerializeByteArray(object target)
		{
			var bytes = (byte[])target;
			localBuffer.Length = 0;
			localBuffer.Append(JsonTokens.Quoter).Append(Convert.ToBase64String(bytes)).Append(JsonTokens.Quoter);
			return localBuffer.ToString();
		}

		private string SerializeObject(object target)
		{
			var type = target.GetType();
			var mapper = mappers.GetMapper(type);
			string json;
			if (mapper.Serializer != null)
			{
				json = mapper.Serializer(target).ToString();
			}
			else
			{
				var manager = types[type];
				var sb = new StringBuilder(objectSize);
				sb.Append(JsonTokens.LeftBrace);
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
					sb.Append(JsonTokens.Quoter);
					sb.Append(mapper.GetKey(itemName));
					sb.Append(JsonTokens.Quoter);
					sb.Append(JsonTokens.Colon);
					var serializer = GetSerializer(propValue);
					sb.Append(serializer(propValue));
					sb.Append(JsonTokens.Comma);
				}
				sb.Remove(sb.Length - 1, 1);
				sb.Append(JsonTokens.RightBrace);
				json = sb.ToString();
			}

			return json;
		}

		private string SerializeEnumerable(object target)
		{
			var sb = new StringBuilder(objectSize);
			sb.Append(JsonTokens.LeftBracket);
			var hasValue = false;
			var items = target as IEnumerable;
			foreach (var item in items)
			{
				var itemSerializer = GetSerializer(item);
				sb.Append(itemSerializer(item)).Append(JsonTokens.Comma);
				hasValue = true;
			}
			if (hasValue)
			{
				sb.Remove(sb.Length - 1, 1);
			}
			sb.Append(JsonTokens.RightBracket);
			return sb.ToString();
		}

		private string SerializeList(object target)
		{
			var sb = new StringBuilder(objectSize);
			sb.Append(JsonTokens.LeftBracket);
			var hasValue = false;
			var items = target as IList;
			for (var i = 0; i < items.Count; i++)
			{
				var item = items[i];
				sb.Append(Build(item)).Append(JsonTokens.Comma);
				hasValue = true;
			}

			if (hasValue)
			{
				sb.Remove(sb.Length - 1, 1);
			}
			sb.Append(JsonTokens.RightBracket);
			return sb.ToString();
		}

		private string SerializeNull(object target)
		{
			return JsonTokens.Null;
		}

		private string SerializeDict(object target)
		{
			var sb = new StringBuilder(objectSize);
			sb.Append(JsonTokens.LeftBrace);
			// TODO: serialize dict with key type is not string.
			var type = target.GetType();
			var keyType = type.GetGenericArguments()[0];
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
			return sb.ToString();
		}

		private string SerializeBoolean(object target)
		{
			string json;
			var val = (bool)target;
			if (val)
			{
				json = Messages.True;
			}
			else
			{
				json = Messages.False;
			}

			return json;
		}

		private string SerializeNumber(object target)
		{
			return target.ToString();
		}

		private string SerializeString(object target)
		{
            localBuffer.Length = 0;
            localBuffer.Append(JsonTokens.Quoter).Append(target).Append(JsonTokens.Quoter);
            return localBuffer.ToString();
		}

		private string SerializeTime(object target)
		{
			localBuffer.Length = 0;
			var dt = (DateTime) target;
			object dateFormat;
			var time = config.TryGetValue(Messages.DateFormat, out dateFormat) ? dt.ToString((string)dateFormat) : dt.FastToStringInvariantCulture();
			localBuffer.Append(JsonTokens.Quoter).Append(time).Append(JsonTokens.Quoter);
			return localBuffer.ToString();
		}

		private string SerializeArray(object target)
		{
			var array = (Array)target;
			var sb = new StringBuilder(objectSize);
			sb.Append(JsonTokens.LeftBracket);
			var hasValue = false;
			for (var i = 0; i < array.Length; i++)
			{
				var item = array.GetValue(i);
				sb.Append(Build(item)).Append(JsonTokens.Comma);
				hasValue = true;
			}
			if (hasValue)
			{
				sb.Remove(sb.Length - 1, 1);
			}
			sb.Append(JsonTokens.RightBracket);
			return sb.ToString();
		}
    }

}
