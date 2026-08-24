using System.Collections.Generic;
using UnityEngine;

namespace Kamilunavo.Deadreach.Presentation
{
    [DisallowMultipleComponent]
    public sealed class Production13BunkerAtmosphere : MonoBehaviour
    {
        private readonly Dictionary<Light, float> _baseIntensity = new();
        private Transform _holoRotor;
        private Light[] _lights;

        private void Start()
        {
            _holoRotor = FindDeepChild(transform, "P13_HoloRotor");
            _lights = GetComponentsInChildren<Light>(true);
            foreach (var light in _lights)
            {
                if (light != null)
                    _baseIntensity[light] = light.intensity;
            }
        }

        private void Update()
        {
            if (_holoRotor != null)
                _holoRotor.Rotate(0f, 16f * Time.unscaledDeltaTime, 0f, Space.Self);

            if (_lights == null)
                return;

            var time = Time.unscaledTime;
            foreach (var light in _lights)
            {
                if (light == null || !_baseIntensity.TryGetValue(light, out var baseline))
                    continue;

                if (light.name.Contains("Emergency"))
                {
                    var pulse = 0.72f + 0.28f * Mathf.Sin(time * 3.8f + light.transform.position.x * 0.2f);
                    light.intensity = baseline * Mathf.Clamp01(pulse);
                }
                else if (light.name.Contains("Holo"))
                {
                    var pulse = 0.88f + 0.12f * Mathf.Sin(time * 2.1f);
                    light.intensity = baseline * pulse;
                }
                else if (light.name.Contains("Monitor"))
                {
                    var noise = Mathf.PerlinNoise(time * 0.42f, light.transform.position.z * 0.11f);
                    light.intensity = baseline * Mathf.Lerp(0.88f, 1.05f, noise);
                }
            }
        }

        private static Transform FindDeepChild(Transform parent, string name)
        {
            foreach (var child in parent.GetComponentsInChildren<Transform>(true))
            {
                if (child != null && child.name == name)
                    return child;
            }
            return null;
        }
    }
}
