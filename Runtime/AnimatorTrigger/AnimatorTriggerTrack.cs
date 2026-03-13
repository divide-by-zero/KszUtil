using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
#if UNITY_EDITOR
#endif

namespace KszUtil.AnimatorTrigger
{
    /// <summary>
    /// Track that can be used to control the active state of a GameObject.
    /// </summary>
    [Serializable]
    [TrackClipType(typeof(AnimatorPlayableAsset))]
    [TrackBindingType(typeof(Animator))]
    [ExcludeFromPreset]
#if UNITY_EDITOR
    [DisplayName("Animator Trigger Track")]
#endif
    public class AnimatorTriggerTrack : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            // TestMixerBehaviourを作成
            var playable = ScriptPlayable<AnimatorDummyMixerBehaviour>.Create(graph, inputCount);
            var trackBinding = go.GetComponent<PlayableDirector>().GetGenericBinding(this) as Animator;
            // TrackにバインドされたGameObjectをTestMixerBehaviourに渡す
            playable.GetBehaviour().m_TrackBinding = trackBinding;

            foreach (var timelineClip in GetClips())
            {
                var animatorPlayableAsset = timelineClip.asset as AnimatorPlayableAsset;
                if (animatorPlayableAsset != null)
                {
                    timelineClip.displayName = animatorPlayableAsset.parameter.ParameterName;
                }
            }

            return playable;
        }
    }
}