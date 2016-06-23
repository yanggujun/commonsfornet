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
using System.Threading;

namespace Commons.Collections.Queue
{
    public class MemQueue<T> : IMemQueue<T>
    {
        private readonly BlockingCollection<T> queue = new BlockingCollection<T>(new ConcurrentQueue<T>());
        private int threadNumber;
        private List<Thread> threads;
        private readonly IMessageHandler<T> handler;

        public MemQueue(string name, Action<T> handle, int threadNumber = 1)
            :this(name, new ActionHandler<T>(handle), threadNumber)
        {

        }

        public MemQueue(string name, IMessageHandler<T> handler, int threadNumber = 1)
        {
            QueueName = name;
            this.threadNumber = threadNumber;
            this.handler = handler;
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

        public void Enqueue(T message)
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
            T element;
            while (queue.TryTake(out element, -1))
            {
                try
                {
                    handler.Handle(element);
                }
                catch
                {
                }
            }
        }
    }
}
