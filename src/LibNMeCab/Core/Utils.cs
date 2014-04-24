//  MeCab -- Yet Another Part-of-Speech and Morphological Analyzer
//
//  Copyright(C) 2001-2006 Taku Kudo <taku@chasen.org>
//  Copyright(C) 2004-2006 Nippon Telegraph and Telephone Corporation
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace NMeCab.Core
{
    public static class Utils
    {
        public static double LogSumExp(double x, double y, bool flg)
        {
            const double MinusLogEpsilon = 50.0;

            if (flg) return y;  // init mode
            double vMin = Math.Min(x, y);
            double vMax = Math.Max(x, y);
            if (vMax > vMin + MinusLogEpsilon)
                return vMax;
            else
                return vMax + Math.Log(Math.Exp(vMin - vMax) + 1.0);
        }
    }
}
