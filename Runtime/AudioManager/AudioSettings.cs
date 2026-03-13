using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

namespace KszUtil.AudioManager
{
    [CreateAssetMenu]
    public class AudioSettings : ScriptableObject
    {
        /// <summary>
        /// 登録用データ AudioClip に別名を付ける
        /// </summary>
        [Serializable]
        public class RegisteredSeData
        {
            public string name;
            public AudioClip audioClip;

            [Range(0, 2)] public float volume = 1;
            [Range(0, 2)] public float pitch = 1;
            public bool isLoop = false;
        }

        /// <summary>
        /// 登録用データのリスト
        /// </summary>
        [SerializeField] private List<RegisteredSeData> _registerSeDataList;

        public AudioMixer AudioMixer;
        public AudioMixerGroup MixerBgmGroup;
        public AudioMixerGroup MixerSeGroup;

        public IReadOnlyList<RegisteredSeData> registeredSeDataList => _registerSeDataList;

#if UNITY_EDITOR
        //AudioClip登録文字列を管理するEnumを作成
        private void CreatePathClass()
        {
            //ディクショナリー初期化
            var registeredAudioDictionary = _registerSeDataList.Select(data => data.name).ToList();

            var mono = UnityEditor.MonoScript.FromScriptableObject(this);
            var path = UnityEditor.AssetDatabase.GetAssetPath(mono);
            var folder = Path.GetDirectoryName(path);

            //定数クラス作成
            ConstantsClassCreator.Create("Utilities", "AudioEnum", "登録されたAudioClipを定数で管理するクラス", registeredAudioDictionary, folder);
        }

        [ContextMenu("GenerateEnum")]
        private void GenerateEnum()
        {
            this.CreatePathClass();
        }
#endif
    }
}