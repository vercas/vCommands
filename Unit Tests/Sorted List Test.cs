#if false
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject1
{
    [TestClass]
    public class SortedListTest
    {
        [TestMethod]
        public void Length_And_Insertion()
        {
            var list = new vCommands.Utilities.SortedList<int, char>();

            Assert.AreEqual(0, list.Count);

            for (int i = 1; i <= 100; i++)
            {
                list[i] = (char)(2 * i);
                Assert.AreEqual(i, list.Count);
            }
        }

        [TestMethod]
        public void Length_And_Random_Insertion()
        {
            var list = new vCommands.Utilities.SortedList<int, char>();
            var rand = new Random();

            Assert.AreEqual(0, list.Count);

            for (int i = 1; i <= 100; i++)
            {
                int r = rand.Next(char.MinValue, char.MaxValue / 2);
                list[r] = (char)(r * 2);
                Assert.AreEqual(i, list.Count);
            }
        }

        [TestMethod]
        public void Length_And_Removal()
        {
            var list = new vCommands.Utilities.SortedList<int, char>();

            for (int i = 1; i <= 100; i++)
            {
                list[i] = (char)(2 * i);
            }

            Assert.AreEqual(100, list.Count);

            for (int i = 1; i <= 50; i++)
            {
                list.Remove(i * 2);
                Assert.AreEqual(100 - i, list.Count);
            }
        }

        [TestMethod]
        public void Containment_1()
        {
            var list = new vCommands.Utilities.SortedList<int, char>();

            for (int i = 1; i <= 100; i++)
            {
                list[i] = (char)(2 * i);
            }

            Assert.AreEqual(100, list.Count);

            for (int i = -100; i <= 100; i++)
                Assert.AreEqual(i > 0, list.ContainsKey(i), "Key {0}", i);
        }

        [TestMethod]
        public void Containment_2()
        {
            var list = new vCommands.Utilities.SortedList<int, char>();

            for (int i = 1; i <= 50; i++)
            {
                list[i * 2] = (char)(4 * i);
            }

            Assert.AreEqual(50, list.Count);

            for (int i = 1; i <= 100; i++)
                Assert.AreEqual(i % 2 == 0, list.ContainsKey(i), "Key {0}", i);
        }
    }
}
#endif