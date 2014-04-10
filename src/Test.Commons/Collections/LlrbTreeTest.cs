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

        [Test]
        public void TestTreeSetRemove()
        {
            Random r = new Random((int)(DateTime.Now.Ticks & 0x0000FFFF));
            List<Order> orderlist = new List<Order>();
            TreeSet<Order> orderSet = new TreeSet<Order>(new OrderComparer());

            for (var i = 0; i < 1000; i++)
            {
                var next = r.Next();
                var orderToSet = new Order();
                orderToSet.Id = next;
                orderToSet.Name = "(JHOI(*^Y" + next;
                if (!orderSet.Contains(orderToSet))
                {
                    var orderToList = new Order();
                    orderToList.Id = next;
                    orderToList.Name = "(*^TGUHJIO" + next;
                    orderlist.Add(orderToList);
                    orderSet.Add(orderToSet);
                }
            }
            for (var i = 0; i < 500; i++)
            {
                Assert.IsTrue(orderSet.Remove(orderlist[i]));
                Assert.IsFalse(orderSet.Contains(orderlist[i]));
            }
            var notExist = new Order();
            notExist.Id = 1;
            notExist.Name = "not exist";
            Assert.IsFalse(orderSet.Remove(notExist));
        }

        [Test]
        public void TestTreeSetRemoveMax()
        {
            Random r = new Random((int)(DateTime.Now.Ticks & 0x0000FFFF));
            List<Order> orderlist = new List<Order>();
            TreeSet<Order> orderSet = new TreeSet<Order>(new OrderComparer());

            for (var i = 0; i < 200; )
            {
                var next = r.Next();
                var orderToSet = new Order();
                orderToSet.Id = next;
                orderToSet.Name = "(JHOI(*^Y" + next;
                if (!orderSet.Contains(orderToSet))
                {
                    var orderToList = new Order();
                    orderToList.Id = next;
                    orderToList.Name = "(*^TGUHJIO" + next;
                    orderlist.Add(orderToList);
                    orderSet.Add(orderToSet);
                    i++;
                }
            }

            var orderedList = orderlist.OrderByDescending(o => o.Id).ToList();
            for (var i = 0; i < 50; i++)
            {
                var maxOrder = orderedList[i];
                Assert.IsTrue(orderSet.Contains(maxOrder));
                var comparer = new OrderComparer();
                Assert.That(comparer.Compare(maxOrder, orderSet.Max) == 0);
                orderSet.RemoveMax();
                Assert.IsFalse(orderSet.Contains(maxOrder));
                Assert.That(comparer.Compare(maxOrder, orderSet.Max) > 0);
            }

            Assert.AreEqual(150, orderSet.Count);
        }

        [Test]
        public void TestTreeSetRemoveMin()
        {
            Random r = new Random((int)(DateTime.Now.Ticks & 0x0000FFFF));
            List<Order> orderlist = new List<Order>();
            TreeSet<Order> orderSet = new TreeSet<Order>(new OrderComparer());

            for (var i = 0; i < 200; )
            {
                var next = r.Next();
                var orderToSet = new Order();
                orderToSet.Id = next;
                orderToSet.Name = "(JHOI(*^Y" + next;
                if (!orderSet.Contains(orderToSet))
                {
                    var orderToList = new Order();
                    orderToList.Id = next;
                    orderToList.Name = "(*^TGUHJIO" + next;
                    orderlist.Add(orderToList);
                    orderSet.Add(orderToSet);
                    i++;
                }
            }

            var orderedList = orderlist.OrderBy(o => o.Id).ToList();
            for (var i = 0; i < 50; i++)
            {
                var minOrder = orderedList[i];
                Assert.IsTrue(orderSet.Contains(minOrder));
                var comparer = new OrderComparer();
                Assert.That(comparer.Compare(minOrder, orderSet.Min) == 0);
                orderSet.RemoveMin();
                Assert.IsFalse(orderSet.Contains(minOrder));
                Assert.That(comparer.Compare(minOrder, orderSet.Min) < 0);
            }

            Assert.AreEqual(150, orderSet.Count);
        }

        [Test]
        public void TestCopyTo()
        {
            Random r = new Random((int)(DateTime.Now.Ticks & 0x0000FFFF));
            TreeSet<Order> orderSet = new TreeSet<Order>(new OrderComparer());
            for (var i = 0; i < 1000; )
            {
                var o = new Order();
                o.Id = r.Next();
                o.Name = i + " age4356A;";
                if (!orderSet.Contains(o))
                {
                    orderSet.Add(o);
                    i++;
                }
            }
            var orders = new Order[1003];
            orderSet.CopyTo(orders, 3);
            for (var i = 3; i < 1003; i++)
            {
                Assert.IsTrue(orderSet.Contains(orders[i]));
            }
            for (var i = 0; i < 3; i++)
            {
                Assert.IsNull(orders[i]);
            }
        }

        [Test]
        public void TestEnumerator()
        {
            Random r = new Random((int)(DateTime.Now.Ticks & 0x0000FFFF));
            List<Order> orderlist = new List<Order>();
            TreeSet<Order> orderSet = new TreeSet<Order>(new OrderComparer());

            for (var i = 0; i < 200; )
            {
                var next = r.Next();
                var orderToSet = new Order();
                orderToSet.Id = next;
                orderToSet.Name = "(JHOI(*^Y" + next;
                if (!orderSet.Contains(orderToSet))
                {
                    var orderToList = new Order();
                    orderToList.Id = next;
                    orderToList.Name = "(*^TGUHJIO" + next;
                    orderlist.Add(orderToList);
                    orderSet.Add(orderToSet);
                    i++;
                }
            }
            var total = 0;
            foreach (var item in orderSet)
            {
                total++;
                Assert.IsTrue(orderlist.Contains(item, new OrderEqualityComparer()));
            }
            Assert.AreEqual(total, orderSet.Count);
        }
    }
} 