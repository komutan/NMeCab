﻿using NMeCab.Core;

namespace NMeCab
{
    /// <summary>
    /// UniDic（ver2.2.0以後）形式の辞書を使用する場合の形態素ノードです。
    /// 素性情報CSVを分解して各項目の情報を取得するプロパティも備えています。
    /// </summary>
    public class MeCabUniDic22Node : MeCabNodeBase<MeCabUniDic22Node>
    {
        private string[] features = null;

        private string GetFeatureAt(int index)
        {
            if (this.features == null)
            {
                var featureCsv = this.Feature;
                if (featureCsv == null) return "";
                this.features = StrUtils.SplitCsvRow(featureCsv, 29, 16);
            }

            if (index >= this.features.Length)
                return "";

            return this.features[index];
        }

        /// <summary>
        /// 品詞大分類を取得
        /// </summary>
        public string Pos1
        {
            get { return this.GetFeatureAt(0); }
        }

        /// <summary>
        /// 品詞中分類を取得
        /// </summary>
        public string Pos2
        {
            get { return this.GetFeatureAt(1); }
        }

        /// <summary>
        /// 品詞小分類を取得
        /// </summary>
        public string Pos3
        {
            get { return this.GetFeatureAt(2); }
        }

        /// <summary>
        /// 品詞細分類を取得
        /// </summary>
        public string Pos4
        {
            get { return this.GetFeatureAt(3); }
        }

        /// <summary>
        /// 活用型を取得
        /// </summary>
        public string CType
        {
            get { return this.GetFeatureAt(4); }
        }

        /// <summary>
        /// 活用形を取得
        /// </summary>
        public string CForm
        {
            get { return this.GetFeatureAt(5); }
        }

        /// <summary>
        /// 語彙素読みを取得
        /// </summary>
        public string LForm
        {
            get { return this.GetFeatureAt(6); }
        }

        /// <summary>
        /// 語彙素（語彙素表記＋語彙素細分類）を取得
        /// </summary>
        public string Lemma
        {
            get { return this.GetFeatureAt(7); }
        }

        /// <summary>
        /// 書字形出現形を取得
        /// </summary>
        public string Orth
        {
            get { return this.GetFeatureAt(8); }
        }

        /// <summary>
        /// 発音形出現形を取得
        /// </summary>
        public string Pron
        {
            get { return this.GetFeatureAt(9); }
        }

        /// <summary>
        /// 書字形基本形を取得
        /// </summary>
        public string OrthBase
        {
            get { return this.GetFeatureAt(10); }
        }

        /// <summary>
        /// 発音形基本形を取得
        /// </summary>
        public string PronBase
        {
            get { return this.GetFeatureAt(11); }
        }

        /// <summary>
        /// 語種を取得
        /// </summary>
        public string Goshu
        {
            get { return this.GetFeatureAt(12); }
        }

        /// <summary>
        /// 語頭変化型を取得
        /// </summary>
        public string IType
        {
            get { return this.GetFeatureAt(13); }
        }

        /// <summary>
        /// 語頭変化形を取得
        /// </summary>
        public string IForm
        {
            get { return this.GetFeatureAt(14); }
        }

        /// <summary>
        /// 語末変化型を取得
        /// </summary>
        public string FType
        {
            get { return this.GetFeatureAt(15); }
        }

        /// <summary>
        /// 語末変化形を取得
        /// </summary>
        public string FForm
        {
            get { return this.GetFeatureAt(16); }
        }

        /// <summary>
        /// 語頭変化結合形を取得
        /// </summary>
        public string IConType
        {
            get { return this.GetFeatureAt(17); }
        }

        /// <summary>
        /// 語末変化結合形を取得
        /// </summary>
        public string FConType
        {
            get { return this.GetFeatureAt(18); }
        }

        /// <summary>
        /// 語彙素類を取得
        /// </summary>
        /// <remarks>
        /// この項目の「英語」の名前は、
        /// UniDic 2.2.0 や 2.3.0 の配布物に含まれる dicrc ファイルでは「type」となっているが、
        /// UniDic の FAQ (https://unidic.ninjal.ac.jp/faq#col_name) には「lType」と記載されている。
        /// </remarks>
        public string LType
        {
            get { return this.GetFeatureAt(19); }
        }

        /// <summary>
        /// 仮名形出現形を取得
        /// </summary>
        public string Kana
        {
            get { return this.GetFeatureAt(20); }
        }

        /// <summary>
        /// 仮名形基本形を取得
        /// </summary>
        public string KanaBase
        {
            get { return this.GetFeatureAt(21); }
        }

        /// <summary>
        /// 語形出現形を取得
        /// </summary>
        public string Form
        {
            get { return this.GetFeatureAt(22); }
        }

        /// <summary>
        /// 語形基本形を取得
        /// </summary>
        public string FormBase
        {
            get { return this.GetFeatureAt(23); }
        }

        /// <summary>
        /// アクセント型を取得
        /// </summary>
        public string AType
        {
            get { return this.GetFeatureAt(24); }
        }

        /// <summary>
        /// アクセント結合型を取得
        /// </summary>
        public string AConType
        {
            get { return this.GetFeatureAt(25); }
        }

        /// <summary>
        /// アクセント修飾型を取得
        /// </summary>
        public string AModType
        {
            get { return this.GetFeatureAt(26); }
        }

        /// <summary>
        /// 語彙表IDを取得
        /// </summary>
        public string LId
        {
            get { return this.GetFeatureAt(27); }
        }

        /// <summary>
        /// 語彙素IDを取得
        /// </summary>
        public string LemmaId
        {
            get { return this.GetFeatureAt(28); }
        }
    }
}