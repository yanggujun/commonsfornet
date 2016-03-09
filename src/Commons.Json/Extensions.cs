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
			       || ch == JsonTokens.LineSeparator;
		}

		public static bool IsJsonPrimitive(this Type type)
		{
			return type.IsPrimitive || type == typeof (string) || type == typeof(DateTime);
		}

		public static bool IsJsonNumber(this Type type)
		{
			return type == typeof (int) || type == typeof (long)
				   || type == typeof(short)
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

		public static bool IsList(this Type type)
		{
			Type itemType;
			return type.IsList(out itemType);
		}

		public static bool IsList(this Type type, out Type itemType)
		{
			var isList = false;
			itemType = null;

			foreach (var interfaceType in type.GetInterfaces())
			{
				if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof (IList<>))
				{
					itemType = type.GetGenericArguments()[0];
					isList = true;
					break;
				}
			}

			return isList;
		}

        public static bool IsDictionary(this Type type)
        {
            var isDict = false;
            foreach (var interfaceType in type.GetInterfaces())
            {
                if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IDictionary<,>))
                {
                    isDict = true;
                    break;
                }
            }

            return isDict;
        }

        public static bool IsSupported(this Type type)
        {
            var isSupported = false;
            if (type.IsList())
            {
                isSupported = true;
            }
            else if (!type.InstanceOf(typeof(IEnumerable)) && !type.IsArray)
            {
                isSupported = true;
            }

            return isSupported;
        }

        public static bool InstanceOf(this Type subType, Type baseType)
        {
            return subType.IsSubclassOf(baseType) || subType == baseType;
        }
	}
}
