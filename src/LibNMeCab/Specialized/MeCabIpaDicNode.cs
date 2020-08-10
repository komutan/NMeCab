namespace NMeCab.Specialized
{
    /// <summary>
    /// IPA形式の辞書を使用する場合の形態素ノードです。
    /// 素性情報CSVを分解して各項目の情報を取得するプロパティも備えています。
    /// </summary>
    public class MeCabIpaDicNode : MeCabNodeBase<MeCabIpaDicNode>
    {
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
