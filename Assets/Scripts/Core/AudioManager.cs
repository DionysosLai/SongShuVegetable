using System.Collections.Generic;
using UnityEngine;
using SongShuVegetable.Utils;

namespace SongShuVegetable.Core
{
    public class AudioManager : SingletonMonoBehaviour<AudioManager>
    {
        // ── Inspector 配置 ────────────────────────────────────
        [SerializeField] private AudioSource _bgmSource;
        [SerializeField] private AudioSource _sfxSource;
        [SerializeField] private int _sfxPoolSize = 8;

        [Range(0f, 1f)] [SerializeField] private float _bgmVolume = 0.6f;
        [Range(0f, 1f)] [SerializeField] private float _sfxVolume = 1f;

        // ── 属性 ─────────────────────────────────────────────
        public float BGMVolume
        {
            get => _bgmVolume;
            set { _bgmVolume = Mathf.Clamp01(value); _bgmSource.volume = _bgmVolume; }
        }

        public float SFXVolume
        {
            get => _sfxVolume;
            set { _sfxVolume = Mathf.Clamp01(value); }
        }

        // 多 SFX 同时播放的音源池
        private readonly List<AudioSource> _sfxPool = new();
        private int _sfxPoolIndex = 0;

        // ── 生命周期 ──────────────────────────────────────────
        protected override void Awake()
        {
            base.Awake();
            SetupAudioSources();
            BuildSFXPool();
        }

        // ── BGM 接口 ──────────────────────────────────────────
        public void PlayBGM(AudioClip clip, bool loop = true)
        {
            if (clip == null) return;
            if (_bgmSource.clip == clip && _bgmSource.isPlaying) return;

            _bgmSource.clip = clip;
            _bgmSource.loop = loop;
            _bgmSource.volume = _bgmVolume;
            _bgmSource.Play();
            Log.Info("AudioManager", $"BGM 播放：{clip.name}");
        }

        public void StopBGM()
        {
            _bgmSource.Stop();
        }

        public void PauseBGM()  => _bgmSource.Pause();
        public void ResumeBGM() => _bgmSource.UnPause();

        // ── SFX 接口 ──────────────────────────────────────────

        /// <summary>在世界坐标播放一次性音效。</summary>
        public void PlaySFX(AudioClip clip, Vector3 position = default, float volumeScale = 1f)
        {
            if (clip == null) return;
            AudioSource source = GetNextSFXSource();
            source.transform.position = position;
            source.volume = _sfxVolume * volumeScale;
            source.PlayOneShot(clip);
        }

        /// <summary>播放全局 UI 音效（不受位置影响）。</summary>
        public void PlayUI(AudioClip clip, float volumeScale = 1f)
        {
            if (clip == null) return;
            _sfxSource.PlayOneShot(clip, _sfxVolume * volumeScale);
        }

        // ── 内部工具 ──────────────────────────────────────────
        private void SetupAudioSources()
        {
            if (_bgmSource == null)
            {
                _bgmSource = gameObject.AddComponent<AudioSource>();
                _bgmSource.playOnAwake = false;
                _bgmSource.loop = true;
            }
            if (_sfxSource == null)
            {
                _sfxSource = gameObject.AddComponent<AudioSource>();
                _sfxSource.playOnAwake = false;
                _sfxSource.spatialBlend = 0f;
            }
            _bgmSource.volume = _bgmVolume;
        }

        private void BuildSFXPool()
        {
            for (int i = 0; i < _sfxPoolSize; i++)
            {
                var go = new GameObject($"SFXSource_{i}");
                go.transform.SetParent(transform);
                var src = go.AddComponent<AudioSource>();
                src.playOnAwake = false;
                src.spatialBlend = 1f;  // 3D 空间音效
                _sfxPool.Add(src);
            }
        }

        private AudioSource GetNextSFXSource()
        {
            var source = _sfxPool[_sfxPoolIndex];
            _sfxPoolIndex = (_sfxPoolIndex + 1) % _sfxPool.Count;
            return source;
        }
    }
}
