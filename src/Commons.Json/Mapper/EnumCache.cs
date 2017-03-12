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
using System.Collections.Concurrent;

namespace Commons.Json.Mapper
{
    internal class EnumCache
    {
        // EnumType -> (EnumName -> Enum)
        private readonly ConcurrentDictionary<Type, ConcurrentDictionary<string, object>> cache = new ConcurrentDictionary<Type, ConcurrentDictionary<string, object>>();

        public object GetValue(Type enumType, string enumValue)
        {
            if (cache.ContainsKey(enumType))
            {
                var valueDict = cache[enumType];
                if (valueDict.ContainsKey(enumValue))
                {
                    return valueDict[enumValue];
                }
                return null;
            }
            else
            {
                var valueDict = new ConcurrentDictionary<string, object>();
                Type actualType;
                if (!enumType.IsNullable(out actualType))
                {
                    actualType = enumType;
                }
                var strs = Enum.GetNames(actualType);
                for (var i = 0; i < strs.Length; i++)
                {
                    var value = Enum.Parse(actualType, strs[i]);
                    valueDict[strs[i]] = value;
                }
                cache[enumType] = valueDict;
                return valueDict[enumValue];
            }
        }
    }
}
