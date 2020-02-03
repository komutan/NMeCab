using System;

namespace NMeCab
{
    public class MeCabParam
    {
        /// <summary>
        /// ラティスレベル(どの程度のラティス情報を解析時に構築するか)
        /// </summary>
        public MeCabLatticeLevel LatticeLevel { get; set; }     

        /// <summary>
        /// ソフト分かち書きの温度パラメータ
        /// </summary>
        public float Theta { get; set; } = 0.75f;

        /// <summary>
        /// 未知語の文字数の最大値
        /// </summary>
        public int MaxGroupingSize { get; set; } = 24;

        /// <summary>
        /// 未知語の素性
        /// </summary>
        public string UnkFeature { get; set; } = null;
    }
}
