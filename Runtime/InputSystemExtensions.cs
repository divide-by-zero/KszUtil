using System;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

namespace KszUtil
{
    public static class InputSystemExtensions
    {
        public static IObservable<Unit> OnPerformedAsObservable(this InputAction inputAction)
        {
            return Observable.FromEvent<InputAction.CallbackContext>(
                h => inputAction.performed += h,
                h => inputAction.performed -= h
            ).AsUnitObservable();
        }

        public static IObservable<Vector2Int> OnInputVector2AsObservable(this InputAction inputAction)
        {
            return Observable.FromEvent<InputAction.CallbackContext>(
                h => inputAction.performed += h,
                h => inputAction.performed -= h
            ).Select(ctx => Vector2Int.RoundToInt(ctx.ReadValue<UnityEngine.Vector2>()));
        }
    }
}