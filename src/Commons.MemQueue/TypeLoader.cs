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
using System.IO;
using System.Reflection;
using System.Linq;

namespace Commons.MemQueue
{
    internal class TypeLoader
    {
        public static Type Load(string typeName)
        {
            var currentDir = Directory.GetCurrentDirectory();
            var dll = Directory.GetFiles(currentDir, "*.dll").Concat(Directory.GetFiles(currentDir, "*.exe"));
            foreach (var f in dll)
            {
                var name = new AssemblyName();
                name.Name = f.Substring(0, f.LastIndexOf('.'));
                var ass = Assembly.Load(name);
                var type = ass.GetType(typeName);
                if (type != null)
                {
                    return type;
                }
            }

            return null;
        }
    }
}
