using UnityEngine;

namespace KszUtil
{
    public static class TransformExtension
    {
        private static Vector3 work;

        public static Transform SetPosition(this Transform t, float? x = null, float? y = null, float? z = null)
        {
            work = t.transform.localPosition;
            if (x.HasValue) work.x = x.Value;
            if (y.HasValue) work.y = y.Value;
            if (z.HasValue) work.z = z.Value;
            t.transform.localPosition = work;
            return t;
        }

        public static Transform AddPosition(this Transform t, float x = 0, float y = 0, float z = 0)
        {
            work = t.transform.localPosition;
            work.x += x;
            work.y += y;
            work.z += z;
            t.transform.localPosition = work;
            return t;
        }

        public static Transform AddPosition(this Transform t, Vector2 add)
        {
            return AddPosition(t, add.x, add.y);
        }

        public static Transform AddPosition(this Transform t, Vector3 add)
        {
            return AddPosition(t, add.x, add.y, add.z);
        }

        public static Transform SetAngle(this Transform t, float? x = null, float? y = null, float? z = null)
        {
            work = t.transform.localRotation.eulerAngles;
            if (x.HasValue) work.x = x.Value;
            if (y.HasValue) work.y = y.Value;
            if (z.HasValue) work.z = z.Value;
            t.transform.localRotation = Quaternion.Euler(work);
            return t;
        }

        public static Transform AddAngle(this Transform t, float x = 0, float y = 0, float z = 0)
        {
            work = t.transform.localRotation.eulerAngles;
            work.x += x;
            work.y += y;
            work.z += z;
            t.transform.localRotation = Quaternion.Euler(work);
            return t;
        }

        public static Transform AddAngle(this Transform t, Vector2 add)
        {
            return AddAngle(t, add.x, add.y);
        }

        public static Transform AddAngle(this Transform t, Vector3 add)
        {
            return AddAngle(t, add.x, add.y, add.z);
        }

        public static Transform SetScale(this Transform t, float? x = null, float? y = null, float? z = null)
        {
            work = t.transform.localScale;
            if (x.HasValue) work.x = x.Value;
            if (y.HasValue) work.y = y.Value;
            if (z.HasValue) work.z = z.Value;
            t.transform.localScale = work;
            return t;
        }

        public static Transform AddScale(this Transform t, float x = 0, float y = 0, float z = 0)
        {
            work = t.transform.localScale;
            work.x += x;
            work.y += y;
            work.z += z;
            t.transform.localScale = work;
            return t;
        }

        public static Transform AddScale(this Transform t, Vector2 add)
        {
            return AddScale(t, add.x, add.y);
        }

        public static Transform AddScale(this Transform t, Vector3 add)
        {
            return AddScale(t, add.x, add.y, add.z);
        }

        public static RectTransform ToRectTransform(this Transform t)
        {
            return t as RectTransform;
        }
    }
}
