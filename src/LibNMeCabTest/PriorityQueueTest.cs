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

            for (int i = 0; i < 2; i++)
            {
                //追加 優先度昇順
                for (int j = 0; j < 3; j++)
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
        }

        [TestMethod]
        public void TestMethod2()
        {
            var queue = new NMeCab.Core.PriorityQueue<Element>();
            var collection = new List<Element>();
            var count = 0;

            for (int i = 0; i < 2; i++)
            {
                //追加 優先度降順
                for (int j = 3; j >= 0; j--)
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
                int repeat = rnd.Next(10);
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
                repeat = rnd.Next(collection.Count);
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
        }
    }
}
