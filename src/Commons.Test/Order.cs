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

namespace Commons.Test
{
    public class Order
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public override string ToString()
        {
            return Id.ToString();
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class Bill
    {
        public int Id { get; set; }

        public int Count { get; set; }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class OrderEqualityComparer : IEqualityComparer<Order>
    {

        public bool Equals(Order x, Order y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode(Order obj)
        {
            return obj.Id.GetHashCode();
        }
    }

    public class OrderComparer : IComparer<Order>
    {
        public int Compare(Order x, Order y)
        {
            return x.Id - y.Id;
        }
    }

    public class BillComparer : IComparer<Bill>
    {
        public int Compare(Bill x, Bill y)
        {
            return x.Id - y.Id;
        }
    }

    public class BillEqualityComparer : IEqualityComparer<Bill>
    {

        public bool Equals(Bill x, Bill y)
        {
            return x.Id == y.Id && x.Count == y.Count;
        }

        public int GetHashCode(Bill obj)
        {
            return obj.GetHashCode();
        }
    }

    public class Employee : IComparable<Employee>, IEquatable<Employee>
    {
        public int Id { get; set; }
        public int Mark { get; set; }
        public int CompareTo(Employee other)
        {
            return other == null ? 1 : Mark - other.Mark;
        }

        public bool Equals(Employee other)
        {
            return other == null ? false : Mark == other.Mark;
        }
        public override int GetHashCode()
        {
            return Mark.GetHashCode();
        }
    }
}
