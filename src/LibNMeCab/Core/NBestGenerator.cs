//  MeCab -- Yet Another Part-of-Speech and Morphological Analyzer
//
//  Copyright(C) 2001-2006 Taku Kudo <taku@chasen.org>
//  Copyright(C) 2004-2006 Nippon Telegraph and Telephone Corporation
using System;
using System.Collections;
using System.Collections.Generic;

namespace NMeCab.Core
{
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

        private readonly PriorityQueue<QueueElement> agenda = new PriorityQueue<QueueElement>();

        public NBestGenerator(TNode eos)
        {
            this.agenda.Push(new QueueElement()
            {
                Node = eos,
                Next = null,
                Fx = 0,
                Gx = 0
            });
        }

        public IEnumerator<TNode[]> GetEnumerator()
        {
            while (this.agenda.Count != 0)
            {
                var top = this.agenda.Pop();
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
                    this.agenda.Push(
                        new QueueElement()
                        {
                            Node = path.LNode,
                            Gx = path.Cost + top.Gx,
                            Fx = path.LNode.Cost + path.Cost + top.Gx,
                            Next = top
                        });
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
