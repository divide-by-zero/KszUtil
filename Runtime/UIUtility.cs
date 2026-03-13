using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KszUtil
{
    public static class UIUtility
    {
        public static void SetDirty(this GameObject obj)
        {
            LayoutRebuilder.MarkLayoutForRebuild(obj.transform as RectTransform);
        }

        public static bool SetProperty<T>(ref T currentValue, T newValue) where T : struct
        {
            if (currentValue.Equals((object)newValue)) return false;
            currentValue = newValue;
            return true;
        }

        public static EventTrigger.TriggerEvent AddEvent(this GameObject o, EventTriggerType e)
        {
            var trigger = o.GetComponent<EventTrigger>() ?? o.AddComponent<EventTrigger>();
            var entry = new EventTrigger.Entry();
            entry.eventID = e;
            trigger.triggers.Add(entry);
            return entry.callback;
        }

        public static void RemoveEvent(this GameObject o, EventTriggerType e)
        {
            var trigger = o.GetComponent<EventTrigger>() ?? o.AddComponent<EventTrigger>();
            trigger.triggers.Remove(new EventTrigger.Entry
            {
                eventID = e
            });
        }

        private static IEnumerator LoadUrl(this RawImage image, string url)
        {
            var www = new WWW(url);
            while (www.isDone == false)
            {
                yield return null;
            }
            if (www.error != null)
            {
                yield break;
            }
            image.texture = www.texture;
        }

        public enum Corner
        {
            LeftTop,
            RightTop,
            LeftBottom,
            RightBottom,
        }

        public static Bounds GetWorldBounds(this RectTransform rectTransform)
        {
            var corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);

            var center = corners.Aggregate(Vector3.zero, (current, corner) => current + corner) / corners.Length;
            var size = new Vector3(
                corners.Max(corner => corner.x) - corners.Min(corner => corner.x),
                corners.Max(corner => corner.y) - corners.Min(corner => corner.y),
                1);
            return new Bounds(center, size);
        }

        public static Vector2 GetRelativePosition(this RectTransform rectTransform, Corner corner)
        {
            var parentBounds = rectTransform.parent.GetComponent<RectTransform>().GetWorldBounds();
            var bounds = rectTransform.GetWorldBounds();

            var pos = Vector2.zero;

            switch (corner)
            {
                case Corner.LeftBottom:
                case Corner.RightBottom:
                    pos.y = bounds.min.y - parentBounds.min.y;
                    break;
                case Corner.LeftTop:
                case Corner.RightTop:
                    pos.y = parentBounds.max.y - bounds.max.y;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("corner", corner, null);
            }

            switch (corner)
            {
                case Corner.LeftTop:
                case Corner.LeftBottom:
                    pos.x = bounds.min.x - parentBounds.min.x;
                    break;
                case Corner.RightTop:
                case Corner.RightBottom:
                    pos.x = parentBounds.max.x - bounds.max.x;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("corner", corner, null);
            }

            return pos;
        }

        public static void SetRelativePosition(this RectTransform rectTransform, Vector2 pos, Corner corner)
        {
            var parentBounds = rectTransform.parent.GetComponent<RectTransform>().GetWorldBounds();
            var bounds = rectTransform.GetWorldBounds();
            var anchoredPosition = rectTransform.position;

            switch (corner)
            {
                case Corner.LeftBottom:
                case Corner.RightBottom:
                    anchoredPosition.y += parentBounds.min.y - bounds.min.y + pos.y;
                    break;
                case Corner.LeftTop:
                case Corner.RightTop:
                    anchoredPosition.y += parentBounds.max.y - bounds.max.y - pos.y;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("corner", corner, null);
            }

            switch (corner)
            {
                case Corner.LeftTop:
                case Corner.LeftBottom:
                    anchoredPosition.x += parentBounds.min.x - bounds.min.x + pos.x;
                    break;
                case Corner.RightTop:
                case Corner.RightBottom:
                    anchoredPosition.x += parentBounds.max.x - bounds.max.x - pos.x;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("corner", corner, null);
            }

            rectTransform.position = anchoredPosition;
        }
    }
}
