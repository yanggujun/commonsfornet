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
using System.Collections.Generic;
using System.Dynamic;

namespace Commons.Json
{
    [CLSCompliant(true)]
    public class JsonObject : JsonValue
    {
        private readonly Dictionary<string, JsonValue> valueMap = new Dictionary<string, JsonValue>();

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            var name = binder.Name;
            if (valueMap.ContainsKey(name))
            {
                valueMap[name] = JsonValue.From(value);
            }
            else
            {
                valueMap.Add(name, JsonValue.From(value));
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
                valueMap.Add(name, JsonValue.From(obj));
                result = obj;
            }
            return true;
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            var name = indexes[0] as string;
            var success = false;
            if (!string.IsNullOrWhiteSpace(name))
            {
                JsonValue jvalue;
                success = valueMap.TryGetValue(name, out jvalue);
                if (!success)
                {
                    var obj = new JsonObject();
                    valueMap.Add(name, JsonValue.From(obj));
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
                throw new InvalidOperationException(Messages.JsonObjectCannotIndex);
            }
            return success;
        }

        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            var name = indexes[0] as string;
            var success = false;
            if (!string.IsNullOrWhiteSpace(name))
            {
                if (valueMap.ContainsKey(name))
                {
                    valueMap[name] = JsonValue.From(value);
                }
                else
                {
                    valueMap.Add(name, JsonValue.From(value));
                }
                success = true;
            }
            else
            {
                throw new InvalidOperationException(Messages.JsonObjectCannotIndex);
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
            else if (targetType == typeof(IDictionary<string, JsonValue>))
            {
                var dict = new Dictionary<string, JsonValue>();
                foreach (var item in valueMap)
                {
                    dict.Add(item.Key, item.Value);
                }
                result = dict;
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
            return JsonUtils.FormatJsonObject(valueMap);
        }

        public bool HasValue(string key)
        {
            return valueMap.ContainsKey(key);
        }

        public JsonValue this[string key]
        {
            get { return valueMap[key]; }
            set { valueMap[key] = value; }
        }
    }
}
