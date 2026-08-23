using System;
using UnityEngine;

namespace Kamilunavo.Deadreach.World
{
    public enum SectorArchetype
    {
        QuarantineWard = 0,
        TransitCollapse = 1,
        IndustrialSpill = 2,
        BlackoutPlaza = 3
    }

    public enum SectorHazardKind
    {
        Contamination = 0,
        ElectricalArc = 1,
        Fireline = 2
    }

    [DisallowMultipleComponent]
    public sealed class SectorLayoutDefinition : MonoBehaviour
    {
        [SerializeField] private SectorArchetype archetype;
        [SerializeField] private string displayName = "QUARANTINE WARD";
        [SerializeField] private string hazardProfile = "BIOHAZARD";
        [SerializeField, Min(0)] private int riskBonusScrap = 4;
        [SerializeField, Min(0)] private int rewardPowerBonus = 2;
        [SerializeField] private Color fogColor = new(0.045f, 0.09f, 0.075f);
        [SerializeField, Min(0f)] private float fogDensity = 0.015f;
        [SerializeField] private Color keyColor = new(0.35f, 0.82f, 0.68f);
        [SerializeField, Min(0f)] private float keyIntensity = 0.72f;
        [SerializeField] private Transform playerSpawn;
        [SerializeField] private Transform extractionAnchor;
        [SerializeField] private Transform[] objectiveAnchors = Array.Empty<Transform>();
        [SerializeField] private Transform[] reinforcementAnchors = Array.Empty<Transform>();
        [SerializeField] private Transform[] enemyAnchors = Array.Empty<Transform>();
        [SerializeField] private Transform[] lootAnchors = Array.Empty<Transform>();
        [SerializeField] private SectorHazardZone[] hazards = Array.Empty<SectorHazardZone>();

        public SectorArchetype Archetype => archetype;
        public string DisplayName => string.IsNullOrWhiteSpace(displayName) ? archetype.ToString().ToUpperInvariant() : displayName;
        public string HazardProfile => string.IsNullOrWhiteSpace(hazardProfile) ? "CLEAR" : hazardProfile;
        public int RiskBonusScrap => Mathf.Max(0, riskBonusScrap);
        public int RewardPowerBonus => Mathf.Max(0, rewardPowerBonus);
        public Color FogColor => fogColor;
        public float FogDensity => Mathf.Max(0f, fogDensity);
        public Color KeyColor => keyColor;
        public float KeyIntensity => Mathf.Max(0f, keyIntensity);
        public Vector3 PlayerSpawn => playerSpawn != null ? playerSpawn.position : new Vector3(0f, 1f, -8.5f);
        public Vector3 ExtractionAnchor => extractionAnchor != null ? extractionAnchor.position : new Vector3(0f, 0.08f, 20f);
        public SectorHazardZone[] Hazards => hazards ?? Array.Empty<SectorHazardZone>();

        public void Configure(
            SectorArchetype newArchetype,
            string newDisplayName,
            string newHazardProfile,
            int newRiskBonusScrap,
            int newRewardPowerBonus,
            Color newFogColor,
            float newFogDensity,
            Color newKeyColor,
            float newKeyIntensity,
            Transform newPlayerSpawn,
            Transform newExtractionAnchor,
            Transform[] newObjectiveAnchors,
            Transform[] newReinforcementAnchors,
            Transform[] newEnemyAnchors,
            Transform[] newLootAnchors,
            SectorHazardZone[] newHazards)
        {
            archetype = newArchetype;
            displayName = newDisplayName;
            hazardProfile = newHazardProfile;
            riskBonusScrap = Mathf.Max(0, newRiskBonusScrap);
            rewardPowerBonus = Mathf.Max(0, newRewardPowerBonus);
            fogColor = newFogColor;
            fogDensity = Mathf.Max(0f, newFogDensity);
            keyColor = newKeyColor;
            keyIntensity = Mathf.Max(0f, newKeyIntensity);
            playerSpawn = newPlayerSpawn;
            extractionAnchor = newExtractionAnchor;
            objectiveAnchors = newObjectiveAnchors ?? Array.Empty<Transform>();
            reinforcementAnchors = newReinforcementAnchors ?? Array.Empty<Transform>();
            enemyAnchors = newEnemyAnchors ?? Array.Empty<Transform>();
            lootAnchors = newLootAnchors ?? Array.Empty<Transform>();
            hazards = newHazards ?? Array.Empty<SectorHazardZone>();
        }

        public Vector3 GetObjectivePoint(int index, Vector3 fallback)
        {
            return GetPoint(objectiveAnchors, index, fallback);
        }

        public Vector3 GetReinforcementPoint(int index, Vector3 fallback)
        {
            return GetPoint(reinforcementAnchors, index, fallback);
        }

        public Vector3 GetEnemyPoint(int index, Vector3 fallback)
        {
            return GetPoint(enemyAnchors, index, fallback);
        }

        public Vector3 GetLootPoint(int index, Vector3 fallback)
        {
            return GetPoint(lootAnchors, index, fallback);
        }

        public Vector3[] GetObjectivePoints()
        {
            return GetPoints(objectiveAnchors);
        }

        public int EnemyAnchorCount => enemyAnchors?.Length ?? 0;
        public int LootAnchorCount => lootAnchors?.Length ?? 0;
        public int ReinforcementAnchorCount => reinforcementAnchors?.Length ?? 0;

        private static Vector3 GetPoint(Transform[] anchors, int index, Vector3 fallback)
        {
            if (anchors == null || anchors.Length == 0)
                return fallback;

            var safeIndex = Mathf.Abs(index) % anchors.Length;
            var anchor = anchors[safeIndex];
            return anchor != null ? anchor.position : fallback;
        }

        private static Vector3[] GetPoints(Transform[] anchors)
        {
            if (anchors == null || anchors.Length == 0)
                return Array.Empty<Vector3>();

            var points = new Vector3[anchors.Length];
            for (var i = 0; i < anchors.Length; i++)
                points[i] = anchors[i] != null ? anchors[i].position : Vector3.zero;
            return points;
        }
    }
}
