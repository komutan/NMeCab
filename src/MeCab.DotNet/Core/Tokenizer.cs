//  MeCab -- Yet Another Part-of-Speech and Morphological Analyzer
//
//  Copyright(C) 2001-2006 Taku Kudo <taku@chasen.org>
//  Copyright(C) 2004-2006 Nippon Telegraph and Telephone Corporation
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using MeCab;
#if NeedId
using System.Threading;
#endif

namespace MeCab.Core
{
    public class Tokenizer : IDisposable
    {
        #region Const

        private const string SysDicFile = "sys.dic";
        private const string UnkDicFile = "unk.dic";
        private const int DAResultSize = 512;
        private const int DefaltMaxGroupingSize = 24;
        private const string BosKey = "BOS/EOS";

        #endregion

        #region Field

        private MeCabDictionary[] dic;
        private readonly MeCabDictionary unkDic = new MeCabDictionary();
        private string bosFeature;
        private string unkFeature;
        private Token[][] unkTokens;
        private CharInfo space;
        private readonly CharProperty property = new CharProperty();
        private int maxGroupingSize;
#if NeedId
        [ThreadStaticAttribute]
        private static uint id;
#endif

        #endregion

        #region Open/Clear

        public void Open(MeCabParam param)
        {
            this.dic = new MeCabDictionary[param.UserDic.Length + 1];

            string prefix = param.DicDir;

            this.property.Open(prefix);

            this.unkDic.Open(Path.Combine(prefix, UnkDicFile));
            if (this.unkDic.Type != DictionaryType.Unk)
                throw new MeCabInvalidFileException("not a unk dictionary", this.unkDic.FileName);

            MeCabDictionary sysDic = new MeCabDictionary();
            sysDic.Open(Path.Combine(prefix, SysDicFile));
            if (sysDic.Type != DictionaryType.Sys)
                throw new MeCabInvalidFileException("not a system dictionary", sysDic.FileName);
            this.dic[0] = sysDic;

            for (int i = 0; i < param.UserDic.Length; i++)
            {
                MeCabDictionary d = new MeCabDictionary();
                d.Open(Path.Combine(prefix, param.UserDic[i]));
                if (d.Type != DictionaryType.Usr)
                    throw new MeCabInvalidFileException("not a user dictionary", d.FileName);
                if (!sysDic.IsCompatible(d))
                    throw new MeCabInvalidFileException("incompatible dictionary", d.FileName);
                this.dic[i + 1] = d;
            }

            this.unkTokens = new Token[this.property.Size][];
            for (int i = 0; i < this.unkTokens.Length; i++)
            {
                string key = this.property.Name(i);
                DoubleArray.ResultPair n = this.unkDic.ExactMatchSearch(key);
                if (n.Value == -1)
                    throw new MeCabInvalidFileException("cannot find UNK category: " + key, this.unkDic.FileName);
                this.unkTokens[i] = this.unkDic.GetToken(n);
            }

            this.space = this.property.GetCharInfo(' ');

            this.bosFeature = param.BosFeature;
            this.unkFeature = param.UnkFeature;

            this.maxGroupingSize = param.MaxGroupingSize;
            if (this.maxGroupingSize <= 0) this.maxGroupingSize = DefaltMaxGroupingSize;
        }

#if NeedId
        public void Clear()
        {
            Tokenizer.id = 0u;
        }
#endif

        #endregion

        #region Lookup

