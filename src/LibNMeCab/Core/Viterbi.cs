//  MeCab -- Yet Another Part-of-Speech and Morphological Analyzer
//
//  Copyright(C) 2001-2006 Taku Kudo <taku@chasen.org>
//  Copyright(C) 2004-2006 Nippon Telegraph and Telephone Corporation
using System;
using System.Collections.Generic;

namespace NMeCab.Core
{
    public class Viterbi : IDisposable
    {
        #region Field/Property

        private readonly Tokenizer tokenizer = new Tokenizer();
        private readonly Connector connector = new Connector();

        #endregion

        #region Open/Clear

        public void Open(string dicDir, string[] userDics)
        {
            tokenizer.Open(dicDir, userDics);
            connector.Open(dicDir);
        }

        #endregion

        #region AnalyzeStart

        public unsafe MeCabLattice Analyze(char* str, int len, MeCabParam param)
        {
            var lattice = new MeCabLattice()
            {
                Param = param,
                BeginNodeList = new MeCabNode[len + 1],
                EndNodeList = new MeCabNode[len + 1],
            };

            switch (param.LatticeLevel)
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
                    throw new ArgumentOutOfRangeException(nameof(param.LatticeLevel));
            }

            return lattice;
        }

        #endregion

        #region Analyze

        private unsafe void ForwardBackward(int len, MeCabLattice work)
        {
            work.EndNodeList[0].Alpha = 0f;
            for (int pos = 0; pos <= len; pos++)
                for (MeCabNode node = work.BeginNodeList[pos]; node != null; node = node.BNext)
                    this.CalcAlpha(node, work.Param.Theta);

            work.BeginNodeList[len].Beta = 0f;
            for (int pos = len; pos >= 0; pos--)
                for (MeCabNode node = work.EndNodeList[pos]; node != null; node = node.ENext)
                    this.CalcBeta(node, work.Param.Theta);

            work.Z = work.BeginNodeList[len].Alpha; // alpha of EOS

            for (int pos = 0; pos <= len; pos++)
                for (MeCabNode node = work.BeginNodeList[pos]; node != null; node = node.BNext)
                    node.Prob = (float)Math.Exp(node.Alpha + node.Beta - work.Z);
        }

        private void CalcAlpha(MeCabNode n, double beta)
        {
            n.Alpha = 0f;
            for (MeCabPath path = n.LPath; path != null; path = path.LNext)
            {
                n.Alpha = (float)Utils.LogSumExp(n.Alpha,
                                                 -beta * path.Cost + path.LNode.Alpha,
                                                 path == n.LPath);
            }
        }

        private void CalcBeta(MeCabNode n, double beta)
        {
            n.Beta = 0f;
            for (MeCabPath path = n.RPath; path != null; path = path.RNext)
            {
                n.Beta = (float)Utils.LogSumExp(n.Beta,
                                                -beta * path.Cost + path.RNode.Beta,
                                                path == n.RPath);
            }
        }

        private unsafe void DoViterbi(char* sentence, int len, MeCabLattice work, bool withAllPath)
        {
            work.BosNode = this.tokenizer.GetBosNode(work.Param);
            work.BosNode.Length = len;

            char* begin = sentence;
            char* end = begin + len;
            work.BosNode.Surface = new string(begin, 0, len);
            work.EndNodeList[0] = work.BosNode;

            for (int pos = 0; pos < len; pos++)
            {
                if (work.EndNodeList[pos] != null)
                {
                    MeCabNode rNode = tokenizer.Lookup(begin + pos, end, work.Param);
                    rNode.BPos = pos;
                    rNode.EPos = pos + rNode.RLength;
                    work.BeginNodeList[pos] = rNode;
                    Connect(pos, rNode, work, withAllPath);
                }
            }

            work.EosNode = tokenizer.GetEosNode(work.Param);
            work.EosNode.Surface = end->ToString();
            work.BeginNodeList[len] = work.EosNode;
            for (int pos = len; pos >= 0; pos--)
            {
                if (work.EndNodeList[pos] != null)
                {
                    Connect(pos, work.EosNode, work, withAllPath);
                    break;
                }
            }
        }

        #endregion

        #region Connect

        private void Connect(int pos, MeCabNode rNode, MeCabLattice work, bool withAllPath)
        {
            for (; rNode != null; rNode = rNode.BNext)
            {
                long bestCost = int.MaxValue; // 2147483647

                MeCabNode bestNode = null;

                for (var lNode = work.EndNodeList[pos]; lNode != null; lNode = lNode.ENext)
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
                        var path = new MeCabPath()
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
                rNode.Next = null;
                rNode.Cost = bestCost;
                int x = rNode.RLength + pos;
                rNode.ENext = work.EndNodeList[x];
                work.EndNodeList[x] = rNode;
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
                this.tokenizer.Dispose(); //Nullチェック不要
                this.connector.Dispose(); //Nullチェック不要
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
