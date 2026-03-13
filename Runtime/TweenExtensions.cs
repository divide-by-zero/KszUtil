using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UniRx;
using UnityEngine;

namespace KszUtil
{
    public static class TweenExtensions
    {
        /// <summary>
        /// DOTweenのアニメーションをObservableに変換する
        /// </summary>
        /// <param name="tween"></param>
        /// <returns></returns>
        public static IObservable<Unit> ToObservable(this Tweener tween)
        {
            var subject = new Subject<Unit>();
            tween.OnComplete(() =>
            {
                subject.OnNext(Unit.Default);
                subject.OnCompleted();
            });
            return subject;
        }

        /// <summary>
        /// Tweener を Sequence に変換
        /// </summary>
        /// <param name="tween"></param>
        /// <returns></returns>
        public static Sequence ToSequence(this Tweener tween)
        {
            return DOTween.Sequence().Append(tween);
        }

        /// <summary>
        /// Tweener を Sequence に変換しつつ Tween を Append(逐次処理)
        /// </summary>
        /// <param name="tween"></param>
        /// <param name="appendTween"></param>
        /// <returns></returns>
        public static Sequence Append(this Tweener tween, Tween appendTween)
        {
            return tween.ToSequence().Append(appendTween);
        }

        /// <summary>
        /// Tweener を Sequence に変換しつつ Tween を Join(同時処理)
        /// </summary>
        /// <param name="tween"></param>
        /// <param name="appendTween"></param>
        /// <returns></returns>
        public static Sequence Join(this Tweener tween, Tween joinTween)
        {
            return tween.ToSequence().Join(joinTween);
        }

        /// <summary>
        /// Tween を Coroutine 内の yield return による待機が出来るように変換
        /// </summary>
        /// <param name="tween"></param>
        /// <returns></returns>
        public static CustomYieldInstruction ToYieldInstruction(this Tween tween)
        {
            var isComplete = false;

            TweenCallback onComplete = null;
            onComplete = () =>
            {
                tween.onComplete -= onComplete;
                isComplete = true;
            };
            tween.onComplete += onComplete;

            return new WaitUntil(() => isComplete);
        }

        public static TweenerCore<Vector4, Vector4, VectorOptions> DoOffset(this RectTransform target, float left, float top, float right, float bottom, float duration, bool snapping = false)
        {
            return DoOffset(target, new Vector4(left, bottom, right, top), duration, snapping);
        }

        public static TweenerCore<Vector4, Vector4, VectorOptions> DoOffset(this RectTransform target, Vector2 endMinValue, Vector2 endMaxValue, float duration, bool snapping = false)
        {
            return DoOffset(target, new Vector4(endMinValue.x, endMinValue.y, endMaxValue.x, endMaxValue.y), duration, snapping);
        }

        public static TweenerCore<Vector4, Vector4, VectorOptions> DoOffset(this RectTransform target, Vector4 endValue, float duration, bool snapping = false)
        {
            TweenerCore<Vector4, Vector4, VectorOptions> t =
                DOTween.To(() => new Vector4(target.offsetMin.x, target.offsetMin.y, -target.offsetMax.x, -target.offsetMax.y),
                    x =>
                    {
                        target.offsetMin = new Vector2(x.x, x.y);
                        target.offsetMax = new Vector2(-x.z, -x.w);
                    }, endValue, duration);
            t.SetOptions(snapping).SetTarget(target);
            return t;
        }

    }
}
