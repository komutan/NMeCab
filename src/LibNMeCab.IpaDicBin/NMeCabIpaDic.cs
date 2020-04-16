using System;
using System.IO;

namespace NMeCab
{
    /// <summary>
    /// NMeCabにIPA辞書を同梱したNuGetパッケージのユーティリティクラスです。
    /// </summary>
    public static class NMeCabIpaDic
    {
        /// <summary>
        /// NuGetパッケージに同梱しているIPA辞書を使用し、形態素解析処理の起点を作成します。
        /// </summary>
        /// <returns>形態素解析処理の起点</returns>
        public static MeCabIpaDicTagger CreateTagger()
        {
            var dicDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "IpaDic");
            return MeCabIpaDicTagger.Create(dicDir);
        }
    }
}
