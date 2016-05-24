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
using Commons.Collections.Map;

namespace Commons.Json.Mapper
{
    internal class TypeCache
    {
        private HashedMap<Type, TypeManager> typeManagers = new HashedMap<Type, TypeManager>();

        public T Instantiate<T>(MapperContainer mappers)
        {
            var type = typeof(T);
            return (T) Instantiate(type, mappers);
        }

        public object Instantiate(Type type, MapperContainer mappers)
        {
            if (!type.Deserializable())
            {
                throw new InvalidOperationException(Messages.TypeNotSupported);
            }
            CacheTypeProperties(type);
            object value;
            if (!type.IsEnumerable() && !type.IsDictionary())
            {
                var manager = typeManagers[type];
                value = Initialize(manager, mappers);
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
                if (type.IsJsonPrimitive() || !type.Deserializable())
                {
                    throw new InvalidOperationException(Messages.TypeNotSupported);
                }
                TypeManager manager;
                if (typeManagers.ContainsKey(type))
                {
                    manager = typeManagers[type];
                }
                else
                {
                    CacheTypeProperties(type);
                    manager = typeManagers[type];
                }
                return manager;
            }
        }

        private object Initialize(TypeManager manager, MapperContainer mappers)
        {
            if (!CanBeInstantiated(manager, mappers))
            {
                throw new InvalidOperationException(Messages.NoDefaultConstructor);
            }
            var value = CreateInstance(manager.Type, mappers);
            var properties = manager.Setters;
            foreach (var prop in properties)
            {
                var propType = prop.PropertyType;
                if (!propType.IsJsonPrimitive() && propType.Deserializable() && !propType.IsArray)
                {
                    if (propType.IsEnumerable())
                    {
                        prop.SetValue(value, Activator.CreateInstance(propType), null);
                    }
                    else
                    {
                        prop.SetValue(value, Initialize(typeManagers[propType], mappers), null);
                    }
                }
            }

            return value;
        }

        private void CacheTypeProperties(Type type)
        {
            TypeManager manager;
            Type itemType;
            Type cacheType;
            if (type.IsEnumerable(out itemType))
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

                var properties = new List<PropertyInfo>();
                properties.AddRange(manager.Setters);
                properties.AddRange(manager.Getters);
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

        private object CreateInstance(Type type, MapperContainer mappers)
        {
            if (mappers.ContainsMapper(type))
            {
                var mapper = mappers.GetMapper(type);
                var create = mapper.Create;
                if (create != null)
                {
                    return create();
                }
            }
            Type underlying;
            if (type.IsNullable(out underlying) && !type.IsJsonPrimitive())
            {
                return Activator.CreateInstance(underlying);
            }
            else
            {
                if (type.GetTypeInfo().IsInterface)
                {
                    throw new InvalidOperationException(Messages.TypeNotSupported);
                }
                return Activator.CreateInstance(type);
            }
        }

        private bool CanBeInstantiated(TypeManager manager, MapperContainer mappers)
        {
            var type = manager.Type;
            var init = false;
            if (type.GetTypeInfo().IsClass)
            {
                if (manager.Constructor != null)
                {
                    init = true;
                }
                else if (mappers.ContainsMapper(type))
                {
                    var mapper = mappers.GetMapper(type);
                    var create = mapper.Create;
                    init = create != null;
                }
            }
            else
            {
                init = true;
            }

            return init;
        }
    }
}
