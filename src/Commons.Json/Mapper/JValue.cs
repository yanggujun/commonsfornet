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

		public virtual void As(T value)
		{
			Value = value;
		}

		public static implicit operator T(JPrimitive<T> primitive)
		{
			return primitive.Value;
		}

		public override string ToString()
		{
			return Value.ToString();
		}
	}

	public class JObject : JValue
	{
		private HashedMap<JString, JValue> values = new HashedMap<JString, JValue>();
		private JString lastKey;
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
	}

	public class JString : JPrimitive<string>
	{
		public JString(string value)
		{
			As(value);
		}
	}

	public class JArray : JValue
	{
		private List<JValue> values = new List<JValue>();

		public void Add(JValue value)
		{
			values.Add(value);
		}
	}

	public class JBoolean : JPrimitive<bool>
	{
	}

	public class JNull : JPrimitive<string>
	{
		public override void As(string value)
		{
			throw new InvalidOperationException("Value cannot be set to a null");
		}

		public override string ToString()
		{
			return "null";
		}
	}

	public class JNumber : JPrimitive<long>
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
