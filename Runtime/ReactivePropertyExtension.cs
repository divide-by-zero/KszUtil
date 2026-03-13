using DG.Tweening;
using UniRx;

namespace KszUtil
{
    public static class ReactivePropertyExtension
    {
        public static Tweener DOValueChange(
            this ReactiveProperty<float> target,
            float endValue,
            float duration)
        {
            return DOTween.To(() => target.Value, value => target.Value = value, endValue, duration);
        }
    }
}
