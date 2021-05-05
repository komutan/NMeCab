#if NET35 || NET40 || NET45 || NETSTANDARD1_3 || NETSTANDARD2_0 || NETSTANDARD2_1 || NET5_0
using MeCab;
using System;
using System.Collections.Generic;
using System.Text;

namespace MeCab.Extension.IpaDic
{
    public static class IpaDicFeatureExtension
    {
        /// <summary>
        /// 品詞を取得
        /// </summary>
        public static string GetPartsOfSpeech(this MeCabNode node)
        {
            return node.GetFeatureItem(0);
        }

        /// <summary>
        /// 品詞細分類1を取得
        /// </summary>
        public static string GetPartsOfSpeechSection1(this MeCabNode node)
        {
            return node.GetFeatureItem(1);
        }

        /// <summary>
        /// 品詞細分類2を取得
        /// </summary>
        public static string GetPartsOfSpeechSection2(this MeCabNode node)
        {
            return node.GetFeatureItem(2);
        }

        /// <summary>
        /// 品詞細分類3を取得
        /// </summary>
        public static string GetPartsOfSpeechSection3(this MeCabNode node)
        {
            return node.GetFeatureItem(3);
        }

        /// <summary>
        /// 活用形を取得
        /// </summary>
        public static string GetConjugatedForm(this MeCabNode node)
        {
            return node.GetFeatureItem(4);
        }

        /// <summary>
        /// 活用型を取得
        /// </summary>
        public static string GetInflection(this MeCabNode node)
        {
            return node.GetFeatureItem(5);
        }

        /// <summary>
        /// 活用型を取得
        /// </summary>
        public static string GetOriginalForm(this MeCabNode node)
        {
            return node.GetFeatureItem(6);
        }

        /// <summary>
        /// 読みを取得
        /// </summary>
        public static string GetReading(this MeCabNode node)
        {
            return node.GetFeatureItem(7);
        }

        /// <summary>
        /// 発音を取得
        /// </summary>
        public static string GetPronounciation(this MeCabNode node)
        {
            return node.GetFeatureItem(8);
        }
    }
}

#endif