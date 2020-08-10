//  MeCab -- Yet Another Part-of-Speech and Morphological Analyzer
//
//  Copyright(C) 2001-2006 Taku Kudo <taku@chasen.org>
//  Copyright(C) 2004-2006 Nippon Telegraph and Telephone Corporation

#pragma warning disable CS1591

using System.IO;

namespace NMeCab.Core
{
    public readonly struct Token
    {
        #region  Const/Field/Property

        /// <summary>
        /// 右文脈 id
        /// </summary>
        public readonly ushort LcAttr;

        /// <summary>
        /// 左文脈 id
        /// </summary>
        public readonly ushort RcAttr;

        /// <summary>
        /// 形態素 ID
        /// </summary>
        public readonly ushort PosId;

        /// <summary>
        /// 単語生起コスト
        /// </summary>
        public readonly short WCost;

        /// <summary>
        /// 素性情報の位置
        /// </summary>
        public readonly uint Feature;

        /// <summary>
        /// reserved for noun compound
        /// </summary>
        public readonly uint Compound;

        #endregion

        public Token(BinaryReader reader)
        {
            LcAttr = reader.ReadUInt16();
            RcAttr = reader.ReadUInt16();
            PosId = reader.ReadUInt16();
            WCost = reader.ReadInt16();
            Feature = reader.ReadUInt32();
            Compound = reader.ReadUInt32();
        }

        public override string ToString()
        {
            return $"[LcAttr:{this.LcAttr}][RcAttr:{this.RcAttr}][PosId:{this.PosId}][WCost:{this.WCost}][Feature:{this.Feature}][Compound:{this.Compound}]";
        }
    }
}
