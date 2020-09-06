//  MeCab -- Yet Another Part-of-Speech and Morphological Analyzer
//
//  Copyright(C) 2001-2006 Taku Kudo <taku@chasen.org>
//  Copyright(C) 2004-2006 Nippon Telegraph and Telephone Corporation

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NMeCab.Core
{
    /// <summary>
    /// 文字種情報
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct CharInfo
    {
        #region Const/Field/Property

        private readonly uint bits;

        /// <summary>
        /// 互換カテゴリ
        /// </summary>
        public uint Type
        {
            get { return BitUtils.GetBitField(this.bits, 0, 18); }
        }

        /// <summary>
        /// デフォルトカテゴリ
        /// </summary>
        public uint DefaultType
        {
            get { return BitUtils.GetBitField(this.bits, 18, 8); }
        }

        /// <summary>
        /// 長さ: 未知語の候補生成方法
        /// </summary>
        /// <value>
        /// 1: 1文字までの文字列を未知語とする
        /// 2: 2文字までの文字列を未知語とする
        /// ...
        /// n: n文字までの文字列を未知語とする
        /// </value>
        public uint Length
        {
            get { return BitUtils.GetBitField(this.bits, 18 + 8, 4); }
        }

        /// <summary>
        /// グルーピング: 未知語の候補生成方法
        /// </summary>
        /// <value>
        /// true: 同じ字種でまとめる
        /// false: 同じ字種でまとめない
        /// </value>
        public bool Group
        {
            get { return BitUtils.GetFlag(this.bits, 18 + 8 + 4); }
        }

        /// <summary>
        /// 動作タイミング
        /// そのカテゴリにおいて, いつ未知語処理を動かすか
        /// </summary>
        /// <value>
        /// true: 常に未知語処理を動かす
        /// false: 既知語がある場合は, 未知語処理を動作させない
        /// </value>
        public bool Invoke
        {
            get { return BitUtils.GetFlag(this.bits, 18 + 8 + 4 + 1); }
        }

        #endregion

        #region Constractor

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="bits">ビット列</param>
        public CharInfo(uint bits)
        {
            this.bits = bits;
        }

        #endregion

        #region Method

        /// <summary>
        /// 互換カテゴリ判定
        /// </summary>
        /// <param name="c"></param>
        /// <returns>
        /// true: 互換
        /// false: 非互換
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsKindOf(CharInfo c)
        {
            return BitUtils.CompareAnd(this.bits, c.bits, 0, 18);
        }

        /// <summary>
        /// インスタンスの文字列表現を返します。
        /// </summary>
        /// <returns>文字列表現</returns>
        public override string ToString()
        {
            return string.Format("[Type:{0}][DefaultType:{1}][Length:{2}][Group:{3}][Invoke:{4}]",
                                 this.Type,
                                 this.DefaultType,
                                 this.Length,
                                 this.Group,
                                 this.Invoke);
        }

        #endregion
    }
}
