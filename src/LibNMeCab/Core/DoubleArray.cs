//  MeCab -- Yet Another Part-of-Speech and Morphological Analyzer
//
//  Copyright(C) 2001-2006 Taku Kudo <taku@chasen.org>
//  Copyright(C) 2004-2006 Nippon Telegraph and Telephone Corporation

#pragma warning disable CS1591

using System.Runtime.CompilerServices;
#if !MMF_DIC
using System.IO;
#endif
namespace NMeCab.Core
{
    /// <summary>
    /// Double-Array Trie の実装
    /// </summary>
    public class DoubleArray
    {
        #region Array

        private readonly struct Unit
        {
            public readonly int Base;
            public readonly uint Check;

            public Unit(int @base, uint check)
            {
                Base = @base;
                Check = check;
            }
        }

        public const int UnitSize = sizeof(int) + sizeof(uint);

#if MMF_DIC

        private unsafe Unit* array;

        public int Size
        {
            get { return this.TotalSize / UnitSize; }
        }

        public int TotalSize { get; private set; }

#else

        private Unit[] array;

        public int Size
        {
            get { return this.array.Length; }
        }

        public int TotalSize
        {
            get { return this.Size * UnitSize; }
        }

#endif

        #endregion

        #region Open

#if MMF_DIC

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Open(byte* ptr, int size)
        {
            this.array = (Unit*)ptr;
            this.TotalSize = size;
        }

#else

        public void Open(BinaryReader reader, int size)
        {
            this.array = new Unit[size / UnitSize];

            for (int i = 0; i < array.Length; i++)
            {
                this.array[i] = new Unit(reader.ReadInt32(), reader.ReadUInt32());
            }
        }

#endif

        #endregion

        #region Search

        public readonly struct ResultPair
        {
            public readonly int Value;
            public readonly int Length;

            public ResultPair(int value, int length)
            {
                Value = value;
                Length = length;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe ResultPair ExactMatchSearch(byte* key, int len, int nodePos)
        {
            int b = this.array[nodePos].Base;
            int p;

            for (int i = 0; i < len; i++)
            {
                p = b + key[i] + 1;
                if (b == this.array[p].Check)
                {
                    b = this.array[p].Base;
                }
                else
                {
                    return new ResultPair(-1, 0);
                }
            }

            p = b;
            int n = this.array[b].Base;
            if (b == this.array[p].Check && n < 0)
            {
                return new ResultPair(-n - 1, 0);
            }

            return new ResultPair(-1, 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe int CommonPrefixSearch(byte* key, ResultPair* result, int resultLen, int len, int nodePos = 0)
        {
            int b = this.array[nodePos].Base;
            int num = 0;
            int n;
            int p;

            for (int i = 0; i < len; i++)
            {
                p = b;
                n = this.array[p].Base;

                if (b == this.array[p].Check && n < 0)
                {
                    if (num < resultLen)
                        result[num] = new ResultPair(-n - 1, i);
                    num++;
                }

                p = b + key[i] + 1;
                if (b == this.array[p].Check)
                {
                    b = this.array[p].Base;
                }
                else
                {
                    return num;
                }
            }

            p = b;
            n = this.array[p].Base;

            if (b == this.array[p].Check && n < 0)
            {
                if (num < resultLen)
                    result[num] = new ResultPair(-n - 1, len);
                num++;
            }

            return num;
        }

        #endregion
    }
}
