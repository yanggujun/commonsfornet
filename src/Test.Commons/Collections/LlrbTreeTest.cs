using Commons.Collections;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Commons.Collections
{
    [TestFixture]
    public class LlrbTreeTest
    {
        [Test]
        public void TestTreeSetContructor()
        {
            Random randomValue = new Random((int)(DateTime.Now.Ticks & 0x0000FFFF));
            List<Order> orderList = new List<Order>();
            for (var i = 0; i < 100; i++)
            {
                Order order = new Order();
                order.Id = randomValue.Next();
                order.Name = order.Id + "(*^&^%";
                if (!orderList.Contains(order, new OrderEqualityComparer()))
                {
                    orderList.Add(order);
                }
            }
            TreeSet<Order> orderSet = new TreeSet<Order>(orderList, new OrderComparer());
            foreach (var item in orderList)
            {
                Assert.IsTrue(orderSet.Contains(item));
            }

            TreeSet<int> simpleSet = new TreeSet<int>(Enumerable.Range(0, 100));
            for (var i = 0; i < 100; i++)
            {
                Assert.IsTrue(simpleSet.Contains(i));
            }
            Assert.IsFalse(simpleSet.Contains(101));
        }

        [Test]
        public void TestTreeSetAdd()
        {
            Random randomValue = new Random((int)(DateTime.Now.Ticks & 0x0000FFFF));
            List<Order> orderList = new List<Order>();
            TreeSet<Order> orderSet = new TreeSet<Order>(new OrderComparer());

            for (var i = 0; i < 1000; i++)
            {
                var next = randomValue.Next();
                var orderToSet = new Order();
                orderToSet.Id = next;
                orderToSet.Name = next + "*%%%(*&()*_)(;;";
                if (!orderSet.Contains(orderToSet))
                {
                    var orderToList = new Order();
                    orderToList.Id = next;
                    orderToList.Name = next + "2*%^*((%";
                    orderList.Add(orderToList);
                    orderSet.Add(orderToSet);
                }
            }

            Assert.AreEqual(1000, orderSet.Count);

            foreach (var item in orderList)
            {
                Assert.IsTrue(orderSet.Contains(item));
            }
            orderSet.Clear();
            Assert.AreEqual(0, orderSet.Count);
        }
    }
} 