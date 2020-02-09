//  MeCab -- Yet Another Part-of-Speech and Morphological Analyzer
//
//  Copyright(C) 2001-2006 Taku Kudo <taku@chasen.org>
//  Copyright(C) 2004-2006 Nippon Telegraph and Telephone Corporation
using System.Text;
using NMeCab.Core;

namespace NMeCab
{
    public class MeCabNode : MeCabNodeBase<MeCabNode>
    { }

    public class MeCabIpaDicNode : MeCabNodeBase<MeCabIpaDicNode>
    {
        private string[] features = null;

        private string[] Features
        {
            get
            {
                if (features == null)
                    features = StrUtils.ParseCsvRow(this.Feature, 8, 16);

                return this.features;
            }
        }

        /// <summary>
        /// 品詞
        /// </summary>
        public string PartsOfSpeech
        {
            get { return this.Features[0]; }
        }

        /// <summary>
        /// 品詞細分類1
        /// </summary>
        public string PartsOfSpeechSection1
        {
            get { return this.Features[1]; }
        }

        /// <summary>
        /// 品詞細分類2
        /// </summary>
        public string PartsOfSpeechSection2
        {
            get { return this.Features[2]; }
        }

        /// <summary>
        /// 品詞細分類3
        /// </summary>
        public string PartsOfSpeechSection3
        {
            get { return this.Features[3]; }
        }

        /// <summary>
        /// 活用形
        /// </summary>
        public string ConjugatedForm
        {
            get { return this.Features[4]; }
        }

        /// <summary>
        /// 活用型
        /// </summary>
        public string Inflection
        {
            get { return this.Features[5]; }
        }

        /// <summary>
        /// 原形
        /// </summary>
        public string OriginalForm
        {
            get { return this.Features[6]; }
        }

        /// <summary>
        /// 読み
        /// </summary>
        public string Reading
        {
            get { return this.Features[7]; }
        }

        /// <summary>
        /// 発音
        /// </summary>
        public string Pronounciation
        {
            get { return this.Features[8]; }
        }
    }

    public abstract class MeCabNodeBase<TNode>
        where TNode : MeCabNodeBase<TNode>
    {
        /// <summary>
        /// 累積コストが最小である、一つ前の形態素
        /// </summary>
        public TNode BestPrev { get; set; }

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

            for (var path = this.LPath; path != null; path = path.LNext)
            {
                os.Append("[Path:");
                os.Append(path.LNode.Id).Append(" ");
                os.Append("(Cost:").Append(path.Cost).Append(")");
                os.Append("(Prob:").Append(path.Prob).Append(")");
                os.Append("]");
            }

            return os.ToString();
        }
    }
}
