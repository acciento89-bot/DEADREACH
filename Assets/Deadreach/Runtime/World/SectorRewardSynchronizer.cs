using System.Collections;
using System.Linq;
using Kamilunavo.Deadreach.Core;
using Kamilunavo.Deadreach.Inventory;
using Kamilunavo.Deadreach.Weapons;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Kamilunavo.Deadreach.World
{
    /// <summary>
    /// RunInventory intentionally clones field weapons when they are inserted. SectorDirector boosts
    /// the RunSession event object so full-inventory pending rewards keep the sector bonus; this bridge
    /// mirrors that bonus onto the carried inventory clone when capacity was available.
    /// </summary>
    public sealed class SectorRewardSynchronizer : MonoBehaviour
    {
        private static SectorRewardSynchronizer _instance;
        private RunSession _boundSession;
        private Coroutine _bindRoutine;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void EnsureInstalled()
        {
            if (_instance != null)
                return;

            var root = new GameObject("Systems_SectorRewardSync_012");
            _instance = root.AddComponent<SectorRewardSynchronizer>();
            DontDestroyOnLoad(root);
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += HandleSceneLoaded;
        }

        private void Start()
        {
            ScheduleBind();
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= HandleSceneLoaded;
            Unbind();
        }

        private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            ScheduleBind();
        }

        private void ScheduleBind()
        {
            if (_bindRoutine != null)
                StopCoroutine(_bindRoutine);
            _bindRoutine = StartCoroutine(BindDelayed());
        }

        private IEnumerator BindDelayed()
        {
            yield return null;
            yield return null;

            var session = RunSession.Current;
            if (session == _boundSession)
            {
                _bindRoutine = null;
                yield break;
            }

            Unbind();
            _boundSession = session;
            if (_boundSession != null)
                _boundSession.MissionRewardGranted += HandleMissionRewardGranted;
            _bindRoutine = null;
        }

        private void HandleMissionRewardGranted(WeaponInstanceData eventReward)
        {
            if (eventReward == null)
                return;

            var sector = SectorDirector.Current;
            var inventory = RunInventory.Current;
            var bonus = sector != null ? sector.RewardPowerBonus : 0;
            if (inventory == null || bonus <= 0)
                return;

            var carriedClone = inventory.Weapons.FirstOrDefault(item =>
                item != null && item.instanceId == eventReward.instanceId);
            if (carriedClone == null)
                return;

            carriedClone.itemPower += bonus;
            Debug.Log($"DEADREACH 0.12 carried sector reward synchronized // +{bonus} Item Power.");
        }

        private void Unbind()
        {
            if (_boundSession != null)
                _boundSession.MissionRewardGranted -= HandleMissionRewardGranted;
            _boundSession = null;
        }

        private void OnDestroy()
        {
            Unbind();
            if (_instance == this)
                _instance = null;
        }
    }
}
