using UnityEngine;

namespace Kamilunavo.Deadreach.Feedback
{
    [DisallowMultipleComponent]
    public sealed class RuntimeImpactRing : MonoBehaviour
    {
        private LineRenderer _line;
        private Vector3 _origin;
        private float _startRadius;
        private float _endRadius;
        private float _duration;
        private float _elapsed;
        private float _startWidth;
        private Color _color;
        private int _segments;

        public void Initialize(Material material, Vector3 origin, float startRadius, float endRadius, float duration, Color color, float width, int segments = 48)
        {
            _origin = origin;
            _startRadius = Mathf.Max(0.01f, startRadius);
            _endRadius = Mathf.Max(_startRadius, endRadius);
            _duration = Mathf.Max(0.05f, duration);
            _startWidth = Mathf.Max(0.015f, width);
            _color = color;
            _segments = Mathf.Clamp(segments, 20, 72);

            _line = gameObject.AddComponent<LineRenderer>();
            _line.useWorldSpace = true;
            _line.loop = true;
            _line.positionCount = _segments;
            _line.numCapVertices = 2;
            _line.numCornerVertices = 2;
            _line.alignment = LineAlignment.View;
            _line.textureMode = LineTextureMode.Stretch;
            if (material != null)
                _line.sharedMaterial = material;

            Render(0f);
        }

        private void Update()
        {
            _elapsed += Time.deltaTime;
            var t = Mathf.Clamp01(_elapsed / _duration);
            Render(t);

            if (t >= 1f)
                Destroy(gameObject);
        }

        private void Render(float t)
        {
            if (_line == null)
                return;

            var eased = 1f - Mathf.Pow(1f - t, 2.2f);
            var radius = Mathf.Lerp(_startRadius, _endRadius, eased);
            var alpha = Mathf.Pow(1f - t, 1.25f);
            var color = new Color(_color.r, _color.g, _color.b, _color.a * alpha);
            _line.startColor = color;
            _line.endColor = color;
            _line.startWidth = Mathf.Lerp(_startWidth, _startWidth * 0.18f, t);
            _line.endWidth = _line.startWidth;

            for (var i = 0; i < _segments; i++)
            {
                var angle = i / (float)_segments * Mathf.PI * 2f;
                _line.SetPosition(i, _origin + new Vector3(Mathf.Cos(angle) * radius, 0f, Mathf.Sin(angle) * radius));
            }
        }
    }
}
