using System.Collections;
using System.Linq;
using Kamilunavo.Deadreach.AI;
using Kamilunavo.Deadreach.Player;
using Kamilunavo.Deadreach.Progression;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Kamilunavo.Deadreach.Presentation
{
    public static class Production09CombatDepthBootstrap
    {
        private static bool _subscribed;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Install()
        {
            if (!_subscribed)
            {
                SceneManager.sceneLoaded += HandleSceneLoaded;
                _subscribed = true;
            }

            ScheduleBinding();
        }

        private static void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            ScheduleBinding();
        }

        private static void ScheduleBinding()
        {
            var host = new GameObject("Systems_Production09CombatDepthBootstrap");
            Object.DontDestroyOnLoad(host);
            host.AddComponent<Production09CombatDepthDeferredBinder>();
        }

        internal static void BindNow()
        {
            BindEnemyRoles();
            BindOperatorAbility();
        }

        private static void BindEnemyRoles()
        {
            var infected = Object.FindObjectsByType<InfectedChaser>(FindObjectsSortMode.None)
                .OrderBy(item => item.name)
                .ToArray();

            if (infected.Length == 0)
                return;

            var level = RunDifficultyDirector.Current != null
                ? Mathf.Max(1, RunDifficultyDirector.Current.Level)
                : 1;
            var damageScale = 1f + (level - 1) * 0.045f;

            foreach (var enemy in infected)
            {
                if (enemy == null || enemy.name.StartsWith("BOSS_"))
                    continue;

                var role = ResolveRole(enemy.name);
                var brain = enemy.GetComponent<InfectedCombatRoleBrain>();
                if (brain == null)
                    brain = enemy.gameObject.AddComponent<InfectedCombatRoleBrain>();

                brain.Configure(role, GetSpecialDamage(role) * damageScale);
            }

            Debug.Log($"DEADREACH 0.9 combat roles bound // LEVEL {level:00} // infected {infected.Length}.");
        }

        private static void BindOperatorAbility()
        {
            var player = Object.FindFirstObjectByType<PlayerMotor>();
            if (player == null)
                return;

            if (player.GetComponent<OperatorAbilityController>() == null)
                player.gameObject.AddComponent<OperatorAbilityController>();
        }

        private static InfectedCombatRole ResolveRole(string enemyName)
        {
            if (enemyName.Contains("Runner")) return InfectedCombatRole.Runner;
            if (enemyName.Contains("Brute")) return InfectedCombatRole.Brute;
            if (enemyName.Contains("Stalker")) return InfectedCombatRole.Stalker;
            return InfectedCombatRole.Walker;
        }

        private static float GetSpecialDamage(InfectedCombatRole role)
        {
            return role switch
            {
                InfectedCombatRole.Runner => 13f,
                InfectedCombatRole.Brute => 23f,
                InfectedCombatRole.Stalker => 10f,
                _ => 0f
            };
        }
    }

    public sealed class Production09CombatDepthDeferredBinder : MonoBehaviour
    {
        private IEnumerator Start()
        {
            // RunDifficultyDirector and OperatorRuntimeApplier both configure themselves in Start.
            // Waiting one frame lets 0.9 bind to their final names/profile state without replacing
            // the validated 0.8 scene-authoring path.
            yield return null;
            Production09CombatDepthBootstrap.BindNow();
            Destroy(gameObject);
        }
    }
}
