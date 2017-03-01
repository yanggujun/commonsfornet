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

    public class JPrimitive : JValue
    {
        public JPrimitive(string value, PrimitiveType pType)
        {
            Value = value;
            PrimitiveType = pType;
        }

        public PrimitiveType PrimitiveType { get; private set; }
    }

    public class JString : JValue
    {
		public JString(string value)
		{
			Value = value;
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
    }

    public enum PrimitiveType
    {
        Decimal,
        Integer
    }

}
