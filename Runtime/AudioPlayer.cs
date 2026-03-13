using UnityEngine;

namespace KszUtil
{
    public class AudioPlayer : MonoBehaviour
    {
        /// <summary>
        /// �I�[�f�B�I�\�[�X�B
        /// </summary>
        public AudioSource Source;

        /// <summary>
        /// �t�F�[�h�C���Đ����s�����ǂ����B
        /// </summary>
        public bool IsFade;

        /// <summary>
        /// �t�F�[�h�C�����鎞�̎��ԁB
        /// </summary>
        public double FadeInSeconds = 1.0;

        /// <summary>
        /// �t�F�[�h�C���Đ������ǂ���
        /// </summary>
        bool IsFadePlaying = false;

        /// <summary>
        /// �t�F�[�h�A�E�g�Đ������ǂ���
        /// </summary>
        bool IsFadeStopping = false;

        /// <summary>
        /// �t�F�[�h�A�E�g���鎞�̎��ԁB
        /// </summary>
        double FadeOutSeconds = 1.0;

        /// <summary>
        /// �t�F�[�h�C��/�A�E�g�o�ߎ��ԁB
        /// </summary>
        double FadeDeltaTime = 0;

        /// <summary>
        /// ��{�{�����[���B
        /// </summary>
        float BaseVolume;

        public bool IsFinish { private set; get; }

        /// <summary>
        /// �t���[���������B
        /// </summary>
        void Update()
        {
            // �t�F�[�h�C��
            if (IsFadePlaying)
            {
                FadeDeltaTime += Time.deltaTime;
                if (FadeDeltaTime >= FadeInSeconds)
                {
                    FadeDeltaTime = FadeInSeconds;
                    IsFadePlaying = false;
                }

                Source.volume = (float)(FadeDeltaTime / FadeInSeconds) * BaseVolume;
            }

            // �t�F�[�h�A�E�g
            if (IsFadeStopping)
            {
                FadeDeltaTime += Time.deltaTime;
                if (FadeDeltaTime >= FadeOutSeconds)
                {
                    FadeDeltaTime = FadeOutSeconds;
                    IsFadePlaying = false;
                    Stop();
                }

                Source.volume = (float)(1.0 - FadeDeltaTime / FadeOutSeconds) * BaseVolume;
            }

            if (IsFinish == false && Source.loop == false)
            {
                if (Source.isPlaying == false) IsFinish = true;
            }
        }

        /// <summary>
        /// �Đ����s���܂��B
        /// </summary>
        public void Play()
        {
            BaseVolume = 0.3f;
            FadeDeltaTime = 0;
            Source.volume = 0;
            Source.Play();
            IsFadePlaying = true;
            IsFadeStopping = false;
            IsFinish = false;
        }

        /// <summary>
        /// ��~���s���܂��B
        /// </summary>
        public void Stop()
        {
            Source.Stop();
            Destroy(this);
        }

        /// <summary>
        /// ��~���s���܂��B
        /// <param name="fadeSec">�t�F�[�h�A�E�g�����܂ł̕b���B</param>
        /// </summary>
        public void StopFadeOut(double fadeSec)
        {
            BaseVolume = Source.volume;
            FadeDeltaTime = 0;
            FadeOutSeconds = fadeSec;
            IsFadeStopping = true;
            IsFadePlaying = false;
        }

        /// <summary>
        /// �ꎞ��~���s���܂��B
        /// </summary>
        public void Pause()
        {
            Source.Pause();
        }
    }
}
