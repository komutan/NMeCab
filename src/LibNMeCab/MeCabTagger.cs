//  MeCab -- Yet Another Part-of-Speech and Morphological Analyzer
//
//  Copyright(C) 2001-2006 Taku Kudo <taku@chasen.org>
//  Copyright(C) 2004-2006 Nippon Telegraph and Telephone Corporation

namespace NMeCab
{
    /// <summary>
    /// 形態素解析処理の起点を表します。使用する辞書の形式は限定しません。
    /// </summary>
    public class MeCabTagger : MeCabTaggerBase<MeCabNode>
    {
        /// <summary>
        /// コンストラクタ（非公開）
        /// </summary>
        private MeCabTagger()
        { }

        /// <summary>
        /// 使用する辞書ディレクトリ名の初期値
        /// </summary>
        protected override string DefaltDicDir => "dic";

        /// <summary>
        /// 形態素解析処理の起点を作成します。
        /// </summary>
        /// <param name="dicDir">使用する辞書のディレクトリへのパス</param>
        /// <param name="userDics">使用するユーザー辞書のファイル名のコレクション</param>
        /// <returns>形態素解析処理の起点</returns>
        public static MeCabTagger Create(string dicDir = null, string[] userDics = null)
        {
            return Create(dicDir,
                          userDics,
                          () => new MeCabTagger(),
                          () => new MeCabNode());
        }
    }
}
