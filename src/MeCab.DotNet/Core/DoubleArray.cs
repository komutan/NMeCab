//  MeCab -- Yet Another Part-of-Speech and Morphological Analyzer
//
//  Copyright(C) 2001-2006 Taku Kudo <taku@chasen.org>
//  Copyright(C) 2004-2006 Nippon Telegraph and Telephone Corporation
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
#if NET40 || NET45 || NETSTANDARD2_0 || NETSTANDARD2_1 || NET5_0
using System.IO.MemoryMappedFiles;
#endif

namespace MeCab.Core
{
    /// <summary>
    /// Double-Array Trie の実装
    /// </summary>
    public class DoubleArray : IDisposable
    {
        #region Array

        private struct Unit
        {
            public readonly int Base;
            public readonly uint Check;

            public Unit(int b, uint c)
            {
                this.Base = b;
                this.Check = c;
            }
        }

        public const int UnitSize = sizeof(int) + sizeof(uint);

#if NET40 || NET45 || NETSTANDARD2_0 || NETSTANDARD2_1 || NET5_0

        private MemoryMappedViewAccessor accessor;

        public int Size
        {
            get { return (int)(this.accessor.Capacity) / UnitSize; }
        }

        public int TotalSize
        {
            get { return (int)(this.accessor.Capacity); }
        }

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

#if NET40 || NET45 || NETSTANDARD2_0 || NETSTANDARD2_1 || NET5_0

        public void Open(MemoryMappedFile mmf, long offset, long size)
        {
            this.accessor = mmf.CreateViewAccessor(offset, size, MemoryMappedFileAccess.Read);
        }

#else

        public void Open(BinaryReader reader, uint size)
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

        public struct ResultPair
        {
            public int Value;

            public int Length;

            public ResultPair(int r, int t)
            {
                this.Value = r;
                this.Length = t;
            }
        }

        public unsafe void ExactMatchSearch(byte* key, ResultPair* result, int len, int nodePos)
        {
            *result = this.ExactMatchSearch(key, len, nodePos);
        }

        public unsafe ResultPair ExactMatchSearch(byte* key, int len, int nodePos)
        {
            int b = this.ReadBase(nodePos);
            Unit p;

            for (int i = 0; i < len; i++)
            {
                this.ReadUnit(b + key[i] + 1, out p);
                if (b == p.Check)
                {
                    b = p.Base;
                }
                else
                {
                    return new ResultPair(-1, 0);
                }
            }

            this.ReadUnit(b, out p);
            int n = p.Base;
            if (b == p.Check && n < 0)
            {
                return new ResultPair(-n - 1, len);
            }

            return new ResultPair(-1, 0);
        }

        public unsafe int CommonPrefixSearch(byte* key, ResultPair* result, int resultLen, int len, int nodePos = 0)
        {
            int b = this.ReadBase(nodePos);
            int num = 0;
            int n;
            Unit p;

            for (int i = 0; i < len; i++)
            {
                this.ReadUnit(b, out p);
                n = p.Base;

                if (b == p.Check && n < 0)
                {
                    if (num < resultLen) result[num] = new ResultPair(-n - 1, i);
                    num++;
                }

                this.ReadUnit(b + key[i] + 1, out p);
                if (b == p.Check)
                {
                    b = p.Base;
                }
                else
                {
                    return num;
                }
            }

            this.ReadUnit(b, out p);
            n = p.Base;

            if (b == p.Check && n < 0)
            {
                if (num < resultLen) result[num] = new ResultPair(-n - 1, len);
                num++;
            }

            return num;
        }



        private int ReadBase(int pos)
        {
#if NET40 || NET45 || NETSTANDARD2_0 || NETSTANDARD2_1 || NET5_0
            return this.accessor.ReadInt32(pos * UnitSize);
#else
            return this.array[pos].Base;
#endif
        }

        private void ReadUnit(int pos, out Unit unit)
        {
#if NET40 || NET45 || NETSTANDARD2_0 || NETSTANDARD2_1 || NET5_0
            this.accessor.Read<Unit>(pos * UnitSize, out unit);
#else
            unit = this.array[pos];
#endif
        }

        #endregion

        #region Dispose

        private bool disposed;

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
#if NET40 || NET45 || NETSTANDARD2_0 || NETSTANDARD2_1
                if (this.accessor != null) this.accessor.Dispose();
#endif
            }

            this.disposed = true;
        }

        ~DoubleArray()
        {
            this.Dispose(false);
        }

        #endregion
    }
}
