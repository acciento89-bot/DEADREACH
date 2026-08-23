using Kamilunavo.Deadreach.Audio;
using UnityEngine;

namespace Kamilunavo.Deadreach.Weapons
{
    public enum WeaponRarity
    {
        Common = 0,
        Uncommon = 1,
        Rare = 2,
        Epic = 3,
        Legendary = 4
    }

    public enum WeaponArchetype
    {
        AssaultRifle = 0,
        Smg = 1,
        Shotgun = 2,
        Marksman = 3,
        Sidearm = 4,
        Energy = 5
    }

    [CreateAssetMenu(menuName = "DEADREACH/Weapons/Weapon Definition", fileName = "Weapon_")]
    public sealed class WeaponDefinition : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField] private string weaponId = "prototype-rifle";
        [SerializeField] private string displayName = "DR-7 Rifle";
        [SerializeField] private WeaponArchetype archetype = WeaponArchetype.AssaultRifle;
        [SerializeField] private WeaponRarity rarity = WeaponRarity.Common;

        [Header("Combat")]
        [SerializeField, Min(0.1f)] private float damage = 24f;
        [SerializeField, Min(0.1f)] private float roundsPerSecond = 7.5f;
        [SerializeField, Min(1f)] private float range = 40f;
        [SerializeField, Min(0f)] private float aimTurnSpeed = 24f;

        [Header("Feel")]
        [SerializeField, Range(0f, 1f)] private float hapticStrength = 0.2f;
        [SerializeField, Min(0f)] private float tracerDuration = 0.065f;
        [SerializeField, Min(0.005f)] private float tracerWidth = 0.035f;
        [SerializeField] private AudioCue shotAudio;
        [SerializeField] private AudioCue impactAudio;

        public string WeaponId => weaponId;
        public string DisplayName => displayName;
        public WeaponArchetype Archetype => archetype;
        public WeaponRarity Rarity => rarity;
        public float Damage => damage;
        public float RoundsPerSecond => roundsPerSecond;
        public float Range => range;
        public float AimTurnSpeed => aimTurnSpeed;
        public float HapticStrength => hapticStrength;
        public float TracerDuration => tracerDuration;
        public float TracerWidth => tracerWidth;
        public AudioCue ShotAudio => shotAudio;
        public AudioCue ImpactAudio => impactAudio;
    }
}
