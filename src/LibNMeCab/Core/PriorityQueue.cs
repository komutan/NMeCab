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
            public T Value;

            public readonly LinkedList<Node> Childs = new LinkedList<Node>();

            public Node(T value)
            {
                this.Value = value;
            }
        }

        private Node rootNode;

        public int Count { get; private set; }

        public T First { get { return this.rootNode.Value; } }

        public void Clear()
        {
            this.rootNode = null;
            this.Count = 0;
        }

        public void Push(T item)
        {
            this.rootNode = this.Merge(this.rootNode, new Node(item));
            this.Count++;
        }

        public T Pop()
        {
            T ret = this.First;
            this.RemoveFirst();
            return ret;
        }

        public void RemoveFirst()
        {
            this.rootNode = this.Unify(this.rootNode.Childs);
            this.Count--;
        }

        private Node Merge(Node l, Node r)
        {
            if (l == null) return r;
            if (r == null) return l;

            if (l.Value.CompareTo(r.Value) > 0)
            {
                r.Childs.AddFirst(l);
                return r;
            }
            else
            {
                l.Childs.AddLast(r);
                return l;
            }
        }

        private Node Unify(LinkedList<Node> nodes)
        {
            if (nodes == null || nodes.Count == 0) return null;

            Node x = nodes.First.Value;
            nodes.RemoveFirst();
            if (nodes.Count == 0) return x;

            Node y = nodes.First.Value;
            nodes.RemoveFirst();

            return this.Merge(this.Merge(x, y), this.Unify(nodes));
        }
    }
}
