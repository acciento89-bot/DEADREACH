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
            return CreateFieldWeapon(rarity, seed, WeaponFamily.Rifle);
        }

        public static WeaponInstanceData CreateFieldWeapon(WeaponRarity rarity, int seed)
        {
            var random = new System.Random(seed ^ 0x41D3);
            var family = (WeaponFamily)random.Next(0, 4);
            return CreateFieldWeapon(rarity, seed, family);
        }

        public static WeaponInstanceData CreateFieldWeapon(WeaponRarity rarity, int seed, WeaponFamily family)
        {
            var random = new System.Random(seed);
            var affixCount = GetAffixCount(rarity);
            var affixes = RollAffixes(random, rarity, affixCount);

            return new WeaponInstanceData
            {
                instanceId = Guid.NewGuid().ToString("N"),
                definitionId = GetDefinitionId(family),
                displayNameSnapshot = GetDisplayName(family, rarity),
                visualSkinId = WeaponVisualStyle.RollStandardFinishId(seed ^ 0x5A17),
                family = family,
                rarity = rarity,
                itemPower = GetBaseItemPower(family) + (int)rarity * 18 + random.Next(0, 13),
                affixes = affixes
            };
        }

        public static WeaponInstanceData CreateBossReward(int level, int seed)
        {
            var tier = Mathf.Clamp(Mathf.Max(1, level / 10), 1, 5);
            var rarity = tier >= 3 ? WeaponRarity.Legendary : WeaponRarity.Epic;
            var random = new System.Random(seed ^ (tier * 7919));
            var affixCount = Mathf.Min(4, GetAffixCount(rarity));
            var family = tier switch
            {
                1 => WeaponFamily.Smg,
                2 => WeaponFamily.Shotgun,
                3 => WeaponFamily.Rifle,
                4 => WeaponFamily.Pistol,
                _ => WeaponFamily.Rifle
            };

            var reward = new WeaponInstanceData
            {
                instanceId = Guid.NewGuid().ToString("N"),
                definitionId = $"mutation-{family.ToString().ToLowerInvariant()}",
                displayNameSnapshot = $"MUTATION T{tier} // {GetBossFamilyName(family)} RELIC",
                visualSkinId = $"mutation-{tier}",
                family = family,
                rarity = rarity,
                itemPower = 170 + tier * 28 + random.Next(0, 18),
                affixes = RollAffixes(random, rarity, affixCount)
            };

            if (!reward.affixes.Exists(item => item != null && item.stat == WeaponAffixStat.DamagePercent))
            {
                var damageAffix = new WeaponAffixRollData
                {
                    stat = WeaponAffixStat.DamagePercent,
                    value = 12f + tier * 3.5f
                };

                if (reward.affixes.Count >= affixCount && reward.affixes.Count > 0)
                    reward.affixes[0] = damageAffix;
                else
                    reward.affixes.Add(damageAffix);
            }

            return reward;
        }

        public static WeaponRarity RollRarity(float normalizedDepth, int seed)
        {
            var random = new System.Random(seed);
            var depth = Mathf.Clamp01(normalizedDepth);
            var roll = random.NextDouble();

            var legendary = 0.01 + depth * 0.045;
            var epic = 0.05 + depth * 0.10;
            var rare = 0.17 + depth * 0.18;
            const double uncommon = 0.34;

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

        private static List<WeaponAffixRollData> RollAffixes(System.Random random, WeaponRarity rarity, int affixCount)
        {
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

            return affixes;
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

        private static string GetDefinitionId(WeaponFamily family)
        {
            return family switch
            {
                WeaponFamily.Smg => "rv9-smg",
                WeaponFamily.Pistol => "px4-pistol",
                WeaponFamily.Shotgun => "sg12-shotgun",
                _ => "dr7-rifle"
            };
        }

        private static int GetBaseItemPower(WeaponFamily family)
        {
            return family switch
            {
                WeaponFamily.Pistol => 96,
                WeaponFamily.Smg => 100,
                WeaponFamily.Shotgun => 104,
                _ => 102
            };
        }

        private static string GetDisplayName(WeaponFamily family, WeaponRarity rarity)
        {
            var prefix = family switch
            {
                WeaponFamily.Smg => "RV-9",
                WeaponFamily.Pistol => "PX-4",
                WeaponFamily.Shotgun => "SG-12",
                _ => "DR-7"
            };

            var suffix = rarity switch
            {
                WeaponRarity.Common => "Field Issue",
                WeaponRarity.Uncommon => "Tuned",
                WeaponRarity.Rare => "Vanguard",
                WeaponRarity.Epic => "Blackline",
                WeaponRarity.Legendary => "DEADREACH",
                _ => "Field Issue"
            };

            var type = family switch
            {
                WeaponFamily.Smg => "SMG",
                WeaponFamily.Pistol => "Sidearm",
                WeaponFamily.Shotgun => "Shotgun",
                _ => "Rifle"
            };

            return $"{prefix} {suffix} {type}";
        }

        private static string GetBossFamilyName(WeaponFamily family)
        {
            return family switch
            {
                WeaponFamily.Smg => "RV-9",
                WeaponFamily.Pistol => "PX-4",
                WeaponFamily.Shotgun => "SG-12",
                _ => "DR-7"
            };
        }
    }
}
