//  MeCab -- Yet Another Part-of-Speech and Morphological Analyzer
//
//  Copyright(C) 2001-2006 Taku Kudo <taku@chasen.org>
//  Copyright(C) 2004-2006 Nippon Telegraph and Telephone Corporation
using System;
using System.Runtime.CompilerServices;

namespace NMeCab.Core
{
    public class Viterbi<TNode> : IDisposable
        where TNode : MeCabNodeBase<TNode>
    {
        #region Field/Property

        private readonly Tokenizer<TNode> tokenizer = new Tokenizer<TNode>();
        private readonly Connector<TNode> connector = new Connector<TNode>();

        #endregion

        #region Open/Clear

        public void Open(string dicDir, string[] userDics)
        {
            this.tokenizer.Open(dicDir, userDics);
            this.connector.Open(dicDir);
        }

        #endregion

        #region Analyze

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Analyze(char* str, int len, MeCabLattice<TNode> lattice)
        {
            switch (lattice.Param.LatticeLevel)
            {
                case MeCabLatticeLevel.Zero:
                    this.DoViterbi(str, len, lattice, false);
                    break;
                case MeCabLatticeLevel.One:
                    this.DoViterbi(str, len, lattice, true);
                    break;
                case MeCabLatticeLevel.Two:
                    this.DoViterbi(str, len, lattice, true);
                    this.ForwardBackward(lattice);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(lattice.Param.LatticeLevel));
            }

            this.BuildBestLattice(lattice);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe void DoViterbi(char* str, int len, MeCabLattice<TNode> lattice, bool withAllPath)
        {
            var enc = this.tokenizer.Encoding;
            int bytesLen = enc.GetByteCount(str, len);
            byte* bytesBegin = stackalloc byte[bytesLen];
            if (len > 0) enc.GetBytes(str, len, bytesBegin, bytesLen);
            byte* bytesEnd = bytesBegin + bytesLen;
            char* begin = str;
            char* end = str + len;

            for (int pos = 0; pos < len; pos++)
            {
                if (lattice.EndNodeList[pos] != null)
                {
                    var rNode = tokenizer.Lookup(begin, end, bytesBegin, bytesEnd, lattice);
                    lattice.BeginNodeList[pos] = rNode;
                    this.Connect(pos, rNode, lattice.EndNodeList, withAllPath);
                }

                bytesBegin += enc.GetByteCount(begin, 1);
                begin++;
            }

            for (int pos = len; pos >= 0; pos--)
            {
                if (lattice.EndNodeList[pos] != null)
                {
                    this.Connect(pos, lattice.EosNode, lattice.EndNodeList, withAllPath);
                    break;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Connect(int pos, TNode rNode, TNode[] endNodeList, bool withAllPath)
        {
            for (; rNode != null; rNode = rNode.BNext)
            {
                long bestCost = int.MaxValue; // 2147483647
                TNode bestNode = null;

                for (var lNode = endNodeList[pos]; lNode != null; lNode = lNode.ENext)
                {
                    int localCost = this.connector.Cost(lNode, rNode);
                    long cost = lNode.Cost + localCost;

                    if (cost < bestCost)
                    {
                        bestNode = lNode;
                        bestCost = cost;
                    }

                    if (withAllPath)
                    {
                        var path = new MeCabPath<TNode>()
                        {
                            Cost = localCost,
                            RNode = rNode,
                            LNode = lNode,
                            LNext = rNode.LPath,
                            RNext = lNode.RPath
                        };

                        rNode.LPath = path;
                        lNode.RPath = path;
                    }
                }

                rNode.Prev = bestNode ?? throw new ArgumentException("too long sentence.");
                rNode.Cost = bestCost;

                rNode.BPos = pos;
                rNode.EPos = pos + rNode.RLength;
                if (rNode.RLength != 0)
                {
                    rNode.ENext = endNodeList[rNode.EPos];
                    endNodeList[rNode.EPos] = rNode;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe void ForwardBackward(MeCabLattice<TNode> lattice)
        {
            for (int pos = 0; pos < lattice.BeginNodeList.Length; pos++)
                for (var node = lattice.BeginNodeList[pos]; node != null; node = node.BNext)
                    CalcAlpha(node, lattice.Param.Theta);

            for (int pos = lattice.EndNodeList.Length - 1; pos >= 0; pos--)
                for (var node = lattice.EndNodeList[pos]; node != null; node = node.ENext)
                    CalcBeta(node, lattice.Param.Theta);

            lattice.Z = lattice.EosNode.Alpha; // alpha of EOS

            for (int pos = 0; pos < lattice.BeginNodeList.Length; pos++)
                for (var node = lattice.BeginNodeList[pos]; node != null; node = node.BNext)
                    node.Prob = (float)Math.Exp(node.Alpha + node.Beta - lattice.Z);

            void CalcAlpha(TNode n, double beta)
            {
                n.Alpha = 0f;
                for (var path = n.LPath; path != null; path = path.LNext)
                    n.Alpha = (float)Utils.LogSumExp(n.Alpha,
                                                     -beta * path.Cost + path.LNode.Alpha,
                                                     path == n.LPath);
            }

            void CalcBeta(TNode n, double beta)
            {
                n.Beta = 0f;
                for (var path = n.RPath; path != null; path = path.RNext)
                    n.Beta = (float)Utils.LogSumExp(n.Beta,
                                                    -beta * path.Cost + path.RNode.Beta,
                                                    path == n.RPath);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void BuildBestLattice(MeCabLattice<TNode> lattice)
        {
            var current = lattice.EosNode;
            var prev = current.Prev;

            prev.Next = current;

            current = prev;
            prev = current.Prev;

            while (prev != null)
            {
                current.IsBest = true;
                lattice.BestResultStack.Push(current);
                prev.Next = current;

                current = prev;
                prev = current.Prev;
            }

            lattice.BosNode.Next = current;
        }

        #endregion

        #region Dispose

        private bool disposed;

        /// <summary>
        /// 使用中のリソースを開放する
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;

            if (disposing)
            {
                this.tokenizer.Dispose(); // Nullチェック不要
                this.connector.Dispose(); // Nullチェック不要
            }

            this.disposed = true;
        }

        ~Viterbi()
        {
            this.Dispose(false);
        }

        #endregion
    }
}
