using System.Collections.Generic;
using NMeCab.Core;

namespace NMeCab
{
    /// <summary>
    /// IPA形式の辞書を使用する場合の形態素解析処理の起点を表します。
    /// </summary>
    public class MeCabIpaDicTagger : MeCabTaggerBase<MeCabIpaDicNode>
    {
        /// <summary>
        /// コンストラクタ（非公開）
        /// </summary>
        private MeCabIpaDicTagger()
        { }

        /// <summary>
        /// 形態素解析処理の起点を作成します。
        /// </summary>
        /// <param name="dicDir">使用する辞書のディレクトリへのパス</param>
        /// <param name="userDics">使用するユーザー辞書のファイル名のコレクション</param>
        /// <returns>形態素解析処理の起点</returns>
        public static MeCabIpaDicTagger Create(string dicDir, string[] userDic = null)
        {
            return Create(dicDir, userDic, () => new MeCabIpaDicTagger());
        }

        /// <summary>
        /// 形態素ノードインスタンス生成メソッドです。（内部用）
        /// </summary>
        /// <returns>形態素ノード</returns>
        protected override MeCabIpaDicNode CreateNewNode()
        {
            return new MeCabIpaDicNode();
        }
    }

    /// <summary>
    /// IPA形式の辞書を使用する場合の形態素ノードです。
    /// 素性情報CSVを分解して各項目の情報を取得するプロパティも備えています。
    /// </summary>
    public class MeCabIpaDicNode : MeCabNodeBase<MeCabIpaDicNode>
    {
        private string[] features = null;

        private string GetFeatureAt(int index)
        {
            if (this.features == null)
            {
                var featureCsv = this.Feature;
                if (featureCsv == null) return "";
                this.features = StrUtils.SplitCsvRow(featureCsv, 9, 16);
            }

            if (index >= this.features.Length)
                return "";

            return this.features[index];
        }

        /// <summary>
        /// 品詞
        /// </summary>
        public string PartsOfSpeech
        {
            get { return this.GetFeatureAt(0); }
        }

        /// <summary>
        /// 品詞細分類1
        /// </summary>
        public string PartsOfSpeechSection1
        {
            get { return this.GetFeatureAt(1); }
        }

        /// <summary>
        /// 品詞細分類2
        /// </summary>
        public string PartsOfSpeechSection2
        {
            get { return this.GetFeatureAt(2); }
        }

        /// <summary>
        /// 品詞細分類3
        /// </summary>
        public string PartsOfSpeechSection3
        {
            get { return this.GetFeatureAt(3); }
        }

        /// <summary>
        /// 活用形
        /// </summary>
        public string ConjugatedForm
        {
            get { return this.GetFeatureAt(4); }
        }

        /// <summary>
        /// 活用型
        /// </summary>
        public string Inflection
        {
            get { return this.GetFeatureAt(5); }
        }

        /// <summary>
        /// 原形
        /// </summary>
        public string OriginalForm
        {
            get { return this.GetFeatureAt(6); }
        }

        /// <summary>
        /// 読み
        /// </summary>
        public string Reading
        {
            get { return this.GetFeatureAt(7); }
        }

        /// <summary>
        /// 発音
        /// </summary>
        public string Pronounciation
        {
            get { return this.GetFeatureAt(8); }
        }
    }
}
