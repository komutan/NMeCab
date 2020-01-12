//  MeCab -- Yet Another Part-of-Speech and Morphological Analyzer
//
//  Copyright(C) 2001-2006 Taku Kudo <taku@chasen.org>
//  Copyright(C) 2004-2006 Nippon Telegraph and Telephone Corporation
using System;
using System.Collections.Generic;
using System.Text;
using NMeCab.Core;
using System.IO;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;

namespace NMeCab
{
    public class MeCabParam : NameValueCollection
    {
        public string DicDir
        {
            get { return this[nameof(DicDir)] ?? "dic"; }
            set { this[nameof(DicDir)] = value; }
        }

        public string[] UserDic
        {
            get { return this[nameof(UserDic)].SplitAndTrim(','); }
            set { this[nameof(UserDic)] = string.Join(",", value); }
        }

        public int MaxGroupingSize
        {
            get { return this[nameof(MaxGroupingSize)].ToInt(defalt: 24); }
            set { this[nameof(MaxGroupingSize)] = value.ToString(); }
        }

        /// <summary>
        /// 文頭, 文末の素性
        /// </summary>
        public string BosFeature
        {
            get { return this[nameof(BosFeature)]; }
            set { this[nameof(BosFeature)] = value; }
        }

        public string UnkFeature
        {
            get { return this[nameof(UnkFeature)]; }
            set { this[nameof(UnkFeature)] = value; }
        }


        /// <summary>
        /// コスト値に変換するときのスケーリングファクター
        /// </summary>
        public int CostFactor
        {
            get { return this[nameof(CostFactor)].ToInt(defalt: 0); }
            set { this[nameof(CostFactor)] = value.ToString(); }
        }

        /// <summary>
        /// ソフト分かち書きの温度パラメータ
        /// </summary>
        public float Theta
        {
            get { return this[nameof(Theta)].ToFloat(defalt: 0.75f); }
            set { this[nameof(Theta)] = value.ToString(); }
        }

        /// <summary>
        /// ラティスレベル(どの程度のラティス情報を解析時に構築するか)
        /// </summary>
        public MeCabLatticeLevel LatticeLevel
        {
            get { return this[nameof(LatticeLevel)].ToEnum<MeCabLatticeLevel>(defalt: MeCabLatticeLevel.One); }
            set { this[nameof(LatticeLevel)] = value.ToString(); }
        }

        /// <summary>
        /// 部分解析
        /// </summary>
        public bool Partial
        {
            get { return this[nameof(Partial)].ToBool(defalt: false); }
            set { this[nameof(Partial)] = value.ToString(); }
        }

        /// <summary>
        /// 出力モード
        /// </summary>
        /// <value>
        /// true: 全出力
        /// false: ベスト解のみ
        /// </value>
        public bool AllMorphs
        {
            get { return this[nameof(AllMorphs)].ToBool(defalt: false); }
            set { this[nameof(AllMorphs)] = value.ToString(); }
        }

        public string OutputFormatType
        {
            get { return this[nameof(OutputFormatType)] ?? "lattice"; }
            set { this[nameof(OutputFormatType)] = value; }
        }

        public string RcFile
        {
            get { return this[nameof(RcFile)] ?? "dicrc"; }
            set { this[nameof(RcFile)] = value; }
        }

        public void LoadDicRC()
        {
            var rc = Path.Combine(this.DicDir, this.RcFile);
            if (!File.Exists(rc)) return;

            var ini = new IniParser();
            ini.Load(this, rc, Encoding.ASCII);
        }

        public void Load(NameValueCollection conf, bool overwrites = true)
        {
            foreach (var key in conf.AllKeys)
            {
                if (!overwrites && this[key] != null) continue;

                this[key] = conf[key];
            }
        }

        private string GetConfig([CallerMemberName]string name = null, string defalt = null)
        {
            var wrk = this[name];
            if (!string.IsNullOrEmpty(wrk)) return wrk;

            return defalt;
        }
    }
}
