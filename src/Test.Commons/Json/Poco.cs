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
using Commons.Collections.Set;
namespace Test.Commons.Json
{
	public class Simple
	{
		public string FieldA { get; set; }

        public int FieldB { get; set; }

        public double FieldC { get; set; }

        public bool FieldD { get; set; }
	}

	public class Nested
	{
		public string FieldE { get; set; }

		public Simple Simple { get; set; }

		public int FieldF { get; set; }

		public double FieldG { get; set; }

		public bool FieldH { get; set; }
	}

    public class ArrayNested
    {
        public string FieldI { get; set; }

        public List<Nested> NestedItems { get; set; }
    }

	public class PrimitiveList
	{
		public string FieldJ { get; set; }
		public List<int> FieldK { get; set; } 
	}

    public class SetNested
    {
        public string FieldL { get; set; }
        public HashedSet<int> FieldM { get; set; }
    }

    public class HasDate
    {
        public DateTime Birthday { get; set; }

        public string Name { get; set; }
    }

    public class IListNested
    {
        public string Name { get; set; }
        public IList<int> Numbers { get; set; }
    }

    public struct SimpleStruct
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public double Score { get; set; }
        public DateTime ExamDate { get; set; }
        public bool Pass { get; set; }
    }

    public class PrivateSetter
    {
	    private int age = 23;
        public string Name { get; set; }

	    public int Age
	    {
		    get { return age; }
			private set { age = value; }
	    }
    }

    public class PrivateGetter
    {
        public string Name { get; set; }
        public int Age { private get; set; }

	    public int ActualAge()
	    {
		    return Age;
	    }
    }

    public class NoDefaultConstructor
    {
        public NoDefaultConstructor(string name, int age)
        {
            Name = name;
            Age = age;
        }
        public string Name { get; set; }
        public int Age { get; set; }
    }

    public class ToySet
    {
        public string Name { get; set; }
        public int SetNo { get; set; }
        public double Price { get; set; }
        public int ReleaseYear { get; set; }
        public string Category { get; set; }
        public bool Producing { get; set; }
    }

	public class Photo
	{
		public string Location { get; set; }
		public string Author { get; set; }
		public DateTime Time { get; set; }
		public string Model { get; set; }
	}

    public class Person
    {
        public Person(string nation, string gender)
        {
            this.Nationality = nation;
            this.Gender = gender;
        }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Nationality { get; private set; }
        public string Gender { get; private set; }
    }

	public class Student
	{
		public Person Person { get; set; }
		public Major Major { get; set; }
		public int Grade { get; set; }
		public DateTime ReportDate { get; set; }
	}

	public enum Major
	{
		CS,
		Art,
		Politics,
		Economics,
		Physics,
		Chemistry
	}
}
