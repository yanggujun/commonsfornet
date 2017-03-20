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
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace Commons.Reflect
{
	public class ReflectContext : IReflectContext
    {
		private readonly ConcurrentDictionary<Type, Func<object>> defaultConstructors = new ConcurrentDictionary<Type, Func<object>>();
		private readonly ConcurrentDictionary<Type, ConcurrentDictionary<string, Func<object, object>>> typeGetters = new ConcurrentDictionary<Type, ConcurrentDictionary<string, Func<object, object>>>();
	    public object NewInstance(Type type)
	    {
			try
			{
				Func<object> ctor;
				if (defaultConstructors.TryGetValue(type, out ctor))
				{
					return ctor();
				}
				else
				{
					var ctorMethod = type.GetConstructor(Type.EmptyTypes);
					if (ctorMethod == null)
					{
						throw new InvalidOperationException("No default constructor is found from the type.");
					}
					var ctorExp = Expression.New(ctorMethod);
					ctor = (Func<object>)Expression.Lambda(ctorExp).Compile();
					defaultConstructors[type] = ctor;
					return ctor();
				}
			}
			catch (Exception e)
			{
				throw new TypeLoadException("Cannot load type.", e);
			}
	    }

	    public object NewInstance(Type type, params object[] args)
	    {
			return null;
	    }

	    public object GetProperty(object target, string name)
	    {
			if (target == null)
			{
				return null;
			}
			var type = target.GetType();
			ConcurrentDictionary<string, Func<object, object>> getters;
			if (typeGetters.TryGetValue(type, out getters))
			{
				Func<object, object> getter;
				if (getters.TryGetValue(name, out getter))
				{
					return getter(target);
				}
				else
				{
					getter = CreateGetterDelegate(type, name);
					getters[name] = getter;
					return getter(target);
				}
			}
			else
			{
				getters = new ConcurrentDictionary<string, Func<object, object>>();
				var getter = CreateGetterDelegate(type, name);
				getters[name] = getter;
				typeGetters[type] = getters;
				return getter(target);
			}
	    }

		private Func<object, object> CreateGetterDelegate(Type type, string propName)
		{
			var prop = type.GetProperty(propName);
			if (prop != null)
			{
				var targetParamExp = Expression.Parameter(typeof(object), "target");
				var castExp = Expression.Convert(targetParamExp, type);
				var getMethod = prop.GetGetMethod();
				var getExp = Expression.Call(castExp, getMethod);
				var retExp = Expression.Convert(getExp, typeof(object));
				return (Func<object, object>)Expression.Lambda(retExp, targetParamExp).Compile();
			}

			return null;
		}

	    public void SetProperty(object target, string name, object value)
	    {
		    throw new NotImplementedException();
	    }

	    public object Invoke(object target, string name, params object[] args)
	    {
		    throw new NotImplementedException();
	    }
    }
}
