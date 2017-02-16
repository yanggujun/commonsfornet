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
namespace Commons.Json.Mapper
{
    public class JArray : JValue, IEnumerable
    {
        private List<JValue> values = new List<JValue>();

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
        
        public void AddByte(byte integer)
        {
            values.Add(new JInteger(integer));
        }
        
        [CLSCompliant(false)]
        public void AddSByte(sbyte integer)
        {
            values.Add(new JInteger(integer));
        }

        public void AddInt16(short integer)
        {
            values.Add(new JInteger(integer));
        }

        [CLSCompliant(false)]
        public void AddUInt16(ushort integer)
        {
            values.Add(new JInteger(integer));
        }

        public void AddInt32(int integer)
        {
            values.Add(new JInteger(integer));
        }

        [CLSCompliant(false)]
        public void AddUInt32(uint integer)
        {
            values.Add(new JInteger(integer));
        }

        public void AddInt64(long integer)
        {
            values.Add(new JInteger(integer));
        }

        public void AddBool(bool b)
        {
            values.Add(new JBoolean(b));
        }

        public void AddString(string str)
        {
            values.Add(new JString(str));
        }

        public void AddNull(string key)
        {
            values.Add(JNull.Value);
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
