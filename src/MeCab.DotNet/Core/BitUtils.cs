using System;
using System.Collections.Generic;
using System.Text;

namespace MeCab.Core
{
    /// <summary>
    /// ビット値操作のユーティリティ
    /// </summary>
    /// <remarks>
    /// BitVector32構造体より実行速度に重点を置き、シンプルな実装にする。
    /// </remarks>
    public static class BitUtils
    {
        private const uint One = 0x00000001u;
        private const uint AllZero = 0x00000000u;
        private const uint AllOne = 0xFFFFFFFFu;

        /// <summary>
        /// 指定範囲のビットフィールド値を取り出す
        /// </summary>
        /// <param name="bits">ビット列を表すUInt32値</param>
        /// <param name="offset">開始ビット位置</param>
        /// <param name="len">ビット長</param>
        /// <returns>ビットフィールド値</returns>
        public static uint GetBitField(uint bits, int offset, int len)
        {
            uint mask = ~(AllOne << len);
            return (bits >> offset) & mask;
        }

        /// <summary>
        /// 指定位置のビット値を取り出す
        /// </summary>
        /// <param name="bits">ビット列を表すUInt32値</param>
        /// <param name="offset">ビット位置</param>
        /// <returns>ビット値</returns>
        public static bool GetFlag(uint bits, int offset)
        {
            uint mask = One << offset;
            return (bits & mask) != AllZero;
        }

        /// <summary>
        /// 指定範囲のビット値のAND比較
        /// </summary>
        /// <param name="bits1">ビット列1を表すUInt32値</param>
        /// <param name="bits2">ビット列2を表すUInt32値</param>
        /// <param name="offset">開始ビット位置</param>
        /// <param name="len">ビット長</param>
        /// <returns>比較結果</returns>
        public static bool CompareAnd(uint bits1, uint bits2, int offset, int len)
        {
            uint mask = ~(AllOne << len) << offset;
            return (bits1 & bits2 & mask) != AllZero;
        }

        //public static unsafe uint TurnEndianness(uint value) 
        //{
        //    //System.Net.IPAddress.HostToNetworkOrderでもよいが、

        //    byte* pVal = (byte*)&value;

        //    byte* pDist = stackalloc byte[4];
        //    pDist += 3;

        //    *pDist-- = *pVal++;
        //    *pDist-- = *pVal++;
        //    *pDist-- = *pVal++;
        //    *pDist = *pVal;

        //    return *(uint*)pDist;
        //}
    }
}
