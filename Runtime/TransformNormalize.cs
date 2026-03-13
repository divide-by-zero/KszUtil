using System;
using System.Linq;
using UniRx;
using UnityEngine;

namespace KszUtil
{
    public class TransformNormalize : MonoBehaviour
    {
        [Serializable]
        public class PosRot
        {
            public Vector3 Pos;
            public Quaternion Rot;
            public Vector3 Scale;
        }

        [SerializeField] private PosRot begin;
        [SerializeField] private PosRot end;
        [SerializeField] private bool isReference;
        [SerializeField] private bool isParent;

        public FloatReactiveProperty NormalizeTime = new FloatReactiveProperty();

        // Start is called before the first frame update
        void Start()
        {
            if (isReference)
            {
                begin.Pos = transform.localPosition;
                begin.Rot = transform.localRotation;
                begin.Scale = transform.localScale;
            }

            if (isParent)
            {
            }

            NormalizeTime.Subscribe(f => { UpdatePos(f); }).AddTo(this);
        }

        private void UpdatePos(float f)
        {
            if (isParent)
            {
                foreach (var child in GetComponentsInChildren<TransformNormalize>().Where(normalize => normalize != this))
                {
                    child.NormalizeTime.Value = f;
#if UNITY_EDITOR
                    child.OnValidate();
#endif
                }
            }
            else
            {
                transform.localPosition = Vector3.Lerp(begin.Pos, end.Pos, f);
                transform.localRotation = Quaternion.Lerp(begin.Rot, end.Rot, f);
                transform.localScale = Vector3.Lerp(begin.Scale, end.Scale, f);
            }
        }
#if UNITY_EDITOR
        private void Reset()
        {
            this.RecordBegin();
            this.RecordEnd();
        }

        private void OnValidate()
        {
            UpdatePos(NormalizeTime.Value);
        }

        [ContextMenu("BeginRecord")]
        private void RecordBegin()
        {
            UnityEditor.Undo.RecordObject(this, "BeginRecord");
            begin.Pos = transform.localPosition;
            begin.Rot = transform.localRotation;
            begin.Scale = transform.localScale;
        }

        [ContextMenu("EndRecord")]
        private void RecordEnd()
        {
            UnityEditor.Undo.RecordObject(this, "EndRecord");
            end.Pos = transform.localPosition;
            end.Rot = transform.localRotation;
            end.Scale = transform.localScale;
        }
#endif
    }
}