using NMeCab;
using NMeCab.Specialized;
using System;
using System.Linq;

class Program
{
    static void Main()
    {
        Program.UseNotBeAwareOfDictionaly();
        Program.UseWithUserPreparedDictionaly1();
        Program.UseWithUserPreparedDictionaly2();
        Program.UseNBest();
        Program.UseSoftWakachi();
        Program.UseLattice();

        Console.Read();
    }

    static void UseNotBeAwareOfDictionaly()
    {
        Console.WriteLine("----------------------------------------------------------------------");
        Console.WriteLine("Example of using not be aware of dictionary :");
        Console.WriteLine();

        using (var tagger = MeCabIpaDicTagger.Create()) // Taggerインスタンスを生成
        {
            var nodes = tagger.Parse("皇帝の新しい心"); // 形態素解析を実行
            foreach (var node in nodes) // 形態素ノード配列を順に処理
            {
                Console.WriteLine($"表層形：{node.Surface}");
                Console.WriteLine($"読み　：{node.Reading}");
                Console.WriteLine($"品詞　：{node.PartsOfSpeech}");
                Console.WriteLine();
            }
        }
    }

    static void UseWithUserPreparedDictionaly1()
    {
        Console.WriteLine("----------------------------------------------------------------------");
        Console.WriteLine("Example of using user prepared dictionaly 1 (for general dic) :");
        Console.WriteLine();

        var dicDir = "../../../../../dic/ipadic"; // 辞書のパス

        using (var tagger = MeCabTagger.Create(dicDir)) // 汎用のTaggerインスタンスを生成
        {
            var nodes = tagger.Parse("皇帝の新しい心"); // 形態素解析を実行
            foreach (var node in nodes) // 形態素ノード配列を順に処理
            {
                Console.WriteLine($"表層形：{node.Surface}");
                Console.WriteLine($"素性　：{node.Feature}"); // 全ての素性文字列
                Console.WriteLine();
            }
        }
    }

    static void UseWithUserPreparedDictionaly2()
    {
        Console.WriteLine("----------------------------------------------------------------------");
        Console.WriteLine("Example of using user prepared dictionaly 2 (for IPAdic) :");
        Console.WriteLine();

        var dicDir = "../../../../../dic/ipadic"; // 辞書のパス

        using (var tagger = MeCabIpaDicTagger.Create(dicDir)) // IPAdic形式用のTaggerインスタンスを生成
        {
            var nodes = tagger.Parse("皇帝の新しい心"); // 形態素解析を実行
            foreach (var node in nodes) // 形態素ノード配列を順に処理
            {
                Console.WriteLine($"表層形：{node.Surface}");
                Console.WriteLine($"読み　：{node.Reading}"); // 個別の素性
                Console.WriteLine($"品詞　：{node.PartsOfSpeech}"); // 〃
                Console.WriteLine();
            }
        }
    }

    static void UseNBest()
    {
        Console.WriteLine("----------------------------------------------------------------------");
        Console.WriteLine("Example of using N-Best :");
        Console.WriteLine();

        using (var tagger = MeCabIpaDicTagger.Create())
        {
            var results = tagger.ParseNBest("東京大学"); // Nベスト解を取得
            foreach (var nodes in results.Take(5)) // 上位から5件までの解を処理
            {
                foreach (var node in nodes) // 形態素ノード配列を順に処理
                {
                    Console.WriteLine($"表層形：{node.Surface}");
                    Console.WriteLine($"読み　：{node.Reading}");
                    Console.WriteLine($"品詞　：{node.PartsOfSpeech}");
                    Console.WriteLine();
                }

                Console.WriteLine("----------------");
            }
        }
    }

    static void UseSoftWakachi()
    {
        Console.WriteLine("----------------------------------------------------------------------");
        Console.WriteLine("Example of using Soft-Wakachi :");
        Console.WriteLine();

        using (var tagger = MeCabIpaDicTagger.Create())
        {
            var theta = 1f / 800f / 2f; // 温度パラメータ
            var nodes = tagger.ParseSoftWakachi("本部長", theta); // ソフトわかち解を取得

            foreach (var node in nodes.Where(n => n.Prob > 0.1f)) // 周辺確率＞0.1の形態素ノードだけを処理
            {
                Console.WriteLine($"表層形　：{node.Surface}");
                Console.WriteLine($"読み　　：{node.Reading}");
                Console.WriteLine($"品詞　　：{node.PartsOfSpeech}");
                Console.WriteLine($"周辺確率：{node.Prob}");
                Console.WriteLine();
            }

            // さらに、周辺確率の上位から表層形の異なるものの5件までを取得
            var searchWords = nodes.OrderByDescending(n => n.Prob)
                                   .Select(n => n.Surface)
                                   .Distinct()
                                   .Take(5);
            Console.WriteLine($"上位ワード：{string.Join(",", searchWords)}");
        }
    }

    static void UseLattice()
    {
        Console.WriteLine("----------------------------------------------------------------------");
        Console.WriteLine("Example of using Lattice :");
        Console.WriteLine();

        using (var tagger = MeCabIpaDicTagger.Create())
        {
            var prm = new MeCabParam()
            {
                LatticeLevel = MeCabLatticeLevel.Two,
                Theta = 1f / 800f / 2f
            };

            var lattice = tagger.ParseToLattice("東京大学", prm); // ラティスを取得

            // ラティスから、ベスト解を取得し処理
            foreach (var node in lattice.GetBestNodes())
            {
                Console.Write(node.Surface);
                Console.CursorLeft = 10;
                Console.Write(node.Feature);
                Console.WriteLine();
            }

            Console.WriteLine("--------");

            // ラティスから、2番目と3番目のベスト解を取得し処理
            foreach (var result in lattice.GetNBestResults().Skip(1).Take(2))
            {
                foreach (var node in result)
                {
                    Console.Write(node.Surface);
                    Console.CursorLeft = 10;
                    Console.Write(node.Feature);
                    Console.WriteLine();
                }

                Console.WriteLine("----");
            }

            Console.WriteLine("--------");

            // ラティスから、開始位置別の形態素を取得し処理
            for (int i = 0; i < lattice.BeginNodeList.Length - 1; i++)
            {
                for (var node = lattice.BeginNodeList[i]; node != null; node = node.BNext)
                {
                    if (node.Prob <= 0.001f) continue;

                    Console.CursorLeft = i * 2;
                    Console.Write(node.Surface);
                    Console.CursorLeft = 10;
                    Console.Write(node.Prob.ToString("F3"));
                    Console.CursorLeft = 16;
                    Console.Write(node.Feature);
                    Console.WriteLine();
                }
            }

            Console.WriteLine("--------");

            // ラティスから、最終的な累積コストのみを取得し表示
            Console.WriteLine(lattice.EosNode.Cost);
        }
    }
}