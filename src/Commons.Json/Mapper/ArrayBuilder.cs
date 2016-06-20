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
        private readonly ILauncher launcher;
        private readonly MapperContainer mappers;
        public ArrayBuilder(ConfigContainer configuration, ILauncher launcher, MapperContainer mappers) : base(configuration)
        {
            this.launcher = launcher;
            this.mappers = mappers;
        }

        protected override object DoBuild(object raw, Type targetType, JValue jsonValue)
        {
            var itemType = targetType.GetElementType();
            return BuildArray(raw, itemType, jsonValue);
        }

        protected override bool CanProcess(object raw, Type targetType, JValue jsonValue)
        {
            return targetType.IsArray;
        }

        protected Array BuildArray(object raw, Type itemType, JValue jsonValue)
        {
            JArray array;
            if (!jsonValue.Is<JArray>(out array))
            {
                array = new JArray();
                array.Add(jsonValue);
            }
            return ExtractJsonArray(array, itemType);
        }

        private Array ExtractJsonArray(JArray jsonArray, Type itemType)
        {
            var array = Array.CreateInstance(itemType, jsonArray.Length);
            for (var i = 0; i < jsonArray.Length; i++)
            {
                var jsonValue = GetValueFromJsonArrayItem(jsonArray[i], itemType);
                if (jsonValue != null)
                {
                    array.SetValue(jsonValue, i);
                }
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
                return Successor.Build(null, itemType, jsonValue);
            }
            else
            {
                object itemValue;
                var mapper = mappers.GetMapper(itemType);
                if (mapper.ManualCreate != null)
                {
                    itemValue = mapper.ManualCreate(jsonValue);
                }
                else if (mapper.Create != null)
                {
                    itemValue = mapper.Create();
                }
                else
                {
                    itemValue = launcher.Launch(itemType);
                }
                itemValue = Successor.Build(itemValue, itemType, jsonValue);
                return itemValue;
            }
        }
    }
}
