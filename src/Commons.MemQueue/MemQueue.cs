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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using Commons.Json;

namespace Commons.MemQueue
{
    internal class MemQueue<T> : IMemQueue
    {
        private readonly BlockingCollection<HttpListenerContext> queue = new BlockingCollection<HttpListenerContext>(new ConcurrentQueue<HttpListenerContext>());
        private int threadNumber;
        private List<Thread> threads;
        private readonly IMessageInterpreter<HttpListenerContext> msgInterpreter;
        private readonly IMessageHandler<T> handler;

        public MemQueue(string name, IMessageHandler<T> handler, IMessageInterpreter<HttpListenerContext> msgInterpreter, int threadNumber = 1)
        {
            QueueName = name;
            this.threadNumber = threadNumber;
            this.handler = handler;
            this.msgInterpreter = msgInterpreter;
        }

        public bool IsEmpty
        {
            get
            {
                return queue.Count == 0;
            }
        }

        public string QueueName { get; private set; }

        public void Close()
        {
            queue.CompleteAdding();
        }

        public void Enqueue(HttpListenerContext message)
        {
            if (!queue.IsAddingCompleted)
            {
                queue.Add(message);
            }
        }

        public void Start()
        {
            threads = new List<Thread>();
            for (var i = 0; i < threadNumber; i++)
            {
                var thread = new Thread(Handle);
                threads.Add(thread);
            }
            foreach (var t in threads)
            {
                t.Start();
            }
        }

        private void Handle()
        {
            HttpListenerContext element;
            while (queue.TryTake(out element, -1))
            {
                try
                {
                    var msgType = element.Request.Headers.Get("MsgType");
                    var type = Type.GetType(msgType);
                    var json = msgInterpreter.GetRequest(element);
                    var msg = JsonMapper.To(type, json);
                    var response = handler.Handle((T)msg);
                    msgInterpreter.SendResponse(response);
                }
                catch
                {
                }
            }
        }
    }
}
