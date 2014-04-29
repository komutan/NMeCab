//  MeCab -- Yet Another Part-of-Speech and Morphological Analyzer
//
//  Copyright(C) 2001-2006 Taku Kudo <taku@chasen.org>
//  Copyright(C) 2004-2006 Nippon Telegraph and Telephone Corporation
using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;

namespace NMeCab
{
    [Serializable]
    public class MeCabFileFormatException : MeCabInvalidFileException
    {
        public int LineNo { get; private set; }

        public string Line { get; private set; }

        public override string Message
        {
            get
            {
                StringBuilder os = new StringBuilder();
                os.Append(base.Message);
                if (this.LineNo > 0) os.AppendFormat("[LineNo:{0}]", this.LineNo);
                if (this.Line != null) os.AppendFormat("[Line:{0}]", this.Line);
                return os.ToString();
            }
        }

        public MeCabFileFormatException(string message, string fileName = null, int lineNo = -1, string line = null)
            : base(message, fileName)
        {
            this.LineNo = lineNo;
            this.Line = line;
        }

        public MeCabFileFormatException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.LineNo = info.GetInt32("LineNo");
            this.Line = info.GetString("Line");
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("LineNo", this.LineNo);
            info.AddValue("Line", this.Line);
        }
    }
}
