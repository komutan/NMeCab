using System;
using NMeCab;

namespace NMeCab
{
    public class MeCabLattice
    {
        public MeCabParam Param;
        public MeCabNode EosNode;
        public MeCabNode BosNode;
        public MeCabNode[] EndNodeList;
        public MeCabNode[] BeginNodeList;
        public float Z;
    }
}
