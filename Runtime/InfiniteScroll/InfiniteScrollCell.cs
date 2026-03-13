using System;
using UnityEngine;

namespace KszUtil.InfiniteScroll
{
    public abstract class InfiniteScrollCell<TModel, TContext> : MonoBehaviour where TContext : class, new()
    {
        public void UpdateModel(TModel itemModel)
        {
            Model = itemModel;
            OnUpdateModel(itemModel);
        }

        public void SetContext(TContext context)
        {
            Context = context;
            OnSetContext(context);
        }

        protected virtual void OnSetContext(TContext context)
        {
        }

        protected abstract void OnUpdateModel(TModel itemoModel);

        public TModel Model { protected set; get; }
        public TContext Context { private set; get; }
    }

    #region NullContext

    public abstract class InfiniteScrollCell<TModel> : InfiniteScrollCell<TModel, InfiniteScrollView<TModel>.NullContext>
    {
    }

    #endregion
}