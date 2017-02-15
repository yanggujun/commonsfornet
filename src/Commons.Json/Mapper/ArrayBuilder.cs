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
        protected readonly MapperContainer mappers;
        public ArrayBuilder(ConfigContainer configuration, MapperContainer mappers) : base(configuration)
        {
            this.mappers = mappers;
        }

        protected override object DoBuild(Type targetType, JValue jsonValue)
        {
			if (targetType == typeof(byte[]))
			{
				JString jsonStr;
				if (jsonValue.Is<JArray>())
				{
					return BuildArray(typeof(byte), jsonValue);
				}
				else if (jsonValue.Is<JString>(out jsonStr))
				{
					return Convert.FromBase64String(jsonStr.Value);
				}
				else
				{
					throw new InvalidCastException(Messages.JsonValueTypeNotMatch);
				}
			}
			else
			{
				var itemType = targetType.GetElementType();
				return BuildArray(itemType, jsonValue);
			}
        }

        protected override bool CanProcess(Type targetType, JValue jsonValue)
        {
            return targetType.IsArray;
        }

        protected Array BuildArray(Type itemType, JValue jsonValue)
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
            //TODO: do not use reflection
            var array = Array.CreateInstance(itemType, jsonArray.Length);
            for (var i = 0; i < jsonArray.Length; i++)
            {
                var jsonValue = Successor.Build(itemType, jsonArray[i]);
                if (jsonValue != null)
                {
                    array.SetValue(jsonValue, i);
                }
            }

            return array;
        }
    }
}
