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
    internal class ObjectPropertyBuilder : ValueBuilderSkeleton
    {
        private readonly MapperContainer mappers;
        private readonly TypeContainer types;
        public ObjectPropertyBuilder(ConfigContainer configuration, MapperContainer mappers, TypeContainer types) : base(configuration)
        {
            this.mappers = mappers;
            this.types = types;
        }

        protected override bool CanProcess(object raw, Type targetType, JValue jsonValue)
        {
            return jsonValue.Is<JObject>() && !targetType.IsDictionary();
        }

        protected override object DoBuild(object raw, Type targetType, JValue jsonValue)
        {
            if (targetType.IsJsonArray() || targetType.IsJsonPrimitive() || targetType == typeof(string))
            {
                throw new InvalidCastException(Messages.JsonValueTypeNotMatch);
            }
            PopulateJsonObject(raw, targetType, (JObject)jsonValue);
            return raw;
        }

        private void PopulateJsonObject(object target, Type type, JObject jsonObj)
        {
            var properties = types[type].Setters;
            MapperImpl mapper = mappers.GetMapper(type);

            foreach (var prop in properties)
            {
                if (mapper.IsPropertyIgnored(prop.Name))
                {
                    continue;
                }

                var key = mapper.GetKey(prop.Name);
                if (jsonObj.ContainsKey(key))
                {
                    var propertyType = prop.PropertyType;
                    var jsonValue = jsonObj[key];

                    if (jsonValue.Is<JNull>())
                    {
                        prop.SetValue(target, null, null);
                    }
                    else if (propertyType.IsJsonPrimitive())
                    {
                        var value = Successor.Build(null, propertyType, jsonValue);
                        prop.SetValue(target, value, null);
                    }
                    else if (propertyType.IsArray)
                    {
                        var value = Successor.Build(null, propertyType, jsonValue);
                        prop.SetValue(target, value, null);
                    }
                    else
                    {
                        var propMapper = mappers.GetMapper(propertyType);
                        object propValue = null;
                        if (propMapper.ManualCreate != null)
                        {
                            propValue = propMapper.ManualCreate(jsonValue);
                            prop.SetValue(target, propValue, null);
                        }
                        else
                        {
                            propValue = prop.GetValue(target, null);
                            if (propValue != null)
                            {
                                Successor.Build(propValue, propValue.GetType(), jsonValue);
                                if (propertyType.IsNullable() && !propertyType.IsNullablePrimitive())
                                {
                                    prop.SetValue(target, propValue, null);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
