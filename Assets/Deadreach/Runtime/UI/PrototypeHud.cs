using Kamilunavo.Deadreach.Combat;
using Kamilunavo.Deadreach.Core;
using Kamilunavo.Deadreach.Persistence;
using Kamilunavo.Deadreach.Player;
using UnityEngine;

namespace Kamilunavo.Deadreach.UI
{
    public sealed class PrototypeHud : MonoBehaviour
    {
        private Damageable _playerHealth;
        private GUIStyle _titleStyle;
        private GUIStyle _textStyle;
        private GUIStyle _centerStyle;

        private void Start()
        {
            var player = FindFirstObjectByType<PlayerMotor>();
            if (player != null)
                _playerHealth = player.GetComponent<Damageable>();
        }

        private void OnGUI()
        {
            EnsureStyles();

            var safe = Screen.safeArea;
            var left = safe.x + 22f;
            var top = Screen.height - safe.yMax + 18f;
            var width = Mathf.Min(430f, safe.width * 0.46f);
            var session = RunSession.Current;
            var profile = SaveService.Data;

            GUI.Box(new Rect(left, top, width, 154f), GUIContent.none);
            GUI.Label(new Rect(left + 16f, top + 10f, width - 32f, 30f), "DEADREACH // FIELD TEST", _titleStyle);

            var hp = _playerHealth != null ? Mathf.CeilToInt(_playerHealth.CurrentHealth) : 0;
            var maxHp = _playerHealth != null ? Mathf.CeilToInt(_playerHealth.MaxHealth) : 0;
            GUI.Label(new Rect(left + 16f, top + 46f, width - 32f, 24f), $"HP  {hp}/{maxHp}", _textStyle);
            GUI.Label(new Rect(left + 16f, top + 72f, width - 32f, 24f), $"CARRIED  {session?.CarriedScrap ?? 0} SCRAP", _textStyle);
            GUI.Label(new Rect(left + 16f, top + 98f, width - 32f, 24f), $"SECURED  {profile.securedScrap}   STREAK  {profile.currentExtractionStreak}", _textStyle);

            var progress = session?.ExtractionProgress ?? 0f;
            var barRect = new Rect(left + 16f, top + 128f, width - 32f, 10f);
            GUI.Box(barRect, GUIContent.none);
            if (progress > 0f)
                GUI.Box(new Rect(barRect.x, barRect.y, barRect.width * progress, barRect.height), GUIContent.none);

            GUI.Label(new Rect(left, safe.yMax - 42f, width + 200f, 28f), "WASD / LEFT THUMB = MOVE   •   MOUSE / RIGHT THUMB = AIM + FIRE", _textStyle);

            if (session == null)
                return;

            if (session.IsCompleted)
            {
                GUI.Box(new Rect(safe.center.x - 220f, safe.center.y - 65f, 440f, 130f), GUIContent.none);
                GUI.Label(new Rect(safe.center.x - 200f, safe.center.y - 34f, 400f, 40f), "EXTRACTION SECURED", _centerStyle);
                GUI.Label(new Rect(safe.center.x - 200f, safe.center.y + 8f, 400f, 30f), "Loot banked. Run complete.", _centerStyle);
            }
            else if (session.IsFailed)
            {
                GUI.Box(new Rect(safe.center.x - 220f, safe.center.y - 65f, 440f, 130f), GUIContent.none);
                GUI.Label(new Rect(safe.center.x - 200f, safe.center.y - 34f, 400f, 40f), "RUN LOST", _centerStyle);
                GUI.Label(new Rect(safe.center.x - 200f, safe.center.y + 8f, 400f, 30f), "Unsecured loot was lost.", _centerStyle);
            }
        }

        private void EnsureStyles()
        {
            if (_textStyle != null)
                return;

            _titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 18,
                fontStyle = FontStyle.Bold
            };

            _textStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 14
            };

            _centerStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 20,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
        }
    }
}
