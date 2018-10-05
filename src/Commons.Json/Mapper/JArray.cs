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

using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Commons.Json.Mapper
{
    public class JArray : JValue, IEnumerable
    {
        private readonly List<JValue> values = new List<JValue>();

        public void Add(JValue value)
        {
            values.Add(value);
        }

        public int Count
        {
            get
            {
                return values.Count;
            }
        }

        public void Add(int i)
        {
            values.Add(new JNumber(i));
        }

        public void Add(long i)
        {
            values.Add(new JNumber(i));
        }

        public void Add(short i)
        {
            values.Add(new JNumber(i));
        }

        public void Add(byte i)
        {
            values.Add(new JNumber(i));
        }

        public void Add(float i)
        {
            values.Add(new JNumber(i));
        }

        public void Add(double i)
        {
            values.Add(new JNumber(i));
        }

        public void Add(decimal i)
        {
            values.Add(new JNumber(i));
        }

#pragma warning disable 3001, 3002

        public void Add(uint i)
        {
            values.Add(new JNumber(i));
        }

        public void Add(ushort i)
        {
            values.Add(new JNumber(i));
        }

        public void Add(ulong i)
        {
            values.Add(new JNumber(i));
        }

        public void Add(sbyte i)
        {
            values.Add(new JNumber(i));
        }
#pragma warning restore 3001, 3002

        public void AddString(string s)
        {
            values.Add(new JString(s));
        }

        public void AddBool(bool b)
        {
            if (b)
            {
                values.Add(JBool.True);
            }
            else
            {
                values.Add(JBool.False);
            }
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

        public int Length
        {
            get { return values.Count; }
        }

        public void Clear()
        {
            values.Clear();
        }

        public IEnumerator<JValue> GetEnumerator()
        {
            return values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(JsonTokens.LeftBracket);
            foreach (var item in values)
            {
                sb.Append(item).Append(JsonTokens.Comma);
            }
            sb.Remove(sb.Length - 1, 1);
            sb.Append(JsonTokens.RightBracket);
            return sb.ToString();
        }
    }
}
