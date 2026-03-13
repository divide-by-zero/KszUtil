using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;

namespace KszUtil.Timeline
{
    public abstract class RunnableMarkerBase : MarkerBase
    {
        internal IExposedPropertyTable ExposedPropertyTable { private get; set; }

        public abstract void Run();

        protected T Resolve<T>(ExposedReference<T> exposedReference)
            where T : Object
        {
            return exposedReference.Resolve(ExposedPropertyTable);
        }

#if UNITY_EDITOR
        // Marker.OnInitialize(MarkerAsset) をオーバーライドすると「Serialize 中に FindObjectsOfType() 呼ぶなし」とかで怒られるので OnEnable() で対応
        private void OnEnable()
        {
            if (parent == default)
            {
                // Marker 新規追加直後は parent (TrackAsset) が null の状態になってしまい、RunnableMarkerReceiver をアタッチする
                // 対象の GameObject を見付けられないため、呼び出しを1フレーム遅延させるコトで何とかする
                UniTask.DelayFrame(1).ContinueWith(VerifyRunnableMarkerReceiverAttached).Forget();
                return;
            }

            VerifyRunnableMarkerReceiverAttached();
        }

        private void VerifyRunnableMarkerReceiverAttached()
        {
            var unattachedGameObjects = RunnableMarkerReceiverUnattached;
            foreach (var unattachedGameObject in unattachedGameObjects)
            {
                if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
                {
                    Debug.LogWarning($"[Booth] Marker イベントを正しく受信させるために {unattachedGameObject.name} に RunnableMarkerReceiver コンポーネントを追加してください。", unattachedGameObject);
                }
                else
                {
                    unattachedGameObject.AddComponent<RunnableMarkerReceiver>();
                    UnityEditor.EditorUtility.SetDirty(unattachedGameObject);
                    Debug.Log($"[Booth] Marker イベントを正しく受信させるために {unattachedGameObject.name} に RunnableMarkerReceiver コンポーネントを追加しました。", unattachedGameObject);
                }
            }
        }

        private IEnumerable<GameObject> RunnableMarkerReceiverUnattached =>
            FindObjectsOfType<PlayableDirector>()
                .Where(x => x != default && parent != default && x.playableAsset == parent.timelineAsset)
                .Select(x => x.gameObject)
                .Where(x => x.GetComponent<RunnableMarkerReceiver>() == default);
#endif
    }
}