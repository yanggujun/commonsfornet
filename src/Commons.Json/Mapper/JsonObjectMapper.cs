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
using System.Linq.Expressions;
using System.Reflection;
using Commons.Collections.Map;

namespace Commons.Json.Mapper
{
	internal class JsonObjectMapper<T> : IJsonObjectMapper<T>
	{
        private readonly HashedBimap<string, string> keyPropMap = new HashedBimap<string, string>();

		public IJsonObjectMapper<T> MapProperty(string jsonKey, Expression<Func<T, object>> propertyExp)
		{
            var member = propertyExp.Body as MemberExpression;
            if (member == null)
            {
                throw new ArgumentException(Messages.InvalidProperty);
            }
            var property = member.Member as PropertyInfo;
            if (property == null)
            {
                throw new ArgumentException(Messages.FieldNotProperty);
            }
            keyPropMap[jsonKey] = property.Name;
			return this;
		}

		public IJsonObjectMapper<T> ConstructWith(Func<T> creator)
		{
			return this;
		}

		public IJsonObjectMapper<T> MapCollection(Func<List<T>, IEnumerable<T>> converter)
		{
			return this;
		}

		public IJsonObjectMapper<T> MapWith(IObjectConverter<T> converter)
		{
			return this;
		}

		public IJsonObjectMapper<T> MapWith(Func<JValue, T> converter)
		{
			return this;
		}

        public string GetKey(string propertyName)
        {
            return keyPropMap.KeyOf(propertyName);
        }

        public string GetProperty(string key)
        {
            return keyPropMap.ValueOf(key);
        }

        public Func<T> Create
        {
            get { throw new NotImplementedException(); }
        }

        public Func<List<T>, IEnumerable<T>> CollectionConverter
        {
            get { throw new NotImplementedException(); }
        }

        public IObjectConverter<T> ObjectConverter
        {
            get { throw new NotImplementedException(); }
        }
    }
}
