//  MeCab -- Yet Another Part-of-Speech and Morphological Analyzer
//
//  Copyright(C) 2001-2006 Taku Kudo <taku@chasen.org>
//  Copyright(C) 2004-2006 Nippon Telegraph and Telephone Corporation
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MeCab.Core
{
    public struct Token
    {
        #region  Const/Field/Property

        /// <summary>
        /// 右文脈 id
        /// </summary>
        public ushort LcAttr { get; set; }

        /// <summary>
        /// 左文脈 id
        /// </summary>
        public ushort RcAttr { get; set; }

        /// <summary>
        /// 形態素 ID
        /// </summary>
        public ushort PosId { get; set; }

        /// <summary>
        /// 単語生起コスト
        /// </summary>
        public short WCost { get; set; }

        /// <summary>
        /// 素性情報の位置
        /// </summary>
        public uint Feature { get; set; }

        /// <summary>
        /// reserved for noun compound
        /// </summary>
        public uint Compound { get; set; }

        #endregion

        #region Method

        public static Token Create(BinaryReader reader)
        {
            return new Token()
            {
                LcAttr = reader.ReadUInt16(),
                RcAttr = reader.ReadUInt16(),
                PosId = reader.ReadUInt16(),
                WCost = reader.ReadInt16(),
                Feature = reader.ReadUInt32(),
                Compound = reader.ReadUInt32()
            };
        }

        public override string ToString()
        {
            return string.Format("[LcAttr:{0}][RcAttr:{1}][PosId:{2}][WCost:{3}][Feature:{4}][Compound:{5}]",
                                 this.LcAttr,
                                 this.RcAttr,
                                 this.PosId,
                                 this.WCost,
                                 this.Feature,
                                 this.Compound);
        }

        #endregion
    }
}
