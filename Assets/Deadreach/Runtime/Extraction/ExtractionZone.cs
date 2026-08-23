using Kamilunavo.Deadreach.Core;
using Kamilunavo.Deadreach.Inventory;
using Kamilunavo.Deadreach.Missions;
using Kamilunavo.Deadreach.Player;
using Kamilunavo.Deadreach.Progression;
using UnityEngine;

namespace Kamilunavo.Deadreach.Extraction
{
    [RequireComponent(typeof(Collider))]
    public sealed class ExtractionZone : MonoBehaviour
    {
        [SerializeField, Min(0.25f)] private float extractionDuration = 2.5f;
        [SerializeField] private bool requireLoot = true;

        private PlayerMotor _occupant;
        private float _elapsed;

        private void Awake()
        {
            // Extraction presentation is never allowed to become physical world geometry. Keep every
            // collider attached below this zone as a trigger so runtime/editor dressing cannot trap a
            // CharacterController inside a sealed extraction area.
            EnsureTriggerOnlyColliders();
        }

        private void OnTransformChildrenChanged()
        {
            // Presentation passes may add children after Awake. Re-assert trigger-only collision
            // without touching the player's CharacterController or any external world geometry.
            EnsureTriggerOnlyColliders();
        }

        private void EnsureTriggerOnlyColliders()
        {
            foreach (var collider in GetComponentsInChildren<Collider>(true))
            {
                if (collider != null)
                    collider.isTrigger = true;
            }
        }

        private void Update()
        {
            var session = RunSession.Current;
            if (_occupant == null || session == null || session.IsCompleted || session.IsFailed)
                return;

            var blockedByNoLoot = requireLoot && !HasAnyLoot(session);
            var blockedByBoss = IsBossGateBlocked();
            var blockedByMission = IsMissionGateBlocked();
            session.SetExtractionPresence(true, blockedByNoLoot, blockedByBoss, blockedByMission);

            if (blockedByNoLoot || blockedByBoss || blockedByMission)
            {
                _elapsed = 0f;
                session.SetExtractionProgress(0f);
                return;
            }

            _elapsed += Time.deltaTime;
            var progress = Mathf.Clamp01(_elapsed / extractionDuration);
            session.SetExtractionProgress(progress);

            if (progress >= 1f)
                session.CompleteExtraction();
        }

        private void OnTriggerEnter(Collider other)
        {
            var player = other.GetComponentInParent<PlayerMotor>();
            if (player == null)
                return;

            _occupant = player;
            var session = RunSession.Current;
            if (session != null)
                session.SetExtractionPresence(true, requireLoot && !HasAnyLoot(session), IsBossGateBlocked(), IsMissionGateBlocked());
        }

        private void OnTriggerExit(Collider other)
        {
            var player = other.GetComponentInParent<PlayerMotor>();
            if (player == null || player != _occupant)
                return;

            _occupant = null;
            _elapsed = 0f;
            RunSession.Current?.SetExtractionPresence(false, false, false, false);
        }

        private void OnDisable()
        {
            if (_occupant != null)
                RunSession.Current?.SetExtractionPresence(false, false, false, false);
        }

        private static bool IsBossGateBlocked()
        {
            var director = RunDifficultyDirector.Current;
            return director != null && director.IsBossLevel && !director.BossGateCleared;
        }

        private static bool IsMissionGateBlocked()
        {
            var director = ExpeditionDirector.Current;
            return director != null && !director.PrimaryComplete;
        }

        private static bool HasAnyLoot(RunSession session)
        {
            if (session.CarriedScrap > 0 || session.PendingBossReward != null || session.PendingMissionRewards.Count > 0)
                return true;

            return RunInventory.Current != null && RunInventory.Current.Weapons.Count > 0;
        }
    }
}
