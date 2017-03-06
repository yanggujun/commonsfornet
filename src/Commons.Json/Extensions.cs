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
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Commons.Json
{
    internal static class Extensions
    {
        public static void Verify<T>(this T x, Predicate<T> check)
        {
            if (!check(x))
            {
                throw new ArgumentException(Messages.InvalidFormat);
            }
        }

        public static bool Is<T>(this object obj) where T : class
        {
            var target = obj as T;
            return target != null;
        }

        public static bool Is<T>(this object obj, out T target) where T : class
        {
            target = obj as T;
            return target != null;
        }

        public static bool IsEmpty(this char ch)
        {
            return ch == JsonTokens.Space || ch == JsonTokens.TabChar
                   || ch == JsonTokens.LineSeparator || ch == JsonTokens.Carriage;
        }

        public static bool IsJsonPrimitive(this Type type)
        {
            return type.IsPrimitive() || type == typeof(decimal) 
                || type == typeof (string) || type == typeof(DateTime) || type == typeof(Guid)
                || type.IsEnum() || IsNullablePrimitive(type);
        }

        public static bool IsNullable(this Type type)
        {
            Type underlyingType;
            return IsNullable(type, out underlyingType);
        }

        public static bool IsNullable(this Type type, out Type underlyingType)
        {
			underlyingType = Nullable.GetUnderlyingType(type);
			return underlyingType != null;
        }

        public static bool IsNullablePrimitive(this Type type)
        {
            Type underlying;
            if (IsNullable(type, out underlying))
            {
                return IsJsonPrimitive(underlying);
            }

            return false;
        }

        public static bool IsJsonNumber(this Type type)
        {
            return type == typeof (int) || type == typeof (long)
                   || type == typeof (short) || type == typeof (sbyte)
                   || type == typeof (byte) || type == typeof (uint)
                   || type == typeof (ulong) || type == typeof (ushort)
                   || type == typeof (double) || type == typeof (float)
                   || type == typeof (decimal);
        }

        public static bool IsJsonInteger(this Type type)
        {
            return type == typeof(int) || type == typeof(long) 
                || type == typeof(short) || type == typeof(byte) 
                || type == typeof(uint) || type == typeof(ulong);
        }

        public static bool IsJsonDecimal(this Type type)
        {
            return type == typeof(double) || type == typeof(float) || type == typeof(decimal);
        }

        public static bool IsJsonArray(this Type type)
        {
            return type.IsArray || type == typeof (IList<>) || type == typeof (ArrayList);
        }

        public static bool IsCollection(this Type type)
        {
            Type itemType;
            return type.IsCollection(out itemType);
        }

        public static bool IsCollection(this Type type, out Type itemType)
        {
            var isCollection = false;
            itemType = null;

            if (!type.IsArray)
            {
                foreach (var interfaceType in type.GetInterfaces())
                {
                    if (interfaceType.IsGenericType() && interfaceType.GetGenericTypeDefinition() == typeof(ICollection<>))
                    {
                        itemType = interfaceType.GetGenericArguments()[0];
                        isCollection = true;
                        break;
                    }
                    else if (interfaceType == typeof(ICollection))
                    {
                        isCollection = true;
                        break;
                    }
                }
            }

            return isCollection;
        }

        public static bool IsDictionary(this Type type)
        {
            Type keyType;
            Type valueType;
            return IsDictionary(type, out keyType, out valueType);
        }

        public static bool IsDictionary(this Type type, out Type keyType)
        {
            Type valueType;
            return IsDictionary(type, out keyType, out valueType);
        }

        public static bool IsDictionary(this Type type, out Type keyType, out Type valueType)
        {
            var isDict = false;
            keyType = null;
            valueType = null;
            if (type.IsGenericType() && type.GetGenericTypeDefinition() == typeof(IDictionary<,>))
            {
                isDict = true;
            }
            else
            {
                foreach (var interfaceType in type.GetInterfaces())
                {
                    if (interfaceType.IsGenericType() && interfaceType.GetGenericTypeDefinition() == typeof(IDictionary<,>))
                    {
                        isDict = true;
                        keyType = interfaceType.GetGenericArguments()[0];
                        valueType = interfaceType.GetGenericArguments()[1];
                        break;
                    }
                }
            }

            return isDict;
        }

        public static bool Serializable(this Type type)
        {
            return true;
        }

        public static bool Deserializable(this Type type)
        {
            return true;
        }

		public static Func<object, string> CreateGetKeyDelegate(this Type kvpType)
		{
			var param = Expression.Parameter(typeof(object), "kvp");
			var castExp = Expression.Convert(param, kvpType);
			var getProp = kvpType.GetProperty("Key");
			var method = getProp.GetGetMethod();
			var getExp = Expression.Call(castExp, method);
			var retExp = Expression.Convert(getExp, typeof(string));
			var methodGetKey = (Func<object, string>)Expression.Lambda(retExp, param).Compile();
			return methodGetKey;
		}

		public static Func<object, object> CreateGetValueDelegate(this Type kvpType)
		{
			var param = Expression.Parameter(typeof(object), "kvp");
			var castExp = Expression.Convert(param, kvpType);
			var getProp = kvpType.GetProperty("Value");
			var method = getProp.GetGetMethod();
			var getExp = Expression.Call(castExp, method);
			var retExp = Expression.Convert(getExp, typeof(object));
			var methodGetValue = (Func<object, object>)Expression.Lambda(retExp, param).Compile();
			return methodGetValue;
		}

		public static Action<object, string, object> CreateDictAddDelegate(this Type dictType, Type valueType)
		{
			var dictParam = Expression.Parameter(typeof(object), "dict");
			var keyParam = Expression.Parameter(typeof(string), "key");
			var valueParam = Expression.Parameter(typeof(object), "value");
			var castDictExp = Expression.Convert(dictParam, dictType);
			var castValueExp = Expression.Convert(valueParam, valueType);

			var addMethod = dictType.GetMethod("Add");
			var addExp = Expression.Call(castDictExp, addMethod, keyParam, castValueExp);
			var addDelegate = (Action<object, string, object>)Expression.Lambda(addExp, dictParam, keyParam, valueParam).Compile();
			return addDelegate;
		}

		public static Func<object> CreateNewDelegate(this Type dictType)
		{
			var constructor = dictType.GetConstructor(Type.EmptyTypes);

			var newExp = Expression.New(constructor);
			return (Func<object>)Expression.Lambda(newExp).Compile();
		}

		/// <summary>
		/// Only recognize the format "MM/dd/yyyy HH:mm:ss.fff"
		/// </summary>
		/// <param name="str"></param>
		/// <param name="dt"></param>
		/// <returns></returns>
		public static bool TryFastParseDateInvariantCulture(this string str, out DateTime dt)
		{
			const string format = "MM/dd/yyyy HH:mm:ss.fff";
			if (str.Length > format.Length)
			{
				dt = DateTime.MinValue;
				return false;
			}
			int year = 0;
			int month = 0;
			int day = 0;
			int hour = 0;
			int min = 0;
			int second = 0;
			int ms = 0;
			for (var i = 0; i < str.Length; i++)
			{
				var f = format[i];
				switch (f)
				{
					case 'M':
						month = Multiply10(month) + DigitTable(str[i]);
						break;
					case 'd':
						day = Multiply10(day) + DigitTable(str[i]);
						break;
					case 'y':
						year = Multiply10(year) + DigitTable(str[i]);
						break;
					case 'H':
						hour = Multiply10(hour) + DigitTable(str[i]);
						break;
					case 'm':
						min = Multiply10(min) + DigitTable(str[i]);
						break;
					case 's':
						second = Multiply10(second) + DigitTable(str[i]);
						break;
					case 'f':
						ms = Multiply10(ms) + DigitTable(str[i]);
						break;
					case ':':
					case '/':
					case '.':
					case ' ':
					case '-':
						break;
					default:
						dt = DateTime.MinValue;
						return false;
				}
			}

			dt = new DateTime(year, month, day, hour, min, second, ms);
			return true;
		}

		private static int Multiply10(int i)
		{
			if (i == 0)
			{
				return 0;
			}
			return (i << 3) + (i << 1);
		}

		private static int DigitTable(char ch)
		{
			switch (ch)
			{
				case '0': return 0;
				case '1': return 1;
				case '2': return 2;
				case '3': return 3;
				case '4': return 4;
				case '5': return 5;
				case '6': return 6;
				case '7': return 7;
				case '8': return 8;
				case '9': return 9;
				default:
					throw new ArgumentException();
			}
		}
    }
}
