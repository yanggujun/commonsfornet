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

using Commons.Utils;
using System.Globalization;

namespace Commons.Json.Mapper
{
    internal class PrimitiveBuilder : ValueBuilderSkeleton
    {
        public PrimitiveBuilder(ConfigContainer configuration) : base(configuration)
        {
        }

        protected override object DoBuild(object raw, Type targetType, JValue jsonValue)
        {
            return ExtractPrimitiveValue(jsonValue, targetType);
        }

        protected override bool CanProcess(object raw, Type targetType, JValue jsonValue)
        {
            return (jsonValue.Is<JString>() || jsonValue.Is<JBoolean>()
                || jsonValue.Is<JInteger>() || jsonValue.Is<JDecimal>());
        }

        private object ExtractPrimitiveValue(JValue value, Type type)
        {
            JString str;
            JInteger integer;
            JBoolean boolean;
            JDecimal floating;
            object propertyValue;
            Type actualType;
            if (!type.IsNullable(out actualType))
            {
                actualType = type;
            }
            if (value.Is<JString>(out str))
            {
                if (actualType != typeof (string) && actualType != typeof (DateTime) && !actualType.IsEnum() && actualType != typeof(Guid))
                {
                    throw new InvalidCastException(Messages.JsonValueTypeNotMatch);
                }
                if (actualType == typeof (DateTime))
                {
                    DateTime dt;
                    if (TryParseDate(str, out dt))
                    {
                        propertyValue = dt;
                    }
                    else
                    {
                        throw new InvalidCastException(Messages.InvalidDateFormat);
                    }
                }
                else if (actualType.IsEnum())
                {
                    propertyValue = Enum.Parse(actualType, str);
                }
                else if (actualType == typeof(Guid))
                {
                    Guid guid;
                    if(!Guid.TryParse(str, out guid))
                    {
                        throw new InvalidCastException(Messages.InvalidDateFormat);
                    }
                    propertyValue = guid;
                }
                else
                {
                    string v = str;
                    propertyValue = v;
                }
            }
            else if (value.Is<JInteger>(out integer))
            {
                if (!actualType.IsJsonNumber())
                {
                    throw new InvalidCastException(Messages.JsonValueTypeNotMatch);
                }
                propertyValue = GetIntegerPropertyValue(actualType, integer);
            }
            else if (value.Is<JBoolean>(out boolean))
            {
                if (actualType != typeof (bool))
                {
                    throw new InvalidCastException(Messages.JsonValueTypeNotMatch);
                }
                bool v = boolean;
                propertyValue = v;
            }
            else if (value.Is<JDecimal>(out floating))
            {
                if (actualType != typeof (float) && actualType != typeof (double) && actualType != typeof (decimal))
                {
                    throw new InvalidCastException(Messages.JsonValueTypeNotMatch);
                }
                if (actualType == typeof (float))
                {
                    propertyValue = floating.AsSingle();
                }
                else if (actualType == typeof (double))
                {
                    propertyValue = floating.AsDouble();
                }
                else
                {
                    propertyValue = floating.AsDecimal();
                }
            }
            else if (value.Is<JNull>())
            {
                propertyValue = null;
            }
            else
            {
                // Unlikely to happen.
                throw new InvalidCastException(Messages.JsonValueTypeNotMatch);
            }
            return propertyValue;
        }

        private bool TryParseDate(string str, out DateTime dt)
        {
            object format;
            if (Configuration.TryGetValue("DateFormat", out format))
            {
                return DateTime.TryParse(str, out dt);
            }
            else
            {
                return DateTime.TryParseExact(str, (string)format, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out dt);
            }
        }

        private static object GetIntegerPropertyValue(Type propertyType, JInteger integer)
        {
            object integerObj = null;
            if (propertyType == typeof (long))
            {
                integerObj = integer.AsInt64();
            }
            else if (propertyType == typeof (int))
            {
                integerObj = integer.AsInt32();
            }
            else if (propertyType == typeof (byte))
            {
                integerObj = integer.AsByte();
            }
            else if (propertyType == typeof (sbyte))
            {
                integerObj = integer.AsSByte();
            }
            else if (propertyType == typeof (short))
            {
                integerObj = integer.AsInt16();
            }
            else if (propertyType == typeof (double))
            {
                integerObj = integer.AsDouble();
            }
            else if (propertyType == typeof (float))
            {
                integerObj = integer.AsSingle();
            }
            else if (propertyType == typeof (decimal))
            {
                integerObj = integer.AsDecimal();
            }
            else if (propertyType == typeof (ulong))
            {
                integerObj = integer.AsUInt64();
            }
            else if (propertyType == typeof (uint))
            {
                integerObj = integer.AsUInt32();
            }
            else if (propertyType == typeof (ushort))
            {
                integerObj = integer.AsUInt16();
            }

            return integerObj;
        }
    }
}
