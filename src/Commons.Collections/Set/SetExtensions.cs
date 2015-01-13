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
using System.Collections.Generic;

namespace Commons.Collections.Set
{
    [CLSCompliant(true)]
    public static class SetExtensions
    {
        public static IStrictSet<T> Intersect<T>(this IStrictSet<T> first, IStrictSet<T> second)
        {
            throw new NotImplementedException();
        }

        public static IStrictSet<T> Union<T>(this IStrictSet<T> first, IStrictSet<T> second)
        {
            throw new NotImplementedException();
        }

        public static IStrictSet<T> Differ<T>(this IStrictSet<T> first, IStrictSet<T> second)
        {
            throw new NotImplementedException();
        }

        public static IStrictSet<T> Compliment<T>(this IStrictSet<T> subset, IStrictSet<T> universe)
        {
            throw new NotImplementedException();
        }

        public static bool IsSupersetOf<T>(this IStrictSet<T> origin, IStrictSet<T> other)
        {
            throw new NotImplementedException();
        }

        public static bool IsProperSupersetOf<T>(this IStrictSet<T> origin, IStrictSet<T> other)
        {
            throw new NotImplementedException();
        }

        public static ICollection<IStrictSet<T>> PowerSet<T>(this IStrictSet<T> origin)
        {
            throw new NotImplementedException();
        }
    }
}
