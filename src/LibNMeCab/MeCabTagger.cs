//  MeCab -- Yet Another Part-of-Speech and Morphological Analyzer
//
//  Copyright(C) 2001-2006 Taku Kudo <taku@chasen.org>
//  Copyright(C) 2004-2006 Nippon Telegraph and Telephone Corporation
using System;
using System.Collections.Generic;
using NMeCab.Core;

namespace NMeCab
{
    /// <summary>
    /// 形態素解析処理の起点を表します。使用する辞書の形式は限定しません。
    /// </summary>
    public class MeCabTagger : MeCabTaggerBase<MeCabNode>
    {
        /// <summary>
        /// コンストラクタ（非公開）
        /// </summary>
        private MeCabTagger()
        { }

        /// <summary>
        /// 形態素解析処理の起点を作成します。
        /// </summary>
        /// <param name="dicDir">使用する辞書のディレクトリへのパス</param>
        /// <param name="userDics">使用するユーザー辞書のファイル名のコレクション</param>
        /// <returns>形態素解析処理の起点</returns>
        public static MeCabTagger Create(string dicDir, string[] userDic = null)
        {
            return Create(dicDir, userDic, () => new MeCabTagger());
        }

        /// <summary>
        /// 形態素ノードインスタンス生成メソッドです。
        /// </summary>
        /// <returns>形態素ノード</returns>
        protected override MeCabNode CreateNewNode()
        {
            return new MeCabNode();
        }
    }

    /// <summary>
    /// 形態素解析処理の起点を表す抽象基底クラスです。
    /// </summary>
    /// <typeparam name="TNode">形態素ノードの型</typeparam>
    public abstract class MeCabTaggerBase<TNode> : IDisposable
        where TNode : MeCabNodeBase<TNode>
    {
        private readonly Viterbi<TNode> viterbi = new Viterbi<TNode>();

        /// <summary>
        /// 形態素ノードインスタンス生成メソッドです。（内部用）
        /// </summary>
        /// <returns>形態素ノード</returns>
        protected abstract TNode CreateNewNode();

        #region Static Create

        /// <summary>
        /// 形態素解析処理の起点を作成します。
        /// </summary>
        /// <typeparam name="TTagger">作成する形態素解析処理の起点の具象型</typeparam>
        /// <param name="dicDir">使用する辞書のディレクトリへのパス</param>
        /// <param name="userDics">使用するユーザー辞書のファイル名のコレクション</param>
        /// <param name="allocator">Taggetインスタンス生成メソッド</param>
        /// <returns>形態素解析処理の起点</returns>
        protected static TTagger Create<TTagger>(string dicDir,
                                                 IEnumerable<string> userDics,
                                                 Func<TTagger> allocator)
            where TTagger : MeCabTaggerBase<TNode>
        {
            if (dicDir == null) throw new ArgumentNullException(nameof(dicDir));
            if (allocator == null) throw new ArgumentNullException(nameof(allocator));

            TTagger tagger = null;
            try
            {
                tagger = allocator();
                tagger.viterbi.Open(dicDir, ToNullTrimedArray(userDics));
                return tagger;
            }
            catch (Exception)
            {
                tagger?.Dispose();
                throw;
            }

            string[] ToNullTrimedArray(IEnumerable<string> target)
            {
                if (target == null)
                    return Array.Empty<string>();
                else if (target is string[] ary)
                    return ary;
                else if (target is List<string> list)
                    return list.ToArray();
                else
                    return (new List<string>(target)).ToArray();
            }
        }

        #endregion

        #region Parse

        /// <summary>
        /// 形態素解析を行い、最も確からしい形態素列を取得します。
        /// </summary>
        /// <param name="sentence">解析対象の文字列</param>
        /// <returns>最も確からしい形態素ノードの配列</returns>
        public unsafe TNode[] Parse(string sentence)
        {
            fixed (char* pStr = sentence)
                return this.Parse(pStr, sentence.Length);
        }

        /// <summary>
        /// 形態素解析を行い、最も確からしい形態素列を取得します。
        /// </summary>
        /// <param name="sentence">解析対象の文字列へのポインタ</param>
        /// <param name="length">解析対象の文字列の長さ</param>
        /// <returns>最も確からしい形態素ノードの配列</returns>
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
        /// 形態素解析を行い、確からしい順に複数の形態素列を取得します。
        /// </summary>
        /// <param name="sentence">解析対象の文字列</param>
        /// <returns>形態素ノードの配列を確からしい順に取得する列挙子</returns>
        public unsafe IEnumerable<TNode[]> ParseNBestToNode(string sentence)
        {
            fixed (char* pStr = sentence)
                return this.ParseNBestToNode(pStr, sentence.Length);
        }

        /// <summary>
        /// 形態素解析を行い、確からしい順に複数の形態素列を取得します。
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
        /// 形態素解析を行い、可能性があるすべての形態素を周辺確率付きで取得します。
        /// </summary>
        /// <param name="sentence">解析対象の文字列</param>
        /// <returns>すべての形態素ノードの配列</returns>
        public unsafe TNode[] ParseAllMorphs(string sentence)
        {
            if (sentence == null) throw new ArgumentNullException(nameof(sentence));

            fixed (char* pStr = sentence)
                return this.ParseAllMorphs(pStr, sentence.Length);
        }

        /// <summary>
        /// 形態素解析を行い、可能性があるすべての形態素を周辺確率付きで取得します。
        /// </summary>
        /// <param name="sentence">解析対象の文字列へのポインタ</param>
        /// <param name="length">解析対象の文字列の長さ</param>
        /// <returns>すべての形態素ノードの配列</returns>
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
        /// 形態素解析を行い、結果をラティスとして取得します。
        /// </summary>
        /// <param name="sentence">解析対象の文字列</param>
        /// <param name="param">解析パラメータ</param>
        /// <returns>ラティス</returns>
        public unsafe MeCabLattice<TNode> ParseToLattice(string sentence, MeCabParam param)
        {
            if (sentence == null) throw new ArgumentNullException(nameof(sentence));

            fixed (char* pStr = sentence)
                return this.ParseToLattice(pStr, sentence.Length, param);
        }

        /// <summary>
        /// 形態素解析を行い、結果をラティスとして取得します
        /// </summary>
        /// <param name="sentence">解析対象の文字列へのポインタ</param>
        /// <param name="length">解析対象の文字列の長さ</param>
        /// <param name="param">解析パラメータ</param>
        /// <returns>ラティス</returns>
        public unsafe MeCabLattice<TNode> ParseToLattice(char* sentence, int length, MeCabParam param)
        {
            this.ThrowIfDisposed();
            if (length < 0) throw new ArgumentOutOfRangeException(nameof(length));

            var lattice = new MeCabLattice<TNode>(this.CreateNewNode, param, length);
            this.viterbi.Analyze(sentence, length, lattice);
            return lattice;
        }

        #endregion

        #region Dispose

        private bool disposed;

        /// <summary>
        /// 使用中のリソースを開放します。
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
