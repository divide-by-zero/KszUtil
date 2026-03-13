using System;
using UnityEngine;
using UnityEngine.Playables;

namespace KszUtil.AnimatorTrigger
{
    [Serializable]
    public class PlayableTriggerFloatBehaviour : PlayableBehaviour
    {
        [SerializeField] private string parameterName;
        [SerializeField] private float currentValue;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            var animator = playerData as Animator;
            if (animator == default)
            {
                return;
            }

            animator.SetFloat(parameterName, currentValue);
        }
    }
}