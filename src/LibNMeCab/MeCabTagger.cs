//  MeCab -- Yet Another Part-of-Speech and Morphological Analyzer
//
//  Copyright(C) 2001-2006 Taku Kudo <taku@chasen.org>
//  Copyright(C) 2004-2006 Nippon Telegraph and Telephone Corporation
using System;
using System.Collections.Generic;
using NMeCab.Core;

namespace NMeCab
{
    public class MeCabTagger : IDisposable
    {
        private readonly Viterbi viterbi = new Viterbi();

        #region Constractor

        /// <summary>
        /// コンストラクタ
        /// </summary>
        private MeCabTagger()
        { }

        #endregion

        #region Create

        /// <summary>
        /// MeCabTaggerインスタンスを作成する
        /// </summary>
        /// <param name="dicDir">辞書のディレクトリ</param>
        /// <param name="userDics">ユーザー辞書ファイル名のコレクション</param>
        /// <returns>MeCabTaggerのインスタンス</returns>
        public static MeCabTagger Create(string dicDir = "dic",
                                         IEnumerable<string> userDics = null)
        {
            var userDicAry = userDics == null
                           ? new string[0]
                           : System.Linq.Enumerable.ToArray(userDics); // 限定でLINQを使用

            MeCabTagger tagger = null;
            try
            {
                tagger = new MeCabTagger();
                tagger.viterbi.Open(dicDir, userDicAry);

                return tagger;
            }
            catch (Exception)
            {
                tagger?.Dispose();
                throw;
            }
        }

        #endregion

        #region Parse

        /// <summary>
        /// 解析を行う
        /// </summary>
        /// <param name="str">解析対象の文字列</param>
        /// <returns>文頭の形態素</returns>
        public unsafe MeCabNode Parse(string str)
        {
            if (str == null) throw new ArgumentNullException(nameof(str));

            fixed (char* pStr = str)
                return this.Parse(pStr, str.Length);
        }

        /// <summary>
        /// 解析を行う
        /// </summary>
        /// <param name="str">解析対象の文字列へのポインタ</param>
        /// <param name="len">解析対象の文字列の長さ</param>
        /// <returns>文頭の形態素</returns>
        public unsafe MeCabNode Parse(char* str, int len)
        {
            var param = new MeCabParam();

            var lattice = this.ParseToLattice(str, len, param);
            return lattice.BosNode;
        }

        #endregion

        #region NBest

        /// <summary>
        /// 解析を行い結果を確からしいものから順番に取得する
        /// </summary>
        /// <param name="str">解析対象の文字列</param>
        /// <returns>文頭の形態素を、確からしい順に取得する列挙子</returns>
        public unsafe IEnumerable<MeCabNode> ParseNBestToNode(string str)
        {
            if (str == null) throw new ArgumentNullException(nameof(str));

            fixed (char* pStr = str)
                return this.ParseNBestToNode(pStr, str.Length);
        }

        /// <summary>
        /// 解析を行い結果を確からしいものから順番に取得する
        /// </summary>
        /// <param name="str">解析対象の文字列へのポインタ</param>
        /// <returns>文頭の形態素を、確からしい順に取得する列挙子</returns>
        public unsafe IEnumerable<MeCabNode> ParseNBestToNode(char* str, int len)
        {
            var param = new MeCabParam()
            {
                NBest = true
            };

            var lattice = this.ParseToLattice(str, len, param);
            var nBest = new NBestGenerator();
            nBest.Set(lattice.EosNode);
            return nBest.GetEnumerator();
        }

        #endregion

        #region AllMophs

        public unsafe MeCabNode ParseAllMorphs(string str)
        {
            if (str == null) throw new ArgumentNullException(nameof(str));

            fixed (char* pStr = str)
                return this.ParseAllMorphs(pStr, str.Length);
        }

        public unsafe MeCabNode ParseAllMorphs(char* str, int len)
        {
            var param = new MeCabParam()
            {
                AllMorphs = true
            };

            var lattice = this.ParseToLattice(str, len, param);
            return lattice.BosNode;
        }

        #endregion

        #region Lattice

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public unsafe MeCabLattice ParseToLattice(string str, MeCabParam param)
        {
            if (str == null) throw new ArgumentNullException(nameof(str));

            fixed (char* pStr = str)
                return this.ParseToLattice(pStr, str.Length, param);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <param name="len"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public unsafe MeCabLattice ParseToLattice(char* str, int len, MeCabParam param)
        {
            this.ThrowIfDisposed();
            if (len <= 0)
                throw new ArgumentOutOfRangeException("Please set one or more to length of string.");

            return this.viterbi.Analyze(str, len, param);
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
                this.viterbi.Dispose();
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
