using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using NMeCab;

namespace PerformanceTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Properties.Settings settings = Properties.Settings.Default;
            string targetFile = settings.TargetFile;
            Encoding encoding = Encoding.GetEncoding(settings.TargetEncoding);
            Stopwatch sw = new Stopwatch();

            //開始指示を待機
            Console.WriteLine("Press Enter key to start.");
            Console.ReadLine();

            Console.WriteLine("\t\t\tProcessTime\tTotalMemory");

            //解析準備処理
            GC.Collect();
            sw.Start();
            MeCabTagger tagger = MeCabTagger.Create();
            sw.Stop();
            Console.WriteLine("OpenTagger:\t\t{0:0.000}sec\t{1:#,000}byte",
                              sw.Elapsed.TotalSeconds, GC.GetTotalMemory(false));

            //ファイル読込だけの場合
            using (StreamReader reader = new StreamReader(targetFile, encoding))
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
            using (StreamReader reader = new StreamReader(targetFile, encoding))
            {
                sw.Reset();
                GC.Collect();
                sw.Start();
                for (string line = reader.ReadLine(); line != null; line = reader.ReadLine())
                {
                    MeCabNode node = tagger.ParseToNode(line);
                }
                sw.Stop();
            }
            Console.WriteLine("ParseToNode:\t\t{0:0.000}sec\t{1:#,000}byte",
                              sw.Elapsed.TotalSeconds, GC.GetTotalMemory(false));

            //解析処理（latticeモードの文字列出力）
            tagger.OutPutFormatType = "lattice";
            using (StreamReader reader = new StreamReader(targetFile, encoding))
            {
                sw.Reset();
                GC.Collect();
                sw.Start();
                for (string line = reader.ReadLine(); line != null; line = reader.ReadLine())
                {
                    string ret = tagger.Parse(line);
                }
                sw.Stop();
            }
            Console.WriteLine("Parse(lattice):\t\t{0:0.000}sec\t{1:#,000}byte",
                              sw.Elapsed.TotalSeconds, GC.GetTotalMemory(false));


            //解析処理（Best解5件のNodeの出力）
            tagger.LatticeLevel = MeCabLatticeLevel.One;
            using (StreamReader reader = new StreamReader(targetFile, encoding))
            {
                sw.Reset();
                GC.Collect();
                sw.Start();
                for (string line = reader.ReadLine(); line != null; line = reader.ReadLine())
                {
                    int i = 0;
                    foreach (MeCabNode node in tagger.ParseNBestToNode(line))
                    {
                        if (++i == 5) break;
                    }
                }
                sw.Stop();
            }
            Console.WriteLine("ParseNBestToNode:\t{0:0.000}sec\t{1:#,000}byte",
                              sw.Elapsed.TotalSeconds, GC.GetTotalMemory(false));

            //対象の情報
            using (StreamReader reader = new StreamReader(targetFile, encoding))
            {
                long charCount = 0;
                long lineCount = 0;
                long wordCount = 0;
                for (string line = reader.ReadLine(); line != null; line = reader.ReadLine())
                {
                    charCount += line.Length;
                    lineCount++;
                    MeCabNode node = tagger.ParseToNode(line);
                    for (node = node.Next; node.Next != null; node = node.Next) wordCount++;
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
