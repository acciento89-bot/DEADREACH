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
    }
}
