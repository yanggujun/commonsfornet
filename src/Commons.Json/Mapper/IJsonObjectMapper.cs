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
using System.Linq.Expressions;

namespace Commons.Json.Mapper
{
    //TODO: Dictionary key map
    public interface IJsonObjectMapper<T> : IPropertyMapper<T>, IJsonKeyMapper<T>, INotMapper<T>, IFormatMapper
    {
        IJsonObjectMapper<T> ConstructWith(Func<T> creator);
        IJsonObjectMapper<T> ConstructWith(Func<JValue, T> creator);
        //IJsonObjectMapper<T> MapWith(IObjectConverter<T> converter);
        //IJsonObjectMapper<T> MapWith(Func<JValue, T> converter);
    }

    public interface IPropertyMapper<T>
    {
        IJsonKeyMapper<T> MapProperty(Expression<Func<T, object>> propertyExp);
        IJsonKeyMapper<T> MapProperty(Expression<Func<T, int>> propertyExp);
        IJsonKeyMapper<T> MapProperty(Expression<Func<T, double>> propertyExp);
        IJsonKeyMapper<T> MapProperty(Expression<Func<T, float>> propertyExp);
        IJsonKeyMapper<T> MapProperty(Expression<Func<T, short>> propertyExp);
        IJsonKeyMapper<T> MapProperty(Expression<Func<T, bool>> propertyExp);
        IJsonKeyMapper<T> MapProperty(Expression<Func<T, long>> propertyExp);
        IJsonKeyMapper<T> MapProperty(Expression<Func<T, byte>> propertyExp);
#pragma warning disable 3001
        IJsonKeyMapper<T> MapProperty(Expression<Func<T, sbyte>> propertyExp);
        IJsonKeyMapper<T> MapProperty(Expression<Func<T, uint>> propertyExp);
        IJsonKeyMapper<T> MapProperty(Expression<Func<T, ulong>> propertyExp);
        IJsonKeyMapper<T> MapProperty(Expression<Func<T, ushort>> propertyExp);
#pragma warning restore 3001
        IJsonKeyMapper<T> MapProperty(Expression<Func<T, decimal>> propertyExp);
        IJsonKeyMapper<T> MapProperty(Expression<Func<T, char>> propertyExp);
        IJsonKeyMapper<T> MapProperty(Expression<Func<T, DateTime>> propertyExp);
    }

    public interface IJsonKeyMapper<T>
    {
        IJsonObjectMapper<T> With(string jsonKey);
    }

    public interface INotMapper<T>
    {
        IPropertyMapper<T> Not { get; } 
    }

    public interface IFormatMapper
    {
        IFormatMapper UseDateFormat(string format);
        //IFormatMapper UseCulture(CultureInfo culture);
    }
}
