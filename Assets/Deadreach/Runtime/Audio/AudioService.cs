using UnityEngine;

namespace Kamilunavo.Deadreach.Audio
{
    public sealed class AudioService : MonoBehaviour
    {
        private const int PoolSize = 12;
        private static AudioService _instance;

        private AudioSource[] _sources;
        private int _cursor;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void EnsureInstance()
        {
            if (_instance != null)
                return;

            var root = new GameObject("Systems_Audio");
            _instance = root.AddComponent<AudioService>();
            DontDestroyOnLoad(root);
        }

        private void Awake()
        {
            _sources = new AudioSource[PoolSize];
            for (var i = 0; i < _sources.Length; i++)
            {
                var child = new GameObject($"AudioSource_{i:00}");
                child.transform.SetParent(transform, false);
                var source = child.AddComponent<AudioSource>();
                source.playOnAwake = false;
                source.loop = false;
                source.rolloffMode = AudioRolloffMode.Linear;
                _sources[i] = source;
            }
        }

        public static void Play(AudioCue cue, Vector3 worldPosition)
        {
            if (_instance == null || cue == null)
                return;

            _instance.PlayInternal(cue, worldPosition);
        }

        private void PlayInternal(AudioCue cue, Vector3 worldPosition)
        {
            var clip = cue.GetRandomClip();
            if (clip == null || _sources == null || _sources.Length == 0)
                return;

            var source = FindAvailableSource();
            source.transform.position = worldPosition;
            source.clip = clip;
            source.volume = cue.Volume;
            source.pitch = cue.RandomPitch;
            source.spatialBlend = cue.SpatialBlend;
            source.maxDistance = cue.MaxDistance;
            source.Play();
        }

        private AudioSource FindAvailableSource()
        {
            for (var i = 0; i < _sources.Length; i++)
            {
                var index = (_cursor + i) % _sources.Length;
                if (!_sources[index].isPlaying)
                {
                    _cursor = (index + 1) % _sources.Length;
                    return _sources[index];
                }
            }

            var fallback = _sources[_cursor];
            _cursor = (_cursor + 1) % _sources.Length;
            fallback.Stop();
            return fallback;
        }

        private void OnDestroy()
        {
            if (_instance == this)
                _instance = null;
        }
    }
}
