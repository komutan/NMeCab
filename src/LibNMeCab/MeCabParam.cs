//  MeCab -- Yet Another Part-of-Speech and Morphological Analyzer
//
//  Copyright(C) 2001-2006 Taku Kudo <taku@chasen.org>
//  Copyright(C) 2004-2006 Nippon Telegraph and Telephone Corporation
using System;
using System.Collections.Generic;
using System.Text;
using NMeCab.Core;
using System.IO;

namespace NMeCab
{
    public class MeCabParam
    {
        public string DicDir { get; set; }

        public string[] UserDic { get; set; }

        public int MaxGroupingSize { get; set; }

        /// <summary>
        /// 文頭, 文末の素性
        /// </summary>
        public string BosFeature { get; set; }

        public string UnkFeature { get; set; }

        public bool AlloCateSentence { get; set; }

        /// <summary>
        /// コスト値に変換するときのスケーリングファクター
        /// </summary>
        public int CostFactor { get; set; }

        public const float DefaultTheta = 0.75f;

        /// <summary>
        /// ソフト分かち書きの温度パラメータ
        /// </summary>
        public float Theta { get; set; }

        /// <summary>
        /// ラティスレベル(どの程度のラティス情報を解析時に構築するか)
        /// </summary>
        public MeCabLatticeLevel LatticeLevel { get; set; }

        /// <summary>
        /// 部分解析
        /// </summary>
        public bool Partial { get; set; }

        /// <summary>
        /// 出力モード
        /// </summary>
        /// <value>
        /// true: 全出力
        /// false: ベスト解のみ
        /// </value>
        public bool AllMorphs { get; set; }

        //public bool AllocateSentence { get; set; }

        public string OutputFormatType { get; set; }

        public const string DefaultRcFile = "dicrc";

        public string RcFile { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MeCabParam()
        {
            this.Theta = MeCabParam.DefaultTheta;
            this.RcFile = MeCabParam.DefaultRcFile;

            Properties.Settings settings = Properties.Settings.Default;
            this.DicDir = settings.DicDir;
            this.UserDic = this.SplitStringArray(settings.UserDic, ',');
            this.OutputFormatType = settings.OutputFormatType;
        }

        public void LoadDicRC()
        {
            string rc = Path.Combine(this.DicDir, this.RcFile);
            this.Load(rc);
        }

        public void Load(string path)
        {
            IniParser ini = new IniParser();
            ini.Load(path, Encoding.ASCII);

            this.CostFactor = int.Parse(ini["cost-factor"] ?? "0");
            this.BosFeature = ini["bos-feature"];
        }

        private string[] SplitStringArray(string configStr, char separator)
        {
            if (string.IsNullOrEmpty(configStr)) return new string[0];

            string[] ret = configStr.Split(separator);

            for (int i = 0; i < ret.Length; i++)
                ret[i] = ret[i].Trim();

            return ret;
        }
    }
}
