using UnityEngine;

namespace KszUtil
{
    public class SmoothTransform : MonoBehaviour
    {
        public Vector3 TargetPosition { set; get; }
        public Vector3 TargetScale { set; get; }
        public Quaternion TargetRotation { set; get; }
        public float TimeFact { set; get; } = 1;
        public void Start()
        {
            TargetPosition = transform.localPosition;
            TargetScale = transform.localScale;
            TargetRotation = transform.localRotation;
        }

        public void Update()
        {
            var t = Mathf.Pow(10, -Time.deltaTime * TimeFact); //1秒で今いる場所から1/10まで間を詰めるための値

            transform.localPosition = Vector3.Lerp(transform.localPosition, TargetPosition, t);
            transform.localScale = Vector3.Lerp(transform.localScale, TargetScale, t);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, TargetRotation, t);
        }
    }
}
