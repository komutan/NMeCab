namespace NMeCab.Specialized
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
        public static MeCabIpaDicTagger Create(string dicDir = null,
                                               string[] userDics = null)
        {
            return Create(dicDir,
                          userDics,
                          () => new MeCabIpaDicTagger(),
                          () => new MeCabIpaDicNode(),
                          "IpaDic");
        }
    }
}
