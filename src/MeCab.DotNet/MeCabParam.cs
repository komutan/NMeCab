//  MeCab -- Yet Another Part-of-Speech and Morphological Analyzer
//
//  Copyright(C) 2001-2006 Taku Kudo <taku@chasen.org>
//  Copyright(C) 2004-2006 Nippon Telegraph and Telephone Corporation
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using MeCab.Core;

namespace MeCab
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

#if NETSTANDARD1_3
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="dicdir">dicフォルダへのパス</param>
        /// <remarks>netstandard1.3では、デフォルトのdicフォルダパスを自動的に構成できないため、
        /// 引数に指定する必要があります。</remarks>
        public MeCabParam(string dicdir)
#else
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MeCabParam()
#endif
        {
            this.Theta = MeCabParam.DefaultTheta;
            this.RcFile = MeCabParam.DefaultRcFile;

#if !NETSTANDARD1_3
            // In unit test context, the current folder path is set unstable.
            var assemblyFolderPath = Path.GetDirectoryName(
                this.GetType().Assembly.Location);
            var inPackagePath = CombinePath(assemblyFolderPath!, "..", "InPackage");
            var dicdir = File.Exists(inPackagePath) ?
                CombinePath(assemblyFolderPath, "..", "..", "content", "dic") :
                CombinePath(assemblyFolderPath, "dic");
#endif

            this.DicDir = dicdir;
            this.UserDic = new string[0];
            this.OutputFormatType = "lattice";
        }

        private static string CombinePath(params string[] paths)
        {
#if NET20 || NET35
            var r = paths[0];
            for (var index = 1; index < paths.Length; index++)
            {
                r = Path.Combine(r, paths[index]);
            }
            return r;
#else
            return Path.Combine(paths);
#endif
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
    }
}
