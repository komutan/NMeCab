//  MeCab -- Yet Another Part-of-Speech and Morphological Analyzer
//
//  Copyright(C) 2001-2006 Taku Kudo <taku@chasen.org>
//  Copyright(C) 2004-2006 Nippon Telegraph and Telephone Corporation

#pragma warning disable CS1591

using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace NMeCab.Core
{
    public class Connector<TNode> : IDisposable
        where TNode : MeCabNodeBase<TNode>
    {
        #region Const/Field/Property

        private const string MatrixFile = "matrix.bin";

        private readonly MemoryMappedFileLoader mmfLoader = new MemoryMappedFileLoader();
        private unsafe short* matrix;

        public ushort LSize { get; private set; }
        public ushort RSize { get; private set; }

        #endregion

        #region Open

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Open(string dicDir)
        {
            string fileName = Path.Combine(dicDir, MatrixFile);

            ushort* ptr = (ushort*)this.mmfLoader.Invoke(fileName);

            this.LSize = *ptr++;
            this.RSize = *ptr++;

            long fSize = sizeof(short) * (2 + this.LSize * this.RSize);
            if (this.mmfLoader.FileSize != fSize)
                throw new InvalidDataException($"File size is invalid. {fileName}");

            this.matrix = (short*)ptr;
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
                this.mmfLoader.Dispose();
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
