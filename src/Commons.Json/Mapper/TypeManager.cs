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
using System.Linq;

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

using Commons.Utils;
using System.Linq.Expressions;

namespace Commons.Json.Mapper
{
    internal struct Triple<T1, T2, T3>
    {
        public Triple(T1 v1, T2 v2, T3 v3)
        {
            Value1 = v1;
            Value2 = v2;
            Value3 = v3;
        }

        public T1 Value1 { get; set; }
        public T2 Value2 { get; set; }
        public T3 Value3 { get; set; }
    }
    internal class TypeManager
    {

        public TypeManager(Type type)
        {
            Initialize(type);
        }

        public Type Type
        {
            get;
            set;
        }

        public ConstructorInfo Constructor
        {
            get;
            set;
        }

        public IList<Triple<string, Type, Func<object, object>>> Getters
        {
            get;
        } = new List<Triple<string, Type, Func<object, object>>>();

        public IList<Triple<string, Type, Action<object, object>>> Setters
        {
            get;
        } = new List<Triple<string, Type, Action<object, object>>>();

        public IList<PropertyInfo> Properties
        {
            get; private set;
        } 

        private void Initialize(Type type)
        {
            Type underlying;
            if (type.IsNullable(out underlying) && !type.IsNullablePrimitive())
            {
                Type = underlying;
            }
            else
            {
                Type = type;
            }

            if (Type.IsPrimitive())
            {
                throw new InvalidOperationException("The primitive type cannot be cached.");
            }

            Properties = Type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(x => x.GetIndexParameters().Length == 0).ToList();

            var getters = Type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.CanRead && x.GetIndexParameters().Length == 0 && x.GetGetMethod(false) != null);
            foreach(var get in getters)
            {
                var targetParamExp= Expression.Parameter(typeof(object), "target");
                var castExp = Expression.Convert(targetParamExp, Type);
                var getMethod = get.GetGetMethod();
                var getExp = Expression.Call(castExp, getMethod);
                var retExp = Expression.Convert(getExp, typeof(object));
                var getter = (Func<object, object>) Expression.Lambda(retExp, targetParamExp).Compile();
                Getters.Add(new Triple<string, Type, Func<object, object>>(get.Name, get.PropertyType, getter));
            }

            var setters = Type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.CanWrite && x.GetIndexParameters().Length == 0 && x.GetSetMethod(false) != null);

            foreach(var set in setters)
            {
                var targetParamExp = Expression.Parameter(typeof(object), "target");
                var valueParamExp = Expression.Parameter(typeof(object), "val");
                var setMethod = set.GetSetMethod();
                var castTargetExp = Expression.Convert(targetParamExp, Type);
                var testValueExp = Expression.NotEqual(valueParamExp, Expression.Constant(null));
                var castValueExp = Expression.Convert(valueParamExp, set.PropertyType);

                var setValueExp = Expression.Call(castTargetExp, setMethod, castValueExp);
                var testExp = Expression.IfThen(testValueExp, setValueExp);
                var setter = (Action<object, object>)Expression.Lambda(testExp, targetParamExp, valueParamExp).Compile();
                Setters.Add(new Triple<string, Type, Action<object, object>>(set.Name, set.PropertyType, setter));
            }

            Constructor = type.GetConstructor(Type.EmptyTypes);
        }
    }
}
