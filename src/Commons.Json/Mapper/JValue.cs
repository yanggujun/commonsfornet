// Copyright CommonsForNET.  // Licensed to the Apache Software Foundation (ASF) under one or more
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
using System.Globalization;
using Commons.Collections.Map;

namespace Commons.Json.Mapper
{
	public abstract class JValue
	{
		private CultureInfo culture = CultureInfo.InvariantCulture;

		public virtual CultureInfo Culture 
		{
			get { return culture; }
			set { culture = value; }
		}
	}

	public abstract class JPrimitive<T> : JValue
	{
		public T Value { get; private set; }

		public virtual JPrimitive<T> As(T value)
		{
			Value = value;
			return this;
		}

		public static implicit operator T(JPrimitive<T> primitive)
		{
			return primitive.Value;
		}

		public override string ToString()
		{
			if (Value != null)
			{
				return Value.ToString();
			}
			return string.Empty;
		}

		public override bool Equals(object obj)
		{
			var target = obj as JPrimitive<T>;
			if (target != null && target.Value != null)
			{
				return target.Value.Equals(Value);
			}
			return false;
		}

		public override int GetHashCode()
		{
			if (Value != null)
			{
				return Value.GetHashCode();
			}
			return 0;
		}
	}

	public class JObject : JValue, IEnumerable<KeyValuePair<string, JValue>>
	{
		private HashedMap<string, JValue> values = new HashedMap<string, JValue>();
		private string lastKey;
		public void PutKey(JString key)
		{
			if (key == null)
			{
				throw new ArgumentException();
			}
			lastKey = key;
		}

		public void PutObject(JValue value)
		{
			if (lastKey == null)
			{
				throw new ArgumentException("blaaaah");
			}
			values.Add(lastKey, value);
			lastKey = null;
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

		public IEnumerator<KeyValuePair<string, JValue>> GetEnumerator()
		{
			return values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}

	public class JString : JPrimitive<string>
	{
		public JString(string value)
		{
			As(value);
		}
	}

	public class JArray : JValue, IEnumerable
	{
		private List<JValue> values = new List<JValue>();

		public void Add(JValue value)
		{
			values.Add(value);
		}

		public JValue this[int index]
		{
			get
			{
				if (index < values.Count)
				{
					return values[index];
				}
				return null;
			}
		}

		public IEnumerator<JValue> GetEnumerator()
		{
			return values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}

	public class JBoolean : JPrimitive<bool>
	{
	}

	public class JNull : JValue
	{
		public override string ToString()
		{
			return "null";
		}
	}

	public class JInteger : JPrimitive<long>
	{
		public int AsInt()
		{
			return (int) Value;
		}

		public long AsLong()
		{
			return Value;
		}
	}

	public class JDecimal : JPrimitive<decimal>
	{
		public float AsFloat()
		{
			return Convert.ToSingle(Value);
		}

		public double AsDouble()
		{
			return Convert.ToDouble(Value);
		}

		public override string ToString()
		{
			return Value.ToString(Culture);
		}
	}
}
