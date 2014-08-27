using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NMeCab.Extension;

namespace LibNMeCabTest
{
    [TestClass]
    public class MeCabNodeExtensionTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var node = new NMeCab.MeCabNode()
            {
                Feature = "品詞,品詞細分類1,品詞細分類2,品詞細分類3,活用形,活用型,原形,読み,発音"
            };
            Assert.AreEqual(node.GetPartsOfSpeech(), "品詞");
            Assert.AreEqual(node.GetPartsOfSpeechSection1(), "品詞細分類1");
            Assert.AreEqual(node.GetPartsOfSpeechSection2(), "品詞細分類2");
            Assert.AreEqual(node.GetPartsOfSpeechSection3(), "品詞細分類3");
            Assert.AreEqual(node.GetConjugatedForm(), "活用形");
            Assert.AreEqual(node.GetInflection(), "活用型");
            Assert.AreEqual(node.GetOriginalForm(), "原形");
            Assert.AreEqual(node.GetReading(), "読み");
            Assert.AreEqual(node.GetPronounciation(), "発音");
        }

        [TestMethod]
        public void TestMethod2()
        {
            var node = new NMeCab.MeCabNode()
            {
                Feature = "品詞"
            };
            Assert.AreEqual(node.GetPartsOfSpeech(), "品詞");
            Assert.IsNull(node.GetPartsOfSpeechSection1());
            Assert.IsNull(node.GetPartsOfSpeechSection2());
            Assert.IsNull(node.GetPartsOfSpeechSection3());
            Assert.IsNull(node.GetConjugatedForm());
            Assert.IsNull(node.GetInflection());
            Assert.IsNull(node.GetOriginalForm());
            Assert.IsNull(node.GetReading());
            Assert.IsNull(node.GetPronounciation());
        }

        [TestMethod]
        public void TestMethod3()
        {
            var node = new NMeCab.MeCabNode()
            {
                Feature = null
            };
            Assert.IsNull(node.GetPartsOfSpeech());
            Assert.IsNull(node.GetPartsOfSpeechSection1());
            Assert.IsNull(node.GetPartsOfSpeechSection2());
            Assert.IsNull(node.GetPartsOfSpeechSection3());
            Assert.IsNull(node.GetConjugatedForm());
            Assert.IsNull(node.GetInflection());
            Assert.IsNull(node.GetOriginalForm());
            Assert.IsNull(node.GetReading());
            Assert.IsNull(node.GetPronounciation());
        }
    }
}
