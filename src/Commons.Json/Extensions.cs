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
using System.Collections.Generic;
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
                   || ch == JsonTokens.LineSeparator;
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
    }
}
