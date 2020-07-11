using BenchmarkDotNet.Attributes;
using NMeCab;
using System;
using System.IO;
using System.Text;

namespace LibNMeCab.Benchmarks
{
    public class ParsersBench
    {
        [Params("ipadic", "unidic-cwj-2.2.0")]
        public string Dic { get; set; }

        [Params("kokoro.txt")]
        public string TargetText { get; set; }

        private MeCabTagger tagger;

        [GlobalSetup]
        public void GlobalSetup()
        {
            var dicDir = Helper.SeekDicDir(this.Dic);
            Console.WriteLine($"Open Dictionaly: {dicDir}");
            this.tagger = MeCabTagger.Create(dicDir);
        }

        [Benchmark]
        public void ReadLine()
        {
            using (var reader = new StreamReader(this.TargetText, Encoding.UTF8))
            {
                for (var line = reader.ReadLine(); line != null; line = reader.ReadLine())
                {
                }
            }
        }

        [Benchmark(Baseline = true)]
        public MeCabNode[] ParseToNode()
        {
            MeCabNode[] dummy = null;

            using (var reader = new StreamReader(this.TargetText, Encoding.UTF8))
            {
                for (var line = reader.ReadLine(); line != null; line = reader.ReadLine())
                {
                    dummy = this.tagger.Parse(line);
                }
            }

            return dummy;
        }

        [Benchmark]
        public string ParseToText()
        {
            string dummyA = null;
            string dummyB = null;

            using (var reader = new StreamReader(this.TargetText, Encoding.UTF8))
            {
                for (var line = reader.ReadLine(); line != null; line = reader.ReadLine())
                {
                    var node = this.tagger.Parse(line);
                    foreach (var item in node)
                    {
                        dummyA = item.Surface;
                        dummyB = item.Feature;
                    }
                }
            }

            return dummyA + dummyB;
        }

        [Benchmark]
        public MeCabNode[] ParseNBestToNode()
        {
            MeCabNode[] dummy = null;

            using (var reader = new StreamReader(this.TargetText, Encoding.UTF8))
            {
                for (var line = reader.ReadLine(); line != null; line = reader.ReadLine())
                {
                    int i = 0;
                    foreach (var nodes in tagger.ParseNBest(line))
                    {
                        if (++i == 5) break;

                        dummy = nodes;
                    }
                }
            }

            return dummy;
        }

        [GlobalCleanup]
        public void GlobalCleanup()
        {
            this.tagger?.Dispose();
            Console.WriteLine("Close Dictionaly");
        }
    }
}
