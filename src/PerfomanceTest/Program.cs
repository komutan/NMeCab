using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using NMeCab;

namespace PerfomanceTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var dicDir = "../../../../../dic/ipadic";
            var targetFile = "kokoro.txt";
            var encoding = Encoding.UTF8;
            var sw = new Stopwatch();

            //開始指示を待機
            Console.WriteLine("Press Enter key to start.");
            Console.ReadLine();

            Console.WriteLine("\t\t\tProcessTime\tTotalMemory");

            //解析準備処理
            GC.Collect();
            sw.Start();
            var tagger = MeCabIpaDicTagger.Create(dicDir);
            sw.Stop();
            Console.WriteLine("OpenTagger:\t\t{0:0.000}sec\t{1:#,000}byte",
                              sw.Elapsed.TotalSeconds, GC.GetTotalMemory(false));

            //ファイル読込だけの場合
            using (var reader = new StreamReader(targetFile, encoding))
            {
                sw.Reset();
                GC.Collect();
                sw.Start();
                for (string line = reader.ReadLine(); line != null; line = reader.ReadLine())
                {
                }
                sw.Stop();
            }
            Console.WriteLine("ReadLine:\t\t{0:0.000}sec\t{1:#,000}byte",
                              sw.Elapsed.TotalSeconds, GC.GetTotalMemory(false));

            //解析処理（Nodeの出力）
            using (var reader = new StreamReader(targetFile, encoding))
            {
                sw.Reset();
                GC.Collect();
                sw.Start();
                for (string line = reader.ReadLine(); line != null; line = reader.ReadLine())
                {
                    var node = tagger.Parse(line);
                }
                sw.Stop();
            }
            Console.WriteLine("ParseToNode:\t\t{0:0.000}sec\t{1:#,000}byte",
                              sw.Elapsed.TotalSeconds, GC.GetTotalMemory(false));

            //解析処理（素性文字列分解）
            using (var reader = new StreamReader(targetFile, encoding))
            {
                sw.Reset();
                GC.Collect();
                sw.Start();
                for (string line = reader.ReadLine(); line != null; line = reader.ReadLine())
                {
                    var node = tagger.Parse(line);
                    foreach (var item in node)
                    {
                        var a = item.Surface;
                        var b = item.PartsOfSpeech;
                        var c = item.Reading;
                    }
                }
                sw.Stop();
            }
            Console.WriteLine("ParseToText:\t\t{0:0.000}sec\t{1:#,000}byte",
                              sw.Elapsed.TotalSeconds, GC.GetTotalMemory(false));

            //解析処理（Best解5件のNodeの出力）
            using (var reader = new StreamReader(targetFile, encoding))
            {
                sw.Reset();
                GC.Collect();
                sw.Start();
                for (string line = reader.ReadLine(); line != null; line = reader.ReadLine())
                {
                    int i = 0;
                    foreach (var node in tagger.ParseNBestToNode(line))
                    {
                        if (++i == 5) break;
                    }
                }
                sw.Stop();
            }
            Console.WriteLine("ParseNBestToNode:\t{0:0.000}sec\t{1:#,000}byte",
                              sw.Elapsed.TotalSeconds, GC.GetTotalMemory(false));

            //対象の情報
            using (var reader = new StreamReader(targetFile, encoding))
            {
                long charCount = 0;
                long lineCount = 0;
                long wordCount = 0;
                for (string line = reader.ReadLine(); line != null; line = reader.ReadLine())
                {
                    charCount += line.Length;
                    lineCount++;
                    var node = tagger.Parse(line);
                    wordCount += node.Length;
                }
                Console.WriteLine();
                Console.WriteLine("Target: {0} {1:#,000}byte {2:#,000}char {3:#,000}line ({4:#,000}word)",
                                  targetFile, reader.BaseStream.Position, charCount, lineCount, wordCount);
            }

            tagger.Dispose();

            //終了したことを通知
            Console.WriteLine();
            Console.WriteLine("Finish!");
            Console.WriteLine("Press Enter key to close.");
            Console.ReadLine();
        }
    }
}
