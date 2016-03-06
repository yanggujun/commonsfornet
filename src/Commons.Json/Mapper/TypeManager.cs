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
        private List<PropertyInfo> properties = new List<PropertyInfo>();

        public TypeManager(Type type)
        {
            Type = type;

            properties = Type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.CanRead && x.CanWrite)
                .Where(y => y.GetGetMethod(false) != null && y.GetSetMethod(false) != null)
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

        public IReadOnlyList<PropertyInfo> Properties
        {
            get
            {
                return properties;
            }
        }
    }
}
