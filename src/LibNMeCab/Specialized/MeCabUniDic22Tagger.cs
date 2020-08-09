namespace NMeCab.Specialized
{
    /// <summary>
    /// UniDic（ver2.2.0以後）形式の辞書を使用する場合の形態素解析処理の起点を表します。
    /// </summary>
    public class MeCabUniDic22Tagger : MeCabTaggerBase<MeCabUniDic22Node>
    {
        /// <summary>
        /// コンストラクタ（非公開）
        /// </summary>
        private MeCabUniDic22Tagger()
        { }

        /// <summary>
        /// 形態素解析処理の起点を作成します。
        /// </summary>
        /// <param name="dicDir">使用する辞書のディレクトリへのパス</param>
        /// <param name="userDics">使用するユーザー辞書のファイル名のコレクション</param>
        /// <returns>形態素解析処理の起点</returns>
        public static MeCabUniDic22Tagger Create(string dicDir = null,
                                                 string[] userDics = null)
        {
            return Create(dicDir,
                          userDics,
                          () => new MeCabUniDic22Tagger(),
                          () => new MeCabUniDic22Node(),
                          "UniDic");
        }
    }
}