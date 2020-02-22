//  MeCab -- Yet Another Part-of-Speech and Morphological Analyzer
//
//  Copyright(C) 2001-2006 Taku Kudo <taku@chasen.org>
//  Copyright(C) 2004-2006 Nippon Telegraph and Telephone Corporation
namespace NMeCab
{
    /// <summary>
    /// 辞書の種別
    /// </summary>
    public enum DictionaryType
    {
        /// <summary>
        /// システム辞書
        /// </summary>
        Sys = 0,
        /// <summary>
        /// ユーザー辞書
        /// </summary>
        Usr = 1,
        /// <summary>
        /// 未定義
        /// </summary>
        Unk = 2
    }
}
