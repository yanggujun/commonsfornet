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

namespace Commons.Utils
{
    [CLSCompliant(true)]
    public class FnvHash32 : IHashStrategy
    {
        private const uint FNV_PRIME = 16777619;
        private const uint OFFSET_BASIS = 2166136261;

        public long[] Hash(byte[] bytes)
        {
            Guarder.CheckNull(bytes);
            var hash = OFFSET_BASIS;
            foreach (var b in bytes)
            {
                unchecked
                {
                    hash *= FNV_PRIME;
                    hash ^= b;
                }
            }

            return new [] { (long)hash };
        }
    }
}
