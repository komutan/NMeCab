using NMeCab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace LibNMeCab.Test
{
    public class CommonTaggerTest : IDisposable
    {
        private const string dicDir = "../../../../../dic/ipadic";

        private readonly MeCabTagger tagger;

        public CommonTaggerTest()
        {
            this.tagger = MeCabTagger.Create(dicDir);
        }

        [Fact]
        public void CreateMultiTagger()
        {
            Parallel.For(0, 99, i =>
            {
                var tagger1 = MeCabTagger.Create(dicDir);
                var tagger2 = MeCabTagger.Create(dicDir);
                tagger1.Dispose();
                tagger2.Dispose();
            });
        }

        [Fact]
        public void OneBest()
        {
            Parallel.For(0, 999, i =>
             {
                 var nodes = this.tagger.Parse("すもももももももものうち");

                 Assert.Equal(7, nodes.Length);

                 var node = nodes[0];
                 Assert.Equal("すもも", node.Surface);
                 Assert.Equal("名詞,一般,*,*,*,*,すもも,スモモ,スモモ", node.Feature);
                 Assert.Equal(0, node.BPos);
                 Assert.Equal(3, node.RLength);
                 Assert.Equal(3, node.Length);
                 Assert.Equal(3, node.EPos);

                 node = nodes[1];
                 Assert.Equal("も", node.Surface);
                 Assert.Equal("助詞,係助詞,*,*,*,*,も,モ,モ", node.Feature);
                 Assert.Equal(3, node.BPos);
                 Assert.Equal(1, node.RLength);
                 Assert.Equal(1, node.Length);
                 Assert.Equal(4, node.EPos);

                 node = nodes[2];
                 Assert.Equal("もも", node.Surface);
                 Assert.Equal("名詞,一般,*,*,*,*,もも,モモ,モモ", node.Feature);
                 Assert.Equal(4, node.BPos);
                 Assert.Equal(2, node.RLength);
                 Assert.Equal(2, node.Length);
                 Assert.Equal(6, node.EPos);

                 node = nodes[3];
                 Assert.Equal("も", node.Surface);
                 Assert.Equal("助詞,係助詞,*,*,*,*,も,モ,モ", node.Feature);
                 Assert.Equal(6, node.BPos);
                 Assert.Equal(1, node.RLength);
                 Assert.Equal(1, node.Length);
                 Assert.Equal(7, node.EPos);

                 node = nodes[4];
                 Assert.Equal("もも", node.Surface);
                 Assert.Equal("名詞,一般,*,*,*,*,もも,モモ,モモ", node.Feature);
                 Assert.Equal(7, node.BPos);
                 Assert.Equal(2, node.RLength);
                 Assert.Equal(2, node.Length);
                 Assert.Equal(9, node.EPos);

                 node = nodes[5];
                 Assert.Equal("の", node.Surface);
                 Assert.Equal("助詞,連体化,*,*,*,*,の,ノ,ノ", node.Feature);
                 Assert.Equal(9, node.BPos);
                 Assert.Equal(1, node.RLength);
                 Assert.Equal(1, node.Length);
                 Assert.Equal(10, node.EPos);

                 node = nodes[6];
                 Assert.Equal("うち", node.Surface);
                 Assert.Equal("名詞,非自立,副詞可能,*,*,*,うち,ウチ,ウチ", node.Feature);
                 Assert.Equal(10, node.BPos);
                 Assert.Equal(2, node.RLength);
                 Assert.Equal(2, node.Length);
                 Assert.Equal(12, node.EPos);

                 Assert.True(nodes[0].Prev.IsBest);
                 Assert.True(nodes[1].IsBest);
                 Assert.True(nodes[2].IsBest);
                 Assert.True(nodes[3].IsBest);
                 Assert.True(nodes[4].IsBest);
                 Assert.True(nodes[5].IsBest);
                 Assert.True(nodes[6].IsBest);
                 Assert.True(nodes[6].Next.IsBest);

                 Assert.Equal(MeCabNodeStat.Bos, nodes[0].Prev.Stat);
                 Assert.Equal(MeCabNodeStat.Nor, nodes[0].Stat);
                 Assert.Equal(MeCabNodeStat.Nor, nodes[1].Stat);
                 Assert.Equal(MeCabNodeStat.Nor, nodes[2].Stat);
                 Assert.Equal(MeCabNodeStat.Nor, nodes[3].Stat);
                 Assert.Equal(MeCabNodeStat.Nor, nodes[4].Stat);
                 Assert.Equal(MeCabNodeStat.Nor, nodes[5].Stat);
                 Assert.Equal(MeCabNodeStat.Nor, nodes[6].Stat);
                 Assert.Equal(MeCabNodeStat.Eos, nodes[6].Next.Stat);

                 node = nodes[0].Next;
                 Assert.Same(nodes[1], node);
                 Assert.Same(nodes[0], node.Prev);
                 node = node.Next;
                 Assert.Same(nodes[2], node);
                 Assert.Same(nodes[1], node.Prev);
                 node = node.Next;
                 Assert.Same(nodes[3], node);
                 Assert.Same(nodes[2], node.Prev);
                 node = node.Next;
                 Assert.Same(nodes[4], node);
                 Assert.Same(nodes[3], node.Prev);
                 node = node.Next;
                 Assert.Same(nodes[5], node);
                 Assert.Same(nodes[4], node.Prev);
                 node = node.Next;
                 Assert.Same(nodes[6], node);
                 Assert.Same(nodes[5], node.Prev);
             });
        }

        [Fact]
        public void UnknownSpeech()
        {
            Parallel.For(0, 999, i =>
            {
                var nodes = this.tagger.Parse("今日はエヌメカブを使った");

                Assert.Equal(6, nodes.Length);
                Assert.Equal("今日", nodes[0].Surface);
                Assert.Equal(MeCabNodeStat.Nor, nodes[0].Stat);
                Assert.Equal("は", nodes[1].Surface);
                Assert.Equal(MeCabNodeStat.Nor, nodes[1].Stat);
                Assert.Equal("エヌメカブ", nodes[2].Surface);
                Assert.Equal("名詞,一般,*,*,*,*,*", nodes[2].Feature);
                Assert.Equal(MeCabNodeStat.Unk, nodes[2].Stat);
                Assert.Equal("を", nodes[3].Surface);
                Assert.Equal(MeCabNodeStat.Nor, nodes[3].Stat);
                Assert.Equal("使っ", nodes[4].Surface);
                Assert.Equal(MeCabNodeStat.Nor, nodes[4].Stat);
                Assert.Equal("た", nodes[5].Surface);
                Assert.Equal(MeCabNodeStat.Nor, nodes[5].Stat);

                for (int j = 0; j < nodes.Length; j++)
                {
                    Assert.True(nodes[j].IsBest);
                    Assert.Equal(nodes[j].Prev.EPos, nodes[j].BPos);
                    Assert.Equal(nodes[j].Next.BPos, nodes[j].EPos);
                    Assert.Equal(nodes[j].EPos - nodes[j].BPos, nodes[j].RLength);
                    Assert.Equal(nodes[j].EPos - nodes[j].BPos, nodes[j].Length);
                }
            });
        }

        [Fact]
        public void NBest()
        {
            Parallel.For(0, 999, i =>
            {
                var results = this.tagger.ParseNBest("すもももももももものうち");
                var enumerator = results.GetEnumerator();

                Assert.True(enumerator.MoveNext());
                var nodes1 = enumerator.Current;

                Assert.Equal(7, nodes1.Length);
                Assert.Equal("すもも", nodes1[0].Surface);
                Assert.Equal("名詞,一般,*,*,*,*,すもも,スモモ,スモモ", nodes1[0].Feature);
                Assert.Equal("も", nodes1[1].Surface);
                Assert.Equal("助詞,係助詞,*,*,*,*,も,モ,モ", nodes1[1].Feature);
                Assert.Equal("もも", nodes1[2].Surface);
                Assert.Equal("名詞,一般,*,*,*,*,もも,モモ,モモ", nodes1[2].Feature);
                Assert.Equal("も", nodes1[3].Surface);
                Assert.Equal("助詞,係助詞,*,*,*,*,も,モ,モ", nodes1[3].Feature);
                Assert.Equal("もも", nodes1[4].Surface);
                Assert.Equal("名詞,一般,*,*,*,*,もも,モモ,モモ", nodes1[4].Feature);
                Assert.Equal("の", nodes1[5].Surface);
                Assert.Equal("助詞,連体化,*,*,*,*,の,ノ,ノ", nodes1[5].Feature);
                Assert.Equal("うち", nodes1[6].Surface);
                Assert.Equal("名詞,非自立,副詞可能,*,*,*,うち,ウチ,ウチ", nodes1[6].Feature);

                Assert.True(nodes1[0].Prev.IsBest);
                Assert.True(nodes1[1].IsBest);
                Assert.True(nodes1[2].IsBest);
                Assert.True(nodes1[3].IsBest);
                Assert.True(nodes1[4].IsBest);
                Assert.True(nodes1[5].IsBest);
                Assert.True(nodes1[6].IsBest);
                Assert.True(nodes1[6].Next.IsBest);

                Assert.Equal(MeCabNodeStat.Bos, nodes1[0].Prev.Stat);
                Assert.Equal(MeCabNodeStat.Nor, nodes1[0].Stat);
                Assert.Equal(MeCabNodeStat.Nor, nodes1[1].Stat);
                Assert.Equal(MeCabNodeStat.Nor, nodes1[2].Stat);
                Assert.Equal(MeCabNodeStat.Nor, nodes1[3].Stat);
                Assert.Equal(MeCabNodeStat.Nor, nodes1[4].Stat);
                Assert.Equal(MeCabNodeStat.Nor, nodes1[5].Stat);
                Assert.Equal(MeCabNodeStat.Nor, nodes1[6].Stat);
                Assert.Equal(MeCabNodeStat.Eos, nodes1[6].Next.Stat);

                Assert.True(enumerator.MoveNext());
                var nodes2 = enumerator.Current;
                Assert.NotEmpty(nodes2);

                Assert.True(enumerator.MoveNext());
                var nodes3 = enumerator.Current;
                Assert.NotEmpty(nodes3);

                Assert.True(enumerator.MoveNext());
                var nodes4 = enumerator.Current;
                Assert.NotEmpty(nodes4);

                Assert.True(enumerator.MoveNext());
                var nodes5 = enumerator.Current;
                Assert.NotEmpty(nodes5);

                foreach (var nodesX in results.Take(5))
                {
                    for (int j = 0; j < nodesX.Length; j++)
                    {
                        Assert.NotEmpty(nodesX[j].Surface);
                        Assert.NotEmpty(nodesX[j].Feature);
                        if (j > 0) Assert.Equal(nodesX[j - 1].EPos, nodesX[j].BPos);
                        if (j + 1 < nodesX.Length) Assert.Equal(nodesX[j + 1].BPos, nodesX[j].EPos);
                        Assert.Equal(nodesX[j].EPos - nodesX[j].BPos, nodesX[j].RLength);
                        Assert.Equal(nodesX[j].EPos - nodesX[j].BPos, nodesX[j].Length);
                    }
                }
            });
        }

        [Fact]
        public void SoftWakachi()
        {
            var nodes = this.tagger.ParseSoftWakachi("すもももももももものうち", 0.0007f);

            foreach (var node in nodes)
            {
                Assert.InRange(node.Prob, 0f, 1f);
            }

            Assert.NotEmpty(nodes.Where(n => n.Prob > 0f && n.Prob < 1f));

            var nBestResults = tagger.ParseNBest("すもももももももものうち");
            foreach (var node in nBestResults.Take(10).SelectMany(r => r))
            {
                Assert.Contains(node, nodes, new MeCabNodeComparer());
            }
        }

        [Fact]
        public void SpaceProcessing()
        {
            // スペースのみ
            var nodes = this.tagger.Parse(" ");
            Assert.Empty(nodes);

            // 既知語 後ろスペース
            nodes = this.tagger.Parse("ようこそ ");
            Assert.Single(nodes);
            Assert.Equal("ようこそ", nodes[0].Surface);
            Assert.Equal(0, nodes[0].BPos);
            Assert.Equal(4, nodes[0].EPos);
            Assert.Equal(4, nodes[0].Length);
            Assert.Equal(4, nodes[0].RLength);

            // 未知語 後ろスペース
            nodes = this.tagger.Parse("XXXYYYZZZ ");
            Assert.Single(nodes);
            Assert.Equal("XXXYYYZZZ", nodes[0].Surface);
            Assert.Equal(0, nodes[0].BPos);
            Assert.Equal(9, nodes[0].EPos);
            Assert.Equal(9, nodes[0].Length);
            Assert.Equal(9, nodes[0].RLength);

            // 既知語 前スペース
            nodes = this.tagger.Parse(" ようこそ");
            Assert.Single(nodes);
            Assert.Equal("ようこそ", nodes[0].Surface);
            Assert.Equal(1 - 1, nodes[0].BPos);
            Assert.Equal(5, nodes[0].EPos);
            Assert.Equal(4, nodes[0].Length);
            Assert.Equal(4 + 1, nodes[0].RLength);

            // 未知語 前スペース
            nodes = this.tagger.Parse(" XXXYYYZZZ");
            Assert.Single(nodes);
            Assert.Equal("XXXYYYZZZ", nodes[0].Surface);
            Assert.Equal(1 - 1, nodes[0].BPos);
            Assert.Equal(10, nodes[0].EPos);
            Assert.Equal(9, nodes[0].Length);
            Assert.Equal(10, nodes[0].RLength);

            // 複合
            nodes = this.tagger.Parse(" ようこそ XXXYYYZZZ ");
            Assert.Equal(2, nodes.Length);
            Assert.Equal("ようこそ", nodes[0].Surface);
            Assert.Equal(1 - 1, nodes[0].BPos);
            Assert.Equal(5, nodes[0].EPos);
            Assert.Equal(4, nodes[0].Length);
            Assert.Equal(4 + 1, nodes[0].RLength);
            Assert.Equal("XXXYYYZZZ", nodes[1].Surface);
            Assert.Equal(6 - 1, nodes[1].BPos);
            Assert.Equal(15, nodes[1].EPos);
            Assert.Equal(9, nodes[1].Length);
            Assert.Equal(9 + 1, nodes[1].RLength);
        }

        public void Dispose()
        {
            this.tagger?.Dispose();
        }

        private class MeCabNodeComparer : IEqualityComparer<MeCabNode>
        {
            public bool Equals(MeCabNode x, MeCabNode y)
            {
                return x.Surface == y.Surface
                       && x.Feature == y.Feature
                       && x.BPos == y.BPos
                       && x.RLength == y.RLength;
            }

            public int GetHashCode(MeCabNode obj)
            {
                return obj.Surface.GetHashCode() ^ obj.Feature.GetHashCode();
            }
        }
    }
}
