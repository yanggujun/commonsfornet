﻿// Copyright CommonsForNET 2014.
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
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

using Commons.Collections.Common;
using System.Text;

namespace Commons.Collections.Json
{
    public class JsonValue : DynamicObject
    {
        private object jvalue;

        public JsonValue(object value)
        {
            jvalue = value == null ? null : Convert(value);
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
                var items = new object[values.Length];
                for (var i = 0; i < values.Length; i++)
                {
                    items[i] = new JsonValue(values[i]);
                }
                obj = items;
            }
            else if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                var items = value as IEnumerable;
                var list = new ArrayList();
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
            var jobj = jvalue as JsonObject;
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
            var jobj = jvalue as JsonObject;
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
            return base.TryGetIndex(binder, indexes, out result);
        }

        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            return base.TrySetIndex(binder, indexes, value);
        }

        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            result = jvalue;
            return true;
        }

        public override string ToString()
        {
            if (jvalue == null)
            {
                return "null";
            }
            var type = jvalue.GetType();
            var str = string.Empty;
            if (type.IsPrimitive || type == typeof(bool))
            {
                str = jvalue.ToString();
            }
            else if (type == typeof(string))
            {
                var builder = new StringBuilder();
                builder.Append("\"").Append(jvalue).Append("\"");
                str = builder.ToString();
            }
            else if (type == typeof(JsonObject))
            {
                str = jvalue.ToString();
            }
            else if (type.IsArray)
            {
                var items = jvalue as object[];
                var builder = new StringBuilder();
                var count = 0;
                var total = items.Length;
                builder.Append("[");
                foreach (var item in items)
                {
                    builder.Append(item.ToString());
                    count++;
                    if (count < total)
                    {
                        builder.Append(",");
                    }
                }
                builder.Append("]");
                str = builder.ToString();
            }
            return str;
        }
    }
}
