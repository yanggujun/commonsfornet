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

namespace Commons.Json.Mapper
{
    internal class TypeManager
    {
        private readonly List<PropertyInfo> getters = new List<PropertyInfo>();
        private readonly List<PropertyInfo> setters = new List<PropertyInfo>();

        public TypeManager(Type type)
        {
            Type = type;

            getters = Type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.CanRead)
                .Where(y => y.GetGetMethod(false) != null)
                .Where(z => z.PropertyType.Serializable())
                .ToList();

            setters = Type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.CanWrite)
                .Where(y => y.GetSetMethod(false) != null)
                .Where(z => z.PropertyType.Deserializable())
                .ToList();

            Constructor = type.GetConstructor(Type.EmptyTypes);
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
#if NET45
        public IReadOnlyList<PropertyInfo> Getters
#else
        public IList<PropertyInfo> Getters
#endif
        {
            get
            {
                return getters;
            }
        }

#if NET45
        public IReadOnlyList<PropertyInfo> Setters
#else
        public IList<PropertyInfo> Setters
#endif
        {
            get
            {
                return setters;
            }
        }
    }
}
