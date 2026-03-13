using System;
using System.Linq;
using UniRx;
using UnityEngine;
namespace KszUtil
{
    public class RectTransformNormalize : MonoBehaviour
    {
        [Serializable]
        public class PosRot
        {
            public Vector2 sizeDelta;
            public Vector2 anchoredPosition;
            public Vector3 localScale;
            public Vector2 rectTransformAnchorMin;
            public Vector2 rectTransformAnchorMax;
            public Vector2 rectTransformPivot;

            public void Record(RectTransform rectTransform)
            {
                sizeDelta = rectTransform.sizeDelta;
                anchoredPosition = rectTransform.anchoredPosition;
                localScale = rectTransform.localScale;
                rectTransformAnchorMin = rectTransform.anchorMin;
                rectTransformAnchorMax = rectTransform.anchorMax;
                rectTransformPivot = rectTransform.pivot;
            }
        }

        [SerializeField] private PosRot begin;
        [SerializeField] private PosRot end;
        [SerializeField] private bool isParent;

        public FloatReactiveProperty NormalizeTime = new FloatReactiveProperty();

        // Start is called before the first frame update
        void Start()
        {
            if (isParent)
            {
            }

            NormalizeTime.Subscribe(f =>
            {
                this.UpdatePos(f);
            }).AddTo(this);
        }

        private void UpdatePos(float f)
        {
            if (isParent)
            {
                foreach (var child in this.GetComponentsInChildren<RectTransformNormalize>().Where(child => child != this))
                {
                    child.NormalizeTime.Value = f;
                    #if UNITY_EDITOR
                    child.OnValidate();
                    #endif
                }
            }
            else
            {
                var rectTransform = this.transform.ToRectTransform();
                rectTransform.sizeDelta = Vector2.Lerp(begin.sizeDelta, end.sizeDelta, f);
                rectTransform.anchoredPosition = Vector2.Lerp(begin.anchoredPosition, end.anchoredPosition, f);
                rectTransform.localScale = Vector3.Lerp(begin.localScale, end.localScale, f);
                rectTransform.anchorMin = Vector2.Lerp(begin.rectTransformAnchorMin, end.rectTransformAnchorMin, f);
                rectTransform.anchorMax = Vector2.Lerp(begin.rectTransformAnchorMax, end.rectTransformAnchorMax, f);
                rectTransform.pivot = Vector2.Lerp(begin.rectTransformPivot, end.rectTransformPivot, f);
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
            this.UpdatePos(NormalizeTime.Value);
        }

        [ContextMenu("BeginRecord")]
        private void RecordBegin()
        {
            UnityEditor.Undo.RecordObject(this, "BeginRecord");
            begin.Record(transform.ToRectTransform());
        }

        [ContextMenu("EndRecord")]
        private void RecordEnd()
        {
            UnityEditor.Undo.RecordObject(this, "EndRecord");
            end.Record(transform.ToRectTransform());
        }
      #endif
    }
}
