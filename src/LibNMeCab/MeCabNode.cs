//  MeCab -- Yet Another Part-of-Speech and Morphological Analyzer
//
//  Copyright(C) 2001-2006 Taku Kudo <taku@chasen.org>
//  Copyright(C) 2004-2006 Nippon Telegraph and Telephone Corporation
using System.Text;
using NMeCab.Core;

namespace NMeCab
{
    /// <summary>
    /// 形態素ノードを表します。使用する辞書の形式は限定しません。
    /// </summary>
    public class MeCabNode : MeCabNodeBase<MeCabNode>
    { }

    /// <summary>
    /// 形態素ノードを表す抽象基底クラスです。
    /// </summary>
    /// <typeparam name="TNode">連結する形態素ノードの具象型</typeparam>
    public abstract class MeCabNodeBase<TNode>
        where TNode : MeCabNodeBase<TNode>
    {
        /// <summary>
        /// 一つ前の形態素
        /// </summary>
        public TNode Prev { get; set; }

        /// <summary>
        /// 一つ後の形態素
        /// </summary>
        public TNode Next { get; set; }

        /// <summary>
        /// 同じ開始位置で始まる形態素
        /// </summary>
        public TNode BNext { get; set; }

        /// <summary>
        /// 同じ位置で終わる形態素
        /// </summary>
        public TNode ENext { get; set; }

        /// <summary>
        /// 前の形態素の候補へのパス
        /// </summary>
        public MeCabPath<TNode> LPath { get; set; }

        /// <summary>
        /// 次の形態素の候補へのパス
        /// </summary>
        public MeCabPath<TNode> RPath { get; set; }

        /// <summary>
        /// 形態素の表層形
        /// </summary>
        public string Surface { get; set; }

        private string feature;

        /// <summary>
        /// CSVで表記された素性情報
        /// </summary>
        public string Feature
        {
            get
            {
                if (this.feature == null && this.Dictionary != null)
                    this.feature = this.Dictionary.GetFeature(this.featurePos);
                return this.feature;
            }
            set
            {
                this.feature = value;
            }
        }

        private uint featurePos;
        private MeCabDictionary Dictionary { get; set; }

        /// <summary>
        /// 素性情報を遅延読込するための値設定
        /// </summary>
        /// <param name="featurePos">辞書内の素性情報の位置</param>
        /// <param name="dic">検索元の辞書</param>
        public void SetFeature(uint featurePos, MeCabDictionary dic)
        {
            this.feature = null;
            this.featurePos = featurePos;
            this.Dictionary = dic;
        }

        /// <summary>
        /// 解析の単位で形態素に付与するユニークID
        /// </summary>
        public uint Id { get; set; }

        /// <summary>
        /// 形態素の長さ
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// 形態素の長さ(先頭のスペースを含む)
        /// </summary>
        public int RLength { get; set; }

        /// <summary>
        /// 左文脈ID
        /// </summary>
        public ushort LCAttr { get; set; }

        /// <summary>
        /// 右文脈ID
        /// </summary>
        public ushort RCAttr { get; set; }

        /// <summary>
        /// 形態素ID
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
        /// ベスト解か
        /// </summary>
        public bool IsBest { get; set; }

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

        /// <summary>
        /// 開始位置
        /// </summary>
        public int BPos { get; set; }

        /// <summary>
        /// 終了位置
        /// </summary>
        public int EPos { get; set; }

        /// <summary>
        /// インスタンスの文字列表現を返します。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder os = new StringBuilder();
            os.Append(this.Id).Append(" ");
            os.Append("[Surface:");
            if (this.Stat == MeCabNodeStat.Bos)
                os.Append("BOS");
            else if (this.Stat == MeCabNodeStat.Eos)
                os.Append("EOS");
            else
                os.Append(this.Surface);
            os.Append("]");
            os.Append("[Feature:").Append(this.Feature).Append("]");
            os.Append("[BPos:").Append(this.BPos).Append("]");
            os.Append("[EPos:").Append(this.EPos).Append("]");
            os.Append("[RCAttr:").Append(this.RCAttr).Append("]");
            os.Append("[LCAttr:").Append(this.LCAttr).Append("]");
            os.Append("[PosId:").Append(this.PosId).Append("]");
            os.Append("[CharType:").Append(this.CharType).Append("]");
            os.Append("[Stat:").Append((int)this.Stat).Append("]");
            os.Append("[IsBest:").Append(this.IsBest).Append("]");
            os.Append("[Alpha:").Append(this.Alpha).Append("]");
            os.Append("[Beta:").Append(this.Beta).Append("]");
            os.Append("[Prob:").Append(this.Prob).Append("]");
            os.Append("[Cost:").Append(this.Cost).Append("]");
            return os.ToString();
        }
    }
}
