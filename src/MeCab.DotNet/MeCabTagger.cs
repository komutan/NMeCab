//  MeCab -- Yet Another Part-of-Speech and Morphological Analyzer
//
//  Copyright(C) 2001-2006 Taku Kudo <taku@chasen.org>
//  Copyright(C) 2004-2006 Nippon Telegraph and Telephone Corporation
using System;
using System.Collections.Generic;
using System.Text;
using MeCab.Core;

namespace MeCab
{
    public class MeCabTagger : IDisposable
    {
        private readonly Viterbi viterbi = new Viterbi();
        private readonly Writer writer = new Writer();

        #region Mode

        /// <summary>
        /// 部分解析モード
        /// </summary>
        public bool Partial
        {
            get { this.ThrowIfDisposed(); return this.viterbi.Partial; }
            set { this.ThrowIfDisposed(); this.viterbi.Partial = value; }
        }

        /// <summary>
        /// ソフト分かち書きの温度パラメータ
        /// </summary>
        public float Theta
        {
            get { this.ThrowIfDisposed(); return this.viterbi.Theta; }
            set { this.ThrowIfDisposed(); this.viterbi.Theta = value; }
        }

        /// <summary>
        /// ラティスレベル(どの程度のラティス情報を解析時に構築するか)
        /// </summary>
        /// <value>
        /// 0: 最適解のみが出力可能なレベル (デフォルト, 高速) 
        /// 1: N-best 解が出力可能なレベル (中速) 
        /// 2: ソフトわかち書きが可能なレベル (低速) 
        /// </value>
        public MeCabLatticeLevel LatticeLevel
        {
            get { this.ThrowIfDisposed(); return this.viterbi.LatticeLevel; }
            set { this.ThrowIfDisposed(); this.viterbi.LatticeLevel = value; }
        }

        /// <summary>
        /// 全出力モード
        /// </summary>
        /// <value>
        /// true: 全出力
        /// false: ベスト解のみ
        /// </value>
        public bool AllMorphs
        {
            get { this.ThrowIfDisposed(); return this.viterbi.AllMorphs; }
            set { this.ThrowIfDisposed(); this.viterbi.AllMorphs = value; }
        }

        /// <summary>
        /// 解析結果のフォーマット
        /// </summary>
        public string OutPutFormatType
        {
            get { this.ThrowIfDisposed(); return this.writer.OutputFormatType; }
            set { this.ThrowIfDisposed(); this.writer.OutputFormatType = value; }
        }

        #endregion

        #region Constractor

        /// <summary>
        /// コンストラクタ
        /// </summary>
        private MeCabTagger()
        {
        }

        #endregion

        #region Open/Create

        /// <summary>
        /// MeCabTaggerを開く
        /// </summary>
        /// <param name="param">初期化パラメーター</param>
        private void Open(MeCabParam param)
        {
            this.viterbi.Open(param);

            this.writer.Open(param);
        }
#if !NETSTANDARD1_3
        /// <summary>
        /// MeCabTaggerのインスタンスを生成する
        /// </summary>
        /// <returns>MeCabTaggerのインスタンス</returns>
        public static MeCabTagger Create()
        {
            MeCabParam param = new MeCabParam();
            param.LoadDicRC();
            return MeCabTagger.Create(param);
        }
#endif
        /// <summary>
        /// MeCabTaggerのインスタンスを生成する
        /// </summary>
        /// <param name="param">初期化パラメーター</param>
        /// <returns>MeCabTaggerのインスタンス</returns>
        public static MeCabTagger Create(MeCabParam param)
        {
            MeCabTagger tagger = new MeCabTagger();
            tagger.Open(param);
            return tagger;
        }

        #endregion

        #region Parse

        /// <summary>
        /// 解析を行う
        /// </summary>
        /// <param name="str">解析対象の文字列</param>
        /// <returns>解析結果の文字列</returns>
        public unsafe string Parse(string str)
        {
            fixed (char* pStr = str)
                return this.Parse(pStr, str.Length);
        }

