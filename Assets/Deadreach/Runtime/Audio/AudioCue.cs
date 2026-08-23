using UnityEngine;

namespace Kamilunavo.Deadreach.Audio
{
    [CreateAssetMenu(menuName = "DEADREACH/Audio/Audio Cue", fileName = "AudioCue_")]
    public sealed class AudioCue : ScriptableObject
    {
        [SerializeField] private AudioClip[] clips;
        [SerializeField, Range(0f, 1f)] private float volume = 1f;
        [SerializeField, Range(0.5f, 1.5f)] private float pitchMin = 0.96f;
        [SerializeField, Range(0.5f, 1.5f)] private float pitchMax = 1.04f;
        [SerializeField, Range(0f, 1f)] private float spatialBlend = 1f;
        [SerializeField, Min(1f)] private float maxDistance = 30f;

        public AudioClip GetRandomClip()
        {
            if (clips == null || clips.Length == 0)
                return null;

            return clips[Random.Range(0, clips.Length)];
        }

        public float Volume => volume;
        public float RandomPitch => Random.Range(Mathf.Min(pitchMin, pitchMax), Mathf.Max(pitchMin, pitchMax));
        public float SpatialBlend => spatialBlend;
        public float MaxDistance => maxDistance;
    }
}
