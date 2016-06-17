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
using Commons.Utils;

namespace Commons.Json.Mapper
{
    internal class TypeContainer
    {
        private HashedMap<Type, TypeManager> typeManagers = new HashedMap<Type, TypeManager>();

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
                    Cache(type);
                    manager = typeManagers[type];
                }
                return manager;
            }
        }

        private void Cache(Type type)
        {
            TypeManager manager;
            Type itemType;
            Type cacheType;
            if (type.IsCollection(out itemType))
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
                        Cache(propertyType);
                    }
                }
            }
        }
    }
}
