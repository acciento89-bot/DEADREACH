using Kamilunavo.Deadreach.Core;
using Kamilunavo.Deadreach.Graphics;
using Kamilunavo.Deadreach.Persistence;
using UnityEngine;

namespace Kamilunavo.Deadreach.UI
{
    public sealed class BunkerPrototypeMenu : MonoBehaviour
    {
        private GUIStyle _titleStyle;
        private GUIStyle _labelStyle;
        private GUIStyle _smallStyle;
        private GUIStyle _buttonStyle;

        private void OnGUI()
        {
            EnsureStyles();
            var safe = Screen.safeArea;
            var width = Mathf.Min(560f, safe.width - 40f);
            var height = 430f;
            var x = safe.center.x - width * 0.5f;
            var y = safe.center.y - height * 0.5f;
            var data = SaveService.Data;
            var stashCount = data.stashWeapons?.Count ?? 0;

            GUI.Box(new Rect(x, y, width, height), GUIContent.none);
            GUI.Label(new Rect(x + 24f, y + 18f, width - 48f, 42f), "DEADREACH // BUNKER", _titleStyle);
            GUI.Label(new Rect(x + 24f, y + 68f, width - 48f, 28f), $"SECURED SCRAP  {data.securedScrap}", _labelStyle);
            GUI.Label(new Rect(x + 24f, y + 98f, width - 48f, 28f), $"EXTRACTIONS  {data.successfulExtractions}", _labelStyle);
            GUI.Label(new Rect(x + 24f, y + 128f, width - 48f, 28f), $"STREAK  {data.currentExtractionStreak}   //   BEST  {data.bestExtractionStreak}", _labelStyle);
            GUI.Label(new Rect(x + 24f, y + 158f, width - 48f, 28f), $"WEAPON STASH  {stashCount}", _labelStyle);

            DrawRecentWeapons(data, x + 24f, y + 190f, width - 48f);

            if (GUI.Button(new Rect(x + 24f, y + 300f, width - 48f, 62f), "DEPLOY // DEAD CITY", _buttonStyle))
                SceneFlowService.LoadExpedition();

            if (GUI.Button(new Rect(x + 24f, y + 376f, width - 48f, 38f), $"GRAPHICS // {MobileQualityService.Current.ToString().ToUpperInvariant()}", _buttonStyle))
                CycleGraphicsPreset();
        }

        private void DrawRecentWeapons(DeadreachProfileData data, float x, float y, float width)
        {
            if (data.stashWeapons == null || data.stashWeapons.Count == 0)
            {
                GUI.Label(new Rect(x, y, width, 70f), "STASH EMPTY // Extract weapon cases from Dead City.", _smallStyle);
                return;
            }

            GUI.Label(new Rect(x, y, width, 22f), "RECENT EXTRACTIONS", _smallStyle);
            var start = Mathf.Max(0, data.stashWeapons.Count - 3);
            var row = 0;
            for (var i = data.stashWeapons.Count - 1; i >= start; i--)
            {
                var weapon = data.stashWeapons[i];
                if (weapon == null)
                    continue;

                GUI.Label(
                    new Rect(x, y + 24f + row * 22f, width, 22f),
                    $"{weapon.rarity.ToString().ToUpperInvariant()}  //  {weapon.displayNameSnapshot}  //  POWER {weapon.itemPower}",
                    _smallStyle);
                row++;
            }
        }

        private static void CycleGraphicsPreset()
        {
            var next = MobileQualityService.Current switch
            {
                GraphicsPreset.Performance => GraphicsPreset.Balanced,
                GraphicsPreset.Balanced => GraphicsPreset.Ultra,
                _ => GraphicsPreset.Performance
            };

            MobileQualityService.Apply(next);
        }

        private void EnsureStyles()
        {
            if (_labelStyle != null)
                return;

            _titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 26,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };

            _labelStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 17,
                alignment = TextAnchor.MiddleLeft
            };

            _smallStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 13,
                alignment = TextAnchor.MiddleLeft
            };

            _buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 18,
                fontStyle = FontStyle.Bold
            };
        }
    }
}
