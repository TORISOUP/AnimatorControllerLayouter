# AnimatorController Layouter

`AnimatorController`のステートマシンの配置を調整してくれるエディタ拡張です。

# 使い方

UnityPackageを導入するとメニューが追加されます。
`[Custom utils] -> [Layout AnimatorController]` でウィンドウが開きます。

## 設定項目

|要素|意味|
|:--:|:--:|
|Controller|対象のAnimatorController|
|Target Layer|AnimatorControllerのLayer|
|計算回数|配置のシミュレーションを実行する回数。大きくしすぎるとフリーズします。|
|ばね係数|矢印がつながっているノード同士の引き合う力|
|ばねの自然長|矢印がつながっているノード同士の配置間隔|
|斥力|ノード同士が離れ合う力|
|斥力の効果範囲|ノード同士がこれより近い場合は反発しあう|

* すべてのノードは均等に反発しあう
* つながったノード同士は引き合う
* デフォルトノードは固定される

というルールに基づいてノードの再配置が実行されます。

いきなりキレイに配置されないので、手動で配置を調整しながら実行するとよいでしょう。


## LICENSE

MIT License.
