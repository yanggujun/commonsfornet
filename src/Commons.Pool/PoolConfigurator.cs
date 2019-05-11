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
    internal class PoolConfigurator<T> : IPoolConfigurator<T> where T : class
    {
        public string Key { get; set; }
        public int InitialSize { get; set; }
        public int MaxSize { get; set; }
        public int AcquiredInvalidLimit { get; set; }
        public IPooledObjectFactory<T> ObjectFactory { get; set; }
        public Func<T> ObjectCreator { get; set; }
        public Action<T> ObjectDestroyer { get; set; }
        public IPooledObjectValidator<T> ObjectValidator { get; set; }

        public void ByFactory(IPooledObjectFactory<T> factory)
        {
            ObjectFactory = factory;
        }

        public void CreateWith(Func<T> creator)
        {
            ObjectCreator = creator;
        }

        public void DestroyWith(Action<T> destroyer)
        {
            ObjectDestroyer = destroyer;
        }

        public void ValidateWith(IPooledObjectValidator<T> validator)
        {
            ObjectValidator = validator;
        }
    }
}
