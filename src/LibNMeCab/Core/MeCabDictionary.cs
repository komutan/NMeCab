//  MeCab -- Yet Another Part-of-Speech and Morphological Analyzer
//
//  Copyright(C) 2001-2006 Taku Kudo <taku@chasen.org>
//  Copyright(C) 2004-2006 Nippon Telegraph and Telephone Corporation
using System;
using System.Text;
using System.IO;
using System.Runtime.CompilerServices;
#if MMF_DIC
using System.IO.MemoryMappedFiles;
#endif

namespace NMeCab.Core
{
    public class MeCabDictionary : IDisposable
    {
        #region  Const/Field/Property

        private const uint DictionaryMagicID = 0xEF718F77u;
        private const uint DicVersion = 102u;

#if MMF_DIC
        private MemoryMappedFile mmf;
        private MemoryMappedViewAccessor mmva;
        private unsafe Token* tokens;
        private unsafe byte* features;
#else
        private Token[] tokens;
        private byte[] features;
#endif

        private readonly DoubleArray da = new DoubleArray();

        private Encoding encoding;

        /// <summary>
        /// 辞書の文字コード
        /// </summary>
        public string CharSet
        {
            get { return this.encoding.WebName; }
        }

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

            var sourceFileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            try
            {
                this.mmf = MemoryMappedFile.CreateFromFile(sourceFileStream, null, 0L, MemoryMappedFileAccess.Read, HandleInheritability.None, false);
            }
            catch (Exception)
            {
                sourceFileStream.Dispose();
                throw;
            }
            this.mmva = mmf.CreateViewAccessor(0L, 0L, MemoryMappedFileAccess.Read);

            byte* ptr = null;
            this.mmva.SafeMemoryMappedViewHandle.AcquirePointer(ref ptr);

            using (var stream = this.mmf.CreateViewStream(0L, 0L, MemoryMappedFileAccess.Read))
            using (var reader = new BinaryReader(stream))
            {
                uint magic = reader.ReadUInt32();
                if (this.mmva.Capacity < (magic ^ DictionaryMagicID))
                    throw new InvalidDataException($"dictionary file is broken. {fileName}");

                this.Version = reader.ReadUInt32();
                if (this.Version != DicVersion)
                    throw new InvalidDataException($"incompatible version dictionaly. {fileName}");

                this.Type = (DictionaryType)reader.ReadUInt32();
                this.LexSize = reader.ReadUInt32();
                this.LSize = reader.ReadUInt32();
                this.RSize = reader.ReadUInt32();
                uint dSize = reader.ReadUInt32();
                uint tSize = reader.ReadUInt32();
                uint fSize = reader.ReadUInt32();
                reader.ReadUInt32(); //dummy

                string charSet = StrUtils.GetString(reader.ReadBytes(32), Encoding.ASCII);
                this.encoding = StrUtils.GetEncoding(charSet);

                ptr += stream.Position;

                this.da.Open(ptr, (int)dSize);
                ptr += dSize;

                this.tokens = (Token*)ptr;
                ptr += tSize;

                this.features = ptr;
            }
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

            string charSet = StrUtils.GetString(reader.ReadBytes(32), Encoding.ASCII);
            this.encoding = StrUtils.GetEncoding(charSet);

            this.da.Open(reader, (int)dSize);

            this.tokens = new Token[tSize / sizeof(Token)];
            for (int i = 0; i < this.tokens.Length; i++)
                this.tokens[i] = Token.Create(reader);

            this.features = reader.ReadBytes((int)fSize);
        }

#endif

        #endregion

        #region Search

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe DoubleArray.ResultPair ExactMatchSearch(string key)
        {
            fixed (char* pKey = key)
                return this.ExactMatchSearch(pKey, key.Length, 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe DoubleArray.ResultPair ExactMatchSearch(char* key, int len, int nodePos = 0)
        {
            //if (this.encoding == Encoding.Unicode)
            //    return this.da.ExactMatchSearch((byte*)key, len, nodePos);

            //エンコード
            int maxByteCount = this.encoding.GetMaxByteCount(len);
            byte* bytes = stackalloc byte[maxByteCount];
            int bytesLen = this.encoding.GetBytes(key, len, bytes, maxByteCount);

            var result = this.da.ExactMatchSearch(bytes, bytesLen, nodePos);

            //文字数をデコードしたものに変換
            result.Length = this.encoding.GetCharCount(bytes, result.Length);

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe int CommonPrefixSearch(char* key, int len, DoubleArray.ResultPair* result, int rLen)
        {
            //if (this.encoding == Encoding.Unicode)
            //    return this.da.CommonPrefixSearch((byte*)key, result, rLen, len);

            //エンコード
            int maxByteLen = this.encoding.GetMaxByteCount(len);
            byte* bytes = stackalloc byte[maxByteLen];
            int bytesLen = this.encoding.GetBytes(key, len, bytes, maxByteLen);

            int n = this.da.CommonPrefixSearch(bytes, result, rLen, bytesLen);

            //文字数をデコードしたものに変換
            for (int i = 0; i < n; i++)
                result[i].Length = this.encoding.GetCharCount(bytes, result[i].Length);

            return n;
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
#endif

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe string GetFeature(uint featurePos)
        {
            return StrUtils.GetString(this.features, (long)featurePos, this.encoding);
        }

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
                if (this.mmva != null)
                {
                    this.mmva.SafeMemoryMappedViewHandle.ReleasePointer();
                    this.mmva.Dispose();
                }

                if (this.mmf != null)
                    this.mmf.Dispose();
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
