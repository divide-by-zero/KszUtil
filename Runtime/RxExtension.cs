using System;
using System.Collections.Generic;
using DG.Tweening;
using UniRx;

namespace KszUtil
{
    public static class RxExtension
    {
        /// <summary>
        /// 値を変更せずに、通知だけしたい
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rp"></param>
        public static void ForceNotify<T>(this ReactiveProperty<T> rp)
        {
            rp.SetValueAndForceNotify(rp.Value);
        }

        // /// <summary>
        // /// DOTweenのアニメーションをObservableに変換する
        // /// </summary>
        // /// <param name="tween"></param>
        // /// <returns></returns>
        // public static IObservable<Unit> ToObservable(this Tweener tween)
        // {
        //     var subject = new Subject<Unit>();
        //     tween.OnComplete(() =>
        //     {
        //         subject.OnNext(Unit.Default);
        //         subject.OnCompleted();
        //     });
        //     return subject;
        // }

        /// <summary>
        /// 中身を全て入れ替える.
        /// </summary>
        public static ReactiveCollection<T> Set<T>(this ReactiveCollection<T> self, IList<T> source)
        {
            var before = self.Count;
            var after = source.Count;
            var minCount = before < after ? before : after;

            //Replace.
            for (var i = 0; i < minCount; i++)
            {
                self[i] = source[i];
            }

            //Add.
            for (var i = before; i < after; i++)
            {
                self.Add(source[i]);
            }

            //Remove.
            for (var i = before - 1; after <= i; i--)
            {
                self.RemoveAt(i);
            }
            return self;
        }
    }
}
