# UnityEditorMemo
![](https://img.shields.io/badge/version-3.1.3-brightgreen)  
UnityEditor上でメモを取ることが出来ます。  
開発中に疑問に思ったことや、次エディタを開いたときに何から始めるかを記述するなど、様々な用途で使うことができます。  
  
[AssetStore](https://assetstore.unity.com/packages/tools/utilities/unityeditormemo-81812)  
[Package](https://github.com/charcolle/UnityEditorMemo/releases)

![main](desc/unityeditormemo_ver3_main.png)

# 対応バージョン
Unity 2018 or newer

# PackageManagerでダウンロード
以下の一文をmanifest.jsonに追加してください。  
*NOTE: セーブデータシステムがPackageManager向けに最適化されていないと思います。ご利用にはご注意ください。*
``` 
"com.charcolle.unityeditormemo": "https://github.com/charcolle/UnityEditorMemoPackageManager.git" 
```

# 使い方
[Youtube](https://www.youtube.com/watch?v=yL0bPQLsaRA)   

![main](desc/unityeditormemo_ver3_main_overview.png)

* [UnityEditorMemoを開く](##UnityEditorMemoを開く)
* [メモ](##メモ)
* [カテゴリー](##カテゴリー)
* [Preference](##Preference)
* [シーンメモ](##シーンメモ)
* [Slack連携](##Slack連携)

## UnityEditorMemoを開く
[Window]->[UnityEditorMemo]を選択します。

![main](desc/unityeditormemo_ver3_open.png)

## メモ
ポストしたメモの一覧が表示されます。  
下部のカラートグルは**Label**といい、色を選択するとそのLabelのみを表示することができます。
  
    
### メモの作成
カテゴリーを選択し、ポストビューにテキストを入力して[**Post**]ボタンを押します。  
日付の隣のメニューからLabelが変更できます。Postボタンの横の絵文字を選択するとメモに絵文字が付属されます。  
**URL**のTextFieldにURLを記入すると、メモを右クリックした際にURLを開くことができます。

![main](desc/unityeditormemo_ver3_post_view.png)

### メモの編集
右クリックメニューの[**Edit**]を選択するか、メモを**ダブルクリック**することで編集モードに切り替わります。  
編集モードでは、日付以外のすべてを変更することが可能です。Labelに関して、右クリックメニューの[**Label**]から変更可能です。  
右クリックメニューの[**Done**]を選択するか、メモを**ダブルクリック**することで編集モードを終了します。  

![main](desc/unityeditormemo_ver3_open.png)

### メモの削除
右クリックメニューの[**Delete**]を選択するとメモを削除します。

![main](desc/unityeditormemo_ver3_memo_delete.png)

### Assetを添付
メモに対してProject内のAssetを添付することが可能です。メモからAssetへ素早くアクセス出来て作業効率が上がります。  
編集モードでオブジェクトの添付を解除することができます。

![main](desc/unityeditormemo_ver3_objectref.png)

### メモのバックアップ
#### データ出力
メモデータの移行のため、メモのデータをすべてエクスポートします。**.unitymemo**ファイルが作成され、これはインポート用に使われます。
#### インポート
出力されたデータを現在のプロジェクトに反映します。
* Override...現在のメモのデータを消して上書きします。
* Additive...現在あるメモはそのままに、追加的にインポートします。

*Note: defaultカテゴリーはAdditiveではインポートされません*

## カテゴリー

### カテゴリーの作成
[Menu]から[**Create New Category**]を選択します。カテゴリービューにnew Categoryが出現するので、名前を編集します。

![main](desc/unityeditormemo_ver3_category_create.png)

### カテゴリーの編集
対象のカテゴリーを長く選択するか、右クリックメニューの[**Rename**]で名前を変更します。

![main](desc/unityeditormemo_ver3_category_create_2.png)

### カテゴリーの削除
対象のカテゴリーを右クリックメニューの[**Delete**]を選択することで削除します。

![main](desc/unityeditormemo_ver3_category_delete.png)

### カテゴリーの並べ替え
カテゴリーをドラッグすると任意の位置へ移動します。

![main](desc/unityeditormemo_ver3_category_sort.png)

### カテゴリー表示切替
メニューの隣の[≡]トグルからカテゴリービューの表示・非表示を切り変えます。  

![main](desc/unityeditormemo_ver3_category_hide.png)

*Note: defaultカテゴリーは名前の変更・削除はできない仕様です*


## Preference
UnityEditorのPreferenceを選択するか、Menuから[**Open Preference**]を選択することでUnityEditorMemoのPreferenceを開きます。Preferenceでは以下の設定ができます。
* Memoのフォントサイズ
* Label名の設定
* UnitySceneMemoの有効化・無効化、各種設定
* Slackへの投稿機能

![main](desc/unityeditormemo_ver3_preference_main.png)

## シーンメモ
シーンのScene上でGameObjectにメモを貼り付けます。  
*Note: メモを付けるシーンはProject内に保存されている必要があります*

### シーンメモ機能を利用する
Preferenceで[**Enable UnitySceneMemo**]にチェックを入れます。  

![main](desc/unityeditormemo_scene_enable.png)

*NOTE: チェックを入れた後、一度UnityEditorを再生したりスクリプトをコンパイルしたあとに有効化されます*

### GameObjectにシーンメモを作成する
対象のGameObjectを選択し **[+]ボタン** を押してシーンメモを作成します。  
吹き出しマークが出現し、それを押すとメモの内容を表示します。

![main](desc/unityeditormemo_ver3_scenememo_create.png)


### シーンメモを編集する
**[≡]ボタン** を押すか、右クリックメニューの[**Edit**]を選択すると編集モードに切り替わります。  
ドロップダウンメニューでメモの色を変更します。  
[**ShowAtScene**]を有効化すると、そのGameObjectを選択したときにSceneView上でメモを表示します。  
[ShowAtScene]を有効化して下に出てきた設定から、SceneView上での大きさや文字色を調整します。 

![main](desc/unityeditormemo_ver3_scenememo.png)

### シーンメモを削除する
メモを表示し、右クリックメニューの[**Delete**]で削除することができます。

### メモにシーンメモを添付する
シーンメモを持つGameObjectであれば、メモへドラッグすることでシーンメモを保持し閲覧出来ます。

![main](desc/unityeditormemo_ver3_scenememo_2.png)


## Slack連携
メモをポストする際、Slackにも送信することができます。  
*NOTE: 送信のみの機能なので、Slackに投稿されたものは編集したり削除したりすることはできません*

### Slack連携機能を有効化する
Preferenceで[**Use Slack Integration**]にチェックを入れます。  
Slackに投稿するために必要な情報を入力します。

![main](desc/unityeditormemo_ver3_slack_1.png)

有効化するとポストビューが更新されます。

#### 投稿する
この機能を有効化したらすべてのメモがSlackに通知されるわけではありません。  
**[Post to Slack]**トグルをオンしているときのみSlackに投稿されるので、投稿したいときだけこのトグルをオンにすると良いでしょう。

![main](desc/unityeditormemo_ver3_slack_2.png)

