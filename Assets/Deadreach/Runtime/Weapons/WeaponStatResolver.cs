using UnityEngine;

namespace Kamilunavo.Deadreach.Weapons
{
    public readonly struct WeaponRuntimeStats
    {
        public WeaponRuntimeStats(float damage, float roundsPerSecond, float range, float critChance, float critMultiplier)
        {
            Damage = Mathf.Max(0.1f, damage);
            RoundsPerSecond = Mathf.Max(0.1f, roundsPerSecond);
            Range = Mathf.Max(1f, range);
            CritChance = Mathf.Clamp01(critChance);
            CritMultiplier = Mathf.Max(1f, critMultiplier);
        }

        public float Damage { get; }
        public float RoundsPerSecond { get; }
        public float Range { get; }
        public float CritChance { get; }
        public float CritMultiplier { get; }
    }

    public static class WeaponStatResolver
    {
        public static WeaponRuntimeStats Resolve(float baseDamage, float baseRoundsPerSecond, float baseRange, WeaponInstanceData instance)
        {
            var damageMultiplier = 1f;
            var fireRateMultiplier = 1f;
            var rangeMultiplier = 1f;
            var critChance = 0.05f;
            var critMultiplier = 1.5f;

            if (instance != null)
            {
                ApplyFamilyProfile(instance.family, ref damageMultiplier, ref fireRateMultiplier, ref rangeMultiplier, ref critChance, ref critMultiplier);
                ApplyItemPower(instance, ref damageMultiplier, ref rangeMultiplier, ref critChance);
            }

            if (instance?.affixes != null)
            {
                foreach (var affix in instance.affixes)
                {
                    if (affix == null)
                        continue;

                    switch (affix.stat)
                    {
                        case WeaponAffixStat.DamagePercent:
                            damageMultiplier += affix.value / 100f;
                            break;
                        case WeaponAffixStat.FireRatePercent:
                            fireRateMultiplier += affix.value / 100f;
                            break;
                        case WeaponAffixStat.RangePercent:
                            rangeMultiplier += affix.value / 100f;
                            break;
                        case WeaponAffixStat.CritChancePercent:
                            critChance += affix.value / 100f;
                            break;
                        case WeaponAffixStat.CritDamagePercent:
                            critMultiplier += affix.value / 100f;
                            break;
                    }
                }
            }

            return new WeaponRuntimeStats(
                baseDamage * damageMultiplier,
                baseRoundsPerSecond * fireRateMultiplier,
                baseRange * rangeMultiplier,
                critChance,
                critMultiplier);
        }

        public static float GetItemPowerDamageMultiplier(WeaponInstanceData instance)
        {
            if (instance == null)
                return 1f;

            return Mathf.Clamp(1f + (instance.itemPower - 100) * 0.0024f, 0.86f, 1.65f);
        }

        private static void ApplyItemPower(
            WeaponInstanceData instance,
            ref float damageMultiplier,
            ref float rangeMultiplier,
            ref float critChance)
        {
            // Production 0.8 turns Item Power from presentation-only metadata into real progression.
            // 100 power is the field baseline. The cap keeps old/high-tier saves from exploding balance.
            damageMultiplier *= GetItemPowerDamageMultiplier(instance);

            // Calibration is intentionally secondary to the weapon's rolled affixes/family identity.
            // Each workshop rank adds a small handling/precision gain in addition to its Item Power bump.
            var calibration = Mathf.Clamp(instance.upgradeLevel, 0, 10);
            rangeMultiplier *= 1f + calibration * 0.006f;
            critChance += calibration * 0.0025f;
        }

        private static void ApplyFamilyProfile(
            WeaponFamily family,
            ref float damageMultiplier,
            ref float fireRateMultiplier,
            ref float rangeMultiplier,
            ref float critChance,
            ref float critMultiplier)
        {
            switch (family)
            {
                case WeaponFamily.Smg:
                    damageMultiplier *= 0.72f;
                    fireRateMultiplier *= 1.62f;
                    rangeMultiplier *= 0.72f;
                    critChance += 0.015f;
                    break;
                case WeaponFamily.Pistol:
                    damageMultiplier *= 0.86f;
                    fireRateMultiplier *= 0.82f;
                    rangeMultiplier *= 0.78f;
                    critChance += 0.055f;
                    critMultiplier += 0.2f;
                    break;
                case WeaponFamily.Shotgun:
                    // The current combat layer remains hitscan-single-ray; the shotgun profile
                    // therefore represents a heavy slug until pellet spread is introduced later.
                    damageMultiplier *= 1.72f;
                    fireRateMultiplier *= 0.42f;
                    rangeMultiplier *= 0.46f;
                    critMultiplier += 0.12f;
                    break;
                default:
                    damageMultiplier *= 1.0f;
                    fireRateMultiplier *= 1.0f;
                    rangeMultiplier *= 1.0f;
                    break;
            }
        }
    }
}
