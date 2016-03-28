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
using System.Globalization;
using System.Text;

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

	public class JString : JPrimitive<string>
	{
		public JString(string value)
		{
			As(value);
		}

		public override string ToString()
		{
			return new StringBuilder()
					.Append(JsonTokens.Quoter)
					.Append(Value)
					.Append(JsonTokens.Quoter).ToString();
		}

		public override bool Equals(object obj)
		{
			var theOther = obj as JString;
			return theOther != null && Value.Equals(theOther.Value);
		}

		public override int GetHashCode()
		{
			return Value == null ? 0 : Value.GetHashCode();
		}
	}


	public class JBoolean : JPrimitive<bool>
	{
        public JBoolean(bool b)
        {
            As(b);
        }

		public override bool Equals(object obj)
		{
            var b = obj as JBoolean;
            return b != null && Value.Equals(b.Value);
		}

		public override int GetHashCode()
		{
			return Value.GetHashCode();
		}

		public override string ToString()
		{
			return Value.ToString();
		}
	}

	public class JNull : JValue
	{
        private static JNull value = new JNull();

        private JNull()
        {
        }

        public static JNull Value { get { return value; } }

		public override string ToString()
		{
			return "null";
		}
	}

	public class JInteger : JPrimitive<long>
	{
        public JInteger(long integer)
        {
            As(integer);
        }
		public int AsInt32()
		{
			return Convert.ToInt32(Value);
		}

		public long AsInt64()
		{
			return Convert.ToInt64(Value);
		}

		public short AsInt16()
		{
			return Convert.ToInt16(Value);
		}

        public byte AsByte()
        {
            return Convert.ToByte(Value);
        }

        [CLSCompliant(false)]
		public ulong AsUInt64()
		{
			return Convert.ToUInt64(Value);
		}

        [CLSCompliant(false)]
		public uint AsUInt32()
		{
			return Convert.ToUInt32(Value);
		}

        [CLSCompliant(false)]
		public ushort AsUInt16()
		{
			return Convert.ToUInt16(Value);
		}

        [CLSCompliant(false)]
		public sbyte AsSByte()
		{
			return Convert.ToSByte(Value);
		}

		public double AsDouble()
		{
			return Convert.ToDouble(Value);
		}

		public float AsSingle()
		{
			return Convert.ToSingle(Value);
		}

		public decimal AsDecimal()
		{
			return Convert.ToDecimal(Value);
		}

		public override string ToString()
		{
			return Value.ToString(Culture);
		}

		public override bool Equals(object obj)
		{
            var integer = obj as JInteger;
            return integer != null && Value.Equals(integer.Value);
		}

		public override int GetHashCode()
		{
			return Value.GetHashCode();
		}
	}

	public class JDecimal : JPrimitive<decimal>
	{
        public JDecimal(decimal dec)
        {
            As(dec);
        }

		public float AsSingle()
		{
			return Convert.ToSingle(Value);
		}

		public double AsDouble()
		{
			return Convert.ToDouble(Value);
		}

        public decimal AsDecimal()
        {
            return Convert.ToDecimal(Value);
        }

		public override string ToString()
		{
			return Value.ToString(Culture);
		}

		public override bool Equals(object obj)
		{
            var dec = obj as JDecimal;
            return dec != null && Value.Equals(dec.Value);
		}

		public override int GetHashCode()
		{
			return Value.GetHashCode();
		}
	}
}
