using System;
using System.Collections.Generic;
using NMeCab;
using NMeCab.Core;

namespace NMeCab
{
    public class MeCabLattice
    {
        public MeCabParam Param;
        public MeCabNode EosNode;
        public MeCabNode BosNode;
        public MeCabNode[] EndNodeList;
        public MeCabNode[] BeginNodeList;
        public float Z;

        public MeCabNode[] GetBestNodes()
        {
            var stack = new Stack<MeCabNode>();

            for (var node = this.EosNode.Prev; node.Prev != null; node = node.Prev)
                stack.Push(node);

            return stack.ToArray();
        }

        public MeCabNode[] GetAllNodes()
        {
            var list = new List<MeCabNode>();

            for (int pos = 0; pos < this.BeginNodeList.Length - 1; pos++)
            {
                for (var node = this.BeginNodeList[pos]; node != null; node = node.BNext)
                {
                    list.Add(node);

                    for (var path = node.LPath; path != null; path = path.LNext)
                    {
                        path.Prob = path.LNode.Alpha
                                    - this.Param.Theta * path.Cost
                                    + path.RNode.Beta - this.Z;
                    }
                }
            }

            return list.ToArray();
        }

        internal IEnumerable<MeCabNode[]> GetNBestResults()
        {
            return new NBestGenerator(this.EosNode);
        }
    }
}
