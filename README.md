# 形態素解析エンジンNMeCab
![NMeCab](icon/NMeCab-icon-100.png)

# リポジトリ移転について  
2010年から[OSDN](https://ja.osdn.net/projects/nmecab)のリポジトリで開発し公開してきたNMeCabですが、バージョン0.10.0からは、こちらGitHubで開発し公開していきます。  
しばらく開発を停滞させてしまっていましたが、再開したいと思いますので、応援いただけると幸いです。

# これは何？

NMeCabは.NETで開発した日本語形態素解析エンジンです。.NET環境で簡単に使用できるライブラリとし、NuGetでのインストールも可能にしてあります。  
その名の通り、元はMeCabというC++で開発された形態素解析エンジンを参考にしており、同じ解析手法を組み込んであります。  
形態素解析には膨大な日本語情報を学習させた辞書データが必要になりますが、NMeCabではMeCabに対応した辞書をそのまま使用できます。

# NuGet

| NuGet ID | ステータス | 説明 |
| --- | --- | --- |
| [LibNMeCab](https://www.nuget.org/packages/LibNMeCab) | [![NuGet LibNMeCab](https://img.shields.io/nuget/v/LibNMeCab.svg)](https://www.nuget.org/packages/LibNMeCab) | NMeCabライブラリ単体パッケージ |
| [LibNMeCab.IpaDicBin](https://www.nuget.org/packages/LibNMeCab.IpaDicBin) | [![NuGet LibNMeCab.IpaDicBin](https://img.shields.io/nuget/v/LibNMeCab.IpaDicBin.svg)](https://www.nuget.org/packages/LibNMeCab.IpaDicBin) | IPA辞書パッケージ |

辞書パッケージをNuGetパッケージマネージャーでインストールすると、依存するNMeCabライブラリ単体パッケージも同時にインストールされます。

# 使い方

## 辞書を意識しないで使用する

LibNMeCab.IpaDicBinをNuGetでインストールするだけで、すぐに形態素解析を始めることができます。

サンプルコード
```csharp
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
```

まず形態素解析処理の起点となるTaggerインスタンスを `NMeCabIpaDic.CreateTagger()` により生成します。
このTaggerインスタンスは使用後に必ず `Dispose()` メソッドを呼び出す必要があるため、ここでは `using` ステートメントを使用しています。

そのTaggerインスタンスの `Parse(string)` メソッドへ任意の文字列を渡すと形態素解析が実施され、形態素に分割された結果が配列として返却されます。
（以前のNMeCabは先頭ノードが返却され、連結リストでアクセスする方式でしたが、Linqからより使いやすいよう変更してあります。必要に応じて連結リストも使用できます。）

配列の中身は形態素ノードであり、形態素解析により得られた情報がプロパティに格納されているので、自由に取得することができます。
（使用する辞書により得られる情報のフォーマットが異なるので、以前のNMeCabは辞書に格納されたcsv文字列のままでしたが、IPA辞書ならば細かく分割した情報を取得できるよう変更してあります。）

サンプルコードの結果
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

## 辞書を意識して使用する

To be continued