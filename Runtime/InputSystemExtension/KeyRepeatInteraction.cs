using UnityEngine;
using UnityEngine.InputSystem;

namespace KszUtil.InputSystemExtension
{
    public class KeyRepeatInteraction : IInputInteraction
    {
        // ボタンが最初に押されてからリピート処理が始まるまでの時間[s]
        public float repeatDelay = 0.5f;

        // ボタンが押されている間のリピート処理の間隔[s]
        public float repeatInterval = 0.2f;

        // ボタンの閾値（0の場合はデフォルト設定値を使用）
        public float pressPoint = 0;

        // 設定値かデフォルト値の値を格納するフィールド
        private float PressPointOrDefault => pressPoint > 0 ? pressPoint : InputSystem.settings.defaultButtonPressPoint;
        private float ReleasePointOrDefault => PressPointOrDefault * InputSystem.settings.buttonReleaseThreshold;

        // 次のリピート時刻[s]
        private double _nextRepeatTime;

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#else
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
#endif
        public static void Initialize()
        {
            // 初回にInteractionを登録する必要がある
            InputSystem.RegisterInteraction<KeyRepeatInteraction>();
        }

        public void Process(ref InputInteractionContext context)
        {
            // 設定値のチェック
            if (repeatDelay <= 0 || repeatInterval <= 0)
            {
                Debug.LogError("initialDelayとrepeatIntervalは0より大きい値を設定してください。");
                return;
            }

            if (context.timerHasExpired)
            {
                // リピート時刻に達したら再びPerformedに遷移
                if (context.time >= _nextRepeatTime)
                {
                    // リピート処理の次回実行時刻を設定
                    _nextRepeatTime = context.time + repeatInterval;

                    // リピート時の処理
                    context.PerformedAndStayPerformed();

                    // 次のリピート時刻にProcessメソッドが呼ばれるようにタイムアウトを設定
                    context.SetTimeout(repeatInterval);
                }

                return;
            }

            switch (context.phase)
            {
                case InputActionPhase.Waiting:
                    // ボタンが押されたらStartedに遷移
                    if (context.ControlIsActuated(PressPointOrDefault))
                    {
                        // ボタンが押された時の処理
                        context.Started();
                        context.PerformedAndStayPerformed();

                        // リピート処理の初回実行時刻を設定
                        _nextRepeatTime = context.time + repeatDelay;

                        // 次のリピート時刻にProcessメソッドが呼ばれるようにタイムアウトを設定
                        context.SetTimeout(repeatDelay);
                    }

                    break;

                case InputActionPhase.Performed:
                    // ボタンが離されたらCanceledに遷移
                    if (!context.ControlIsActuated(ReleasePointOrDefault))
                    {
                        // ボタンが離された時の処理
                        context.Canceled();
                    }

                    break;
            }
        }

        public void Reset()
        {
            _nextRepeatTime = 0;
        }
    }
}