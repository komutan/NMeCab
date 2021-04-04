#pragma warning disable CS1591

using System;
using System.Runtime.CompilerServices;

namespace NMeCab.Core
{
    /// <summary>
    /// 優先度付き先入れ先出しコレクション（実装アルゴリズムはPairing Heap）
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PriorityQueue<T>
        where T : IComparable<T>
    {
        private class HeapNode
        {
            public T Value { get; private set; }
            public int ChildsCount { get; private set; }
            private HeapNode firstChild;
            private HeapNode lastChild;
            private HeapNode next;

            public void AddFirstChild(HeapNode newNode)
            {
                this.ChildsCount++;
                if (this.ChildsCount == 1)
                    this.lastChild = newNode;
                else
                    newNode.next = this.firstChild;
                this.firstChild = newNode;
            }

            public void AddLastChild(HeapNode newNode)
            {
                this.ChildsCount++;
                if (this.ChildsCount == 1)
                    this.firstChild = newNode;
                else
                    this.lastChild.next = newNode;
                this.lastChild = newNode;
            }

            public HeapNode PollFirstChild()
            {
                HeapNode ret = this.firstChild;
                this.ChildsCount--;
                if (this.ChildsCount == 0)
                {
                    this.firstChild = null;
                    this.lastChild = null;
                }
                else
                {
                    this.firstChild = ret.next;
                    ret.next = null;
                }
                return ret;
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

        public T Pop()
        {
            if (this.Count == 0) throw new InvalidOperationException("Empty");

            this.Count--;
            T ret = this.rootNode.Value;
            this.rootNode = this.UnifyChildNodes(this.rootNode);
            return ret;
        }

        [MethodImpl(Utils.DefaultMethodImplOption)]
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

        [MethodImpl(Utils.DefaultMethodImplOption)]
        private HeapNode UnifyChildNodes(HeapNode node)
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
