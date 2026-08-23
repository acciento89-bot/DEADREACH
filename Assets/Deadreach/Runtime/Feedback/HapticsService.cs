using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Kamilunavo.Deadreach.Feedback
{
    public sealed class HapticsService : MonoBehaviour
    {
        private static HapticsService _instance;
        private Coroutine _gamepadRoutine;
        private float _lastMobileVibrationTime = -10f;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void EnsureInstance()
        {
            if (_instance != null)
                return;

            var root = new GameObject("Systems_Haptics");
            _instance = root.AddComponent<HapticsService>();
            DontDestroyOnLoad(root);
        }

        private void OnEnable()
        {
            CombatFeedback.ShotFired += HandleShot;
            CombatFeedback.PlayerDamaged += HandlePlayerDamaged;
            CombatFeedback.PlayerDied += HandlePlayerDied;
        }

        private void OnDisable()
        {
            CombatFeedback.ShotFired -= HandleShot;
            CombatFeedback.PlayerDamaged -= HandlePlayerDamaged;
            CombatFeedback.PlayerDied -= HandlePlayerDied;
            StopGamepadRumble();
        }

        private void HandleShot(ShotFeedback feedback)
        {
            PulseGamepad(Mathf.Lerp(0.05f, 0.22f, feedback.HapticStrength), 0.035f);
        }

        private void HandlePlayerDamaged(float normalizedDamage)
        {
            PulseGamepad(Mathf.Lerp(0.25f, 0.7f, normalizedDamage), 0.09f);
            VibrateMobileThrottled(0.18f);
        }

        private void HandlePlayerDied()
        {
            PulseGamepad(0.9f, 0.22f);
            VibrateMobileThrottled(0f);
        }

        private void PulseGamepad(float strength, float duration)
        {
            if (Gamepad.current == null)
                return;

            if (_gamepadRoutine != null)
                StopCoroutine(_gamepadRoutine);

            _gamepadRoutine = StartCoroutine(GamepadPulse(Mathf.Clamp01(strength), duration));
        }

        private IEnumerator GamepadPulse(float strength, float duration)
        {
            var gamepad = Gamepad.current;
            if (gamepad == null)
                yield break;

            gamepad.SetMotorSpeeds(strength * 0.75f, strength);
            yield return new WaitForSecondsRealtime(Mathf.Max(0.02f, duration));

            if (gamepad == Gamepad.current)
                gamepad.SetMotorSpeeds(0f, 0f);

            _gamepadRoutine = null;
        }

        private void StopGamepadRumble()
        {
            if (_gamepadRoutine != null)
            {
                StopCoroutine(_gamepadRoutine);
                _gamepadRoutine = null;
            }

            Gamepad.current?.SetMotorSpeeds(0f, 0f);
        }

        private void VibrateMobileThrottled(float minimumInterval)
        {
#if UNITY_IOS || UNITY_ANDROID
            if (Time.unscaledTime - _lastMobileVibrationTime < minimumInterval)
                return;

            _lastMobileVibrationTime = Time.unscaledTime;
            Handheld.Vibrate();
#endif
        }

        private void OnDestroy()
        {
            StopGamepadRumble();
            if (_instance == this)
                _instance = null;
        }
    }
}
