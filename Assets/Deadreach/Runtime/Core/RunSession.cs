using System;
using System.Collections;
using Kamilunavo.Deadreach.Combat;
using Kamilunavo.Deadreach.Persistence;
using Kamilunavo.Deadreach.Player;
using UnityEngine;

namespace Kamilunavo.Deadreach.Core
{
    public sealed class RunSession : MonoBehaviour
    {
        public static RunSession Current { get; private set; }

        [SerializeField, Min(0f)] private float resultScreenDuration = 2.2f;

        public event Action<int> ScrapChanged;
        public event Action ExtractionCompleted;
        public event Action RunFailed;

        public int CarriedScrap { get; private set; }
        public float ExtractionProgress { get; private set; }
        public bool IsCompleted { get; private set; }
        public bool IsFailed { get; private set; }

        private Damageable _playerHealth;
        private Coroutine _returnRoutine;

        private void Awake()
        {
            if (Current != null && Current != this)
            {
                Destroy(gameObject);
                return;
            }

            Current = this;
        }

        private void Start()
        {
            var player = FindFirstObjectByType<PlayerMotor>();
            if (player == null)
                return;

            _playerHealth = player.GetComponent<Damageable>();
            if (_playerHealth != null)
                _playerHealth.Died += HandlePlayerDeath;
        }

        private void OnDestroy()
        {
            if (_playerHealth != null)
                _playerHealth.Died -= HandlePlayerDeath;

            if (Current == this)
                Current = null;
        }

        public void CollectScrap(int amount)
        {
            if (IsCompleted || IsFailed || amount <= 0)
                return;

            CarriedScrap += amount;
            ScrapChanged?.Invoke(CarriedScrap);
        }

        public void SetExtractionProgress(float normalized)
        {
            if (IsCompleted || IsFailed)
                return;

            ExtractionProgress = Mathf.Clamp01(normalized);
        }

        public void CompleteExtraction()
        {
            if (IsCompleted || IsFailed)
                return;

            IsCompleted = true;
            ExtractionProgress = 1f;
            SaveService.RegisterExtraction(CarriedScrap);
            CarriedScrap = 0;
            ScrapChanged?.Invoke(CarriedScrap);
            ExtractionCompleted?.Invoke();
            BeginReturnToBunker();
        }

        public void AbandonRun()
        {
            FailRun();
        }

        private void HandlePlayerDeath()
        {
            FailRun();
        }

        private void FailRun()
        {
            if (IsCompleted || IsFailed)
                return;

            IsFailed = true;
            CarriedScrap = 0;
            ExtractionProgress = 0f;
            SaveService.RegisterFailedRun();
            ScrapChanged?.Invoke(CarriedScrap);
            RunFailed?.Invoke();
            BeginReturnToBunker();
        }

        private void BeginReturnToBunker()
        {
            if (_returnRoutine == null)
                _returnRoutine = StartCoroutine(ReturnToBunkerAfterDelay());
        }

        private IEnumerator ReturnToBunkerAfterDelay()
        {
            yield return new WaitForSecondsRealtime(resultScreenDuration);
            SceneFlowService.LoadBunker();
        }
    }
}
