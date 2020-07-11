using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LibNMeCab.Benchmarks
{
    public static class Helper
    {
        public static string SeekDicDir(string dic)
        {
            var dicDirPrefix = AppDomain.CurrentDomain.BaseDirectory;

            var dicDir = Path.Combine(dicDirPrefix, "dic", dic);

            while (!Directory.Exists(dicDir))
            {
                dicDirPrefix = Directory.GetParent(dicDirPrefix).FullName;
                dicDir = Path.Combine(dicDirPrefix, "dic", dic);
            }

            return dicDir;
        }
    }
}
