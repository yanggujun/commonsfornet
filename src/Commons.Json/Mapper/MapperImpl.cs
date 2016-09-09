﻿// Copyright CommonsForNET.
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
        private readonly HashedMap<string, string> keyPropMap = new HashedMap<string, string>(new IgnoreCaseStringEquator());
        private readonly HashedMap<string, string> propKeyMap = new HashedMap<string, string>(new IgnoreCaseStringEquator());
        private readonly HashedSet<string> ignoredProps = new HashedSet<string>();
        private bool propMapped;

        public void Map(string key, string prop)
        {
            keyPropMap[key] = prop;
            propKeyMap[prop] = key;
            propMapped = true;
        }

        public void IgnoreProperty(string prop)
        {
            ignoredProps.Add(prop);
        }

        public string GetKey(string prop)
        {
            string key;
            if (!propMapped)
            {
                key = prop;
            }
            else
            {
                if (propKeyMap.ContainsKey(prop))
                {
                    key = propKeyMap[prop];
                }
                else
                {
                    key = prop;
                }
            }
            return key;
        }

        public string GetProp(string key)
        {
            string prop;
            if (!propMapped)
            {
                prop = key;
            }
            else
            {
                if (keyPropMap.ContainsKey(key))
                {
                    prop = keyPropMap[key];
                }
                else
                {
                    prop = key;
                }
            }
            return prop;
        }

        public bool TryGetKey(string prop, out string key)
        {
            return propKeyMap.TryGetValue(prop, out key);
        }

        public bool TryGetProperty(string key, out string prop)
        {
            return keyPropMap.TryGetValue(key, out prop);
        }

        public IReadOnlyStrictSet<string> IgnoredProperties
        {
            get { return ignoredProps; }
        }

        public Func<object> Create {get;set;}

        public Func<JValue, object> ManualCreate { get; set; }

        public Func<object, JValue> Serializer { get; set; }
    }
}
