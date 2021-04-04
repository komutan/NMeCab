#pragma warning disable CS1591

#if NET20 || NET35 || NETSTANDARD1_3
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace NMeCab.Core
{
    public class MemoryMappedFileLoader
    {
        private bool invoked = false;
        private bool disposed = false;
        private IntPtr ptr = IntPtr.Zero;

        public long FileSize { get; private set; }

        public unsafe byte* Invoke(string file)
        {
            if (this.invoked) throw new InvalidOperationException();

            this.invoked = true;

            using (var source = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                this.FileSize = source.Length;
                this.ptr = Marshal.AllocCoTaskMem((int)this.FileSize);

                byte* wrkPtr = (byte*)this.ptr;
                byte[] buff = new byte[4096];
                fixed (byte* pBuff = buff)
                {
                    while (true)
                    {
                        int len = source.Read(buff, 0, buff.Length);
                        if (len == 0) return (byte*)this.ptr;

                        for (int i = 0; i < len; i++)
                            *wrkPtr++ = pBuff[i];
                    }
                }
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;

            if (this.ptr != IntPtr.Zero)
                Marshal.FreeCoTaskMem(this.ptr);

            disposed = true;
        }

        ~MemoryMappedFileLoader()
        {
            this.Dispose(false);
        }
    }
}
#else
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

#if NET40 || NET45
            this.mmf = MemoryMappedFile.CreateFromFile(fileStream,
                                                       null,
                                                       0L,
                                                       MemoryMappedFileAccess.Read,
                                                       null,
                                                       HandleInheritability.None,
                                                       false);
#else
            this.mmf = MemoryMappedFile.CreateFromFile(fileStream,
                                                       null,
                                                       0L,
                                                       MemoryMappedFileAccess.Read,
                                                       HandleInheritability.None,
                                                       false);
#endif

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
#endif