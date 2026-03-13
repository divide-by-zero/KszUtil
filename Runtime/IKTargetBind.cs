using UnityEngine;

namespace KszUtil
{
    public class IKTargetBind : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private IKTarget[] _ikTargets;

        private void OnAnimatorIK(int layerIndex)
        {
            foreach (var ikTarget in _ikTargets)
            {
                ikTarget.ApplyAnimator(_animator);
            }
        }
    }
}
