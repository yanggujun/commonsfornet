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
#if NETSTANDARD1_3
using System.Reflection;
#endif

using Commons.Utils;

namespace Commons.Json.Mapper
{
    internal class ObjectBuilder : IObjectBuilder
    {
        private readonly MapperContainer mappers;
        private readonly TypeContainer types;

        public ObjectBuilder(MapperContainer mappers, TypeContainer types)
        {
            this.mappers = mappers;
            this.types = types;
        }

        public object Build(Type type)
        {
            object value = null;
            if (!type.IsCollection() && !type.IsDictionary())
            {
                var manager = types[type];
                value = Instantiate(manager);
            }
            else if (!type.IsInterface() && !type.IsAbstract())
            {
                value = Activator.CreateInstance(type);
            }
            else
            {
                throw new InvalidOperationException(Messages.TypeNotSupported);
            }

            return value;
        }

        private object Instantiate(TypeManager manager)
        {
            var value = CreateInstance(manager);
            var properties = manager.Setters;
            foreach (var prop in properties)
            {
                var propType = prop.PropertyType;
                if (propType.IsCollection() && !propType.IsInterface() && !propType.IsAbstract())
                {
                    prop.SetValue(value, Activator.CreateInstance(propType), null);
                }
                else if (!propType.IsInterface() && !propType.IsAbstract() && !propType.IsArray && !propType.IsJsonPrimitive()) 
                {
                    prop.SetValue(value, Instantiate(types[propType]), null);
                }
            }

            return value;
        }

        private object CreateInstance(TypeManager manager)
        {
            var type = manager.Type;
            var mapper = mappers.GetMapper(type);
            var create = mapper.Create;
            if (create != null)
            {
                return create();
            }

            Type underlying;
            if (type.IsNullable(out underlying) && !type.IsJsonPrimitive())
            {
                return Activator.CreateInstance(underlying);
            }
            else
            {
                if (type.IsInterface() || type.IsAbstract())
                {
                    throw new InvalidOperationException(Messages.TypeNotSupported);
                }
                if (type.IsClass() && manager.Constructor == null)
                {
                    throw new InvalidOperationException(Messages.NoDefaultConstructor);
                }
                return Activator.CreateInstance(type);
            }
        }
    }
}
