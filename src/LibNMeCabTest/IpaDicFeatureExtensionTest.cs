using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NMeCab.Extension.IpaDic;

namespace LibNMeCabTest
{
    [TestClass]
    public class IpaDicFeatureExtensionTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var node = new NMeCab.MeCabNode()
            {
                Feature = "品詞,品詞細分類1,品詞細分類2,品詞細分類3,活用形,活用型,原形,読み,発音"
            };
            Assert.AreEqual("品詞", node.GetPartsOfSpeech());
            Assert.AreEqual("品詞細分類1", node.GetPartsOfSpeechSection1());
            Assert.AreEqual("品詞細分類2", node.GetPartsOfSpeechSection2());
            Assert.AreEqual("品詞細分類3", node.GetPartsOfSpeechSection3());
            Assert.AreEqual("活用形", node.GetConjugatedForm());
            Assert.AreEqual("活用型", node.GetInflection());
            Assert.AreEqual("原形", node.GetOriginalForm());
            Assert.AreEqual("読み", node.GetReading());
            Assert.AreEqual("発音", node.GetPronounciation());
        }

        [TestMethod]
        public void TestMethod2()
        {
            var node = new NMeCab.MeCabNode()
            {
                Feature = ",,,,,,,,"
            };
            Assert.AreEqual("", node.GetPartsOfSpeech());
            Assert.AreEqual("", node.GetPartsOfSpeechSection1());
            Assert.AreEqual("", node.GetPartsOfSpeechSection2());
            Assert.AreEqual("", node.GetPartsOfSpeechSection3());
            Assert.AreEqual("", node.GetConjugatedForm());
            Assert.AreEqual("", node.GetInflection());
            Assert.AreEqual("", node.GetOriginalForm());
            Assert.AreEqual("", node.GetReading());
            Assert.AreEqual("", node.GetPronounciation());
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

        [TestMethod]
        public void TestMethod4()
        {
            var node = new NMeCab.MeCabNode()
            {
                Feature = ""
            };
            Assert.AreEqual("", node.GetPartsOfSpeech());
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
        public void TestMethod5()
        {
            var node = new NMeCab.MeCabNode()
            {
                Feature = "品詞"
            };
            Assert.AreEqual("品詞", node.GetPartsOfSpeech());
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
        public void TestMethod6()
        {
            var node = new NMeCab.MeCabNode()
            {
                Feature = "品詞,品詞細分類1"
            };
            Assert.AreEqual("品詞", node.GetPartsOfSpeech());
            Assert.AreEqual("品詞細分類1", node.GetPartsOfSpeechSection1());
            Assert.IsNull(node.GetPartsOfSpeechSection2());
            Assert.IsNull(node.GetPartsOfSpeechSection3());
            Assert.IsNull(node.GetConjugatedForm());
            Assert.IsNull(node.GetInflection());
            Assert.IsNull(node.GetOriginalForm());
            Assert.IsNull(node.GetReading());
            Assert.IsNull(node.GetPronounciation());
        }

        [TestMethod]
        public void TestMethod7()
        {
            var node = new NMeCab.MeCabNode()
            {
                Feature = "品詞,品詞細分類1,"
            };
            Assert.AreEqual("品詞", node.GetPartsOfSpeech());
            Assert.AreEqual("品詞細分類1", node.GetPartsOfSpeechSection1());
            Assert.AreEqual("", node.GetPartsOfSpeechSection2());
            Assert.IsNull(node.GetPartsOfSpeechSection3());
            Assert.IsNull(node.GetConjugatedForm());
            Assert.IsNull(node.GetInflection());
            Assert.IsNull(node.GetOriginalForm());
            Assert.IsNull(node.GetReading());
            Assert.IsNull(node.GetPronounciation());
        }
    }
}
