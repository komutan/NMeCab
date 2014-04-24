//  MeCab -- Yet Another Part-of-Speech and Morphological Analyzer
//
//  Copyright(C) 2001-2006 Taku Kudo <taku@chasen.org>
//  Copyright(C) 2004-2006 Nippon Telegraph and Telephone Corporation
using System;
using System.Collections.Generic;
using System.Text;

namespace NMeCab.Core
{
    public class MeCabPath
    {
        #region  Const/Field/Property

        public MeCabNode RNode { get; set; }

        public MeCabPath RNext { get; set; }

        public MeCabNode LNode { get; set; }

        public MeCabPath LNext { get; set; }

        public int Cost { get; set; }

        public float Prob { get; set; }

        #endregion

        #region Method

        public override string ToString()
        {
            return string.Format("[Cost:{0}][Prob:{1}][LNode:{2}][RNode;{3}]",
                                 this.Cost,
                                 this.Prob,
                                 this.LNode,
                                 this.RNode);

        }

        #endregion
    }
}
