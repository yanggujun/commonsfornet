using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Commons
{
    public class Order
    {
        public int Id { get; set; }

        public string Name { get; set; }
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
}
