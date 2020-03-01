using System;
using Xunit;
using NMeCab;

namespace LibNMeCabTest
{
    public class UnitTest1
    {
        [Fact]
        public void OneBest()
        {
            using (var tagger = MeCabTagger.Create("../../../../../dic/ipadic"))
            {
                var nodes = tagger.Parse("すもももももももものうち");
                Assert.Equal("すもも", nodes[0].Surface);
                Assert.Equal("も", nodes[1].Surface);
                Assert.Equal("もも", nodes[2].Surface);
                Assert.Equal("も", nodes[3].Surface);
                Assert.Equal("もも", nodes[4].Surface);
                Assert.Equal("の", nodes[5].Surface);
                Assert.Equal("うち", nodes[6].Surface);
            }
        }

        [Fact]
        public void NBest()
        {
            using (var tagger = MeCabTagger.Create("../../../../../dic/ipadic"))
            {
                var enumerator = tagger.ParseNBestToNode("すもももももももものうち").GetEnumerator();

                Assert.True(enumerator.MoveNext());
                var nodes1 = enumerator.Current;
                Assert.Equal("すもも", nodes1[0].Surface);
                Assert.Equal("も", nodes1[1].Surface);
                Assert.Equal("もも", nodes1[2].Surface);
                Assert.Equal("も", nodes1[3].Surface);
                Assert.Equal("もも", nodes1[4].Surface);
                Assert.Equal("の", nodes1[5].Surface);
                Assert.Equal("うち", nodes1[6].Surface);
                Assert.True(nodes1[0].IsBest);
                Assert.True(nodes1[1].IsBest);
                Assert.True(nodes1[2].IsBest);
                Assert.True(nodes1[3].IsBest);
                Assert.True(nodes1[4].IsBest);
                Assert.True(nodes1[5].IsBest);
                Assert.True(nodes1[6].IsBest);

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
        }

        [Fact]
        public void IpaDic()
        {
            using (var tagger = MeCabIpaDicTagger.Create("../../../../../dic/ipadic"))
            {
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
    }
}
