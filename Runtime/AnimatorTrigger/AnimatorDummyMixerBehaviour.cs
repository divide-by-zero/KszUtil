using UnityEngine;
using UnityEngine.Playables;

namespace KszUtil.AnimatorTrigger
{
    /// <summary>
    /// 各子Clipに親TrackにBindされたGameObjectを渡すためのダミー
    /// </summary>
    class AnimatorDummyMixerBehaviour : PlayableBehaviour
    {
        // トラックに入っているGameObject
        public Animator m_TrackBinding;

        // timelineの開始時 初期化
        public override void OnGraphStart(Playable playable)
        {
            if (m_TrackBinding == default)
            {
                return;
            }

            // Trackの全clipの数を取得
            var inputCount = playable.GetInputCount();

            for (var i = 0; i < inputCount; i++)
            {
                var inputPlayable = (ScriptPlayable<AnimatorParameterBehaviour>)playable.GetInput(i);
                var input = inputPlayable.GetBehaviour();
                input.animator = m_TrackBinding;
            }
        }
    }
}