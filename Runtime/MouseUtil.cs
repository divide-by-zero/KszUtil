using UnityEngine;

namespace KszUtil
{
    public static class MouseUtil
    {
        public static bool UnprojectMousePosition(out Vector3 worldPos, Vector2 mousePos, Plane plane)
        {
            float depth;
            Camera cam = Camera.main;
            if (cam == null) cam = Camera.current;
            if (cam == null)
            {
                worldPos = Vector3.zero;
                return false;
            }

            Ray ray = cam.ScreenPointToRay(mousePos);

            if (plane.Raycast(ray, out depth))
            {
                worldPos = ray.origin + ray.direction * depth;
                return true;
            }
            else
            {
                worldPos = Vector3.zero;
                return false;
            }
        }

        public static bool UnprojectMousePosition(out Vector3 worldPos, out Vector3 normal, Vector2 mousePos, LayerMask mask)
        {
            Camera cam = Camera.main;
            normal = Vector3.up;
            if (cam == null) cam = Camera.current;
            if (cam == null)
            {
                worldPos = Vector3.zero;
                return false;
            }

            Ray ray = cam.ScreenPointToRay(mousePos);

            if (Physics.Raycast(ray, out var hit, 100, mask))
            {
                worldPos = hit.point;
                normal = hit.normal;
                return true;
            }
            else
            {
                worldPos = Vector3.zero;
                return false;
            }
        }

        public static bool UnprojectMousePositionXZ(out Vector3 worldPos, Vector2 mousePos)
        {
            return UnprojectMousePosition(out worldPos, mousePos, new Plane(Vector3.up, new Vector3(0, 0, 0)));
        }

        public static bool UnprojectMousePositionXY(out Vector3 worldPos, Vector2 mousePos)
        {
            return UnprojectMousePosition(out worldPos, mousePos, new Plane(Vector3.forward, new Vector3(0, 0, 0)));
        }

        public static Vector3 ClampToScreen(Vector3 worldPos)
        {
            Camera cam = Camera.main;
            if (cam == null) cam = Camera.current;
            if (cam == null)
            {
                return Vector3.zero;
            }

            return cam.WorldToViewportPoint(worldPos);
        }

        public static Vector2 ToXYScreenPos(this Vector3 mousePos)
        {
            Vector3 pos;
            UnprojectMousePositionXY(out pos, mousePos);
            return pos;
        }

        public static Vector2 ToXYScreenPos(this Vector2 mousePos)
        {
            Vector3 pos;
            UnprojectMousePositionXY(out pos, mousePos);
            return pos;
        }
    }
}