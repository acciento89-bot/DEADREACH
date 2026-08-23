using System;
using System.Collections.Generic;
using UnityEngine;

namespace Kamilunavo.Deadreach.Weapons
{
    public static class WeaponLootFactory
    {
        private static readonly WeaponAffixStat[] AffixPool =
        {
            WeaponAffixStat.DamagePercent,
            WeaponAffixStat.FireRatePercent,
            WeaponAffixStat.RangePercent,
            WeaponAffixStat.CritChancePercent,
            WeaponAffixStat.CritDamagePercent
        };

        public static WeaponInstanceData CreatePrototypeRifle(WeaponRarity rarity, int seed)
        {
            var random = new System.Random(seed);
            var affixCount = GetAffixCount(rarity);
            var affixes = new List<WeaponAffixRollData>(affixCount);
            var available = new List<WeaponAffixStat>(AffixPool);

            for (var i = 0; i < affixCount && available.Count > 0; i++)
            {
                var statIndex = random.Next(0, available.Count);
                var stat = available[statIndex];
                available.RemoveAt(statIndex);

                affixes.Add(new WeaponAffixRollData
                {
                    stat = stat,
                    value = RollValue(random, stat, rarity)
                });
            }

            return new WeaponInstanceData
            {
                instanceId = Guid.NewGuid().ToString("N"),
                definitionId = "dr7-rifle",
                displayNameSnapshot = GetDisplayName(rarity),
                rarity = rarity,
                itemPower = 100 + (int)rarity * 18 + random.Next(0, 13),
                affixes = affixes
            };
        }

        public static WeaponRarity RollRarity(float normalizedDepth, int seed)
        {
            var random = new System.Random(seed);
            var depth = Mathf.Clamp01(normalizedDepth);
            var roll = random.NextDouble();

            var legendary = 0.01 + depth * 0.045;
            var epic = 0.05 + depth * 0.10;
            var rare = 0.17 + depth * 0.18;
            var uncommon = 0.34;

            if (roll < legendary)
                return WeaponRarity.Legendary;
            if (roll < legendary + epic)
                return WeaponRarity.Epic;
            if (roll < legendary + epic + rare)
                return WeaponRarity.Rare;
            if (roll < legendary + epic + rare + uncommon)
                return WeaponRarity.Uncommon;
            return WeaponRarity.Common;
        }

        private static int GetAffixCount(WeaponRarity rarity)
        {
            return rarity switch
            {
                WeaponRarity.Common => 0,
                WeaponRarity.Uncommon => 1,
                WeaponRarity.Rare => 2,
                WeaponRarity.Epic => 3,
                WeaponRarity.Legendary => 4,
                _ => 0
            };
        }

        private static float RollValue(System.Random random, WeaponAffixStat stat, WeaponRarity rarity)
        {
            var rarityScale = 1f + (int)rarity * 0.22f;
            var t = (float)random.NextDouble();

            return stat switch
            {
                WeaponAffixStat.DamagePercent => Mathf.Lerp(4f, 12f, t) * rarityScale,
                WeaponAffixStat.FireRatePercent => Mathf.Lerp(3f, 9f, t) * rarityScale,
                WeaponAffixStat.RangePercent => Mathf.Lerp(5f, 14f, t) * rarityScale,
                WeaponAffixStat.CritChancePercent => Mathf.Lerp(1.5f, 5f, t) * rarityScale,
                WeaponAffixStat.CritDamagePercent => Mathf.Lerp(8f, 22f, t) * rarityScale,
                _ => 0f
            };
        }

        private static string GetDisplayName(WeaponRarity rarity)
        {
            return rarity switch
            {
                WeaponRarity.Common => "DR-7 Field Rifle",
                WeaponRarity.Uncommon => "DR-7 Tuned Rifle",
                WeaponRarity.Rare => "DR-7 Vanguard Rifle",
                WeaponRarity.Epic => "DR-7 Blackline Rifle",
                WeaponRarity.Legendary => "DR-7 DEADREACH Rifle",
                _ => "DR-7 Rifle"
            };
        }
    }
}
