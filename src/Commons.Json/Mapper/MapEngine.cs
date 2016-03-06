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
using System.Collections.Generic;
using System.Reflection;

namespace Commons.Json.Mapper
{
	internal class MapEngine<T> : IMapEngine<T>
	{
		private IJsonObjectMapper<T> mapper;
        private T target;
        private TypeCache typeCache;
		public MapEngine(T target, IJsonObjectMapper<T> mapper, TypeCache typeCache)
		{
			this.mapper = mapper;
            this.target = target;
            this.typeCache = typeCache;
		}

		public T Map(JValue jsonValue)
		{
            InternalMap(target, jsonValue);
            return target;
		}

		public JValue Map(T target)
		{
			throw new NotImplementedException();
		}

        private void InternalMap(T target, JValue jsonValue)
        {
            JObject jsonObj;
            JArray jsonArray;
            if (jsonValue.Is<JObject>(out jsonObj))
            {
                var type = typeof(T);
                var properties = typeCache[type].Properties;
                foreach (var prop in properties)
                {
                    var propertyType = prop.PropertyType;
                    if (propertyType.IsPrimitive || propertyType == typeof(string))
                    {
                        var name = prop.Name;
                        var value = jsonObj[name];
                        var valueType = value.GetType();
                        if (valueType == typeof(JArray) || valueType == typeof(JObject))
                        {
                            throw new InvalidOperationException(Messages.JsonValueTypeNotMatch);
                        }

                        JString str;
                        JInteger integer;
                        JBoolean boolean;
                        JDecimal floating;
                        if (value.Is<JString>(out str))
                        {
                            if (propertyType != typeof(string))
                            {
                                throw new InvalidOperationException(Messages.JsonValueTypeNotMatch);
                            }
                            string v = str;
                            prop.SetValue(target, v);
                        }
                        else if (value.Is<JInteger>(out integer))
                        {
                            if (propertyType != typeof(long) && propertyType != typeof(int) 
                                && prop.PropertyType != typeof(byte))
                            {
                                throw new InvalidOperationException(Messages.JsonValueTypeNotMatch);
                            }
                            if (propertyType == typeof(long))
                            {
                                prop.SetValue(target, integer.AsLong());
                            }
                            else if (propertyType == typeof(int))
                            {
                                prop.SetValue(target, integer.AsInt());
                            }
                            else
                            {
                                prop.SetValue(target, integer.AsByte());
                            }
                        }
                        else if (value.Is<JBoolean>(out boolean))
                        {
                            if (propertyType != typeof(bool))
                            {
                                throw new InvalidOperationException(Messages.JsonValueTypeNotMatch);
                            }
                            bool v = boolean;
                            prop.SetValue(target, v);
                        }
                        else if (value.Is<JDecimal>(out floating))
                        {
                            if (propertyType != typeof(float) && propertyType != typeof(double) && propertyType != typeof(decimal))
                            {
                                throw new InvalidOperationException(Messages.JsonValueTypeNotMatch);
                            }
                            if (propertyType == typeof(float))
                            {
                                prop.SetValue(target, floating.AsFloat());
                            }
                            else if (propertyType == typeof(double))
                            {
                                prop.SetValue(target, floating.AsDouble());
                            }
                            else
                            {
                                prop.SetValue(target, floating.AsDecimal());
                            }
                        }
                        else if (value.Is<JNull>())
                        {
                            prop.SetValue(target, null);
                        }
                        else
                        {
                            throw new InvalidOperationException(Messages.JsonValueTypeNotMatch);
                        }
                    }
                    else
                    {

                    }
                }
            }
        }
	}
}
