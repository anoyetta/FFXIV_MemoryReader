FFXIV_MemoryReader
===

FFXIV_MemoryReader は Final Fantasy XIV (FFXIV) 向けの Advanced Combat Tracker (ACT) 用プラグインです。

# 概要

**FFXIV_MemoryReader は単体で使用するプラグインではありません。**  
**他のFFXIV用ACTプラグインを補助するためのプラグインです**

FFXIV_MemoryReader は Final Fantasy XIV のメモリをスキャンし、  
そのスキャン結果を他のACTプラグインに提供するプラグインです。


# For Developers

## Requirements

- `Visual Studio` (with `NuGet Package Manager`)
- `Advanced Combat Tracker.exe`
- `libz.exe`

## プロジェクト一覧の説明

- **FFXIV_MemoryReader.Base**
 : ACTのプラグインとして認識させるための基本となる部分です。
- **FFXIV_MemoryReader.Core**
 : Final Fantasy XIV のメモリスキャンを実施する部分です。
- **FFXIV_MemoryReader.Model**
 : プラグインで使用される型を定義しています。


## ソリューションを開いたらまずやること

- NuGet パッケージの修復 (NLog を NuGet から取得しています)
- `Advanced Combat Tracker.exe` を `Thirdparty` ディレクトリに置く。
- `libz.exe` を `Thirdparty` ディレクトリに置く。


## ビルド

Release でビルドをすると、ビルド後の実行処理により、 `Distributions` ディレクトリに  
プラグイン本体と SDK の DLL が作成されます。

```
Distributions/  
   ├ FFXIV_MemoryReader.dll
   └ SDK/
      ├ FFXIV_MemoryReader.Base.dll
      └ FFXIV_MemoryReader.Model.dll
```

なお、プラグインの本体 `FFXIV_MemoryReader.dll` は以下の4つの DLL を `libz` で結合した物です。
- FFXIV_MemoryReader.Base.dll
- FFXIV_MemoryReader.Core.dll
- FFXIV_MemoryReader.Model.dll
- NLog.dll

## あなたのプラグインで実施すること

サンプルのプロジェクト `FFXIV_MemoryReader_SampleClient` を参考にして下さい。

具体的には、まず、

- `FFXIV_MemoryReader.Base.dll` を参照に追加
- `FFXIV_MemoryReader.Core.dll` を参照に追加

あとは、コードを書くだけです。

以下は周辺のキャラクター情報を取得する例です。

```C#
    IActPluginV1 plugin = null;
    foreach (var p in ActGlobals.oFormActMain.ActPlugins)
    {
        if (p.pluginFile.Name == "FFXIV_MemoryReader.dll")
        {
            plugin = p.pluginObj;
            break;
        }
    }

    if (plugin != null)
    {
        var memoryPlugin = plugin as MemoryPlugin;
        List<Combatant> combatants = memoryPlugin.GetCombatants();
        if (combatants != null)
        {
            // 任意の処理
            // combatants には周辺のキャラクター情報が入っています。
        }
    }
```

## [Optional] 作成したプラグインをまとめる

あなたがこのように作成した ACT プラグイン DLL は、少なくとも2つの外部ライブラリ  
`FFXIV_MemoryReader.Base.dll`, `FFXIV_MemoryReader.Model.dll` を必要とします。  
(他にパッケージを使った場合はそれも必要となります。)

ビルド結果の `Release`, `Debug` フォルダには、あなたが作った DLL の他に、  
必要となる DLL がコピーされているはずです。

ですが、ACT でこのプラグインを読み込んだ際、これらの外部 DLL は読み込まれず、  
実際に使う場合にエラーとなるでしょう。

これらの回避方法は3つあります。

1. ACT 本体 `Advanced Combat Tracker.exe` と同じディレクトリに、全てのDLLを配置する。
2. 必要な DLL をプラグイン本体の DLL でアセンブリとしてロードする機能を付け加える
3. libz.exe 等を使って1つの DLL にまとめる


`1` が最も簡単ですが、多数のプラグインがある場合、このフォルダは壊滅的な状態になるでしょう。  
また、他のプラグインを導入した際に、誤って必要なファイルが書き換えられる可能性があります。

`2` は他の多くの FFXIV 用 ACT プラグインで採用されている方式です。  
プラグインがロードされる際に、必要な DLL を独自に読み込む機能を付けます。  
参考: [anoyetta/ACT.SpecialSpellTimer](https://github.com/anoyetta/ACT.SpecialSpellTimer/blob/master/ACT.SpecialSpellTimer/AssemblyResolver.cs)
/ 
[hibiyasleep/OverlayPlugin](https://github.com/hibiyasleep/OverlayPlugin/blob/master/OverlayPlugin/AssemblyResolver.cs)


`3` は本プロジェクトで採用している方式です。必要な DLL を `libz` を使って1つにまとめてしまうことで解決しています。  
どのように実施しているかは、Visual Studio で サンプルプロジェクトのプロパティを開き、  
「ビルドイベント」->「ビルド後イベントのコマンドライン」を参考にして下さい。

