using UnityEngine;

namespace KszUtil.InfiniteScroll.Sample
{
    public class NullCell : InfiniteScrollCell<object>
    {
        protected override void OnUpdateModel(object itemoModel)
        {
            Debug.Log(itemoModel);
        }
    }
}