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
using Commons.Json.Mapper;

namespace Commons.Json
{
    public class JsonValue : DynamicObject
    {
	    private JValue jsonValue;

        public JsonValue()
        {

        }

        public JsonValue(JValue value)
        {
	        jsonValue = value;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var success = false;
            if (null != jsonValue)
            {
	            var obj = jsonValue as JObject;
	            if (obj == null)
	            {
		            throw new InvalidOperationException();
	            }
                result = new JsonValue(JValue.From(obj[binder.Name]));
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
            if (null != jsonValue)
            {
	            var obj = jsonValue as JObject;
	            if (null == obj)
	            {
		            throw new InvalidOperationException();
	            }
                obj[binder.Name] = JValue.From(value);
            }
            else
            {
                var newObj = new JObject();
                newObj[binder.Name] = JValue.From(value);
                jsonValue = newObj;
            }

            return true;
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            var hasValue = false;
            if (jsonValue != null && indexes.Length > 0)
            {
                int index;
                if (int.TryParse(indexes[0].ToString(), out index) && index >= 0)
                {
                    var array = jsonValue as JArray;
                    if (array == null)
                    {
                        throw new InvalidOperationException();
                    }
                    result = new JsonValue(array[index]);
                    hasValue = true;
                }
                else if (indexes[0].GetType().IsAssignableFrom(typeof(string)))
                {
                    var jsonObject = jsonValue as JObject;
                    if (jsonObject == null)
                    {
                        throw new InvalidOperationException();
                    }
                    result = new JsonValue(jsonObject[indexes[0] as string]);
                    hasValue = true;
                }
                else
                {
                    hasValue = base.TryGetIndex(binder, indexes, out result);
                }
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
            if (jsonValue != null && int.TryParse(indexes[0].ToString(), out index) && index >= 0)
            {
                var array = jsonValue as JArray;
                if (array == null)
                {
                    throw new InvalidOperationException();
                }
                if (index > array.Length - 1)
                {
                    throw new InvalidOperationException();
                }
                array[index] = JValue.From(value);
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
            return jsonValue == null ? "null" : jsonValue.ToString();
        }
    }
}
