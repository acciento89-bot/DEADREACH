using UnityEngine;

namespace Kamilunavo.Deadreach.Presentation
{
    [RequireComponent(typeof(Light))]
    public sealed class Production06LightFlicker : MonoBehaviour
    {
        private Light _light;
        private float _baseIntensity;
        private float _seed;

        private void Awake()
        {
            _light = GetComponent<Light>();
            _baseIntensity = _light.intensity;
            _seed = Random.Range(0f, 100f);
        }

        private void Update()
        {
            if (_light == null)
                return;

            var noise = Mathf.PerlinNoise(_seed, Time.unscaledTime * 5.5f);
            var pulse = Mathf.Sin(Time.unscaledTime * 15f + _seed) > 0.82f ? 0.35f : 1f;
            _light.intensity = _baseIntensity * Mathf.Lerp(0.35f, 1.15f, noise) * pulse;
        }
    }
}
