//  MeCab -- Yet Another Part-of-Speech and Morphological Analyzer
//
//  Copyright(C) 2001-2006 Taku Kudo <taku@chasen.org>
//  Copyright(C) 2004-2006 Nippon Telegraph and Telephone Corporation
using MeCab;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace MeCab.Core
{
    public class Viterbi : IDisposable
    {
        #region InnerClass

        private class ThreadData
        {
            public MeCabNode EosNode;
            public MeCabNode BosNode;
            public MeCabNode[] EndNodeList;
            public MeCabNode[] BeginNodeList;
            public float Z;
        }

        #endregion

        #region Field/Property

        private readonly Tokenizer tokenizer = new Tokenizer();
        private readonly Connector connector = new Connector();

        private MeCabLatticeLevel level;
        private float theta;
        private int costFactor;

        public float Theta
        {
            get { return this.theta * this.costFactor; }
            set { this.theta = value / this.costFactor; }
        }

        public unsafe MeCabLatticeLevel LatticeLevel
        {
            get
            {
                return this.level;
            }
            set
            {
                this.level = value;
                this.connect = this.ConnectNomal;
                this.analyze = this.DoViterbi;
                if (value >= MeCabLatticeLevel.One)
                    this.connect = this.ConnectWithAllPath;
                if (value >= MeCabLatticeLevel.Two)
                    this.analyze = this.ForwardBackward;
            }
        }

        public bool Partial { get; set; }

        public bool AllMorphs
        {
            get
            {
                return this.buildLattice == this.BuildAllLattice;
            }
            set
            {
                if (value)
                    this.buildLattice = this.BuildAllLattice;
                else
                    this.buildLattice = this.BuildBestLattice;
            }
        }

        #endregion

        #region Open/Clear

        public void Open(MeCabParam param)
        {
            tokenizer.Open(param);
            connector.Open(param);

            this.costFactor = param.CostFactor;
            this.Theta = param.Theta;
            this.LatticeLevel = param.LatticeLevel;
            this.Partial = param.Partial;
            this.AllMorphs = param.AllMorphs;
        }

#if NeedId
        public void Clear()
        {
            this.tokenizer.Clear();
        }
#endif

        #endregion

        #region AnalyzeStart

        public unsafe MeCabNode Analyze(char* str, int len)
        {
#if NeedId
            this.Clear();
#endif

            ThreadData work = new ThreadData()
            {
                EndNodeList = new MeCabNode[len + 4],
                BeginNodeList = new MeCabNode[len + 4]
            };

            if (this.Partial)
            {
                string newStr = this.InitConstraints(str, len, work);
                fixed (char* pNewStr = newStr)
                {
                    this.analyze(pNewStr, newStr.Length, work);
                    return this.buildLattice(work);
                }
            }

            this.analyze(str, len, work);
            return this.buildLattice(work);
        }

        #endregion

        #region Analyze

        private unsafe delegate void AnalyzeAction(char* str, int len, ThreadData work);

        private AnalyzeAction analyze;

        private unsafe void ForwardBackward(char* sentence, int len, ThreadData work)
        {
            this.DoViterbi(sentence, len, work);

            work.EndNodeList[0].Alpha = 0f;
            for (int pos = 0; pos <= len; pos++)
                for (MeCabNode node = work.BeginNodeList[pos]; node != null; node = node.BNext)
                    this.CalcAlpha(node, this.theta);

            work.BeginNodeList[len].Beta = 0f;
            for (int pos = len; pos >= 0; pos--)
                for (MeCabNode node = work.EndNodeList[pos]; node != null; node = node.ENext)
                    this.CalcBeta(node, this.theta);

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

        private unsafe void DoViterbi(char* sentence, int len, ThreadData work)
        {
            work.BosNode = this.tokenizer.GetBosNode();
            work.BosNode.Length = len;

            char* begin = sentence;
            char* end = begin + len;
            work.BosNode.Surface = new string(begin, 0, len);
            work.EndNodeList[0] = work.BosNode;

            for (int pos = 0; pos < len; pos++)
            {
                if (work.EndNodeList[pos] != null)
                {
                    MeCabNode rNode = tokenizer.Lookup(begin + pos, end);
                    rNode = this.FilterNode(rNode, pos, work);
                    rNode.BPos = pos;
                    rNode.EPos = pos + rNode.RLength;
                    work.BeginNodeList[pos] = rNode;
                    this.connect(pos, rNode, work);
                }
            }

            work.EosNode = tokenizer.GetEosNode();
            work.EosNode.Surface = end->ToString();
            work.BeginNodeList[len] = work.EosNode;
            for (int pos = len; pos >= 0; pos--)
            {
                if (work.EndNodeList[pos] != null)
                {
                    this.connect(pos, work.EosNode, work);
                    break;
                }
            }
        }

        #endregion

        #region Connect

        private delegate void ConnectAction(int pos, MeCabNode rNode, ThreadData work);

        private ConnectAction connect;

        private void ConnectWithAllPath(int pos, MeCabNode rNode, ThreadData work)
        {
            for (; rNode != null; rNode = rNode.BNext)
            {
                long bestCost = int.MaxValue; // 2147483647

                MeCabNode bestNode = null;

                for (MeCabNode lNode = work.EndNodeList[pos]; lNode != null; lNode = lNode.ENext)
                {
                    int lCost = this.connector.Cost(lNode, rNode); // local cost
                    long cost = lNode.Cost + lCost;

                    if (cost < bestCost)
                    {
                        bestNode = lNode;
                        bestCost = cost;
                    }

                    MeCabPath path = new MeCabPath()
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

                if (bestNode == null) throw new ArgumentException("too long sentence.");

                rNode.Prev = bestNode;
                rNode.Next = null;
                rNode.Cost = bestCost;
                int x = rNode.RLength + pos;
                rNode.ENext = work.EndNodeList[x];
                work.EndNodeList[x] = rNode;
            }
        }

        private void ConnectNomal(int pos, MeCabNode rNode, ThreadData work)
        {
            for (; rNode != null; rNode = rNode.BNext)
            {
                long bestCost = int.MaxValue; // 2147483647

                MeCabNode bestNode = null;

                for (MeCabNode lNode = work.EndNodeList[pos]; lNode != null; lNode = lNode.ENext)
                {
                    long cost = lNode.Cost + this.connector.Cost(lNode, rNode);

                    if (cost < bestCost)
                    {
                        bestNode = lNode;
                        bestCost = cost;
                    }
                }

                if (bestNode == null) throw new MeCabException("too long sentence.");

                rNode.Prev = bestNode;
                rNode.Next = null;
                rNode.Cost = bestCost;
                int x = rNode.RLength + pos;
                rNode.ENext = work.EndNodeList[x];
                work.EndNodeList[x] = rNode;
            }
        }

        #endregion

        #region Lattice

        private delegate MeCabNode BuildLatticeFunc(ThreadData work);

        private BuildLatticeFunc buildLattice;

        private MeCabNode BuildAllLattice(ThreadData work)
        {
            if (this.BuildBestLattice(work) == null) return null;

            MeCabNode prev = work.BosNode;

            for (int pos = 0; pos < work.BeginNodeList.Length; pos++)
            {
                for (MeCabNode node = work.BeginNodeList[pos]; node != null; node = node.BNext)
                {
                    prev.Next = node;
                    node.Prev = prev;
                    prev = node;
                    for (MeCabPath path = node.LPath; path != null; path = path.LNext)
                    {
                        path.Prob = (float)(path.LNode.Alpha
                                            - this.theta * path.Cost
                                            + path.RNode.Beta - work.Z);
                    }
                }
            }

            return work.BosNode;
        }

        private MeCabNode BuildBestLattice(ThreadData work)
        {
            MeCabNode node = work.EosNode;
            for (MeCabNode prevNode; node.Prev != null; )
            {
                node.IsBest = true;
                prevNode = node.Prev;
                prevNode.Next = node;
                node = prevNode;
            }
            return work.BosNode;
        }

        #endregion

        #region Partial

        private unsafe string InitConstraints(char* sentence, int sentenceLen, ThreadData work)
        {
            string str = new string(sentence, 0, sentenceLen);
            StringBuilder os = new StringBuilder();
            os.Append(' ');
            int pos = 0;

            foreach (string line in str.Split('\r', '\n'))
            {
                if (line == "") continue;
                if (line == "EOS") break;

                string[] column = line.Split('\t');
                os.Append(column[0]).Append(' ');
                int len = column[0].Length;

                if (column.Length == 2)
                {
                    if (column[1] == "\0") throw new ArgumentException("use \\t as separator");
                    MeCabNode c = this.tokenizer.GetNewNode();
                    c.Surface = column[0];
                    c.Feature = column[1];
                    c.Length = len;
                    c.RLength = len + 1;
                    c.BNext = null;
                    c.WCost = 0;
                    work.BeginNodeList[pos] = c;
                }

                pos += len + 1;
            }

            return os.ToString();
        }

        private MeCabNode FilterNode(MeCabNode node, int pos, ThreadData work)
        {
            if (!this.Partial) return node;

            MeCabNode c = work.BeginNodeList[pos];
            if (c == null) return node;
            bool wild = (c.Feature == "*");

            MeCabNode prev = null;
            MeCabNode result = null;

            for (MeCabNode n = node; n != null; n = n.BNext)
            {
                if (c.Surface == n.Surface
                    && (wild || this.PartialMatch(c.Feature, n.Feature)))
                {
                    if (prev != null)
                    {
                        prev.BNext = n;
                        prev = n;
                    }
                    else
                    {
                        result = n;
                        prev = result;
                    }
                }
            }
            if (result == null) result = c;
            if (prev != null) prev.BNext = null;

            return result;
        }

        private bool PartialMatch(string f1, string f2)
        {
            string[] c1 = f1.Split(',');
            string[] c2 = f2.Split(',');

            int n = Math.Min(c1.Length, c2.Length);

            for (int i = 0; i < n; i++)
                if (c1[i] != "*" && c2[i] != "*" && c1[i] != c2[i]) return false;

            return true;
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
