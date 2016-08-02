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
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Commons.Collections.Map;

namespace Commons.Messaging.Cache
{
    public class AssemblyCache : ICache<string, Assembly>
    {
        private readonly HashedMap<string, Assembly> assemblies = new HashedMap<string, Assembly>();
        public void Add(string key, Assembly val)
        {
            assemblies.Add(key, val);
        }

        public bool Contains(string key)
        {
            return assemblies.ContainsKey(key);
        }

        public Assembly From(string key)
        {
            return assemblies[key];
        }

        public IEnumerator<Assembly> GetEnumerator()
        {
            foreach (var kvp in assemblies)
            {
                yield return kvp.Value;
            }
        }

        public void Remove(string key)
        {
            assemblies.Remove(key);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
