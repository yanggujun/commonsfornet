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
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Commons.Collections.Map;
using Commons.Utils;

namespace Commons.Json.Mapper
{
    internal class JsonObjectMapper<T> : IJsonObjectMapper<T>
    {
        private readonly MapperImpl mapper;
        private readonly ConfigContainer configuration;
        private LambdaExpression lastExp;
        private bool not;

        public JsonObjectMapper(MapperImpl mapper, ConfigContainer configuration)
        {
            this.mapper = mapper;
            this.configuration = configuration;
        }

        public IJsonKeyMapper<T> MapProperty(Expression<Func<T, int>> propertyExp)
        {
            Put(propertyExp);
            return this;
        }

        public IJsonKeyMapper<T> MapProperty(Expression<Func<T, double>> propertyExp)
        {
            Put(propertyExp);
            return this;
        }

        public IJsonKeyMapper<T> MapProperty(Expression<Func<T, object>> propertyExp)
        {
            Put(propertyExp);
            return this;
        }

        public IJsonKeyMapper<T> MapProperty(Expression<Func<T, float>> propertyExp)
        {
            Put(propertyExp);
            return this;
        }

        public IJsonKeyMapper<T> MapProperty(Expression<Func<T, short>> propertyExp)
        {
            Put(propertyExp);
            return this;
        }

        public IJsonKeyMapper<T> MapProperty(Expression<Func<T, bool>> propertyExp)
        {
            Put(propertyExp);
            return this;
        }

        public IJsonKeyMapper<T> MapProperty(Expression<Func<T, long>> propertyExp)
        {
            Put(propertyExp);
            return this;
        }

        public IJsonKeyMapper<T> MapProperty(Expression<Func<T, byte>> propertyExp)
        {
            Put(propertyExp);
            return this;
        }

        public IJsonKeyMapper<T> MapProperty(Expression<Func<T, sbyte>> propertyExp)
        {
            Put(propertyExp);
            return this;
        }

        public IJsonKeyMapper<T> MapProperty(Expression<Func<T, uint>> propertyExp)
        {
            Put(propertyExp);
            return this;
        }

        public IJsonKeyMapper<T> MapProperty(Expression<Func<T, ulong>> propertyExp)
        {
            Put(propertyExp);
            return this;
        }

        public IJsonKeyMapper<T> MapProperty(Expression<Func<T, ushort>> propertyExp)
        {
            Put(propertyExp);
            return this;
        }

        public IJsonKeyMapper<T> MapProperty(Expression<Func<T, decimal>> propertyExp)
        {
            Put(propertyExp);
            return this;
        }

        public IJsonKeyMapper<T> MapProperty(Expression<Func<T, char>> propertyExp)
        {
            Put(propertyExp);
            return this;
        }

        public IJsonKeyMapper<T> MapProperty(Expression<Func<T, DateTime>> propertyExp)
        {
            Put(propertyExp);
            return this;
        }

        public IJsonObjectMapper<T> ConstructWith(Func<T> creator)
        {
            mapper.Create = () => creator();
            return this;
        }

        public IJsonObjectMapper<T> ConstructWith(Func<JValue, T> creator)
        {
            mapper.ManualCreate = x => creator(x);
            return this;
        }

        public IJsonObjectMapper<T> SerializeBy(Func<T, JValue> serializer)
        {
            mapper.Serializer = x =>
            {
                var v = (T)x;
                return serializer(v);
            };
            return this;
        }

        public IJsonObjectMapper<T> With(string jsonKey)
        {
            Guarder.CheckNull(jsonKey, "jsonKey");
            if (lastExp == null)
            {
                throw new InvalidOperationException(Messages.NoPropertyToMap);
            }
            Map(jsonKey, lastExp);
            lastExp = null;
            return this;
        }

        public IPropertyMapper<T> Not
        {
            get
            {
                not = !not;
                return this;
            }
        }

        private void Map(string jsonKey, LambdaExpression exp)
        {
            mapper.Map(jsonKey, GetPropertyName(exp));
        }

        private string GetPropertyName(LambdaExpression exp)
        {
            var member = exp.Body as MemberExpression;
            if (member == null)
            {
                throw new ArgumentException(Messages.InvalidProperty);
            }
            var property = member.Member as PropertyInfo;
            if (property == null)
            {
                throw new ArgumentException(Messages.FieldNotProperty);
            }

            return property.Name;
        }

        private void Put(LambdaExpression exp)
        {
            if (not)
            {
                var propName = GetPropertyName(exp);
                if (!mapper.IgnoredProperties.Contains(propName))
                {
                    mapper.IgnoreProperty(GetPropertyName(exp));
                }
                lastExp = null;
            }
            else
            {
                lastExp = exp;
            }
        }

        public IFormatMapper UseDateFormat(string format)
        {
            configuration.Add(Messages.DateFormat, format);
            return this;
        }

    }
}
