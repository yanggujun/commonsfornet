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
using System.Reflection;
using System.Text;
using Commons.Json;
using Commons.Messaging.Cache;
using Commons.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Commons.Messaging
{
    [CLSCompliant(false)]
    public class InboundController : IMessageController<HttpContext>
    {
        private readonly IRouter<Type> router;
        private AtomicInt64 sequence;
        private readonly ITypeLoader typeLoader;
        private readonly ICache<long, HttpContext> contexts;

        public InboundController(IRouter<Type> router, ICache<long, HttpContext> contexts, ITypeLoader typeLoader)
        {
            this.router = router;
            this.contexts = contexts;
            sequence = new AtomicInt64();
            this.typeLoader = typeLoader;
        }

        public void Accept(HttpContext requestContext)
        {
            StringValues typeValues;
            var hasAsm = requestContext.Request.Headers.TryGetValue(Constants.MessageType, out typeValues);
            if (hasAsm && typeValues.Count > 0)
            {
                var typeName = typeValues[0];
                var type = typeLoader.Load(typeName);
                if (type == null)
                {
                    throw new InvalidOperationException("The type does not exist");
                }
                var index = sequence.Increment();
                contexts.Add(index, requestContext);
                var length = requestContext.Request.ContentLength;
                var buffer = new byte[length.Value];
                requestContext.Request.Body.Read(buffer, 0, (int)length.Value);
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
                throw new InvalidOperationException("The http header does not contain enough information.");
            }
        }
    }
}
