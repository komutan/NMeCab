using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;
#if MMF_DIC
using System.IO.MemoryMappedFiles;
#endif

namespace NMeCab.Core
{
    public static class StrUtils
    {
        private const byte Nul = (byte)0;

        /// <summary>
        /// バイト配列の中から終端が\0で表された文字列を取り出す。
        /// </summary>
        /// <remarks>
        /// バイト配列の長さはInt32.MaxValueを超えていても良い。
        /// </remarks>
        /// <param name="bytes">バイト配列</param>
        /// <param name="enc">文字エンコーディング</param>
        /// <returns>文字列（\0は含まない）</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetString(byte[] bytes, Encoding enc)
        {
            return StrUtils.GetString(bytes, 0L, enc);
        }

        /// <summary>
        /// バイト配列の中から終端が\0で表された文字列を取り出す。
        /// </summary>
        /// <remarks>
        /// バイト配列の長さはInt32.MaxValueを超えていても良い。
        /// </remarks>
        /// <param name="bytes">バイト配列</param>
        /// <param name="offset">オフセット位置</param>
        /// <param name="enc">文字エンコーディング</param>
        /// <returns>文字列（\0は含まない）</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static string GetString(byte[] bytes, long offset, Encoding enc)
        {
            fixed (byte* pBytes = bytes)
                return StrUtils.GetString(pBytes, offset, enc);
        }

        /// <summary>
        /// バイト配列の中から終端が\0で表された文字列を取り出す。
        /// </summary>
        /// <remarks>
        /// バイト配列の長さはInt32.MaxValueを超えていても良い。
        /// </remarks>
        /// <param name="bytes">デコードするバイトへのポインタ</param>
        /// <param name="offset">オフセット位置</param>
        /// <param name="enc">文字エンコーディング</param>
        /// <returns>文字列（\0は含まない）</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static string GetString(byte* bytes, long offset, Encoding enc)
        {
            return StrUtils.GetString(bytes + offset, enc);
        }

        /// <summary>
        /// バイト配列の中から終端が\0で表された文字列を取り出す。
        /// </summary>
        /// <remarks>
        /// バイト配列の長さはInt32.MaxValueを超えていても良い。
        /// </remarks>
        /// <param name="bytes">デコードする最初のバイトへのポインタ</param>
        /// <param name="enc">文字エンコーディング</param>
        /// <returns>文字列（\0は含まない）</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static string GetString(byte* bytes, Encoding enc)
        {
            //バイト長のカウント
            int byteCount = 0;
            while (bytes[byteCount] != Nul) //終端\0に到達するまでシーク
                checked { byteCount++; } //文字列のバイト長がInt32.MaxValueを超えたならエラー

            if (byteCount == 0) return "";

            return enc.GetString(bytes, byteCount);
        }

        /// <summary>
        /// 指定の名前に対応するエンコーディングを取得する（.NET FWが対応していない名前にもアドホックに対応）
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Encoding GetEncoding(string name)
        {
            switch (name.ToUpper())
            {
                case "UTF8":
                    return Encoding.UTF8;
                default:
                    return Encoding.GetEncoding(name);
            }
        }

        /// <summary>
        /// 単一行のCSV形式の文字列を配列に変換する
        /// </summary>
        /// <param name="csvRowString">単一行のCSV形式の文字列</param>
        /// <param name="defaltColumnBuffSize">配列の内部バッファの初期値</param>
        /// <param name="defaltStringBuffSize">配列内の文字列の内部バッファの初期値</param>
        /// <returns>変換後の文字列配列</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string[] SplitCsvRow(string csvRowString,
                                           int defaltColumnBuffSize,
                                           int defaltStringBuffSize)
        {
            var ret = new List<string>(defaltColumnBuffSize);
            var stb = new StringBuilder(defaltStringBuffSize);
            Action<char> action = Normal;

            foreach (var c in csvRowString)
                action(c);

            ret.Add(stb.ToString());

            return ret.ToArray();

            #region Inner methods

            void Normal(char c)
            {
                switch (c)
                {
                    case ',':
                        ret.Add(stb.ToString());
                        stb.Clear();
                        break;
                    case '"':
                        action = AfterLeftWQuote;
                        break;
                    default:
                        stb.Append(c);
                        break;
                }
            }

            void AfterLeftWQuote(char c)
            {
                switch (c)
                {
                    case '"':
                        stb.Append(c);
                        action = Normal;
                        break;
                    default:
                        stb.Append(c);
                        action = InsideWQuote;
                        break;
                }
            }

            void InsideWQuote(char c)
            {
                switch (c)
                {
                    case '"':
                        action = AfterRightWQuote;
                        break;
                    default:
                        stb.Append(c);
                        break;
                }
            }

            void AfterRightWQuote(char c)
            {
                switch (c)
                {
                    case ',':
                        ret.Add(stb.ToString());
                        stb.Clear();
                        action = Normal;
                        break;
                    case '"':
                        stb.Append(c);
                        action = InsideWQuote;
                        break;
                    default:
                        stb.Append(c);
                        action = Normal;
                        break;
                }
            }

            #endregion
        }
    }
}
