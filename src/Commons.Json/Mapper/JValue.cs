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

        public string Value { get; protected set; }
    }

    public class JNumber : JValue
    {
        public JNumber(string value, NumberType pType)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException(Messages.InvalidNumber);
            } 
            Value = value;
            NumType = pType;
        }

        public JNumber(Int32 v)
        {
            Value = v.ToString();
            NumType = NumberType.Integer;
        }

        public JNumber(Int64 v)
        {
            Value = v.ToString();
            NumType = NumberType.Integer;
        }

        public JNumber(Int16 v)
        {
            Value = v.ToString();
            NumType = NumberType.Integer;
        }

        public JNumber(byte v)
        {
            Value = v.ToString();
            NumType = NumberType.Integer;
        }

        public JNumber(float v)
        {
            Value = v.ToString(CultureInfo.InvariantCulture);
            NumType = NumberType.Decimal;
        }

        public JNumber(double v)
        {
            Value = v.ToString(CultureInfo.InvariantCulture);
            NumType = NumberType.Decimal;
        }

        public JNumber(decimal v)
        {
            Value = v.ToString(CultureInfo.InvariantCulture);
            NumType = NumberType.Decimal;
        }

#pragma warning disable 3001, 3002

        public JNumber(UInt64 v)
        {
            Value = v.ToString(CultureInfo.InvariantCulture);
            NumType = NumberType.Integer;
        }

        public JNumber(UInt16 v)
        {
            Value = v.ToString(CultureInfo.InvariantCulture);
            NumType = NumberType.Integer;
        }

        public JNumber(UInt32 v)
        {
            Value = v.ToString(CultureInfo.InvariantCulture);
            NumType = NumberType.Integer;
        }

        public JNumber(sbyte v)
        {
            Value = v.ToString(CultureInfo.InvariantCulture);
            NumType = NumberType.Integer;
        }
#pragma warning restore 3001, 3002

        public NumberType NumType { get; private set; }

        public float GetFloat()
        {
            return float.Parse(Value);
        }

        public double GetDouble()
        {
            return double.Parse(Value);
        }

        public decimal GetDecimal()
        {
            return decimal.Parse(Value);
        }

        public int GetInt32()
        {
            return int.Parse(Value);
        }

        public long GetInt64()
        {
            return long.Parse(Value);
        }

        public short GetInt16()
        {
            return short.Parse(Value);
        }

        public byte GetByte()
        {
            return byte.Parse(Value);
        }

#pragma warning disable 3002
        public uint GetUInt32()
        {
            return uint.Parse(Value);
        }

        public ulong GetUInt64()
        {
            return ulong.Parse(Value);
        }

        public ushort GetUInt16()
        {
            return ushort.Parse(Value);
        }

        public sbyte GetSByte()
        {
            return sbyte.Parse(Value);
        }
#pragma warning restore 3002

        public override string ToString()
        {
            return Value;
        }
    }

    public class JString : JValue
    {
        public JString(string value)
        {
            Value = value;
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(Value))
            {
                return new StringBuilder().Append(JsonTokens.Quoter).Append(JsonTokens.Quoter).ToString();
            }
            return new StringBuilder().Append(JsonTokens.Quoter).Append(Value).Append(JsonTokens.Quoter).ToString();
        }
    }

    public class JBool : JValue
    {
        private static object trueLock = new object();
        private static object falseLock = new object();
        private static JBool trueValue;
        private static JBool falseValue;

        private JBool()
        {
        }

        public static JBool True
        {
            get
            {
                if (trueValue == null)
                {
                    lock (trueLock)
                    {
                        if (trueValue == null)
                        {
                            trueValue = new JBool();
                            trueValue.Value = JsonTokens.True;
                            return trueValue;
                        }
                    }
                }

                return trueValue;
            }
        }

        public static JBool False
        {
            get
            {
                if (falseValue == null)
                {
                    lock (falseLock)
                    {
                        if (falseValue == null)
                        {
                            falseValue = new JBool();
                            falseValue.Value = JsonTokens.False;
                            return falseValue;
                        }
                    }
                }
                return falseValue;
            }
        }

        public override string ToString()
        {
            return Value;
        }
    }

    public class JNull : JValue
    {
        private static object locker = new object();
        private static JNull instance;
        private JNull()
        {
        }

        public static JNull Null
        {
            get
            {
                if (instance == null)
                {
                    lock (locker)
                    {
                        if (instance == null)
                        {
                            instance = new JNull();
                            return instance;
                        }
                    }
                }
                return instance;
            }
        }

        public override string ToString()
        {
            return JsonTokens.Null;
        }
    }

    public enum NumberType
    {
        Decimal,
        Integer
    }

}
