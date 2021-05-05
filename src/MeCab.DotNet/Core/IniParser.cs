using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MeCab.Core
{
    public class IniParser
    {
        public char SplitChar { get; set; }
        public char[] SkipChars { get; set; }
        public char[] TrimChars { get; set; }
        public bool IsRewrites { get; set; }

        private readonly Dictionary<string, string> dic = new Dictionary<string, string>();

        public IniParser()
        {
            this.SplitChar = '=';
            this.SkipChars = new char[] { ';', '#' };
            this.TrimChars = new char[] { ' ', '\t' };
        }

        public string this[string key]
        {
            get { return this.dic[key]; }
            set { this.dic[key] = value; }
        }

        public void Load(string fileName, Encoding encoding)
        {
            using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.Read))
            {
                TextReader reader = new StreamReader(fs, encoding);
                this.Load(reader, fileName);
            }
        }

        public void Load(TextReader reader, string fileName = null)
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
                if (!this.IsRewrites && this.dic.ContainsKey(key)) continue;

                string value = line.Substring(eqPos + 1).TrimStart(this.TrimChars);
                this.dic[key] = value;
            }
        }

        public void Clear()
        {
            this.dic.Clear();
        }
    }
}
