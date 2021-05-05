//  MeCab -- Yet Another Part-of-Speech and Morphological Analyzer
//
//  Copyright(C) 2001-2006 Taku Kudo <taku@chasen.org>
//  Copyright(C) 2004-2006 Nippon Telegraph and Telephone Corporation
using System;

namespace MeCab
{
#if NET20 || NET35 || NET40 || NET45 || NETSTANDARD2_0 || NETSTANDARD2_1 || NET5_0
    using System.Runtime.Serialization;

    [Serializable]
#endif
    public class MeCabException : Exception
    {
        public MeCabException(string message)
            : base(message)
        { }

        public MeCabException(string message, Exception ex)
            : base(message, ex)
        { }

#if NET20 || NET35 || NET40 || NET45 || NETSTANDARD2_0 || NETSTANDARD2_1 || NET5_0
        public MeCabException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
#endif
    }
}
