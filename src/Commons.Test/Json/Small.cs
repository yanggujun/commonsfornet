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

namespace Commons.Test.Json
{
	public class Message : IEquatable<Message>
	{
		public string message { get; set; }
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
	public class Complex : IEquatable<Complex>
	{
		public decimal x { get; set; }
		public float y { get; set; }
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
	public class SmallPost : IEquatable<SmallPost>
	{
		private static DateTime NOW = DateTime.UtcNow;

		public Guid ID { get; set; }
		public string title { get; set; }
		public bool active { get; set; }
		public DateTime created { get; set; }
        public int Count { get; set; }
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
            instance.Count = NOW.Millisecond;
			return instance;
		}
	}

    public class CompletePrimitiveObject
    {
        public string F1 { get; set; }
        public int F2 { get; set; }
        public double F3 { get; set; }
        public float F4 { get; set; }
        public short F5 { get; set; }
        public bool F6 { get; set; }
        public long F7 { get; set; }
        public byte F8 { get; set; }
        public sbyte F9 { get; set; }
        public uint F10 { get; set; }
        public ulong F11 { get; set; }
        public ushort F12 { get; set; }
        public decimal F13 { get; set; }
        public char F14 { get; set; }
        public DateTime F15 { get; set; }
        public Simple F16 { get; set; }
        public Guid F17 { get; set; }
    }
}