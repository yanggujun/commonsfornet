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

namespace Commons.Json.Mapper
{
    internal class ArrayBuilder : ValueBuilderSkeleton
    {
        private readonly IObjectBuilder objBuilder;
        public ArrayBuilder(ConfigContainer configuration, IObjectBuilder objBuilder) : base(configuration)
        {
            this.objBuilder = objBuilder;
        }

        protected override object DoBuild(object raw, Type targetType, JValue jsonValue)
        {
            JArray array;
            if (!jsonValue.Is<JArray>(out array))
            {
                array = new JArray();
                array.Add(jsonValue);
            }
            return ExtractJsonArray(array, targetType);
        }

        protected override bool CanProcess(object raw, Type targetType, JValue jsonValue)
        {
            return targetType.IsArray;
        }

        private object ExtractJsonArray(JArray jsonArray, Type arrayType)
        {
            var itemType = arrayType.GetElementType();
            var array = Array.CreateInstance(itemType, jsonArray.Length);
            for (var i = 0; i < jsonArray.Length; i++)
            {
                var jsonValue = GetValueFromJsonArrayItem(jsonArray[i], itemType);
                array.SetValue(jsonValue, i);
            }

            return array;
        }

        protected object GetValueFromJsonArrayItem(JValue jsonValue, Type itemType)
        {
            if (itemType.IsJsonPrimitive())
            {
                return Successor.Build(null, itemType, jsonValue);
            }
            else if (itemType.IsArray)
            {
                JArray array;
                if (!jsonValue.Is<JArray>(out array))
                {
                    array = new JArray();
                    array.Add(jsonValue);
                }
                return ExtractJsonArray(array, itemType);
            }
            else
            {
                var itemValue = objBuilder.Build(itemType);
                Successor.Build(itemValue, itemType, jsonValue);
                return itemValue;
            }
        }
    }
}
