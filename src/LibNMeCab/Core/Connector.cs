//  MeCab -- Yet Another Part-of-Speech and Morphological Analyzer
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
        private MemoryMappedFile mmf;
        private MemoryMappedViewAccessor mmva;
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
            var sourceFileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            try
            {
                this.mmf = MemoryMappedFile.CreateFromFile(sourceFileStream, null, 0L, MemoryMappedFileAccess.Read, HandleInheritability.None, false);
            }
            catch (Exception)
            {
                sourceFileStream.Dispose();
                throw;
            }
            this.mmva = this.mmf.CreateViewAccessor(0L, 0L, MemoryMappedFileAccess.Read);

            byte* ptr = null;
            this.mmva.SafeMemoryMappedViewHandle.AcquirePointer(ref ptr);

            using (var stream = mmf.CreateViewStream(0L, 0L, MemoryMappedFileAccess.Read))
            using (var reader = new BinaryReader(stream))
            {
                this.LSize = reader.ReadUInt16();
                this.RSize = reader.ReadUInt16();

                long fSize = stream.Position + sizeof(short) * this.LSize * this.RSize;
                if (this.mmva.Capacity < fSize)
                    throw new InvalidDataException($"File size is invalid. {fileName}");

                ptr += stream.Position;
                this.matrix = (short*)ptr;
            }
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
                if (this.mmva != null)
                {
                    this.mmva.SafeMemoryMappedViewHandle.ReleasePointer();
                    this.mmva.Dispose();
                }

                if (this.mmf != null)
                    this.mmf.Dispose();
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
