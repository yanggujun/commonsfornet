// Copyright CommonsForNET.  // Licensed to the Apache Software Foundation (ASF) under one or more
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
    internal class MapEngineFactory : IMapEngineFactory
    {
        public IMapEngine CreateMapEngine(object target, Type type, CollectionBuilder builder, MapperContainer mappers, TypeContainer types, ConfigContainer configuration)
        {
            var jsonBuilder = new JsonBuilder(mappers, types, configuration);
            var launcher = new Launcher(mappers, types);

            var nullBuilder = new NullBuilder(configuration);
            var primitiveBuilder = new PrimitiveBuilder(configuration);
            var arrayBuilder = new ArrayBuilder(configuration, launcher, mappers);
            var enumBuilder = new EnumerableBuilder(configuration, launcher, builder, mappers);
            var dictBuilder = new DictionaryBuilder(configuration, mappers, launcher);
            var objPropBuilder = new ObjectPropertyBuilder(configuration, mappers, types);

            nullBuilder.SetSuccessor(primitiveBuilder);
            primitiveBuilder.SetSuccessor(arrayBuilder);
            arrayBuilder.SetSuccessor(enumBuilder);
            enumBuilder.SetSuccessor(dictBuilder);
            dictBuilder.SetSuccessor(objPropBuilder);
            objPropBuilder.SetSuccessor(nullBuilder);

            return new MapEngine(jsonBuilder, nullBuilder);
        }
    }
}
