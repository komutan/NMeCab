//  MeCab -- Yet Another Part-of-Speech and Morphological Analyzer
//
//  Copyright(C) 2001-2006 Taku Kudo <taku@chasen.org>
//  Copyright(C) 2004-2006 Nippon Telegraph and Telephone Corporation
using NMeCab.Core;
using System;
using System.Collections;
using System.Collections.Generic;

namespace NMeCab
{
    /// <summary>
    /// 確からしい順に形態素列を取得する列挙子を公開します。
    /// </summary>
    /// <typeparam name="TNode"></typeparam>
    public class NBestGenerator<TNode> : IEnumerable<TNode[]>
        where TNode : MeCabNodeBase<TNode>
    {
        private class QueueElement : IComparable<QueueElement>
        {
            public TNode Node { get; set; }
            public QueueElement Next { get; set; }
            public long Fx { get; set; }
            public long Gx { get; set; }

            public int CompareTo(QueueElement other)
            {
                return this.Fx.CompareTo(other.Fx);
            }
        }

        private readonly QueueElement eos;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="eos">末尾の形態素ノード</param>
        public NBestGenerator(TNode eos)
        {
            this.eos = new QueueElement()
            {
                Node = eos,
                Next = null,
                Fx = 0,
                Gx = 0
            };
        }

        /// <summary>
        /// 形態素列の列挙子を返します。
        /// </summary>
        /// <returns>形態素列の列挙子</returns>
        public IEnumerator<TNode[]> GetEnumerator()
        {
            var agenda = new PriorityQueue<QueueElement>();
            agenda.Push(this.eos);

            do
            {
                var top = agenda.Pop();
                var rNode = top.Node;

                if (rNode.Stat == MeCabNodeStat.Bos)
                {
                    var list = new List<TNode>();

                    for (var n = top.Next; n.Next != null; n = n.Next)
                    {
                        list.Add(n.Node);
                    }

                    yield return list.ToArray();
                }

                for (var path = rNode.LPath; path != null; path = path.LNext)
                {
                    agenda.Push(
                        new QueueElement()
                        {
                            Node = path.LNode,
                            Gx = path.Cost + top.Gx,
                            Fx = path.LNode.Cost + path.Cost + top.Gx,
                            Next = top
                        });
                }
            } while (agenda.Count != 0);
        }

        /// <summary>
        /// 形態素列の列挙子を返します。
        /// </summary>
        /// <returns>形態素列の列挙子</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
