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

using Commons.Utils;

namespace Commons.Json.Mapper
{
	public class JObject : JValue, IEnumerable<KeyValuePair<string, JValue>>
	{
		private readonly Dictionary<string, JValue> values = new Dictionary<string, JValue>();

		public JValue this[string key]
		{
			get { return values[key]; }
			set { values[key] = value; }
		}

		public bool ContainsKey(string key)
		{
			return values.ContainsKey(key);
		}

		public bool TryGetValue(string key, out JValue jv)
		{
			return values.TryGetValue(key, out jv);
		}

		public string GetString(string key)
		{
			if (values.ContainsKey(key))
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
			else
			{
				throw new ArgumentException(Messages.KeyNotExist);
			}
		}

		public void SetString(string key, string str)
		{
			values[key] = new JString(str);
		}

		public void SetFloat(string key, float v)
		{
			values[key] = new JNumber(v);
		}

		public float GetFloat(string key)
		{
			return GetNumber(key).GetFloat();
		}

		public void SetDouble(string key, double v)
		{
			values[key] = new JNumber(v);
		}

		public double GetDouble(string key)
		{
			return GetNumber(key).GetDouble();
		}

		public void SetDecimal(string key, decimal v)
		{
			values[key] = new JNumber(v);
		}

		public Decimal GetDecimal(string key)
		{
			return GetNumber(key).GetDecimal();
		}

		public void SetInt32(string key, int v)
		{
			values[key] = new JNumber(v);
		}

		public int GetInt32(string key)
		{
			return GetNumber(key).GetInt32();
		}

		public void SetInt64(string key, long v)
		{
			values[key] = new JNumber(v);
		}

		public long GetInt64(string key)
		{
			return GetNumber(key).GetInt64();
		}

		public void SetInt16(string key, short v)
		{
			values[key] = new JNumber(v);
		}

		public short GetInt16(string key)
		{
			return GetNumber(key).GetInt16();
		}

		public void SetByte(string key, byte v)
		{
			values[key] = new JNumber(v);
		}

		public byte GetByte(string key)
		{
			return GetNumber(key).GetByte();
		}

		public void SetValue(string key, JValue value)
		{
			values[key] = value;
		}
#pragma warning disable 3001, 3002

		public void SetUInt32(string key, uint v)
		{
			values[key] = new JNumber(v);
		}

		public uint GetUInt32(string key)
		{
			return GetNumber(key).GetUInt32();
		}

		public void SetUInt64(string key, ulong v)
		{
			values[key] = new JNumber(v);
		}

		public ulong GetUInt64(string key)
		{
			return GetNumber(key).GetUInt64();
		}

		public void SetUInt16(string key, ushort v)
		{
			values[key] = new JNumber(v);
		}

		public ushort GetUInt16(string key)
		{
			return GetNumber(key).GetUInt16();
		}

		public void SetSByte(string key, sbyte v)
		{
			values[key] = new JNumber(v);
		}

		public sbyte GetSByte(string key)
		{
			return GetNumber(key).GetSByte();
		}

#pragma warning restore 3001, 3002

		public void SetBool(string key, bool b)
		{
			if (b)
			{
				values[key] = JBool.True;
			}
			else
			{
				values[key] = JBool.False;
			}
		}

		public bool GetBool(string key)
		{
			var value = GetValue(key) as JBool;
			if (value == null)
			{
				throw new InvalidCastException(Messages.JsonValueTypeNotMatch);
			}

			return value == JBool.True;
		}

		public JObject GetObject(string key)
		{
			var value = GetValue(key) as JObject;
			if (value == null)
			{
				throw new InvalidCastException(Messages.JsonValueTypeNotMatch);
			}
			return value;
		}

		public JArray GetArray(string key)
		{
			var value = GetValue(key) as JArray;
			if (value == null)
			{
				throw new InvalidCastException(Messages.JsonValueTypeNotMatch);
			}
			return value;
		}

		public JValue GetValue(string key)
		{
			if (values.ContainsKey(key))
			{
				return values[key];
			}
			else
			{
				throw new ArgumentException(Messages.KeyNotExist);
			}
		}

		public void SetEnum<T>(string key, T value) where T : struct
		{
			if (typeof(T).IsEnum())
			{
				SetString(key, value.ToString());
			}
			else
			{
				throw new InvalidCastException(Messages.NotEnum);
			}
		}

		public T GetEnum<T>(string key) where T : struct
		{
			if (typeof(T).IsEnum())
			{
				T result;
				if (!Enum.TryParse<T>(GetString(key), out result))
				{
					throw new InvalidCastException(Messages.JsonValueTypeNotMatch);
				}
				return result;
			}
			else
			{
				throw new InvalidCastException(Messages.NotEnum);
			}
		}

        public bool IsNull(string key)
        {
			return values[key] == JNull.Null;
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

		private JNumber GetNumber(string key)
		{
			var number = GetValue(key) as JNumber;
			if (number == null)
			{
				throw new InvalidCastException(Messages.InvalidNumber);
			}
			return number;
		}
    }
}
