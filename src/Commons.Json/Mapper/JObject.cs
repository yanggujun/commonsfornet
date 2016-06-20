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
        private HashedMap<string, JValue> values = new HashedMap<string, JValue>(new IgnoreCaseStringEquator());

        private string lastKey;
        public void PutKey(JString key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            lastKey = key;
        }

        public void PutObject(JValue value)
        {
            if (lastKey == null)
            {
                throw new ArgumentException(Messages.InvalidFormat);
            }
            values.Add(lastKey, value);
            lastKey = null;
        }

        public bool Validate()
        {
            return lastKey == null;
        }

        public JValue this[JString key]
        {
            get { return values[key]; }
            set { values[key] = value; }
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

        public int GetInt32(string key)
        {
            return GetInteger(values[key]).AsInt32();
        }

        public void SetInt32(string key, int integer)
        {
            values[key] = new JInteger(integer);
        }

        [CLSCompliant(false)]
        public uint GetUInt32(string key)
        {
            return GetInteger(values[key]).AsUInt32();
        }

        [CLSCompliant(false)]
        public void SetUInt32(string key, uint integer)
        {
            values[key] = new JInteger(integer);
        }

        public short GetInt16(string key)
        {
            return GetInteger(values[key]).AsInt16();
        }

        public void SetInt16(string key, short integer)
        {
            values[key] = new JInteger(integer);
        }

        [CLSCompliant(false)]
        public ushort GetUInt16(string key)
        {
            return GetInteger(values[key]).AsUInt16();
        }

        [CLSCompliant(false)]
        public void SetUInt16(string key, ushort integer)
        {
            values[key] = new JInteger(integer);
        }

        public byte GetByte(string key)
        {
            return GetInteger(values[key]).AsByte();
        }

        public void SetByte(string key, byte integer)
        {
            values[key] = new JInteger(integer);
        }

        [CLSCompliant(false)]
        public sbyte GetSByte(string key)
        {
            return GetInteger(values[key]).AsSByte();
        }

        [CLSCompliant(false)]
        public void SetSByte(string key, sbyte integer)
        {
            values[key] = new JInteger(integer);
        }

        public long GetInt64(string key)
        {
            return GetInteger(values[key]).AsInt64();
        }

        public void SetInt64(string key, long integer)
        {
            values[key] = new JInteger(integer);
        }

        public float GetSingle(string key)
        {
            return GetDecimal(values[key]).AsSingle();
        }

        public void SetSingle(string key, float dec)
        {
            values[key] = new JDecimal(Convert.ToDecimal(dec));
        }

        public double GetDouble(string key)
        {
            return GetDecimal(values[key]).AsDouble();
        }

        public void SetDouble(string key, double dec)
        {
            values[key] = new JDecimal(Convert.ToDecimal(dec));
        }

        public decimal GetDecimal(string key)
        {
            return GetDecimal(values[key]).AsDecimal();
        }

        public void SetDecimal(string key, decimal dec)
        {
            values[key] = new JDecimal(dec);
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

        public bool GetBool(string key)
        {
            var b = values[key] as JBoolean;
            if (b == null)
            {
                throw new InvalidCastException(Messages.JsonValueTypeNotMatch);
            }
            return b.Value;
        }

        public void SetBool(string key, bool b)
        {
            values[key] = new JBoolean(b);
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
        
        private JInteger GetInteger(JValue value)
        {
            var integer = value as JInteger;
            if (integer == null)
            {
                throw new InvalidCastException(Messages.JsonValueTypeNotMatch);
            }

            return integer;
        }

        private JDecimal GetDecimal(JValue value)
        {
            var dec = value as JDecimal;
            if (dec == null)
            {
                throw new InvalidCastException(Messages.JsonValueTypeNotMatch);
            }

            return dec;
        }
    }
}
