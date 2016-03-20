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
        private readonly MapperImpl mapper;

        public JsonObjectMapper(MapperImpl mapper)
        {
            this.mapper = mapper;
        }

        public IJsonObjectMapper<T> MapProperty(string jsonKey, Expression<Func<T, int>> propertyExp)
        {
            Map(jsonKey, propertyExp.Body);
            return this;
        }

        public IJsonObjectMapper<T> MapProperty(string jsonKey, Expression<Func<T, double>> propertyExp)
        {
            Map(jsonKey, propertyExp.Body);
            return this;
        }

		public IJsonObjectMapper<T> MapProperty(string jsonKey, Expression<Func<T, object>> propertyExp)
		{
            Map(jsonKey, propertyExp.Body);
			return this;
		}

        public IJsonObjectMapper<T> MapProperty(string jsonKey, Expression<Func<T, float>> propertyExp)
        {
            Map(jsonKey, propertyExp.Body);
            return this;
        }

        public IJsonObjectMapper<T> MapProperty(string jsonKey, Expression<Func<T, short>> propertyExp)
        {
            Map(jsonKey, propertyExp.Body);
            return this;
        }

        public IJsonObjectMapper<T> MapProperty(string jsonKey, Expression<Func<T, bool>> propertyExp)
        {
            Map(jsonKey, propertyExp.Body);
            return this;
        }

        public IJsonObjectMapper<T> MapProperty(string jsonKey, Expression<Func<T, long>> propertyExp)
        {
            Map(jsonKey, propertyExp.Body);
            return this;
        }

        public IJsonObjectMapper<T> MapProperty(string jsonKey, Expression<Func<T, byte>> propertyExp)
        {
            Map(jsonKey, propertyExp.Body);
            return this;
        }

        public IJsonObjectMapper<T> MapProperty(string jsonKey, Expression<Func<T, uint>> propertyExp)
        {
            Map(jsonKey, propertyExp.Body);
            return this;
        }

        public IJsonObjectMapper<T> MapProperty(string jsonKey, Expression<Func<T, ulong>> propertyExp)
        {
            Map(jsonKey, propertyExp.Body);
            return this;
        }

        public IJsonObjectMapper<T> MapProperty(string jsonKey, Expression<Func<T, ushort>> propertyExp)
        {
            Map(jsonKey, propertyExp.Body);
            return this;
        }

        public IJsonObjectMapper<T> MapProperty(string jsonKey, Expression<Func<T, decimal>> propertyExp)
        {
            Map(jsonKey, propertyExp.Body);
            return this;
        }

		public IJsonObjectMapper<T> ConstructWith(Func<T> creator)
		{
            mapper.Create = () => { return creator(); };
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

        private void Map(string jsonKey, Expression exp)
        {
            var member = exp as MemberExpression;
            if (member == null)
            {
                throw new ArgumentException(Messages.InvalidProperty);
            }
            var property = member.Member as PropertyInfo;
            if (property == null)
            {
                throw new ArgumentException(Messages.FieldNotProperty);
            }
            mapper.Map(jsonKey, property.Name);
        }
    }
}
