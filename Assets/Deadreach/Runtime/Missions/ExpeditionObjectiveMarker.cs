using UnityEngine;

namespace Kamilunavo.Deadreach.Missions
{
    [DisallowMultipleComponent]
    public sealed class ExpeditionObjectiveMarker : MonoBehaviour
    {
        private LineRenderer _ring;
        private LineRenderer _beam;
        private Light _light;
        private Transform _core;
        private Material _material;
        private Color _color;
        private float _radius;
        private bool _completed;

        public Vector3 WorldPosition => transform.position;

        public void Initialize(Vector3 position, Color color, float radius = 1.85f)
        {
            transform.position = position;
            _radius = Mathf.Max(0.8f, radius);
            _color = color;
            BuildVisuals();
            ApplyColor(color);
        }

        public void SetWorldPosition(Vector3 position)
        {
            transform.position = position;
        }

        public void SetTheme(Color color, float radius)
        {
            _color = color;
            _radius = Mathf.Max(0.8f, radius);
            RebuildRing();
            ApplyColor(color);
        }

        public void SetCompleted(bool completed)
        {
            _completed = completed;
            ApplyColor(completed ? new Color(0.18f, 1f, 0.45f, 0.95f) : _color);
        }

        private void BuildVisuals()
        {
            if (_ring != null)
                return;

            var shader = Shader.Find("Sprites/Default") ?? Shader.Find("Universal Render Pipeline/Unlit");
            if (shader != null)
                _material = new Material(shader) { name = "Runtime_011_MissionMarker" };

            var ringObject = new GameObject("Objective_Ring");
            ringObject.transform.SetParent(transform, false);
            ringObject.transform.localPosition = Vector3.up * 0.055f;
            _ring = ringObject.AddComponent<LineRenderer>();
            _ring.useWorldSpace = false;
            _ring.loop = true;
            _ring.positionCount = 48;
            _ring.numCornerVertices = 2;
            _ring.numCapVertices = 2;
            _ring.alignment = LineAlignment.TransformZ;
            _ring.sharedMaterial = _material;
            RebuildRing();

            var beamObject = new GameObject("Objective_Beam");
            beamObject.transform.SetParent(transform, false);
            _beam = beamObject.AddComponent<LineRenderer>();
            _beam.useWorldSpace = false;
            _beam.positionCount = 2;
            _beam.SetPosition(0, Vector3.up * 0.12f);
            _beam.SetPosition(1, Vector3.up * 3.4f);
            _beam.startWidth = 0.055f;
            _beam.endWidth = 0.012f;
            _beam.sharedMaterial = _material;

            var coreObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            coreObject.name = "Objective_Core";
            coreObject.transform.SetParent(transform, false);
            coreObject.transform.localPosition = Vector3.up * 0.58f;
            coreObject.transform.localScale = new Vector3(0.34f, 0.34f, 0.34f);
            var collider = coreObject.GetComponent<Collider>();
            if (collider != null)
                collider.enabled = false;
            var renderer = coreObject.GetComponent<Renderer>();
            if (renderer != null && _material != null)
                renderer.sharedMaterial = _material;
            _core = coreObject.transform;

            var lightObject = new GameObject("Objective_Light");
            lightObject.transform.SetParent(transform, false);
            lightObject.transform.localPosition = Vector3.up * 1.15f;
            _light = lightObject.AddComponent<Light>();
            _light.type = LightType.Point;
            _light.range = 6.5f;
            _light.shadows = LightShadows.None;
        }

        private void RebuildRing()
        {
            if (_ring == null)
                return;

            for (var i = 0; i < _ring.positionCount; i++)
            {
                var angle = i / (float)_ring.positionCount * Mathf.PI * 2f;
                _ring.SetPosition(i, new Vector3(Mathf.Cos(angle) * _radius, 0f, Mathf.Sin(angle) * _radius));
            }

            _ring.startWidth = 0.085f;
            _ring.endWidth = 0.085f;
        }

        private void ApplyColor(Color color)
        {
            if (_material != null)
            {
                if (_material.HasProperty("_BaseColor")) _material.SetColor("_BaseColor", color);
                if (_material.HasProperty("_Color")) _material.SetColor("_Color", color);
            }

            if (_ring != null)
            {
                _ring.startColor = color;
                _ring.endColor = new Color(color.r, color.g, color.b, 0.42f);
            }

            if (_beam != null)
            {
                _beam.startColor = new Color(color.r, color.g, color.b, 0.86f);
                _beam.endColor = new Color(color.r, color.g, color.b, 0.02f);
            }

            if (_light != null)
                _light.color = color;
        }

        private void Update()
        {
            var pulse = 0.5f + 0.5f * Mathf.Sin(Time.unscaledTime * (_completed ? 2.2f : 4.4f));
            if (_ring != null)
            {
                var width = Mathf.Lerp(_completed ? 0.055f : 0.07f, _completed ? 0.075f : 0.12f, pulse);
                _ring.startWidth = width;
                _ring.endWidth = width;
            }

            if (_core != null)
            {
                _core.Rotate(Vector3.up, (_completed ? 35f : 70f) * Time.unscaledDeltaTime, Space.World);
                var scale = Mathf.Lerp(0.88f, 1.12f, pulse);
                _core.localScale = Vector3.one * 0.34f * scale;
            }

            if (_light != null)
                _light.intensity = Mathf.Lerp(_completed ? 1.2f : 2.1f, _completed ? 2.1f : 4.4f, pulse);
        }

        private void OnDestroy()
        {
            if (_material != null)
                Destroy(_material);
        }
    }
}
