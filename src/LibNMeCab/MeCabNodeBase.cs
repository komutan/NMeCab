//  MeCab -- Yet Another Part-of-Speech and Morphological Analyzer
//
//  Copyright(C) 2001-2006 Taku Kudo <taku@chasen.org>
//  Copyright(C) 2004-2006 Nippon Telegraph and Telephone Corporation

namespace NMeCab
{
    /// <summary>
    /// 形態素ノードを表す抽象基底クラスです。
    /// </summary>
    /// <typeparam name="TNode">連結する形態素ノードの具象型</typeparam>
    public abstract class MeCabNodeBase<TNode> : MeCabNodeSuperBase
        where TNode : MeCabNodeBase<TNode>
    {
        /// <summary>
        /// 一つ前の形態素
        /// </summary>
        public TNode Prev { get; set; }

        /// <summary>
        /// 一つ後の形態素
        /// </summary>
        public TNode Next { get; set; }

        /// <summary>
        /// 同じ開始位置で始まる形態素
        /// </summary>
        public TNode BNext { get; set; }

        /// <summary>
        /// 同じ位置で終わる形態素
        /// </summary>
        public TNode ENext { get; set; }

        /// <summary>
        /// 前の形態素の候補へのパス
        /// </summary>
        public MeCabPath<TNode> LPath { get; set; }

        /// <summary>
        /// 次の形態素の候補へのパス
        /// </summary>
        public MeCabPath<TNode> RPath { get; set; }
    }
}
