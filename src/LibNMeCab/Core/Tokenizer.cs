//  MeCab -- Yet Another Part-of-Speech and Morphological Analyzer
//
//  Copyright(C) 2001-2006 Taku Kudo <taku@chasen.org>
//  Copyright(C) 2004-2006 Nippon Telegraph and Telephone Corporation

#pragma warning disable CS1591

using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace NMeCab.Core
{
    public class Tokenizer<TNode> : IDisposable
        where TNode : MeCabNodeBase<TNode>
    {
        #region Const

        private const string SysDicFile = "sys.dic";
        private const string UnkDicFile = "unk.dic";
        private const int DAResultSize = 512;

        #endregion

        #region Field/Property

        private MeCabDictionary[] dic;
        private readonly MeCabDictionary unkDic = new MeCabDictionary();
        private Token[][] unkTokens;
        private CharInfo space;
        private readonly CharProperty property = new CharProperty();

        public Encoding Encoding { get; private set; }

        #endregion

        #region Open/Clear

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Open(string dicDir, string[] userDics)
        {
            this.property.Open(dicDir);

            this.dic = new MeCabDictionary[userDics.Length + 1];
            for (int i = 0; i < this.dic.Length; i++)
                this.dic[i] = new MeCabDictionary();

            var sysDic = this.dic[0];
            sysDic.Open(Path.Combine(dicDir, SysDicFile));
            if (sysDic.Type != DictionaryType.Sys)
                throw new InvalidDataException($"not a system dictionary. {sysDic.FileName ?? ""}");

            for (int i = 0; i < userDics.Length; i++)
            {
                var d = this.dic[i + 1];
                d.Open(Path.Combine(dicDir, userDics[i]));
                if (d.Type != DictionaryType.Usr)
                    throw new InvalidDataException($"not a user dictionary. {d.FileName ?? ""}");
                if (!sysDic.IsCompatible(d))
                    throw new InvalidDataException($"incompatible dictionary. {d.FileName ?? ""}");
            }

            this.unkDic.Open(Path.Combine(dicDir, UnkDicFile));
            if (this.unkDic.Type != DictionaryType.Unk)
                throw new InvalidDataException($"not a unk dictionary. {UnkDicFile}");

            this.unkTokens = new Token[this.property.Size][];
            for (int i = 0; i < this.unkTokens.Length; i++)
            {
                fixed (byte* key = this.property.Name(i))
                {
                    var n = this.unkDic.ExactMatchSearch(key, StrUtils.GetLength(key));
                    if (n.Value == -1)
                        throw new InvalidDataException($"cannot find UNK category: {StrUtils.GetString(key, this.Encoding)} {this.unkDic.FileName ?? ""}");

                    this.unkTokens[i] = this.unkDic.GetTokensArray(n.Value);
                }
            }

            this.space = this.property.GetCharInfo(' ');

            this.Encoding = StrUtils.GetEncoding(sysDic.CharSet);
        }

        #endregion

        #region Lookup

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe TNode Lookup(char* begin,
                                   char* end,
                                   byte* bytesBegin,
                                   byte* bytesEnd,
                                   MeCabParam param,
                                   Func<TNode> nodeAllocator)
        {
            CharInfo cInfo;

            if (end - begin > ushort.MaxValue) end = begin + ushort.MaxValue;

            int leftSpaceLen;
            char* begin2 = property.SeekToOtherType(begin, end, this.space, &cInfo, &leftSpaceLen);
            if (begin2 >= end) return null;
            byte* bytesBegin2 = bytesBegin + this.Encoding.GetByteCount(begin, leftSpaceLen);

            TNode resultNode = null;
            var daResults = stackalloc DoubleArray.ResultPair[DAResultSize];

            foreach (MeCabDictionary it in this.dic)
            {
                int n = it.CommonPrefixSearch(bytesBegin2, (int)(bytesEnd - bytesBegin2), daResults, DAResultSize);
                for (int i = 0; i < n; i++)
                {
                    int length = this.Encoding.GetCharCount(bytesBegin2, daResults->Length);
                    int rLength = (int)(begin2 - begin) + length;
                    var tokenSize = it.GetTokenSize(daResults->Value);
                    var tokens = it.GetTokens(daResults->Value);
                    for (int j = 0; j < tokenSize; j++)
                    {
                        var newNode = nodeAllocator();
                        newNode.Surface = new string(begin2, 0, length);
                        newNode.Length = length;
                        newNode.RLength = rLength;
                        newNode.LCAttr = tokens->LcAttr;
                        newNode.RCAttr = tokens->RcAttr;
                        newNode.PosId = tokens->PosId;
                        newNode.WCost = tokens->WCost;
                        newNode.PFeature = it.GetFeature(tokens->Feature);
                        tokens++;
                        newNode.Encoding = this.Encoding;
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

            if (cInfo.Group)
            {
                char* tmp = begin3;
                CharInfo fail;
                int cLen;
                begin3 = this.property.SeekToOtherType(begin3, end, cInfo, &fail, &cLen);
                if (cLen <= param.MaxGroupingSize)
                    this.AddUnknown(ref resultNode, cInfo, begin, begin2, begin3, nodeAllocator);
                groupBegin3 = begin3;
                begin3 = tmp;
            }

            for (int i = 1; i <= cInfo.Length; i++)
            {
                if (begin3 > end) break;
                if (begin3 == groupBegin3) continue;
                this.AddUnknown(ref resultNode, cInfo, begin, begin2, begin3, nodeAllocator);
                if (!cInfo.IsKindOf(this.property.GetCharInfo(*begin3))) break;
                begin3 += 1;
            }

            if (resultNode == null) this.AddUnknown(ref resultNode, cInfo, begin, begin2, begin3, nodeAllocator);

            return resultNode;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe void AddUnknown(ref TNode resultNode, CharInfo cInfo,
                                       char* begin, char* begin2, char* begin3,
                                       Func<TNode> nodeAllocator)
        {
            var tokens = this.unkTokens[cInfo.DefaultType];
            fixed (Token* fpTokens = tokens)
            {
                Token* pTokens = fpTokens;
                var length = (int)(begin3 - begin2);
                var rLength = (int)(begin3 - begin);
                var surface = new string(begin2, 0, length);

                for (int i = 0; i < tokens.Length; i++)
                {
                    var newNode = nodeAllocator();
                    newNode.Surface = surface;
                    newNode.Length = length;
                    newNode.RLength = rLength;
                    newNode.LCAttr = pTokens->LcAttr;
                    newNode.RCAttr = pTokens->RcAttr;
                    newNode.PosId = pTokens->PosId;
                    newNode.WCost = pTokens->WCost;
                    newNode.PFeature = this.unkDic.GetFeature(pTokens->Feature);
                    newNode.Encoding = this.Encoding;
                    newNode.CharType = cInfo.DefaultType;
                    newNode.Stat = MeCabNodeStat.Unk;
                    newNode.BNext = resultNode;
                    resultNode = newNode;
                    pTokens++;
                }
            }
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

                this.unkDic.Dispose();  // Nullチェック不要
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
