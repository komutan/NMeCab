//  MeCab -- Yet Another Part-of-Speech and Morphological Analyzer
//
//  Copyright(C) 2001-2006 Taku Kudo <taku@chasen.org>
//  Copyright(C) 2004-2006 Nippon Telegraph and Telephone Corporation
using System;
using System.Collections.Generic;
using System.Text;

namespace NMeCab
{
    public class MeCabInvalidFileException : MeCabException
    {
        public string FileName { get; private set; }

        public override string Message
        {
            get
            {
                StringBuilder os = new StringBuilder();
                os.Append(base.Message);
                if (this.FileName != null) os.AppendFormat("[FileName:{0}]", this.FileName);
                return os.ToString();
            }
        }

        public MeCabInvalidFileException(string message, string fileName)
            : base(message)
        {
            this.FileName = fileName;
        }
    }
}
