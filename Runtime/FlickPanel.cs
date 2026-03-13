using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace KszUtil
{
    public class FlickPanel : MonoBehaviour
    {
        public Action<float, float> OnFlick { set; get; }
        public Action<float, float> OnDrag { set; get; }

        //タップ・フリック検出時間
        public const float tapDetectTime = 2;

        //タップ検出距離
        public const float tapDetectLength = 10;
        private const float tapDetectLengthSqrt = tapDetectLength * tapDetectLength;

        private Vector2 currentPos;
        private float holdTime;
        private Vector2 tapPos;
        private float tapTime;

        private static bool isDrag;

        void Start()
        {
            gameObject.AddEvent(EventTriggerType.PointerDown).AddListener(arg =>
            {
                var eventData = arg as PointerEventData;
                var mousePos = eventData.position;

                isDrag = true;
                tapPos = mousePos;
                tapTime = Time.time;
                currentPos = mousePos;
            });

            gameObject.AddEvent(EventTriggerType.Drag).AddListener(arg =>
            {
                var eventData = arg as PointerEventData;
                var mousePos = eventData.position;
                if (isDrag)
                {
                    var d = currentPos - mousePos;
                    if (d.sqrMagnitude > tapDetectLengthSqrt)
                    {
                        if (d.x * d.x > d.y * d.y)
                        { //xの方が移動量が多い
                            OnDrag?.Invoke(d.x, 0);
                        }
                        else
                        { //y軸の方が移動量が多い
                            OnDrag?.Invoke(0, d.y);
                        }
                    }
                    currentPos = mousePos;
                }
            });

            gameObject.AddEvent(EventTriggerType.PointerUp).AddListener(arg =>
            {
                {
                    var eventData = arg as PointerEventData;
                    var mousePos = eventData.position;
                    if (isDrag)
                    {
                        if (Time.time - tapTime < tapDetectTime)
                        {
                            var d = tapPos - mousePos;
                            if (d.sqrMagnitude >= tapDetectLengthSqrt)
                                if (d.x * d.x > d.y * d.y)
                                {
                                    OnFlick?.Invoke(d.x, 0);
                                }
                                else
                                {
                                    OnFlick?.Invoke(0, d.y);
                                }
                        }
                    }
                }
                isDrag = false;
            });
        }
    }
}
