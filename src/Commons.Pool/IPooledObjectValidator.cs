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


namespace Commons.Pool
{
    /// <summary>
    /// Validator used to test if objects are still 'usable', as defined by the client's provided implementations, before acquiring or returning.
    /// </summary>
    /// <remarks>
    /// If a validation fails the object could be invalidated by the pool, by calling <see cref="IObjectPool{T}.Invalidate(T)"/>.
    /// </remarks>
    /// <typeparam name="T">The object type.</typeparam>
    public interface IPooledObjectValidator<in T>
    {
        /// <summary>
        /// Indicates if the pool must validate the object just before returning it to the client. If the validation fails the object must be discarded.
        /// </summary>
        bool ValidateOnAcquire { get; }

        /// <summary>
        /// Indicates if the pool must validate the object just before returning it to the pool. If the validation fails the object must be discarded.
        /// </summary>
        bool ValidateOnReturn { get; }

        /// <summary>
        /// Validates the given instance to check if it is still good to use. This method should never throw exceptions because of regular validation logic.
        /// </summary>
        /// <param name="obj">The instance to validate.</param>
        /// <returns>Return <see langword="true"/> if the object is still valid, <see langword="false"/> otherwise.</returns>
        bool Validate(T obj);
    }
}
