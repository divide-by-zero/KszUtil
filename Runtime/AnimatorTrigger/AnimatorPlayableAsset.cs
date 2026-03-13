using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Playables;

namespace KszUtil.AnimatorTrigger
{
    /// <summary>
    /// Playable Asset class for Animator Tracks
    /// </summary>
#if UNITY_EDITOR
    [DisplayName("Animator Trigger Clip")]
#endif
    [Serializable]
    class AnimatorPlayableAsset : PlayableAsset
    {
        public enum ParameterType
        {
            Trigger,
            Bool,
            Int,
            Float
        }

        public interface IAnimatorParameter
        {
            string ParameterName { get; }
            int IntValue { get; }
            float FloatValue { get; }
            bool BoolValue { get; }
            ParameterType ParameterType { get; }
        }

        public abstract class AnimatorParameterBase : IAnimatorParameter
        {
            public string parameterName;

            protected AnimatorParameterBase(IAnimatorParameter srcParam)
            {
                parameterName = srcParam?.ParameterName;
            }

            public string ParameterName => parameterName;
            public abstract ParameterType ParameterType { get; }

            public virtual int IntValue => 0;
            public virtual float FloatValue => 0;
            public virtual bool BoolValue => false;
        }

        [Serializable]
        public class AnimatorIntParameter : AnimatorParameterBase
        {
            public int intValue;

            public override ParameterType ParameterType => ParameterType.Int;
            public override int IntValue => intValue;

            public AnimatorIntParameter(IAnimatorParameter srcParam) : base(srcParam)
            {
            }
        }

        [Serializable]
        public class AnimatorFloatParameter : AnimatorParameterBase
        {
            public float floatValue;

            public override ParameterType ParameterType => ParameterType.Float;
            public override float FloatValue => floatValue;

            public AnimatorFloatParameter(IAnimatorParameter srcParam) : base(srcParam)
            {
            }
        }

        [Serializable]
        public class AnimatorBoolParameter : AnimatorParameterBase
        {
            public bool boolValue;

            public override ParameterType ParameterType => ParameterType.Bool;
            public override bool BoolValue => boolValue;

            public AnimatorBoolParameter(IAnimatorParameter srcParam) : base(srcParam)
            {
            }
        }

        [Serializable]
        public class AnimatorTriggerParameter : AnimatorParameterBase
        {
            public override ParameterType ParameterType => ParameterType.Trigger;

            public AnimatorTriggerParameter(IAnimatorParameter srcParam) : base(srcParam)
            {
            }
        }

        public ParameterType parameterType;
        [SerializeReference] public AnimatorParameterBase parameter = new AnimatorTriggerParameter(null);

        private void OnValidate()
        {
            switch (parameterType)
            {
                case ParameterType.Int when parameter is not AnimatorIntParameter:
                    parameter = new AnimatorIntParameter(parameter);
                    break;
                case ParameterType.Float when parameter is not AnimatorFloatParameter:
                    parameter = new AnimatorFloatParameter(parameter);
                    break;
                case ParameterType.Bool when parameter is not AnimatorBoolParameter:
                    parameter = new AnimatorBoolParameter(parameter);
                    break;
                case ParameterType.Trigger when parameter is not AnimatorTriggerParameter:
                    parameter = new AnimatorTriggerParameter(parameter);
                    break;
            }
        }

        public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
        {
            var playable = ScriptPlayable<AnimatorParameterBehaviour>.Create(graph);

            var animatorControlPlayableBehaviour = playable.GetBehaviour();
            animatorControlPlayableBehaviour.parameter = parameter;

            return playable;
        }
    }
}