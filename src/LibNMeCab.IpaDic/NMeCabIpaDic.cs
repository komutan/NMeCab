using System;
using System.IO;

namespace NMeCab.IpaDic
{
    /// <summary>
    /// 
    /// </summary>
    public static class NMeCabIpaDic
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static MeCabIpaDicTagger CreateTagger()
        {
            var dicDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "IpaDic");
            return MeCabIpaDicTagger.Create(dicDir);
        }
    }
}
