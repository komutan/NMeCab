using System;
using NMeCab;

namespace SampleCode
{
    class Program
    {
        static void Main()
        {
            using (var tagger = NMeCabIpaDic.CreateTagger())
            {
                var nodes = tagger.Parse("皇帝の新しい心");
                foreach (var node in nodes)
                {
                    Console.WriteLine("表層系：" + node.Surface);
                    Console.WriteLine("読み　：" + node.Reading);
                    Console.WriteLine("品詞　：" + node.PartsOfSpeech);
                    Console.WriteLine();
                }
            }
        }
    }
}
