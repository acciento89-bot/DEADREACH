using System;
using System.Collections.Generic;

namespace Kamilunavo.Deadreach.Weapons
{
    public enum WeaponAffixStat
    {
        DamagePercent = 0,
        FireRatePercent = 1,
        RangePercent = 2,
        CritChancePercent = 3,
        CritDamagePercent = 4
    }

    [Serializable]
    public sealed class WeaponAffixRollData
    {
        public WeaponAffixStat stat;
        public float value;
    }

    [Serializable]
    public sealed class WeaponInstanceData
    {
        public string instanceId;
        public string definitionId;
        public string displayNameSnapshot;
        public string visualSkinId;
        public WeaponRarity rarity;
        public int itemPower;
        public List<WeaponAffixRollData> affixes = new();

        public WeaponInstanceData Clone()
        {
            var copy = new WeaponInstanceData
            {
                instanceId = instanceId,
                definitionId = definitionId,
                displayNameSnapshot = displayNameSnapshot,
                visualSkinId = visualSkinId,
                rarity = rarity,
                itemPower = itemPower,
                affixes = new List<WeaponAffixRollData>()
            };

            if (affixes != null)
            {
                foreach (var affix in affixes)
                {
                    copy.affixes.Add(new WeaponAffixRollData
                    {
                        stat = affix.stat,
                        value = affix.value
                    });
                }
            }

            return copy;
        }
    }
}
