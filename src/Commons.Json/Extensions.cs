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

namespace Commons.Json
{
	internal static class Extensions
	{
		public static void Verify<T>(this T x, Predicate<T> check)
        {
            if (!check(x))
            {
                throw new ArgumentException(Messages.InvalidFormat);
            }
        }

		public static bool Is<T>(this object obj) where T : class
		{
			var target = obj as T;
			return target != null;
		}

		public static bool Is<T>(this object obj, out T target) where T : class
		{
			target = obj as T;
			return target != null;
		}

		public static bool IsEmpty(this char ch)
		{
			return ch == JsonTokens.Space || ch == JsonTokens.TabChar || ch == JsonTokens.LineSeparator;
		}

	}
}
