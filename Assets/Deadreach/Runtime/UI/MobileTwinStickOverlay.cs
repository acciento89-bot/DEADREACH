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

        [SerializeField, Range(0.2f, 1f)] private float idleOpacity = 0.34f;
        [SerializeField, Range(0.2f, 1f)] private float activeOpacity = 0.72f;
        [SerializeField, Min(70f)] private float idleRadius = 88f;

        private Texture2D _outerTexture;
        private Texture2D _innerTexture;
        private Texture2D _aimTexture;
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
            _aimTexture = CreateReticleTexture(96);
        }

        private void OnGUI()
        {
            if (!ShouldShow())
                return;

            var input = DeadreachInput.Current;
            if (input == null)
                return;

            EnsureStyle();
            var safe = Screen.safeArea;
            var baseline = Mathf.Max(92f, safe.yMin + 105f);
            var leftIdle = new Vector2(safe.xMin + 118f, baseline);
            var rightIdle = new Vector2(safe.xMax - 118f, baseline);

            if (input.HasMoveTouch)
            {
                DrawMoveStick(input.MoveTouchOrigin, input.MoveTouchPosition, input.VirtualStickRadius, activeOpacity);
            }
            else
            {
                DrawRing(leftIdle, idleRadius, idleOpacity);
                DrawHint(leftIdle, "MOVE", idleOpacity);
            }

            if (input.HasAimTouch)
            {
                DrawAim(input.AimScreenPosition, idleRadius * 0.72f, activeOpacity);
            }
            else
            {
                DrawAim(rightIdle, idleRadius * 0.8f, idleOpacity);
                DrawHint(rightIdle, "AIM / FIRE", idleOpacity);
            }
        }

        private bool ShouldShow()
        {
            if (SceneManager.GetActiveScene().name == SceneFlowService.BunkerSceneName)
                return false;

            return Application.isMobilePlatform || Touchscreen.current != null;
        }

        private void DrawMoveStick(Vector2 origin, Vector2 current, float radius, float opacity)
        {
            var delta = Vector2.ClampMagnitude(current - origin, radius);
            DrawRing(origin, radius, opacity);
            DrawDisc(origin + delta, radius * 0.42f, opacity);
        }

        private void DrawRing(Vector2 screenPosition, float radius, float opacity)
        {
            var old = GUI.color;
            GUI.color = new Color(0.46f, 0.91f, 1f, opacity);
            GUI.DrawTexture(ToGuiRect(screenPosition, radius * 2f), _outerTexture, ScaleMode.StretchToFill, true);
            GUI.color = old;
        }

        private void DrawDisc(Vector2 screenPosition, float radius, float opacity)
        {
            var old = GUI.color;
            GUI.color = new Color(0.78f, 0.97f, 1f, opacity);
            GUI.DrawTexture(ToGuiRect(screenPosition, radius * 2f), _innerTexture, ScaleMode.StretchToFill, true);
            GUI.color = old;
        }

        private void DrawAim(Vector2 screenPosition, float radius, float opacity)
        {
            var old = GUI.color;
            GUI.color = new Color(1f, 0.67f, 0.25f, opacity);
            GUI.DrawTexture(ToGuiRect(screenPosition, radius * 2f), _aimTexture, ScaleMode.StretchToFill, true);
            GUI.color = old;
        }

        private void DrawHint(Vector2 screenPosition, string text, float opacity)
        {
            var old = GUI.color;
            GUI.color = new Color(1f, 1f, 1f, opacity * 0.9f);
            var guiY = Screen.height - screenPosition.y;
            GUI.Label(new Rect(screenPosition.x - 80f, guiY + idleRadius + 4f, 160f, 24f), text, _labelStyle);
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
            if (_labelStyle != null)
                return;

            _labelStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 12,
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

        private static Texture2D CreateReticleTexture(int size)
        {
            var texture = new Texture2D(size, size, TextureFormat.RGBA32, false)
            {
                name = "Runtime_AimReticle",
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp
            };

            var pixels = new Color32[size * size];
            var center = (size - 1) * 0.5f;
            var max = size * 0.5f;

            for (var y = 0; y < size; y++)
            {
                for (var x = 0; x < size; x++)
                {
                    var dx = x - center;
                    var dy = y - center;
                    var distance = Mathf.Sqrt(dx * dx + dy * dy);
                    var ring = distance > max * 0.48f && distance < max * 0.58f;
                    var horizontal = Mathf.Abs(dy) < 1.5f && Mathf.Abs(dx) > max * 0.16f && Mathf.Abs(dx) < max * 0.78f;
                    var vertical = Mathf.Abs(dx) < 1.5f && Mathf.Abs(dy) > max * 0.16f && Mathf.Abs(dy) < max * 0.78f;
                    var visible = ring || horizontal || vertical;
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
            if (_aimTexture != null)
                Destroy(_aimTexture);

            if (_instance == this)
                _instance = null;
        }
    }
}
