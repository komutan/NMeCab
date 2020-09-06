//  MeCab -- Yet Another Part-of-Speech and Morphological Analyzer
//
//  Copyright(C) 2001-2006 Taku Kudo <taku@chasen.org>
//  Copyright(C) 2004-2006 Nippon Telegraph and Telephone Corporation
using NMeCab.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace NMeCab
{
    /// <summary>
    /// 形態素解析処理の起点を表す抽象基底クラスです。
    /// </summary>
    /// <typeparam name="TNode">形態素ノードの型</typeparam>
    public abstract class MeCabTaggerBase<TNode> : IDisposable
        where TNode : MeCabNodeBase<TNode>
    {
        private readonly Viterbi<TNode> viterbi = new Viterbi<TNode>();

        private Func<TNode> nodeAllocator;

        #region Static Create

        /// <summary>
        /// 形態素解析処理の起点を作成します。
        /// </summary>
        /// <typeparam name="TTagger">作成する形態素解析処理の起点の具象型</typeparam>
        /// <param name="dicDir">使用する辞書のディレクトリへのパス</param>
        /// <param name="userDics">使用するユーザー辞書のファイル名のコレクション</param>
        /// <param name="taggerAllocator">Taggetインスタンス生成メソッド</param>
        /// <param name="nodeAllocator">Nodeインスタンス生成メソッド</param>
        /// <param name="defaultDicDirName">使用する辞書のディレクトリへのパスが無いときに使用するディレクトリ名の初期値</param>
        /// <returns>形態素解析処理の起点</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static TTagger Create<TTagger>(string dicDir,
                                                 IEnumerable<string> userDics,
                                                 Func<TTagger> taggerAllocator,
                                                 Func<TNode> nodeAllocator,
                                                 string defaultDicDirName)
            where TTagger : MeCabTaggerBase<TNode>
        {
            TTagger tagger = null;
            try
            {
                tagger = taggerAllocator();
                tagger.nodeAllocator = nodeAllocator;
                tagger.viterbi.Open(GetTrimedDicDir(), GetTirmedUserDics());
                return tagger;
            }
            catch (Exception)
            {
                tagger?.Dispose();
                throw;
            }

            string GetTrimedDicDir()
            {
                return dicDir ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, defaultDicDirName);
            }

            string[] GetTirmedUserDics()
            {
                if (userDics == null)
                    return Array.Empty<string>();
                else if (userDics is string[] ary)
                    return ary;
                else if (userDics is List<string> list)
                    return list.ToArray();
                else
                    return (new List<string>(userDics)).ToArray();
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
        public unsafe IEnumerable<TNode[]> ParseNBest(string sentence)
        {
            fixed (char* pStr = sentence)
                return this.ParseNBest(pStr, sentence.Length);
        }

        /// <summary>
        /// 形態素解析を行い、確からしい順に複数の形態素列を取得します。
        /// </summary>
        /// <param name="sentence">解析対象の文字列へのポインタ</param>
        /// <param name="length">解析対象の文字列の長さ</param>
        /// <returns>形態素の配列を確からしい順に取得する列挙子</returns>
        public unsafe IEnumerable<TNode[]> ParseNBest(char* sentence, int length)
        {
            var param = new MeCabParam()
            {
                LatticeLevel = MeCabLatticeLevel.One
            };

            return this.ParseToLattice(sentence, length, param).GetNBestResults();
        }

        #endregion

        #region SoftWakachi

        /// <summary>
        /// 形態素解析を行い、可能性があるすべての形態素を周辺確率付きで取得します。
        /// </summary>
        /// <param name="sentence">解析対象の文字列</param>
        /// <param name="theta">ソフト分かち書きの温度パラメータ</param>
        /// <returns>すべての形態素ノードの配列</returns>
        public unsafe TNode[] ParseSoftWakachi(string sentence,
                                               float theta = MeCabParam.DefaltTheta)
        {
            if (sentence == null) throw new ArgumentNullException(nameof(sentence));

            fixed (char* pStr = sentence)
                return this.ParseSoftWakachi(pStr, sentence.Length, theta);
        }

        /// <summary>
        /// 形態素解析を行い、可能性があるすべての形態素を周辺確率付きで取得します。
        /// </summary>
        /// <param name="sentence">解析対象の文字列へのポインタ</param>
        /// <param name="length">解析対象の文字列の長さ</param>
        /// <param name="theta">ソフト分かち書きの温度パラメータ</param>
        /// <returns>すべての形態素ノードの配列</returns>
        public unsafe TNode[] ParseSoftWakachi(char* sentence, int length,
                                               float theta = MeCabParam.DefaltTheta)
        {
            var param = new MeCabParam()
            {
                LatticeLevel = MeCabLatticeLevel.Two,
                Theta = theta
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

            var lattice = new MeCabLattice<TNode>(this.nodeAllocator, param, length);
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

        /// <summary>
        /// 使用中のリソースを開放します。
        /// </summary>
        /// <param name="disposing">マネージドリソースとアンマネージドリソースの両方を解放する場合はtrue。アンマネージド リソースだけを解放する場合はfalse。</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;

            if (disposing)
            {
                this.viterbi.Dispose();
            }

            this.disposed = true;
        }

        /// <summary>
        /// デストラクタ
        /// </summary>
        ~MeCabTaggerBase()
        {
            this.Dispose(false);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ThrowIfDisposed()
        {
            if (this.disposed)
                throw new ObjectDisposedException(this.GetType().FullName);
        }

        #endregion
    }
}
