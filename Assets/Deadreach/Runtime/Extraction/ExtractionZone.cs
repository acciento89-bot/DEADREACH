using Kamilunavo.Deadreach.Core;
using Kamilunavo.Deadreach.Inventory;
using Kamilunavo.Deadreach.Player;
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
            GetComponent<Collider>().isTrigger = true;
        }

        private void Update()
        {
            var session = RunSession.Current;
            if (_occupant == null || session == null || session.IsCompleted || session.IsFailed)
                return;

            var blocked = requireLoot && !HasAnyLoot(session);
            session.SetExtractionPresence(true, blocked);

            if (blocked)
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
                session.SetExtractionPresence(true, requireLoot && !HasAnyLoot(session));
        }

        private void OnTriggerExit(Collider other)
        {
            var player = other.GetComponentInParent<PlayerMotor>();
            if (player == null || player != _occupant)
                return;

            _occupant = null;
            _elapsed = 0f;
            RunSession.Current?.SetExtractionPresence(false, false);
        }

        private void OnDisable()
        {
            if (_occupant != null)
                RunSession.Current?.SetExtractionPresence(false, false);
        }

        private static bool HasAnyLoot(RunSession session)
        {
            if (session.CarriedScrap > 0)
                return true;

            return RunInventory.Current != null && RunInventory.Current.Weapons.Count > 0;
        }
    }
}
