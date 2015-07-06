#if EXT
using System;
using System.Collections.Generic;
using System.Text;

namespace NMeCab.Extension
{
    public static class FeatureExtension
    {
        /// <summary>
        /// 素性情報の指定番目の値を取得する
        /// </summary>
        public static string GetFeatureItem(this MeCabNode node, int index)
        {
            return node.Feature.GetCsvItem(index);
        }

        /// <summary>
        /// CSV文字列の指定番目の値を取得する
        /// </summary>
        private static string GetCsvItem(this string csvRow, int index)
        {
            if (csvRow == null) return null;

            string[] items = csvRow.Split(',');
            if (items.Length <= index) return null;

            return items[index];
        }
    }
}
#endif