using System;
using UnityEngine;
using UnityEngine.Playables;

namespace KszUtil.AnimatorTrigger
{
    [Serializable]
    public class PlayableFloatTriggerClip : PlayableAsset
    {
        [SerializeField] private PlayableTriggerFloatBehaviour value;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            return ScriptPlayable<PlayableTriggerFloatBehaviour>.Create(graph, value);
        }
    }
}