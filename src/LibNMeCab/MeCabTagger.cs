//  MeCab -- Yet Another Part-of-Speech and Morphological Analyzer
//
//  Copyright(C) 2001-2006 Taku Kudo <taku@chasen.org>
//  Copyright(C) 2004-2006 Nippon Telegraph and Telephone Corporation
using System;
using System.Collections.Generic;
using NMeCab.Core;

namespace NMeCab
{
    public class MeCabTagger : MeCabTaggerBase<MeCabNode>
    {
        protected override MeCabNode CreateNewNode()
        {
            return new MeCabNode();
        }
    }

    public class MeCabIpaDicTagger : MeCabTaggerBase<MeCabIpaDicNode>
    {
        protected override MeCabIpaDicNode CreateNewNode()
        {
            return new MeCabIpaDicNode();
        }
    }

    public abstract class MeCabTaggerBase<TNode> : IDisposable
        where TNode : MeCabNodeBase<TNode>
    {
        private readonly Viterbi<TNode> viterbi = new Viterbi<TNode>();

        protected abstract TNode CreateNewNode();

        #region Open

        private bool opened = false;

        public void Open(string dicDir, IEnumerable<string> userDics)
        {
            this.Open(dicDir, new List<string>(userDics).ToArray());
        }

        public void Open(string dicDir, string[] userDics)
        {
            if (this.opened)
                throw new InvalidOperationException("this tagger was alredy opened.");

            this.viterbi.Open(dicDir, userDics);

            this.opened = true;
        }

        private void ThrowIfNotOpend()
        {
            if (!this.opened)
                throw new InvalidOperationException("this tagger was not opned.");
        }

        #endregion

        #region Parse

        /// <summary>
        /// 解析を行う
        /// </summary>
        /// <param name="sentence">解析対象の文字列</param>
        /// <returns>形態素の配列</returns>
        public unsafe TNode[] Parse(string sentence)
        {
            fixed (char* pStr = sentence)
                return this.Parse(pStr, sentence.Length);
        }

        /// <summary>
        /// 解析を行う
        /// </summary>
        /// <param name="sentence">解析対象の文字列へのポインタ</param>
        /// <param name="length">解析対象の文字列の長さ</param>
        /// <returns>形態素の配列</returns>
        public unsafe TNode[] Parse(char* sentence, int length)
        {
            var param = new MeCabParam()
            {
                LatticeLevel = MeCabLatticeLevel.Zero
            };

            return this.ParseToLattice(sentence, length, param).GetBestNodes();
        }

        #endregion

        #region NBest

        /// <summary>
        /// 解析を行い結果を確からしいものから順番に取得する
        /// </summary>
        /// <param name="sentence">解析対象の文字列</param>
        /// <returns>形態素の配列を確からしい順に取得する列挙子</returns>
        public unsafe IEnumerable<TNode[]> ParseNBestToNode(string sentence)
        {
            fixed (char* pStr = sentence)
                return this.ParseNBestToNode(pStr, sentence.Length);
        }

        /// <summary>
        /// 解析を行い結果を確からしいものから順番に取得する
        /// </summary>
        /// <param name="sentence">解析対象の文字列へのポインタ</param>
        /// <param name="length">解析対象の文字列の長さ</param>
        /// <returns>形態素の配列を確からしい順に取得する列挙子</returns>
        public unsafe IEnumerable<TNode[]> ParseNBestToNode(char* sentence, int length)
        {
            var param = new MeCabParam()
            {
                LatticeLevel = MeCabLatticeLevel.One
            };

            return this.ParseToLattice(sentence, length, param).GetNBestResults();
        }

        #endregion

        #region AllMophs

        /// <summary>
        /// 解析を行い可能性があるすべての形態素を取得する
        /// </summary>
        /// <param name="sentence">解析対象の文字列</param>
        /// <returns>形態素の配列</returns>
        public unsafe TNode[] ParseAllMorphs(string sentence)
        {
            if (sentence == null)
                throw new ArgumentNullException(nameof(sentence));

            fixed (char* pStr = sentence)
                return this.ParseAllMorphs(pStr, sentence.Length);
        }

        /// <summary>
        /// 解析を行い可能性があるすべての形態素を取得する
        /// </summary>
        /// <param name="sentence">解析対象の文字列へのポインタ</param>
        /// <param name="length">解析対象の文字列の長さ</param>
        /// <returns>形態素の配列</returns>
        public unsafe TNode[] ParseAllMorphs(char* sentence, int length)
        {
            var param = new MeCabParam()
            {
                LatticeLevel = MeCabLatticeLevel.Two
            };

            return this.ParseToLattice(sentence, length, param).GetAllNodes();
        }

        #endregion

        #region Lattice

        /// <summary>
        /// 解析を行い、結果をラティスとして取得する
        /// </summary>
        /// <param name="sentence">解析対象の文字列</param>
        /// <param name="param">解析パラメータ</param>
        /// <returns>ラティス</returns>
        public unsafe MeCabLattice<TNode> ParseToLattice(string sentence, MeCabParam param)
        {
            if (sentence == null)
                throw new ArgumentNullException(nameof(sentence));

            fixed (char* pStr = sentence)
                return this.ParseToLattice(pStr, sentence.Length, param);
        }

        /// <summary>
        /// 解析を行い、結果をラティスとして取得する
        /// </summary>
        /// <param name="sentence">解析対象の文字列へのポインタ</param>
        /// <param name="length">解析対象の文字列の長さ</param>
        /// <param name="param">解析パラメータ</param>
        /// <returns>ラティス</returns>
        public unsafe MeCabLattice<TNode> ParseToLattice(char* sentence, int length, MeCabParam param)
        {
            this.ThrowIfNotOpend();
            this.ThrowIfDisposed();
            if (length <= 0)
                throw new ArgumentOutOfRangeException("Please set one or more to length of string.");

            var lattice = new MeCabLattice<TNode>(this.CreateNewNode, param, length);
            this.viterbi.Analyze(sentence, length, lattice);
            return lattice;
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

        ~MeCabTaggerBase()
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
