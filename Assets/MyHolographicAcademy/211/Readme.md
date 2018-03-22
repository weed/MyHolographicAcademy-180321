# My Holographic Academy 211（日本語）



Unity や MRTK のバージョンに依存せずに HoloAcademy 211 を学習できるようにしたものです。
# 準備
## プロジェクトの準備
1. Unityでプロジェクトを新規作成
1. MRTKを導入
1. メニュー＞Mixed Reality Toolkit＞Configure＞Apply Mixed Reality Project Setting（チェックボックスはデフォルト）
1. Assets / MyHolographicAcademy / 211 フォルダを新規作成
## シーンの準備
1. 211 フォルダで Gesture シーンを新規作成
1. メニュー＞Mixed Reality Toolkit＞Configure＞Apply Mixed Reality Scene Setting（チェックボックスはデフォルト）
1. Directional Lightを削除
1. Asset > HoloToolkit > Input > Prefabs > Cursor > Cursorプレハブをシーンに追加
## デバッグ用3D Textを作成
1. Asset > HoloToolkit > UX > Prefabs > 3DTextPrefab をヒエラルキービューにドラッグ
1. 新しい GameObject を右クリックして、名前を「Debug Log」に変更します。
1. Debug Log の Transform の Position を (0.1, 0, 2) にします。
1. Debug Log の Text Mesh コンポーネントの Anchor を Middle left にし、Alignment を Left にします。
1. DebugLog スクリプトがなければ新規作成
### DebugLog スクリプト
```csharp
using HoloToolkit.Unity.InputModule;
using UnityEngine;
using HoloToolkit.Unity;

namespace EDUCATION.FEELPHYSICS.MY_HOLOGRAPHIC_ACADEMY
{
    /// <summary>
    /// 書いたスクリプトが正常に動作しているか、3D Textに表示して確認する
    /// </summary>
    public class DebugLog : Singleton<DebugLog>
    {
        #region Public Valiables

        [Tooltip("3D Text の Text Mesh")]
        public TextMesh MyTextMesh;

        /// <summary>
        /// 表示するメッセージを受け取るための public 変数
        /// </summary>
        public string Log = "";

        #endregion

        #region MonoBehaviour CallBacks

        /// <summary>
        /// スクリプトの public 変数を表示し続ける
        /// </summary>
        private void Update()
        {
            this.MyTextMesh.text = ""
                // 以下の2行がフォーカスが、外れたときにうまくはたらかない
                + "Position: " + GazeManager.Instance.HitPosition.ToString()
                + "\nNormal: " + GazeManager.Instance.HitNormal.ToString()
                + "\nFocused: " + this.FocusedGameObjectName()
                + "\nLog:\n" + this.Log
                ;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// フォーカスされている GameObject の名前を取得する
        /// </summary>
        /// <returns>フォーカスされている GameObject の名前</returns>
        private string FocusedGameObjectName()
        {
            string focusedName;

            if (GazeManager.Instance.HitObject == null)
            {
                focusedName = "null";
            }
            else
            {
                focusedName = GazeManager.Instance.HitInfo.collider.gameObject.name;
            }

            return focusedName;
        }

        #endregion
    }
}
```

# Gesture
## エアタップ
### ねらい
オブジェクトを gaze した状態でエアタップし、オブジェクトの色を変えます
### 準備
1. Cube を作り、Test Cube と名前を付けます
1. Transform を Position(0, 0, 2), Scale(0.1, 0.1, 0.1) にする
### MyGazeGestureManager スクリプト
1. Assets / MyHolographicAcademy / 211 / Scripts フォルダを新規作成

```csharp
using HoloToolkit.Unity.InputModule;
using UnityEngine;
using UnityEngine.XR.WSA.Input;

/// <summary>
/// ジェスチャーのエアタップを監視して、エアタップされたら、
/// そのとき gaze でフォーカスしていたオブジェクトに OnSelect メッセージを送る
/// </summary>
public class MyGazeGestureManager : MonoBehaviour
{
    #region Public Valiables

    /// <summary>
    /// シングルトン化するための変数
    /// </summary>
    public static MyGazeGestureManager Instance;

    /// <summary>
    /// gaze されているホログラムを格納する
    /// </summary>
    public GameObject FocusedObject;

    #endregion

    #region Public Valuables

    /// <summary>
    /// GestureRecognizer を保持するための変数
    /// </summary>
    private GestureRecognizer recognizer;

    #endregion

    #region MonoBehaviour CallBacks

    /// <summary>
    /// エアタップを監視するイベントを登録する
    /// </summary>
    private void Awake()
    {
        Instance = this;

        // Select ジェスチャーを感知するために GestureRecognizer を準備する
        this.recognizer = new GestureRecognizer();
        this.recognizer.TappedEvent += this.Recognizer_TappedEvent;
        this.recognizer.StartCapturingGestures();
    }

    /// <summary>
    /// エアタップされたらフォーカスされているオブジェクトに OnSelect メッセージを送る
    /// </summary>
    /// <param name="source">source</param>
    /// <param name="tapCount">tapCount</param>
    /// <param name="headRay">headRay</param>
    private void Recognizer_TappedEvent(InteractionSourceKind source, int tapCount, Ray headRay)
    {
        // フォーカスされたオブジェクトとその親に OnSelect メッセージを送る
        if (this.FocusedObject != null)
        {
            this.FocusedObject.SendMessageUpwards("OnSelect");
        }
    }

    /// <summary>
    /// gaze してフォーカスしているオブジェクトを取得し続ける
    /// </summary>
    private void Update()
    {
        // どのホログラムがこのフレームでフォーカスされているか明らかにする
        GameObject oldFocusedObject = this.FocusedObject;

        this.FocusedObject = GazeManager.Instance.HitObject;

        // もしこのフレームでフォーカスされているオブジェクトが変わった場合は、
        // 新しいジェスチャーを探し始める
        if (this.FocusedObject != oldFocusedObject)
        {
            this.recognizer.CancelGestures();
            this.recognizer.StartCapturingGestures();
        }
    }

    #endregion
}
```
1. ヒエラルキービューで EmptyObject として manager オブジェクトを作ります
1. 上記のスクリプトを Managers オブジェクトにアタッチ
