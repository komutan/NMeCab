using System;

namespace NMeCab
{
    public class MeCabParam
    {
        /// <summary>
        ///  N-BEST解を出力
        /// </summary>
        public bool NBest { get; set; } = false;

        /// <summary>
        /// 全形態素を出力
        /// </summary>
        public bool AllMorphs { get; set; } = false;

        /// <summary>
        /// 周辺確率を出力
        /// </summary>
        public bool MarginalProbe { get; set; } = false;

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
