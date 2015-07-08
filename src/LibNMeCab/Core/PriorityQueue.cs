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
            if (this.Count == 0) throw new InvalidOperationException("Empty");

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

            Node[] tmp = new Node[nodes.Count / 2]; //擬似的Stack

            for (int i = 0; i < tmp.Length; i++)
            {
                Node x = nodes.First.Value;
                nodes.RemoveFirst();
                Node y = nodes.First.Value;
                nodes.RemoveFirst();
                tmp[i] = this.Merge(x, y);
            }

            Node z;
            if (nodes.Count == 1)
                z = nodes.First.Value;
            else
                z = null;

            for (int i = tmp.Length - 1; i >= 0; i--)
                z = this.Merge(tmp[i], z);

            return z;
        }
    }
}
