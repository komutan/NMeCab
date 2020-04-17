# <img src="icon/NMeCab-icon-100.png" width="30"> 形態素解析エンジンNMeCab

## リポジトリ移転について

2010年から[OSDN](https://ja.osdn.net/projects/nmecab)で開発し公開してきたNMeCabですが、バージョン0.10.0からは、こちらGitHubで開発し公開していきます。これからも沢山利用して貰えると開発のモチベーションが上がりますので、よろしくお願いいたします。

## これは何？

NMeCabは.NETで開発した日本語形態素解析エンジンです。.NET環境で簡単に使用できるライブラリとし、NuGetでのインストールも可能にしてあります。  
その名の通り、元はMeCabというC++で開発された形態素解析エンジンを参考にしており、同じ解析手法を組み込んであります。  
形態素解析には膨大な日本語情報を学習させた辞書データが必要になりますが、NMeCabではMeCabに対応した辞書をそのまま使用できます。

## NuGet

| NuGet ID | ステータス | 説明 |
| --- | --- | --- |
| [LibNMeCab](https://www.nuget.org/packages/LibNMeCab) | [![NuGet LibNMeCab](https://img.shields.io/nuget/v/LibNMeCab.svg)](https://www.nuget.org/packages/LibNMeCab) | NMeCabライブラリ単体パッケージ |
| [LibNMeCab.IpaDicBin](https://www.nuget.org/packages/LibNMeCab.IpaDicBin) | [![NuGet LibNMeCab.IpaDicBin](https://img.shields.io/nuget/v/LibNMeCab.IpaDicBin.svg)](https://www.nuget.org/packages/LibNMeCab.IpaDicBin) | IPA辞書パッケージ |

辞書パッケージをNuGetパッケージマネージャーでインストールすると、依存するNMeCabライブラリ単体パッケージも同時にインストールされます。

## 簡単な使い方

### 辞書を意識しないで使用する

`LibNMeCab.IpaDicBin` をNuGetでインストールするだけで、すぐに形態素解析を始めることができます。

サンプルコード:
```csharp
using System;
using NMeCab;

namespace SampleCode
{
    class Program
    {
        static void Main()
        {
            using (var tagger = NMeCabIpaDic.CreateTagger()) // Taggerインスタンスを生成
            {
                var nodes = tagger.Parse("皇帝の新しい心"); // 形態素解析を実行
                foreach (var node in nodes) // 形態素ノード配列を順に処理
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
```

まず `NMeCabIpaDic.CreateTagger()` により、形態素解析処理の起点となるTaggerインスタンスを生成します。このインスタンスは使用後に必ず `Dispose()` メソッドを呼び出して、リソース開放を行う必要があるので、このサンプルでは `using` ステートメントを記述しています。
>- Taggerインスタンスは生成時に読み込んだ辞書リソースを保持しているため、 `Dispose()` を忘れるとそれが解放されないことに注意してください。
>- .NETに習熟している方であれば「一度読み込んだ辞書リソースを使い続けたいので、すぐには `Dispose()` したくない」という場合に、`using` ステートメント以外の方法を使い、Taggerインスタンスを使いまわすこともできます。

そのTaggerインスタンスの `Parse(string sentence)` メソッドへ任意の文字列を渡すと形態素解析が行われ、形態素に分割された結果が配列として返却されます。
>- 以前のNMeCabは、先頭ノードが返却され連結リストを辿って他のノードへアクセスする方式でしたが、Linqなどからより使いやすいよう、デフォルトを配列に変更してあります。
>- また、今でも必要に応じて、連結リスト（Prev、Nextプロパティ）により前後のノードへアクセスできるようにしてあります。

配列の中身は形態素ノード（MeCabNodeBase継承クラス）のインスタンスであり、形態素解析により得られた情報がプロパティに格納されています。このサンプルでは、表層系（ `Surface` ）、読み（ `Reading` ）、品詞（ `PartsOfSpeech` ）だけを取得しコンソールに出力しています。

他にもプロパティで取得できる情報があります。XML文書化コメントに記載してあるので、VisualStudioのIntelliSenseなどにより確認してみてください。
>- 使用する辞書により得られる情報（素性文字列）のフォーマットが異なるので、以前のNMeCabはオリジナルのMeCabと同様にcsv形式の素性文字列をまとめて取得するだけとしていましたが、今は個別の素性情報のプロパティを持たせてあります。
>- 辞書により異なるプロパティを持つ形態素ノードを使い分ける設計としたので、詳しくはMeCabNodeBaseクラスなどのソースコードを参照してください。別途解説も記載したいと思います。

サンプルコードの結果:
```
表層系：皇帝
読み　：コウテイ
品詞　：名詞

表層系：の
読み　：ノ
品詞　：助詞

表層系：新しい
読み　：アタラシイ
品詞　：形容詞

表層系：心
読み　：ココロ
品詞　：名詞
```

### 辞書を意識して使用する

`LibNMeCab` だけをNuGetでインストールし、辞書は自分で用意したものを使うこともできます。

Webで配布されているMeCab用の辞書を入手するほか、自らコーパスを準備して学習させて構築することも可能ですが、後者には膨大な作業が必要なため、前者のケースが多いと思います。

NMeCabで使う辞書は、MeCabの `mecab-dict-index` コマンドを使って 「解析用バイナリ辞書」 にしておく必要があります。MeCabのインストール方法と使用方法については、[MeCabのサイト](https://taku910.github.io/mecab/)などを参照してください。なお、解析用バイナリ辞書の文字コードが選べるときは、「utf-8」にしておくことが無難です。

結果として以下のファイルが必要になりますので、任意のディレクトリにまとめて配置してください。
- char.bin
- matrix.bin
- sys.dic
- uni.dic

上記以外のファイルは、同じディレクトリにあってもなくても、NMeCabの動作に影響しません。

サンプルコード：
```csharp
using System;
using NMeCab;

namespace SampleCode
{
    class Program
    {
        static void Main()
        {
            var dicDir = @"C:\Program Files (x86)\MeCab\dic\ipadic";

            using (var tagger = MeCabTagger.Create(dicDir)) // Taggerインスタンスを生成
            {
                var nodes = tagger.Parse("皇帝の新しい心"); // 形態素解析を実行
                foreach (var node in nodes) // 形態素ノード配列を順に処理
                {
                    Console.WriteLine("表層系：" + node.Surface);
                    Console.WriteLine("素性　：" + node.Feature); // csv
                    Console.WriteLine();
                }
            }
        }
    }
}
```

このサンプルでは `MeCabTagger.Create(string dicDir, string[] userDics = null)` により、辞書ディレクトリパスを指定してTaggerインスタンスを生成しています。
>- ここでは、WindowsでMeCabインストーラーを使いデフォルト設定でインストールしたときの辞書ディレクトリパスにしてみましたが、当然、任意のパスを指定できます。
>- このパスは、NMeCab内部で.NET標準クラスライブラリにそのまま渡しているので、.NETの仕様に従っていれば相対パスなども指定可能です。
>- もしも、パスが不正な時や、辞書ファイルが壊れている時などは、IO関係の例外がスローされます。

サンプルコード:
``` csharp
using System;
using NMeCab;

namespace SampleCode
{
    class Program
    {
        static void Main()
        {
            var dicDir = @"C:\Program Files (x86)\MeCab\dic\ipadic";

            using (var tagger = MeCabIpaDicTagger.Create(dicDir)) // Taggerインスタンスを生成
            {
                var nodes = tagger.Parse("皇帝の新しい心"); // 形態素解析を実行
                foreach (var node in nodes) // 形態素ノード配列を順に処理
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
```

使用する辞書の素性フォーマットがIPA辞書と同じであれば、このサンプルのように、`MeCabIpaDicTagger.Create(string dicDir, string[] userDics = null)` により、IPA辞書用のTaggarインスタンスを生成し、形態素解析結果として、個別の素性情報のプロパティを持った形態素ノードを取得することもできます。

## ユーザー辞書を使用する場合

サンプルコード:
``` csharp
            var dicDir = @"C:\Program Files (x86)\MeCab\dic\ipadic";
            var userDic = new[] {"usr1.dic", "usr2.dic"}; // ユーザー辞書

            using (var tagger = MeCabIpaDicTagger.Create(dicDir, userDic)) // Taggerインスタンスを生成
            {
```

やはり解析用バイナリ辞書にした後、同じ辞書ディレクトリに配置して、Taggerインスタンス生成メソッドの第2引数にそのファイル名の配列を渡してください。

## Nベスト解
coming soon.
## ソフトわかち書き
coming soon.
## ラティス出力
coming soon.
## 新しい素性フォーマットへの対応
coming soon.
