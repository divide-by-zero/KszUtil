using System;

namespace KszUtil
{
    public static class EnumExtension
    {
        /// <summary>
        /// 指定したフラグの中の一つでも当てはまるかどうか
        /// </summary>
        public static bool HasAnyFlags<T>(this T TargetPlatformType, T platformType) where T : Enum
        {
            return (Convert.ToInt32(TargetPlatformType) & Convert.ToInt32(platformType)) != 0;
        }

        /// <summary>
        /// 指定した複合フラグを含んでいるかどうか
        /// </summary>
        public static bool HasAllFlag<T>(this T TargetPlatformType, T platformType) where T : Enum
        {
            return TargetPlatformType.HasFlag(platformType);
        }
    }
}