using NMeCab.Specialized;
using Xunit;

namespace LibNMeCab.Test
{
    public class IpaDicTaggerTest
    {
        [Fact]
        public void NodeProperty()
        {
            var node = new MeCabIpaDicNode()
            {
                Feature = "1,2,3,4,5,6,7,8,9"
            };

            Assert.Equal("1", node.PartsOfSpeech);
            Assert.Equal("2", node.PartsOfSpeechSection1);
            Assert.Equal("3", node.PartsOfSpeechSection2);
            Assert.Equal("4", node.PartsOfSpeechSection3);
            Assert.Equal("5", node.ConjugatedForm);
            Assert.Equal("6", node.Inflection);
            Assert.Equal("7", node.OriginalForm);
            Assert.Equal("8", node.Reading);
            Assert.Equal("9", node.Pronounciation);
        }

        [Fact]
        public void ParseTest()
        {
            const string dicDir = "../../../../../dic/ipadic";

            using (var tagger = MeCabIpaDicTagger.Create(dicDir))
            {
                var nodes = tagger.Parse("東京へ行け");

                var node1 = nodes[0];
                Assert.Equal("名詞", node1.PartsOfSpeech);
                Assert.Equal("固有名詞", node1.PartsOfSpeechSection1);
                Assert.Equal("地域", node1.PartsOfSpeechSection2);
                Assert.Equal("一般", node1.PartsOfSpeechSection3);
                Assert.Equal("*", node1.ConjugatedForm);
                Assert.Equal("*", node1.Inflection);
                Assert.Equal("東京", node1.OriginalForm);
                Assert.Equal("トウキョウ", node1.Reading);
                Assert.Equal("トーキョー", node1.Pronounciation);

                var node2 = nodes[2];
                Assert.Equal("動詞", node2.PartsOfSpeech);
                Assert.Equal("自立", node2.PartsOfSpeechSection1);
                Assert.Equal("*", node2.PartsOfSpeechSection2);
                Assert.Equal("*", node2.PartsOfSpeechSection3);
                Assert.Equal("五段・カ行促音便", node2.ConjugatedForm);
                Assert.Equal("命令ｅ", node2.Inflection);
                Assert.Equal("行く", node2.OriginalForm);
                Assert.Equal("イケ", node2.Reading);
                Assert.Equal("イケ", node2.Pronounciation);
            }
        }
    }
}
