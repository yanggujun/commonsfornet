using Commons.Collections;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Commons.Collections
{
    [TestFixture]
    public class TreeSetLongTest
    {
        [Test]
        public void TestTreeSetRandomOperations()
        {
            for (int j = 0; j < 1000; j++)
            {
                Console.WriteLine(j);
                ITreeSet<int> set = new LlrbTreeSet<int>();

                Random randomValue = new Random((int)(DateTime.Now.Ticks & 0x0000FFFF));
                List<int> list = new List<int>();
                int testNumber = 10000;
                while (set.Count < testNumber)
                {
                    int v = randomValue.Next();
                    if (!set.Contains(v))
                    {
                        set.Add(v);
                        list.Add(v);
                    }
                }

                Assert.AreEqual(testNumber, set.Count);

                Random randomIndex = new Random((int)(DateTime.Now.Ticks & 0x0000FFFF));
                for (int i = 0; i < 1000; i++)
                {
                    int index = randomIndex.Next();
                    index = index < 0 ? (-index) : index;
                    index %= testNumber;
                    int testValue = list[index];
                    Assert.IsTrue(set.Contains(testValue));
                }

                for (int i = 0; i < 100; i++)
                {
                    int min = list.Min();
                    Assert.AreEqual(min, set.Min);
                    set.RemoveMin();
                    Assert.AreEqual(testNumber - i - 1, set.Count);
                    Assert.IsFalse(set.Contains(min));
                    list.Remove(min);
                }

                testNumber -= 100;
                for (int i = 0; i < 100; i++)
                {
                    int max = list.Max();
                    Assert.AreEqual(max, set.Max);
                    set.RemoveMax();
                    Assert.AreEqual(testNumber - i - 1, set.Count);
                    Assert.IsFalse(set.Contains(max));
                    list.Remove(max);
                }

                testNumber -= 100;
                for (int i = 0; i < 1000; i++)
                {
                    int index = randomIndex.Next();
                    index = index < 0 ? (-index) : index;
                    index %= testNumber - i;
                    int toRemove = list[index];
                    Assert.IsTrue(set.Contains(toRemove));
                    Assert.IsTrue(set.Remove(toRemove));
                    Assert.IsFalse(set.Contains(toRemove));
                    Assert.AreEqual(testNumber - i - 1, set.Count);
                    list.Remove(toRemove);
                }
            }
        }

        [Test]
        public void TestTreeSetRemove()
        {
            for (int j = 0; j < 1000; j++)
            {
                Console.WriteLine("round: " + j);
                ITreeSet<int> set = new LlrbTreeSet<int>();

                Random randomValue = new Random((int)(DateTime.Now.Ticks & 0x0000FFFF));
                List<int> list = new List<int>();
                int testNumber = 1000;
                while (set.Count < testNumber)
                {
                    int v = randomValue.Next();
                    if (!set.Contains(v))
                    {
                        set.Add(v);
                        list.Add(v);
                    }
                }

                Assert.AreEqual(testNumber, set.Count);

                Random randomIndex = new Random((int)(DateTime.Now.Ticks & 0x0000FFFF));

                for (int i = 0; i < 100; i++)
                {
                    int index = randomIndex.Next();
                    index = index < 0 ? (-index) : index;
                    index %= testNumber - i;
                    int toRemove = list[index];
                    Assert.IsTrue(set.Contains(toRemove));
                    Assert.IsTrue(set.Remove(toRemove));
                    Assert.IsFalse(set.Contains(toRemove));
                    Assert.AreEqual(testNumber - i - 1, set.Count);
                    list.Remove(toRemove);
                }
            }
        }

        [Test]
        public void TestTreeSetRandomDelete()
        {
            for (int j = 0; j < 1000; j++)
            {
                Console.WriteLine(j);
                ITreeSet<int> set = new LlrbTreeSet<int>();

                Random randomValue = new Random((int)(DateTime.Now.Ticks & 0x0000FFFF));
                List<int> list = new List<int>();
                List<int> writeToFile = new List<int>();
                List<int> deletes = new List<int>();
                int testNumber = 1000;
                while (set.Count < testNumber)
                {
                    int v = randomValue.Next();
                    if (!set.Contains(v))
                    {
                        set.Add(v);
                        list.Add(v);
                        writeToFile.Add(v);
                    }
                }

                Random randomIndex = new Random((int)(DateTime.Now.Ticks & 0x0000FFFF));

                for (int i = 0; i < 100; i++)
                {
                    int index = randomIndex.Next();
                    index = index < 0 ? (-index) : index;
                    index %= testNumber - i;
                    int toRemove = list[index];
                    deletes.Add(toRemove);
                    Assert.IsTrue(set.Contains(toRemove));
                    Assert.IsTrue(set.Remove(toRemove));
                    Assert.IsFalse(set.Contains(toRemove));
                    if (testNumber - i - 1 != set.Count)
                    {
                        WriteResult(writeToFile, deletes);
                    }
                    Assert.AreEqual(testNumber - i - 1, set.Count);
                    list.Remove(toRemove);
                }
            }
        }

        [Test]
        public void TestTreeSetRemoveMin()
        {
            for (int j = 0; j < 5000; j++)
            {
                Random randomValue = new Random((int)DateTime.Now.Ticks & 0x0000FFFFF);
                LlrbTreeSet<int> set = new LlrbTreeSet<int>();
                List<int> list = new List<int>();
                int testNumber = 10000;
                while (set.Count < testNumber)
                {
                    int v = randomValue.Next();
                    if (!set.Contains(v))
                    {
                        set.Add(v);
                        list.Add(v);
                    }
                }
                Assert.AreEqual(testNumber, set.Count);
                int removeMin = 1000;
                List<int> remove = new List<int>();
                for (int i = 0; i < removeMin; i++)
                {
                    int min = set.Min;
                    remove.Add(min);
                    set.RemoveMin();
                    if (testNumber - i - 1 != set.Count)
                    {
                        Console.WriteLine("The failed value to remove: " + min);
                        WriteResult(list, remove);
                    }
                    Assert.AreEqual(testNumber - i - 1, set.Count);
                    Assert.IsFalse(set.Contains(min));
                }
            }
        }

        [Test]
        public void TestTreeSetRemoveMax()
        {
            for (int j = 0; j < 5000; j++)
            {
                Random randomValue = new Random((int)DateTime.Now.Ticks & 0x0000FFFFF);
                LlrbTreeSet<int> set = new LlrbTreeSet<int>();
                List<int> list = new List<int>();
                int testNumber = 10000;
                while (set.Count < testNumber)
                {
                    int v = randomValue.Next();
                    if (!set.Contains(v))
                    {
                        set.Add(v);
                        list.Add(v);
                    }
                }
                Assert.AreEqual(testNumber, set.Count);
                int removeMax = 1000;
                List<int> remove = new List<int>();
                for (int i = 0; i < removeMax; i++)
                {
                    int max = set.Max;
                    remove.Add(max);
                    set.RemoveMax();
                    if (testNumber - i - 1 != set.Count)
                    {
                        Console.WriteLine("The failed value to remove: " + max);
                        WriteResult(list, remove);
                    }
                    Assert.AreEqual(testNumber - i - 1, set.Count);
                    Assert.IsFalse(set.Contains(max));
                }
            }
        }

        [Test]
        public void TestTreeSetFixedValue()
        {
            ITreeSet<int> set = new LlrbTreeSet<int>();
            List<int> list = new List<int>();
            using (FileStream fs = new FileStream(@"c:\misc\test.txt", FileMode.Open))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    string value = null;
                    while (null != (value = sr.ReadLine()))
                    {
                        set.Add(int.Parse(value));
                    }
                }
            }
            using (FileStream fs = new FileStream(@"c:\misc\delete.txt", FileMode.Open))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    string value = null;
                    while (null != (value = sr.ReadLine()))
                    {
                        list.Add(int.Parse(value));
                    }
                }
            }

            int total = set.Count;
            for (int i = 0; i < list.Count; i++)
            {
                int remove = list[i];
                set.Remove(remove);
                Assert.AreEqual(total - i - 1, set.Count);
            }
        }

        private static void WriteResult(List<int> writeToFile, List<int> deletes)
        {
            using (FileStream fs = new FileStream(@"c:\misc\test.txt", FileMode.Create))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    foreach (var f in writeToFile)
                    {
                        sw.WriteLine(f);
                    }
                    sw.Close();
                }
            }
            using (FileStream fs = new FileStream(@"c:\misc\delete.txt", FileMode.Create))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    foreach (var f in deletes)
                    {
                        sw.WriteLine(f);
                    }
                    sw.Close();
                }
            }
        }
    }
}
