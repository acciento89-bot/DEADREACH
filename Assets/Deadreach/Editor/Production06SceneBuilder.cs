using UnityEditor;
using UnityEngine;

namespace Kamilunavo.Deadreach.Editor
{
    public static class Production06SceneBuilder
    {
        [MenuItem("DEADREACH/Build Production Slice 0.6", priority = 0)]
        public static void Build()
        {
            BuildInternal("0.6", "weapon families + reward presentation + sector/boss identity + mobile responsive bootstrap");
        }

        [MenuItem("DEADREACH/Build Production Slice 0.7", priority = 1)]
        public static void Build07()
        {
            BuildInternal("0.7", "0.6 functional content + responsive layout polish + corrected Arsenal orientation + sector FX material pass");
        }

        [MenuItem("DEADREACH/Build Production Slice 0.8", priority = 2)]
        public static void Build08()
        {
            BuildInternal("0.8", "0.7 validated presentation + schema-v6 workshop economy + real Item Power + permanent Bunker progression");
        }

        [MenuItem("DEADREACH/Build Production Slice 0.9", priority = 3)]
        public static void Build09()
        {
            BuildInternal("0.9", "0.8 validated progression + distinct infected combat roles + active operator abilities + fixed-zone mobile controls");
        }

        [MenuItem("DEADREACH/Build Production Slice 0.10", priority = 4)]
        public static void Build10()
        {
            BuildInternal("0.10", "0.9 validated combat depth + operator ability VFX + infected special impact FX + camera/critical feedback polish");
        }

        [MenuItem("DEADREACH/Build Production Slice 0.11", priority = 5)]
        public static void Build11()
        {
            BuildInternal("0.11", "0.10 validated combat impact + expedition director + mission variety + objective-gated extraction + optional risk/reward caches + reinforcement waves");
        }

        [MenuItem("DEADREACH/Build Production Slice 0.12", priority = 6)]
        public static void Build12()
        {
            if (!BuildInternal("0.12", "0.11 validated expedition director + multi-sector world expansion + route variants + hazards + sector-aware mission/reinforcement anchors"))
                return;

            if (!ApplyStable012World())
                return;

            Debug.Log("DEADREACH Production Slice 0.12 generated: four playable sector layouts + expanded side routes + sector hazards + dynamic geography + decluttered Transit / Industrial / Blackout objective arenas.");
        }

        [MenuItem("DEADREACH/Build Production Slice 0.13", priority = 7)]
        public static void Build13()
        {
            if (!BuildInternal("0.13", "0.12 validated sector expansion + premium tactical command-center UI + cinematic bunker presentation + full menu chrome overhaul"))
                return;

            if (!ApplyStable012World())
                return;

            if (!Production13BunkerScenePass.Apply())
            {
                Debug.LogError("DEADREACH Production Slice 0.13 aborted: premium Bunker scene pass failed after stable 0.12 world generation.");
                return;
            }

            Debug.Log("DEADREACH Production Slice 0.13 generated: stable 0.12 expedition world + cinematic premium Bunker command center + tactical animated menu design system.");
        }

        private static bool ApplyStable012World()
        {
            if (!Production12SectorScenePass.Apply())
            {
                Debug.LogError("DEADREACH Production world generation aborted: Production 0.12 sector world pass failed after base scene generation.");
                return false;
            }

            if (!Production12LayoutPolishPass.Apply())
            {
                Debug.LogError("DEADREACH Production world generation aborted: accepted Production 0.12 layout polish failed after sector generation.");
                return false;
            }

            return true;
        }

        private static bool BuildInternal(string version, string featureSummary)
        {
            if (!Production06WeaponArtSetup.EnsureReady())
            {
                Debug.LogError($"DEADREACH Production Slice {version} aborted: weapon-family production art is not ready.");
                return false;
            }

            // Reuse the fully real-Unity-validated scene generation pipeline. Newer progression,
            // presentation and responsive systems bootstrap at runtime so we do not duplicate or
            // destabilize the accepted scene authoring path.
            BunkerHubSceneBuilder.BuildCompleteSlice();
            Debug.Log($"DEADREACH Production Slice {version} generated: 0.5 validated baseline + {featureSummary}.");
            return true;
        }
    }
}
