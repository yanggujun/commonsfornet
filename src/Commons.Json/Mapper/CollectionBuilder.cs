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
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Commons.Collections.Set;
using Commons.Utils;

namespace Commons.Json.Mapper
{
    internal class CollectionBuilder
    {
		private const string AddMethod = "Add";
		private const string AddLastMethod = "AddLast";
		private const string PushMethod = "Push";
		private const string EnqueueMethod = "Enqueue";
	    private readonly ConcurrentDictionary<Type, Action<IEnumerable, object>> collectionAdders = new ConcurrentDictionary<Type, Action<IEnumerable, object>>();

	    private readonly ConcurrentDictionary<Type, Func<object>> constructors = new ConcurrentDictionary<Type, Func<object>>();

        public void Configure(Type type, Action<IEnumerable, object> builder)
        {
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
            var type = collection.GetType();
			Action<IEnumerable, object> builder;
			if (collectionAdders.ContainsKey(type))
			{
				builder = collectionAdders[type];
			}
			else if (type.IsGenericType())
			{
				var genericType = type.GetGenericTypeDefinition();
				if (genericType == typeof(List<>) || genericType == typeof(HashSet<>) || genericType == typeof(HashedSet<>))
				{
					builder = CreateAddDelegate(type, AddMethod);
					collectionAdders[type] = builder;
				}
				else if (genericType == typeof(Stack<>))
				{
					builder = CreateAddDelegate(type, PushMethod);
					collectionAdders[type] = builder;
				}
				else if (genericType == typeof(Queue<>))
				{
					builder = CreateAddDelegate(type, EnqueueMethod);
					collectionAdders[type] = builder;
				}
				else if (genericType == typeof(LinkedList<>))
				{
					builder = CreateAddDelegate(type, AddLastMethod);
					collectionAdders[type] = builder;
				}
				else
				{
					throw new NotSupportedException();
				}
			}
            else
            {
	            throw new NotSupportedException();
            }
			builder(collection, value);
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
    }
}
