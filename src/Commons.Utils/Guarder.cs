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
    public static class Guarder
    {
        public static void CheckNull(params object[] objs)
        {
            foreach (var obj in objs)
            {
                if (null == obj)
                {
                    throw new ArgumentNullException("The input argument is null");
                }
            }
        }

        public static void ValidateString(params string[] inputs)
        {
            foreach (var item in inputs)
            {
                if (string.IsNullOrEmpty(item))
                {
                    throw new ArgumentNullException("The input string argument is null");
                }
                if (string.IsNullOrEmpty(item.Trim()))
                {
                    throw new ArgumentNullException("the input string argument is empty");
                }
            }
        }
		
		public static void ValidateNotNull<T>(this T obj, string message)
		{
			Validate(obj, x => x != null, new ArgumentNullException(message));
		}

		public static void Validate<T, E>(this T obj, Predicate<T> predicate, E ex) where E : Exception
		{
			if (!predicate(obj))
			{
				throw ex;
			}
		}
    }
}
