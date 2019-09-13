//  MeCab -- Yet Another Part-of-Speech and Morphological Analyzer
//
//  Copyright(C) 2001-2006 Taku Kudo <taku@chasen.org>
//  Copyright(C) 2004-2006 Nippon Telegraph and Telephone Corporation
using System;
using System.Collections.Generic;
using System.Text;
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

        private struct Unit
        {
#pragma warning disable 0649
            public int Base;
            public uint Check;
#pragma warning restore 0649
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
                this.array[i] = new Unit()
                {
                    Base = reader.ReadInt32(),
                    Check = reader.ReadUInt32()
                };
            }
        }

#endif

        #endregion

        #region Search

        public struct ResultPair
        {
            public int Value;
            public int Length;
        }

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
                    return new ResultPair() { Value = -1, Length = 0 };
                }
            }

            p = b;
            int n = this.array[b].Base;
            if (b == this.array[p].Check && n < 0)
            {
                return new ResultPair() { Value = -n - 1, Length = 0 };
            }

            return new ResultPair() { Value = -1, Length = 0 };
        }

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
                        result[num] = new ResultPair() { Value = -n - 1, Length = i };
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
                    result[num] = new ResultPair() { Value = -n - 1, Length = len };
                num++;
            }

            return num;
        }

        #endregion
    }
}