        /// <summary>
        /// 解析を行う
        /// </summary>
        /// <param name="str">解析対象の文字列へのポインタ</param>
        /// <param name="len">解析対象の文字列の長さ</param>
        /// <returns>解析結果の文字列</returns>
        public unsafe string Parse(char* str, int len)
        {
            MeCabNode n = this.ParseToNode(str, len);
            if (n == null) return null;
            StringBuilder os = new StringBuilder();
            this.writer.Write(os, n);
            return os.ToString();
        }

        /// <summary>
        /// 解析を行う
        /// </summary>
        /// <param name="str">解析対象の文字列</param>
        /// <returns>文頭の形態素</returns>
        public unsafe MeCabNode ParseToNode(string str)
        {
            if (str == null) throw new ArgumentNullException("str");

            fixed (char* pStr = str)
                return this.ParseToNode(pStr, str.Length);
        }

        /// <summary>
        /// 解析を行う
        /// </summary>
        /// <param name="str">解析対象の文字列へのポインタ</param>
        /// <param name="len">解析対象の文字列の長さ</param>
        /// <returns>文頭の形態素</returns>
        public unsafe MeCabNode ParseToNode(char* str, int len)
        {
            this.ThrowIfDisposed();

            return this.viterbi.Analyze(str, len);
        }

        /// <summary>
        /// 解析を行う
        /// </summary>
        /// <param name="str">解析対象の文字列</param>
        /// <returns>文頭の形態素</returns>
        public IEnumerable<MeCabNode> ParseToNodes(string str)
        {
            var node = this.ParseToNode(str);
            while (node != null)
            {
                yield return node;
                node = node.Next;
            }
        }
        #endregion

        #region NBest

        /// <summary>
        /// 解析を行い結果を確からしいものから順番に取得する
        /// </summary>
        /// <param name="str">解析対象の文字列</param>
        /// <returns>文頭の形態素を確からしい順に取得するための列挙子</returns>
        public unsafe IEnumerable<MeCabNode> ParseNBestToNode(string str)
        {
            fixed (char* pStr = str)
                return this.ParseNBestToNode(pStr, str.Length);
        }

        /// <summary>
        /// 解析を行い結果を確からしいものから順番に取得する
        /// </summary>
        /// <param name="str">解析対象の文字列へのポインタ</param>
        /// <returns>文頭の形態素を確からしい順に取得するための列挙子の公開</returns>
        public unsafe IEnumerable<MeCabNode> ParseNBestToNode(char* str, int len)
        {
            if (this.LatticeLevel == 0)
                throw new InvalidOperationException("Please set one or more to LatticeLevel.");

            MeCabNode n = this.ParseToNode(str, len);
            NBestGenerator nBest = new NBestGenerator();
            nBest.Set(n);
            return nBest.GetEnumerator();
        }

        /// <summary>
        /// ParseのN-Best解出力version
        /// </summary>
        /// <param name="n">必要な解析結果の個数</param>
        /// <param name="str">解析対象の文字列</param>
        /// <returns>解析結果の文字列</returns>
        public unsafe string ParseNBest(int n, string str)
        {
            fixed (char* pStr = str)
                return this.ParseNBest(n, pStr, str.Length);
        }

        /// <summary>
        /// ParseのN-Best解出力version
        /// </summary>
        /// <param name="n">必要な解析結果の個数</param>
        /// <param name="str">解析対象の文字列へのポインタ</param>
        /// <param name="len">解析対象の文字列の長さ</param>
        /// <returns>解析結果の文字列</returns>
        public unsafe string ParseNBest(int n, char* str, int len)
        {
            if (n <= 0) throw new ArgumentOutOfRangeException("n", "");

            if (n == 1) return this.Parse(str, len);

            StringBuilder os = new StringBuilder();
            foreach (MeCabNode node in this.ParseNBestToNode(str, len))
            {
                this.writer.Write(os, node);
                if (--n == 0) break;
            }
            return os.ToString();
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
                this.viterbi.Dispose(); //Nullチェック不要
            }

            this.disposed = true;
        }

        ~MeCabTagger()
        {
            this.Dispose(false);
        }

        private void ThrowIfDisposed()
        {
            if (this.disposed)
                throw new ObjectDisposedException(this.GetType().FullName);
        }

        #endregion
    }
}
