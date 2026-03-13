using System;
using System.Linq;
using UniRx;
using UnityEngine;

namespace KszUtil
{
    public class Rigidbody2DTransformNormalize : MonoBehaviour
    {
        [Serializable]
        public class PosRot
        {
            public Vector3 Pos;
            public Vector3 Rot;
            public Vector3 Scale;
        }

        [SerializeField] private PosRot begin;
        [SerializeField] private PosRot end;
        [SerializeField] private bool isReference;
        [SerializeField] private bool isParent;
        [SerializeField] private Rigidbody2D _rigidbody2D;

        public PosRot Begin => begin;
        public PosRot End => end;

        public FloatReactiveProperty NormalizeTime = new FloatReactiveProperty();

        void Start()
        {
            if (isReference)
            {
                begin.Pos = transform.localPosition;
                begin.Rot = transform.localRotation.eulerAngles;
                begin.Scale = transform.localScale;
            }

            if (isParent)
            {
            }

            NormalizeTime.Subscribe(f => UpdatePos(f)).AddTo(this);
        }

        private void UpdatePos(float f)
        {
            if (isParent)
            {
                foreach (var child in GetComponentsInChildren<Rigidbody2DTransformNormalize>().Where(normalize => normalize != this))
                {
                    child.NormalizeTime.Value = f;
#if UNITY_EDITOR
                    child.OnValidate();
#endif
                }
            }
            else
            {
                if (_rigidbody2D == null || transform.parent == null) return;
                if (Application.isPlaying == false)
                {
                    _rigidbody2D.transform.localPosition = Vector3.Lerp(begin.Pos, end.Pos, f);
                    _rigidbody2D.transform.localRotation = Quaternion.Euler(0, Mathf.Lerp(begin.Rot.y, end.Rot.y, f), Mathf.Lerp(begin.Rot.z, end.Rot.z, f));
                    _rigidbody2D.transform.localScale = Vector3.Lerp(begin.Scale, end.Scale, f);
                }
                else
                {
                    _rigidbody2D.MovePosition(_rigidbody2D.transform.parent.TransformPoint(Vector3.Lerp(begin.Pos, end.Pos, f)));
                    _rigidbody2D.MoveRotation(transform.parent.eulerAngles.z + Mathf.Lerp(begin.Rot.z, end.Rot.z, f));
                    if (begin.Scale != end.Scale)
                    {
                        _rigidbody2D.transform.localScale = Vector3.Lerp(begin.Scale, end.Scale, f);
                    }
                }
            }
        }
#if UNITY_EDITOR
        private void Reset()
        {
            this.RecordBegin();
            this.RecordEnd();
            TryGetComponent(out _rigidbody2D);
        }

        private void OnValidate()
        {
            _rigidbody2D.isKinematic = true;
            UpdatePos(NormalizeTime.Value);
        }

        [ContextMenu("BeginRecord")]
        private void RecordBegin()
        {
            UnityEditor.Undo.RecordObject(this, "BeginRecord");
            begin.Pos = _rigidbody2D.transform.localPosition;
            begin.Rot = _rigidbody2D.transform.localRotation.eulerAngles;
            begin.Scale = _rigidbody2D.transform.localScale;
        }

        [ContextMenu("EndRecord")]
        private void RecordEnd()
        {
            UnityEditor.Undo.RecordObject(this, "EndRecord");
            end.Pos = _rigidbody2D.transform.localPosition;
            end.Rot = _rigidbody2D.transform.localRotation.eulerAngles;
            end.Scale = _rigidbody2D.transform.localScale;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.parent.TransformPoint(begin.Pos), 1f);
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.parent.TransformPoint(begin.Pos), transform.parent.TransformPoint(end.Pos));
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.parent.TransformPoint(end.Pos), 1f);
        }
#endif
    }
}