
using System;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace MeCab
{
    [TestFixture]
    public class MeCabTaggerTest
    {
        /// <summary>
        /// 複数個のインスタンスを作っても例外が発生しないことの確認。
        /// 従来、MemoryMappedFileに起因して、辞書が使用中という旨のIOExceptionが発生していた。
        /// net45はMemoryMappedFileを利用するので確認可能。netcoreapp2.1は元々利用しないので従来から成功する。
        /// </summary>
        [Test]
        public void CreateMulti()
        {
            using var tagger1 = MeCabTagger.Create();
            using var tagger2 = MeCabTagger.Create();

            GC.KeepAlive(tagger1);
            GC.KeepAlive(tagger2);
        }

        [Test]
        public void OneBest()
        {
            using var tagger = MeCabTagger.Create();

            var nodes = tagger.ParseToNodes("すもももももももものうち").ToArray();

            Assert.AreEqual(9, nodes.Length);

            var node = nodes[1];
            Assert.AreEqual("すもも", node.Surface);
            Assert.AreEqual("名詞,一般,*,*,*,*,すもも,スモモ,スモモ", node.Feature);
            Assert.AreEqual(0, node.BPos);
            Assert.AreEqual(3, node.RLength);
            Assert.AreEqual(3, node.Length);
            Assert.AreEqual(3, node.EPos);

            node = nodes[2];
            Assert.AreEqual("も", node.Surface);
            Assert.AreEqual("助詞,係助詞,*,*,*,*,も,モ,モ", node.Feature);
            Assert.AreEqual(3, node.BPos);
            Assert.AreEqual(1, node.RLength);
            Assert.AreEqual(1, node.Length);
            Assert.AreEqual(4, node.EPos);

            node = nodes[3];
            Assert.AreEqual("もも", node.Surface);
            Assert.AreEqual("名詞,一般,*,*,*,*,もも,モモ,モモ", node.Feature);
            Assert.AreEqual(4, node.BPos);
            Assert.AreEqual(2, node.RLength);
            Assert.AreEqual(2, node.Length);
            Assert.AreEqual(6, node.EPos);

            node = nodes[4];
            Assert.AreEqual("も", node.Surface);
            Assert.AreEqual("助詞,係助詞,*,*,*,*,も,モ,モ", node.Feature);
            Assert.AreEqual(6, node.BPos);
            Assert.AreEqual(1, node.RLength);
            Assert.AreEqual(1, node.Length);
            Assert.AreEqual(7, node.EPos);

            node = nodes[5];
            Assert.AreEqual("もも", node.Surface);
            Assert.AreEqual("名詞,一般,*,*,*,*,もも,モモ,モモ", node.Feature);
            Assert.AreEqual(7, node.BPos);
            Assert.AreEqual(2, node.RLength);
            Assert.AreEqual(2, node.Length);
            Assert.AreEqual(9, node.EPos);

            node = nodes[6];
            Assert.AreEqual("の", node.Surface);
            Assert.AreEqual("助詞,連体化,*,*,*,*,の,ノ,ノ", node.Feature);
            Assert.AreEqual(9, node.BPos);
            Assert.AreEqual(1, node.RLength);
            Assert.AreEqual(1, node.Length);
            Assert.AreEqual(10, node.EPos);

            node = nodes[7];
            Assert.AreEqual("うち", node.Surface);
            Assert.AreEqual("名詞,非自立,副詞可能,*,*,*,うち,ウチ,ウチ", node.Feature);
            Assert.AreEqual(10, node.BPos);
            Assert.AreEqual(2, node.RLength);
            Assert.AreEqual(2, node.Length);
            Assert.AreEqual(12, node.EPos);

            Assert.True(nodes[0].IsBest);
            Assert.True(nodes[1].IsBest);
            Assert.True(nodes[2].IsBest);
            Assert.True(nodes[3].IsBest);
            Assert.True(nodes[4].IsBest);
            Assert.True(nodes[5].IsBest);
            Assert.True(nodes[6].IsBest);
            Assert.True(nodes[7].IsBest);
            Assert.True(nodes[8].IsBest);

            Assert.AreEqual(MeCabNodeStat.Bos, nodes[0].Stat);
            Assert.AreEqual(MeCabNodeStat.Nor, nodes[1].Stat);
            Assert.AreEqual(MeCabNodeStat.Nor, nodes[2].Stat);
            Assert.AreEqual(MeCabNodeStat.Nor, nodes[3].Stat);
            Assert.AreEqual(MeCabNodeStat.Nor, nodes[4].Stat);
            Assert.AreEqual(MeCabNodeStat.Nor, nodes[5].Stat);
            Assert.AreEqual(MeCabNodeStat.Nor, nodes[6].Stat);
            Assert.AreEqual(MeCabNodeStat.Nor, nodes[7].Stat);
            Assert.AreEqual(MeCabNodeStat.Eos, nodes[8].Stat);

            Assert.AreSame(nodes[1], nodes[0].Next);
            Assert.AreSame(nodes[2], nodes[1].Next);
            Assert.AreSame(nodes[3], nodes[2].Next);
            Assert.AreSame(nodes[4], nodes[3].Next);
            Assert.AreSame(nodes[5], nodes[4].Next);
            Assert.AreSame(nodes[6], nodes[5].Next);
            Assert.AreSame(nodes[7], nodes[6].Next);

            Assert.AreSame(nodes[0], nodes[1].Prev);
            Assert.AreSame(nodes[1], nodes[2].Prev);
            Assert.AreSame(nodes[2], nodes[3].Prev);
            Assert.AreSame(nodes[3], nodes[4].Prev);
            Assert.AreSame(nodes[4], nodes[5].Prev);
            Assert.AreSame(nodes[5], nodes[6].Prev);
            Assert.AreSame(nodes[5], nodes[6].Prev);
            Assert.AreSame(nodes[6], nodes[7].Prev);
        }
    }
}
