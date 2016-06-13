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

namespace Commons.Utils
{
    [CLSCompliant(true)]
    public static class Extensions
    {
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

    }
}
