using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace KszUtil.SceneManager
{
    public interface ISceneBase
    {
        UniTask _InitializeAsync();
        object Vm { set; get; }
    }

    public abstract class SceneBase : SceneBase<object>
    {
    }

    public abstract class SceneBase<TViewModel> : MonoBehaviour, ISceneBase
    {
        public TViewModel ViewModel
        {
            set => Vm = value; //Vmに直接入れてもいいが、こっちの方が IDEの保管が効くので一応用意
            get => (TViewModel)_vm;
        }

        private object _vm;

        public object Vm
        {
            get => _vm;
            set
            {
                if (value != null && value is TViewModel == false) throw new Exception("ViewModelの型が違うよ");
                _vm = value;
                //                _InitializeAsync();   //TODO VMセットしたらInitializeAsyncってのもいいかなぁと思ったのだけれど。暴発しているっぽい
            }
        }

        public async UniTask _InitializeAsync()
        {
            await InitializeAsync(ViewModel);
            IsInitialize = true;
        }

        protected virtual UniTask InitializeAsync(TViewModel viewModel) => UniTask.CompletedTask;

        public bool IsInitialize { private set; get; }

        private void Awake()
        {
            if (IsInitialize == false && KszSceneManager.Instance.IsInjection == false)
            {
                Debug.Log("まだ初期化されていないっぽいですし、シーン遷移中でもないようなので自分でInitialize呼びますね");
                _InitializeAsync();
            }
        }
    }
}