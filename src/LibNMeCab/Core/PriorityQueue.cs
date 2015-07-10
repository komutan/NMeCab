using System;
using System.Collections.Generic;
using System.Text;

namespace NMeCab.Core
{
    public class PriorityQueue<T>
        where T : IComparable<T>
    {
        private class HeapNode
        {
            public T Value { get; private set; }
            public int ChildsCount { get; private set; }
            public HeapNode FirstChild { get; private set; }
            public HeapNode LastChild { get; private set; }
            public HeapNode Prev { get; private set; }
            public HeapNode Next { get; private set; }

            public void AddFirstChild(HeapNode first)
            {
                this.ChildsCount++;
                if (this.ChildsCount == 1)
                {
                    this.LastChild = first;
                }
                else
                {
                    HeapNode old = this.FirstChild;
                    first.Next = old;
                    old.Prev = first;
                }
                this.FirstChild = first;
            }

            public void AddLastChild(HeapNode last)
            {
                this.ChildsCount++;
                if (this.ChildsCount == 1)
                {
                    this.FirstChild = last;
                }
                else
                {
                    HeapNode old = this.LastChild;
                    last.Prev = old;
                    old.Next = last;
                }
                this.LastChild = last;
            }

            public HeapNode PollFirstChild()
            {
                this.ChildsCount--;
                if (this.ChildsCount == 0)
                {
                    this.LastChild.Prev = null;
                    this.LastChild = null;
                }
                HeapNode first = this.FirstChild;
                this.FirstChild = first.Next;
                first.Next = null;
                return first;
            }

            public HeapNode(T value)
            {
                this.Value = value;
            }
        }

        private HeapNode rootNode;

        public int Count { get; private set; }

        public void Clear()
        {
            this.Count = 0;
            this.rootNode = null;
        }

        public void Push(T item)
        {
            this.Count++;
            this.rootNode = this.MergeNodes(this.rootNode, new HeapNode(item));
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
            this.Count--;
            this.rootNode = this.UnifyChilds(this.rootNode);
        }

        private HeapNode MergeNodes(HeapNode l, HeapNode r)
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

        private HeapNode UnifyChilds(HeapNode node)
        {
            HeapNode[] tmp = new HeapNode[node.ChildsCount / 2]; //必要な要素数が明らかなのでStackではなく配列

            for (int i = 0; i < tmp.Length; i++)
            {
                HeapNode x = node.PollFirstChild();
                HeapNode y = node.PollFirstChild();
                tmp[i] = this.MergeNodes(x, y);
            }

            HeapNode z;
            if (node.ChildsCount == 1) //子要素数が奇数の場合、まだ１つ残っている子要素をここで処理
                z = node.PollFirstChild();
            else
                z = null;

            for (int i = tmp.Length - 1; i >= 0; i--) //逆順ループで配列をStackのように振る舞わせる
            {
                z = this.MergeNodes(tmp[i], z);
            }

            return z;
        }
    }
}
