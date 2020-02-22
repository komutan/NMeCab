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

        #region AnalyzeStart

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
                    this.ForwardBackward(len, lattice);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(lattice.Param.LatticeLevel));
            }
        }

        #endregion

        #region Analyze

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe void ForwardBackward(int len, MeCabLattice<TNode> lattice)
        {
            lattice.EndNodeList[0].Alpha = 0f;
            for (int pos = 0; pos <= len; pos++)
                for (var node = lattice.BeginNodeList[pos]; node != null; node = node.BNext)
                    this.CalcAlpha(node, lattice.Param.Theta);

            lattice.BeginNodeList[len].Beta = 0f;
            for (int pos = len; pos >= 0; pos--)
                for (var node = lattice.EndNodeList[pos]; node != null; node = node.ENext)
                    this.CalcBeta(node, lattice.Param.Theta);

            lattice.Z = lattice.BeginNodeList[len].Alpha; // alpha of EOS

            for (int pos = 0; pos <= len; pos++)
                for (var node = lattice.BeginNodeList[pos]; node != null; node = node.BNext)
                    node.Prob = (float)Math.Exp(node.Alpha + node.Beta - lattice.Z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CalcAlpha(TNode n, double beta)
        {
            n.Alpha = 0f;
            for (var path = n.LPath; path != null; path = path.LNext)
            {
                n.Alpha = (float)Utils.LogSumExp(n.Alpha,
                                                 -beta * path.Cost + path.LNode.Alpha,
                                                 path == n.LPath);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CalcBeta(TNode n, double beta)
        {
            n.Beta = 0f;
            for (var path = n.RPath; path != null; path = path.RNext)
            {
                n.Beta = (float)Utils.LogSumExp(n.Beta,
                                                -beta * path.Cost + path.RNode.Beta,
                                                path == n.RPath);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe void DoViterbi(char* str, int len, MeCabLattice<TNode> lattice, bool withAllPath)
        {
            var begin = str;
            var end = str + len;

            for (int pos = 0; pos < len; pos++)
            {
                if (lattice.EndNodeList[pos] != null)
                {
                    var rNode = tokenizer.Lookup(begin, end, lattice);
                    rNode.BPos = pos;
                    rNode.EPos = pos + rNode.RLength;
                    lattice.BeginNodeList[pos] = rNode;
                    this.Connect(pos, rNode, lattice.EndNodeList, withAllPath);
                }

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

        #endregion

        #region Connect

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Connect(int pos, TNode rNode, TNode[] endNodeList, bool withAllPath)
        {
            for (; rNode != null; rNode = rNode.BNext)
            {
                long bestCost = int.MaxValue; // 2147483647
                TNode bestNode = null;

                for (var lNode = endNodeList[pos]; lNode != null; lNode = lNode.ENext)
                {
                    int lCost = this.connector.Cost(lNode, rNode); // local cost
                    long cost = lNode.Cost + lCost;

                    if (cost < bestCost)
                    {
                        bestNode = lNode;
                        bestCost = cost;
                    }

                    if (withAllPath)
                    {
                        var path = new MeCabPath<TNode>()
                        {
                            Cost = lCost,
                            RNode = rNode,
                            LNode = lNode,
                            LNext = rNode.LPath,
                            RNext = lNode.RPath
                        };

                        rNode.LPath = path;
                        lNode.RPath = path;
                    }
                }

                if (bestNode == null) throw new ArgumentException("too long sentence.");

                rNode.Prev = bestNode;
                rNode.Cost = bestCost;
                int x = rNode.RLength + pos;
                rNode.ENext = endNodeList[x];
                endNodeList[x] = rNode;
            }
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
