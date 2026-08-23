using System;
using Kamilunavo.Deadreach.Feedback;
using UnityEngine;

namespace Kamilunavo.Deadreach.Combat
{
    public enum CombatFaction
    {
        Neutral = 0,
        Survivor = 1,
        Infected = 2
    }

    public readonly struct DamageInfo
    {
        public DamageInfo(float amount, CombatFaction sourceFaction, Vector3 hitPoint, Vector3 hitDirection)
        {
            Amount = Mathf.Max(0f, amount);
            SourceFaction = sourceFaction;
            HitPoint = hitPoint;
            HitDirection = hitDirection;
        }

        public float Amount { get; }
        public CombatFaction SourceFaction { get; }
        public Vector3 HitPoint { get; }
        public Vector3 HitDirection { get; }
    }

    public sealed class Damageable : MonoBehaviour
    {
        [SerializeField] private CombatFaction faction = CombatFaction.Neutral;
        [SerializeField, Min(1f)] private float maxHealth = 100f;
        [SerializeField] private bool destroyOnDeath;

        public event Action<DamageInfo> Damaged;
        public event Action Died;

        public CombatFaction Faction => faction;
        public float MaxHealth => maxHealth;
        public float CurrentHealth { get; private set; }
        public float NormalizedHealth => maxHealth <= 0f ? 0f : CurrentHealth / maxHealth;
        public bool IsDead { get; private set; }

        private void Awake()
        {
            CurrentHealth = maxHealth;
        }

        public void Configure(CombatFaction newFaction, float newMaxHealth, bool shouldDestroyOnDeath = false)
        {
            faction = newFaction;
            maxHealth = Mathf.Max(1f, newMaxHealth);
            destroyOnDeath = shouldDestroyOnDeath;
            CurrentHealth = maxHealth;
            IsDead = false;
        }

        public bool TakeDamage(DamageInfo info)
        {
            if (IsDead || info.Amount <= 0f)
                return false;

            if (faction != CombatFaction.Neutral && info.SourceFaction == faction)
                return false;

            CurrentHealth = Mathf.Max(0f, CurrentHealth - info.Amount);
            Damaged?.Invoke(info);

            if (faction == CombatFaction.Survivor)
                CombatFeedback.RaisePlayerDamaged(maxHealth <= 0f ? 0f : info.Amount / maxHealth);

            if (CurrentHealth <= 0f)
                Die();

            return true;
        }

        public void Heal(float amount)
        {
            if (IsDead || amount <= 0f)
                return;

            CurrentHealth = Mathf.Min(maxHealth, CurrentHealth + amount);
        }

        private void Die()
        {
            if (IsDead)
                return;

            IsDead = true;

            if (faction == CombatFaction.Survivor)
                CombatFeedback.RaisePlayerDied();

            Died?.Invoke();

            if (destroyOnDeath)
                Destroy(gameObject);
        }
    }
}
