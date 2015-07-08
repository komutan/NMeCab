using System;
using System.Collections.Generic;
using System.Text;

namespace NMeCab.Core
{
    public class PriorityQueue<T>
        where T : IComparable<T>
    {
        private class Node
        {
            public T Value { get; private set; }

            public int ChiledsCount { get; private set; }

            public Node FirstChild { get; private set; }

            public Node LastChild { get; private set; }

            public Node Prev { get; private set; }

            public Node Next { get; private set; }

            public void AddFirstChild(Node first)
            {
                this.ChiledsCount++;
                if (this.ChiledsCount == 1)
                {
                    this.LastChild = first;
                }
                else
                {
                    Node old = this.FirstChild;
                    first.Next = old;
                    old.Prev = first;
                }
                this.FirstChild = first;
            }

            public void AddLastChild(Node last)
            {
                this.ChiledsCount++;
                if (this.ChiledsCount == 1)
                {
                    this.FirstChild = last;
                }
                else
                {
                    Node old = this.LastChild;
                    last.Prev = old;
                    old.Next = last;
                }
                this.LastChild = last;
            }

            public Node PollFirstChild()
            {
                this.ChiledsCount--;
                if (this.ChiledsCount == 0)
                {
                    this.LastChild.Prev = null;
                    this.LastChild = null;
                }
                Node first = this.FirstChild;
                this.FirstChild = first.Next;
                first.Next = null;
                return first;
            }

            public Node(T value)
            {
                this.Value = value;
            }
        }

        private Node rootNode;

        public int Count { get; private set; }

        public void Clear()
        {
            this.Count = 0;
            this.rootNode = null;
        }

        public void Push(T item)
        {
            this.Count++;
            this.rootNode = this.Merge(this.rootNode, new Node(item));
        }

        public T Peek()
        {
            return this.rootNode.Value;
        }

        public T Pop()
        {
            T ret = this.Peek();
            this.RemoveFirst();
            return ret;
        }

        public void RemoveFirst()
        {
            if (this.Count == 0) throw new InvalidOperationException("Empty");

            this.Count--;
            this.rootNode = this.Unify(this.rootNode);
        }

        private Node Merge(Node l, Node r)
        {
            if (l == null) return r;
            if (r == null) return l;

            if (l.Value.CompareTo(r.Value) > 0)
            {
                r.AddFirstChild(l);
                return r;
            }
            else
            {
                l.AddLastChild(r);
                return l;
            }
        }

        private Node Unify(Node node)
        {
            if (node == null || node.ChiledsCount == 0) return null;

            Node[] tmp = new Node[node.ChiledsCount / 2]; //擬似的Stack

            for (int i = 0; i < tmp.Length; i++)
            {
                Node x = node.PollFirstChild();
                Node y = node.PollFirstChild();
                tmp[i] = this.Merge(x, y);
            }

            Node z;
            if (node.ChiledsCount == 1)
                z = node.PollFirstChild();
            else
                z = null;

            for (int i = tmp.Length - 1; i >= 0; i--)
            {
                z = this.Merge(tmp[i], z);
            }

            return z;
        }
    }
}
