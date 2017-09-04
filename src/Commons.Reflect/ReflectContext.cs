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

using Commons.Utils;
using System;
using System.Collections.Concurrent;

namespace Commons.Reflect
{
    public class ReflectContext : IReflectContext
    {
        private readonly ConcurrentDictionary<Type, IInvoker> invokers = new ConcurrentDictionary<Type, IInvoker>();

        public IInvoker GetInvoker(Type type)
        {
            IInvoker invoker;
            if (!invokers.TryGetValue(type, out invoker))
            {
                if (type.IsPrimitive() || !type.IsClass())
                {
                    invokers[type] = null;
                }
                else
                {
                    invoker = new Invoker(type);
                    invokers[type] = invoker;
                }
            }
            return invoker;
        }
    }
}
