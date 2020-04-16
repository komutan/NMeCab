//  MeCab -- Yet Another Part-of-Speech and Morphological Analyzer
//
//  Copyright(C) 2001-2006 Taku Kudo <taku@chasen.org>
//  Copyright(C) 2004-2006 Nippon Telegraph and Telephone Corporation
namespace NMeCab
{
    /// <summary>
    /// 形態素ノードの種類
    /// </summary>
    public enum MeCabNodeStat
    {
        /// <summary>
        /// 通常ノード
        /// </summary>
        Nor = 0,
        /// <summary>
        /// 未知語ノード
        /// </summary>
        Unk = 1,
        /// <summary>
        /// 文頭ノード
        /// </summary>
        Bos = 2,
        /// <summary>
        /// 文末ノード
        /// </summary>
        Eos = 3
    }
}