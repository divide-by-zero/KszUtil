namespace KszUtil.AudioManager
{
    /// <summary>
    /// 登録されたAudioClipを定数で管理するクラス 
    /// </summary> 
    public enum AudioEnum
    {
        Coin,
        Damage,
        FreeHand,
        BoneCrash,
        StaminaNone,
        Trap,
        Save,
        StaminaMax,
        Splash,
        DragStart,
        DragEnd,
        DragGrab,
        SwitchOn,
        SwitchOff,
        ItemPurchase,
        ButtonPositive,
        ButtonNegative,
        AvatarGet,
        Purchase,
        Jump,
        CupIn,
        PopEmote,
    }

    public static class AudioEnumExtension
    {
        public static string ToPath(this AudioEnum evalue)
        {
            switch (evalue)
            {
                case AudioEnum.Coin: return "Coin";
                case AudioEnum.Damage: return "Damage";
                case AudioEnum.FreeHand: return "FreeHand";
                case AudioEnum.BoneCrash: return "BoneCrash";
                case AudioEnum.StaminaNone: return "StaminaNone";
                case AudioEnum.Trap: return "Trap";
                case AudioEnum.Save: return "Save";
                case AudioEnum.StaminaMax: return "StaminaMax";
                case AudioEnum.Splash: return "Splash";
                case AudioEnum.DragStart: return "DragStart";
                case AudioEnum.DragEnd: return "DragEnd";
                case AudioEnum.DragGrab: return "DragGrab";
                case AudioEnum.SwitchOn: return "SwitchOn";
                case AudioEnum.SwitchOff: return "SwitchOff";
                case AudioEnum.ItemPurchase: return "ItemPurchase";
                case AudioEnum.ButtonPositive: return "ButtonPositive";
                case AudioEnum.ButtonNegative: return "ButtonNegative";
                case AudioEnum.AvatarGet: return "AvatarGet";
                case AudioEnum.Purchase: return "Purchase";
                case AudioEnum.Jump: return "Jump";
                case AudioEnum.CupIn: return "CupIn";
                case AudioEnum.PopEmote: return "PopEmote";
            }

            return null;
        }
    }
}