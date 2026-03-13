using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace KszUtil.InfiniteScroll.Sample
{
    public class StringInfiniteScrollCell : InfiniteScrollCell<string, StringInfiniteScrollView.DataContext>
    {
        [SerializeField] private Button _button;
        [SerializeField] private Text _text;

        protected override void OnSetContext(StringInfiniteScrollView.DataContext dataContext)
        {
            _button.OnClickAsObservable().Subscribe(_ => Context.OnClickSubject.OnNext(Model)).AddTo(this);
            dataContext.SelectString.Subscribe(s => UpdateColor(s)).AddTo(this);
        }

        protected override void OnUpdateModel(string itemModel)
        {
            _text.text = itemModel;
            UpdateColor(Context.SelectString.Value);
        }

        private void UpdateColor(string s)
        {
            _text.color = s == Model ? Color.red : Color.black;
        }

        private void OnMouseOver()
        {
            Context.OnMouseOverSubject.OnNext(Model);
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            Context.OnMouseOverSubject.OnNext(Model);
        }
    }
}