using UnityEngine;
using UnityEngine.Timeline;

namespace KszUtil.AnimatorTrigger
{
    [TrackClipType(typeof(PlayableFloatTriggerClip))]
    [TrackBindingType(typeof(Animator))]
    public class AnimatorFloatParameterTrack : TrackAsset
    {
    }
}