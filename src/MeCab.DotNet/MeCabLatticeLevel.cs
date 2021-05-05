//  MeCab -- Yet Another Part-of-Speech and Morphological Analyzer
//
//  Copyright(C) 2001-2006 Taku Kudo <taku@chasen.org>
//  Copyright(C) 2004-2006 Nippon Telegraph and Telephone Corporation
namespace MeCab
{
    /// <summary>
    /// ラティスレベル(どの程度のラティス情報を解析時に構築するか)
    /// </summary>
    public enum MeCabLatticeLevel
    {
        /// <summary>
        /// 0: 最適解のみが出力可能なレベル (デフォルト, 高速) 
        /// </summary>
        Zero = 0,
        /// <summary>
        /// 1: N-best 解が出力可能なレベル (中速) 
        /// </summary>
        One = 1,
        /// <summary>
        /// 2: ソフトわかち書きが可能なレベル (低速)
        /// </summary>
        Two = 2
    }
}
