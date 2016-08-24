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
    internal class MapEngine : IMapEngine
    {
        private readonly IJsonBuilder jsonBuilder;
        private readonly IValueBuilder valueBuilder;

        public MapEngine(IJsonBuilder jsonBuilder, IValueBuilder valueBuilder)
        {
            this.jsonBuilder = jsonBuilder;
            this.valueBuilder = valueBuilder;
        }

        public object Map(object raw, Type type, JValue jsonValue)
        {
            return valueBuilder.Build(raw, type, jsonValue);
        }

        public string Map(object target)
        {
            return jsonBuilder.Build(target);
        }
    }
}
