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
using System.Linq.Expressions;
using System.Reflection;

namespace Commons.Reflect
{
    internal class Invoker : IInvoker
    {
        private readonly Func<object> defaultCtor;
        private readonly Dictionary<string, Func<object, object>> getters = new Dictionary<string, Func<object, object>>();
        private readonly Dictionary<string, Action<object, object>> setters = new Dictionary<string, Action<object, object>>();

        public Invoker(Type type)
        {
            var ctor = type.GetConstructor(Type.EmptyTypes);
            if (ctor != null)
            {
                var newExp = Expression.New(ctor);
                defaultCtor = (Func<object>)Expression.Lambda(newExp).Compile();
            }

            var getMethods = type.GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(x => x.CanRead && x.GetIndexParameters().Length == 0 && x.GetGetMethod(false) != null);
            foreach (var get in getMethods)
            {
                var targetParamExp = Expression.Parameter(typeof(object), "target");
                var castExp = Expression.Convert(targetParamExp, type);
                var getMethod = get.GetGetMethod();
                var getExp = Expression.Call(castExp, getMethod);
                var retExp = Expression.Convert(getExp, typeof(object));
                var getter = (Func<object, object>)Expression.Lambda(retExp, targetParamExp).Compile();
                getters[get.Name] = getter;
            }

            var setMethods = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.CanWrite && x.GetIndexParameters().Length == 0 && x.GetSetMethod(false) != null);
            foreach (var set in setMethods)
            {
                var targetParamExp = Expression.Parameter(typeof(object), "target");
                var valueParamExp = Expression.Parameter(typeof(object), "val");
                var setMethod = set.GetSetMethod();
                var castTargetExp = Expression.Convert(targetParamExp, type);
                var testValueExp = Expression.NotEqual(valueParamExp, Expression.Constant(null));
                var castValueExp = Expression.Convert(valueParamExp, set.PropertyType);

                var setValueExp = Expression.Call(castTargetExp, setMethod, castValueExp);
                var testExp = Expression.IfThen(testValueExp, setValueExp);
                var setter = (Action<object, object>)Expression.Lambda(testExp, targetParamExp, valueParamExp).Compile();
                setters[set.Name] = setter;
            }

			Getters = getters.Select(x => new Tuple<string, Func<object, object>>(x.Key, x.Value)).ToArray();
			Setters = setters.Select(x => new Tuple<string, Action<object, object>>(x.Key, x.Value)).ToArray();
        }

        public object GetProperty(object target, string name)
        {
            Func<object, object> getter;
            if (!getters.TryGetValue(name, out getter))
            {
                throw new InvalidOperationException("The property does not exist.");
            }
            return getter(target);
        }

        public object NewInstance()
        {
            if (defaultCtor == null)
            {
                throw new InvalidOperationException("The type does not contain a default constructor.");
            }
            return defaultCtor();
        }

        public void SetProperty(object target, string name, object value)
        {
            Action<object, object> setter;
            if (!setters.TryGetValue(name, out setter))
            {
                throw new InvalidOperationException("The property does not exist.");
            }
            setter(target, value);
        }

	    public Func<object, object> GetGetter(string name)
	    {
			Func<object, object> getter;
			if (!getters.TryGetValue(name, out getter))
			{
				getter = null;
			}
			return getter;
	    }

	    public Action<object, object> GetSetter(string name)
	    {
			Action<object, object> setter;
			if (!setters.TryGetValue(name, out setter))
			{
				setter = null;
			}
			return setter;
	    }

		public Tuple<string, Func<object, object>>[] Getters
		{
			get; private set;
		}

		public Tuple<string, Action<object, object>>[] Setters
		{
			get; private set;
		}
    }
}
