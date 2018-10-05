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
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Commons.Utils;

namespace Commons.Json.Mapper
{
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

        public List<Tuple<PropertyInfo, Func<object, object>>> Getters
        {
            get;
        } = new List<Tuple<PropertyInfo, Func<object, object>>>();

        public List<Tuple<PropertyInfo, Action<object, object>>> Setters
        {
            get;
        } = new List<Tuple<PropertyInfo, Action<object, object>>>();

        public List<PropertyInfo> Properties
        {
            get; private set;
        } 

        public object Launch()
        {
            if (Launcher == null)
            {
                throw new InvalidOperationException(Messages.NoDefaultConstructor);
            }
            return Launcher();
        }

        private Func<object> Launcher { get; set; }

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
                Getters.Add(new Tuple<PropertyInfo, Func<object, object>>(get, getter));
            }

            var setters = Type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.CanWrite && x.GetIndexParameters().Length == 0 && x.GetSetMethod(false) != null);

            if (!Type.IsValueType())
            {
                foreach (var set in setters)
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
                    Setters.Add(new Tuple<PropertyInfo, Action<object, object>>(set, setter));
                }
                Constructor = Type.GetConstructor(Type.EmptyTypes);

                if (Constructor != null)
                {
                    var newExp = Expression.New(Constructor);
                    Launcher = (Func<object>)Expression.Lambda(newExp).Compile();
                }
            }
            else
            {
                foreach (var set in setters)
                {
                    Action<object, object> setter = (target, val) => set.SetValue(target, val, null);
                    Setters.Add(new Tuple<PropertyInfo, Action<object, object>>(set, setter));
                }
                var newExp = Expression.New(Type);
                var convertExp = Expression.Convert(newExp, typeof(object));
                Launcher = (Func<object>)Expression.Lambda(convertExp).Compile();
            }
            // TODO: abstract? inherit?
        }
    }
}
