using UnityEngine;

namespace CleanWave.Audio
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private AudioSource bgmSource;
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioClip pickupClip;
        [SerializeField] private AudioClip correctClip;
        [SerializeField] private AudioClip wrongClip;
        [SerializeField] private AudioClip clearClip;

        public void PlayPickup() => PlaySfx(pickupClip);
        public void PlayCorrect() => PlaySfx(correctClip);
        public void PlayWrong() => PlaySfx(wrongClip);
        public void PlayClear() => PlaySfx(clearClip);

        public void SetBgmVolume(float value)
        {
            if (bgmSource != null)
            {
                bgmSource.volume = Mathf.Clamp01(value);
            }
        }

        public void SetSfxVolume(float value)
        {
            if (sfxSource != null)
            {
                sfxSource.volume = Mathf.Clamp01(value);
            }
        }

        private void PlaySfx(AudioClip clip)
        {
            if (clip != null && sfxSource != null)
            {
                sfxSource.PlayOneShot(clip);
            }
        }
    }
}
