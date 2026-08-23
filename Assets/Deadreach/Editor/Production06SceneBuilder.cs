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
            BuildInternal("0.9", "0.8 validated progression + distinct infected combat roles + active operator abilities + mobile ability input");
        }

        private static void BuildInternal(string version, string featureSummary)
        {
            if (!Production06WeaponArtSetup.EnsureReady())
            {
                Debug.LogError($"DEADREACH Production Slice {version} aborted: weapon-family production art is not ready.");
                return;
            }

            // Reuse the fully real-Unity-validated scene generation pipeline. Newer progression,
            // presentation and responsive systems bootstrap at runtime so we do not duplicate or
            // destabilize the accepted scene authoring path.
            BunkerHubSceneBuilder.BuildCompleteSlice();
            Debug.Log($"DEADREACH Production Slice {version} generated: 0.5 validated baseline + {featureSummary}.");
        }
    }
}
