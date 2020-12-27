#pragma warning disable IDE0059

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace NMeCab.Core
{
    /// <summary>
    /// 文字列関係操作ユーティリティ
    /// </summary>
    public static class StrUtils
    {
        private const byte Nul = (byte)0;

        /// <summary>
        /// 終端が\0で表されたバイト配列の長さを取得する
        /// </summary>
        /// <param name="bytes">終端が\0で表されたバイト配列の開始位置のポインタ</param>
        /// <returns>バイト配列の長さ</returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static unsafe int GetLength(byte* bytes)
        {
            int len = 0;

            while (bytes[len] != Nul) // 終端\0に到達するまでシーク
            {
                checked { len++; } // Int32.MaxValueを超えたならエラー
            }

            return len;
        }

        /// <summary>
        /// \0で区切られたバイト配列から文字列を取り出す。
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
        /// \0で区切られたバイト配列から文字列を取り出す。
        /// </summary>
        /// <remarks>
        /// バイト配列の長さはInt32.MaxValueを超えていても良い。
        /// </remarks>
        /// <param name="bytes">終端が\0で表されたバイト配列の開始位置のポインタ</param>
        /// <param name="offset">オフセット位置</param>
        /// <param name="enc">文字エンコーディング</param>
        /// <returns>文字列（\0は含まない）</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static string GetString(byte* bytes, long offset, Encoding enc)
        {
            return StrUtils.GetString(bytes + offset, enc);
        }

        /// <summary>
        /// \0で区切られたバイト配列から文字列を取り出す。
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
            int len = GetLength(bytes);
            return enc.GetString(bytes, len);
        }

        /// <summary>
        /// 大文字・小文字・区切り記号有無の異なる名称が指定されても、該当するであろうエンコーディングを取得する
        /// </summary>
        /// <param name="name">エンコーディング名</param>
        /// <returns>エンコーディング</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Encoding GetEncodingOrNull(this string name)
        {
            var cmpInf = CultureInfo.InvariantCulture.CompareInfo;
            var opt = CompareOptions.IgnoreCase | CompareOptions.IgnoreSymbols;

            foreach (var encInf in Encoding.GetEncodings())
            {
                if (cmpInf.Compare(encInf.Name, name, opt) == 0)
                {
                    return encInf.GetEncoding();
                }
            }

            return null;
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
