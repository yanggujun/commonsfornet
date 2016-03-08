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
using Commons.Collections.Map;

namespace Commons.Json.Mapper
{
    internal class TypeCache
    {
        private HashedMap<Type, TypeManager> typeManagers = new HashedMap<Type, TypeManager>();

        public T Instantiate<T>()
        {
            var type = typeof(T);
            if (!type.IsSupported())
            {
                throw new InvalidOperationException(Messages.TypeNotSupported);
            }
	        return (T) Instantiate(type);
        }

	    public object Instantiate(Type type)
	    {
            CacheTypeProperties(type);
            object value;
            Type itemType;
            if (!type.IsList(out itemType))
            {
                var manager = typeManagers[type];
                value = Initialize(manager);
            }
            else
            {
                value = Activator.CreateInstance(type);
            }

            return value;
	    }

        public TypeManager this[Type type]
        {
            get
            {
                return typeManagers[type];
            }
        }

	    private object Initialize(TypeManager manager)
	    {
		    var value = manager.Constructor.Invoke(null);
		    var properties = manager.Properties;
		    foreach (var prop in properties)
		    {
			    if (!prop.PropertyType.IsJsonPrimitive() && prop.PropertyType.IsSupported())
			    {
					prop.SetValue(value, Initialize(typeManagers[prop.PropertyType]));
			    }
		    }

		    return value;
	    }

        private void CacheTypeProperties(Type type)
        {
            TypeManager manager;
            Type itemType;
            Type cacheType;
            if (type.IsList(out itemType))
            {
                cacheType = itemType;
            }
            else
            {
                cacheType = type;
            }
            if (!cacheType.IsJsonPrimitive() && !typeManagers.ContainsKey(cacheType))
            {
                manager = new TypeManager(cacheType);
                typeManagers[cacheType] = manager;

				var properties = manager.Properties;
				foreach (var property in properties)
				{
					var propertyType = property.PropertyType;
					if (!propertyType.IsJsonPrimitive())
					{
						CacheTypeProperties(propertyType);
					}
				}
            }
        }
    }
}
