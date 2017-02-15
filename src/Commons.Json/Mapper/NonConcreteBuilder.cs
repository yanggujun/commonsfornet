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
using System.Collections;
#if NETSTANDARD1_3
using System.Reflection;
#endif

using Commons.Utils;

namespace Commons.Json.Mapper
{
	internal class NonConcreteBuilder : ValueBuilderSkeleton
    {
        private readonly MapperContainer mappers;
        public NonConcreteBuilder(ConfigContainer configuration, MapperContainer mappers) : base(configuration)
        {
            this.mappers = mappers;
        }

        protected override bool CanProcess(Type targetType, JValue jsonValue)
        {
            return (targetType.IsInterface() || targetType.IsAbstract()) && typeof(IEnumerable).IsAssignableFrom(targetType);
        }

        protected override object DoBuild(Type targetType, JValue jsonValue)
        {
            var mapper = mappers.GetMapper(targetType);
            object target = null;
            if (mapper.Create != null)
            {
                target = mapper.Create();
            }
            else if (mapper.ManualCreate != null)
            {
                target = mapper.ManualCreate(jsonValue);
            }

            return target;
        }
    }
}
