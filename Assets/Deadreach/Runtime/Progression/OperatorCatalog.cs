using System;
using System.Collections;
using System.Collections.Generic;
using Kamilunavo.Deadreach.Combat;
using Kamilunavo.Deadreach.Persistence;
using Kamilunavo.Deadreach.Player;
using Kamilunavo.Deadreach.Presentation;
using UnityEngine;

namespace Kamilunavo.Deadreach.Progression
{
    public readonly struct OperatorDefinition
    {
        public OperatorDefinition(string id, string name, string role, string description, float healthMultiplier, float moveMultiplier, float damageMultiplier, Color accent)
        {
            Id = id;
            Name = name;
            Role = role;
            Description = description;
            HealthMultiplier = healthMultiplier;
            MoveMultiplier = moveMultiplier;
            DamageMultiplier = damageMultiplier;
            Accent = accent;
        }

        public string Id { get; }
        public string Name { get; }
        public string Role { get; }
        public string Description { get; }
        public float HealthMultiplier { get; }
        public float MoveMultiplier { get; }
        public float DamageMultiplier { get; }
        public Color Accent { get; }
    }

    public static class OperatorCatalog
    {
        private static readonly OperatorDefinition[] Definitions =
        {
            new("ranger", "SAM", "RANGER", "Balanced field operator. Reliable under every deployment condition.", 1f, 1f, 1f, new Color(0.92f, 0.72f, 0.28f)),
            new("scout", "RAVEN", "SCOUT", "Fast recon specialist. Trades durability for speed and positioning.", 0.88f, 1.14f, 0.96f, new Color(0.22f, 0.82f, 0.72f)),
            new("warden", "BRIGGS", "WARDEN", "Heavy survivor built for close pressure. Slower, tougher and harder hitting.", 1.22f, 0.9f, 1.08f, new Color(0.86f, 0.32f, 0.2f))
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
                health.Configure(CombatFaction.Survivor, 100f * definition.HealthMultiplier);

            GetComponent<PlayerMotor>()?.SetMoveSpeedMultiplier(definition.MoveMultiplier);
            GetComponent<HitscanWeapon>()?.SetOperatorDamageMultiplier(definition.DamageMultiplier);

            StartCoroutine(ApplyVisualTintWhenReady(definition));
        }

        private IEnumerator ApplyVisualTintWhenReady(OperatorDefinition definition)
        {
            var binder = GetComponent<ProductionVisualBinder>();
            if (binder == null)
                yield break;

            for (var frame = 0; frame < 12 && !binder.HasProductionVisual; frame++)
                yield return null;

            var visual = binder.VisualInstance;
            if (visual == null)
                yield break;

            foreach (var renderer in visual.GetComponentsInChildren<Renderer>(true))
            {
                if (renderer == null || IsWeaponRenderer(renderer.transform))
                    continue;

                var materials = renderer.materials;
                for (var i = 0; i < materials.Length; i++)
                {
                    var material = materials[i];
                    if (material == null)
                        continue;

                    var tint = Color.Lerp(Color.white, definition.Accent, 0.22f);
                    if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", tint);
                    if (material.HasProperty("_Color")) material.SetColor("_Color", tint);
                }
            }
        }

        private static bool IsWeaponRenderer(Transform transform)
        {
            var current = transform;
            for (var depth = 0; current != null && depth < 5; depth++, current = current.parent)
            {
                var name = current.name.ToLowerInvariant();
                if (name.Contains("weapon") || name.Contains("pistol") || name.Contains("rifle") || name.Contains("gun"))
                    return true;
            }

            return false;
        }
    }
}
