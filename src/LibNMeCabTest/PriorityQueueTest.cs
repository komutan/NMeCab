using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace LibNMeCabTest
{
    public class Element : IComparable<Element>
    {
        public int Priority { get; set; }

        public int Order { get; set; }

        public int CompareTo(Element other)
        {
            return this.Priority.CompareTo(other.Priority);
        }

        public override int GetHashCode()
        {
            return this.Priority;
        }

        public override bool Equals(object obj)
        {
            var other = obj as Element;
            return other != null
                && this.Priority == other.Priority
                && this.Order == other.Order;
        }

        public override string ToString()
        {
            return "priority:" + this.Priority + " order:" + this.Order;
        }
    }

    [TestClass]
    public class PriorityQueueTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var queue = new NMeCab.Core.PriorityQueue<Element>();
            var collection = new List<Element>();
            var count = 0;

            for (int i = 0; i < 5; i++)
            {
                //追加 優先度昇順
                for (int j = 0; j < 5; j++)
                {
                    var item = new Element { Priority = j, Order = count };
                    queue.Push(item);
                    collection.Add(item);
                    count++;
                    Assert.AreEqual(queue.Count, count);
                }
            }

            //並べ直し
            collection = (from e in collection
                          orderby e.Priority, e.Order
                          select e).ToList();

            //取り出し
            foreach (var expected in collection)
            {
                var actual = queue.Pop();
                count--;

                Assert.AreEqual(expected, actual);
                Assert.AreEqual(count, queue.Count);
            }

            this.EmptyExceptionTest(queue);
        }

        [TestMethod]
        public void TestMethod2()
        {
            var queue = new NMeCab.Core.PriorityQueue<Element>();
            var collection = new List<Element>();
            var count = 0;

            for (int i = 0; i < 5; i++)
            {
                //追加 優先度降順
                for (int j = 5; j >= 0; j--)
                {
                    var item = new Element { Priority = j, Order = count };
                    queue.Push(item);
                    collection.Add(item);
                    count++;

                    Assert.AreEqual(count, queue.Count);
                }
            }

            //並べ直し
            collection = (from e in collection
                          orderby e.Priority, e.Order
                          select e).ToList();

            //取り出し
            foreach (var expected in collection)
            {
                var actual = queue.Pop();
                count--;

                Assert.AreEqual(expected, actual);
                Assert.AreEqual(count, queue.Count);
            }

            this.EmptyExceptionTest(queue);
        }

        [TestMethod]
        public void TestMethod3()
        {
            var queue = new NMeCab.Core.PriorityQueue<Element>();
            var collection = new List<Element>();
            var order = 0;
            var count = 0;
            var rnd = new Random();

            //追加と取り出しを一定数繰り返す
            for (int i = 0; i < 1000; i++)
            {
                //ランダム優先度でランダム個追加
                int repeat = rnd.Next(1, 10);
                for (int j = 0; j < repeat; j++)
                {
                    var item = new Element { Priority = rnd.Next(10), Order = order };
                    collection.Add(item);
                    queue.Push(item);
                    order++;
                    count++;

                    Assert.AreEqual(count, queue.Count);
                }

                //並べ直し
                collection = (from e in collection
                              orderby e.Priority, e.Order
                              select e).ToList();

                //ランダム個取り出し
                repeat = rnd.Next(1, collection.Count);
                for (int j = 0; j < repeat; j++)
                {
                    var actual = queue.Pop();
                    var expected = collection[j];
                    count--;

                    Assert.AreEqual(expected, actual);
                    Assert.AreEqual(count, queue.Count);
                }
                collection.RemoveRange(0, repeat);
            }

            while (queue.Count > 0) queue.Pop(); //空にする
            this.EmptyExceptionTest(queue);
        }

        [TestMethod]
        public void TestMethod4()
        {
            var queue = new NMeCab.Core.PriorityQueue<string>();

            for (int i = 0; i < 10; i++) queue.Push("abc");

            queue.Clear(); //テスト

            Assert.AreEqual(0, queue.Count);
            this.EmptyExceptionTest(queue);
        }

        [TestMethod]
        public void TestMethod5()
        {
            var queue = new NMeCab.Core.PriorityQueue<int>();

            //10万件挿入
            for (int i = 0; i < 100000; i++) queue.Push(i % 5);

            //取り出し
            while (queue.Count > 0) queue.Pop();
        }

        private void EmptyExceptionTest<T>(NMeCab.Core.PriorityQueue<T> queue)
            where T : IComparable<T>
        {
            try
            {
                queue.Pop();//空なら例外発生
            }
            catch (InvalidOperationException)
            {
                return;
            }
            Assert.Fail("Not Throwed Empty Exception");
        }
    }
}
