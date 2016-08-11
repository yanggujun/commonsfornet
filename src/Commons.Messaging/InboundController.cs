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
using System.Text;
using Commons.Collections.Sequence;
using Commons.Json;
using Commons.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Commons.Messaging
{
    [CLSCompliant(false)]
    public class InboundController : IMessageController<HttpContext>
    {
        private readonly IRouter router;
        private ISequence sequence;
        private readonly ITypeLoader typeLoader;

        public InboundController(IRouter router, ITypeLoader typeLoader, ISequence sequence)
        {
            this.router = router;
            this.sequence = sequence;
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

                object message = null;
                var index = sequence.Next();
                if (requestContext.Request.Method == "POST")
                {
                    var length = requestContext.Request.ContentLength;
                    var buffer = new byte[length.Value];
                    requestContext.Request.Body.Read(buffer, 0, (int)length.Value);
                    var content = Encoding.UTF8.GetString(buffer);
                    message = JsonMapper.To(type, content);
                }
                else
                {
                    message = Activator.CreateInstance(type);
                }

                var target = router.FindTarget(type);
                var inbound = new InboundInfo
                {
                    SequenceNo = index,
                    Content = message,
                };
                var result = target.Dispatch(inbound);
                string response;
                if (result.GetType() == typeof(string))
                {
                    response = result as string;
                }
                else
                {
                    response = JsonMapper.ToJson(result);
                }
                requestContext.Response.WriteAsync(response).Wait();
            }
            else
            {
                throw new InvalidOperationException("The http header does not contain enough information.");
            }
        }
    }
}
