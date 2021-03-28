//  MeCab -- Yet Another Part-of-Speech and Morphological Analyzer
//
//  Copyright(C) 2001-2006 Taku Kudo <taku@chasen.org>
//  Copyright(C) 2004-2006 Nippon Telegraph and Telephone Corporation
using System;
using System.Text;

namespace MeCab
{
#if NET20 || NET35 || NET40 || NET45 || NETSTANDARD2_0 || NETSTANDARD2_1 || NET5_0
    using System.Runtime.Serialization;
    using System.Security.Permissions;

    [Serializable]
#endif
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

#if NET20 || NET35 || NET40 || NET45 || NETSTANDARD2_0 || NETSTANDARD2_1 || NET5_0
        public MeCabInvalidFileException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.FileName = info.GetString("FileName");
        }

#if !NET5_0
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
#endif
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("FileName", this.FileName);
        }
#endif
    }
}
