//  MeCab -- Yet Another Part-of-Speech and Morphological Analyzer
//
//  Copyright(C) 2001-2006 Taku Kudo <taku@chasen.org>
//  Copyright(C) 2004-2006 Nippon Telegraph and Telephone Corporation
using System;
using System.Collections.Generic;
using System.Text;

namespace NMeCab.Core
{
    public class NBestGenerator
    {
        private class QueueElement : IComparable<QueueElement>
        {
            public MeCabNode Node { get; set; }
            public QueueElement Next { get; set; }
            public long Fx { get; set; }
            public long Gx { get; set; }

            public int CompareTo(QueueElement other)
            {
                return this.Fx.CompareTo(other.Fx);
            }

            public override string ToString()
            {
                return this.Node.ToString();
            }
        }

        private PriorityQueue<QueueElement> agenda = new PriorityQueue<QueueElement>();

        public void Set(MeCabNode node)
        {
            for (; node.Next != null; node = node.Next) { } // seek to EOS;
            this.agenda.Clear();
            QueueElement eos = new QueueElement()
            {
                Node = node,
                Next = null,
                Fx = 0,
                Gx = 0
            };
            this.agenda.Push(eos);
        }

        public MeCabNode Next()
        {
            while (this.agenda.Count != 0)
            {
                QueueElement top = this.agenda.Pop();
                MeCabNode rNode = top.Node;

                if (rNode.Stat == MeCabNodeStat.Bos)
                {
                    for (QueueElement n = top; n.Next != null; n = n.Next)
                    {
                        n.Node.Next = n.Next.Node; // change next & prev
                        n.Next.Node.Prev = n.Node;
                    }
                    return rNode;
                }

                for (MeCabPath path = rNode.LPath; path != null; path = path.LNext)
                {
                    QueueElement n = new QueueElement()
                    {
                        Node = path.LNode,
                        Gx = path.Cost + top.Gx,
                        Fx = path.LNode.Cost + path.Cost + top.Gx,
                        Next = top
                    };
                    this.agenda.Push(n);
                }
            }
            return null;
        }

        public IEnumerable<MeCabNode> GetEnumerator()
        {
            for (MeCabNode rNode = this.Next(); rNode != null; rNode = this.Next())
                yield return rNode;
        }
    }
}
