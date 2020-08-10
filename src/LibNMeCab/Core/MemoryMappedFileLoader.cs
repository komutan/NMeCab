#pragma warning disable CS1591

using System;
using System.IO;
using System.IO.MemoryMappedFiles;

namespace NMeCab.Core
{
    public class MemoryMappedFileLoader : IDisposable
    {
        private bool invoked = false;
        private bool disposed = false;
        private FileStream fileStream = null;
        private MemoryMappedFile mmf = null;
        private MemoryMappedViewAccessor mmva = null;

        public long FileSize
        {
            get { return this.fileStream?.Length ?? 0L; }
        }

        public unsafe byte* Invoke(string file)
        {
            if (this.invoked) throw new InvalidOperationException();

            this.invoked = true;

            this.fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read);

            this.mmf = MemoryMappedFile.CreateFromFile(fileStream,
                                                       null,
                                                       0L,
                                                       MemoryMappedFileAccess.Read,
                                                       HandleInheritability.None,
                                                       false);

            this.mmva = mmf.CreateViewAccessor(0L, 0L, MemoryMappedFileAccess.Read);

            byte* ptr = null;
            this.mmva.SafeMemoryMappedViewHandle.AcquirePointer(ref ptr);

            return ptr;
        }

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
                if (this.mmva != null)
                {
                    this.mmva.SafeMemoryMappedViewHandle.ReleasePointer();
                    this.mmva.Dispose();
                }

                this.mmf?.Dispose();

                this.fileStream?.Dispose();
            }

            disposed = true;
        }

        ~MemoryMappedFileLoader()
        {
            this.Dispose(false);
        }
    }
}
