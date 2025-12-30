using UnityEngine;

namespace CardGame.Managers
{
    /// <summary>
    /// Manages all audio in the game.
    /// </summary>
    public class AudioManager : Utilities.Singleton<AudioManager>
    {
        [Header("Audio Sources")]
        [SerializeField] private AudioSource _musicSource;
        [SerializeField] private AudioSource _sfxSource;

        [Header("Volume Settings")]
        [SerializeField] private float _masterVolume = 1f;
        [SerializeField] private float _musicVolume = 0.7f;
        [SerializeField] private float _sfxVolume = 1f;

        public float MasterVolume => _masterVolume;
        public float MusicVolume => _musicVolume;
        public float SFXVolume => _sfxVolume;

        protected override void Awake()
        {
            base.Awake();
            InitializeAudioSources();
        }

        private void InitializeAudioSources()
        {
            if (_musicSource == null)
            {
                _musicSource = gameObject.AddComponent<AudioSource>();
                _musicSource.loop = true;
                _musicSource.playOnAwake = false;
            }

            if (_sfxSource == null)
            {
                _sfxSource = gameObject.AddComponent<AudioSource>();
                _sfxSource.playOnAwake = false;
            }

            UpdateVolumes();
        }

        /// <summary>
        /// Plays background music.
        /// </summary>
        public void PlayMusic(AudioClip clip, bool loop = true)
        {
            if (clip == null) return;

            _musicSource.clip = clip;
            _musicSource.loop = loop;
            _musicSource.Play();
        }

        /// <summary>
        /// Stops the currently playing music.
        /// </summary>
        public void StopMusic()
        {
            _musicSource.Stop();
        }

        /// <summary>
        /// Plays a sound effect.
        /// </summary>
        public void PlaySFX(AudioClip clip, float volumeScale = 1f)
        {
            if (clip == null) return;
            _sfxSource.PlayOneShot(clip, volumeScale);
        }

        /// <summary>
        /// Sets the master volume.
        /// </summary>
        public void SetMasterVolume(float volume)
        {
            _masterVolume = Mathf.Clamp01(volume);
            UpdateVolumes();
        }

        /// <summary>
        /// Sets the music volume.
        /// </summary>
        public void SetMusicVolume(float volume)
        {
            _musicVolume = Mathf.Clamp01(volume);
            UpdateVolumes();
        }

        /// <summary>
        /// Sets the SFX volume.
        /// </summary>
        public void SetSFXVolume(float volume)
        {
            _sfxVolume = Mathf.Clamp01(volume);
            UpdateVolumes();
        }

        private void UpdateVolumes()
        {
            if (_musicSource != null)
                _musicSource.volume = _masterVolume * _musicVolume;

            if (_sfxSource != null)
                _sfxSource.volume = _masterVolume * _sfxVolume;
        }
    }
}
