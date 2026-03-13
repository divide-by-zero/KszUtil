using System;
using UniRx;

namespace KszUtil.InfiniteScroll.Sample
{
    public class StringInfiniteScrollView : InfiniteScrollView<string, StringInfiniteScrollView.DataContext>
    {
        public class DataContext
        {
            public Subject<string> OnClickSubject { get; } = new();
            public Subject<string> OnMouseOverSubject { get; } = new();
            public ReactiveProperty<string> SelectString { get; } = new();
        }

        public IObservable<string> OnClickAsObservable() => base.Context.OnClickSubject;
        public IObservable<string> OnMouseOverAsObservable() => base.Context.OnMouseOverSubject;
    }
}