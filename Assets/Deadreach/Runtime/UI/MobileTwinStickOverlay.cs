using Kamilunavo.Deadreach.Core;
using Kamilunavo.Deadreach.Input;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Kamilunavo.Deadreach.UI
{
    public sealed class MobileTwinStickOverlay : MonoBehaviour
    {
        private static MobileTwinStickOverlay _instance;

        [SerializeField, Range(0.2f, 1f)] private float idleOpacity = 0.38f;
        [SerializeField, Range(0.2f, 1f)] private float activeOpacity = 0.82f;

        private Texture2D _outerTexture;
        private Texture2D _innerTexture;
        private GUIStyle _labelStyle;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void EnsureInstance()
        {
            if (_instance != null)
                return;

            var root = new GameObject("UI_MobileTwinStick");
            _instance = root.AddComponent<MobileTwinStickOverlay>();
            DontDestroyOnLoad(root);
        }

        private void Awake()
        {
            _outerTexture = CreateCircleTexture(128, 0.72f, 0.12f);
            _innerTexture = CreateCircleTexture(96, 0.95f, 0f);
        }

        private void OnGUI()
        {
            if (!ShouldShow())
                return;

            var input = DeadreachInput.Current;
            if (input == null)
                return;

            EnsureStyle();
            var radius = input.VirtualStickRadius;
            var moveCenter = input.MoveStickCenter;
            var aimCenter = input.AimStickCenter;

            DrawStick(
                moveCenter,
                input.HasMoveTouch ? input.MoveTouchPosition : moveCenter,
                radius,
                new Color(0.35f, 0.9f, 1f),
                "MOVE",
                input.HasMoveTouch ? activeOpacity : idleOpacity);

            DrawStick(
                aimCenter,
                input.HasAimTouch ? input.AimTouchPosition : aimCenter,
                radius,
                new Color(1f, 0.62f, 0.18f),
                "AIM / FIRE",
                input.HasAimTouch ? activeOpacity : idleOpacity);
        }

        private bool ShouldShow()
        {
            if (SceneManager.GetActiveScene().name == SceneFlowService.BunkerSceneName)
                return false;

            return Application.isMobilePlatform || Touchscreen.current != null;
        }

        private void DrawStick(Vector2 origin, Vector2 current, float radius, Color tint, string label, float opacity)
        {
            var delta = Vector2.ClampMagnitude(current - origin, radius);
            var old = GUI.color;

            GUI.color = new Color(tint.r, tint.g, tint.b, opacity * 0.74f);
            GUI.DrawTexture(ToGuiRect(origin, radius * 2f), _outerTexture, ScaleMode.StretchToFill, true);

            GUI.color = new Color(tint.r, tint.g, tint.b, opacity);
            GUI.DrawTexture(ToGuiRect(origin + delta, radius * 0.72f), _innerTexture, ScaleMode.StretchToFill, true);
            GUI.color = old;

            DrawHint(origin, radius, label, opacity);
        }

        private void DrawHint(Vector2 origin, float radius, string text, float opacity)
        {
            var old = GUI.color;
            GUI.color = new Color(1f, 1f, 1f, opacity * 0.92f);
            var guiY = Screen.height - origin.y;
            GUI.Label(new Rect(origin.x - radius, guiY + radius + 8f, radius * 2f, 30f), text, _labelStyle);
            GUI.color = old;
        }

        private static Rect ToGuiRect(Vector2 screenPosition, float size)
        {
            return new Rect(
                screenPosition.x - size * 0.5f,
                Screen.height - screenPosition.y - size * 0.5f,
                size,
                size);
        }

        private void EnsureStyle()
        {
            var targetSize = Mathf.Clamp(Mathf.RoundToInt(Screen.safeArea.height * 0.023f), 14, 22);
            if (_labelStyle != null && _labelStyle.fontSize == targetSize)
                return;

            _labelStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = targetSize,
                fontStyle = FontStyle.Bold
            };
        }

        private static Texture2D CreateCircleTexture(int size, float fillRadius, float innerCutout)
        {
            var texture = new Texture2D(size, size, TextureFormat.RGBA32, false)
            {
                name = "Runtime_TouchCircle",
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp
            };

            var pixels = new Color32[size * size];
            var center = (size - 1) * 0.5f;
            var max = size * 0.5f;
            var outer = max * fillRadius;
            var inner = outer * innerCutout;

            for (var y = 0; y < size; y++)
            {
                for (var x = 0; x < size; x++)
                {
                    var dx = x - center;
                    var dy = y - center;
                    var distance = Mathf.Sqrt(dx * dx + dy * dy);
                    var visible = distance <= outer && (innerCutout <= 0f || distance >= inner);
                    pixels[y * size + x] = visible ? new Color32(255, 255, 255, 255) : new Color32(255, 255, 255, 0);
                }
            }

            texture.SetPixels32(pixels);
            texture.Apply(false, true);
            return texture;
        }

        private void OnDestroy()
        {
            if (_outerTexture != null)
                Destroy(_outerTexture);
            if (_innerTexture != null)
                Destroy(_innerTexture);

            if (_instance == this)
                _instance = null;
        }
    }
}
