// Copyright CommonsForNET 2014.
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

namespace Commons.Collections.Common
{
    /// <summary>
    /// A 32bit murmur hash implementation.
    /// </summary>
    internal class MurmurHash32 : IHashStrategy
    {
        public UInt32[] Hash(Byte[] bytes)
        {
            UInt32[] result = new UInt32[1];
            result[0] = Calc(bytes);
            return result;
        }

        private uint Calc(byte[] bytes)
        {
            var seed = (uint)DateTime.Now.Ticks & 0x0000ffff;
            var length = bytes.Length;
            var blockNumber = length / 4;
            var h1 = seed;
            const uint c1 = 0xcc9e2d51;
            const uint c2 = 0x1b873593;
            for (int i = 0, j = 0; i < blockNumber; i++, j += 4)
            {
                var k1 = BitConverter.ToUInt32(bytes, j);
                k1 *= c1;
                k1 = RotateLeft32(k1, 15);
                k1 *= c2;

                h1 ^= k1;
                h1 = RotateLeft32(h1, 13);
                h1 = h1 * 5 + 0xe6546b64;
            }

            uint k2 = 0;
            var tailIndex = blockNumber * 4;
            var remains = length & 3;

            for (var i = remains; i > 0; i--)
            {
                k2 ^= (uint)(bytes[tailIndex + i - 1] << (i - 1) * 8);
            }
            k2 *= c1;
            k2 = RotateLeft32(k2, 15);
            k2 *= c2;
            h1 ^= k2;

            h1 ^= (uint)length;
            h1 = FinalMix(h1);
            return h1;
        }

        private static uint FinalMix(uint h)
        {
            h ^= h >> 16;
            h *= 0x85ebca66;
            h ^= h >> 13;
            h *= 0xc2b2ae35;
            h ^= h >> 16;
            return h;
        }

        private static uint RotateLeft32(uint x, byte r)
        {
            return (x << r) | (x >> (32 - r));
        }

    }
}
