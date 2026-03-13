using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace KszUtil
{
    [RequireComponent(typeof(Button))]
    public class MouseBind : MonoBehaviour
    {
        [SerializeField] private Button _targetButton;

        [SerializeField] private bool _isDirect;

        public int _mouseButtonId;

        private InputAction _inputAction;

        private void Reset()
        {
            _targetButton = GetComponent<Button>();
        }

        private static List<RaycastResult> raycastResultList = new List<RaycastResult>();
        private PointerEventData _pointerEventData;

        private void Awake()
        {
            if (_targetButton == null)
            {
                TryGetComponent(out _targetButton);
            }

            _inputAction = _mouseButtonId switch
            {
                0 => InputSystem.actions.FindAction("UI/Click"),
                1 => InputSystem.actions.FindAction("UI/RightClick"),
                2 => InputSystem.actions.FindAction("UI/MiddleClick")
            };
        }

        private void Update()
        {
            if (_targetButton.interactable == false) return;

            var isButtonPressed = _inputAction.WasPerformedThisFrame();
            var isButtonReleased = _inputAction.WasReleasedThisFrame();

            //指定したキーの押下
            if (isButtonPressed)
            {
//                var rect = RectTransformUtility.CalculateRelativeRectTransformBounds(_targetButton.transform);
//                Debug.Log(_targetButton.transform.position + "," + rect);
                _pointerEventData = new PointerEventData(EventSystem.current)
                {
                    button = PointerEventData.InputButton.Left,
                    position = _targetButton.transform.position // + rect.center //指定したボタンの位置にマウスがある体  //TODO 左上指定とかの場合に他のパネルの方が優先順位が叩くて、イベントが吸われることがある
                };

                if (_isDirect)
                {
                    _pointerEventData.pointerPress = _targetButton.gameObject;
                    ExecuteEvents.Execute(_targetButton.gameObject, _pointerEventData, ExecuteEvents.pointerDownHandler);
                    return;
                }

                EventSystem.current.RaycastAll(_pointerEventData, raycastResultList);
                var validGameObject = raycastResultList.Select(result => result.gameObject).FirstOrDefault(go => go != null); //一番最初にぶつかっている有効なGameObject取得
                raycastResultList.Clear();
                if (validGameObject == null)
                {
                    return;
                }

                var currentPointerDownHandlerObject = ExecuteEvents.GetEventHandler<IPointerDownHandler>(validGameObject); //ボタン位置にあるGameObjectからIPointerDownHandlerを保持しているGameObjectを取得
                if (currentPointerDownHandlerObject != _targetButton.gameObject)
                {
                    return; //ボタン位置から得られたGameObjectとボタンのGameObjectが異なる＝別のもので遮られている ので処理しない
                }

                _pointerEventData.pointerPress = currentPointerDownHandlerObject;
                ExecuteEvents.Execute(currentPointerDownHandlerObject, _pointerEventData, ExecuteEvents.pointerDownHandler);
            }

            //指定したキーの押上
            if (_pointerEventData != null && _pointerEventData.pointerPress != null && isButtonReleased)
            {
                ExecuteEvents.Execute(_pointerEventData.pointerPress, _pointerEventData, ExecuteEvents.pointerUpHandler);
                ExecuteEvents.Execute(_pointerEventData.pointerPress, _pointerEventData, ExecuteEvents.pointerClickHandler);
                _pointerEventData = null;
            }
        }
    }
}