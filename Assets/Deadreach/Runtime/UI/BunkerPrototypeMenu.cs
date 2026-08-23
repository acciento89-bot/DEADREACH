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
        private GUIStyle _buttonStyle;

        private void OnGUI()
        {
            EnsureStyles();
            var safe = Screen.safeArea;
            var width = Mathf.Min(520f, safe.width - 40f);
            var height = 330f;
            var x = safe.center.x - width * 0.5f;
            var y = safe.center.y - height * 0.5f;
            var data = SaveService.Data;

            GUI.Box(new Rect(x, y, width, height), GUIContent.none);
            GUI.Label(new Rect(x + 24f, y + 22f, width - 48f, 42f), "DEADREACH // BUNKER", _titleStyle);
            GUI.Label(new Rect(x + 24f, y + 78f, width - 48f, 28f), $"SECURED SCRAP  {data.securedScrap}", _labelStyle);
            GUI.Label(new Rect(x + 24f, y + 110f, width - 48f, 28f), $"EXTRACTIONS  {data.successfulExtractions}", _labelStyle);
            GUI.Label(new Rect(x + 24f, y + 142f, width - 48f, 28f), $"STREAK  {data.currentExtractionStreak}   //   BEST  {data.bestExtractionStreak}", _labelStyle);

            if (GUI.Button(new Rect(x + 24f, y + 190f, width - 48f, 62f), "DEPLOY // DEAD CITY", _buttonStyle))
                SceneFlowService.LoadExpedition();

            if (GUI.Button(new Rect(x + 24f, y + 266f, width - 48f, 42f), $"GRAPHICS // {MobileQualityService.Current.ToString().ToUpperInvariant()}", _buttonStyle))
                CycleGraphicsPreset();
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

            _buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 18,
                fontStyle = FontStyle.Bold
            };
        }
    }
}
