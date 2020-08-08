//  MeCab -- Yet Another Part-of-Speech and Morphological Analyzer
//
//  Copyright(C) 2001-2006 Taku Kudo <taku@chasen.org>
//  Copyright(C) 2004-2006 Nippon Telegraph and Telephone Corporation
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

#if MMF_DIC
        private readonly MemoryMappedFileLoader mmfLoader = new MemoryMappedFileLoader();
        private unsafe Token* tokens;
        private unsafe byte* features;
#else
        private Token[] tokens;
        private byte[] features;
#endif

        private readonly DoubleArray da = new DoubleArray();

        /// <summary>
        /// 辞書の文字コード
        /// </summary>
        public string CharSet { get; private set; }

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

#if MMF_DIC

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

            this.CharSet = StrUtils.GetString(bytePtr, Encoding.ASCII);
            bytePtr += 32;

            this.da.Open(bytePtr, (int)dSize);
            bytePtr += dSize;

            this.tokens = (Token*)bytePtr;
            bytePtr += tSize;

            this.features = bytePtr;
        }

#else

        public void Open(string fileName)
        {
            this.FileName = fileName;

            using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (BinaryReader reader = new BinaryReader(fileStream))
            {
                this.Open(reader);
            }
        }

        public unsafe void Open(BinaryReader reader)
        {
            uint magic = reader.ReadUInt32();
            //CanSeekの時のみストリーム長のチェック
            if (reader.BaseStream.CanSeek && reader.BaseStream.Length != (magic ^ DictionaryMagicID))
                throw new InvalidDataException($"dictionary file is broken. {this.FileName}");

            this.Version = reader.ReadUInt32();
            if (this.Version != DicVersion)
                throw new InvalidDataException($"incompatible version dictionaly. {this.FileName}");

            this.Type = (DictionaryType)reader.ReadUInt32();
            this.LexSize = reader.ReadUInt32();
            this.LSize = reader.ReadUInt32();
            this.RSize = reader.ReadUInt32();
            uint dSize = reader.ReadUInt32();
            uint tSize = reader.ReadUInt32();
            uint fSize = reader.ReadUInt32();
            reader.ReadUInt32(); //dummy

            this.CharSet = StrUtils.GetString(reader.ReadBytes(32), 0, Encoding.ASCII);

            this.da.Open(reader, (int)dSize);

            this.tokens = new Token[tSize / sizeof(Token)];
            for (int i = 0; i < this.tokens.Length; i++)
                this.tokens[i] = new Token(reader);

            this.featuresIntPtr = Marshal.AllocCoTaskMem((int)fSize);
            this.features = (byte*)this.featuresIntPtr.ToPointer();
            this.features = reader.ReadBytes((int)fSize);
        }

#endif

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

#if MMF_DIC
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
#else
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ArraySegment<Token> GetTokens(int value)
        {
            return new ArraySegment<Token>(this.tokens, this.GetTokenPos(value), this.GetTokenSize(value));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Token[] GetTokensArray(int value)
        {
            var ret = new Token[this.GetTokenSize(value)];
            Array.Copy(this.tokens, this.GetTokenPos(value), ret, 0, ret.Length);
            return ret;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public unsafe string GetFeature(uint featurePos)
            {
                return StrUtils.GetString(this.features, (long)featurePos, this.encoding);
            }
#endif
        #endregion

        #region etc.

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsCompatible(MeCabDictionary d)
        {
            return (this.Version == d.Version &&
                    this.LSize == d.LSize &&
                    this.RSize == d.RSize &&
                    this.CharSet == d.CharSet);
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
#if MMF_DIC
                this.mmfLoader.Dispose();
#endif
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
