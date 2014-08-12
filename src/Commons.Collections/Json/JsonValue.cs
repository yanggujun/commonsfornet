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

using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace Commons.Collections.Json
{
    internal class JsonValue : DynamicObject
    {
        private readonly object jsonValue;

        public JsonValue(object value)
        {
            jsonValue = value == null ? null : Convert(value);
        }

        private static object Convert(object value)
        {
            var type = value.GetType();
            object obj = null;
            if (type.IsPrimitive || type == typeof(bool) || type == typeof(string) || type == typeof(JsonObject))
            {
                obj = value;
            }
            else if (type.IsArray)
            {
                var values = value as object[];
                var items = new JsonValue[values.Length];
                for (var i = 0; i < values.Length; i++)
                {
                    items[i] = new JsonValue(values[i]);
                }
                obj = items;
            }
            else if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                var items = value as IEnumerable;
				var list = new List<JsonValue>();
                foreach (var item in items)
                {
                    list.Add(new JsonValue(item));
                }
                obj = list.ToArray();
            }
            else
            {
                obj = value.ToString();
            }
            return obj;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var jobj = jsonValue as JsonObject;
            var success = false;
            if (null != jobj)
            {
                dynamic obj = jobj;
                result = obj[binder.Name];
                success = true;
            }
            else
            {
                success = base.TryGetMember(binder, out result);
            }
            return success;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            var jobj = jsonValue as JsonObject;
            var success = false;
            if (null != jobj)
            {
                dynamic obj = jobj;
                obj[binder.Name] = value;
                success = true;
            }
            else
            {
                success = base.TrySetMember(binder, value);
            }
            return success;
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            var hasValue = false;
            int index;
            if (jsonValue != null && jsonValue.GetType().IsArray && int.TryParse(indexes[0].ToString(), out index) && index >= 0)
            {
                result = ((JsonValue[])jsonValue)[index];
                hasValue = true;
            }
            else
            {
                hasValue = base.TryGetIndex(binder, indexes, out result);
            }

            return hasValue;
        }

        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
			var success = false;
			int index;
			if (jsonValue != null && jsonValue.GetType().IsArray && int.TryParse(indexes[0].ToString(), out index) && index >= 0)
			{
				((JsonValue[])jsonValue)[index] = new JsonValue(value);
				success = true;
			}
			else
			{
				success = base.TrySetIndex(binder, indexes, value);
			}
			return success;
        }

        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            result = jsonValue;
            return true;
        }

        public override string ToString()
        {
            return jsonValue == null ? "null" : jsonValue.FormatJsonValue();
        }
    }
}
