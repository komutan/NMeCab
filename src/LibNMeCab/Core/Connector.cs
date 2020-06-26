﻿//  MeCab -- Yet Another Part-of-Speech and Morphological Analyzer
//
//  Copyright(C) 2001-2006 Taku Kudo <taku@chasen.org>
//  Copyright(C) 2004-2006 Nippon Telegraph and Telephone Corporation
using System;
using System.IO;
using System.Runtime.CompilerServices;
#if MMF_MTX
using System.IO.MemoryMappedFiles;
#endif

namespace NMeCab.Core
{
    public class Connector<TNode> : IDisposable
        where TNode : MeCabNodeBase<TNode>
    {
        #region Const/Field/Property

        private const string MatrixFile = "matrix.bin";

#if MMF_MTX
        private readonly MemoryMappedFileLoader mmfLoader = new MemoryMappedFileLoader();
        private unsafe short* matrix;
#else
        private short[] matrix;
#endif

        public ushort LSize { get; private set; }
        public ushort RSize { get; private set; }

        #endregion

        #region Open

        public unsafe void Open(string dicDir)
        {
            string fileName = Path.Combine(dicDir, MatrixFile);

#if MMF_MTX
            ushort* ptr = (ushort*)this.mmfLoader.Invoke(fileName);

            this.LSize = *ptr++;
            this.RSize = *ptr++;

            long fSize = sizeof(short) * (2 + this.LSize * this.RSize);
            if (this.mmfLoader.FileSize != fSize)
                throw new InvalidDataException($"File size is invalid. {fileName}");

            this.matrix = (short*)ptr;
#else
            using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var reader = new BinaryReader(stream))
            {
                this.LSize = reader.ReadUInt16();
                this.RSize = reader.ReadUInt16();

                this.matrix = new short[this.LSize * this.RSize];
                for (int i = 0; i < this.matrix.Length; i++)
                {
                    this.matrix[i] = reader.ReadInt16();
                }

                if (reader.BaseStream.ReadByte() != -1)
                    throw new InvalidDataException($"File size is invalid. {fileName}");
            }
#endif
        }

        #endregion

        #region Cost

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe int Cost(TNode lNode, TNode rNode)
        {
            int pos = lNode.RCAttr + this.LSize * rNode.LCAttr;
            return this.matrix[pos] + rNode.WCost;
        }

        #endregion

        #region Dispose

        private bool disposed;

        /// <summary>
        /// 使用中のリソースを開放する
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
#if MMF_MTX
                this.mmfLoader.Dispose();
#endif
            }

            this.disposed = true;
        }

        ~Connector()
        {
            Dispose(false);
        }

        #endregion
    }
}
