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
using Commons.Collections.Map;
using Commons.Collections.Set;
using System.Collections;

namespace Commons.Json.Mapper
{
    internal class MapperImpl
    {
        private readonly HashedBimap<string, string> keyPropMap = new HashedBimap<string, string>(new IgnoreCaseStringEquator(), new IgnoreCaseStringEquator());
        private readonly HashedSet<string> ignoredProps = new HashedSet<string>();

        public void Map(string key, string prop)
        {
            keyPropMap.Enforce(key, prop);
        }

        public void IgnoreProperty(string prop)
        {
            ignoredProps.Add(prop);
        }

        public string GetKey(string prop)
        {
            string key;
            if (keyPropMap.ContainsValue(prop))
            {
                key = keyPropMap.KeyOf(prop);
            }
            else
            {
                key = prop;
            }
            return key;
        }

        public string GetProp(string key)
        {
            string prop;
            if (keyPropMap.ContainsKey(key))
            {
                prop = keyPropMap.ValueOf(key);
            }
            else
            {
                prop = key;
            }
            return prop;
        }

        public bool TryGetKey(string prop, out string key)
        {
            return keyPropMap.TryGetKey(prop, out key);
        }

        public bool TryGetProperty(string key, out string property)
        {
            return keyPropMap.TryGetValue(key, out property);
        }

        public IReadOnlyStrictSet<string> IgnoredProperties
        {
            get { return ignoredProps; }
        }

        public Func<object> Create {get;set;}

        public Func<JValue, object> ManualCreate { get; set; }
    }
}
