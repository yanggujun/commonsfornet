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
using Commons.Collections.Queue;
using Commons.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Commons.Messaging
{
    [CLSCompliant(false)]
    public class HttpFacade<T>
    {
        private MemQueue<T> queue = new MemQueue<T>(typeof(T).GetType().ToString(), x => Console.WriteLine(x));

        public void OnHttpRequest(HttpContext context)
        {
            StringValues strValues;
            if (context.Request.Headers.TryGetValue(Contants.MessageType, out strValues))
            {
                if (strValues.Count > 1)
                {
                    var assemblyName = strValues[0];
                    var typeName = strValues[1];
                    var assembly = Assembly.Load(new AssemblyName(assemblyName));
                    var type = assembly.GetType(typeName);
                    var length = context.Request.Body.Length;
                    var buffer = new byte[length];
                    context.Request.Body.Read(buffer, 0, (int)length);
                    var json = Encoding.UTF8.GetString(buffer);
                    var message = (T)JsonMapper.To(type, json);
                    queue.Enqueue(message);
                }
            }
        }
    }
}
