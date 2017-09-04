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

namespace Commons.Reflect
{
    public static class Reflector
    {
        private static readonly object locker = new object();
        private static IReflectContext context;

        private static IReflectContext ContextInstance
        {
            get
            {
                if (context == null)
                {
                    lock (locker)
                    {
                        if (context == null)
                        {
                            context = new ReflectContext();
                        }
                    }
                }

                return context;
            }
        }

        public static IInvoker GetInvoker(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(string.Format("The argument {0} cannot be null.", nameof(type)));
            }

            var invoker = ContextInstance.GetInvoker(type);
            if (invoker == null)
            {
                throw new NotSupportedException("The type cannot be reflected");
            }
            return invoker;
        }

    }
}
