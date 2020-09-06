//  MeCab -- Yet Another Part-of-Speech and Morphological Analyzer
//
//  Copyright(C) 2001-2006 Taku Kudo <taku@chasen.org>
//  Copyright(C) 2004-2006 Nippon Telegraph and Telephone Corporation

namespace NMeCab
{
    /// <summary>
    /// 形態素ノード間のパスを表します。
    /// </summary>
    /// <typeparam name="TNode">形態素ノードの型</typeparam>
    public class MeCabPath<TNode>
        where TNode : MeCabNodeBase<TNode>
    {
        #region  Const/Field/Property

        /// <summary>
        /// 右側の形態素ノード
        /// </summary>
        public TNode RNode { get; set; }

        /// <summary>
        /// 左側の形態素ノードが同じで、右側の形態素ノードが異なる、別のパス
        /// </summary>
        public MeCabPath<TNode> RNext { get; set; }

        /// <summary>
        /// 左側の形態素ノード
        /// </summary>
        public TNode LNode { get; set; }

        /// <summary>
        /// 右側の形態素ノードが同じで、左側の形態素ノードが異なる、別のパス
        /// </summary>
        public MeCabPath<TNode> LNext { get; set; }

        /// <summary>
        /// 左右の形態素ノード間の接続コスト
        /// </summary>
        public int Cost { get; set; }

        /// <summary>
        /// 周辺確率
        /// </summary>
        public float Prob { get; set; }

        #endregion

        #region Method

        /// <summary>
        /// インスタンスの文字列表現を返します。
        /// </summary>
        /// <returns>文字列表現</returns>
        public override string ToString()
        {
            return $"[Cost:{this.Cost}][Prob:{this.Prob}][LNode:{this.LNode}][RNode;{this.RNode}]";
        }

        #endregion
    }
}
