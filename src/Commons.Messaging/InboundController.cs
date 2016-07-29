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
using System.Reflection;
using System.Text;
using Commons.Json;
using Commons.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Commons.Messaging
{
    [CLSCompliant(false)]
    public class InboundController : IMessageController<HttpContext>
    {
        private IRouter<Type> router;
        private ConcurrentDictionary<long, HttpContext> contextTable;
        private AtomicInt64 sequence;
        public InboundController(IRouter<Type> router)
        {
            this.router = router;
            contextTable = new ConcurrentDictionary<long, HttpContext>();
            sequence = new AtomicInt64();
        }
        public void Accept(HttpContext requestContext)
        {
            StringValues strValues;
            if (requestContext.Request.Headers.TryGetValue(Contants.MessageType, out strValues))
            {
                if (strValues.Count > 1)
                {
                    var assemblyName = strValues[0];
                    var typeName = strValues[1];
                    var assembly = Assembly.Load(new AssemblyName(assemblyName));
                    var type = assembly.GetType(typeName);
                    if (type == null)
                    {
                        throw new InvalidOperationException("The type does not exist");
                    }
                    var index = sequence.Increment();
                    contextTable[index] = requestContext;
                    var length = requestContext.Request.Body.Length;
                    var buffer = new byte[length];
                    requestContext.Request.Body.Read(buffer, 0, (int)length);
                    var content = Encoding.UTF8.GetString(buffer);
                    var message = JsonMapper.To(type, content);
                    var target = router.FindTarget(message);
                    var inbound = new InboundInfo
                    {
                        SequenceNo = index,
                        Content = message,
                    };
                    target.Dispatch(inbound);
                }
                else
                {
                    throw new InvalidOperationException("The http header does not have adequat message type information");
                }
            }
            else
            {
                throw new InvalidOperationException("The http header does not contain message type information.");
            }
        }
    }
}
