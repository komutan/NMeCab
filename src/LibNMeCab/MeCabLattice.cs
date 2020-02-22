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

        /// <summary>
        /// 解析パラメータ
        /// </summary>
        public MeCabParam Param { get; }

        /// <summary>
        /// 開始位置をインデックスとした形態素ノード配列
        /// </summary>
        public TNode[] BeginNodeList { get; }

        /// <summary>
        /// 終了位置をインデックスとした形態素ノード配列
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

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="nodeAllocator"></param>
        /// <param name="param"></param>
        /// <param name="length"></param>
        public MeCabLattice(Func<TNode> nodeAllocator, MeCabParam param, int length)
        {
            this.nodeAllocator = nodeAllocator;
            this.Param = param;
            this.BeginNodeList = new TNode[length + 1];
            this.EndNodeList = new TNode[length + 1];

            var bosNode = CreateNewNode();
            bosNode.Stat = MeCabNodeStat.Bos;
            this.EndNodeList[0] = bosNode;
            this.BosNode = bosNode;

            var eosNode = CreateNewNode();
            eosNode.Stat = MeCabNodeStat.Eos;
            this.BeginNodeList[length] = eosNode;
            this.EosNode = eosNode;
        }

        /// <summary>
        /// 新しい形態素ノードを作成します。
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal TNode CreateNewNode()
        {
            var newNode = this.nodeAllocator();
            newNode.Id = this.seqNum++;
            return newNode;
        }

        /// <summary>
        /// ベスト解を取得します。
        /// </summary>
        /// <returns>ベスト解の形態素ノードの配列</returns>
        public TNode[] GetBestNodes()
        {
            var stack = new Stack<TNode>();

            for (var node = this.EosNode.Prev; node.Prev != null; node = node.Prev)
            {
                node.IsBest = true;
                node.Prev.Next = node;
                stack.Push(node);
            }

            return stack.ToArray();
        }

        /// <summary>
        /// Nベスト解を取得します。
        /// </summary>
        /// <returns>形態素の配列を確からしい順に取得する列挙子</returns>
        public IEnumerable<TNode[]> GetNBestResults()
        {
            return new NBestGenerator<TNode>(this.EosNode);
        }

        /// <summary>
        /// すべての形態素を取得します。
        /// </summary>
        /// <returns>すべての形態素ノードの配列</returns>
        public TNode[] GetAllNodes()
        {
            for (var node = this.EosNode.Prev; node.Prev != null; node = node.Prev)
                node.IsBest = true;

            var prev = this.BosNode;
            var list = new List<TNode>();

            for (int pos = 0; pos < this.BeginNodeList.Length - 1; pos++)
            {
                for (var node = this.BeginNodeList[pos]; node != null; node = node.BNext)
                {
                    prev.Next = node;
                    node.Prev = prev;
                    prev = node;
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
    }
}
