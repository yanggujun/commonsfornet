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
using Commons.Utils;

namespace Commons.Messaging
{
    public class TypedMessageRouter : IRouter
    {
        private HashedMap<Type, IDispatcher> routeTable = new HashedMap<Type, IDispatcher>();

        public void AddTarget(Type target, IDispatcher dispatcher)
        {
            if (!routeTable.ContainsKey(target))
            {
                routeTable.Add(target, dispatcher);
            }
        }

        public void AddTarget<T>(IDispatcher dispatcher)
        {
            AddTarget(typeof(T), dispatcher);
        }

        public IDispatcher FindTarget<T>()
        {
            return FindTarget(typeof(T));
        }

        public IDispatcher FindTarget(Type type)
        {
            IDispatcher dispatcher = null;
            if (routeTable.ContainsKey(type))
            {
                dispatcher = routeTable[type];
            }
            else
            {
                var superTypes = type.SuperTypes();
                foreach (var t in superTypes)
                {
                    if (routeTable.ContainsKey(t))
                    {
                        dispatcher = routeTable[t];
                    }
                }
            }

            return dispatcher;
        }

        public IDispatcher[] FindTargets(Type type)
        {
            var dispatchers = new List<IDispatcher>();
            if (routeTable.ContainsKey(type))
            {
                dispatchers.Add(routeTable[type]);
            }

            var superTypes = type.SuperTypes();
            foreach (var t in superTypes)
            {
                if (routeTable.ContainsKey(t))
                {
                    dispatchers.Add(routeTable[t]);
                }
            }

            return dispatchers.ToArray();
        }

        public IDispatcher[] FindTargets<T>()
        {
            return FindTargets(typeof(T));
        }

        public void RemoveTarget<T>()
        {
            RemoveTarget(typeof(T));
        }

        public void RemoveTarget(Type target)
        {
            if (routeTable.ContainsKey(target))
            {
                routeTable.Remove(target);
            }
        }
    }
}
