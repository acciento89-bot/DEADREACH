using System;
using System.Collections.Generic;
using Kamilunavo.Deadreach.Combat;
using Kamilunavo.Deadreach.Persistence;
using Kamilunavo.Deadreach.Player;
using UnityEngine;

namespace Kamilunavo.Deadreach.Progression
{
    public readonly struct OperatorDefinition
    {
        public OperatorDefinition(
            string id,
            string name,
            string role,
            string description,
            float healthMultiplier,
            float moveMultiplier,
            float damageMultiplier,
            Color accent,
            string abilityName,
            string abilityDescription,
            float abilityCooldown)
        {
            Id = id;
            Name = name;
            Role = role;
            Description = description;
            HealthMultiplier = healthMultiplier;
            MoveMultiplier = moveMultiplier;
            DamageMultiplier = damageMultiplier;
            Accent = accent;
            AbilityName = abilityName;
            AbilityDescription = abilityDescription;
            AbilityCooldown = abilityCooldown;
        }

        public string Id { get; }
        public string Name { get; }
        public string Role { get; }
        public string Description { get; }
        public float HealthMultiplier { get; }
        public float MoveMultiplier { get; }
        public float DamageMultiplier { get; }
        public Color Accent { get; }
        public string AbilityName { get; }
        public string AbilityDescription { get; }
        public float AbilityCooldown { get; }
    }

    public static class OperatorCatalog
    {
        private static readonly OperatorDefinition[] Definitions =
        {
            new(
                "ranger",
                "SAM",
                "RANGER",
                "Balanced field operator. Active // FIELD PATCH restores 32% max health.",
                1f,
                1f,
                1f,
                new Color(0.92f, 0.72f, 0.28f),
                "FIELD PATCH",
                "Restore 32% of maximum health. Cannot be wasted at full health.",
                18f),
            new(
                "scout",
                "RAVEN",
                "SCOUT",
                "Fast recon specialist. Active // VECTOR DASH instantly repositions through danger.",
                0.88f,
                1.14f,
                0.96f,
                new Color(0.22f, 0.82f, 0.72f),
                "VECTOR DASH",
                "Dash 4.6m in current movement direction or facing direction.",
                7.5f),
            new(
                "warden",
                "BRIGGS",
                "WARDEN",
                "Heavy survivor. Active // SHOCKWAVE punishes infected that collapse the perimeter.",
                1.22f,
                0.9f,
                1.08f,
                new Color(0.86f, 0.32f, 0.2f),
                "SHOCKWAVE",
                "Damage every infected within 4.6m around Briggs.",
                12f)
        };

        public static IReadOnlyList<OperatorDefinition> All => Definitions;

        public static OperatorDefinition Get(string id)
        {
            foreach (var definition in Definitions)
            {
                if (string.Equals(definition.Id, id, StringComparison.OrdinalIgnoreCase))
                    return definition;
            }

            return Definitions[0];
        }
    }

    [DisallowMultipleComponent]
    public sealed class OperatorRuntimeApplier : MonoBehaviour
    {
        private void Start()
        {
            var definition = OperatorCatalog.Get(SaveService.Data.selectedCharacterId);

            var health = GetComponent<Damageable>();
            if (health != null)
            {
                var bunkerHealthMultiplier = SaveService.GetPlayerHealthMultiplier();
                health.Configure(CombatFaction.Survivor, 100f * definition.HealthMultiplier * bunkerHealthMultiplier);
            }

            GetComponent<PlayerMotor>()?.SetMoveSpeedMultiplier(definition.MoveMultiplier);
            GetComponent<HitscanWeapon>()?.SetOperatorDamageMultiplier(definition.DamageMultiplier);

            // The selected profile maps to a distinct production character prefab through
            // ProductionVisualBinder. Do not recolor the body here; preserve the authored model.
            Debug.Log($"DEADREACH operator active: {definition.Name} // {definition.Role} // ability {definition.AbilityName} // Medbay {SaveService.Data.medbayLevel}.");
        }
    }
}
