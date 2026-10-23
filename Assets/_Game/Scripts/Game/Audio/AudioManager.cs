
using UnityEngine;

namespace VertigoGames.Audio
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Sound Effects")]
        [SerializeField] private AudioClip bombHitClip;
        [SerializeField] private AudioClip collectButtonClip;
        [SerializeField] private AudioClip wheelSpinClip;
        [SerializeField] private AudioClip buttonClickClip;

        [Header("Volume")]
        [SerializeField, Range(0f, 1f)]
        private float sfxVolume = 1f;

        [SerializeField, Range(0f, 1f)]
        private float wheelSpinVolume = 0.7f;

        private AudioSource sfxSource;
        private AudioSource wheelSpinSource;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.LogError("There is more than one AudioManager in the scene.");
                Destroy(gameObject);
                return;
            }

            Instance = this;

            AudioSource[] sources = GetComponents<AudioSource>();

            if (sources.Length > 0)
                sfxSource = sources[0];
            else
                sfxSource = gameObject.AddComponent<AudioSource>();

            sfxSource.playOnAwake = false;
            sfxSource.loop = false;
            sfxSource.volume = sfxVolume;

            if (sources.Length > 1)
                wheelSpinSource = sources[1];
            else
                wheelSpinSource = gameObject.AddComponent<AudioSource>();

            wheelSpinSource.playOnAwake = false;
            wheelSpinSource.loop = true;
            wheelSpinSource.volume = wheelSpinVolume;
        }

        public void PlayBombHit()
        {
            PlaySFX(bombHitClip);
        }

        public void PlayCollectButton()
        {
            PlaySFX(collectButtonClip);
        }

        public void PlayButtonClick()
        {
            PlaySFX(buttonClickClip);
        }

        public void StartWheelSpin()
        {
            if (wheelSpinClip == null || wheelSpinSource == null)
                return;

            wheelSpinSource.Stop();
            wheelSpinSource.clip = wheelSpinClip;
            wheelSpinSource.loop = false;
            wheelSpinSource.volume = wheelSpinVolume;
            wheelSpinSource.Play();
        }

        public void StopWheelSpin()
        {
            if (wheelSpinSource != null)
                wheelSpinSource.Stop();
        }

        private void PlaySFX(AudioClip clip)
        {
            if (clip == null || sfxSource == null)
                return;

            sfxSource.PlayOneShot(clip, sfxVolume);
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }
    }
}
