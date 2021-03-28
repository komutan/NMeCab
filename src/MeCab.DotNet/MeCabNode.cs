//  MeCab -- Yet Another Part-of-Speech and Morphological Analyzer
//
//  Copyright(C) 2001-2006 Taku Kudo <taku@chasen.org>
//  Copyright(C) 2004-2006 Nippon Telegraph and Telephone Corporation
using System;
using System.Collections.Generic;
using System.Text;
using MeCab.Core;
using System.Globalization;

namespace MeCab
{
    public class MeCabNode
    {
        /// <summary>
        /// 一つ前の形態素
        /// </summary>
        public MeCabNode Prev { get; set; }

        /// <summary>
        /// 一つ先の形態素
        /// </summary>
        public MeCabNode Next { get; set; }

        /// <summary>
        /// 同じ位置で終わる形態素
        /// </summary>
        public MeCabNode ENext { get; set; }

        /// <summary>
        /// 同じ開始位置で始まる形態素
        /// </summary>
        public MeCabNode BNext { get; set; }

        internal MeCabPath RPath { get; set; }

        internal MeCabPath LPath { get; set; }

        //internal MeCabNode[] BeginNodeList { get; set; }

        //internal MeCabNode[] EndNodeList { get; set; }

        /// <summary>
        /// 形態素の文字列情報
        /// </summary>
        public string Surface { get; set; }

        private string feature;

        /// <summary>
        /// CSV で表記された素性情報
        /// </summary>
        public string Feature
        {
            get
            {
                if (feature == null && Dictionary != null)
                    feature = Dictionary.GetFeature(featurePos);
                return feature;
            }
            set
            {
                feature = value;
            }
        }

        private uint featurePos;
        private MeCabDictionary Dictionary { get; set; }

        /// <summary>
        /// 素性情報を遅延読込するための値設定
        /// </summary>
        /// <param name="featurePos">辞書内の素性情報の位置</param>
        /// <param name="dic">検索元の辞書</param>
        internal void SetFeature(uint featurePos, MeCabDictionary dic)
        {
            feature = null;
            this.featurePos = featurePos;
            Dictionary = dic;
        }

#if NeedId
        /// <summary>
        /// 形態素に付与される ユニークID
        /// </summary>
        public uint Id { get; set; }
#endif

        /// <summary>
        /// 形態素の長さ
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// 形態素の長さ(先頭のスペースを含む)
        /// </summary>
        public int RLength { get; set; }

        /// <summary>
        /// 右文脈 id
        /// </summary>
        public ushort RCAttr { get; set; }

        /// <summary>
        /// 左文脈 id
        /// </summary>
        public ushort LCAttr { get; set; }

        /// <summary>
        /// 形態素 ID
        /// </summary>
        public ushort PosId { get; set; }

        /// <summary>
        /// 文字種情報
        /// </summary>
        public uint CharType { get; set; }

        /// <summary>
        /// 形態素の種類
        /// </summary>
        public MeCabNodeStat Stat { get; set; }

        /// <summary>
        /// ベスト解
        /// </summary>
        public bool IsBest { get; set; }

        ///// <summary>
        ///// it is avaialbe only when BOS node
        ///// </summary>
        //public int SentenceLength { get; set; }

        /// <summary>
        /// forward backward の foward log 確率
        /// </summary>
        public float Alpha { get; set; }

        /// <summary>
        /// forward backward の backward log 確率
        /// </summary>
        public float Beta { get; set; }

        /// <summary>
        /// 周辺確率
        /// </summary>
        public float Prob { get; set; }

        /// <summary>
        /// 単語生起コスト
        /// </summary>
        public short WCost { get; set; }

        /// <summary>
        /// 累積コスト
        /// </summary>
        public long Cost { get; set; }

        //public Token Token { get; set; }

        public int BPos { get; set; }

        public int EPos { get; set; }

        public override string ToString()
        {
            StringBuilder os = new StringBuilder();
#if NeedId
            os.Append(this.Id).Append(" ");
#endif
            os.Append("[Surface:");
            if (Stat == MeCabNodeStat.Bos)
                os.Append("BOS");
            else if (Stat == MeCabNodeStat.Eos)
                os.Append("EOS");
            else
                os.Append(Surface);
            os.Append("]");

            os.Append("[Feature:").Append(Feature).Append("]");
            os.Append("[BPos:").Append(BPos).Append("]");
            os.Append("[EPos:").Append(EPos).Append("]");
            os.Append("[RCAttr:").Append(RCAttr).Append("]");
            os.Append("[LCAttr:").Append(LCAttr).Append("]");
            os.Append("[PosId:").Append(PosId).Append("]");
            os.Append("[CharType:").Append(CharType).Append("]");
            os.Append("[Stat:").Append((int)Stat).Append("]");
            os.Append("[IsBest:").Append(IsBest).Append("]");
            os.Append("[Alpha:").Append(Alpha).Append("]");
            os.Append("[Beta:").Append(Beta).Append("]");
            os.Append("[Prob:").Append(Prob).Append("]");
            os.Append("[Cost:").Append(Cost).Append("]");

            for (MeCabPath path = LPath; path != null; path = path.LNext)
            {
                os.Append("[Path:");
#if NeedId
                os.Append(path.LNode.Id).Append(" ");
#endif
                os.Append("(Cost:").Append(path.Cost).Append(")");
                os.Append("(Prob:").Append(path.Prob).Append(")");
                os.Append("]");
            }

            return os.ToString();
        }
    }
}
