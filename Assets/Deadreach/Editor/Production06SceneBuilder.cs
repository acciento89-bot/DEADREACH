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

        private static void BuildInternal(string version, string featureSummary)
        {
            if (!Production06WeaponArtSetup.EnsureReady())
            {
                Debug.LogError($"DEADREACH Production Slice {version} aborted: weapon-family production art is not ready.");
                return;
            }

            // Reuse the fully real-Unity-validated 0.5 scene generation pipeline. 0.6/0.7 presentation,
            // responsive layout and sector/reward polish bootstrap at runtime so we do not duplicate
            // or destabilize the accepted scene authoring path.
            BunkerHubSceneBuilder.BuildCompleteSlice();
            Debug.Log($"DEADREACH Production Slice {version} generated: 0.5 validated baseline + {featureSummary}.");
        }
    }
}
