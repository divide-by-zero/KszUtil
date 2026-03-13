using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

namespace KszUtil.AudioManager
{
    public class AudioManager
    {
        public static AudioManager Instance { get; private set; }

        private AudioMixer _audioMixer;
        private AudioMixerGroup _mixerBgmGroup;
        private AudioMixerGroup _mixerSeGroup;

        private GameObject _audioSourceHolder;

        private GameObject audioSourceHolder
        {
            get
            {
                _audioSourceHolder = _audioSourceHolder == null ? new GameObject(nameof(AudioManager)) : _audioSourceHolder;
                GameObject.DontDestroyOnLoad(_audioSourceHolder);
                return _audioSourceHolder;
            }
        }

        public float SeVolume
        {
            set
            {
                GameConfig.SeVolume = value;
                _audioMixer.SetFloat("seVolume", Todb(value / 10.0f));
            }
            get => GameConfig.SeVolume;
        }

        /// <summary>
        /// ０～１の値をいい感じのdbの値に変換する
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float Todb(float value)
        {
            return Mathf.Max(Mathf.Log10(value * value) * 20, -80);
        }

        public float BgmVolume
        {
            set
            {
                GameConfig.BgmVolume = value;
                _audioMixer.SetFloat("bgmVolume", Todb(value / 10.0f));
            }
            get => GameConfig.BgmVolume;
        }

        public AudioManager(AudioSettings audioSettings)
        {
            Instance = this;
            foreach (var registerSeData in audioSettings.registeredSeDataList)
            {
                RegisterSe(registerSeData.name, registerSeData.audioClip, registerSeData.volume, registerSeData.pitch, registerSeData.isLoop);
            }

            for (var i = 0; i < audioSourceList.Length; ++i)
            {
                if (audioSourceList[i] == null)
                {
                    audioSourceList[i] = audioSourceHolder.AddComponent<AudioSource>();
                }
            }

            _mixerBgmGroup = audioSettings.MixerBgmGroup;
            _mixerSeGroup = audioSettings.MixerSeGroup;
            _audioMixer = audioSettings.AudioMixer;
        }

        private bool _isMute;

        public void Mute(bool isMute)
        {
            if (isMute == true)
            {
                this.AllStop();
            }

            _isMute = isMute;
        }

        /// <summary>
        /// 管理用データ どのAudioClipをいつ、どのAudioSourceで再生したのかを管理
        /// </summary>
        class SoundFormat
        {
            public AudioClip clip;
            public AudioSource audioSource;
            public float volume;
            public float pitch;
            public bool isLoop;
            public int playedFrame;
        }

        /// <summary>
        /// AudioSource(音の発生源)を先に用意 とりあえず20
        /// </summary>
        private AudioSource[] audioSourceList = new AudioSource[20];

        private const int playableFrameDistance = 10;
        private Dictionary<string, SoundFormat> sndList = new Dictionary<string, SoundFormat>();

        public void RegisterSe(string name, AudioClip clip, float sndVolume = 1.0f, float sndPitch = 1.0f, bool isLoop = false)
        {
            //登録済み
            if (sndList.ContainsKey(name))
            {
                return;
            }

            var snd = new SoundFormat();
            snd.playedFrame = -1000;
            snd.clip = clip;
            snd.volume = sndVolume;
            snd.pitch = sndPitch;
            snd.isLoop = isLoop;
            sndList.Add(name, snd);
        }

        public void Play(AudioEnum name, float? volume = null, float? pitch = null)
        {
#pragma warning disable CS0612
            Play(name.ToPath(), volume, pitch);
#pragma warning restore CS0612
        }

        public void Stop(AudioEnum name)
        {
#pragma warning disable CS0612
            Stop(name.ToPath());
#pragma warning restore CS0612
        }

        [Obsolete]
        public void Play(string name, float? volume = null, float? pitch = null)
        {
            if (sndList.TryGetValue(name, out var snd) == false)
            {
                Debug.LogWarning($"Target Sound Not Registered[{name}]");
                return; //その名前登録されてない
            }

            if (Time.frameCount - snd.playedFrame < playableFrameDistance) return; //そんな短いフレームでの再生は無視

            //前回のAudioSourceが再生中なら一旦止める
            if (snd.audioSource?.clip == snd.clip && snd.audioSource.isPlaying)
            {
                if (snd.isLoop) return; //Loop再生されているものなので、上書きでPlayはさせない
                snd.audioSource.Stop();
            }

            snd.playedFrame = Time.frameCount;
            snd.audioSource = this.GetEmptyAudioSource();
            Play(snd.audioSource, snd.clip, volume ?? snd.volume, pitch ?? snd.pitch, snd.isLoop);
        }

        [Obsolete]
        public void Stop(string name)
        {
            if (sndList.ContainsKey(name) == false)
            {
                Debug.LogWarning($"Target Sound Not Registered[{name}]");
                return; //その名前登録されてない
            }

            var snd = sndList[name];
            if (snd.audioSource?.clip == snd.clip && snd.audioSource.isPlaying)
            {
                snd.audioSource.Stop();
            }
        }

        public void Play(AudioSource audio, AudioClip clip, float volume = 1.0f, float pitch = 1.0f, bool isLoop = false)
        {
            if (_isMute) return; //Mute中
            if (audio == null) return;
            audio.clip = clip;
            audio.outputAudioMixerGroup = _mixerSeGroup;
            audio.volume = volume;
            audio.loop = isLoop;
            audio.pitch = pitch;
            audio.Play();
        }

        public void AllStop()
        {
            foreach (var snd in sndList.Values)
            {
                if (snd?.audioSource?.isPlaying == true)
                {
                    snd.audioSource.Stop();
                }
            }
        }

        private AudioSource GetEmptyAudioSource()
        {
            return audioSourceList.FirstOrDefault(source => source?.isPlaying == false);
        }

        public AudioPlayer AudioBgmPlayers { get; set; }

        public void PlayBGM(AudioClip clip, bool isLoop = true)
        {
            if (AudioBgmPlayers != null && AudioBgmPlayers.Source.clip == clip) return; //すでに同じ曲がBGMになっている場合は再生スキップ
            var emptyAudioSource = GetEmptyAudioSource();
            if (emptyAudioSource == null) return; //再生可能なAudioSourceがない
            var mainAuidioPlayer = audioSourceHolder.AddComponent<AudioPlayer>();
            mainAuidioPlayer.Source = emptyAudioSource;
            mainAuidioPlayer.Source.loop = isLoop;
            mainAuidioPlayer.Source.clip = clip;
            mainAuidioPlayer.Source.pitch = 1;
            mainAuidioPlayer.Source.outputAudioMixerGroup = _mixerBgmGroup;
            mainAuidioPlayer.IsFade = true;
            mainAuidioPlayer.FadeInSeconds = 1.5f;
            mainAuidioPlayer.Play();
            AudioBgmPlayers?.StopFadeOut(1.5f);
            AudioBgmPlayers = mainAuidioPlayer;
        }

        public void SetVolume(AudioEnum audioEnum, float f)
        {
            if (sndList.TryGetValue(audioEnum.ToPath(), out var soundFormat))
            {
                soundFormat.volume = f;
            }
        }
    }
}