namespace NMeCab
{
    /// <summary>
    /// UniDic（ver2.1.0以後）形式の辞書を使用する場合の形態素解析処理の起点を表します。
    /// </summary>
    public class MeCabUniDic21Tagger : MeCabTaggerBase<MeCabUniDic21Node>
    {
        /// <summary>
        /// コンストラクタ（非公開）
        /// </summary>
        private MeCabUniDic21Tagger()
        { }

        /// <summary>
        /// 使用する辞書ディレクトリ名の初期値
        /// </summary>
        protected override string DefaltDicDir => "UniDic";

        /// <summary>
        /// 形態素解析処理の起点を作成します。
        /// </summary>
        /// <param name="dicDir">使用する辞書のディレクトリへのパス</param>
        /// <param name="userDics">使用するユーザー辞書のファイル名のコレクション</param>
        /// <returns>形態素解析処理の起点</returns>
        public static MeCabUniDic21Tagger Create(string dicDir = null, string[] userDics = null)
        {
            return Create(dicDir,
                          userDics,
                          () => new MeCabUniDic21Tagger(),
                          () => new MeCabUniDic21Node());
        }
    }
}