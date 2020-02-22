//  MeCab -- Yet Another Part-of-Speech and Morphological Analyzer
//
//  Copyright(C) 2001-2006 Taku Kudo <taku@chasen.org>
//  Copyright(C) 2004-2006 Nippon Telegraph and Telephone Corporation
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using NMeCab.Core;

namespace NMeCab
{
     /// <summary>
     /// 形態素解析中の情報の集合を表します。
     /// </summary>
     /// <typeparam name="TNode">形態素ノードの型</typeparam>
    public class MeCabLattice<TNode>
        where TNode : MeCabNodeBase<TNode>
    {
        private Func<TNode> nodeAllocator;

        private uint seqNum = 0u;

        public MeCabParam Param { get; }

        /// <summary>
        /// 開始位置別の形態素ノード配列
        /// </summary>
        public TNode[] BeginNodeList { get; }

        /// <summary>
        /// 終了位置別の形態素ノード配列
        /// </summary>
        public TNode[] EndNodeList { get; }

        /// <summary>
        /// 開始ノード
        /// </summary>
        public TNode BosNode { get; }

        /// <summary>
        /// 終了ノード
        /// </summary>
        public TNode EosNode { get; }

        public float Z { get; internal set; } = 0.0f;

        public MeCabLattice(Func<TNode> nodeAllocator, MeCabParam param, int length)
        {
            this.nodeAllocator = nodeAllocator;
            this.Param = param;
            this.BeginNodeList = new TNode[length + 1];
            this.EndNodeList = new TNode[length + 1];

            var bosNode = CreateNewNode();
            bosNode.Stat = MeCabNodeStat.Bos;
            this.EndNodeList[length] = BosNode;
            this.BosNode = bosNode;

            var eosNode = this.EosNode;
            eosNode.Stat = MeCabNodeStat.Eos;
            this.BeginNodeList[0] = bosNode;
            this.EosNode = eosNode;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TNode CreateNewNode()
        {
            var newNode = this.nodeAllocator();
            newNode.Id = this.seqNum++;
            return newNode;
        }

        /// <summary>
        ///  ベスト解を取得します。
        /// </summary>
        /// <returns></returns>
        public TNode[] GetBestNodes()
        {
            var stack = new Stack<TNode>();

            for (var node = this.EosNode.Prev; node.Prev != null; node = node.Prev)
            {
                node.Prev.Next = node;
                stack.Push(node);
            }

            return stack.ToArray();
        }

        public TNode[] GetAllNodes()
        {
            var list = new List<TNode>();

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

        public IEnumerable<TNode[]> GetNBestResults()
        {
            return new NBestGenerator<TNode>(this.EosNode);
        }
    }
}
