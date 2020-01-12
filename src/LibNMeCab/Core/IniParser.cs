using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections.Specialized;

namespace NMeCab.Core
{
    public class IniParser
    {
        public char SplitChar { get; set; }
        public char[] SkipChars { get; set; }
        public char[] TrimChars { get; set; }
        public bool IsRewrites { get; set; }

        public IniParser()
        {
            this.SplitChar = '=';
            this.SkipChars = new char[] { ';', '#' };
            this.TrimChars = new char[] { ' ', '\t' };
        }

        public void Load(NameValueCollection store, string fileName, Encoding encoding)
        {
            using (TextReader reader = new StreamReader(fileName, encoding))
                this.Load(store, reader, fileName);
        }

        public void Load(NameValueCollection store, TextReader reader, string fileName = null)
        {
            int lineNo = 0;
            for (string line = reader.ReadLine(); line != null; line = reader.ReadLine())
            {
                lineNo++;

                line = line.Trim(this.TrimChars);
                if (line == "" || Array.IndexOf<char>(this.SkipChars, line[0]) != -1) continue;

                int eqPos = line.IndexOf(this.SplitChar);
                if (eqPos <= 0) throw new MeCabFileFormatException("Format error.", fileName, lineNo, line);

                string key = line.Substring(0, eqPos).TrimEnd(this.TrimChars);
                if (!this.IsRewrites && store[key] != null) continue;

                string value = line.Substring(eqPos + 1).TrimStart(this.TrimChars);
                store[key] = value;
            }
        }
    }
}
