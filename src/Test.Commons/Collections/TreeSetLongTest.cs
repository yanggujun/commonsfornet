// Copyright CommonsForNET. Author: Gujun Yang. email: gujun.yang@gmail.com
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
using System.IO;
using System.Linq;
using NUnit.Framework;
using Commons.Collections;

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
                int removeMin = 10000;
                List<int> remove = new List<int>();
                for (int i = 0; i < removeMin; i++)
                {
                    int min = set.Min;
                    remove.Add(min);
                    set.RemoveMin();
                    Assert.AreEqual(testNumber - i - 1, set.Count);
                    Assert.IsFalse(set.Contains(min));
                }
            }
        }

        [Test]
        public void TestTreeSetRemoveMax()
        {
            for (int j = 0; j < 1000; j++)
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
                int removeMax = 10000;
                List<int> remove = new List<int>();
                for (int i = 0; i < removeMax; i++)
                {
                    int max = set.Max;
                    remove.Add(max);
                    set.RemoveMax();
                    Assert.AreEqual(testNumber - i - 1, set.Count);
                    Assert.IsFalse(set.Contains(max));
                }
            }
        }

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
            for (int i = 0; i < total; i++)
            {
                if (i == 25)
                {

                }
                set.RemoveMax();
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
