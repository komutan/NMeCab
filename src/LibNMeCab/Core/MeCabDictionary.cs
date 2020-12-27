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
    public class MeCabDictionary : IDisposable
    {
        #region  Const/Field/Property

        private const uint DictionaryMagicID = 0xEF718F77u;
        private const uint DicVersion = 102u;

        private readonly MemoryMappedFileLoader mmfLoader = new MemoryMappedFileLoader();
        private unsafe Token* tokens;
        private unsafe byte* features;

        private readonly DoubleArray da = new DoubleArray();

        /// <summary>
        /// 辞書の文字コード
        /// </summary>
        public Encoding Encoding { get; private set; }

        /// <summary>
        /// バージョン
        /// </summary>
        public uint Version { get; private set; }

        /// <summary>
        /// 辞書のタイプ
        /// </summary>
        public DictionaryType Type { get; private set; }

        public uint LexSize { get; private set; }

        /// <summary>
        /// 左文脈 ID のサイズ
        /// </summary>
        public uint LSize { get; private set; }

        /// <summary>
        /// 右文脈 ID のサイズ
        /// </summary>
        public uint RSize { get; private set; }

        /// <summary>
        /// 辞書のファイル名
        /// </summary>
        public string FileName { get; private set; }

        #endregion

        #region Open

        public unsafe void Open(string fileName)
        {
            this.FileName = fileName;

            uint* uintPtr = (uint*)this.mmfLoader.Invoke(fileName);

            uint magic = *uintPtr++;
            if (this.mmfLoader.FileSize != (magic ^ DictionaryMagicID))
                throw new InvalidDataException($"dictionary file is broken. {fileName}");

            this.Version = *uintPtr++;
            if (this.Version != DicVersion)
                throw new InvalidDataException($"incompatible version dictionaly. {fileName}");

            this.Type = (DictionaryType)(*uintPtr++);
            this.LexSize = *uintPtr++;
            this.LSize = *uintPtr++;
            this.RSize = *uintPtr++;
            uint dSize = *uintPtr++;
            uint tSize = *uintPtr++;
            uint fSize = *uintPtr++;
            uintPtr++; // dummy

            byte* bytePtr = (byte*)uintPtr;

            var encName = StrUtils.GetString(bytePtr, Encoding.ASCII);
            this.Encoding = encName.GetEncodingOrNull()
                            ?? throw new Exception($"not supported encoding dictionary. {encName} {fileName}");
            bytePtr += 32;

            this.da.Open(bytePtr, (int)dSize);
            bytePtr += dSize;

            this.tokens = (Token*)bytePtr;
            bytePtr += tSize;

            this.features = bytePtr;
        }

        #endregion

        #region Search

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe DoubleArray.ResultPair ExactMatchSearch(byte* key, int len, int nodePos = 0)
        {
            return this.da.ExactMatchSearch(key, len, nodePos);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe int CommonPrefixSearch(byte* key, int len, DoubleArray.ResultPair* result, int rLen)
        {
            return this.da.CommonPrefixSearch(key, result, rLen, len);
        }

        #endregion

        #region Get Infomation

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetTokenSize(int value)
        {
            return 0xFF & value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetTokenPos(int value)
        {
            return value >> 8;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe Token* GetTokens(int value)
        {
            return this.tokens + this.GetTokenPos(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe Token[] GetTokensArray(int value)
        {
            var ret = new Token[this.GetTokenSize(value)];
            var t = this.GetTokens(value);

            for (int i = 0; i < ret.Length; i++)
            {
                ret[i] = t[i];
            }

            return ret;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe byte* GetFeature(uint featurePos)
        {
            return this.features + featurePos;
        }

        #endregion

        #region etc.

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsCompatible(MeCabDictionary d)
        {
            return (this.Version == d.Version &&
                    this.LSize == d.LSize &&
                    this.RSize == d.RSize &&
                    this.Encoding.CodePage == d.Encoding.CodePage);
        }

        #endregion

        #region Dispose

        private bool disposed;

        /// <summary>
        /// 使用されているリソースを開放する
        /// </summary>
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
                this.mmfLoader.Dispose();
            }

            this.disposed = true;
        }

        ~MeCabDictionary()
        {
            this.Dispose(false);
        }

        #endregion
    }
}
