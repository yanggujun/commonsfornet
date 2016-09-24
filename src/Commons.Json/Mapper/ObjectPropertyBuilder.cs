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
using System.Linq;

namespace Commons.Json.Mapper
{
    internal class ObjectPropertyBuilder : ValueBuilderSkeleton
    {
        private readonly MapperContainer mappers;
        private readonly TypeContainer types;
        public ObjectPropertyBuilder(ConfigContainer configuration, MapperContainer mappers, TypeContainer types) : base(configuration)
        {
            this.mappers = mappers;
            this.types = types;
        }

        protected override bool CanProcess(Type targetType, JValue jsonValue)
        {
            return jsonValue.Is<JObject>() && !targetType.IsDictionary();
        }

        protected override object DoBuild(Type targetType, JValue jsonValue)
        {
            if (targetType.IsJsonArray() || targetType.IsJsonPrimitive() || targetType == typeof(string))
            {
                throw new InvalidCastException(Messages.JsonValueTypeNotMatch);
            }
            return PopulateJsonObject(targetType, (JObject)jsonValue);
        }

        private object PopulateJsonObject(Type type, JObject jsonObj)
        {
            var properties = types[type].Setters;
            MapperImpl mapper = mappers.GetMapper(type);
            object target;
            if (mapper.Create != null)
            {
                target = mapper.Create();
            }
            else if (mapper.ManualCreate != null)
            {
                target = mapper.ManualCreate(jsonObj);
                return target;
            }
            else
            {
                target = types[type].Launch();
            }

            foreach (var prop in properties)
            {
                if (mapper.IsPropertyIgnored(prop.Value1))
                {
                    continue;
                }

                var key = mapper.GetKey(prop.Value1);
                var propertyType = prop.Value2;
                if (jsonObj.ContainsKey(key))
                {
                    var jsonValue = jsonObj[key];
                    var value = Successor.Build(propertyType, jsonValue);
                    prop.Value3(target, value);

                    //if (jsonValue.Is<JNull>())
                    //{
                    //    prop.Value3(target, null);
                    //}
                    //else if (propertyType.IsJsonPrimitive())
                    //{
                    //    var value = Successor.Build(propertyType, jsonValue);
                    //    prop.Value3(target, value);
                    //}
                    //else if (propertyType.IsArray)
                    //{
                    //    var value = Successor.Build(propertyType, jsonValue);
                    //    prop.Value3(target, value);
                    //}
                    //else
                    //{
                    //    var propMapper = mappers.GetMapper(propertyType);
                    //    object propValue = null;
                    //    if (propMapper.ManualCreate != null)
                    //    {
                    //        propValue = propMapper.ManualCreate(jsonValue);
                    //        prop.Value3(target, propValue);
                    //    }
                    //    else
                    //    {
                    //        propValue = Successor.Build(propertyType, jsonValue);
                    //        prop.Value3(target, propValue);
                    //    }
                    //}
                }
            }

            return target;
        }
    }
}
