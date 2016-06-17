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

namespace Commons.Json.Mapper
{
    internal class EnumerableBuilder : ArrayBuilder
    {
        private readonly CollectionBuilder colBuilder;

        public EnumerableBuilder(ConfigContainer configuration, IObjectBuilder objBuilder, CollectionBuilder colBuilder) 
            : base(configuration, objBuilder)
        {
            this.colBuilder = colBuilder;
        }

        protected override bool CanProcess(object raw, Type targetType, JValue jsonValue)
        {
            return targetType.IsCollection() && !targetType.IsDictionary();
        }

        protected override object DoBuild(object raw, Type targetType, JValue jsonValue)
        {
            if (raw != null)
            {
                var itemType = targetType.GetGenericArguments()[0];
                var array = (Array)BuildArray(raw, itemType, jsonValue);
                foreach (var item in array)
                {
                    colBuilder.Build((IEnumerable)raw, item);
                }
            }

            return raw;
        }
    }
}
