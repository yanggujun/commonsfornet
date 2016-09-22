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
    internal class Launcher : ILauncher
    {
        private readonly MapperContainer mappers;
        private readonly TypeContainer types;

        public Launcher(MapperContainer mappers, TypeContainer types)
        {
            this.mappers = mappers;
            this.types = types;
        }

        public object Launch(Type type)
        {
            var mapper = mappers.GetMapper(type);
            var create = mapper.Create;
            object value = null;
            if (create != null)
            {
                value = create();
            }
            else if (!type.IsInterface() && !type.IsAbstract())
            {
                value = Instantiate(type);
            }

            return value;
        }

        private object Instantiate(Type type)
        {
            var value = CreateInstance(type);
            if (value != null && !type.IsCollection())
            {
                var manager = types[type];
                var properties = manager.Setters;
                foreach (var prop in properties)
                {
                    var propType = prop.Value2;
                    var propValue = Launch(propType);
                    if (propValue != null)
                    {
                        prop.Value3(value, propValue);
                    }
                }
            }

            return value;
        }

        private object CreateInstance(Type type)
        {
            if (type.IsJsonPrimitive() || type.IsInterface() 
                || type.IsAbstract() || type.IsArray)
            {
                return null;
            }
            object instance;
            Type underlying;
            if (type.IsNullable(out underlying) && !type.IsJsonPrimitive())
            {
                instance = Activator.CreateInstance(underlying);
            }
            else if (type.IsCollection())
            {
                instance = Activator.CreateInstance(type);
            }
            else
            {
                var manager = types[type];
                if (type.IsClass() && manager.Constructor == null)
                {
                    throw new InvalidOperationException(Messages.NoDefaultConstructor);
                }
                instance = Activator.CreateInstance(type);
            }

            return instance;
        }
    }
}
