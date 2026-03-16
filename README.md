# KszUtil

Personal utility library for Unity projects.

## セットアップ

### 1. manifest.json に以下を追加

KszUtil本体:
```json
"com.ksz.util": "https://github.com/divide-by-zero/KszUtil.git"
```

### 2. 追加で必要な依存パッケージ

以下のパッケージも `manifest.json` の `dependencies` に追加してください。

```json
"com.cysharp.unitask": "https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask",
"com.neuecc.unirx": "https://github.com/neuecc/UniRx.git?path=Assets/Plugins/UniRx/Scripts",
"jp.hadashikick.vcontainer": "https://github.com/hadashiA/VContainer.git?path=VContainer/Assets/VContainer#1.15.4",
```

### 3. DOTween

Asset Store から DOTween (HOTween v2) をインポートしてください。

- https://assetstore.unity.com/packages/tools/animation/dotween-hotween-v2-27676

### 4. Scripting Define Symbols

以下のシンボルを Project Settings > Player > Scripting Define Symbols に追加してください。

- `UNITASK_DOTWEEN_SUPPORT`
