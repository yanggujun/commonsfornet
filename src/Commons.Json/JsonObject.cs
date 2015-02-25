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
using System.Dynamic;
using Commons.Collections.Map;

namespace Commons.Json
{
    [CLSCompliant(true)]
    public class JsonObject : DynamicObject
    {
        private readonly LinkedHashedMap<string, JsonValue> valueMap = new LinkedHashedMap<string, JsonValue>();

        public static JsonObject Parse(string json)
        {
            return json.ToJson();
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
            if (!success)
            {
                var obj = new JsonObject();
                valueMap.Add(name, new JsonValue(obj));
                result = obj;
            }
            return true;
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            var name = indexes[0] as string;
            var success = false;
            if (!string.IsNullOrEmpty(name))
            {
                JsonValue jvalue;
                success = valueMap.TryGetValue(name, out jvalue);
                if (!success)
                {
                    var obj = new JsonObject();
                    valueMap.Add(name, new JsonValue(obj));
                    result = obj;
                    success = true;
                }
                else
                {
                    result = jvalue;
                }
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
            else
            {
                result = null;
            }

            return success;
        }

        public override string ToString()
        {
            return JsonParser.FormatJsonObject(valueMap);
        }
    }
}
