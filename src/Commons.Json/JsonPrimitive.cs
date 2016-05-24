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
using System.Reflection;
using System.Text;

namespace Commons.Json
{
    [CLSCompliant(true)]
    public class JsonPrimitive : JsonValue
    {
        private object primitive;
        public JsonPrimitive(object value)
        {
            if (value != null)
            {
                var type = value.GetType();
                if (!type.GetTypeInfo().IsPrimitive && type != typeof(string) && type != typeof(DateTime))
                {
                    throw new ArgumentException(Messages.InvalidValue);
                }
                primitive = value;
            }
        }

        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            if (primitive == null)
            {
                result = null;
            }
            else if (binder.Type == typeof (double))
            {
                result = Convert.ToDouble(primitive);
            }
            else if (binder.Type == typeof (float))
            {
                result = Convert.ToSingle(primitive);
            }
            else if (binder.Type == typeof (decimal))
            {
                result = Convert.ToDecimal(primitive);
            }
            else if (binder.Type == typeof (Int64))
            {
                result = Convert.ToInt64(primitive);
            }
            else if (binder.Type == typeof (UInt64))
            {
                result = Convert.ToUInt64(primitive);
            }
            else if (binder.Type == typeof (Int32))
            {
                result = Convert.ToInt32(primitive);
            }
            else if (binder.Type == typeof (UInt32))
            {
                result = Convert.ToUInt32(primitive);
            }
            else if (binder.Type == typeof (Int16))
            {
                result = Convert.ToInt16(primitive);
            }
            else if (binder.Type == typeof (UInt16))
            {
                result = Convert.ToUInt16(primitive);
            }
            else if (binder.Type == typeof (byte))
            {
                result = Convert.ToByte(primitive);
            }
            else if (binder.Type == typeof (sbyte))
            {
                result = Convert.ToSByte(primitive);
            }
            else
            {
                result = primitive;
            }
            return true;
        }

        public override string ToString()
        {
            var result = string.Empty;
            if (primitive == null)
            {
                result = JsonTokens.Null;
            }
            else if (primitive is string || primitive is DateTime)
            {
                var sb = new StringBuilder();
                sb.Append(JsonTokens.Quoter);
                sb.Append(primitive.ToString());
                sb.Append(JsonTokens.Quoter);
                result = sb.ToString();
            }
            else
            {
                result = primitive.ToString();
            }

            return result;
        }
    }
}
