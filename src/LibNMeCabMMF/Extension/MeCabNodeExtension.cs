using System;
using System.Collections.Generic;
using System.Text;

namespace NMeCab.Extension
{
    public static class MeCabNodeExtension
    {
        /// <summary>
        /// 品詞を取得
        /// </summary>
        public static string GetPartsOfSpeech(this MeCabNode node)
        {
            return GetCsvElement(node.Feature, 0);
        }

        /// <summary>
        /// 品詞細分類1を取得
        /// </summary>
        public static string GetPartsOfSpeechSection1(this MeCabNode node)
        {
            return GetCsvElement(node.Feature, 1);
        }

        /// <summary>
        /// 品詞細分類2を取得
        /// </summary>
        public static string GetPartsOfSpeechSection2(this MeCabNode node)
        {
            return GetCsvElement(node.Feature, 2);
        }

        /// <summary>
        /// 品詞細分類3を取得
        /// </summary>
        public static string GetPartsOfSpeechSection3(this MeCabNode node)
        {
            return GetCsvElement(node.Feature, 3);
        }

        /// <summary>
        /// 活用形を取得
        /// </summary>
        public static string GetConjugatedForm(this MeCabNode node)
        {
            return GetCsvElement(node.Feature, 4);
        }

        /// <summary>
        /// 活用型を取得
        /// </summary>
        public static string GetInflection(this MeCabNode node)
        {
            return GetCsvElement(node.Feature, 5);
        }

        /// <summary>
        /// 活用型を取得
        /// </summary>
        public static string GetOriginalForm(this MeCabNode node)
        {
            return GetCsvElement(node.Feature, 6);
        }

        /// <summary>
        /// 読みを取得
        /// </summary>
        public static string GetReading(this MeCabNode node)
        {
            return GetCsvElement(node.Feature, 7);
        }

        /// <summary>
        /// 発音を取得
        /// </summary>
        public static string GetPronounciation(this MeCabNode node)
        {
            return GetCsvElement(node.Feature, 8);
        }

        private static string GetCsvElement(string csvRow, int index)
        {
            if (string.IsNullOrEmpty(csvRow)) return null;

            string[] items = csvRow.Split(',');
            if (items.Length <= index) return null;

            return items[index];
        }
    }
}
