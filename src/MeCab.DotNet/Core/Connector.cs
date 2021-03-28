//  MeCab -- Yet Another Part-of-Speech and Morphological Analyzer
//
//  Copyright(C) 2001-2006 Taku Kudo <taku@chasen.org>
//  Copyright(C) 2004-2006 Nippon Telegraph and Telephone Corporation
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using MeCab;
#if NET40 || NET45 || NETSTANDARD2_0 || NETSTANDARD2_1 || NET5_0
using System.IO.MemoryMappedFiles;
#endif

namespace MeCab.Core
{
    public class Connector : IDisposable
    {
        #region Const/Field/Property

        private const string MatrixFile = "matrix.bin";

#if NET40 || NET45 || NETSTANDARD2_0 || NETSTANDARD2_1 || NET5_0
        private MemoryMappedFile mmf;
        private MemoryMappedViewAccessor matrix;
        private FileStream fileStream;
#else
        private short[] matrix;
#endif

        public ushort LSize { get; private set; }

        public ushort RSize { get; private set; }

        #endregion

        #region Open

        public void Open(MeCabParam param)
        {
            string fileName = Path.Combine(param.DicDir, MatrixFile);
            this.Open(fileName);
        }

#if NET40 || NET45 || NETSTANDARD2_0 || NETSTANDARD2_1 || NET5_0
        public void Open(string fileName)
        {
            // https://github.com/komutan/NMeCab/blob/4d61926834b4a63e38cee050c0b7382c52a71226/src/LibNMeCab/Core/MemoryMappedFileLoader.cs#L28
            this.fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);

            // MMFインスタンスを生成するが、後でDisposeするために保持しておく
#if NET40 || NET45
            this.mmf = MemoryMappedFile.CreateFromFile(
                fileStream,
                null,
                0L,
                MemoryMappedFileAccess.Read,
                null,
                HandleInheritability.None,
                false);
#else
            this.mmf = MemoryMappedFile.CreateFromFile(
                fileStream,
                null,
                0L,
                MemoryMappedFileAccess.Read,
                HandleInheritability.None,
                false);
#endif
            this.Open(this.mmf);
        }

        public void Open(MemoryMappedFile mmf)
        {
            using (MemoryMappedViewStream stream = mmf.CreateViewStream(
                                                        0L, 0L, MemoryMappedFileAccess.Read))
            using (BinaryReader reader = new BinaryReader(stream))
            {
                this.LSize = reader.ReadUInt16();
                this.RSize = reader.ReadUInt16();

                long offset = stream.Position;
                long size = this.LSize * this.RSize * sizeof(short);
                this.matrix = mmf.CreateViewAccessor(offset, size, MemoryMappedFileAccess.Read);
            }
        }

#else
        public void Open(string fileName)
        {
            using (FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            using (BinaryReader reader = new BinaryReader(stream))
            {
                this.Open(reader, fileName);
            }
        }

        public void Open(BinaryReader reader, string fileName = null)
        {
            this.LSize = reader.ReadUInt16();
            this.RSize = reader.ReadUInt16();

            this.matrix = new short[this.LSize * this.RSize];
            for (int i = 0; i < this.matrix.Length; i++)
            {
                this.matrix[i] = reader.ReadInt16();
            }

            if (reader.BaseStream.ReadByte() != -1)
                throw new MeCabInvalidFileException("file size is invalid", fileName);
        }
#endif

#endregion

#region Cost

        public int Cost(MeCabNode lNode, MeCabNode rNode)
        {
            int pos = lNode.RCAttr + this.LSize * rNode.LCAttr;

#if NET40 || NET45 || NETSTANDARD2_0 || NETSTANDARD2_1 || NET5_0
            return this.matrix.ReadInt16(pos * sizeof(short)) + rNode.WCost;
#else
            return this.matrix[pos] + rNode.WCost;
#endif
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
#if NET40 || NET45 || NETSTANDARD2_0 || NETSTANDARD2_1 || NET5_0
                if (this.mmf != null) this.mmf.Dispose();
                if (this.matrix != null) this.matrix.Dispose();
                this.fileStream?.Dispose();
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
