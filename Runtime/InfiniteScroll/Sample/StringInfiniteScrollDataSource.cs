using UniRx;
using UnityEngine;

namespace KszUtil.InfiniteScroll.Sample
{
    public class StringInfiniteScrollDataSource : MonoBehaviour, StringInfiniteScrollView.IScrollItemModelBindable,NullScrollView.IScrollItemModelBindable
    {
        [SerializeField] private StringInfiniteScrollView _infiniteScrollView;
        [SerializeField] private NullScrollView _nullScrollView;

        private void Start()
        {
            _infiniteScrollView.BindDataSource(this);
            _infiniteScrollView.OnClickAsObservable().Subscribe(itemModel =>
            {
                // _infiniteScrollView.ScrollToCenter(30);
                Debug.Log(itemModel);
                _infiniteScrollView.Context.SelectString.Value = itemModel;
            });
            _infiniteScrollView.OnMouseOverAsObservable().Subscribe(s =>
                Debug.Log($"MouseOver: {s}")).AddTo(this);
            
            
        }

        public string GetItemModel(int index)
        {
            return index.ToString();
        }

        object InfiniteScrollView<object, InfiniteScrollView<object>.NullContext>.IScrollItemModelBindable.GetItemModel(int index)
        {
            return GetItemModel(index);
        }

        public int MaxCell() => 100;
    }
}