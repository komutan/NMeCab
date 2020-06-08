using NMeCab;
using System;
using Xunit;

namespace LibNMeCabTest
{
    public class UniDic21TaggerTest
    {
        [Fact]
        public void NodeProperty()
        {
            var node = new MeCabUniDic21Node()
            {
                Feature = "0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25"
            };

            Assert.Equal("0", node.Pos1);
            Assert.Equal("1", node.Pos2);
            Assert.Equal("2", node.Pos3);
            Assert.Equal("3", node.Pos4);
            Assert.Equal("4", node.CType);
            Assert.Equal("5", node.CForm);
            Assert.Equal("6", node.LForm);
            Assert.Equal("7", node.Lemma);
            Assert.Equal("8", node.Orth);
            Assert.Equal("9", node.Pron);
            Assert.Equal("10", node.OrthBase);
            Assert.Equal("11", node.PronBase);
            Assert.Equal("12", node.Goshu);
            Assert.Equal("13", node.IType);
            Assert.Equal("14", node.IForm);
            Assert.Equal("15", node.FType);
            Assert.Equal("16", node.FForm);
            Assert.Equal("17", node.Kana);
            Assert.Equal("18", node.KanaBase);
            Assert.Equal("19", node.Form);
            Assert.Equal("20", node.FormBase);
            Assert.Equal("21", node.IConType);
            Assert.Equal("22", node.FConType);
            Assert.Equal("23", node.AType);
            Assert.Equal("24", node.AConType);
            Assert.Equal("25", node.AModType);
        }

        [Fact(Skip = "Execute only when the unidic-2.1.2 is prepared")]
        public void ParseTest()
        {
            const string dicDir = "../../../../../dic/unidic-2.1.2";

            using (var tagger = MeCabUniDic21Tagger.Create(dicDir))
            {
                var nodes = tagger.Parse("こおりつけ！");

                var node = nodes[0];

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
        }
    }
}