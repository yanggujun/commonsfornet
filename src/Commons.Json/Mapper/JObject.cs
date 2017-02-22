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
using System.Collections.Generic;
using System.Text;
using Commons.Collections.Map;
using System.Reflection;
using Commons.Utils;

namespace Commons.Json.Mapper
{
    public class JObject : JValue, IEnumerable<KeyValuePair<string, JValue>>
    {
        private Dictionary<string, JValue> values = new Dictionary<string, JValue>();

        private string lastKey;
        public void PutKey(JString key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            lastKey = key.Value;
        }

        public void PutObject(JValue value)
        {
            if (lastKey == null)
            {
                throw new ArgumentException(Messages.InvalidFormat);
            }
            values[lastKey] = value;
            lastKey = null;
        }

        public bool Validate()
        {
            return lastKey == null;
        }

        public JValue this[JString key]
        {
            get { return values[key.Value]; }
            set { values[key.Value] = value; }
        }

        public JValue this[string key]
        {
            get { return values[key]; }
            set { values[key] = value; }
        }

        public bool ContainsKey(string key)
        {
            return values.ContainsKey(key);
        }

        public string GetString(string key)
        {
            if (values[key].Is<JNull>())
            {
                return null;
            }
            var str = values[key] as JString;
            if (str == null)
            {
                throw new InvalidCastException(Messages.JsonValueTypeNotMatch);
            }
            return str.Value;
        }

        public void SetString(string key, string str)
        {
            values[key] = new JString(str);
        }

        public T GetEnum<T>(string key) where T : struct
        {
            var type = typeof(T);
            if (!type.IsEnum())
            {
                throw new ArgumentException(Messages.NotEnum);
            }

            var str = GetString(key);
            T result;
            if (!Enum.TryParse<T>(str, out result))
            {
                throw new InvalidCastException(Messages.JsonValueTypeNotMatch);
            }

            return result;
        }

        public void SetEnum<T>(string key, T value) where T : struct
        {
            var type = typeof(T);
            if (!type.IsEnum())
            {
                throw new ArgumentException(Messages.NotEnum);
            }

            SetString(key, value.ToString());
        }

        public JArray GetArray(string key)
        {
            var array = values[key] as JArray;
            if (array == null)
            {
                throw new InvalidCastException(Messages.JsonValueTypeNotMatch);
            }
            return array;
        }

        public void SetArray(string key, JArray array)
        {
            values[key] = array;
        }

        public bool IsNull(string key)
        {
            return (values[key] as JNull) == null;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(JsonTokens.LeftBrace);
            foreach (var item in values)
            {
                sb.Append(JsonTokens.Quoter)
                    .Append(item.Key)
                    .Append(JsonTokens.Quoter)
                    .Append(JsonTokens.Colon)
                    .Append(item.Value).Append(JsonTokens.Comma);
            }
            sb.Remove(sb.Length - 1, 1);
            sb.Append(JsonTokens.RightBrace);
            return sb.ToString();
        }

        public IEnumerator<KeyValuePair<string, JValue>> GetEnumerator()
        {
            return values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
