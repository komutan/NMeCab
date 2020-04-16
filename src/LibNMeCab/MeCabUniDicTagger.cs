namespace NMeCab
{
    /// <summary>
    /// UniDic形式の辞書を使用する場合の形態素解析処理の起点を表します。
    /// </summary>
    public class MeCabUniDicTagger : MeCabTaggerBase<MeCabUniDicNode>
    {
        /// <summary>
        /// コンストラクタ（非公開）
        /// </summary>
        private MeCabUniDicTagger()
        { }

        /// <summary>
        /// 形態素解析処理の起点を作成します。
        /// </summary>
        /// <param name="dicDir">使用する辞書のディレクトリへのパス</param>
        /// <param name="userDics">使用するユーザー辞書のファイル名のコレクション</param>
        /// <returns>形態素解析処理の起点</returns>
        public static MeCabUniDicTagger Create(string dicDir, string[] userDics = null)
        {
            return Create(dicDir, userDics, () => new MeCabUniDicTagger());
        }

        /// <summary>
        /// 形態素ノードインスタンス生成メソッドです。（内部用）
        /// </summary>
        /// <returns>形態素ノード</returns>
        protected override MeCabUniDicNode CreateNewNode()
        {
            return new MeCabUniDicNode();
        }
    }
}