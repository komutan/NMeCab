//  MeCab -- Yet Another Part-of-Speech and Morphological Analyzer
//
//  Copyright(C) 2001-2006 Taku Kudo <taku@chasen.org>
//  Copyright(C) 2004-2006 Nippon Telegraph and Telephone Corporation
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
#if NeedId
using System.Threading;
#endif

namespace NMeCab.Core
{
    public class Tokenizer : IDisposable
    {
        #region Const

        private const string SysDicFile = "sys.dic";
        private const string UnkDicFile = "unk.dic";
        private const int DAResultSize = 512;

        #endregion

        #region Field

        private MeCabDictionary[] dic;
        private readonly MeCabDictionary unkDic = new MeCabDictionary();
        private Token[][] unkTokens;
        private CharInfo space;
        private readonly CharProperty property = new CharProperty();
#if NeedId
        [ThreadStaticAttribute]
        private static uint id;
#endif

        #endregion

        #region Open/Clear

        public void Open(string dicDir, string[] userDics)
        {
            this.property.Open(dicDir);

            this.dic = new MeCabDictionary[userDics.Length + 1];
            for (int i = 0; i < this.dic.Length; i++)
                this.dic[i] = new MeCabDictionary();

            var sysDic = this.dic[0];
            sysDic.Open(Path.Combine(dicDir, SysDicFile));
            if (sysDic.Type != DictionaryType.Sys)
                throw new MeCabInvalidFileException("not a system dictionary", SysDicFile);

            for (int i = 0; i < userDics.Length; i++)
            {
                var d = this.dic[i + 1];
                d.Open(Path.Combine(dicDir, userDics[i]));
                if (d.Type != DictionaryType.Usr)
                    throw new MeCabInvalidFileException("not a user dictionary", userDics[i]);
                if (!sysDic.IsCompatible(d))
                    throw new MeCabInvalidFileException("incompatible dictionary", userDics[i]);
            }

            this.unkDic.Open(Path.Combine(dicDir, UnkDicFile));
            if (this.unkDic.Type != DictionaryType.Unk)
                throw new MeCabInvalidFileException("not a unk dictionary", this.unkDic.FileName);

            this.unkTokens = new Token[this.property.Size][];
            for (int i = 0; i < this.unkTokens.Length; i++)
            {
                string key = this.property.Name(i);
                var n = this.unkDic.ExactMatchSearch(key);
                if (n.Value == -1)
                    throw new MeCabInvalidFileException("cannot find UNK category: " + key, this.unkDic.FileName);

                this.unkTokens[i] = this.unkDic.GetTokensArray(n.Value);
            }

            this.space = this.property.GetCharInfo(' ');
        }

#if NeedId
        public void Clear()
        {
            Tokenizer.id = 0u;
        }
#endif

        #endregion

        #region Lookup

        public unsafe MeCabNode Lookup(char* begin, char* end, MeCabParam param)
        {
            CharInfo cInfo;
            MeCabNode resultNode = null;
            int cLen;

            if (end - begin > ushort.MaxValue) end = begin + ushort.MaxValue;
            char* begin2 = property.SeekToOtherType(begin, end, this.space, &cInfo, &cLen);

            var daResults = stackalloc DoubleArray.ResultPair[DAResultSize];

            foreach (MeCabDictionary it in this.dic)
            {
                int n = it.CommonPrefixSearch(begin2, (int)(end - begin2), daResults, DAResultSize);
                for (int i = 0; i < n; i++)
                {
#if MMF_DIC
                    var tokenSize = it.GetTokenSize(daResults->Value);
                    var tokens = it.GetTokens(daResults->Value);
                    for (int j = 0; j < tokenSize; j++)
#else
                    var seg = it.GetTokens(daResults->Value);
                    var tokens = seg.Array;
                    for (int j = seg.Offset; j < seg.Offset + seg.Count; j++)
#endif
                    {
                        var newNode = this.GetNewNode();
                        this.ReadNodeInfo(it, tokens[j], newNode);
                        newNode.Length = daResults->Length;
                        newNode.RLength = (int)(begin2 - begin) + daResults->Length;
                        newNode.Surface = new string(begin2, 0, newNode.Length);
                        newNode.Stat = MeCabNodeStat.Nor;
                        newNode.CharType = cInfo.DefaultType;
                        newNode.BNext = resultNode;
                        resultNode = newNode;
                    }
                    daResults++;
                }
            }

            if (resultNode != null && !cInfo.Invoke) return resultNode;

            char* begin3 = begin2 + 1;
            char* groupBegin3 = null;

            if (begin3 > end)
            {
                this.AddUnknown(ref resultNode, cInfo, begin, begin2, begin3, param);
                return resultNode;
            }

            if (cInfo.Group)
            {
                char* tmp = begin3;
                CharInfo fail;
                begin3 = this.property.SeekToOtherType(begin3, end, cInfo, &fail, &cLen);
                if (cLen <= param.MaxGroupingSize) this.AddUnknown(ref resultNode, cInfo, begin, begin2, begin3, param);
                groupBegin3 = begin3;
                begin3 = tmp;
            }

            for (int i = 1; i <= cInfo.Length; i++)
            {
                if (begin3 > end) break;
                if (begin3 == groupBegin3) continue;
                cLen = i;
                this.AddUnknown(ref resultNode, cInfo, begin, begin2, begin3, param);
                if (!cInfo.IsKindOf(this.property.GetCharInfo(*begin3))) break;
                begin3 += 1;
            }

            if (resultNode == null) this.AddUnknown(ref resultNode, cInfo, begin, begin2, begin3, param);

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
                                       char* begin, char* begin2, char* begin3,
                                       MeCabParam param)
        {
            var token = this.unkTokens[cInfo.DefaultType];
            for (int i = 0; i < token.Length; i++)
            {
                var newNode = this.GetNewNode();
                this.ReadNodeInfo(this.unkDic, token[i], newNode);
                newNode.Length = (int)(begin3 - begin2);
                newNode.RLength = (int)(begin3 - begin);
                newNode.Surface = new string(begin2, 0, newNode.Length);
                newNode.CharType = cInfo.DefaultType;
                newNode.Stat = MeCabNodeStat.Unk;
                if (param.UnkFeature != null) newNode.Feature = param.UnkFeature;
                newNode.BNext = resultNode;
                resultNode = newNode;
            }
        }

        #endregion

        #region Get Node

        public MeCabNode GetBosNode(MeCabParam param)
        {
            var bosNode = this.GetNewNode();
            bosNode.Surface = "";
            bosNode.Feature = "";
            bosNode.IsBest = true;
            bosNode.Stat = MeCabNodeStat.Bos;
            return bosNode;
        }

        public MeCabNode GetEosNode(MeCabParam param)
        {
            var eosNode = this.GetBosNode(param); // same
            eosNode.Stat = MeCabNodeStat.Eos;
            return eosNode;
        }

        public MeCabNode GetNewNode()
        {
            var node = new MeCabNode();
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
                    foreach (var d in this.dic)
                        d?.Dispose();

                this.unkDic.Dispose();  //Nullチェック不要
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