        public unsafe MeCabNode Lookup(char* begin, char* end)
        {
            CharInfo cInfo;
            MeCabNode resultNode = null;
            int cLen;

            if (end - begin > ushort.MaxValue) end = begin + ushort.MaxValue;
            char* begin2 = property.SeekToOtherType(begin, end, this.space, &cInfo, &cLen);

            DoubleArray.ResultPair* daResults = stackalloc DoubleArray.ResultPair[DAResultSize];

            foreach (MeCabDictionary it in this.dic)
            {
                int n = it.CommonPrefixSearch(begin2, (int)(end - begin2), daResults, DAResultSize);

                for (int i = 0; i < n; i++)
                {
                    Token[] token = it.GetToken(daResults[i]);
                    for (int j = 0; j < token.Length; j++)
                    {
                        MeCabNode newNode = this.GetNewNode();
                        this.ReadNodeInfo(it, token[j], newNode);
                        //newNode.Token = token[j];
                        newNode.Length = daResults[i].Length;
                        newNode.RLength = (int)(begin2 - begin) + daResults[i].Length;
                        newNode.Surface = new string(begin2, 0, daResults[i].Length);
                        newNode.Stat = MeCabNodeStat.Nor;
                        newNode.CharType = cInfo.DefaultType;
                        newNode.BNext = resultNode;
                        resultNode = newNode;
                    }
                }
            }

            if (resultNode != null && !cInfo.Invoke) return resultNode;

            char* begin3 = begin2 + 1;
            char* groupBegin3 = null;

            if (begin3 > end)
            {
                this.AddUnknown(ref resultNode, cInfo, begin, begin2, begin3);
                return resultNode;
            }

            if (cInfo.Group)
            {
                char* tmp = begin3;
                CharInfo fail;
                begin3 = this.property.SeekToOtherType(begin3, end, cInfo, &fail, &cLen);
                if (cLen <= maxGroupingSize) this.AddUnknown(ref resultNode, cInfo, begin, begin2, begin3);
                groupBegin3 = begin3;
                begin3 = tmp;
            }

            for (int i = 1; i <= cInfo.Length; i++)
            {
                if (begin3 > end) break;
                if (begin3 == groupBegin3) continue;
                cLen = i;
                this.AddUnknown(ref resultNode, cInfo, begin, begin2, begin3);
                if (!cInfo.IsKindOf(this.property.GetCharInfo(*begin3))) break;
                begin3 += 1;
            }

            if (resultNode == null) this.AddUnknown(ref resultNode, cInfo, begin, begin2, begin3);

            return resultNode;
        }

        private void ReadNodeInfo(MeCabDictionary dic, Token token, MeCabNode node)
        {
            node.LCAttr = token.LcAttr;
            node.RCAttr = token.RcAttr;
            node.PosId = token.PosId;
            node.WCost = token.WCost;
            //node.Token = token;
            //node.Feature = dic.GetFeature(token); //この段階では素性情報を取得しない
            node.SetFeature(token.Feature, dic); //そのかわり遅延取得を可能にする
        }

        private unsafe void AddUnknown(ref MeCabNode resultNode, CharInfo cInfo,
                                       char* begin, char* begin2, char* begin3)
        {
            Token[] token = this.unkTokens[cInfo.DefaultType];
            for (int i = 0; i < token.Length; i++)
            {
                MeCabNode newNode = this.GetNewNode();
                this.ReadNodeInfo(this.unkDic, token[i], newNode);
                newNode.CharType = cInfo.DefaultType;
                newNode.Surface = new string(begin2, 0, (int)(begin3 - begin2));
                newNode.Length = (int)(begin3 - begin2);
                newNode.RLength = (int)(begin3 - begin);
                newNode.BNext = resultNode;
                newNode.Stat = MeCabNodeStat.Unk;
                if (this.unkFeature != null) newNode.Feature = this.unkFeature;
                resultNode = newNode;
            }
        }

        #endregion

        #region Get Node

        public MeCabNode GetBosNode()
        {
            MeCabNode bosNode = this.GetNewNode();
            bosNode.Surface = BosKey; // dummy
            bosNode.Feature = this.bosFeature;
            bosNode.IsBest = true;
            bosNode.Stat = MeCabNodeStat.Bos;
            return bosNode;
        }

        public MeCabNode GetEosNode()
        {
            MeCabNode eosNode = this.GetBosNode(); // same
            eosNode.Stat = MeCabNodeStat.Eos;
            return eosNode;
        }

        public MeCabNode GetNewNode()
        {
            MeCabNode node = new MeCabNode();
#if NeedId
            node.Id = Tokenizer.id++;
#endif
            return node;
        }

        #endregion

        #region Dispose

        private bool disposed;

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;

            if (disposing)
            {
                if (this.dic != null)
                    foreach (MeCabDictionary d in this.dic)
                        if (d != null) d.Dispose();

                if (this.unkDic != null) this.unkDic.Dispose();
            }

            this.disposed = true;
        }

        ~Tokenizer()
        {
            this.Dispose(false);
        }

        #endregion
    }
}
