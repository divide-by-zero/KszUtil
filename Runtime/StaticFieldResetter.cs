using System;
using System.Collections.Generic;
using UnityEngine;

namespace KszUtil
{
    /// <summary>
    /// Domain Reload 無効時に static 変数をリセットするためのユーティリティ。
    /// 各クラスの static コンストラクタから Register() でリセット処理を登録してください。
    /// </summary>
    public static class StaticFieldResetter
    {
        private static readonly List<Action> ResetActions = new();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetAll()
        {
            foreach (var action in ResetActions)
                action();
        }

        public static void Register(Action resetAction)
        {
            if (!ResetActions.Contains(resetAction))
                ResetActions.Add(resetAction);
        }
    }
}