using UnityEngine;

namespace Kounosuke
{
    public class AudioManager : MonoBehaviour
    {
        private static AudioManager instance;

        [Header("Audio Sources")]
        [SerializeField] private AudioSource bgmSource;
        [SerializeField] private AudioSource seSource;
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public static AudioManager Instance()
        {
            return instance;
        }
        public void PlayBGM(AudioClip clip, bool loop = true)
        {
            if (clip == null) return;

            bgmSource.clip = clip;
            bgmSource.loop = loop;
            bgmSource.Play();
        }

        public void StopBGM()
        {
            bgmSource.Stop();
        }

        public void PlaySE(AudioClip clip)
        {
            if (clip == null) return;

            seSource.PlayOneShot(clip);
        }

        public void SetBGMVolume(float volume)
        {
            bgmSource.volume = Mathf.Clamp01(volume);
        }

        public void SetSEVolume(float volume)
        {
            seSource.volume = Mathf.Clamp01(volume);
        }
    }
}