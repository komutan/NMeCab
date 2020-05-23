using NMeCab;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Xunit;

namespace LibNMeCabTest
{
    public class UnitTest1
    {
        [Fact]
        public void OneBest()
        {
            using var tagger = MeCabTagger.Create("../../../../../dic/ipadic");
            var nodes = tagger.Parse("すもももももももものうち");

            Assert.Equal(7, nodes.Length);
            Assert.Equal("すもも", nodes[0].Surface);
            Assert.Equal("名詞,一般,*,*,*,*,すもも,スモモ,スモモ", nodes[0].Feature);
            Assert.Equal("も", nodes[1].Surface);
            Assert.Equal("助詞,係助詞,*,*,*,*,も,モ,モ", nodes[1].Feature);
            Assert.Equal("もも", nodes[2].Surface);
            Assert.Equal("名詞,一般,*,*,*,*,もも,モモ,モモ", nodes[2].Feature);
            Assert.Equal("も", nodes[3].Surface);
            Assert.Equal("助詞,係助詞,*,*,*,*,も,モ,モ", nodes[3].Feature);
            Assert.Equal("もも", nodes[4].Surface);
            Assert.Equal("名詞,一般,*,*,*,*,もも,モモ,モモ", nodes[4].Feature);
            Assert.Equal("の", nodes[5].Surface);
            Assert.Equal("助詞,連体化,*,*,*,*,の,ノ,ノ", nodes[5].Feature);
            Assert.Equal("うち", nodes[6].Surface);
            Assert.Equal("名詞,非自立,副詞可能,*,*,*,うち,ウチ,ウチ", nodes[6].Feature);

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

            var tmp = nodes[0].Next;
            Assert.Same(nodes[1], tmp);
            tmp = tmp.Next;
            Assert.Same(nodes[2], tmp);
            tmp = tmp.Next;
            Assert.Same(nodes[3], tmp);
            tmp = tmp.Next;
            Assert.Same(nodes[4], tmp);
            tmp = tmp.Next;
            Assert.Same(nodes[5], tmp);
            tmp = tmp.Next;
            Assert.Same(nodes[6], tmp);
        }

        [Fact]
        public void NBest()
        {
            using var tagger = MeCabTagger.Create("../../../../../dic/ipadic");
            var enumerator = tagger.ParseNBest("すもももももももものうち").GetEnumerator();

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
        }

        [Fact]
        public void SoftWakachi()
        {
            using var tagger = MeCabTagger.Create("../../../../../dic/ipadic");
            var nodes = tagger.ParseSoftWakachi("すもももももももものうち", 0.0007f);

            foreach (var node in nodes)
            {
                Assert.InRange(node.Prob, 0f, 1f);
            }

            Assert.NotEmpty(nodes.Where(n => n.Prob > 0f));
            Assert.NotEmpty(nodes.Where(n => n.Prob < 1f));

            var nBestResults = tagger.ParseNBest("すもももももももものうち");
            foreach (var node in nBestResults.Take(10).SelectMany(r => r))
            {
                Assert.Contains(node, nodes, new MeCabNodeComparer());
            }
        }

