using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace KszUtil
{
    public static class UniTaskExtension
    {
        /// <summary>
        /// スローモーション処理 Utilに移動しても良いかも
        /// </summary>
        /// <param name="slow"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public static async UniTask SlowmotionAsync(float slow, float time, CancellationToken ct)
        {
            Time.timeScale = slow;
            await UniTask.Delay(TimeSpan.FromSeconds(time), true, cancellationToken: ct);
            Time.timeScale = 1.0f;
        }

        // public static IObservable<PointerEventData> OnDragAsObservable(this Component component)
        // {
        //     if (component == null || component.gameObject == null) return Observable.Empty<PointerEventData>();
        //     return GetOrAddComponent<ObservableDragTrigger>(component.gameObject).OnDragAsObservable();
        // }
        //
        // public static IObservable<PointerEventData> OnBeginDragAsObservable(this Component component)
        // {
        //     if (component == null || component.gameObject == null) return Observable.Empty<PointerEventData>();
        //     return GetOrAddComponent<ObservableBeginDragTrigger>(component.gameObject).OnBeginDragAsObservable();
        // }
        //
        // public static IObservable<PointerEventData> OnEndDragAsObservable(this Component component)
        // {
        //     if (component == null || component.gameObject == null) return Observable.Empty<PointerEventData>();
        //     return GetOrAddComponent<ObservableEndDragTrigger>(component.gameObject).OnEndDragAsObservable();
        // }

        static T GetOrAddComponent<T>(GameObject gameObject) where T : Component
        {
            var component = gameObject.GetComponent<T>();
            if (component == null)
            {
                component = gameObject.AddComponent<T>();
            }

            return component;
        }
    }
}