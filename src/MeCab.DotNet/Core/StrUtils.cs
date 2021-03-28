using System;
using System.Collections.Generic;
using System.Text;
#if NET40 || NET45 || NETSTANDARD2_0 || NETSTANDARD2_1 || NET5_0
using System.IO.MemoryMappedFiles;
#endif

namespace MeCab.Core
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
                return StrUtils.GetString(pBytes + offset, enc);
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
            while (*bytes != Nul) //終端\0に到達するまでシーク
            {
                checked { byteCount++; } //文字列のバイト長がInt32.MaxValueを超えたならエラー
                bytes++;
            }
            bytes -= byteCount;

            //生成されうる最大文字数のバッファを確保
            int maxCharCount = enc.GetMaxCharCount(byteCount);
            fixed (char* buff = new char[maxCharCount])
            {
                //バイト配列を文字列にデコード
                int len = enc.GetChars(bytes, byteCount, buff, maxCharCount);
                return new string(buff, 0, len);
            }
        }

#if NET40 || NET45 || NETSTANDARD2_0 || NETSTANDARD2_1 || NET5_0

        /// <summary>
        /// MemoryMappedViewAccessorから終端が\0で表された文字列を取り出す。
        /// </summary>
        /// <remarks>
        /// MemoryMappedViewAccessorの容量はInt32.MaxValueを超えていても良い。
        /// </remarks>
        /// <param name="accessor">MemoryMappedViewAccessor</param>
        /// <param name="index">オフセット位置</param>
        /// <param name="enc">文字エンコーディング</param>
        /// <param name="buffSize">内部で使用するバッファの初期サイズ</param>
        /// <returns>文字列（\0は含まない）</returns>
        public static string GetString(MemoryMappedViewAccessor accessor, long offset, Encoding enc,
                                        int buffSize = 128)
        {
            byte[] buff = new byte[buffSize]; //IO回数削減のためのバッファ配列
            accessor.ReadArray<byte>(offset, buff, 0, buffSize); //初期読込

            //バイト長のカウント
            int byteCount = 0;
            while (buff[byteCount] != Nul) //終端\0に到達するまでシーク
            {
                byteCount++;

                if (byteCount == buffSize) //バッファ配列の終端
                {
                    //バッファ配列の拡張と追加読込
                    checked { buffSize *= 2; } //Int32.MaxValueを超えたならエラー
                    byte[] newBuff = new byte[buffSize];
                    Buffer.BlockCopy(buff, 0, newBuff, 0, byteCount);
                    accessor.ReadArray<byte>(offset + byteCount, newBuff, byteCount, buffSize - byteCount);
                    buff = newBuff;
                }
            }

            //バッファ配列を文字列にデコード
            return enc.GetString(buff, 0, byteCount);
        }

#endif

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
