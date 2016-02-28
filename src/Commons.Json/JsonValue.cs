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

using System.Collections;
using System.Collections.Generic;
using System.Dynamic;

namespace Commons.Json
{
    public abstract class JsonValue : DynamicObject
    {
        public static JsonValue From(object value)
        {
            var type = value.GetType();
            object obj = null;

            if (type.IsPrimitive || type == typeof(bool) || type == typeof(string))
            {
                return new JsonPrimitive(value);
            }
            else if (type.IsArray)
            {
                var values = value as object[];
                var items = new JsonValue[values.Length];
                for (var i = 0; i < values.Length; i++)
                {
                    items[i] = From(values[i]);
                }
                return new JsonArray(items);
            }
            else if (typeof(IDictionary).IsAssignableFrom(type))
            {
                //TODO:
                return new JsonObject();
            }
            else if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                var items = value as IEnumerable;
                var list = new List<JsonValue>();
                foreach (var item in items)
                {
                    list.Add(From(item));
                }
                return new JsonArray(list.ToArray());
            }
            else
            {
                return new JsonPrimitive(value);
            }
            return null;
        }
    }
}
