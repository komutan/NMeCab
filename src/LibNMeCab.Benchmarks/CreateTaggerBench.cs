using BenchmarkDotNet.Attributes;
using NMeCab;
using System;

namespace LibNMeCab.Benchmarks
{
    public class CreateTaggerBench
    {
        [Params("ipadic", "unidic-cwj-2.2.0")]
        public string Dic { get; set; }

        private string dicDir;

        [GlobalSetup]
        public void GlobalSetup()
        {
            this.dicDir = Helper.SeekDicDir(this.Dic);
            Console.WriteLine($"Target Dictionaly: {this.dicDir}");
        }

        [Benchmark]
        public MeCabTagger CreateAndDisposeTagger()
        {
            var tagger = MeCabTagger.Create(this.dicDir);
            tagger.Dispose();
            return tagger;
        }
    }
}
