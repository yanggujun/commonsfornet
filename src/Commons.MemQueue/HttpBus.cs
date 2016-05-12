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
using System.Net;
using System.Reflection;
using Commons.Collections.Map;
using Commons.Utils;

namespace Commons.MemQueue
{
    public class HttpBus
    {
        private readonly HashedMap<string, IMemQueue> queues = new HashedMap<string, IMemQueue>();
        private readonly HashedMap<string, Type> types = new HashedMap<string, Type>();

        public void Configure<T>(Action<IQueueDescriptor<T>> queueConfig)
        {
            var type = typeof(T);
            var fullName = type.FullName;
            var queueDesc = new QueueDescriptor<T>();
            queueConfig(queueDesc);
            var queue = queueDesc.Instance();
            queues.Add(fullName, queue);
        }

        public void Post(string url, string json)
        {
        }

        public void Post<T>(string url, T obj)
        {

        }

        public void Post<T>(string url, T obj, Action<T> jsonize)
        {

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
                    if (queues.ContainsKey(msgType))
                    {
                        queues[msgType].Enqueue(contex);
                    }
                    else
                    {
                        Type type;
                        if (types.ContainsKey(msgType))
                        {
                            type = types[msgType];
                        }
                        else
                        {
                            type = Type.GetType(msgType);
                            if (type == null)
                            {
                                type = TypeLoader.Load(msgType);
                            }
                        }

                        foreach (var t in type.SuperTypes())
                        {
                            if (queues.ContainsKey(t.FullName))
                            {
                                queues[msgType] = queues[t.FullName];
                                queues[t.FullName].Enqueue(contex);
                                break;
                            }
                        }
                    }
                }
                catch (Exception e)
                {

                }
            }
        }

    }
}