        [Fact]
        public void SpaceProcessing()
        {
            using var tagger = MeCabTagger.Create("../../../../../dic/ipadic");

            // スペースのみ
            var nodes = tagger.Parse(" ");
            Assert.Empty(nodes);

            // 既知語 後ろスペース
            nodes = tagger.Parse("ようこそ ");
            Assert.Single(nodes);
            Assert.Equal("ようこそ", nodes[0].Surface);
            Assert.Equal(0, nodes[0].BPos);
            Assert.Equal(4, nodes[0].EPos);
            Assert.Equal(4, nodes[0].Length);
            Assert.Equal(4, nodes[0].RLength);

            // 未知語 後ろスペース
            nodes = tagger.Parse("XXXYYYZZZ ");
            Assert.Single(nodes);
            Assert.Equal("XXXYYYZZZ", nodes[0].Surface);
            Assert.Equal(0, nodes[0].BPos);
            Assert.Equal(9, nodes[0].EPos);
            Assert.Equal(9, nodes[0].Length);
            Assert.Equal(9, nodes[0].RLength);

            // 既知語 前スペース
            nodes = tagger.Parse(" ようこそ");
            Assert.Single(nodes);
            Assert.Equal("ようこそ", nodes[0].Surface);
            Assert.Equal(1 - 1, nodes[0].BPos);
            Assert.Equal(5, nodes[0].EPos);
            Assert.Equal(4, nodes[0].Length);
            Assert.Equal(4 + 1, nodes[0].RLength);

            // 未知語 前スペース
            nodes = tagger.Parse(" XXXYYYZZZ");
            Assert.Single(nodes);
            Assert.Equal("XXXYYYZZZ", nodes[0].Surface);
            Assert.Equal(1 - 1, nodes[0].BPos);
            Assert.Equal(10, nodes[0].EPos);
            Assert.Equal(9, nodes[0].Length);
            Assert.Equal(10, nodes[0].RLength);

            // 複合
            nodes = tagger.Parse(" ようこそ XXXYYYZZZ ");
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

        [Fact]
        public void IpaDic()
        {
            using var tagger = MeCabIpaDicTagger.Create("../../../../../dic/ipadic");
            var node = tagger.Parse("すもも")[0];
            Assert.Equal("名詞", node.PartsOfSpeech);
            Assert.Equal("一般", node.PartsOfSpeechSection1);
            Assert.Equal("*", node.PartsOfSpeechSection2);
            Assert.Equal("*", node.PartsOfSpeechSection3);
            Assert.Equal("*", node.ConjugatedForm);
            Assert.Equal("*", node.Inflection);
            Assert.Equal("すもも", node.OriginalForm);
            Assert.Equal("スモモ", node.Reading);
            Assert.Equal("スモモ", node.Pronounciation);
        }

        [Fact]
        public void UniDic21()
        {
            using var tagger = MeCabUniDic21Tagger.Create("../../../../../dic/unidic-2.1.2");
            var node = tagger.Parse("こおりつけ！")[0];

            Assert.Equal("動詞", node.Pos1); // 品詞大分類
            Assert.Equal("一般", node.Pos2); // 品詞中分類
            Assert.Equal("*", node.Pos3); // 品詞小分類
            Assert.Equal("*", node.Pos4); // 品詞細分類

            Assert.Equal("五段-カ行", node.CType); // 活用型
            Assert.Equal("命令形", node.CForm); // 活用形

            Assert.Equal("コオリツク", node.LForm); // 語彙素読み
            Assert.Equal("凍り付く", node.Lemma); // 語彙素

            Assert.Equal("こおりつけ", node.Orth); // 書字形出現形 平仮名表記
            Assert.Equal("コーリツケ", node.Pron); // 発音形出現形 発音形だと「コーリ」と長音記号を使って表記
            Assert.Equal("コオリツケ", node.Kana); // 仮名形出現形 「凍り」の仮名形は「コオリ」

            Assert.Equal("和", node.Goshu); // 語種

            Assert.Equal("こおりつく", node.OrthBase); // 書字形基本形
            Assert.Equal("コーリツク", node.PronBase); // 発音形基本形
            Assert.Equal("コオリツク", node.KanaBase); // 仮名形基本形
            Assert.Equal("コオリツク", node.FormBase); // 語形基本形
        }

        [Fact]
        public void UniDic22()
        {
            using var tagger = MeCabUniDic22Tagger.Create("../../../../../dic/unidic-cwj-2.3.0");
            var node = tagger.Parse("こおりつけ！")[0];

            Assert.Equal("動詞", node.Pos1); // 品詞大分類
            Assert.Equal("一般", node.Pos2); // 品詞中分類
            Assert.Equal("*", node.Pos3); // 品詞小分類
            Assert.Equal("*", node.Pos4); // 品詞細分類

            Assert.Equal("五段-カ行", node.CType); // 活用型
            Assert.Equal("命令形", node.CForm); // 活用形

            Assert.Equal("コオリツク", node.LForm); // 語彙素読み
            Assert.Equal("凍り付く", node.Lemma); // 語彙素

            Assert.Equal("こおりつけ", node.Orth); // 書字形出現形 平仮名表記
            Assert.Equal("コーリツケ", node.Pron); // 発音形出現形 発音形だと「コーリ」と長音記号を使って表記
            Assert.Equal("コオリツケ", node.Kana); // 仮名形出現形 「凍り」の仮名形は「コオリ」

            Assert.Equal("和", node.Goshu); // 語種

            Assert.Equal("こおりつく", node.OrthBase); // 書字形基本形
            Assert.Equal("コーリツク", node.PronBase); // 発音形基本形
            Assert.Equal("コオリツク", node.KanaBase); // 仮名形基本形
            Assert.Equal("コオリツク", node.FormBase); // 語形基本形

            Assert.Matches(@"^\d+$", node.LId); // 語彙表ID
            Assert.Matches(@"^\d+$", node.LemmaId); // 語彙素ID
        }

        [Theory]
        [InlineData(@"a", new[] { "a" })]
        [InlineData(@"a,b", new[] { "a", "b" })]
        [InlineData(@"a,b,c", new[] { "a", "b", "c" })]
        [InlineData(@"a,b,c,d,e", new[] { "a", "b", "c", "d", "e" })]
        [InlineData(@"""a""", new[] { "a" })]
        [InlineData(@"""a"",""b""", new[] { "a", "b" })]
        [InlineData(@"""a"",""b"",""c""", new[] { "a", "b", "c" })]
        [InlineData(@"""a"",""b"",""c"",""d"",""e""", new[] { "a", "b", "c", "d", "e" })]
        [InlineData(@"""a,b"",c,d,e", new[] { "a,b", "c", "d", "e" })]
        [InlineData(@"a,""b,c,d"",e", new[] { "a", "b,c,d", "e" })]
        [InlineData(@"a,b,c,""d,e""", new[] { "a", "b", "c", "d,e" })]
        [InlineData(@"""a,b,c,d,e""", new[] { "a,b,c,d,e" })]
        [InlineData(@"""a."",""b."",""c.""", new[] { "a.", "b.", "c." })]
        [InlineData(@""".a"","".b"","".c""", new[] { ".a", ".b", ".c" })]
        [InlineData(@""""",b,c", new[] { @"""", "b", "c" })]
        [InlineData(@"a,"""",c", new[] { "a", @"""", "c" })]
        [InlineData(@"a,b,""""", new[] { "a", "b", @"""" })]
        [InlineData(@"""""a,b,c", new[] { @"""a", "b", "c" })]
        [InlineData(@"a"""",b,c", new[] { @"a""", "b", "c" })]
        [InlineData(@"a,""""b"""",c", new[] { "a", @"""b""", "c" })]
        [InlineData(@"a,b,""""c", new[] { "a", "b", @"""c" })]
        [InlineData(@"a,b,c""""", new[] { "a", "b", @"c""" })]
        public void ParseCsv(string input, string[] expected)
        {
            var actual = NMeCab.Core.StrUtils.SplitCsvRow(input, 1, 1);
            Assert.Equal(expected, actual);
        }

        class MeCabNodeComparer : IEqualityComparer<MeCabNodeSuperBase>
        {
            public bool Equals([AllowNull] MeCabNodeSuperBase x, [AllowNull] MeCabNodeSuperBase y)
            {
                return x.Surface == y.Surface && x.Feature == y.Feature;
            }

            public int GetHashCode([DisallowNull] MeCabNodeSuperBase obj)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}
