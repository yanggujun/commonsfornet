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
using System.Dynamic;
using System.Text;

namespace Commons.Json
{
    [CLSCompliant(true)]
    public class JsonArray : JsonValue
    {
        private readonly List<JsonValue> values;

        public JsonArray()
        {
            values = new List<JsonValue>();
        }
        public JsonArray(JsonValue[] values)
        {
            this.values = new List<JsonValue>(values);
        }

        public void Add(JsonValue value)
        {
            values.Add(value);
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            var index = 0;

            try
            {
                index = (int) indexes[0];
            }
            catch
            {
                throw new InvalidOperationException(Messages.IndexerCannotApply);
            }

            if (index < values.Count)
            {
                result = values[index];
            }
            else
            {
                throw new InvalidOperationException(Messages.OutOfRange);
            }
            return true;
        }

        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            var index = 0;

            try
            {
                index = (int) indexes[0];
            }
            catch
            {
                throw new InvalidOperationException(Messages.IndexerCannotApply);
            }

            if (index < values.Count)
            {
                values[index] = From(value);
            }
            else if (index == values.Count)
            {
                values.Add(From(value));
            }
            else
            {
                throw new InvalidOperationException(Messages.OutOfRange);
            }

            return true;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            throw new InvalidOperationException();
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            throw new InvalidOperationException();
        }

        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            var jsonValues = new JsonValue[values.Count];
            for (var i = 0; i < jsonValues.Length; i++)
            {
                jsonValues[i] = values[i];
            }
            result = jsonValues;
            return true;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(JsonTokens.LeftBracket).Append(JsonTokens.Space);
            for (var i = 0; i < values.Count; i++)
            {
                sb.Append(values[i].ToString());
                if (i != values.Count - 1)
                {
                    sb.Append(JsonTokens.Comma);
                }
            }
            sb.Append(JsonTokens.RightBracket);
            return sb.ToString();
        }

        public int Length
        {
            get { return values.Count; }
        }

    }
}
