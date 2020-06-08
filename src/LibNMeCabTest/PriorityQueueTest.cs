using NMeCab.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace LibNMeCabTest
{
    public class PriorityQueueTest
    {
        [Fact]
        public void TestMethod1()
        {
            var queue = new PriorityQueue<TestElement>();
            var collection = new List<TestElement>();
            var count = 0;

            for (int i = 0; i < 2; i++)
            {
                //追加 優先度昇順
                for (int j = 0; j < 3; j++)
                {
                    var item = new TestElement { Priority = j, Order = count };
                    queue.Push(item);
                    collection.Add(item);
                    count++;
                    Assert.Equal(queue.Count, count);
                }
            }

            //並べ直し
            collection = collection.OrderBy(e => e.Priority)
                                   .ThenBy(e => e.Order)
                                   .ToList();

            //取り出し
            foreach (var expected in collection)
            {
                var actual = queue.Pop();
                count--;

                Assert.Equal(expected, actual);
                Assert.Equal(count, queue.Count);
            }
        }

        [Fact]
        public void TestMethod2()
        {
            var queue = new PriorityQueue<TestElement>();
            var collection = new List<TestElement>();
            var count = 0;

            for (int i = 0; i < 2; i++)
            {
                //追加 優先度降順
                for (int j = 3; j >= 0; j--)
                {
                    var item = new TestElement { Priority = j, Order = count };
                    queue.Push(item);
                    collection.Add(item);
                    count++;

                    Assert.Equal(count, queue.Count);
                }
            }

            //並べ直し
            collection = collection.OrderBy(e => e.Priority)
                                   .ThenBy(e => e.Order)
                                   .ToList();

            //取り出し
            foreach (var expected in collection)
            {
                var actual = queue.Pop();
                count--;

                Assert.Equal(expected, actual);
                Assert.Equal(count, queue.Count);
            }
        }

        [Fact]
        public void TestMethod3()
        {
            var queue = new PriorityQueue<TestElement>();
            var collection = new List<TestElement>();
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
                    var item = new TestElement { Priority = rnd.Next(10), Order = order };
                    collection.Add(item);
                    queue.Push(item);
                    order++;
                    count++;

                    Assert.Equal(count, queue.Count);
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

                    Assert.Equal(expected, actual);
                    Assert.Equal(count, queue.Count);
                }

                collection.RemoveRange(0, repeat);
            }
        }
    }

    public class TestElement : IComparable<TestElement>
    {
        public int Priority { get; set; }

        public int Order { get; set; }

        public int CompareTo(TestElement other)
        {
            return this.Priority.CompareTo(other.Priority);
        }

        public override int GetHashCode()
        {
            return this.Priority;
        }

        public override bool Equals(object obj)
        {
            return obj is TestElement other
                && this.Priority == other.Priority
                && this.Order == other.Order;
        }

        public override string ToString()
        {
            return "priority:" + this.Priority + " order:" + this.Order;
        }
    }
}
