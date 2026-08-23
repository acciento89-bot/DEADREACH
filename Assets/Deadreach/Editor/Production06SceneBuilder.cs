using UnityEditor;
using UnityEngine;

namespace Kamilunavo.Deadreach.Editor
{
    public static class Production06SceneBuilder
    {
        [MenuItem("DEADREACH/Build Production Slice 0.6", priority = 0)]
        public static void Build()
        {
            if (!Production06WeaponArtSetup.EnsureReady())
            {
                Debug.LogError("DEADREACH Production Slice 0.6 aborted: weapon-family production art is not ready.");
                return;
            }

            // Reuse the fully real-Unity-validated 0.5 scene generation pipeline. 0.6 presentation,
            // mobile responsive layout, sector identity and reward UI bootstrap at runtime so we do
            // not duplicate or destabilize the accepted scene authoring path.
            BunkerHubSceneBuilder.BuildCompleteSlice();
            Debug.Log("DEADREACH Production Slice 0.6 generated: 0.5 validated baseline + weapon families + reward presentation + sector/boss identity + mobile responsive bootstrap.");
        }
    }
}
