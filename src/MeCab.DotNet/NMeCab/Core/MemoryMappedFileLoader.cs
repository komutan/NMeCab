using System;
using System.IO;
using System.Runtime.InteropServices;

namespace NMeCab.Core
{
    public class MemoryMappedFileLoader : IDisposable
    {

        private bool invoked = false;
        private bool disposed = false;
        private IntPtr ptr;

        public long FileSize { get; private set; }

        public unsafe byte* Invoke(string file)
        {
            if (this.invoked) throw new InvalidOperationException();

            this.invoked = true;

            using FileStream source = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read);

            this.FileSize = source.Length;
            this.ptr = Marshal.AllocCoTaskMem((int)source.Length);

            byte* wrkPtr = (byte*)this.ptr;
            byte[] buff = new byte[4096];
            for (int i = 0; i < source.Length; i += buff.Length)
            {
                int len = source.Read(buff, 0, buff.Length);
                for (int j = 0; j < len; j++)
                    *wrkPtr++ = buff[j];
            }

            return (byte*)this.ptr;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;

            Marshal.FreeCoTaskMem(this.ptr);

            disposed = true;
        }

        ~MemoryMappedFileLoader()
        {
            this.Dispose(false);
        }
    }
}
