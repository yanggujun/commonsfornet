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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Commons.Collections.Map;
using Commons.Messaging.Cache;

namespace Commons.Messaging
{
    public class TypeLoader : ITypeLoader
    {
        private readonly ICache<string, Assembly> assemblies;
        private readonly ICache<string, Type> types;

        public TypeLoader(ICache<string, Assembly> assemblyCache, ICache<string, Type> typeCache)
        {
            assemblies = assemblyCache;
            types = typeCache;
            var dir = AppContext.BaseDirectory;
            var files = Directory.GetFiles(dir, "*.dll").Concat(Directory.GetFiles(dir, "*.exe"));
            foreach (var f in files)
            {
                var file = new FileInfo(f);
                var fn = file.Name;
                var lastDot = fn.LastIndexOf(".");
                var name = fn.Substring(0, lastDot);
                var assemblyName = new AssemblyName(name);
                try
                {
                    var assembly = Assembly.Load(assemblyName);
                    assemblies.Add(name, assembly);
                }
                catch (Exception)
                {

                }
            }
        }

        public Type Load(string assemblyQualifiedName)
        {
            if (types.Contains(assemblyQualifiedName))
            {
                return types.From(assemblyQualifiedName);
            }
            else
            {
                foreach(var assembly in assemblies)
                {
                    var t = assembly.Value.GetType(assemblyQualifiedName);
                    if (t != null)
                    {
                        types.Add(assemblyQualifiedName, t);
                        return t;
                    }
                }
            }

            return null;
        }
    }
}
