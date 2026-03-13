using UnityEngine;

namespace KszUtil
{
    public class RandomMusicPlay : MonoBehaviour
    {
        public AudioClip[] Clips;

        // Use this for initialization
        void Start()
        {
            var s = Clips.RandomAt();
            AudioManager.AudioManager.Instance.PlayBGM(s, Clips.Length == 1); //1曲しかないのでループさせる
        }

        // Update is called once per frame
        void Update()
        {
            if (AudioManager.AudioManager.Instance.AudioBgmPlayers.Source.loop) return;
            if (AudioManager.AudioManager.Instance.AudioBgmPlayers?.Source.time >= AudioManager.AudioManager.Instance.AudioBgmPlayers.Source.clip.length - 3.0f)
            {
                var s = Clips.RandomAt(); //本当は今再生しているもの以外の曲を選ぶ
                AudioManager.AudioManager.Instance.PlayBGM(s, false);
            }
        }
    }
}