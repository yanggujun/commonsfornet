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
using System;
using System.Net;
using System.Reflection;

namespace Commons.MemQueue
{
    public class HttpBus
    {
        private readonly HashedMap<Type, IMemQueue> queues = new HashedMap<Type, IMemQueue>();

        public void AddQueue(Type type, IMemQueue queue)
        {
            queues.Add(type, queue);
        }

        public void Listen(string url)
        {
            foreach (var kvp in queues)
            {
                kvp.Value.Start();
            }

            var listener = new HttpListener();
            listener.Prefixes.Add(url);
            listener.Start();
            while (true)
            {
                try
                {

                    var contex = listener.GetContext();
                    var msgType = contex.Request.Headers.Get("MsgType");
                    var lastDot = msgType.LastIndexOf('.');
                    var assemblyName = msgType.Substring(0, lastDot);
                    var ass = Assembly.Load(assemblyName);
                    var type = ass.GetType(msgType);
                    if (queues.ContainsKey(type))
                    {
                        var queue = queues[type];
                        queue.Enqueue(contex);
                    }
                }
                catch (Exception e)
                {

                }
            }
        }
    }
}
