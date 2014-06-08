// Copyright CommonsForNET 2014.
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

using Commons.Collections.Map;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace Commons.Collections.Json
{
    public class JsonObject : DynamicObject
    {
        private readonly HashMap<string, JsonValue> valueMap = new HashMap<string, JsonValue>();

        public JsonObject()
        {
        }

        public JsonObject(string json)
        {
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            var name = binder.Name;
            if (valueMap.ContainsKey(name))
            {
                valueMap[name] = new JsonValue(value);
            }
            else
            {
                valueMap.Add(name, new JsonValue(value));
            }

            return true;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var name = binder.Name;
            JsonValue jvalue;
            var success = valueMap.TryGetValue(name, out jvalue);
            result = jvalue;
            return success;
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            var name = indexes[0] as string;
            var success = false;
            if (!string.IsNullOrEmpty(name))
            {
                JsonValue jvalue;
                success = valueMap.TryGetValue(name, out jvalue);
                result = jvalue;
            }
            else
            {
                result = null;
            }
            return success;
        }

        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            var name = indexes[0] as string;
            var success = false;
            if (!string.IsNullOrEmpty(name))
            {
                if (valueMap.ContainsKey(name))
                {
                    valueMap[name] = new JsonValue(value);
                }
                else
                {
                    valueMap.Add(name, new JsonValue(value));
                }
                success = true;
            }

            return success;
        }

        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            var targetType = binder.Type;
            var success = false;
            if (targetType == typeof(string))
            {
                result = ToString();
                success = true;
            }
            else if (targetType == typeof(IDictionary<string, string>))
            {
                result = new HashMap<string, string>();
                //TODO:
                success = true;
            }
            else
            {
                result = null;
            }

            return success;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append("{\n");
            var count = 0;
            var total = valueMap.Count;
            foreach (var item in valueMap)
            {
                builder.Append("\t").Append("\"").Append(item.Key).Append("\"").Append(": ").Append(item.Value.ToString());
                count++;
                if (count < total)
                {
                    builder.Append(",\n");
                }
            }
            builder.Append("\n}");
            return builder.ToString();
        }
    }
}
