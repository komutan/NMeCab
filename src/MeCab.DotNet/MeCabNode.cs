//  MeCab -- Yet Another Part-of-Speech and Morphological Analyzer
//
//  Copyright(C) 2001-2006 Taku Kudo <taku@chasen.org>
//  Copyright(C) 2004-2006 Nippon Telegraph and Telephone Corporation
using System;
using System.Collections.Generic;
using System.Text;
using MeCab.Core;
using System.Globalization;

namespace MeCab
{
    public class MeCabNode : NMeCab.MeCabNodeBase<MeCabNode>
    {
        public new MeCabNodeStat Stat
        {
            get { return (MeCabNodeStat)base.Stat; }
        }
    }
}
