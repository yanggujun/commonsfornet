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
using System.Text;

namespace Commons.Utils
{
    [CLSCompliant(true)]
    public static class Extensions
    {
        private const char Zero = '0';
        private const char TimeSlash = '/';
        private const char TimeSpace = ' ';
        private const char TimeColon = ':';
        private const char TimeDot = '.';

        public static byte[] ToBytes(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                throw new ArgumentException("The input string is null or empty.");
            }
            var bytes = new byte[str.Length * sizeof(char)];
            Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        public static void Swallow<T>(this T target, Action<T> action)
        {
            Guarder.CheckNull(target, "target");
            Guarder.CheckNull(action, "action");
            try
            {
                action(target);
            }
            catch
            {
                //quiet
            }
        }

        public static IEnumerable<Type> SuperTypes(this Type type, bool includeSelf = true)
        {
            if (includeSelf)
            {
                yield return type;
            }

            var tmp = type;
            while (tmp.GetBaseType() != null)
            {
                tmp = tmp.GetBaseType();
                yield return tmp;
            }

            foreach (var i in type.GetInterfaces())
            {
                yield return i;
            }
        }

        public static Type GetBaseType(this Type type)
        {
#if NET40
            return type.BaseType;
#else
            return type.GetTypeInfo().BaseType;
#endif
        }

        public static bool IsPrimitive(this Type type)
        {
#if NET40
            return type.IsPrimitive;
#else
            return type.GetTypeInfo().IsPrimitive;
#endif
        }

        public static bool IsValueType(this Type type)
        {
#if NET40
            return type.IsValueType;
#else
            return type.GetTypeInfo().IsValueType;
#endif
        }

        public static bool IsEnum(this Type type)
        {
#if NET40
            return type.IsEnum;
#else
            return type.GetTypeInfo().IsEnum;
#endif
        }

        public static bool IsGenericType(this Type type)
        {
#if NET40
            return type.IsGenericType;
#else
            return type.GetTypeInfo().IsGenericType;
#endif
        }

        public static bool IsInterface(this Type type)
        {
#if NET40
            return type.IsInterface;
#else
            return type.GetTypeInfo().IsInterface;
#endif
        }

        public static bool IsAbstract(this Type type)
        {
#if NET40
            return type.IsAbstract;
#else
            return type.GetTypeInfo().IsAbstract;
#endif
        }

        public static bool IsClass(this Type type)
        {
#if NET40
            return type.IsClass;
#else
            return type.GetTypeInfo().IsClass;
#endif
        }

        public static bool IsSubTypeOf(this Type type, Type anotherType)
        {
#if NET40
            return type.IsSubclassOf(anotherType);
#else
            return type.GetTypeInfo().IsSubclassOf(anotherType);
#endif
        }

        /// <summary>
        /// Converts the datetime to string format "MM/dd/yyyy HH:mm:ss.fff"
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string FastToStringInvariantCulture(this DateTime dt)
        {
            var timeArray = new char[23];
            timeArray[0] = (char)(dt.Month / 10 + Zero);
            timeArray[1] = (char)(dt.Month % 10 + Zero);
            timeArray[2] = TimeSlash;
            timeArray[3] = (char)(dt.Day / 10 + Zero);
            timeArray[4] = (char)(dt.Day % 10 + Zero);
            timeArray[5] = TimeSlash;
            timeArray[6] = (char)(dt.Year / 1000 + Zero);
            timeArray[7] = (char)((dt.Year / 100) % 10 + Zero);
            timeArray[8] = (char)((dt.Year / 10) % 10 + Zero);
            timeArray[9] = (char)((dt.Year % 10) + Zero);
            timeArray[10] = TimeSpace;

            var ticks = dt.Ticks;
            int hour = (int)((ticks / 0x861c46800L)) % 24;
            int minute = (int)((ticks / 0x23c34600L)) % 60;
            int second = (int)(((ticks / 0x989680L)) % 60L);
            int ms = (int)(((ticks / 0x2710L)) % 0x3e8L);
            timeArray[11] = (char)(hour / 10 + Zero);
            timeArray[12] = (char)(hour % 10 + Zero);
            timeArray[13] = TimeColon;
            timeArray[14] = (char)(minute / 10 + Zero);
            timeArray[15] = (char)(minute % 10 + Zero);
            timeArray[16] = TimeColon;
            timeArray[17] = (char)(second / 10 + Zero);
            timeArray[18] = (char)(second % 10 + Zero);
            timeArray[19] = TimeDot;
            timeArray[20] = (char)(ms / 100 + Zero);
            timeArray[21] = (char)(ms / 10 % 10 + Zero);
            timeArray[22] = (char)(ms % 10 + Zero);
            return new string(timeArray);
        }

        /// <summary>
        /// A faster alternative for Convert.ToBase64String
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string Base64Encode(this byte[] bytes)
        {
            return ChromiumBase64Codec.Encode(bytes);
        }

        /// <summary>
        /// A faster alternative for Convert.FromBase64String
        /// </summary>
        /// <param name="origin"></param>
        /// <returns></returns>
        public static byte[] Base64Decode(this string origin)
        {
            return ChromiumBase64Codec.Decode(origin);
        }
    }
}
