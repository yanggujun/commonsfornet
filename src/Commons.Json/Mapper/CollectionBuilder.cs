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

using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using Commons.Collections.Map;
using Commons.Collections.Queue;
using Commons.Collections.Set;
using Commons.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Commons.Json.Mapper
{
    internal class CollectionBuilder
    {
        private readonly Dictionary<Type, Action<IEnumerable, object>> builders = 
            new Dictionary<Type, Action<IEnumerable, object>>();

	    private readonly Dictionary<Type, Action<IEnumerable, object>> collectionAdders = new Dictionary<Type, Action<IEnumerable, object>>();

	    private readonly Dictionary<Type, Func<object>> constructors = new Dictionary<Type, Func<object>>();

        public CollectionBuilder()
        {
			// TODO: support non-generic collections
            Configure(typeof(IList<>), Add);
            Configure(typeof(ISet<>), Add);
            Configure(typeof(LinkedList<>), AddLast);
            Configure(typeof(Queue<>), Enqueue);
            Configure(typeof(Stack<>), Push);
            Configure(typeof(IStrictSet<>), Add);
            Configure(typeof(List<>), Add);
            Configure(typeof(HashSet<>), Add);
        }

        public void Configure(Type type, Action<IEnumerable, object> builder)
        {
            builders[type] = builder;
        }

	    public IEnumerable Construct(Type type)
	    {
		    if (!type.IsCollection())
		    {
			    throw new ArgumentException();
		    }

		    if (!constructors.ContainsKey(type))
		    {
			    var newExp = Expression.New(type.GetConstructor(Type.EmptyTypes));
			    constructors[type] = (Func<object>)Expression.Lambda(newExp).Compile();
		    }
			return (IEnumerable)constructors[type]();
	    }

        public void Build(IEnumerable collection, object value)
        {
            var filled = false;
            var type = collection.GetType();
            if (builders.ContainsKey(type))
            {
                builders[type](collection, value);
                filled = true;
            }
            else if (type.IsGenericType())
            {
	            var genericType = type.GetGenericTypeDefinition();
	            if (builders.ContainsKey(genericType))
	            {
		            builders[genericType](collection, value);
		            filled = true;
	            }
            }
            else
            {
	            throw new NotSupportedException();
            }

            if (!filled)
            {
                foreach (var interfaceType in type.GetInterfaces())
                {
                    if (interfaceType.IsGenericType())
                    {
                        var genericType = interfaceType.GetGenericTypeDefinition();
                        if (builders.ContainsKey(genericType))
                        {
                            builders[genericType](collection, value);
                            filled = true;
                            break;
                        }
                    }
                }
            }

            if (!filled)
            {
                throw new InvalidCastException(Messages.TypeNotSupported);
            }
        }

	    private Action<IEnumerable, object> CreateAddDelegate(Type collectionType, string methodName)
	    {
	        var colParam = Expression.Parameter(typeof (IEnumerable), "col");
	        var itemParam = Expression.Parameter(typeof (object), "item");

	        var itemType = collectionType.GetGenericArguments()[0];
		    var method = collectionType.GetMethod(methodName, new[] {itemType});
	        var castItemExp = Expression.Convert(itemParam, itemType);
	        var castColExp = Expression.Convert(colParam, collectionType);

	        var addExp = Expression.Call(castColExp, method, castItemExp);
		    return Expression.Lambda<Action<IEnumerable, object>>(addExp, colParam, itemParam).Compile();
	    }


        private void Add(IEnumerable list, object obj)
        {
	        var type = list.GetType();
	        if (!collectionAdders.ContainsKey(type))
	        {
		        var del = CreateAddDelegate(type, Messages.AddMethod);
		        collectionAdders[type] = del;
	        }

	        collectionAdders[type](list, obj);
        }

        private void AddLast(IEnumerable linkedList, object obj)
        {
	        var type = linkedList.GetType();
	        if (!collectionAdders.ContainsKey(type))
	        {
		        var del = CreateAddDelegate(type, Messages.AddLastMethod);
		        collectionAdders[type] = del;
	        }

	        collectionAdders[type](linkedList, obj);
        }

        private void Enqueue(IEnumerable queue, object obj)
        {
	        var type = queue.GetType();
	        if (!collectionAdders.ContainsKey(type))
	        {
		        var del = CreateAddDelegate(type, Messages.EnqueueMethod);
		        collectionAdders[type] = del;
	        }

	        collectionAdders[type](queue, obj);
        }

        private void Push(IEnumerable stack, object obj)
        {
	        var type = stack.GetType();
	        if (!collectionAdders.ContainsKey(type))
	        {
		        var del = CreateAddDelegate(type, Messages.PushMethod);
		        collectionAdders[type] = del;
	        }

	        collectionAdders[type](stack, obj);
        }
    }
}
