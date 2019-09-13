using System;
using System.Collections.Generic;
using System.Text;
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
        public unsafe static string GetString(byte* bytes, Encoding enc)
        {
            //バイト長のカウント
            int byteCount = 0;
            while (bytes[byteCount] != Nul) //終端\0に到達するまでシーク
            {
                checked { byteCount++; } //文字列のバイト長がInt32.MaxValueを超えたならエラー
            }

            if (byteCount == 0)
                return "";

            //生成されうる最大文字数のバッファを確保
            int maxCharCount = enc.GetMaxCharCount(byteCount);
            fixed (char* buff = new char[maxCharCount])
            {
                //バイト配列を文字列にデコード
                int len = enc.GetChars(bytes, byteCount, buff, maxCharCount);
                return new string(buff, 0, len);
            }
        }

        /// <summary>
        /// 指定の名前に対応するエンコーディングを取得する（.NET FWが対応していない名前にもアドホックに対応）
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
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
    }
}
