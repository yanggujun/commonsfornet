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

namespace Commons.Json.Mapper
{
    internal class DictionaryBuilder : ValueBuilderSkeleton
    {
        private readonly MapperContainer mappers;
        private readonly IObjectBuilder objBuilder;
        public DictionaryBuilder(ConfigContainer configuration, MapperContainer mappers, IObjectBuilder objBuilder) : base(configuration)
        {
            this.mappers = mappers;
            this.objBuilder = objBuilder;
        }

        protected override bool CanProcess(object raw, Type targetType, JValue jsonValue)
        {
            return targetType.IsDictionary();
        }

        protected override object DoBuild(object raw, Type targetType, JValue jsonValue)
        {
            Type keyType;
            Type valueType;
            targetType.IsDictionary(out keyType, out valueType);
            JObject jsonObj;

            if (jsonValue.Is<JObject>(out jsonObj))
            {
                throw new InvalidCastException(Messages.JsonValueTypeNotMatch);
            }

            if (keyType != typeof(string))
            {
                throw new InvalidCastException(Messages.JsonValueTypeNotMatch);
            }
            var method = targetType.GetMethod(Messages.AddMethod);
            foreach (var kvp in jsonObj)
            {
                var key = kvp.Key;
                object value = null;
                if (!valueType.IsJsonPrimitive())
                {
                    value = objBuilder.Build(valueType);
                }
                value = Successor.Build(value, valueType, kvp.Value);
                method.Invoke(raw, new[] { key, value });
            }

            return raw;
        }
    }
}
