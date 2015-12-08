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

namespace Commons.Pool
{
	[CLSCompliant(true)]
	public interface ICountable
	{
        /// <summary>
        /// The number of the idle objects.
        /// </summary>
        int IdleCount { get; }

        /// <summary>
        /// The number of the active objects.
        /// </summary>
        int ActiveCount { get; }

        /// <summary>
        /// The maximum number of all the objects.
        /// </summary>
        int Capacity { get; }

        /// <summary>
        /// The initial size of the pool. If the pool grows, its size eventually reaches the <see cref="Capacity"/>;
        /// </summary>
        int InitialSize { get; }
	}
}
