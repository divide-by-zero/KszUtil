using UnityEngine;

namespace KszUtil
{
    public static class GameConfig
    {
        public static float BgmVolume
        {
            set { PlayerPrefs.SetFloat("BgmVolume", value); }
            get { return PlayerPrefs.GetFloat("BgmVolume", 5); }
        }

        public static float SeVolume
        {
            set { PlayerPrefs.SetFloat("SeVolume", value); }
            get { return PlayerPrefs.GetFloat("SeVolume", 5); }
        }

        public static int ResolutionLevel
        {
            set { PlayerPrefs.SetInt("ResolutionLevel", value); }
            get { return PlayerPrefs.GetInt("ResolutionLevel", 0); }
        }

        public static void Reset()
        {
            PlayerPrefs.DeleteAll();
        }
    }
}