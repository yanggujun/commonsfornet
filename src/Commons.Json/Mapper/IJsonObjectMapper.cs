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
using System.Linq.Expressions;
using System.Reflection;

namespace Commons.Json.Mapper
{
    //TODO: Dictionary key map
	[CLSCompliant(true)]
	public interface IJsonObjectMapper<T>
	{
		IJsonObjectMapper<T> MapProperty(string jsonKey, Expression<Func<T, object>> propertyExp);
        IJsonObjectMapper<T> MapProperty(string jsonKey, Expression<Func<T, int>> propertyExp);
        IJsonObjectMapper<T> MapProperty(string jsonKey, Expression<Func<T, double>> propertyExp);
        IJsonObjectMapper<T> MapProperty(string jsonKey, Expression<Func<T, float>> propertyExp);
        IJsonObjectMapper<T> MapProperty(string jsonKey, Expression<Func<T, short>> propertyExp);
        IJsonObjectMapper<T> MapProperty(string jsonKey, Expression<Func<T, bool>> propertyExp);
        IJsonObjectMapper<T> MapProperty(string jsonKey, Expression<Func<T, long>> propertyExp);
        IJsonObjectMapper<T> MapProperty(string jsonKey, Expression<Func<T, byte>> propertyExp);
        IJsonObjectMapper<T> MapProperty(string jsonKey, Expression<Func<T, uint>> propertyExp);
        IJsonObjectMapper<T> MapProperty(string jsonKey, Expression<Func<T, ulong>> propertyExp);
        IJsonObjectMapper<T> MapProperty(string jsonKey, Expression<Func<T, ushort>> propertyExp);
        IJsonObjectMapper<T> MapProperty(string jsonKey, Expression<Func<T, decimal>> propertyExp);
		IJsonObjectMapper<T> ConstructWith(Func<T> creator);
		IJsonObjectMapper<T> MapCollection(Func<List<T>, IEnumerable<T>> converter);
		IJsonObjectMapper<T> MapWith(IObjectConverter<T> converter);
		IJsonObjectMapper<T> MapWith(Func<JValue, T> converter);
	}
}
