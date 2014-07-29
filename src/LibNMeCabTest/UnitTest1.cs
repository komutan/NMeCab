using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LibNMeCabTest
{
    [TestClass]
    public class PriorityQueueTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var target = new NMeCab.Core.PriorityQueue<int>();

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    target.Push(j);
                }
            }

            int count = target.Count;
            int wrk1 = 0;
            for (int k = 0; k < count; k++)
            {
                int wrk2 = target.Pop();
                Assert.IsTrue(wrk1 <= wrk2);
                wrk1 = wrk2;
            }
        }

        [TestMethod]
        public void TestMethod2()
        {
            var target = new NMeCab.Core.PriorityQueue<int>();

            for (int i = 0; i < 10; i++)
            {
                for (int j = 10; j >= 0; j--)
                {
                    target.Push(j);
                }
            }

            int count = target.Count;
            int wrk1 = 0;
            for (int k = 0; k < count; k++)
            {
                int wrk2 = target.Pop();
                Assert.IsTrue(wrk1 <= wrk2);
                wrk1 = wrk2;
            }
        }

        [TestMethod]
        public void TestMethod3()
        {
            var target = new NMeCab.Core.PriorityQueue<int>();
            var rnd = new Random();

            for (int i = 0; i < 1000; i++)
            {
                int count = rnd.Next(10);
                for (int j = 0; j < count; j++)
                {
                    target.Push(rnd.Next(10));
                }

                count = rnd.Next(target.Count);
                int wrk1 = 0;
                for (int k = 0; k < count; k++)
                {
                    int wrk2 = target.Pop();
                    Assert.IsTrue(wrk1 <= wrk2);
                    wrk1 = wrk2;
                }
            }
        }
    }
}
