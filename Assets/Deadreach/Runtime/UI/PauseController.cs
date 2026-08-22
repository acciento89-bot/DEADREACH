using Kamilunavo.Deadreach.Core;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Kamilunavo.Deadreach.UI
{
    public sealed class PauseController : MonoBehaviour
    {
        private static PauseController _instance;
        private bool _paused;
        private GUIStyle _buttonStyle;
        private GUIStyle _titleStyle;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void EnsureInstance()
        {
            if (_instance != null)
                return;

            var gameObject = new GameObject("Systems_Pause");
            _instance = gameObject.AddComponent<PauseController>();
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            var keyboardPressed = Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame;
            var gamepadPressed = Gamepad.current != null && Gamepad.current.startButton.wasPressedThisFrame;
            if (keyboardPressed || gamepadPressed)
                TogglePause();
        }

        private void OnGUI()
        {
            if (SceneManager.GetActiveScene().name == SceneFlowService.BunkerSceneName)
                return;

            EnsureStyles();
            var safe = Screen.safeArea;
            var buttonRect = new Rect(safe.xMax - 74f, Screen.height - safe.yMax + 18f, 56f, 48f);

            if (!_paused)
            {
                if (GUI.Button(buttonRect, "II", _buttonStyle))
                    SetPaused(true);
                return;
            }

            GUI.Box(new Rect(safe.x, Screen.height - safe.yMax, safe.width, safe.height), GUIContent.none);
            var width = Mathf.Min(430f, safe.width - 36f);
            var x = safe.center.x - width * 0.5f;
            var y = safe.center.y - 150f;
            GUI.Label(new Rect(x, y, width, 54f), "PAUSED", _titleStyle);

            if (GUI.Button(new Rect(x, y + 78f, width, 62f), "RESUME", _buttonStyle))
                SetPaused(false);

            if (GUI.Button(new Rect(x, y + 156f, width, 62f), "ABANDON RUN // BUNKER", _buttonStyle))
            {
                RunSession.Current?.AbandonRun();
                SetPaused(false);
                SceneFlowService.LoadBunker();
            }
        }

        private void TogglePause() => SetPaused(!_paused);

        private void SetPaused(bool paused)
        {
            _paused = paused;
            Time.timeScale = paused ? 0f : 1f;
            AudioListener.pause = paused;
        }

        private void OnDestroy()
        {
            if (_instance == this)
                _instance = null;

            if (_paused)
            {
                Time.timeScale = 1f;
                AudioListener.pause = false;
            }
        }

        private void EnsureStyles()
        {
            if (_buttonStyle != null)
                return;

            _buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 18,
                fontStyle = FontStyle.Bold
            };

            _titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 30,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
        }
    }
}
