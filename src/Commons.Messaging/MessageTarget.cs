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
using System.Net.Http;
using System.Text;
using Commons.Json;

namespace Commons.Messaging
{
    public class MessageTarget : ITarget
    {
        private readonly string address;
        public MessageTarget(string address)
        {
            this.address = address;
        }
        public string Send(string json, Type messageType)
        {
            var http = new HttpClient();
            var assembly = messageType.AssemblyQualifiedName;
            var typeName = messageType.FullName;
            http.DefaultRequestHeaders.Add(Constants.MessageAssembly, new string[] { assembly });
            http.DefaultRequestHeaders.Add(Constants.MessageType, new string[] { typeName });
            var bytes = Encoding.UTF8.GetBytes(json);
            var content = new ByteArrayContent(bytes);
            var post = http.PostAsync(address, content);
            post.Wait();
            var response = post.Result;
            var result = response.Content.ReadAsByteArrayAsync();
            result.Wait();
            return Encoding.UTF8.GetString(result.Result);
        }

        public string Send<T>(T message)
        {
            return Send(JsonMapper.ToJson(message), typeof(T));
        }
    }
}
