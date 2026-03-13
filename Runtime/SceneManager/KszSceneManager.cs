using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace KszUtil.SceneManager
{
    public class KszSceneManager : SingletonMonoBehaviour<KszSceneManager>
    {
        /// <summary>フェード中の透明度</summary>
        private float fadeAlpha = 0;

        /// <summary>フェード中かどうか</summary>
        private bool isFading;

        /// <summary>
        /// フェードスピード
        /// </summary>
        private float fadeSpeed = 0.5f;

        //データ流し込み中かどうか
        public bool IsInjection { get; private set; }

        private Color targetColor = Color.black;

        public UniTask LoadAsync(int sceneIndex)
        {
            return _LoadAsync<SceneBase>(UnityEngine.SceneManagement.SceneManager.GetSceneAt(sceneIndex).name);
        }

        /// <summary>
        ///   <para>Loads the Scene by its name or index in Build Settings.</para>
        /// </summary>
        /// <param name="sceneName">Name or path of the Scene to load.</param>
        public UniTask LoadAsync(string sceneName)
        {
            return _LoadAsync<SceneBase>(sceneName);
        }

        public UniTask LoadAsync<T>(string sceneName, Func<object> viewModelFunc = null, T mySelf = null) where T : MonoBehaviour, ISceneBase => _LoadAsync(sceneName, null, viewModelFunc, mySelf);
        public UniTask LoadAsync<T>(string sceneName, Action<T> paramSetAction = null, T mySelf = null) where T : MonoBehaviour, ISceneBase => _LoadAsync(sceneName, paramSetAction, null, mySelf);

        private async UniTask _LoadAsync<T>(string sceneName, Action<T> paramSetAction = null, Func<object> viewModelFunc = null, T mySelf = null) where T : MonoBehaviour, ISceneBase
        {
            IsInjection = true;
            //だんだん暗く
            isFading = true;
            var raw = SetFadeObject(targetColor, 0);
            await raw.DOFade(1.0f, fadeSpeed);

            //本当はロード画面なんかを挟みたいような
            if (viewModelFunc != null || paramSetAction != null)
            {
                await UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                var findSceneObject = FindObjectsOfType<T>().FirstOrDefault(t => t != mySelf);
                if (findSceneObject == null)
                {
                    throw new Exception($"Scene:{sceneName}に {typeof(T).Name}が見つかりません。");
                }

                if (viewModelFunc != null) findSceneObject.Vm = viewModelFunc.Invoke();
                paramSetAction?.Invoke(findSceneObject);

                await findSceneObject._InitializeAsync();

                await UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
            }
            else
            {
                await UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
            }

            //流し込み完了
            IsInjection = false;

            raw = SetFadeObject(targetColor, 1);
            await raw.DOFade(0.0f, fadeSpeed / 2);

            isFading = false;
            Destroy(raw.gameObject);
        }

        private static RawImage SetFadeObject(Color color, float alpha)
        {
            var fadeCamvas = new GameObject("FadeCanvas");
            var canvas = fadeCamvas.AddComponent<Canvas>();
            canvas.sortingOrder = 100;
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            fadeCamvas.AddComponent<GraphicRaycaster>();
            var image = fadeCamvas.AddComponent<RawImage>();

            color.a = alpha;
            image.color = color;

            return image;
        }
    }
}