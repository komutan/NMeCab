namespace NMeCab
{
    /// <summary>
    /// 形態素解析処理のパラメータを表します。
    /// </summary>
    public class MeCabParam
    {
        /// <summary>
        /// ラティスレベル(どの程度のラティス情報を解析時に構築するか)
        /// </summary>
        public MeCabLatticeLevel LatticeLevel { get; set; }

        /// <summary>
        /// ソフト分かち書きの温度パラメータの初期値
        /// </summary>
        public const float DefaltTheta = 0.75f;

        /// <summary>
        /// ソフト分かち書きの温度パラメータ
        /// </summary>
        public float Theta { get; set; } = DefaltTheta;

        /// <summary>
        /// 未知語の文字数の最大値の初期値
        /// </summary>
        public const int DefaltMaxGroupingSize = 24;

        /// <summary>
        /// 未知語の文字数の最大値
        /// </summary>
        public int MaxGroupingSize { get; set; } = DefaltMaxGroupingSize;
    }
}
