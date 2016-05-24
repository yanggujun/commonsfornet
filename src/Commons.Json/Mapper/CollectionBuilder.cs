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

using Commons.Collections.Map;
using Commons.Collections.Queue;
using Commons.Collections.Set;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Commons.Json.Mapper
{
    internal class CollectionBuilder
    {
        private ReferenceMap<Type, Action<IEnumerable, object>> builders = 
            new ReferenceMap<Type, Action<IEnumerable, object>>();

        public CollectionBuilder()
        {
            Configure(typeof(IList<>), Add);
            Configure(typeof(ISet<>), Add);
            Configure(typeof(LinkedList<>), AddLast);
            Configure(typeof(Queue<>), Enqueue);
            Configure(typeof(Stack<>), Push);
            Configure(typeof(IStrictSet<>), Add);
            Configure(typeof(List<>), Add);
            Configure(typeof(HashSet<>), Add);
        }

        public void Configure(Type type, Action<IEnumerable, object> builder)
        {
            builders[type] = builder;
        }

        public void Build(IEnumerable collection, object value)
        {
            var filled = false;
            var type = collection.GetType();
            if (builders.ContainsKey(type))
            {
                builders[type](collection, value);
                filled = true;
            }
            else if (type.GetTypeInfo().IsGenericType)
            {
                var genericType = type.GetGenericTypeDefinition();
                if (builders.ContainsKey(genericType))
                {
                    builders[genericType](collection, value);
                    filled = true;
                }
            }

            if (!filled)
            {
                foreach (var interfaceType in type.GetInterfaces())
                {
                    if (interfaceType.GetTypeInfo().IsGenericType)
                    {
                        var genericType = interfaceType.GetGenericTypeDefinition();
                        if (builders.ContainsKey(genericType))
                        {
                            builders[genericType](collection, value);
                            filled = true;
                            break;
                        }
                    }
                }
            }

            if (!filled)
            {
                throw new InvalidCastException(Messages.TypeNotSupported);
            }
        }

        private void Add(IEnumerable list, object obj)
        {
            var type = list.GetType();
            var method = type.GetMethod(Messages.AddMethod);
            method.Invoke(list, new[] { obj });
        }

        private void AddLast(IEnumerable linkedList, object obj)
        {
            var type = linkedList.GetType();
            var method = type.GetMethod(Messages.AddLastMethod, new[] { obj.GetType() });
            method.Invoke(linkedList, new[] { obj });
        }

        private void Enqueue(IEnumerable queue, object obj)
        {
            var type = queue.GetType();
            var method = type.GetMethod(Messages.EnqueueMethod);
            method.Invoke(queue, new[] { obj });
        }

        private void Push(IEnumerable stack, object obj)
        {
            var type = stack.GetType();
            var method = type.GetMethod(Messages.PushMethod);
            method.Invoke(stack, new[] { obj });
        }
    }
}
