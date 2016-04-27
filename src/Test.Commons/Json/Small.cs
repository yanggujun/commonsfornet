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
using System.Runtime.Serialization;

namespace Test.Commons.Json
{
	[DataContract]
	public class Message : IEquatable<Message>
	{
		[DataMember(Order = 1)]
		public string message { get; set; }
		[DataMember(Order = 2)]
		public int version { get; set; }
		public override int GetHashCode() { return version; }
		public override bool Equals(object obj) { return base.Equals(obj as Message); }
		public bool Equals(Message other)
		{
			return other != null && other.message == this.message && other.version == this.version;
		}
		public static Message Create(int i)
		{
            var instance = new Message();
			instance.message = "some message " + i;
			instance.version = i;
			return instance;
		}
	}
	[DataContract]
	public class Complex : IEquatable<Complex>
	{
		[DataMember(Order = 1)]
		public decimal x { get; set; }
		[DataMember(Order = 2)]
		public float y { get; set; }
		[DataMember(Order = 3)]
		public long z { get; set; }
		public override int GetHashCode() { return (int)z; }
		public override bool Equals(object obj) { return Equals(obj as Complex); }
		public bool Equals(Complex other)
		{
			return other != null && other.x == this.x && other.y == this.y && other.z == this.z;
		}
		public static Complex Create(int i)
		{
            var instance = new Complex();
			instance.x = i / 1000m;
			instance.y = -i / 1000f;
			instance.z = i;
			return instance;
		}
	}
	[DataContract]
	public class SmallPost : IEquatable<SmallPost>
	{
		private static DateTime NOW = DateTime.UtcNow;

		[DataMember(Order = 2)]
		public Guid ID { get; set; }
		[DataMember(Order = 3)]
		public string title { get; set; }
		[DataMember(Order = 4)]
		public bool active { get; set; }
		[DataMember(Order = 5)]
		public DateTime created { get; set; }
		public override int GetHashCode() { return ID.GetHashCode(); }
		public override bool Equals(object obj) { return Equals(obj as SmallPost); }
		public bool Equals(SmallPost other)
		{
			return other != null && other.ID == this.ID && other.title == this.title
				&& other.active == this.active && other.created == this.created;
		}
        public static SmallPost Create(int i)
		{
            var instance = new SmallPost();
			instance.ID = Guid.NewGuid();
			instance.title = "some title " + i;
			instance.active = i % 2 == 0;
			instance.created = NOW.AddMinutes(i).Date;
			return instance;
		}
	}
}