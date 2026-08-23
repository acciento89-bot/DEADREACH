using UnityEngine;

namespace Kamilunavo.Deadreach.Feedback
{
    [DisallowMultipleComponent]
    public sealed class RuntimeImpactLine : MonoBehaviour
    {
        private LineRenderer _line;
        private float _duration;
        private float _elapsed;
        private float _startWidth;
        private Color _color;

        public void Initialize(Material material, Vector3 start, Vector3 end, float duration, Color color, float width)
        {
            _duration = Mathf.Max(0.05f, duration);
            _startWidth = Mathf.Max(0.015f, width);
            _color = color;

            _line = gameObject.AddComponent<LineRenderer>();
            _line.useWorldSpace = true;
            _line.positionCount = 2;
            _line.numCapVertices = 4;
            _line.numCornerVertices = 2;
            _line.alignment = LineAlignment.View;
            _line.textureMode = LineTextureMode.Stretch;
            if (material != null)
                _line.sharedMaterial = material;

            _line.SetPosition(0, start);
            _line.SetPosition(1, end);
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

            var alpha = Mathf.Pow(1f - t, 1.4f);
            var start = new Color(_color.r, _color.g, _color.b, _color.a * alpha);
            var end = new Color(_color.r, _color.g, _color.b, _color.a * alpha * 0.18f);
            _line.startColor = start;
            _line.endColor = end;
            _line.startWidth = Mathf.Lerp(_startWidth, _startWidth * 0.12f, t);
            _line.endWidth = _line.startWidth * 0.35f;
        }
    }
}
