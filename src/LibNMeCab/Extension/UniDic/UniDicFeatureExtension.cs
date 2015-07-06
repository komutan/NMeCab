#if EXT
using System;
using System.Collections.Generic;
using System.Text;

namespace NMeCab.Extension.UniDic
{
    public static class UniDicFeatureExtension
    {
        /// <summary>
        /// 品詞大分類を取得
        /// </summary>
        public static string GetPos1(this MeCabNode node)
        {
            return node.GetFeatureItem(0);
        }

        /// <summary>
        /// 品詞中分類を取得
        /// </summary>
        public static string GetPos2(this MeCabNode node)
        {
            return node.GetFeatureItem(1);
        }
        
        /// <summary>
        /// 品詞小分類を取得
        /// </summary>
        public static string GetPos3(this MeCabNode node)
        {
            return node.GetFeatureItem(2);
        }
        
        /// <summary>
        /// 品詞細分類を取得
        /// </summary>
        public static string GetPos4(this MeCabNode node)
        {
            return node.GetFeatureItem(3);
        }
        
        /// <summary>
        /// 活用型を取得
        /// </summary>
        public static string GetCType(this MeCabNode node)
        {
            return node.GetFeatureItem(4);
        }
        
        /// <summary>
        /// 活用形を取得
        /// </summary>
        public static string GetCForm(this MeCabNode node)
        {
            return node.GetFeatureItem(5);
        }
        
        /// <summary>
        /// 語彙素読みを取得
        /// </summary>
        public static string GetLForm(this MeCabNode node)
        {
            return node.GetFeatureItem(6);
        }
        
        /// <summary>
        /// 語彙素（語彙素表記+ 語彙素細分類）を取得
        /// </summary>
        public static string GetLemma(this MeCabNode node)
        {
            return node.GetFeatureItem(7);
        }
        
        /// <summary>
        /// 書字形出現形を取得
        /// </summary>
        public static string GetOrth(this MeCabNode node)
        {
            return node.GetFeatureItem(8);
        }
        
        /// <summary>
        /// 発音形出現形を取得
        /// </summary>
        public static string GetPron(this MeCabNode node)
        {
            return node.GetFeatureItem(9);
        }
        
        /// <summary>
        /// 書字形基本形を取得
        /// </summary>
        public static string GetOrthBase(this MeCabNode node)
        {
            return node.GetFeatureItem(10);
        }
        
        /// <summary>
        /// 発音形基本形を取得
        /// </summary>
        public static string GetPronBase(this MeCabNode node)
        {
            return node.GetFeatureItem(11);
        }
        
        /// <summary>
        /// 語種を取得
        /// </summary>
        public static string GetGoshu(this MeCabNode node)
        {
            return node.GetFeatureItem(12);
        }
        
        /// <summary>
        /// 語頭変化型を取得
        /// </summary>
        public static string GetIType(this MeCabNode node)
        {
            return node.GetFeatureItem(13);
        }
        
        /// <summary>
        /// 語頭変化形を取得
        /// </summary>
        public static string GetIForm(this MeCabNode node)
        {
            return node.GetFeatureItem(14);
        }

        /// <summary>
        /// 語末変化型を取得
        /// </summary>
        public static string GetFType(this MeCabNode node)
        {
            return node.GetFeatureItem(15);
        }

        /// <summary>
        /// 語末変化形を取得
        /// </summary>
        public static string GetFForm(this MeCabNode node)
        {
            return node.GetFeatureItem(16);
        }

        public static string GetKana(this MeCabNode node)
        {
            return node.GetFeatureItem(17);
        }
        
        public static string GetKanaBase(this MeCabNode node)
        {
            return node.GetFeatureItem(18);
        }
        
        public static string GetForm(this MeCabNode node)
        {
            return node.GetFeatureItem(19);
        }
        
        public static string GetFormBase(this MeCabNode node)
        {
            return node.GetFeatureItem(20);
        }
        
        public static string GetIConType(this MeCabNode node)
        {
            return node.GetFeatureItem(21);
        }
        
        public static string GetFConType(this MeCabNode node)
        {
            return node.GetFeatureItem(22);
        }
        
        public static string GetAType(this MeCabNode node)
        {
            return node.GetFeatureItem(23);
        }
        
        public static string GetAConType(this MeCabNode node)
        {
            return node.GetFeatureItem(24);
        }
        
        public static string GetAModType(this MeCabNode node)
        {
            return node.GetFeatureItem(25);
        }
    }
}
#endif