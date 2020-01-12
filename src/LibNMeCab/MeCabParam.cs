//  MeCab -- Yet Another Part-of-Speech and Morphological Analyzer
//
//  Copyright(C) 2001-2006 Taku Kudo <taku@chasen.org>
//  Copyright(C) 2004-2006 Nippon Telegraph and Telephone Corporation
using System;
using System.Collections.Specialized;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using NMeCab.Core;

namespace NMeCab
{
    public class MeCabParam : NameValueCollection
    {
        public string DicDir
        {
            get { return this.GetOrDefalt("dic"); }
            set { this.Set(value); }
        }

        public string[] UserDic
        {
            get { return this.GetTrimedStrAry(','); }
            set { this.Set(value); }
        }

        public int MaxGroupingSize
        {
            get { return this.GetOrDefalt(24); }
            set { this.Set(value); }
        }

        /// <summary>
        /// 文頭, 文末の素性
        /// </summary>
        public string BosFeature
        {
            get { return this.GetOrDefalt(""); }
            set { this.Set(value); }
        }

        public string UnkFeature
        {
            get { return this.GetOrDefalt(""); }
            set { this.Set(value); }
        }

        /// <summary>
        /// コスト値に変換するときのスケーリングファクター
        /// </summary>
        public int CostFactor
        {
            get { return this.GetOrDefalt(0); }
            set { this.Set(value); }
        }

        /// <summary>
        /// ソフト分かち書きの温度パラメータ
        /// </summary>
        public float Theta
        {
            get { return this.GetOrDefalt(0.75f); }
            set { this.Set(value); }
        }

        /// <summary>
        /// ラティスレベル(どの程度のラティス情報を解析時に構築するか)
        /// </summary>
        public MeCabLatticeLevel LatticeLevel
        {
            get { return this.GetOrDefalt(MeCabLatticeLevel.One); }
            set { this.Set(value); }
        }

        /// <summary>
        /// 部分解析
        /// </summary>
        public bool Partial
        {
            get { return this.GetOrDefalt(false); }
            set { this.Set(value); }
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
            get { return this.GetOrDefalt(false); }
            set { this.Set(value); }
        }

        public string OutputFormatType
        {
            get { return this.GetOrDefalt("lattice"); }
            set { this.Set(value); }
        }

        public string RcFile
        {
            get { return this.GetOrDefalt("dicrc"); }
            set { this.Set(value); }
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

        private string GetOrDefalt(string defalt, [CallerMemberName]string name = null)
        {
            return this[name] ?? defalt;
        }

        private int GetOrDefalt(int defalt, [CallerMemberName]string name = null)
        {
            var wrkObj = this.BaseGet(name);
            if (wrkObj == null) return defalt;

            var wrkStr = wrkObj as string;
            if (wrkStr != null) return int.Parse(wrkStr);

            return (int)wrkObj;
        }

        private float GetOrDefalt(float defalt, [CallerMemberName]string name = null)
        {
            var wrkObj = this.BaseGet(name);
            if (wrkObj == null) return defalt;

            var wrkStr = wrkObj as string;
            if (wrkStr != null) return float.Parse(wrkStr);

            return (float)wrkObj;
        }

        private bool GetOrDefalt(bool defalt, [CallerMemberName]string name = null)
        {
            var wrkObj = this.BaseGet(name);
            if (wrkObj == null) return defalt;

            var wrkStr = wrkObj as string;
            if (wrkStr != null) bool.Parse(wrkStr);

            return (bool)wrkObj;
        }

        private TEnum GetOrDefalt<TEnum>(TEnum defalt, [CallerMemberName]string name = null)
            where TEnum : Enum
        {
            var wrkObj = this.BaseGet(name);
            if (wrkObj == null) return defalt;

            var wrkStr = wrkObj as string;
            if (wrkStr != null) return (TEnum)Enum.Parse(typeof(TEnum), wrkStr, true);

            return (TEnum)wrkObj;
        }

        private string[] GetTrimedStrAry(char separator, [CallerMemberName]string name = null)
        {
            string[] ret;

            var wrkObj = this.BaseGet(name);
            if (wrkObj == null) return new string[0];

            var wrkStr = wrkObj as string;
            if (wrkStr != null)
                ret = wrkStr.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries);
            else
                ret = (string[])wrkObj;

            for (int i = 0; i < ret.Length; i++)
                ret[i] = ret[i].Trim();

            return ret;
        }

        private void Set(object value, [CallerMemberName]string name = null)
        {
            this.BaseSet(name, value);
        }
    }
}
