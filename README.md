# 形態素解析エンジンNMeCab

<img src="icon/NMeCab-icon.svg">

## 目次

<!-- TOC -->

- [形態素解析エンジンNMeCab](#形態素解析エンジンnmecab)
    - [目次](#目次)
    - [リポジトリ移転について](#リポジトリ移転について)
    - [これは何？](#これは何)
    - [そもそも形態素解析とは？](#そもそも形態素解析とは)
    - [NuGet](#nuget)
    - [使い方](#使い方)
        - [最も簡単な使い方](#最も簡単な使い方)
        - [辞書を自分で用意して使用する](#辞書を自分で用意して使用する)
        - [ユーザー辞書を使用する](#ユーザー辞書を使用する)
        - [Nベスト解](#nベスト解)
        - [ソフトわかち書き](#ソフトわかち書き)
        - [ラティス出力](#ラティス出力)
        - [新しい素性フォーマットへの対応](#新しい素性フォーマットへの対応)
    - [謝辞](#謝辞)
    - [スター!](#スター)

<!-- /TOC -->

## リポジトリ移転について

2010年から[OSDN](https://ja.osdn.net/projects/nmecab)で開発し公開してきたNMeCabですが、バージョン0.10.0からは、こちらGitHubで開発し公開していきます。

## これは何？

NMeCabは.NETで開発した日本語形態素解析エンジンです。  
その名の通り、元々はMeCabというOSSの日本語形態素解析エンジンをC++からC#へ移植したものですが、独自の機能追加も行っています。  
辞書データはMeCabに対応したものをそのまま使用できます。  
NMeCabバージョン0.10.0では、.NET Standard 2.0ライブラリにしてあります。（.NET Core 2.0 以上、.NET Framework 4.6.1 以上 などで使用できます）

## そもそも形態素解析とは？

文章を形態素（日本語では単語と同じ）に分割し、品詞・読み・活用型などの情報も付与することです。

## NuGet

| NuGet ID / Status | 説明 |
| --- | --- |
| [LibNMeCab ![Stat](https://img.shields.io/nuget/v/LibNMeCab.svg)](https://www.nuget.org/packages/LibNMeCab) | NMeCabライブラリ単体パッケージ |
| [LibNMeCab.IpaDicBin ![Stat](https://img.shields.io/nuget/v/LibNMeCab.IpaDicBin.svg)](https://www.nuget.org/packages/LibNMeCab.IpaDicBin) | IPA辞書パッケージ |

辞書パッケージをNuGetでインストールすると、依存するNMeCabライブラリ単体パッケージも同時にインストールされます。

## 使い方

以下はC#によるサンプルコードで説明します。

### 最も簡単な使い方

`LibNMeCab.IpaDicBin` をNuGetでインストールするだけで、必要なライブラリと辞書ファイル「IPA辞書」が一括でプロジェクトに追加され、形態素解析エンジンの準備が整います。

サンプルコード:
```csharp
using System;
using NMeCab.Specialized;

class Program
{
    static void Main()
    {
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
}
```

まずNMeCab.Specialized名前空間にあるクラスを使用するため、 `using NMeCab.Specialized;` を宣言します。

次に `MeCabIpaDicTagger.Create()` により、形態素解析処理の起点となるTaggerインスタンス（MeCabTaggerBase継承クラスのインスタンス）を生成します。

このTaggerインスタンスはIDisposableを実装しているので、使用後に必ず `Dispose()` メソッドを呼び出す必要があります。
そのため、このサンプルでは `using` ステートメントを記述しています。
- 補足
  - Taggerインスタンスは生成時に辞書リソースを確保しており、`Dispose()` メソッドによりそれが解放されます。
  - .NETにある程度習熟している方であれば言うまでもありませんが、usingステートメントを記述せず、Taggerインスタンスをスコープの広い変数に保持して使い回すなどして、一度確保した辞書リソースを再利用することもできます。なお、NMeCabTaggerインスタンスはスレッドセーフです。

そのTaggerインスタンスを使い `tagger.Parse("皇帝の新しい心")` のように任意の文字列をParseメソッドに渡すだけで、形態素解析を実行できます。

結果は、形態素ノード（MeCabNodeBase継承クラスのインスタンス）の配列の形で返却されるので、ここではforeachループで処理しています。
- 補足
  - 以前のNMeCabは実行パフォーマンスを重視し、オリジナルのMeCabと同様に先頭ノードが返却され、他のノードへは連結リストをたどってアクセスする方式でした。今のNMeCabでは、Linqなどから使いやすい、配列を返却する方式に変更してあります。
  - また、今でも必要に応じて、連結リスト（Prev、Nextプロパティ）により前後のノードへアクセスできるようにしてあります。

形態素解析により得られた情報が、形態素ノードのプロパティにより取得できます。
このサンプルでは、表層形（ `Surface` ）、読み（ `Reading` ）、品詞（ `PartsOfSpeech` ）を取得しコンソールに出力しています。

他にもプロパティで取得できる情報があります。
VisualStudioのIntelliSenseなどにより閲覧できるよう、XML文書化コメントに記載してあるので確認してみてください。
- 補足
  - このサンプルではIPA辞書に特化した形態素ノードが返却されているので、IPA辞書フォーマットの素性情報が取得できます。
  - 使用する辞書により「格納されている情報＝素性文字列」のフォーマットが異なるので、以前のNMeCabはオリジナルのMeCabと同様に全ての素性文字列をまとめて取得できるだけとしていましたが、今は辞書により異なるプロパティを持つ形態素ノードを使い分ける設計としてあります。詳しくはMeCabNodeBaseクラスのソースコードや、[新しい素性フォーマットへの対応](#新しい素性フォーマットへの対応)を参照してください

サンプルコードの結果:
```
表層形：皇帝
読み　：コウテイ
品詞　：名詞

表層形：の
読み　：ノ
品詞　：助詞

表層形：新しい
読み　：アタラシイ
品詞　：形容詞

表層形：心
読み　：ココロ
品詞　：名詞
```

### 辞書を自分で用意して使用する

`LibNMeCab` だけをNuGetでインストールし、辞書は自分で用意したものを使うこともできます。

NMeCabで使う辞書は、MeCabの `mecab-dict-index` コマンドを使って 「解析用バイナリ辞書」にしたものである必要があります。
最初から解析用バイナリ辞書の状態で配布されているものを入手できれば簡単です。
バイナリ化する前の辞書しか入手できないときや、自分で辞書を作成するときは、[MeCabのサイト](https://taku910.github.io/mecab/)などを参照してください。
なお、文字コードが選べるときは「utf-8」にしておくことが無難です。

結果として以下のファイルが必要になりますので、任意のディレクトリにまとめて配置してください。これ以外のファイルは、同じディレクトリにあってもなくてもNMeCabの動作には影響しません。
- char.bin
- matrix.bin
- sys.dic
- uni.dic

以下の表の通り、NMeCabでは辞書の素性フォーマット別にTaggerクラスを準備してあります。

| 素性フォーマット | 名前空間 | Taggarクラス | デフォルトの辞書ディレクトリ名 |
| --- | --- | --- | --- |
| 汎用 | NMeCab | MeCabTagger | dic |
| IPA辞書 | NMeCab.Specialized | MeCabIpaDicTagger | IpaDic |
| UniDic ver 2.1.x | NMeCab.Specialized | MeCabUniDic21Tagger | UniDic |
| UniDic ver 2.2.x | NMeCab.Specialized | MeCabUniDic22Tagger | UniDic |

- 補足
  - 上記のデフォルトの辞書ディレクトリ名は、 `MeCabTagger.create()` のように、辞書のパスを指定しなかった場合に適用されます。パスのルートはNMeCabのDLLの配置先です。

まずは、汎用のTaggerインスタンスを生成して使うサンプルを示します。

サンプルコード：
```csharp
using System;
using NMeCab;

class Program
{
    static void Main()
    {
        var dicDir = @"C:\Program Files (x86)\MeCab\dic\ipadic"; // 辞書のパス

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
}
```

汎用のTaggerクラス「MeCabTagger」は、NMeCab名前空間にあるので、`using NMeCab;`を宣言しています。

`MeCabTagger.Create(dicDir)` により、辞書のあるディレクトリへのパスを指定して汎用のTaggerインスタンスを生成し、 `tagger.Parse(～` で汎用の形態素ノード配列の形で形態素解析結果を取得しています。
- 補足
  - ここでは、WindowsでMeCabインストーラーを使いデフォルト設定でインストールしたときにIPA辞書ファイルが置かれるパスにしてみましたが、当然、任意のパスを指定できます。
  - このパス文字列は、その環境のランタイムの仕様に従っていれば大丈夫であり、相対パスなども指定可能です。（ちなみにパスの区切り文字をスラッシュにすれば、Windows/Unix系共通で使えるためお勧めです）
  - もし、パスが不正な時や、辞書ファイルがない時・壊れている時などは、I/O系の例外がスローされます。

次にIPA辞書用のTaggarインスタンスを生成して使うサンプルを示します。

サンプルコード:
``` csharp
using System;
using NMeCab.Specialized;

class Program
{
    static void Main()
    {
        var dicDir = @"C:\Program Files (x86)\MeCab\dic\ipadic"; // 辞書のパス

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
}
```

`using NMeCab.Specialized;` で名前空間の使用を宣言し、`MeCabIpaDicTagger.Create(dicDir)` によって辞書のパスを指定しながらIPA辞書用Taggerインスタンスを生成し、 `tagger.Parse(～` でIPA辞書用の形態素ノード配列の形で形態素解析結果を取得しています。

### ユーザー辞書を使用する

システム辞書は必ず必要ですが、システム辞書に含まれていない単語を追加したいとき、ユーザー辞書を使うことも可能です。

やはりMeCabで解析用バイナリ辞書にしたファイルを、システム辞書と同じディレクトリに配置してください。
ファイル名は任意です。
複数のユーザー辞書を使用することも可能です。
文字コードはシステム辞書と同一にすることが無難です。
素性フォーマットも、基本的に合わせてください。

サンプルコード:
``` csharp
        var dicDir = @"C:\Program Files (x86)\MeCab\dic\ipadic"; // 辞書のパス
        var userDics = new[] { "usr1.dic", "usr2.dic" }; // ユーザー辞書ファイル名の一覧

        using (var tagger = MeCabIpaDicTagger.Create(dicDir, userDics)) // ユーザー辞書も指定してTaggerインスタンスを生成
        {
```

Taggerインスタンス生成メソッドの第2引数にユーザー辞書ファイル名の配列を渡すと、システム辞書と同時にそれらのユーザー辞書が読み込まれます。

### Nベスト解

ここまでの解説では、Taggerインスタンスの `Parse(string sentence)` メソッドにより、最も確からしい形態素解析結果だけを取得していました。
Taggerインスタンスの `ParseNBest(string sentence)` メソッドを使うと、確からしい順番に複数の形態素解析結果を取得できます。結果はIEnumerable<T>型です。
下のサンプルでは、LinqのTakeにより上位5件の結果を取得し表示しています。

サンプルコード:
``` csharp
using System;
using System.Linq;
using NMeCab.Specialized;

class Program
{
    static void Main()
    {
        using (var tagger = NMeCabIpaDic.CreateTagger())
        {
            var results = tagger.ParseNBest("皇帝の新しい心"); // Nベスト解を取得
            foreach (var nodes in results.Take(5)) // 上位から5件までの解を処理
            {
                foreach (var node in nodes) // 形態素ノード配列を順に処理
                {
                    Console.WriteLine($"表層形：{node.Surface}");
                    Console.WriteLine($"読み　：{node.Reading}");
                    Console.WriteLine($"品詞　：{node.PartsOfSpeech}");
                    Console.WriteLine();
                }

                Console.WriteLine("----");
            }
        }
    }
}
```

### ソフトわかち書き

Taggerインスタンスの `ParseSoftWakachi(string sentence, float theta)` メソッドでは、その文章に含まれる可能性がある形態素を洗いざらい取得できます。
また取得した形態素ノードの `Prob` プロパティにより「その形態素の含まれる確率＝周辺確率」も取得できます。

引数 `theta` は、周辺確率のなめらかさを指定する温度パラメータです。
温度パラメータを大きな値にすると、最も確からしい解の形態素の周辺確率が1または∞、その他は全て0となります。温度パラメータを0に近付けると、中間の周辺確率値も現れます。

- 補足
  - 日本語の性質上、形態素解析の正解は一意でなく曖昧なケースがあります。またもちろん形態素解析エンジンの精度の限界もあります。そのために考案された手法がソフトわかち書きです。検索エンジンのインデクシングや将来のNLPなどに応用できるはずです。
  - 下のサンプルの温度パラメータには「辞書のコストファクター800の逆数÷2」を指定していますが、どんな値が良いかを知るには試行錯誤が必要です。

下のサンプルでは、周辺確率の大きな形態素だけを抽出して表示しています。

サンプルコード：
```csharp
using System;
using System.Linq;
using NMeCab.Specialized;

class Program
{
    static void Main()
    {
        using (var tagger = NMeCabIpaDic.CreateTagger())
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
}
```

サンプルコードの結果:
```
表層形　：本部
読み　　：ホンブ
品詞　　：名詞
周辺確率：0.5966396

表層形　：本
読み　　：ホン
品詞　　：接頭詞
周辺確率：0.2812245

表層形　：部長
読み　　：ブチョウ
品詞　　：名詞
周辺確率：0.3622776

表層形　：長
読み　　：チョウ
品詞　　：名詞
周辺確率：0.5903029

上位ワード：本部,長,部長,本,部
```

### ラティス出力

Taggerインスタンスの `ParseToLattice(string sentence)` メソッドでは、「解析情報の束＝ラティス」のインスタンスを取得できます。

サンプルコード：
```csharp
using System;
using System.Linq;
using NMeCab.Specialized;

class Program
{
    static void Main()
    {
        using (var tagger = NMeCabIpaDic.CreateTagger())
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

            // ラティスから、最終的な累積コストのみを取得
            Console.WriteLine(lattice.EosNode.Cost);
        }
    }
}
```

サンプルコードの結果:
```
東京大学  名詞,固有名詞,組織,*,*,*,東京大学,トウキョウダイガク,トーキョーダイガク
--------
東京      名詞,固有名詞,地域,一般,*,*,東京,トウキョウ,トーキョー
大学      名詞,一般,*,*,*,*,大学,ダイガク,ダイガク
----
東京      名詞,固有名詞,地域,一般,*,*,東京,トウキョウ,トーキョー
大        名詞,接尾,一般,*,*,*,大,ダイ,ダイ
学        名詞,接尾,一般,*,*,*,学,ガク,ガク
----
--------
東京大学  0.763 名詞,固有名詞,組織,*,*,*,東京大学,トウキョウダイガク,トーキョーダイガク
東京大    0.007 名詞,固有名詞,組織,*,*,*,東京大,トウキョウダイ,トーキョーダイ
東京      0.230 名詞,固有名詞,地域,一般,*,*,東京,トウキョウ,トーキョー
    大学  0.005 名詞,固有名詞,地域,一般,*,*,大学,ダイガク,ダイガク
    大学  0.004 名詞,固有名詞,人名,名,*,*,大学,ダイガク,ダイガク
    大学  0.200 名詞,一般,*,*,*,*,大学,ダイガク,ダイガク
    大    0.010 名詞,接尾,一般,*,*,*,大,ダイ,ダイ
    大    0.001 接頭詞,名詞接続,*,*,*,*,大,オオ,オー
    大    0.003 接頭詞,名詞接続,*,*,*,*,大,ダイ,ダイ
    大    0.005 名詞,一般,*,*,*,*,大,ダイ,ダイ
      学  0.021 名詞,接尾,一般,*,*,*,学,ガク,ガク
      学  0.002 名詞,固有名詞,人名,名,*,*,学,マナブ,マナブ
      学  0.004 名詞,一般,*,*,*,*,学,ガク,ガク
--------
3620
```

### 新しい素性フォーマットへの対応

ポリモーフィズムを知る方ならすぐに想像される通りだと思いますが、 `MeCabNodeBase` 、 `MeCabTaggerBase` を継承させた独自のクラスをコーディングすることになります。
ただし、キャスト処理を無くしたり、実行パフォーマンスを向上させる目的から、少し変わって見えるかもしれないコードが必要となりますので、以下を参考にしてください。

`MeCabNodeBase` を継承した形態素ノードクラスに、素性フォーマットに合わせた独自のプロパティをコーディングしてください。
それぞれのプロパティ内で、素性情報の任意の列の値を `GetFeatureAt(1)` のようにインデックス番号を指定して取得します。

```csharp
using NMeCab;

namespace MyApp
{
    /// <summary>
    /// MyDicの形態素ノードです。
    /// </summary>
    public class MyDicNode : MeCabNodeBase<MyDicNode>
    {
        /// <summary>
        /// 素性情報1
        /// </summary>
        public string Feature1
        {
            get { return this.GetFeatureAt(0); }
        }

        /// <summary>
        /// 素性情報2
        /// </summary>
        public string Feature2
        {
            get { return this.GetFeatureAt(1); }
        }
    }
}
```

`MeCabTaggerBase` を継承したTaggerクラスは、定型的なコードとなります。
下記をコピーして「MyApp」「MyDic」の部分を書き換えるだけでOKです。

```csharp
using NMeCab;

namespace MyApp
{
    /// <summary>
    /// MyDicを使用する場合の形態素解析処理の起点を表します。
    /// </summary>
    public class MyDicTagger : MeCabTaggerBase<MyDicNode>
    {
        /// <summary>
        /// コンストラクタ（非公開）
        /// </summary>
        private MeCabTagger()
        { }

        /// <summary>
        /// 形態素解析処理の起点を作成します。
        /// </summary>
        /// <param name="dicDir">使用する辞書のディレクトリへのパス</param>
        /// <param name="userDics">使用するユーザー辞書のファイル名のコレクション</param>
        /// <returns>形態素解析処理の起点</returns>
        public static MyDicTagger Create(string dicDir = null,
                                         string[] userDics = null)
        {
            return Create(dicDir,
                          userDics,
                          () => new MyDicTagger(), // Tagger生成関数
                          () => new MyDicNode(), // 形態素ノード生成関数
                          "MyDic"); // デフォルトの辞書ディレクトリ名
        }
    }
}
```

上の2クラスができたら、作成したTaggerクラスのCreateメソッドを呼び出して使用できます。

```csharp
using System;
using NMeCab;

class Program
{
    static void Main()
    {
        var dicDir = @"C:\Program Files (x86)\MeCab\dic\mydic"; // 辞書のパス

        using (var tagger = MyDicTagger.Create(dicDir)) // MyDic形式用のTaggerインスタンスを生成
        {
            var nodes = tagger.Parse("皇帝の新しい心"); // 形態素解析を実行
            foreach (var node in nodes) // 形態素ノード配列を順に処理
            {
                Console.WriteLine($"素性情報1：{node.Feature1}");
                Console.WriteLine($"素性情報2：{node.Feature2}");
            }
        }
    }
}
```

## 謝辞

Kouji Matsui ( ＠kekyo ) 氏の素晴らしい情報とコードの公開に感謝いたします。
LibNMeCab.IpaDicBin に使用している辞書ファイルをNuGet/MSBuildで扱うコードは、 ＠kekyo 氏のオープンソースをほぼそのまま使用させて頂いたものです。

## スター!

スターを頂ければ励みになりますので、よろしくお願いいたします。

実は、2020年5月中旬になにげなくリポジトリをプライベート設定にしたのですが、そのために、それまで頂いていたいたスターが全て消えてしまいました。申し訳ありませんでした。もう一度スターを頂けたらありがたいです。
