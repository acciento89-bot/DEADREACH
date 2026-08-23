using Kamilunavo.Deadreach.Core;
using Kamilunavo.Deadreach.Graphics;
using Kamilunavo.Deadreach.Persistence;
using Kamilunavo.Deadreach.Weapons;
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
            var width = Mathf.Min(620f, safe.width - 40f);
            var height = Mathf.Min(560f, safe.height - 30f);
            var x = safe.center.x - width * 0.5f;
            var y = safe.center.y - height * 0.5f;
            var data = SaveService.Data;
            var stashCount = data.stashWeapons?.Count ?? 0;
            var equipped = SaveService.GetEquippedPrimaryWeapon();

            GUI.Box(new Rect(x, y, width, height), GUIContent.none);
            GUI.Label(new Rect(x + 24f, y + 16f, width - 48f, 42f), "DEADREACH // BUNKER", _titleStyle);
            GUI.Label(new Rect(x + 24f, y + 62f, width - 48f, 27f), $"SECURED SCRAP  {data.securedScrap}", _labelStyle);
            GUI.Label(new Rect(x + 24f, y + 90f, width - 48f, 27f), $"EXTRACTIONS  {data.successfulExtractions}", _labelStyle);
            GUI.Label(new Rect(x + 24f, y + 118f, width - 48f, 27f), $"STREAK  {data.currentExtractionStreak}   //   BEST  {data.bestExtractionStreak}", _labelStyle);
            GUI.Label(new Rect(x + 24f, y + 146f, width - 48f, 27f), $"WEAPON STASH  {stashCount}", _labelStyle);

            DrawEquippedWeapon(equipped, x + 24f, y + 180f, width - 48f);
            DrawRecentWeapons(data, x + 24f, y + 272f, width - 48f);

            if (stashCount > 0 && GUI.Button(new Rect(x + 24f, y + height - 178f, width - 48f, 42f), "EQUIP NEXT STASH WEAPON", _buttonStyle))
                EquipNextWeapon(data, equipped);

            if (GUI.Button(new Rect(x + 24f, y + height - 126f, width - 48f, 62f), "DEPLOY // DEAD CITY", _buttonStyle))
                SceneFlowService.LoadExpedition();

            if (GUI.Button(new Rect(x + 24f, y + height - 52f, width - 48f, 36f), $"GRAPHICS // {MobileQualityService.Current.ToString().ToUpperInvariant()}", _buttonStyle))
                CycleGraphicsPreset();
        }

        private void DrawEquippedWeapon(WeaponInstanceData weapon, float x, float y, float width)
        {
            GUI.Label(new Rect(x, y, width, 22f), "EQUIPPED PRIMARY", _smallStyle);

            if (weapon == null)
            {
                GUI.Label(new Rect(x, y + 24f, width, 24f), "FIELD DR-7 // no extracted weapon equipped yet", _smallStyle);
                return;
            }

            GUI.Label(
                new Rect(x, y + 24f, width, 24f),
                $"{weapon.rarity.ToString().ToUpperInvariant()}  //  {weapon.displayNameSnapshot}  //  POWER {weapon.itemPower}",
                _smallStyle);

            var affixText = BuildAffixSummary(weapon);
            GUI.Label(new Rect(x, y + 48f, width, 40f), affixText, _smallStyle);
        }

        private void DrawRecentWeapons(DeadreachProfileData data, float x, float y, float width)
        {
            if (data.stashWeapons == null || data.stashWeapons.Count == 0)
            {
                GUI.Label(new Rect(x, y, width, 54f), "STASH EMPTY // Extract weapon cases from Dead City.", _smallStyle);
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

                var equippedMarker = weapon.instanceId == data.equippedPrimaryWeaponId ? "  [EQUIPPED]" : string.Empty;
                GUI.Label(
                    new Rect(x, y + 24f + row * 22f, width, 22f),
                    $"{weapon.rarity.ToString().ToUpperInvariant()}  //  {weapon.displayNameSnapshot}  //  PWR {weapon.itemPower}{equippedMarker}",
                    _smallStyle);
                row++;
            }
        }

        private static string BuildAffixSummary(WeaponInstanceData weapon)
        {
            if (weapon.affixes == null || weapon.affixes.Count == 0)
                return "AFFIXES // none";

            var summary = "AFFIXES // ";
            var shown = Mathf.Min(3, weapon.affixes.Count);
            for (var i = 0; i < shown; i++)
            {
                var affix = weapon.affixes[i];
                if (affix == null)
                    continue;

                if (i > 0)
                    summary += "   |   ";

                summary += $"{ShortStatName(affix.stat)} +{affix.value:0.#}%";
            }

            if (weapon.affixes.Count > shown)
                summary += $"   +{weapon.affixes.Count - shown} more";

            return summary;
        }

        private static string ShortStatName(WeaponAffixStat stat)
        {
            return stat switch
            {
                WeaponAffixStat.DamagePercent => "DMG",
                WeaponAffixStat.FireRatePercent => "ROF",
                WeaponAffixStat.RangePercent => "RNG",
                WeaponAffixStat.CritChancePercent => "CRIT",
                WeaponAffixStat.CritDamagePercent => "CRIT DMG",
                _ => stat.ToString().ToUpperInvariant()
            };
        }

        private static void EquipNextWeapon(DeadreachProfileData data, WeaponInstanceData current)
        {
            if (data.stashWeapons == null || data.stashWeapons.Count == 0)
                return;

            var currentIndex = -1;
            if (current != null)
                currentIndex = data.stashWeapons.FindIndex(item => item != null && item.instanceId == current.instanceId);

            for (var step = 1; step <= data.stashWeapons.Count; step++)
            {
                var index = (currentIndex + step + data.stashWeapons.Count) % data.stashWeapons.Count;
                var candidate = data.stashWeapons[index];
                if (candidate != null && SaveService.EquipPrimaryWeapon(candidate.instanceId))
                    return;
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
                alignment = TextAnchor.MiddleLeft,
                wordWrap = true
            };

            _buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 17,
                fontStyle = FontStyle.Bold
            };
        }
    }
}
