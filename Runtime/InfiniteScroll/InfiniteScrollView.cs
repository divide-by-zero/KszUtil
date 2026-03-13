using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace KszUtil.InfiniteScroll
{
    [RequireComponent(typeof(ScrollRect))]
    public abstract class InfiniteScrollView<TModel, TContext> : MonoBehaviour where TContext : class, new()
    {
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private GridLayoutGroup _gridLayout;
        [SerializeField] private int _outRangeReserveCount = 2; // 画面外に表示するセルの数
        [SerializeField] private int _reserveCount = 2; // 画面内に表示するセルの数
        [SerializeField] private InfiniteScrollCell<TModel, TContext> _cellPrefab;
        [SerializeField] private bool _isLoop;
        [SerializeField] private ScrollDirection _scrollDirection;

        public enum ScrollDirection
        {
            Horizontal,
            Vertical
        }

        private InfiniteScrollCell<TModel, TContext>[] _cells = Array.Empty<InfiniteScrollCell<TModel, TContext>>();

        public TContext Context { get; } = new();

        // public IObservable<TModel> OnClickAsObservable() => _onCellCreateSubject.Select(cell => cell.OnSelectAsObservable()).Merge();
        private ReplaySubject<InfiniteScrollCell<TModel, TContext>> _onCellCreateSubject = new();
        public IObservable<InfiniteScrollCell<TModel, TContext>> OnCellCreateAsObservable() => _onCellCreateSubject;
        private IScrollItemModelBindable _itemModelBindable;

        private Vector2 CellSize => _gridLayout.cellSize + _gridLayout.spacing;
        private int ScrollIndex => (int)_scrollDirection; //スクロール方向 0:横 1:縦 Vector2 の Indexアクセスに使用
        public Vector2Int ValidVec => _scrollDirection == ScrollDirection.Horizontal ? new Vector2Int(1, 0) : new Vector2Int(0, 1);

        private void Reset()
        {
            TryGetComponent(out _scrollRect);
            _gridLayout = AddOrCreateComponent<GridLayoutGroup>(_scrollRect.content);
            var contentSizeFitter = AddOrCreateComponent<ContentSizeFitter>(_gridLayout);
            contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }

        private T AddOrCreateComponent<T>(Component component) where T : Component
        {
            if (component.TryGetComponent(out T tComponent))
            {
                return tComponent;
            }

            return component.gameObject.AddComponent<T>();
        }

        private void OnValidate()
        {
            _gridLayout.constraint = _scrollDirection == ScrollDirection.Horizontal ? GridLayoutGroup.Constraint.FixedRowCount : GridLayoutGroup.Constraint.FixedColumnCount;
            _gridLayout.constraintCount = 1;
            _scrollRect.horizontal = _scrollDirection == ScrollDirection.Horizontal;
            _scrollRect.vertical = _scrollDirection == ScrollDirection.Vertical;

            _scrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHide;
            _scrollRect.horizontalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHide;

            if (_isLoop)
            {
                _scrollRect.movementType = ScrollRect.MovementType.Unrestricted;
            }
        }

        public void BindDataSource(IScrollItemModelBindable dataSource)
        {
            _itemModelBindable = dataSource;

            _cells = new InfiniteScrollCell<TModel, TContext>[_reserveCount + _outRangeReserveCount * 2];
            for (var i = 0; i < _cells.Length; i++)
            {
                var cell = Instantiate(_cellPrefab, _gridLayout.transform);
                cell.SetContext(Context);
                _cells[i] = cell;
                _onCellCreateSubject.OnNext(cell);
            }

            LeftTopIndex.Subscribe(_ => UpdateCells());
            _scrollRect.OnValueChangedAsObservable()
                .Subscribe(_ => UpdateScroll())
                .AddTo(this);

            if (_isLoop)
            {
                Observable.EveryUpdate()
                    .Select(_ => _scrollRect.content.position)
                    .DistinctUntilChanged()
                    .Throttle(TimeSpan.FromSeconds(0.5f))
                    .Subscribe(_ => PositionNormalize())
                    .AddTo(this);
            }
        }

        private void PositionNormalize()
        {
            var rectTransform = _gridLayout.transform.ToRectTransform();
            if (LeftTopIndex.Value > _itemModelBindable.MaxCell())
            {
                rectTransform.anchoredPosition += new Vector2(CellSize[ScrollIndex] * _itemModelBindable.MaxCell(), 0);
            }

            if (LeftTopIndex.Value < 0)
            {
                rectTransform.anchoredPosition -= new Vector2(CellSize[ScrollIndex] * _itemModelBindable.MaxCell(), 0);
            }
        }

        //現在の左上端のセルのインデックス
        public IntReactiveProperty LeftTopIndex { private set; get; } = new IntReactiveProperty();

        /// <summary>
        /// 現在の左上端のセルのインデックスを更新
        /// </summary>
        private void UpdateScroll()
        {
            var rectTransform = _gridLayout.transform.ToRectTransform();
            var leftTop = new Vector2(-rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y);
            LeftTopIndex.Value = (int)(leftTop[ScrollIndex] / CellSize[ScrollIndex]);
        }

        public void UpdateCells()
        {
            var startIndex = LeftTopIndex.Value;

            var totalWidth = CellSize * _itemModelBindable.MaxCell();
            var leftTop = startIndex * CellSize[ScrollIndex] - _outRangeReserveCount * CellSize[ScrollIndex];
            var rightBottom = totalWidth[ScrollIndex] - (startIndex + _reserveCount + _outRangeReserveCount) * CellSize[ScrollIndex];
            if (_scrollDirection == ScrollDirection.Horizontal)
            {
                _gridLayout.padding = new RectOffset((int)leftTop, (int)rightBottom, 0, 0);
            }
            else
            {
                _gridLayout.padding = new RectOffset(0, 0, (int)leftTop, (int)rightBottom);
            }

            for (var index = 0; index < _cells.Length; index++)
            {
                var cell = _cells[index];
                var currentIndex = startIndex + index - _outRangeReserveCount;

                if (_isLoop)
                {
                    while (currentIndex < 0) currentIndex += _itemModelBindable.MaxCell();
                    while (currentIndex >= _itemModelBindable.MaxCell()) currentIndex -= _itemModelBindable.MaxCell();
                }

                var itemModel = _itemModelBindable.GetItemModel(currentIndex);
                cell.UpdateModel(itemModel);
            }
        }

        /// <summary>
        /// 指定したIndex番目のCellを中心にする
        /// </summary>
        /// <param name="saveDataPlayerIndex"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void ScrollToCenter(int centerIndex)
        {
            if (_scrollDirection == ScrollDirection.Horizontal)
            {
                _scrollRect.content.anchoredPosition = new Vector2(-centerIndex * CellSize.x + (_scrollRect.viewport.rect.width - CellSize.x) / 2, 0);
            }
            else
            {
                _scrollRect.content.anchoredPosition = new Vector2(0, -centerIndex * CellSize.y + (_scrollRect.viewport.rect.height - CellSize.y) / 2);
            }

            UpdateScroll();
        }

        //TODO Func<int> MaxCell を外から指定できるでもよかったかもなぁ

        //スクロールアイテムモデルをバインドするためのインターフェース
        public interface IScrollItemModelBindable
        {
            TModel GetItemModel(int index);
            int MaxCell();
        }
    }

    #region NullContext

    [RequireComponent(typeof(ScrollRect))]
    public abstract class InfiniteScrollView<TModel> : InfiniteScrollView<TModel, InfiniteScrollView<TModel>.NullContext>
    {
        public sealed class NullContext
        {
        }
    }

    #endregion
}