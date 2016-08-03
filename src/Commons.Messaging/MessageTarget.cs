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
        public MessageTarget(string address)
        {
            Address = address;
        }

        public string Address
        {
            get;
            private set;
        }

        public string Send(string json, Type messageType)
        {
            using (var http = new HttpClient())
            {
                var fullName = messageType.FullName;
                http.DefaultRequestHeaders.Add(Constants.MessageType, new string[] { fullName });
                var bytes = Encoding.UTF8.GetBytes(json);
                var content = new ByteArrayContent(bytes);
                http.Timeout = new TimeSpan(0, 0, 10);
                var post = http.PostAsync(Address, content);
                post.Wait();
                var response = post.Result;
                response.EnsureSuccessStatusCode();
                var result = response.Content.ReadAsStringAsync();
                result.Wait();
                return result.Result;
            }
        }

        public string Send<T>(T message)
        {
            return Send(JsonMapper.ToJson(message), typeof(T));
        }
    }
}
