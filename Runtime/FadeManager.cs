using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace KszUtil
{
    /// <summary>
    ///     シーン遷移時のフェードイン・アウトを制御するためのクラス
    /// </summary>
    public class FadeManager_ : SingletonMonoBehaviour<FadeManager_> //TODO そろそろ要らない
    {
        /// <summary>暗転用黒テクスチャ</summary>
        private Texture2D blackTexture;

        /// <summary>フェード中の透明度</summary>
        private float fadeAlpha = 0;

        /// <summary>フェード中かどうか</summary>
        private bool isFading;

        private Color targetColor;

        protected override void Awake()
        {
            base.Awake();
            if (this != Instance)
            {
                Destroy(this);
                return;
            }

            DontDestroyOnLoad(gameObject);
            targetColor = Color.black;
        }

        /// <summary>
        ///     画面遷移
        /// </summary>
        /// <param name='scene'>シーン名</param>
        /// <param name='interval'>暗転にかかる時間(秒)</param>
        public void LoadLevel(string scene, float interval)
        {
            StartCoroutine(TransScene(scene, interval / 2.0f));
        }

        /// <summary>
        ///     シーン遷移用コルーチン
        /// </summary>
        /// <param name='scene'>シーン名</param>
        /// <param name='interval'>暗転にかかる時間(秒)</param>
        private IEnumerator TransScene(string scene, float interval)
        {
            //だんだん暗く
            var raw = SetFadeObject();
            isFading = true;
            float time = 0;
            while (time <= interval)
            {
                if (raw == null)
                {
                    yield return null;
                    continue;
                }

                targetColor.a = Mathf.Lerp(0f, 1f, time / interval);
                raw.color = targetColor;
                time += Time.deltaTime;
                yield return 0;
            }

            targetColor.a = 1.0f;
            raw.color = targetColor;

            //シーン切替
            if (scene == null)
            {
                Application.Quit();
            }
            else
            {
                yield return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(scene);
            }

            //だんだん明るく
            time = 0;
            raw = SetFadeObject();

            while (time <= interval)
            {
                if (raw == null)
                {
                    yield return null;
                    continue;
                }

                targetColor.a = Mathf.Lerp(1f, 0f, time / interval);
                raw.color = targetColor;
                time += Time.deltaTime;
                yield return 0;
            }

            isFading = false;
            Destroy(raw.gameObject);
        }

        private RawImage SetFadeObject()
        {
            var fadeCamvas = new GameObject("FadeCanvas");
            var canvas = fadeCamvas.AddComponent<Canvas>();
            canvas.sortingOrder = 100;
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            fadeCamvas.AddComponent<GraphicRaycaster>();
            var image = fadeCamvas.AddComponent<RawImage>();
            return image;
        }

        public void LoadScene(string scene)
        {
            LoadLevel(scene, 0.5f);
        }
    }
}