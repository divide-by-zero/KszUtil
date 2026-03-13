using UnityEngine;
using UnityEngine.Playables;

namespace KszUtil.AnimatorTrigger
{
    class AnimatorParameterBehaviour : PlayableBehaviour
    {
        public AnimatorPlayableAsset.IAnimatorParameter parameter;
        public Animator animator;
        private bool _isEnterPlaying;

        // クリップが開始されたときに呼び出されます
        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            base.OnBehaviourPlay(playable, info);
            if (animator == null)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(parameter.ParameterName))
            {
                return;
            }

            _isEnterPlaying = true;
            switch (parameter.ParameterType)
            {
                case AnimatorPlayableAsset.ParameterType.Trigger:
                    animator.SetTrigger(parameter.ParameterName);
                    break;
                case AnimatorPlayableAsset.ParameterType.Bool:
                    animator.SetBool(parameter.ParameterName, parameter.BoolValue);
                    break;
                case AnimatorPlayableAsset.ParameterType.Int:
                    animator.SetInteger(parameter.ParameterName, parameter.IntValue);
                    break;
                case AnimatorPlayableAsset.ParameterType.Float:
                    animator.SetFloat(parameter.ParameterName, parameter.FloatValue);
                    break;
            }
        }

        // クリップが一時停止または終了したときに呼び出されます
        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            base.OnBehaviourPause(playable, info);
            if (_isEnterPlaying == false)
            {
                return;
            }

            if (animator == null)
            {
                return;
            }

            switch (parameter.ParameterType)
            {
                case AnimatorPlayableAsset.ParameterType.Trigger:
                    animator.ResetTrigger(parameter.ParameterName);
                    break;
                case AnimatorPlayableAsset.ParameterType.Bool:
                    animator.SetBool(parameter.ParameterName, !parameter.BoolValue);
                    break;
                case AnimatorPlayableAsset.ParameterType.Int:
                    break;
                case AnimatorPlayableAsset.ParameterType.Float:
                    break;
            }
        }
    }
}